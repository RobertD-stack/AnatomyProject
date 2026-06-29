using System.Collections;

using System.Collections.Generic;

using System.Globalization;

using System.IO;

using System.Text;

using UnityEngine;

using UnityEngine.Networking;



public enum TrainingMode

{

    On,

    Off

}



[System.Serializable]

public class PubMedQaItem

{

    public string pmid;

    public string question;

    public string gold;

    public string prompt;

}



[System.Serializable]

public class PubMedQaPromptSet

{

    public int seed;

    public PubMedQaItem[] items;

}



public class RunAutomaticPrompts : MonoBehaviour

{

    [Header("References")]

    [Tooltip("UnityAndGeminiV3 on the scene; used for Ollama /api/chat requests.")]

    public UnityAndGeminiV3 gemini;

    [Tooltip("pubmedqa_test_prompts.json from compile_test_prompts.py. Optional if the file exists under Assets/Scripts/.")]

    public TextAsset promptSetJson;



    private const string DefaultPromptFileName = "pubmedqa_test_prompts.json";

    private const string CompileScriptFileName = "compile_test_prompts.py";



    [Header("Python")]

    [Tooltip("Full path to python.exe, or \"python\" if it is on PATH when Unity starts.")]

    public string pythonExecutable = "python";

    [Tooltip("Download PubMedQA and write pubmedqa_test_prompts.json before the batch run.")]

    public bool compilePromptsOnStartup = true;



    [Header("Run")]

    [Tooltip("When On, TextToSpeech disables itself during the batch.")]

    public TrainingMode trainingMode = TrainingMode.On;

    [Tooltip("Models to benchmark in order. Leave empty to use gemini.ollamaModel only.")]

    public OllamaModel[] ollamaModels;

    [Tooltip("0 = all prompts. Use a small number for smoke tests.")]

    public int maxPrompts = 100;

    [Tooltip("Optional pause between requests.")]

    public float delayBetweenRequestsSeconds = 0f;



    private const string DataFolderName = "Data";



    [Header("Output")]

    public bool logResultsToCsv = true;

    [Tooltip("Combined CSV filename under Assets/Data/. Set automatically once per run.")]

    public string resultsCsvFileName;



    private bool _batchSettingsSaved;

    private bool _savedUseMicrophoneInput;

    private bool _savedSuppressModelResponseEvents;

    private LlmBackend _savedLlmBackend;

    private OllamaModel _savedOllamaModel;

    private Coroutine _batchCoroutine;

    private readonly Dictionary<string, (int correct, int total)> _modelAccuracy =
        new Dictionary<string, (int correct, int total)>();

    private const string ResultsCsvHeader =
        "pmid,gold,raw_prediction,parsed_prediction,correct,ollama_model,model,total_seconds,timestamp,accuracy_percent";



    private void Start()

    {
        if (trainingMode == TrainingMode.On) {
            _batchCoroutine = StartCoroutine(RunAllModelsCoroutine());
        }

    }



    private void OnDisable()

    {

        if (_batchCoroutine != null)

        {

            StopCoroutine(_batchCoroutine);

            _batchCoroutine = null;

        }



        RestoreGeminiSettings();

    }



    public void RunBatch()

    {

        if (_batchCoroutine != null)

            StopCoroutine(_batchCoroutine);



        _batchCoroutine = StartCoroutine(RunAllModelsCoroutine());

    }



    private IEnumerator RunAllModelsCoroutine()

