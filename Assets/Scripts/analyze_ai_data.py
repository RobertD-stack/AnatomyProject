from __future__ import annotations

import argparse
import json
import re
import sys
from pathlib import Path

import matplotlib.pyplot as plt
import pandas as pd

# Assets/Scripts/analyze_ai_data.py -> Assets/Data, Assets/Scripts
SCRIPTS_DIR = Path(__file__).resolve().parent
DATA_DIR = SCRIPTS_DIR.parent / "Data"
PROMPTS_JSON = SCRIPTS_DIR / "pubmedqa_test_prompts.json"

AI_DATA_PATTERN = re.compile(r"^ai_data_(\d{8}_\d{6})\.csv$")
TIMING_TOLERANCE = 0.05


def find_latest_ai_data_csv(data_dir: Path = DATA_DIR) -> Path:
    candidates: list[tuple[str, Path]] = []
    for path in data_dir.glob("ai_data_*.csv"):
        match = AI_DATA_PATTERN.match(path.name)
        if match:
            candidates.append((match.group(1), path))
    if not candidates:
        raise FileNotFoundError(f"No ai_data_*.csv files found in {data_dir}")
    candidates.sort(key=lambda item: item[0], reverse=True)
    return candidates[0][1]


def matching_results_csv(ai_data_path: Path, data_dir: Path = DATA_DIR) -> Path:
    match = AI_DATA_PATTERN.match(ai_data_path.name)
    if not match:
        raise ValueError(f"Unexpected ai_data filename: {ai_data_path.name}")
    results_path = data_dir / f"pubmedqa_results_{match.group(1)}.csv"
    if not results_path.exists():
        raise FileNotFoundError(
            f"Matching results file not found: {results_path.name}\n"
            "RunAutomaticPrompts writes pubmedqa_results with the same timestamp as ai_data."
        )
    return results_path


def load_prompt_metadata(prompts_json: Path = PROMPTS_JSON) -> pd.DataFrame:
    if not prompts_json.exists():
        return pd.DataFrame(columns=["pmid", "question", "gold"])
    payload = json.loads(prompts_json.read_text(encoding="utf-8"))
    rows = [
        {"pmid": item["pmid"], "question": item["question"], "gold": item["gold"]}
        for item in payload.get("items", [])
    ]
    return pd.DataFrame(rows)


def missing_data_by_model(results_df: pd.DataFrame, ai_df: pd.DataFrame) -> pd.DataFrame:
    """Count rows per model and gaps between pubmedqa_results and ai_data."""
    res_counts = results_df.groupby("model", as_index=False).size().rename(
        columns={"size": "pubmedqa_results_rows"}
    )
    ai_counts = ai_df.groupby("modelName", as_index=False).size().rename(
        columns={"modelName": "model", "size": "ai_data_rows"}
    )
    merged = res_counts.merge(ai_counts, on="model", how="outer").fillna(0)
    merged["pubmedqa_results_rows"] = merged["pubmedqa_results_rows"].astype(int)
    merged["ai_data_rows"] = merged["ai_data_rows"].astype(int)
    expected = int(merged["pubmedqa_results_rows"].max()) if len(merged) else 0
    merged["missing_ai_data_rows"] = (
        merged["pubmedqa_results_rows"] - merged["ai_data_rows"]
    ).clip(lower=0)
    merged["skipped_requests"] = (expected - merged["pubmedqa_results_rows"]).clip(lower=0)
    return merged.sort_values("model").reset_index(drop=True)


def _attach_results_fields(row: dict, res_row: pd.Series) -> dict:
    row["pmid"] = str(res_row["pmid"])
    row["gold"] = res_row["gold"]
    row["parsed_prediction"] = res_row["parsed_prediction"]
    row["correct"] = int(res_row["correct"])
    row["ollama_model"] = res_row["ollama_model"]
    row["model"] = res_row["model"]
    return row


