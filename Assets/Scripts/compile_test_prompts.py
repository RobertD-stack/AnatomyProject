import argparse
import json
import random
import secrets
import urllib.request
from pathlib import Path

PQA_URL = "https://raw.githubusercontent.com/pubmedqa/pubmedqa/master/data/ori_pqal.json"
GT_URL = "https://raw.githubusercontent.com/pubmedqa/pubmedqa/master/data/test_ground_truth.json"
OUT = Path(__file__).parent / "pubmedqa_test_prompts.json"


# We build a user prompt to answer the question, and then cut that answer down to yes or no
# We ask the model to include an explanation to imitate a human answer.
def build_user_prompt(question: str, contexts: list[str]) -> str:
    abstract = "\n\n".join(contexts)
    return (
        "Answer the biomedical research question using only the abstract below.\n"
        "Reply with exactly one word: yes, no, or maybe, followed by a short explanation of 2-3 sentences.\n\n"
        f"Question: {question}\n\n"
        f"Abstract:\n{abstract}\n\n"
        "Answer:"
    )


def main() -> None:
    parser = argparse.ArgumentParser(description="Build pubmedqa_test_prompts.json for Unity batch eval.")
    parser.add_argument(
        "--seed",
        type=int,
        default=None,
        help="RNG seed for PMID order (default: random each run)",
    )
    args = parser.parse_args()

    pqal = json.loads(urllib.request.urlopen(PQA_URL).read())
    ground_truth = json.loads(urllib.request.urlopen(GT_URL).read())

    seed = args.seed if args.seed is not None else secrets.randbelow(2**31)
    rng = random.Random(seed)
    pmids = list(ground_truth.keys())
    rng.shuffle(pmids)

    items = []
    for pmid in pmids:
        gold = ground_truth[pmid]
        row = pqal[pmid]
        items.append({
            "pmid": pmid,
            "question": row["QUESTION"],
            "gold": gold,
            "prompt": build_user_prompt(row["QUESTION"], row["CONTEXTS"]),
        })

    payload = {"seed": seed, "items": items}
    OUT.write_text(json.dumps(payload, ensure_ascii=False, indent=2), encoding="utf-8")
    print(f"Wrote {len(items)} prompts to {OUT} (seed={seed})")


if __name__ == "__main__":
    main()