    {

        yield return null;



        if (gemini == null)

            gemini = FindObjectOfType<UnityAndGeminiV3>();



        if (gemini == null)

        {

            Debug.LogError("[RunAutomaticPrompts] Assign UnityAndGeminiV3 in the Inspector.");

            yield break;

        }



        if (compilePromptsOnStartup)

        {

            if (!RunCompileTestPrompts())

                yield break;

        }



        string promptJson = LoadPromptJsonText();

        if (promptJson == null)

            yield break;



        SaveGeminiSettingsForBatch();

        gemini.useMicrophoneInput = false;

        gemini.llmBackend = LlmBackend.Ollama;

        gemini.suppressModelResponseEvents = true;

        EnsureRunCsvFiles();

        if (ollamaModels == null || ollamaModels.Length == 0)

        {

            yield return RunPromptLoopCoroutine(promptJson);

        }

        else

        {

            for (int i = 0; i < ollamaModels.Length; i++)

            {
                if (i > 0) {
                    compilePromptsOnStartup = false;
                }

                if (!isActiveAndEnabled)

                    yield break;



                gemini.ollamaModel = ollamaModels[i];

                Debug.Log($"[RunAutomaticPrompts] Model {i + 1}/{ollamaModels.Length}: {ollamaModels[i]}");

                yield return RunPromptLoopCoroutine(promptJson);

                yield return UnloadOllamaModelCoroutine();

            }

        }



        RestoreGeminiSettings();

        _batchCoroutine = null;

    }



    private IEnumerator UnloadOllamaModelCoroutine()

    {

        string modelName = GetOllamaModelName(gemini.ollamaModel, gemini.customOllamaModel);

        string url = gemini.ollamaBaseUrl.TrimEnd('/') + "/api/chat";

        string jsonData = "{\"model\":\"" + modelName + "\",\"messages\":[],\"keep_alive\":0,\"stream\":false}";

        byte[] jsonToSend = Encoding.UTF8.GetBytes(jsonData);



        using (UnityWebRequest www = new UnityWebRequest(url, "POST"))

        {

            www.uploadHandler = new UploadHandlerRaw(jsonToSend);

            www.downloadHandler = new DownloadHandlerBuffer();

            www.SetRequestHeader("Content-Type", "application/json");

            if (gemini.ollamaRequestTimeoutSeconds > 0)

                www.timeout = gemini.ollamaRequestTimeoutSeconds;

            yield return www.SendWebRequest();

        }

    }



    private static string GetOllamaModelName(OllamaModel model, string customModel)

    {

        switch (model)

        {

            case OllamaModel.AnatomyTutorFast: return "anatomy-tutor-fast";

            case OllamaModel.AnatomyTutor: return "anatomy-tutor";

            case OllamaModel.NcatMedLlama: return "ncatmedllama";

            case OllamaModel.Meditron: return "meditron";

            case OllamaModel.MedLlama2: return "medllama2";

            case OllamaModel.Llama3_2: return "llama3.2";

            case OllamaModel.Llama3_2_1B: return "llama3.2:1b";

            case OllamaModel.Qwen2_5_3B: return "qwen2.5:3b";

            case OllamaModel.Custom: return customModel;

            default: return "anatomy-tutor-fast";

        }

    }



    private void SaveGeminiSettingsForBatch()

    {

        if (_batchSettingsSaved)

            return;



        _savedUseMicrophoneInput = gemini.useMicrophoneInput;

        _savedLlmBackend = gemini.llmBackend;

        _savedSuppressModelResponseEvents = gemini.suppressModelResponseEvents;

        _savedOllamaModel = gemini.ollamaModel;

        _batchSettingsSaved = true;

    }

    private void EnsureRunCsvFiles()

    {

        Directory.CreateDirectory(DataDirectory);

        string stamp = System.DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");

        resultsCsvFileName = "pubmedqa_results_" + stamp + ".csv";

        gemini.ollamaLogCsvFileName = "ai_data_" + stamp + ".csv";

        _modelAccuracy.Clear();

        string path = Path.Combine(DataDirectory, resultsCsvFileName);

        using (var writer = new StreamWriter(path, false, Encoding.UTF8))

        {

            writer.WriteLine(ResultsCsvHeader);

        }

    }



    private IEnumerator RunPromptLoopCoroutine(string promptJsonText)

