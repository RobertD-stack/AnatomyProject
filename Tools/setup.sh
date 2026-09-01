#!/usr/bin/env bash
# Usage:
#   chmod +x Tools/setup.sh
#   ./Tools/setup.sh
set -u

SKIP_OLLAMA=0
SKIP_WHISPER=0
SKIP_PYTHON=0
FAILURES=()

for arg in "$@"; do
  case "$arg" in
    --skip-ollama) SKIP_OLLAMA=1 ;;
    --skip-whisper) SKIP_WHISPER=1 ;;
    --skip-python) SKIP_PYTHON=1 ;;
    *) echo "Unknown argument: $arg" >&2; exit 2 ;;
  esac
done

REPO_ROOT="$(cd "$(dirname "$0")/.." && pwd)"
cd "$REPO_ROOT"

step() { printf '\n==> %s\n' "$1"; }
ok() { printf '    OK  %s\n' "$1"; }
warn() { printf '    WARN  %s\n' "$1"; }
fail() { printf '    FAIL  %s\n' "$1"; FAILURES+=("$1"); }

echo "AnatomyProject machine setup"
echo "Repo: $REPO_ROOT"

step "Git"
if ! command -v git >/dev/null 2>&1; then
  fail "git is not on PATH"
else
  ok "$(git --version)"
fi

step "whisper.unity local package"
WHISPER_ROOT="$(cd "$REPO_ROOT/.." && pwd)/whisper.unity-master"
WHISPER_PKG="$WHISPER_ROOT/Packages/com.whisper.unity"
if [ "$SKIP_WHISPER" -eq 1 ]; then
  warn "Skipping Whisper clone"
elif [ -f "$WHISPER_PKG/package.json" ]; then
  ok "Found $WHISPER_PKG"
elif ! command -v git >/dev/null 2>&1; then
  fail "whisper (git missing)"
else
  git clone --depth 1 https://github.com/Macoron/whisper.unity.git "$WHISPER_ROOT"
  if [ -f "$WHISPER_PKG/package.json" ]; then
    ok "Whisper package ready"
  else
    fail "whisper clone incomplete"
  fi
fi

step "Python packages"
PYTHON_BIN=""
for candidate in python3 python; do
  if command -v "$candidate" >/dev/null 2>&1; then
    PYTHON_BIN="$candidate"
    break
  fi
done
if [ "$SKIP_PYTHON" -eq 1 ]; then
  warn "Skipping Python packages"
elif [ -z "$PYTHON_BIN" ]; then
  fail "python"
else
  ok "$("$PYTHON_BIN" --version 2>&1)"
  "$PYTHON_BIN" -m pip install --upgrade pip
  if "$PYTHON_BIN" -m pip install -r "$REPO_ROOT/Tools/requirements.txt"; then
    ok "Installed packages from Tools/requirements.txt"
  else
    fail "python-packages"
  fi
fi

step "Ollama"
if [ "$SKIP_OLLAMA" -eq 1 ]; then
  warn "Skipping Ollama"
elif ! command -v ollama >/dev/null 2>&1; then
  fail "ollama not installed. Get it from https://ollama.com/download"
else
  ok "$(ollama --version 2>&1)"
  if ! curl -sf --max-time 2 http://127.0.0.1:11434/api/tags >/dev/null; then
    ollama serve >/dev/null 2>&1 &
    sleep 3
  fi
  pull_model() {
    echo "    ollama pull $1"
    ollama pull "$1" || warn "Could not pull $1"
  }
  pull_model "llama3.2:1b"
  pull_model "llama3.2"
  pull_model "qwen2.5:3b"
  pull_model "medllama2"
  pull_model "meditron"
  pull_model "ncatmedllama"
  MODELFILE="$REPO_ROOT/Assets/OllamaCustom/Modelfile"
  if [ -f "$MODELFILE" ]; then
    FAST_FILE="$(mktemp)"
    sed 's/^FROM .*/FROM llama3.2:1b/' "$MODELFILE" > "$FAST_FILE"
    echo "    ollama create anatomy-tutor-fast"
    if ollama create anatomy-tutor-fast -f "$FAST_FILE"; then
      ok "Created anatomy-tutor-fast"
    else
      warn "Could not create anatomy-tutor-fast"
    fi
    rm -f "$FAST_FILE"
    echo "    ollama create anatomy-tutor"
    ollama create anatomy-tutor -f "$MODELFILE" || warn "Could not create anatomy-tutor"
  fi
fi

step "API key templates"
AUTH_DEST="$REPO_ROOT/Assets/Auth/auth.json"
if [ ! -f "$AUTH_DEST" ]; then
  cp "$REPO_ROOT/Tools/templates/auth.json.example" "$AUTH_DEST"
  warn "Created Assets/Auth/auth.json from template. Paste your key."
else
  ok "Assets/Auth/auth.json already exists (left unchanged)"
fi
GEMINI_DEST="$REPO_ROOT/Assets/GeminiManager/JSON_KEY_TEMPLATE.json"
if [ ! -f "$GEMINI_DEST" ]; then
  cp "$REPO_ROOT/Tools/templates/gemini-key.json.example" "$GEMINI_DEST"
  warn "Created Gemini key file from template. Paste your key."
else
  ok "Gemini key asset already exists (left unchanged)"
fi

step "What Unity will not install for you"
cat <<'EOF'
    Unity Editor: 6000.1.3f1 (Unity 6.1). Open this folder in Unity Hub.
    Restart Unity Hub after installing Git so Package Manager can see git.
    Hugging Face key: paste into Assets/Resources/HuggingFaceAPIConfig if needed.
    SendData.cs talks to http://localhost:3000 — optional, not in this repo.
    VR: install an OpenXR runtime separately (Quest Link, SteamVR, etc.).
    Default AI model is anatomy-tutor-fast via Ollama at localhost:11434.
EOF

if [ "${#FAILURES[@]}" -gt 0 ]; then
  echo ""
  echo "Setup finished with missing items: ${FAILURES[*]}"
  echo "Fix those, then re-run Tools/setup.sh"
  exit 1
fi

echo ""
echo "Setup finished. Open AnatomyProject in Unity Hub, then press Play."
exit 0
