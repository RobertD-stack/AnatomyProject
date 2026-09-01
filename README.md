# Skeleton Puzzle (AnatomyProject)

Unity anatomy puzzle + AI evaluation project.

**This folder is the git repository root.** Paths are `Assets/`, `ProjectSettings/`, etc. (no `Skeleton Puzzle/` prefix).

Open this folder in Unity Hub (**Editor 6000.1.3f1**).

## First-time clone

Unity restores registry packages by itself. Everything else (Ollama models, Whisper checkout, Python) is installed by the setup script:

**Windows**

```powershell
powershell -ExecutionPolicy Bypass -File Tools\setup.ps1
```

**macOS / Linux**

```bash
chmod +x Tools/setup.sh
./Tools/setup.sh
```

Then paste API keys into the files the script created or left in place (`Assets/Auth/auth.json`, `Assets/GeminiManager/JSON_KEY_TEMPLATE.json`). Hugging Face keys go on `Assets/Resources/HuggingFaceAPIConfig`.

Quit Unity Hub fully after installing Git so the Editor can see `git.exe`.
