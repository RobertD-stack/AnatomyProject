#!/usr/bin/env python3
"""
Benchmark local Ollama models (latency, throughput, memory, Unity-style chat).

HOW THIS SCRIPT IS ORGANIZED
----------------------------
1. Constants & LatencyResult  — shared prompts and a typed result container
2. OllamaBenchmark            — base class: HTTP calls, latency tests, per-model run
3. AdvancedOllamaBenchmark    — extends base: throughput loop + RAM sampling
4. parse_args / main          — CLI entry point, loops models, optional JSON export

DEPENDENCIES (see requirements.txt)
-----------------------------------
- requests  — POST to Ollama's REST API (same as curl, but from Python)
- psutil    — read system RAM (advanced memory profile only)

Usage:
  pip install -r requirements.txt
  python benchmark_ollama.py --models ncatmedllama llama3.2:1b medllama2
  python benchmark_ollama.py --installed
  python benchmark_ollama.py --models ncatmedllama --advanced --duration 30
"""

from __future__ import annotations

import argparse
import json
import subprocess
import sys
import time
from dataclasses import asdict, dataclass
from datetime import datetime, timezone
from pathlib import Path
from typing import Any

import psutil
import requests

# Ollama serves on this port by default when you run `ollama serve`.
DEFAULT_BASE_URL = "http://localhost:11434"

# Prompts chosen to match this project: anatomy content + realistic reply length.
ANATOMY_PROMPT = "What is the femur and what does it do?"
THROUGHPUT_PROMPT = "Write a short paragraph about human anatomy for high school students."
WARMUP_PROMPT = "Hello"  # tiny prompt to load the model before timed tests


# @dataclass auto-generates __init__, so we can return structured results
# without writing a lot of boilerplate property code.
@dataclass
class LatencyResult:
    """One timed request broken into fields we care about for comparison."""

    model: str
    endpoint: str  # "/api/generate" or "/api/chat"
    prompt: str
    success: bool
    total_seconds: float       # entire round-trip (nanoseconds -> seconds below)
    load_seconds: float        # time Ollama spent loading weights into RAM/VRAM
    prompt_eval_count: int     # tokens in the prompt
    eval_count: int            # tokens in the model's reply
    eval_tokens_per_second: float
    response_preview: str      # first ~120 chars, for sanity-checking output
    error: str | None = None


