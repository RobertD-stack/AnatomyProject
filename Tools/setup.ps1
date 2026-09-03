# Requires PowerShell 5.1+. Run from anywhere:
#   powershell -ExecutionPolicy Bypass -File Tools\setup.ps1
# Optional:
#   -SkipOllama / -SkipWhisper / -SkipPython

param(
    [switch]$SkipOllama,
    [switch]$SkipWhisper,
    [switch]$SkipPython
)

$ErrorActionPreference = "Stop"
$RepoRoot = Split-Path -Parent $PSScriptRoot
Set-Location $RepoRoot

function Write-Step($message) {
    Write-Host ""
    Write-Host "==> $message" -ForegroundColor Cyan
}

function Write-Ok($message) {
    Write-Host "    OK  $message" -ForegroundColor Green
}

function Write-Warn($message) {
    Write-Host "    WARN  $message" -ForegroundColor Yellow
}

function Write-Fail($message) {
    Write-Host "    FAIL  $message" -ForegroundColor Red
}

function Test-Cmd($name) {
    return [bool](Get-Command $name -ErrorAction SilentlyContinue)
}

$failures = New-Object System.Collections.Generic.List[string]

Write-Host "AnatomyProject machine setup"
Write-Host "Repo: $RepoRoot"

Write-Step "Git"
if (-not (Test-Cmd "git")) {
    Write-Fail "git is not on PATH. Install Git for Windows, then fully quit Unity Hub and reopen it."
    $failures.Add("git")
}
else {
    Write-Ok (git --version)
}

# --- Whisper Unity package (lives beside the repo, not in Git) ---
Write-Step "whisper.unity local package"
$whisperRoot = Join-Path (Split-Path -Parent $RepoRoot) "whisper.unity-master"
$whisperPkg = Join-Path (Join-Path $whisperRoot "Packages") "com.whisper.unity"
if ($SkipWhisper) {
    Write-Warn "Skipping Whisper clone"
}
elseif (Test-Path (Join-Path $whisperPkg "package.json")) {
    Write-Ok "Found $whisperPkg"
}
elseif (-not (Test-Cmd "git")) {
    Write-Fail "Cannot clone Whisper without git"
    $failures.Add("whisper")
}
else {
    Write-Host "    Cloning https://github.com/Macoron/whisper.unity.git -> $whisperRoot"
    git clone --depth 1 https://github.com/Macoron/whisper.unity.git $whisperRoot
    if (Test-Path (Join-Path $whisperPkg "package.json")) {
        Write-Ok "Whisper package ready for file:../whisper.unity-master/..."
    }
    else {
        Write-Fail "Clone finished but $whisperPkg is missing"
        $failures.Add("whisper")
    }
}

# --- Python (Unity calls "python" for logging / PubMedQA helpers) ---
Write-Step "Python packages"
$python = $null
foreach ($candidate in @("python", "python3", "py")) {
    if (Test-Cmd $candidate) {
        $python = $candidate
        break
    }
}
if ($SkipPython) {
    Write-Warn "Skipping Python packages"
}
elseif ($null -eq $python) {
    Write-Fail "Python not on PATH. Install Python 3.10+ and re-run."
    $failures.Add("python")
}
else {
    Write-Ok (& $python --version 2>&1 | Out-String).Trim()
    $req = Join-Path $PSScriptRoot "requirements.txt"
    & $python -m pip install --upgrade pip
    & $python -m pip install -r $req
    if ($LASTEXITCODE -ne 0) {
        Write-Fail "pip install failed"
        $failures.Add("python-packages")
    }
    else {
        Write-Ok "Installed packages from Tools/requirements.txt"
    }
}

# --- Ollama (models are local; Unity talks to http://localhost:11434) ---
Write-Step "Ollama"
if ($SkipOllama) {
    Write-Warn "Skipping Ollama"
}
else {
    if (-not (Test-Cmd "ollama")) {
        Write-Warn "ollama not found. Trying winget..."
        if (Test-Cmd "winget") {
            winget install --id Ollama.Ollama -e --accept-source-agreements --accept-package-agreements
            $env:Path = [Environment]::GetEnvironmentVariable("Path", "Machine") + ";" + [Environment]::GetEnvironmentVariable("Path", "User")
        }
        if (-not (Test-Cmd "ollama")) {
            Write-Fail "Install Ollama from https://ollama.com/download then re-run this script."
            $failures.Add("ollama")
        }
    }

    if (Test-Cmd "ollama") {
        Write-Ok (ollama --version 2>&1 | Out-String).Trim()
        Write-Host "    Starting ollama serve if needed..."
        try {
            Invoke-RestMethod -Uri "http://127.0.0.1:11434/api/tags" -TimeoutSec 2 | Out-Null
        }
        catch {
            Start-Process -FilePath "ollama" -ArgumentList "serve" -WindowStyle Hidden
            Start-Sleep -Seconds 3
        }

        $models = @(
            "llama3.2:1b",
            "llama3.2",
            "qwen2.5:3b",
            "medllama2",
            "meditron",
            "ncatmedllama"
        )

        foreach ($model in $models) {
            Write-Host "    ollama pull $model"
            ollama pull $model
            if ($LASTEXITCODE -ne 0) {
                Write-Warn "Could not pull $model (name may be custom or unavailable)."
            }
        }

        $modelFile = Join-Path (Join-Path $RepoRoot "Assets\OllamaCustom") "Modelfile"
        if (Test-Path $modelFile) {
            $fastFile = Join-Path $env:TEMP "anatomy-tutor-fast.Modelfile"
            $fastBody = (Get-Content -LiteralPath $modelFile -Raw) -replace "FROM\s+\S+", "FROM llama3.2:1b"
            Set-Content -LiteralPath $fastFile -Value $fastBody -Encoding utf8
            Write-Host "    ollama create anatomy-tutor-fast (Unity default)"
            ollama create anatomy-tutor-fast -f $fastFile
            if ($LASTEXITCODE -eq 0) {
                Write-Ok "Created anatomy-tutor-fast"
            }
            else {
                Write-Warn "Could not create anatomy-tutor-fast"
            }

            Write-Host "    ollama create anatomy-tutor from Assets/OllamaCustom/Modelfile"
            ollama create anatomy-tutor -f $modelFile
            if ($LASTEXITCODE -eq 0) {
                Write-Ok "Created anatomy-tutor"
            }
            else {
                Write-Warn "Could not create anatomy-tutor (needs the FROM base model pulled first)"
            }
        }
    }
}

# --- Summary ---
Write-Step "What Unity will not install for you"
Write-Host @"
    Unity Editor: 6000.1.3f1 (Unity 6.1). Open this folder in Unity Hub.
    Restart Unity Hub after installing Git so Package Manager can see git.exe.
    API keys: copy the templates yourself (see README.md).
    SendData.cs talks to http://localhost:3000 — that backend is optional and not in this repo.
    VR: OpenXR runtime (Quest Link, SteamVR, etc.) is installed separately.
    Default AI model in the scene is anatomy-tutor-fast via Ollama at localhost:11434.
"@

if ($failures.Count -gt 0) {
    Write-Host ""
    Write-Host "Setup finished with missing items: $($failures -join ', ')" -ForegroundColor Yellow
    Write-Host "Fix those, then re-run Tools\setup.ps1"
    exit 1
}

Write-Host ""
Write-Host "Setup finished. Open AnatomyProject in Unity Hub, then press Play." -ForegroundColor Green
exit 0