def align_ai_with_results(
    ai_df: pd.DataFrame, results_df: pd.DataFrame
) -> tuple[pd.DataFrame, pd.DataFrame]:
    paired_rows: list[dict] = []
    skipped_rows: list[dict] = []
    ai_i = 0

    for _, res_row in results_df.iterrows():
        if ai_i >= len(ai_df):
            skipped_rows.append(res_row.to_dict())
            continue

        ai_row = ai_df.iloc[ai_i]
        model_match = str(res_row["model"]) == str(ai_row["modelName"])
        time_match = pd.notna(res_row["total_seconds"]) and abs(
            float(res_row["total_seconds"]) - float(ai_row["totalRequestTime"])
        ) <= TIMING_TOLERANCE

        if model_match and time_match:
            paired_rows.append(_attach_results_fields(ai_row.to_dict(), res_row))
            ai_i += 1
        else:
            skipped_rows.append(res_row.to_dict())

    return pd.DataFrame(paired_rows), pd.DataFrame(skipped_rows)


def _finish_merged_frame(merged: pd.DataFrame) -> pd.DataFrame:
    prompts = load_prompt_metadata()
    if not prompts.empty:
        merged = merged.merge(prompts, on="pmid", how="left", suffixes=("", "_json"))
        if "gold_json" in merged.columns:
            merged["gold"] = merged["gold"].fillna(merged["gold_json"])
            merged = merged.drop(columns=["gold_json"])

    merged["prompt_index"] = merged.groupby("model").cumcount() + 1
    return merged


def load_run_data(
    ai_data_path: Path, results_path: Path
) -> tuple[pd.DataFrame, pd.DataFrame]:
    ai_df = pd.read_csv(ai_data_path)
    results_df = pd.read_csv(results_path)

    timing_match = (
        len(ai_df) == len(results_df)
        and (ai_df["modelName"].values == results_df["model"].values).all()
        and (
            ai_df["totalRequestTime"].round(6) == results_df["total_seconds"].round(6)
        ).all()
    )

    if timing_match:
        merged = ai_df.copy()
        merged["pmid"] = results_df["pmid"].astype(str)
        merged["gold"] = results_df["gold"]
        merged["parsed_prediction"] = results_df["parsed_prediction"]
        merged["correct"] = results_df["correct"].astype(int)
        merged["ollama_model"] = results_df["ollama_model"]
        merged["model"] = results_df["model"]
        skipped = pd.DataFrame()
    else:
        if len(ai_df) != len(results_df):
            print(
                f"Warning: row count mismatch (ai_data={len(ai_df)}, "
                f"results={len(results_df)}); aligning by model + timing.",
                file=sys.stderr,
            )
        merged, skipped = align_ai_with_results(ai_df, results_df)

    if merged.empty:
        raise ValueError("No ai_data rows could be aligned with pubmedqa_results.")

    return _finish_merged_frame(merged), skipped


def print_section(title: str) -> None:
    print()
    print("=" * len(title))
    print(title)
    print("=" * len(title))


def summarize_by_model(df: pd.DataFrame) -> pd.DataFrame:
    return (
        df.groupby("model", as_index=False)
        .agg(
            prompts=("pmid", "count"),
            correct=("correct", "sum"),
            accuracy=("correct", "mean"),
            totalRequestTime_mean=("totalRequestTime", "mean"),
            totalRequestTime_median=("totalRequestTime", "median"),
            loadSeconds_mean=("loadSeconds", "mean"),
            loadSeconds_median=("loadSeconds", "median"),
        )
        .assign(accuracy=lambda d: (d["accuracy"] * 100).round(1))
        .sort_values("model")
    )


def per_prompt_timing_pivot(df: pd.DataFrame, metric: str) -> pd.DataFrame:
    label = df["pmid"]
    if "question" in df.columns and df["question"].notna().any():
        label = df["pmid"] + " | " + df["question"].str.slice(0, 80)
    pivot_df = df.copy()
    pivot_df["prompt_label"] = label
    return pivot_df.pivot_table(
        index="prompt_label",
        columns="model",
        values=metric,
        aggfunc="first",
    )