    {

        PubMedQaPromptSet set = JsonUtility.FromJson<PubMedQaPromptSet>(promptJsonText);

        if (set?.items == null || set.items.Length == 0)

        {

            Debug.LogError("[RunAutomaticPrompts] No items in prompt JSON.");

            yield break;

        }



        int total = maxPrompts > 0 ? Mathf.Min(maxPrompts, set.items.Length) : set.items.Length;

        Debug.Log($"[RunAutomaticPrompts] Prompt set seed={set.seed}, using {total}/{set.items.Length} shuffled prompts.");

        Debug.Log($"[RunAutomaticPrompts] Running {total} PubMedQA prompts via Ollama ({gemini.ollamaModel}).");



        int correct = 0;

        int scored = 0;

        for (int i = 0; i < total; i++)

        {

            if (!isActiveAndEnabled)

                yield break;



            PubMedQaItem item = set.items[i];

            if (item == null || string.IsNullOrWhiteSpace(item.prompt))

            {

                Debug.LogWarning($"[RunAutomaticPrompts] Skipping empty prompt at index {i}.");

                continue;

            }



            gemini.sampleTestPrompt = item.prompt;

            yield return gemini.SendPromptRequestToOllama("");



            if (!gemini.lastOllamaRequestSucceeded)

            {

                Debug.LogWarning(

                    $"[RunAutomaticPrompts] Skipping pmid={item.pmid} ({gemini.ollamaModel}): Ollama request failed.");

                if (delayBetweenRequestsSeconds > 0f)

                    yield return new WaitForSeconds(delayBetweenRequestsSeconds);

                continue;

            }



            scored++;



            string prediction = gemini.lastModelResponse ?? "";

            string parsed = ParseYesNoMaybe(prediction);

            bool match = !string.IsNullOrEmpty(parsed) &&

                         string.Equals(parsed, item.gold, System.StringComparison.OrdinalIgnoreCase);

            if (match)

                correct++;



            if (logResultsToCsv)

                AppendResultRow(item, prediction, parsed, match);



            Debug.Log($"[RunAutomaticPrompts] {i + 1}/{total} pmid={item.pmid} gold={item.gold} pred={parsed} correct={match}");



            if (delayBetweenRequestsSeconds > 0f)

                yield return new WaitForSeconds(delayBetweenRequestsSeconds);

        }



        float accuracy = scored > 0 ? (float)correct / scored : 0f;

        Debug.Log($"[RunAutomaticPrompts] Done ({gemini.ollamaModel}). Accuracy: {correct}/{scored} ({accuracy:P1}) of {total} prompts");

    }



    private bool RunCompileTestPrompts()

    {

        string scriptsDir = Path.Combine(Application.dataPath, "Scripts");

        string scriptPath = Path.Combine(scriptsDir, CompileScriptFileName);



        if (!File.Exists(scriptPath))

        {

            Debug.LogError("[RunAutomaticPrompts] Script not found: " + scriptPath);

            return false;

        }



        string arguments = "-u \"" + scriptPath + "\"";

        var startInfo = new System.Diagnostics.ProcessStartInfo

        {

            FileName = pythonExecutable,

            Arguments = arguments,

            WorkingDirectory = scriptsDir,

            UseShellExecute = false,

            RedirectStandardOutput = true,

            RedirectStandardError = true,

            CreateNoWindow = true,

        };



        Debug.Log("[RunAutomaticPrompts] Running: " + pythonExecutable + " " + arguments);



        try

        {

            using (System.Diagnostics.Process process = System.Diagnostics.Process.Start(startInfo))

            {

                if (process == null)

                {

                    Debug.LogError("[RunAutomaticPrompts] Failed to start Python (Process.Start returned null).");

                    return false;

                }



                string stdout = process.StandardOutput.ReadToEnd();

                string stderr = process.StandardError.ReadToEnd();

                process.WaitForExit();



                if (!string.IsNullOrEmpty(stdout))

                    Debug.Log("[RunAutomaticPrompts] " + stdout.Trim());



                if (process.ExitCode != 0)

                {

                    Debug.LogError(

                        "[RunAutomaticPrompts] compile_test_prompts.py failed (exit " + process.ExitCode + ").\n" +

                        (string.IsNullOrEmpty(stderr) ? "(no stderr output)" : stderr.Trim()));

                    return false;

                }



                if (!string.IsNullOrEmpty(stderr))

                    Debug.LogWarning("[RunAutomaticPrompts] stderr: " + stderr.Trim());

            }

        }

        catch (System.Exception ex)

        {

            Debug.LogError("[RunAutomaticPrompts] Failed to run Python: " + ex);

            return false;

        }



        string outputPath = Path.Combine(scriptsDir, DefaultPromptFileName);

        if (!File.Exists(outputPath))

        {

            Debug.LogError("[RunAutomaticPrompts] Expected output not found: " + outputPath);

            return false;

        }



        return true;

    }



