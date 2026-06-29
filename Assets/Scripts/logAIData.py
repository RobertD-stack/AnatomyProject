from __future__ import annotations

import csv
import sys
import traceback
from datetime import datetime
from pathlib import Path

CSV_HEADERS = [
    "totalRequestTime",
    "loadSeconds",
    "evalTokensPerSecond",
    "modelName",
    "promptTokenCount",
    "outputTokenCount",
    "timestamp",
]

# Assets/Scripts/logAIData.py -> Assets/Data
DATA_DIR = Path(__file__).resolve().parent.parent / "Data"


def timestamped_log_path(when: datetime | None = None) -> Path:
    """Build Assets/Data/ai_data_YYYYMMDD_HHMMSS.csv for a new log file."""
    when = when or datetime.now()
    return DATA_DIR / f"ai_data_{when.strftime('%Y%m%d_%H%M%S')}.csv"


def append_test_results(
    totalRequestTime: float,
    loadSeconds: float,
    evalTokensPerSecond: float,
    modelName: str,
    promptTokenCount: float,
    outputTokenCount: float,
    log_path: str | Path | None = None,
    timestamp: str | None = None,
) -> Path:
    """Append Ollama diagnostics to a CSV log."""
    log_path = Path(log_path) if log_path is not None else timestamped_log_path()
    log_path.parent.mkdir(parents=True, exist_ok=True)

    if timestamp is None:
        timestamp = datetime.now().strftime("%Y-%m-%d %H:%M:%S")

    write_header = not log_path.exists() or log_path.stat().st_size == 0

    with log_path.open("a", newline="", encoding="utf-8") as log_file:
        writer = csv.writer(log_file)
        if write_header:
            writer.writerow(CSV_HEADERS)
        writer.writerow([
            totalRequestTime,
            loadSeconds,
            evalTokensPerSecond,
            modelName,
            promptTokenCount,
            outputTokenCount,
            timestamp,
        ])

    return log_path.resolve()


def _print_usage() -> None:
    print(
        "Usage: python logAIData.py "
        "<totalRequestTime> <loadSeconds> <evalTokensPerSecond> "
        "<modelName> <promptTokenCount> <outputTokenCount> [log_path]",
        file=sys.stderr,
    )


if __name__ == "__main__":
    try:
        if len(sys.argv) not in (7, 8):
            _print_usage()
            sys.exit(1)

        log_path = sys.argv[7] if len(sys.argv) == 8 else None

        written_path = append_test_results(
            float(sys.argv[1]),
            float(sys.argv[2]),
            float(sys.argv[3]),
            sys.argv[4],
            float(sys.argv[5]),
            float(sys.argv[6]),
            log_path=log_path,
        )
        print(written_path)
    except Exception:
        traceback.print_exc(file=sys.stderr)
        sys.exit(1)