def per_prompt_accuracy_pivot(df: pd.DataFrame) -> pd.DataFrame:
    label = df["pmid"]
    if "question" in df.columns and df["question"].notna().any():
        label = df["pmid"] + " | " + df["question"].str.slice(0, 80)
    pivot_df = df.copy()
    pivot_df["prompt_label"] = label
    return pivot_df.pivot_table(
        index="prompt_label",
        columns="model",
        values="correct",
        aggfunc="first",
    )


def compare_models(df: pd.DataFrame) -> pd.DataFrame:
    """Wide comparison of timing and accuracy for each prompt across models."""
    index = df["pmid"]
    if "question" in df.columns and df["question"].notna().any():
        index = df["pmid"] + " | " + df["question"].str.slice(0, 80)

    rows: list[dict[str, object]] = []
    for prompt_label, group in df.assign(prompt_label=index).groupby("prompt_label"):
        row: dict[str, object] = {"prompt": prompt_label}
        if "gold" in group.columns:
            row["gold"] = group["gold"].iloc[0]
        for model, model_rows in group.groupby("model"):
            model_row = model_rows.iloc[0]
            row[f"{model}__totalRequestTime"] = model_row["totalRequestTime"]
            row[f"{model}__loadSeconds"] = model_row["loadSeconds"]
            row[f"{model}__correct"] = int(model_row["correct"])
        rows.append(row)
    return pd.DataFrame(rows)