class OllamaBenchmark:
    """
    Base benchmarker.

    Design: one small HTTP helper (_post), then specialized measure_* methods
    that all parse the same Ollama JSON timing fields.
    """

    def __init__(self, base_url: str = DEFAULT_BASE_URL, timeout: int = 120):
        self.base_url = base_url.rstrip("/")  # avoid double slashes in URLs
        self.timeout = timeout  # large models can take a while on first run

    def list_installed_models(self) -> list[str]:
        """
        Shell out to `ollama list` instead of calling an API endpoint.
        Parses the NAME column (first token on each line after the header).
        """
        try:
            result = subprocess.run(
                ["ollama", "list"],
                capture_output=True,
                text=True,
                check=True,
                timeout=30,
            )
        except (FileNotFoundError, subprocess.CalledProcessError) as exc:
            raise RuntimeError("Could not run `ollama list`. Is Ollama installed and on PATH?") from exc

        models: list[str] = []
        for line in result.stdout.strip().splitlines()[1:]:  # skip header row
            if line.strip():
                models.append(line.split()[0])
        return models

    def _post(self, path: str, payload: dict[str, Any]) -> dict[str, Any]:
        """Shared POST helper — all benchmarks talk to Ollama through this."""
        response = requests.post(
            f"{self.base_url}{path}",
            json=payload,
            timeout=self.timeout,
        )
        response.raise_for_status()  # turn HTTP 4xx/5xx into an exception
        return response.json()

    def warmup(self, model_name: str) -> None:
        """
        First request after idle often includes model load time.
        Warmup separates "cold start" from the timed latency measurements.
        """
        self._post(
            "/api/generate",
            {"model": model_name, "prompt": WARMUP_PROMPT, "stream": False},
        )

    def _latency_from_response(
        self,
        model_name: str,
        endpoint: str,
        prompt: str,
        data: dict[str, Any],
    ) -> LatencyResult:
        """
        Ollama returns durations in NANOSECONDS and token counts as integers.
        We convert to seconds and compute tok/s from eval_count / eval_duration.

        /api/generate puts text in "response".
        /api/chat puts text in message.content — handled below.
        """
        total_ns = data.get("total_duration", 0) or 0
        load_ns = data.get("load_duration", 0) or 0
        eval_ns = data.get("eval_duration", 0) or 0
        eval_count = int(data.get("eval_count", 0) or 0)
        prompt_eval_count = int(data.get("prompt_eval_count", 0) or 0)

        text = data.get("response") or ""
        if not text and data.get("message"):
            text = data["message"].get("content", "")

        tokens_per_second = 0.0
        if eval_ns > 0 and eval_count > 0:
            tokens_per_second = eval_count / (eval_ns / 1e9)

        preview = text.replace("\n", " ").strip()
        if len(preview) > 120:
            preview = preview[:117] + "..."

        return LatencyResult(
            model=model_name,
            endpoint=endpoint,
            prompt=prompt,
            success=True,
            total_seconds=total_ns / 1e9,
            load_seconds=load_ns / 1e9,
            prompt_eval_count=prompt_eval_count,
            eval_count=eval_count,
            eval_tokens_per_second=round(tokens_per_second, 2),
            response_preview=preview,
        )

    def measure_generate_latency(self, model_name: str, prompt: str) -> LatencyResult:
        """
        Tests POST /api/generate — simple prompt-in, text-out API.
        On failure, returns a LatencyResult with success=False instead of crashing.
        """
        try:
            data = self._post(
                "/api/generate",
                {"model": model_name, "prompt": prompt, "stream": False},
            )
            return self._latency_from_response(model_name, "/api/generate", prompt, data)
        except requests.RequestException as exc:
            return LatencyResult(
                model=model_name,
                endpoint="/api/generate",
                prompt=prompt,
                success=False,
                total_seconds=0.0,
                load_seconds=0.0,
                prompt_eval_count=0,
                eval_count=0,
                eval_tokens_per_second=0.0,
                response_preview="",
                error=str(exc),
            )

    def measure_chat_latency(self, model_name: str, prompt: str) -> LatencyResult:
        """
        Tests POST /api/chat — same endpoint and message shape as UnityAndGeminiV3.cs.
        Comparing generate vs chat shows whether your Unity path behaves differently.
        """
        try:
            data = self._post(
                "/api/chat",
                {
                    "model": model_name,
                    "messages": [{"role": "user", "content": prompt}],
                    "stream": False,
                },
            )
            return self._latency_from_response(model_name, "/api/chat", prompt, data)
        except requests.RequestException as exc:
            return LatencyResult(
                model=model_name,
                endpoint="/api/chat",
                prompt=prompt,
                success=False,
                total_seconds=0.0,
                load_seconds=0.0,
                prompt_eval_count=0,
                eval_count=0,
                eval_tokens_per_second=0.0,
                response_preview="",
                error=str(exc),
            )

    def benchmark_model(self, model_name: str) -> dict[str, Any]:
        """
        Standard per-model run: warmup -> generate latency -> chat latency.
        Returns a dict (not LatencyResult) so we can JSON-serialize the full report.
        """
        print(f"\n=== {model_name} ===")
        print("  Warming up...")
        try:
            self.warmup(model_name)
        except requests.RequestException as exc:
            return {"model": model_name, "error": f"Warmup failed: {exc}"}

        generate = self.measure_generate_latency(model_name, ANATOMY_PROMPT)
        chat = self.measure_chat_latency(model_name, ANATOMY_PROMPT)

        for label, result in [("generate", generate), ("chat", chat)]:
            if result.success:
                print(
                    f"  {label:8} total={result.total_seconds:.2f}s  "
                    f"load={result.load_seconds:.2f}s  "
                    f"tokens={result.eval_count}  "
                    f"tok/s={result.eval_tokens_per_second:.2f}"
                )
            else:
                print(f"  {label:8} FAILED: {result.error}")

        # asdict() flattens dataclasses into plain dicts for json.dumps later.
        return {
            "model": model_name,
            "generate_latency": asdict(generate),
            "chat_latency": asdict(chat),
        }