    private string LoadPromptJsonText()

    {

        string path = Path.Combine(Application.dataPath, "Scripts", DefaultPromptFileName);



        if (compilePromptsOnStartup || !File.Exists(path))

        {

            if (!File.Exists(path))

            {

                Debug.LogError("[RunAutomaticPrompts] Prompt file not found: " + path);

                return null;

            }



            Debug.Log("[RunAutomaticPrompts] Loaded prompts from " + path);

            return File.ReadAllText(path);

        }



        if (promptSetJson != null)

            return promptSetJson.text;



        Debug.Log("[RunAutomaticPrompts] Loaded prompts from " + path);

        return File.ReadAllText(path);

    }



    private void RestoreGeminiSettings()

    {

        if (!_batchSettingsSaved || gemini == null)

            return;



        gemini.useMicrophoneInput = _savedUseMicrophoneInput;

        gemini.llmBackend = _savedLlmBackend;

        gemini.suppressModelResponseEvents = _savedSuppressModelResponseEvents;

        gemini.ollamaModel = _savedOllamaModel;

        _batchSettingsSaved = false;

    }



    private static string DataDirectory =>

        Path.Combine(Application.dataPath, DataFolderName);



    private void AppendResultRow(PubMedQaItem item, string rawPrediction, string parsedPrediction, bool correct)

    {

        OllamaDiagnostics diag = gemini.lastOllamaDiagnostics;

        string ollamaModel = gemini.ollamaModel.ToString();

        string model = diag != null ? diag.model : "";

        string totalSeconds = diag != null

            ? diag.totalSeconds.ToString(CultureInfo.InvariantCulture)

            : "";

        string timestamp = System.DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");



        if (!_modelAccuracy.TryGetValue(ollamaModel, out var stats))

            stats = (0, 0);

        stats.total++;

        if (correct)

            stats.correct++;

        _modelAccuracy[ollamaModel] = stats;

        float accuracyPercent = stats.total > 0
            ? (float)stats.correct / stats.total * 100f
            : 0f;



        string line = string.Join(",",

            CsvField(item.pmid),

            CsvField(item.gold),

            CsvField(rawPrediction),

            CsvField(parsedPrediction),

            correct ? "1" : "0",

            CsvField(ollamaModel),

            CsvField(model),

            totalSeconds,

            CsvField(timestamp),

            accuracyPercent.ToString("F1", CultureInfo.InvariantCulture));



        AppendPubMedQaResultsCsvRow(line);

    }



    private void AppendPubMedQaResultsCsvRow(string line)

    {

        string path = Path.Combine(DataDirectory, resultsCsvFileName);



        using (var writer = new StreamWriter(path, true, Encoding.UTF8))

        {

            writer.WriteLine(line);

        }

    }



    private static string CsvField(string value)

    {

        if (string.IsNullOrEmpty(value))

            return "";



        if (value.Contains(",") || value.Contains("\"") || value.Contains("\n") || value.Contains("\r"))

            return "\"" + value.Replace("\"", "\"\"") + "\"";



        return value;

    }



    private static string ParseYesNoMaybe(string text)

    {

        if (string.IsNullOrWhiteSpace(text))

            return "";



        string normalized = text.Trim().ToLowerInvariant();

        if (normalized.StartsWith("yes"))

            return "yes";

        if (normalized.StartsWith("no"))

            return "no";

        if (normalized.StartsWith("maybe"))

            return "maybe";



        foreach (string word in normalized.Split((char[])null, System.StringSplitOptions.RemoveEmptyEntries))

        {

            if (word == "yes" || word == "no" || word == "maybe")

                return word;

        }



        return "";

    }

}