def plot_results(
    df: pd.DataFrame,
    model_summary: pd.DataFrame,
    accuracy_pivot: pd.DataFrame,
    stamp: str,
    graph_dir: Path = DATA_DIR,
    show: bool = False,
) -> list[Path]:
    graph_dir.mkdir(parents=True, exist_ok=True)
    saved: list[Path] = []

    models = model_summary["model"].tolist()
    x = range(len(models))
    colors = plt.cm.tab10.colors

    # --- Figure 1: model summary ---
    fig, axes = plt.subplots(2, 2, figsize=(12, 9))
    fig.suptitle(f"AI benchmark summary ({stamp})", fontsize=14, fontweight="bold")

    ax = axes[0, 0]
    bars = ax.bar(x, model_summary["accuracy"], color=colors[: len(models)])
    ax.set_xticks(list(x), models, rotation=20, ha="right")
    ax.set_ylabel("Accuracy (%)")
    ax.set_title("Accuracy by model")
    ax.set_ylim(0, 100)
    for bar, value in zip(bars, model_summary["accuracy"]):
        ax.text(
            bar.get_x() + bar.get_width() / 2,
            bar.get_height() + 1,
            f"{value:.1f}%",
            ha="center",
            va="bottom",
            fontsize=9,
        )

    ax = axes[0, 1]
    bars = ax.bar(x, model_summary["totalRequestTime_mean"], color=colors[: len(models)])
    ax.set_xticks(list(x), models, rotation=20, ha="right")
    ax.set_ylabel("Seconds")
    ax.set_title("Mean totalRequestTime by model")
    for bar, value in zip(bars, model_summary["totalRequestTime_mean"]):
        ax.text(
            bar.get_x() + bar.get_width() / 2,
            bar.get_height() + 0.3,
            f"{value:.1f}s",
            ha="center",
            va="bottom",
            fontsize=9,
        )

    ax = axes[1, 0]
    bars = ax.bar(x, model_summary["loadSeconds_mean"], color=colors[: len(models)])
    ax.set_xticks(list(x), models, rotation=20, ha="right")
    ax.set_ylabel("Seconds")
    ax.set_title("Mean loadSeconds by model")
    for bar, value in zip(bars, model_summary["loadSeconds_mean"]):
        ax.text(
            bar.get_x() + bar.get_width() / 2,
            bar.get_height() + 0.005,
            f"{value:.3f}s",
            ha="center",
            va="bottom",
            fontsize=9,
        )

    ax = axes[1, 1]
    box_data = [df.loc[df["model"] == model, "totalRequestTime"] for model in models]
    ax.boxplot(box_data, tick_labels=models)
    ax.set_xticklabels(models, rotation=20, ha="right")
    ax.set_ylabel("Seconds")
    ax.set_title("totalRequestTime distribution by model")

    fig.tight_layout()
    summary_path = graph_dir / f"ai_data_analysis_{stamp}_summary.png"
    fig.savefig(summary_path, dpi=150, bbox_inches="tight")
    saved.append(summary_path)
    if show:
        plt.show()
    plt.close(fig)

    # --- Figure 2: per-prompt accuracy heatmap ---
    heatmap_df = accuracy_pivot.copy()
    heatmap_df.index = [label.split(" | ", 1)[0] for label in heatmap_df.index]
    heatmap_df = heatmap_df.sort_index()

    fig_height = max(6, len(heatmap_df) * 0.12)
    fig, ax = plt.subplots(figsize=(10, fig_height))
    im = ax.imshow(heatmap_df.values, aspect="auto", cmap="RdYlGn", vmin=0, vmax=1)
    ax.set_xticks(range(len(heatmap_df.columns)), heatmap_df.columns, rotation=20, ha="right")
    ax.set_yticks(range(len(heatmap_df.index)), heatmap_df.index, fontsize=7)
    ax.set_title(f"Accuracy per prompt by model ({stamp})")
    ax.set_xlabel("Model")
    ax.set_ylabel("PMID")
    cbar = fig.colorbar(im, ax=ax, shrink=0.6)
    cbar.set_label("Correct (1) / Wrong (0)")
    fig.tight_layout()
    heatmap_path = graph_dir / f"ai_data_analysis_{stamp}_accuracy_heatmap.png"
    fig.savefig(heatmap_path, dpi=150, bbox_inches="tight")
    saved.append(heatmap_path)
    if show:
        plt.show()
    plt.close(fig)

    # --- Figure 3: per-prompt totalRequestTime by model ---
    timing_df = (
        df.pivot_table(index="prompt_index", columns="model", values="totalRequestTime")
        .sort_index()
    )
    fig, ax = plt.subplots(figsize=(10, 5))
    for model in timing_df.columns:
        ax.plot(
            timing_df.index,
            timing_df[model],
            marker="o",
            markersize=3,
            linewidth=1,
            label=model,
            alpha=0.8,
        )
    ax.set_xlabel("Prompt index")
    ax.set_ylabel("totalRequestTime (seconds)")
    ax.set_title(f"Request time per prompt by model ({stamp})")
    ax.legend(loc="upper right", fontsize=8)
    ax.grid(True, alpha=0.3)
    fig.tight_layout()
    timing_path = graph_dir / f"ai_data_analysis_{stamp}_timing.png"
    fig.savefig(timing_path, dpi=150, bbox_inches="tight")
    saved.append(timing_path)
    if show:
        plt.show()
    plt.close(fig)

    # --- Figure 4: per-prompt loadSeconds by model ---
    load_df = (
        df.pivot_table(index="prompt_index", columns="model", values="loadSeconds")
        .sort_index()
    )
    fig, ax = plt.subplots(figsize=(10, 5))
    for model in load_df.columns:
        ax.plot(
            load_df.index,
            load_df[model],
            marker="o",
            markersize=3,
            linewidth=1,
            label=model,
            alpha=0.8,
        )
    ax.set_xlabel("Prompt index")
    ax.set_ylabel("loadSeconds (seconds)")
    ax.set_title(f"Load time per prompt by model ({stamp})")
    ax.legend(loc="upper right", fontsize=8)
    ax.grid(True, alpha=0.3)
    fig.tight_layout()
    load_path = graph_dir / f"ai_data_analysis_{stamp}_load.png"
    fig.savefig(load_path, dpi=150, bbox_inches="tight")
    saved.append(load_path)
    if show:
        plt.show()
    plt.close(fig)

    return saved


