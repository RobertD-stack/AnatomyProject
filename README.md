# Skeleton Puzzle (AnatomyProject)

Unity anatomy puzzle + AI evaluation project.

**This folder is the git repository root.** Paths are `Assets/`, `ProjectSettings/`, etc. (no `Skeleton Puzzle/` prefix).

Open this folder in Unity Hub with **Editor 6000.1.3f1**.

There is an optional helper script (`Tools/setup.ps1` on Windows, `Tools/setup.sh` on macOS/Linux) that can clone Whisper, install Python packages, and pull Ollama models. It does **not** create API key files. Everything below can be done by hand.

---

## 1. Clone the repo

Install Git first: [https://git-scm.com/downloads](https://git-scm.com/downloads)

```bash
git clone <your-repo-url>
cd "Skeleton Puzzle"
```

Fully **quit Unity Hub** after installing Git so Package Manager can see `git` / `git.exe`.

---

## 2. Install Unity 6.1

1. Install [Unity Hub](https://unity.com/download).
2. In Hub, install editor **6000.1.3f1** (Unity 6.1):
   - Archive listing: [https://unity.com/releases/editor/archive](https://unity.com/releases/editor/archive)
   - Direct Hub link pattern: `unityhub://6000.1.3f1`
3. **Add project** → select this folder (`Skeleton Puzzle`).
4. Let Unity restore registry packages (`Addressables`, OpenXR, XR Interaction Toolkit, URP, etc.).

Unity will **not** restore the local Whisper package until step 4 exists on disk.

---

## 3. Clone Whisper (speech-to-text)

The project expects Whisper **beside** this repo, not inside it:

```
<parent>/
  Skeleton Puzzle/          ← this Unity project
  whisper.unity-master/     ← clone here
    Packages/com.whisper.unity/
```

Repo: [https://github.com/Macoron/whisper.unity](https://github.com/Macoron/whisper.unity)

From the **parent** of this project:

```bash
git clone --depth 1 https://github.com/Macoron/whisper.unity.git whisper.unity-master
```

Confirm `whisper.unity-master/Packages/com.whisper.unity/package.json` exists. `Packages/manifest.json` already points at `file:../whisper.unity-master/Packages/com.whisper.unity`.

---

## 4. Install Python (optional helpers)

Used by logging / PubMedQA analysis scripts under `Assets/Scripts/`.

1. Install Python 3.10+: [https://www.python.org/downloads/](https://www.python.org/downloads/)
   - On Windows, check **Add python.exe to PATH**.
2. From this project folder:

```bash
python -m pip install --upgrade pip
python -m pip install -r Tools/requirements.txt
```

That installs `requests`, `psutil`, `pandas`, and `matplotlib`.

---

## 5. Install Ollama and models (local AI)

The default in-scene model is **`anatomy-tutor-fast`** at `http://localhost:11434`.

1. Download Ollama: [https://ollama.com/download](https://ollama.com/download)
2. Start it (`ollama serve` if it is not already running). Confirm [http://127.0.0.1:11434](http://127.0.0.1:11434) responds.
3. Pull base models:

```bash
ollama pull llama3.2:1b
ollama pull llama3.2
ollama pull qwen2.5:3b
ollama pull medllama2
ollama pull meditron
ollama pull ncatmedllama
```

Model pages (names can change on Ollama’s library):

- [llama3.2](https://ollama.com/library/llama3.2)
- [qwen2.5](https://ollama.com/library/qwen2.5)
- [medllama2](https://ollama.com/library/medllama2)
- [meditron](https://ollama.com/library/meditron)

4. Create the custom anatomy models from `Assets/OllamaCustom/Modelfile`:

```bash
# Fast default used by Unity
# Temporarily set the Modelfile FROM line to: FROM llama3.2:1b
ollama create anatomy-tutor-fast -f Assets/OllamaCustom/Modelfile

# Medical-tuned variant (FROM medllama2 in the checked-in Modelfile)
ollama create anatomy-tutor -f Assets/OllamaCustom/Modelfile
```

---

## 6. API keys (manual)

The setup scripts **do not** write key files. Copy the examples yourself and paste keys. Do not commit real keys.

### OpenAI

1. Create a key: [https://platform.openai.com/api-keys](https://platform.openai.com/api-keys)
2. Copy `Tools/templates/auth.json.example` to `Assets/Auth/auth.json`
3. Replace `YOUR_OPENAI_API_KEY`:

```json
{
  "api_key": "YOUR_OPENAI_API_KEY",
  "organization": ""
}
```

### Gemini

1. Create a key: [https://aistudio.google.com/apikey](https://aistudio.google.com/apikey)
2. Copy `Tools/templates/gemini-key.json.example` to `Assets/GeminiManager/JSON_KEY_TEMPLATE.json`
3. Replace `YOUR_GEMINI_API_KEY`:

```json
{
  "key": "YOUR_GEMINI_API_KEY"
}
```

### Hugging Face (optional)

1. Create a token: [https://huggingface.co/settings/tokens](https://huggingface.co/settings/tokens)
2. Paste it on the `HuggingFaceAPIConfig` asset at `Assets/Resources/HuggingFaceAPIConfig`.

---

## 7. VR / OpenXR (optional)

Install a runtime separately, then enable OpenXR in **Project Settings → XR Plug-in Management**.

- Meta Quest Link: [https://www.meta.com/help/quest/articles/headsets-and-accessories/oculus-rift-s/install-app-for-pc/](https://www.meta.com/help/quest/articles/headsets-and-accessories/oculus-rift-s/install-app-for-pc/)
- SteamVR: [https://store.steampowered.com/app/250820/SteamVR/](https://store.steampowered.com/app/250820/SteamVR/)
- OpenXR overview: [https://docs.unity3d.com/Packages/com.unity.xr.openxr@1.14/manual/index.html](https://docs.unity3d.com/Packages/com.unity.xr.openxr@1.14/manual/index.html)

Use the `InputMode` enum on `ToggleVR` (`VR`, `MouseAndKeyboard`, `GetReal3D`).

GetReal3D CAVE testing needs Mechdyne’s full install (daemon, launcher, trackd / trackdsimulator) on the render machine. The Unity package under `Assets/getReal3DPackage/` is not enough to simulate wand tracking on a personal PC.

---

## 8. Optional extras

- **`SendData.cs`** POSTs to `http://localhost:3000/plummies`. That backend is not in this repo; skip it unless you run that service yourself.
- **getReal3D Scene Checker** will warn about Unity `Input.` usage. Most hits are third-party; only wrap your own gameplay scripts if you need cluster-safe input.

---

## Quick checklist

1. Git + Unity Hub + **6000.1.3f1**
2. Clone Whisper next to this folder
3. Python + `Tools/requirements.txt` (if you use the analysis scripts)
4. Ollama + pulls + `anatomy-tutor-fast` / `anatomy-tutor`
5. Copy key templates and paste keys
6. Open this folder in Unity Hub and press Play

Optional script (still skips API keys):

```powershell
powershell -ExecutionPolicy Bypass -File Tools\setup.ps1
```

```bash
chmod +x Tools/setup.sh
./Tools/setup.sh
```