class AdvancedOllamaBenchmark(OllamaBenchmark):
    """
    Extends the base class (inheritance) so we reuse _post, warmup, and latency tests.

    Adds:
    - measure_throughput: how many requests/tokens fit in a fixed time window
    - measure_memory_profile: system RAM snapshots via psutil
    """

    def measure_throughput(self, model_name: str, duration: int = 60) -> dict[str, float]:
        """
        Stress-style test: keep sending requests until `duration` seconds elapse.
        Uses eval_count from each response (real tokens), not len(text.split()).
        """
        start_time = time.time()
        request_count = 0
        total_tokens = 0
        failed_requests = 0

        while time.time() - start_time < duration:
            try:
                data = self._post(
                    "/api/generate",
                    {
                        "model": model_name,
                        "prompt": THROUGHPUT_PROMPT,
                        "stream": False,
                    },
                )
                request_count += 1
                total_tokens += int(data.get("eval_count", 0) or 0)
            except requests.RequestException:
                failed_requests += 1
                continue  # don't abort the whole window on one failure

        elapsed_time = max(time.time() - start_time, 1e-6)  # avoid divide-by-zero

        return {
            "duration_seconds": round(elapsed_time, 2),
            "total_requests": request_count,
            "failed_requests": failed_requests,
            "total_tokens": total_tokens,
            "requests_per_second": round(request_count / elapsed_time, 3),
            "tokens_per_second": round(total_tokens / elapsed_time, 3),
        }

    def measure_memory_profile(
        self, model_name: str, test_prompts: list[str]
    ) -> dict[str, Any]:
        """
        Samples total system RAM before/after each prompt.

        Note: this is whole-machine RAM, not Ollama-only VRAM. Good for rough
        comparisons; not as precise as nvidia-smi or Ollama internals.
        """
        memory_samples: list[dict[str, float | int]] = []

        self.warmup(model_name)

        for prompt in test_prompts:
            mem_before = psutil.virtual_memory().used / (1024 * 1024)

            self._post(
                "/api/generate",
                {"model": model_name, "prompt": prompt, "stream": False},
            )

            mem_after = psutil.virtual_memory().used / (1024 * 1024)
            memory_samples.append(
                {
                    "prompt_length": len(prompt),
                    "memory_before_mb": round(mem_before, 1),
                    "memory_after_mb": round(mem_after, 1),
                    "memory_delta_mb": round(mem_after - mem_before, 1),
                }
            )

        return {
            "samples": memory_samples,
            "peak_memory_mb": max(sample["memory_after_mb"] for sample in memory_samples),
            "average_delta_mb": round(
                sum(float(sample["memory_delta_mb"]) for sample in memory_samples)
                / len(memory_samples),
                2,
            ),
        }

    def benchmark_model_advanced(self, model_name: str, duration: int = 60) -> dict[str, Any]:
        """Runs the standard benchmark, then adds throughput + memory sections."""
        base = self.benchmark_model(model_name)

        print(f"  Throughput test ({duration}s)...")
        throughput = self.measure_throughput(model_name, duration=duration)
        print(
            f"  throughput req/s={throughput['requests_per_second']:.3f}  "
            f"tok/s={throughput['tokens_per_second']:.3f}"
        )

        memory_prompts = [
            "What is the humerus?",
            "Explain the role of the rib cage in breathing.",
            "Describe the difference between arteries and veins.",
        ]
        print("  Memory profile...")
        memory = self.measure_memory_profile(model_name, memory_prompts)
        print(
            f"  memory peak={memory['peak_memory_mb']:.1f} MB  "
            f"avg delta={memory['average_delta_mb']:.1f} MB"
        )

        base["throughput"] = throughput
        base["memory_profile"] = memory
        return base


def parse_args() -> argparse.Namespace:
    """argparse turns command-line flags into a namespace object (args.models, etc.)."""
    parser = argparse.ArgumentParser(description="Benchmark Ollama models.")
    parser.add_argument(
        "--models",
        nargs="*",
        help="Model names to benchmark (e.g. ncatmedllama llama3.2:1b).",
    )
    parser.add_argument(
        "--installed",
        action="store_true",
        help="Benchmark every model returned by `ollama list`.",
    )
    parser.add_argument(
        "--base-url",
        default=DEFAULT_BASE_URL,
        help=f"Ollama base URL (default: {DEFAULT_BASE_URL}).",
    )
    parser.add_argument(
        "--advanced",
        action="store_true",
        help="Run throughput and memory benchmarks (slower).",
    )
    parser.add_argument(
        "--duration",
        type=int,
        default=60,
        help="Throughput test duration in seconds (default: 60).",
    )
    parser.add_argument(
        "--output",
        type=Path,
        default=None,
        help="Optional JSON output path for results.",
    )
    return parser.parse_args()


def main() -> int:
    """
    Entry point:
    1. Parse CLI args
    2. Pick base vs advanced benchmark class
    3. Resolve model list (explicit, --installed, or project defaults)
    4. Loop models, collect results, optionally write JSON
    """
    args = parse_args()
    bench: OllamaBenchmark | AdvancedOllamaBenchmark
    bench = AdvancedOllamaBenchmark(args.base_url) if args.advanced else OllamaBenchmark(args.base_url)

    if args.installed:
        models = bench.list_installed_models()
    elif args.models:
        models = args.models
    else:
        # Default comparison set for this Anatomy project.
        models = ["ncatmedllama", "llama3.2:1b", "medllama2"]

    if not models:
        print("No models to benchmark.", file=sys.stderr)
        return 1

    print(f"Benchmarking {len(models)} model(s) against {args.base_url}")

    results: list[dict[str, Any]] = []
    for model_name in models:
        try:
            if args.advanced:
                results.append(bench.benchmark_model_advanced(model_name, duration=args.duration))
            else:
                results.append(bench.benchmark_model(model_name))
        except requests.RequestException as exc:
            print(f"  Skipped {model_name}: {exc}")

    report = {
        "timestamp": datetime.now(timezone.utc).isoformat(),
        "base_url": args.base_url,
        "advanced": args.advanced,
        "results": results,
    }

    if args.output:
        args.output.write_text(json.dumps(report, indent=2), encoding="utf-8")
        print(f"\nWrote results to {args.output}")

    return 0


# Standard Python pattern: only run main() when executed as a script, not when imported.
if __name__ == "__main__":
    raise SystemExit(main())