def main() -> int:
    parser = argparse.ArgumentParser(
        description="Analyze latest ai_data CSV: timing and accuracy per prompt and model."
    )
    parser.add_argument(
        "--ai-data",
        type=Path,
        default=None,
        help="Path to ai_data CSV (default: latest in Assets/Data)",
    )
    parser.add_argument(
        "--output",
        type=Path,
        default=None,
        help="Optional path to write the wide comparison CSV",
    )
    parser.add_argument(
        "--show-rows",
        type=int,
        default=20,
        help="Number of per-prompt rows to print (0 = summary only)",
    )
    parser.add_argument(
        "--no-graph",
        action="store_true",
        help="Skip generating matplotlib charts",
    )
    parser.add_argument(
        "--show-graph",
        action="store_true",
        help="Display charts interactively (still saves PNG files)",
    )
    parser.add_argument(
        "--graph-dir",
        type=Path,
        default=DATA_DIR,
        help="Directory for saved chart PNGs (default: Assets/Data)",
    )
    args = parser.parse_args()

    ai_data_path = args.ai_data or find_latest_ai_data_csv()
    results_path = matching_results_csv(ai_data_path)
    results_df = pd.read_csv(results_path)
    ai_df = pd.read_csv(ai_data_path)
    missing_df = missing_data_by_model(results_df, ai_df)
    df, skipped = load_run_data(ai_data_path, results_path)

    print_section("Input files")
    print(f"ai_data:   {ai_data_path}")
    print(f"results:   {results_path}")
    print(f"rows:      {len(df)} (aligned)")
    print(f"models:    {', '.join(sorted(df['model'].unique()))}")

    prompts_json = json.loads(PROMPTS_JSON.read_text(encoding="utf-8")) if PROMPTS_JSON.exists() else {}
    if "seed" in prompts_json:
        print(f"prompt seed: {prompts_json['seed']}")

    print_section("Missing / skipped data by model")
    print(missing_df.to_string(index=False))
    stamp = AI_DATA_PATTERN.match(ai_data_path.name).group(1)
    missing_path = DATA_DIR / f"ai_data_analysis_{stamp}_missing.csv"
    missing_df.to_csv(missing_path, index=False)
    print(f"Wrote missing-data CSV: {missing_path}")

    print_section("Accuracy and timing by model")
    model_summary = summarize_by_model(df)
    print(model_summary.to_string(index=False, float_format=lambda x: f"{x:.3f}"))

    print_section("totalRequestTime per prompt (seconds, by model)")
    timing_pivot = per_prompt_timing_pivot(df, "totalRequestTime")
    if args.show_rows == 0:
        print(f"{len(timing_pivot)} prompts (use --show-rows N to print)")
    else:
        print(timing_pivot.head(args.show_rows).to_string(float_format=lambda x: f"{x:.3f}"))
        if len(timing_pivot) > args.show_rows:
            print(f"... {len(timing_pivot) - args.show_rows} more prompts")

    print_section("loadSeconds per prompt (seconds, by model)")
    load_pivot = per_prompt_timing_pivot(df, "loadSeconds")
    if args.show_rows == 0:
        print(f"{len(load_pivot)} prompts (use --show-rows N to print)")
    else:
        print(load_pivot.head(args.show_rows).to_string(float_format=lambda x: f"{x:.3f}"))
        if len(load_pivot) > args.show_rows:
            print(f"... {len(load_pivot) - args.show_rows} more prompts")

    print_section("Accuracy per prompt (1=correct, 0=wrong, by model)")
    accuracy_pivot = per_prompt_accuracy_pivot(df)
    if args.show_rows == 0:
        print(f"{len(accuracy_pivot)} prompts (use --show-rows N to print)")
    else:
        print(accuracy_pivot.head(args.show_rows).to_string())
        if len(accuracy_pivot) > args.show_rows:
            print(f"... {len(accuracy_pivot) - args.show_rows} more prompts")

    comparison = compare_models(df)
    output_path = args.output
    if output_path is None:
        output_path = DATA_DIR / f"ai_data_analysis_{stamp}.csv"
    comparison.to_csv(output_path, index=False)
    print_section("Output")
    print(f"Wrote wide comparison CSV: {output_path}")

    if not args.no_graph:
        graph_paths = plot_results(
            df,
            model_summary,
            accuracy_pivot,
            stamp,
            graph_dir=args.graph_dir,
            show=args.show_graph,
        )
        for path in graph_paths:
            print(f"Wrote chart: {path}")

    return 0


if __name__ == "__main__":
    raise SystemExit(main())
