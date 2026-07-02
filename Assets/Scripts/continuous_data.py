
import json
from pathlib import Path

def append_to_llm_dataset(user_input: str, expected_output: str, file_path="continuous_dataset.jsonl"):
    """
    Appends new data to your growing dataset in a format 
    fine-tuning libraries natively understand.
    """
    record = {
        "messages": [
            {"role": "user", "content": user_input},
            {"role": "assistant", "content": expected_output}
        ]
    }
    
    # Append the new record as a single JSON line
    with open(file_path, "a", encoding="utf-8") as f:
        f.write(json.dumps(record) + "\n")