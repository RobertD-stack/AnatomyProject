using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using System.Collections.Generic;
using TMPro;
using System.IO; 
using System;


[System.Serializable]
public class UnityAndGeminiKey
{
    public string key;
}



[System.Serializable]
public class InlineData
{
    public string mimeType;
    public string data;
}

// Text-only part
[System.Serializable]
public class TextPart
{
    public string text;
}

// Image-capable part
[System.Serializable]
public class ImagePart
{
    public string text;
    public InlineData inlineData;
}

[System.Serializable]
public class TextContent
{
    public string role;
    public TextPart[] parts;
}

[System.Serializable]
public class TextCandidate
{
    public TextContent content;
}

[System.Serializable]
public class TextResponse
{
    public TextCandidate[] candidates;
}

[System.Serializable]
public class ImageContent
{
    public string role;
    public ImagePart[] parts;
}

[System.Serializable]
public class ImageCandidate
{
    public ImageContent content;
}

[System.Serializable]
public class ImageResponse
{
    public ImageCandidate[] candidates;
}


// For text requests
[System.Serializable]
public class ChatRequest
{
    public TextContent[] contents;
    public TextContent system_instruction;
}

[System.Serializable]
public class GeminiStringEvent : UnityEvent<string> { }

public enum LlmBackend
{
    Gemini,
    Ollama
}


public enum OllamaModel
{
    [InspectorName("Anatomy Tutor Fast (1.3 GB)")]
    AnatomyTutorFast,
    [InspectorName("Anatomy Tutor (3.8 GB)")]
    AnatomyTutor,
    [InspectorName("NCat Med Llama (3.8 GB)")]
    NcatMedLlama,
    [InspectorName("Meditron (3.8 GB)")]
    Meditron,
    [InspectorName("MedLlama2 (3.8 GB)")]
    MedLlama2,
    [InspectorName("Llama 3.2 (2.0 GB)")]
    Llama3_2,
    [InspectorName("Llama 3.2 1B (1.3 GB)")]
    Llama3_2_1B,
    [InspectorName("Qwen 2.5 3B (1.9 GB)")]
    Qwen2_5_3B,
    [InspectorName("Custom (other)")]
    Custom
}

[System.Serializable]
public class OllamaChatMessage
{
    public string role;
    public string content;
}

[System.Serializable]
public class OllamaChatResponse
{
    public string model;
    public OllamaChatMessage message;
    public bool done;

    // Timing fields returned by Ollama (nanoseconds unless noted).
    public long total_duration;
    public long load_duration;
    public long prompt_eval_count;
    public long prompt_eval_duration;
    public long eval_count;
    public long eval_duration;
}

[System.Serializable]
public class OllamaDiagnostics
{
    public string model;
    public float totalSeconds;
    public float loadSeconds;
    public int promptEvalCount;
    public int evalCount;
    public float evalTokensPerSecond;
    public string responsePreview;
    public string capturedAtUtc;

    public static OllamaDiagnostics FromResponse(OllamaChatResponse response)
    {
        if (response == null)
            return null;

        string preview = response.message != null ? response.message.content : "";
        if (!string.IsNullOrEmpty(preview))
        {
            preview = preview.Replace("\n", " ").Trim();
            if (preview.Length > 120)
                preview = preview.Substring(0, 117) + "...";
        }

        float evalSeconds = response.eval_duration > 0 ? response.eval_duration / 1_000_000_000f : 0f;
        float tokensPerSecond = evalSeconds > 0f && response.eval_count > 0
            ? response.eval_count / evalSeconds
            : 0f;

        return new OllamaDiagnostics
        {
            model = response.model,
            totalSeconds = response.total_duration / 1_000_000_000f,
            loadSeconds = response.load_duration / 1_000_000_000f,
            promptEvalCount = (int)response.prompt_eval_count,
            evalCount = (int)response.eval_count,
            evalTokensPerSecond = tokensPerSecond,
            responsePreview = preview,
            capturedAtUtc = DateTime.UtcNow.ToString("o"),
        };
    }

    public override string ToString()
    {
        return $"model={model} total={totalSeconds:F2}s load={loadSeconds:F2}s " +
               $"promptTokens={promptEvalCount} evalTokens={evalCount} tok/s={evalTokensPerSecond:F2}";
    }
}


public class UnityAndGeminiV3: MonoBehaviour
{
    private const string educationalSuffix = " Explain this in three short sentences in an educational human anatomy context for high school children. Do not acknowledge directly that we are catering our responses to high school anatomy students.";

    [Header("LLM Backend")]
    public LlmBackend llmBackend = LlmBackend.Ollama;
    [Tooltip("Ollama base URL (run: ollama serve).")]
    public string ollamaBaseUrl = "http://localhost:11434";
    [Tooltip("Local Ollama model to use (run: ollama pull <model>).")]
    public OllamaModel ollamaModel = OllamaModel.AnatomyTutorFast;
    [Tooltip("Used when Ollama Model is set to Custom.")]
    public string customOllamaModel = "anatomy-tutor";
    [Tooltip("Seconds before an Ollama /api/chat request is aborted (0 = no limit).")]
    public int ollamaRequestTimeoutSeconds = 300;

    [Header("Prompt Mode")]
    [Tooltip("When enabled, uses the text field (e.g. microphone transcription) with system instructions and educational suffix. When disabled, uses the sample test prompt.")]
    public bool useMicrophoneInput = false;
    [Tooltip("Sent to the model when Use Microphone Input is disabled.")]
    public string sampleTestPrompt = "This is a test prompt. Respond with a short answer to ensure that the model is working correctly.";

    [Header("JSON API Configuration")]
    public TextAsset jsonApi;

    
    private string apiKey = ""; 
    private string apiEndpoint = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent"; // Edit it and choose your prefer model
    private string imageEndpoint = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash-exp-image-generation:generateContent"; //End point for image generation

    [Header("ChatBot Function")]
    public TMP_Text inputField;
    public TMP_Text uiText;
    [Tooltip("Fires once each time uiText is set from a model reply (wire to TextToSpeech.NotifyModelResponse).")]
    public GeminiStringEvent onModelResponseText;
    public string botInstructions;
    private TextContent[] chatHistory;


    [Header("Prompt Function")]
    public string prompt = "";

    [Header("Image Prompt Function")]
    public string imagePrompt = "";
    public Material skyboxMaterial; 

    [Header("Media Prompt Function")]
    // Receives files with a maximum of 20 MB
    public string mediaFilePath = "";
    public string mediaPrompt = "";
    public enum MediaType
    {
        Video_MP4 = 0,
        Audio_MP3 = 1,
        PDF = 2,
        JPG = 3,
        PNG = 4
    }
    public MediaType mimeType = MediaType.Video_MP4;
    
    [Header("Debug")]
    [Tooltip("Logs the exact request format sent to Gemini (API key redacted; large base64 truncated).")]
    public bool logGeminiRequests = true;
    [Tooltip("Logs Ollama timing/token diagnostics from the latest response.")]
    public bool logOllamaDiagnostics = true;

    [Header("Ollama Diagnostics")]
    [Tooltip("Populated after each successful Ollama /api/chat response.")]
    public OllamaDiagnostics lastOllamaDiagnostics;
    [Tooltip("Full text from the latest Ollama reply (used by batch eval runners).")]
    public string lastModelResponse;
    [HideInInspector]
    public bool lastOllamaRequestSucceeded;
    [HideInInspector]
    public bool suppressModelResponseEvents;

    private const string PromptSystemInstruction =
        "You are a helpful anatomy educator. Do not repeat or echo the user's words. Answer the question or request directly with your own explanation. Never start by restating what the user said.";

    public string GetMimeTypeString()
    {
        switch (mimeType)
        {
            case MediaType.Video_MP4:
                return "video/mp4";
            case MediaType.Audio_MP3:
                return "audio/mp3";
            case MediaType.PDF:
                return "application/pdf";
            case MediaType.JPG:
                return "image/jpeg";
            case MediaType.PNG:
                return "image/png";
            default:
                return "error";
        }
    }

    // Send prompt request to Gemini if the prmopt is not null
    void Start()
    {
        EnsureOllamaLogCsvFileName();
        UnityAndGeminiKey jsonApiKey = JsonUtility.FromJson<UnityAndGeminiKey>(jsonApi.text);
        apiKey = jsonApiKey.key;
        chatHistory = new TextContent[] { };
        if (HasUserPrompt(prompt)) { StartCoroutine(SendPromptRequestToGemini(prompt)); }
        ;
        if (HasUserPrompt(imagePrompt)) { StartCoroutine(SendPromptRequestToGeminiImageGenerator(imagePrompt)); }
        ;
        if (HasUserPrompt(mediaPrompt) && mediaFilePath != "") { StartCoroutine(SendPromptMediaRequestToGemini(mediaPrompt, mediaFilePath)); }
        ;

        // inputField.onEndEdit.AddListener(OnInputChanged);
    }

    public void SubmitPrompt(string promptText)
    {
        Debug.Log("[UnityAndGeminiV3] Microphone Input: " + promptText);
        if (!HasUserPrompt(promptText))
        {
            return;
        } else {
            StartCoroutine(SendPromptRequestToOllama(promptText));
        }
    }



    public IEnumerator SendPromptRequestToGemini(string promptText)
    {
        promptText = ResolvePromptForRequest(promptText);
        if (!HasUserPrompt(promptText))
        {
            yield break;
        }

        if (llmBackend == LlmBackend.Ollama)
        {
            yield return SendPromptRequestToOllama(promptText);
            yield break;
        }

        Debug.Log("Sending to Gemini: " + promptText);
        string url = $"{apiEndpoint}?key={apiKey}";
     
        string escapedText = EscapeJsonString(promptText);
        string jsonData = useMicrophoneInput
            ? "{\"systemInstruction\": {\"parts\": [{\"text\": \"" + EscapeJsonString(PromptSystemInstruction) + "\"}]}, \"contents\": [{\"parts\": [{\"text\": \"" + escapedText + "\"}]}]}"
            : "{\"contents\": [{\"parts\": [{\"text\": \"" + escapedText + "\"}]}]}";
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);

        LogGeminiRequest(
            "Prompt",
            url,
            jsonData,
            $"Mode: {(useMicrophoneInput ? "Microphone" : "Sample test")}\nPrompt: {promptText}\nSystem instruction: {(useMicrophoneInput ? PromptSystemInstruction : "(none)")}");

        // Create a UnityWebRequest with the JSON data
        using (UnityWebRequest www = new UnityWebRequest(url, "POST")){

            www.timeout = 20;
            www.uploadHandler = new UploadHandlerRaw(jsonToSend);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success) {
                Debug.LogError(www.error);
                if (!string.IsNullOrEmpty(www.downloadHandler.text))
                    Debug.LogError("Response: " + www.downloadHandler.text);
            } else {
                Debug.Log("Request complete!");
                TextResponse response = JsonUtility.FromJson<TextResponse>(www.downloadHandler.text);
                if (response.candidates.Length > 0 && response.candidates[0].content.parts.Length > 0)
                    {
                        //This is the response to your request
                        string text = response.candidates[0].content.parts[0].text;
                        Debug.Log(text);
                        // Update my UI text with my response
                        uiText.text = text;
                        onModelResponseText?.Invoke(text);
                    }
                else
                {
                    Debug.Log("No text found.");
                }
            }
        }
    }

    public IEnumerator SendPromptRequestToOllama(string promptText)
    {
        // Determine whether we are using the microphone input or the sample test prompt
        promptText = ResolvePromptForRequest(promptText);
        if (!HasUserPrompt(promptText))
        {
            lastOllamaRequestSucceeded = false;
            yield break;
        }

        lastOllamaRequestSucceeded = false;

        string modelName = GetOllamaModelName();
        string url = ollamaBaseUrl.TrimEnd('/') + "/api/chat";
        string jsonData = useMicrophoneInput
            ? "{\"model\":\"" + modelName + "\",\"stream\":false,\"messages\":[" +
              "{\"role\":\"system\",\"content\":\"" + EscapeJsonString(PromptSystemInstruction) + "\"}," +
              "{\"role\":\"user\",\"content\":\"" + EscapeJsonString(promptText) + "\"}" +
              "]}"
            : "{\"model\":\"" + modelName + "\",\"stream\":false,\"messages\":[" +
              "{\"role\":\"user\",\"content\":\"" + EscapeJsonString(promptText) + "\"}" +
              "]}";

        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);

        Debug.Log("[UnityAndGeminiV3] Sending to Ollama: " + promptText);
        LogGeminiRequest(
            "Ollama",
            url,
            jsonData,
            $"Mode: {(useMicrophoneInput ? "Microphone" : "Sample test")}\nModel: {modelName}\nPrompt: {promptText}");

        using (UnityWebRequest www = new UnityWebRequest(url, "POST"))
        {
            www.uploadHandler = new UploadHandlerRaw(jsonToSend);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            if (ollamaRequestTimeoutSeconds > 0)
                www.timeout = ollamaRequestTimeoutSeconds;

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                lastModelResponse = "";
                lastOllamaDiagnostics = null;
                lastOllamaRequestSucceeded = false;
                Debug.LogError(www.error);
                if (!string.IsNullOrEmpty(www.downloadHandler.text))
                    Debug.LogError("Response: " + www.downloadHandler.text);
            }
            else
            {
                lastOllamaRequestSucceeded = true;
                Debug.Log("Ollama request complete!");
                Debug.Log("Raw response: " + www.downloadHandler.text);
                OllamaChatResponse response = JsonUtility.FromJson<OllamaChatResponse>(www.downloadHandler.text);
                lastOllamaDiagnostics = OllamaDiagnostics.FromResponse(response);
                if (logOllamaDiagnostics && lastOllamaDiagnostics != null)
                    Debug.Log("Ollama diagnostics: " + lastOllamaDiagnostics);
                LogOllamaDiagnosticsToCsv(lastOllamaDiagnostics);

                if (response.message != null && !string.IsNullOrEmpty(response.message.content))
                {
                    string text = response.message.content;
                    Debug.Log(text);
                    lastModelResponse = text;
                    if (!suppressModelResponseEvents)
                    {
                        if (uiText != null)
                            uiText.text = text;
                        onModelResponseText?.Invoke(text);
                    }
                }
                else
                {
                    lastModelResponse = "";
                    Debug.Log("No text found in Ollama response.");
                }
            }
        }
    }

    private static string EscapeJsonString(string value)
    {
        if (value == null) return "";
        return value
            .Replace("\\", "\\\\")
            .Replace("\"", "\\\"")
            .Replace("\n", "\\n")
            .Replace("\r", "\\r")
            .Replace("\t", "\\t");
    }

    public void SendChat()
    {
        string userMessage = inputField.text;
        if (HasUserPrompt(userMessage))
        {
            StartCoroutine(SendChatRequestToGemini(userMessage));
        }
    }

    private IEnumerator SendChatRequestToGemini(string newMessage)
    {
        if (!HasUserPrompt(newMessage))
        {
            yield break;
        }
        newMessage = WithEducationalContext(newMessage);

        string url = $"{apiEndpoint}?key={apiKey}";
     
        TextContent userContent = new TextContent
        {
            role = "user",
            parts = new TextPart[]
            {
                new TextPart { text = newMessage }
            }
        };

        TextContent instruction = new TextContent
        {
            parts = new TextPart[]
            {
                new TextPart {text = botInstructions}
            }
        }; 

        List<TextContent> contentsList = new List<TextContent>(chatHistory);
        contentsList.Add(userContent);
        chatHistory = contentsList.ToArray(); 

        ChatRequest chatRequest = new ChatRequest { contents = chatHistory, system_instruction = instruction };

        string jsonData = JsonUtility.ToJson(chatRequest);

        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);

        LogGeminiRequest(
            "Chat",
            url,
            jsonData,
            $"User message: {newMessage}\nBot instructions: {botInstructions}\nHistory turns: {chatHistory.Length}");

        // Create a UnityWebRequest with the JSON data
        using (UnityWebRequest www = new UnityWebRequest(url, "POST")){
            www.uploadHandler = new UploadHandlerRaw(jsonToSend);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success) {
                Debug.LogError(www.error);
            } else {
                Debug.Log("Request complete!");
                TextResponse response = JsonUtility.FromJson<TextResponse>(www.downloadHandler.text);
                if (response.candidates.Length > 0 && response.candidates[0].content.parts.Length > 0)
                    {
                        //This is the response to your request
                        string reply = response.candidates[0].content.parts[0].text;
                        TextContent botContent = new TextContent
                        {
                            role = "model",
                            parts = new TextPart[]
                            {
                                new TextPart { text = reply }
                            }
                        };

                        Debug.Log(reply);
                        //This part shows the text in the Canvas
                        uiText.text = reply;
                        onModelResponseText?.Invoke(reply);
                        //This part adds the response to the chat history, for your next message
                        contentsList.Add(botContent);
                        chatHistory = contentsList.ToArray();
                    }
                else
                {
                    Debug.Log("No text found.");
                }
             }
        }  
    }


    private IEnumerator SendPromptRequestToGeminiImageGenerator(string promptText)
    {
        if (!HasUserPrompt(promptText))
        {
            yield break;
        }
        promptText = WithEducationalContext(promptText);
        string url = $"{imageEndpoint}?key={apiKey}";
        
        // Create the proper JSON structure with model specification
        string jsonData = $@"{{
            ""contents"": [{{
                ""parts"": [{{
                    ""text"": ""{promptText}""
                }}]
            }}],
            ""generationConfig"": {{
                ""responseModalities"": [""Text"", ""Image""]
            }}
        }}";

        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);

        LogGeminiRequest("Image", url, jsonData, $"Image prompt: {promptText}");

        // Create a UnityWebRequest with the JSON data
        using (UnityWebRequest www = new UnityWebRequest(url, "POST"))
        {
            www.uploadHandler = new UploadHandlerRaw(jsonToSend);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success) 
            {
                Debug.LogError(www.error);
            } 
            else 
            {
                Debug.Log("Request complete!");
                Debug.Log("Full response: " + www.downloadHandler.text); // Log full response for debugging
                
                // Parse the JSON response
                try 
                {
                    ImageResponse response = JsonUtility.FromJson<ImageResponse>(www.downloadHandler.text);
                    
                    if (response.candidates != null && response.candidates.Length > 0 && 
                        response.candidates[0].content != null && 
                        response.candidates[0].content.parts != null)
                    {
                        foreach (var part in response.candidates[0].content.parts)
                        {
                            if (!string.IsNullOrEmpty(part.text))
                            {
                                Debug.Log("Text response: " + part.text);
                            }
                            else if (part.inlineData != null && !string.IsNullOrEmpty(part.inlineData.data))
                            {
                                // This is the base64 encoded image data
                                byte[] imageBytes = System.Convert.FromBase64String(part.inlineData.data);
                                
                                // Create a texture from the bytes
                                Texture2D tex = new Texture2D(2, 2);
                                tex.LoadImage(imageBytes);
                                byte[] pngBytes = tex.EncodeToPNG();
                                string path = Application.persistentDataPath + "/gemini-image.png";
                                File.WriteAllBytes(path, pngBytes);
                                Debug.Log("Saved to: " + path);
                                Debug.Log("Image received successfully!");

                                // Load the saved image back as Texture2D
                                string imagePath = Path.Combine(Application.persistentDataPath, "gemini-image.png");
                                
                                Texture2D panoramaTex = new Texture2D(2, 2);
                                panoramaTex.LoadImage(File.ReadAllBytes(imagePath));

                                Texture2D properlySizedTex = ResizeTexture(panoramaTex, 1024, 512);
                                
                                // Apply to a panoramic skybox material
                                if (skyboxMaterial != null)
                                {
                                    // Switch to panoramic shader
                                    skyboxMaterial.shader = Shader.Find("Skybox/Panoramic");
                                    skyboxMaterial.SetTexture("_MainTex", properlySizedTex);
                                    DynamicGI.UpdateEnvironment();
                                    Debug.Log("Skybox updated with panoramic image!");
                                }
                                else
                                {
                                    Debug.LogError("Skybox material not assigned!");
                                }

                                // Another approach but might cause distorsion

                                
                                // Texture2D savedTex = new Texture2D(2, 2);
                                // savedTex.LoadImage(File.ReadAllBytes(path));

                                // // Convert to Cubemap (simplified approach - may distort)
                                // Cubemap newCubemap = new Cubemap(savedTex.width, TextureFormat.RGBA32, false);
                                // for (int i = 0; i < 6; i++)
                                // {
                                //     newCubemap.SetPixels(savedTex.GetPixels(), (CubemapFace)i);
                                // }
                                // newCubemap.Apply();

                                // // Apply to skybox
                                // if (skyboxMaterial != null)
                                // {
                                //     skyboxMaterial.SetTexture("_Tex", newCubemap);
                                //     DynamicGI.UpdateEnvironment();
                                //     Debug.Log("Skybox updated with new image!");
                                // }                            

                            }
                        }
                    }
                    else
                    {
                        Debug.Log("No valid response parts found.");
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("JSON Parse Error: " + e.Message);
                }
            }
        }
    }


    Texture2D ResizeTexture(Texture2D source, int newWidth, int newHeight)
    {
        RenderTexture rt = RenderTexture.GetTemporary(newWidth, newHeight);
        Graphics.Blit(source, rt);
        Texture2D result = new Texture2D(newWidth, newHeight);
        RenderTexture.active = rt;
        result.ReadPixels(new Rect(0, 0, newWidth, newHeight), 0, 0);
        result.Apply();
        RenderTexture.ReleaseTemporary(rt);
        return result;
    }

    private string WithEducationalContext(string baseText)
    {
        if (string.IsNullOrEmpty(baseText))
        {
            return baseText;
        }

        if (baseText.EndsWith(educationalSuffix))
        {
            return baseText;
        }

        return baseText + educationalSuffix;
    }

    private string ResolvePromptForRequest(string promptText)
    {
        if (!useMicrophoneInput)
        {
            return sampleTestPrompt;
        }

        if (inputField != null && !string.IsNullOrWhiteSpace(inputField.text))
        {
            promptText = inputField.text;
        }

        return WithEducationalContext(promptText);
    }

    private string GetOllamaModelName()
    {
        switch (ollamaModel)
        {
            case OllamaModel.AnatomyTutorFast: return "anatomy-tutor-fast";
            case OllamaModel.AnatomyTutor: return "anatomy-tutor";
            case OllamaModel.NcatMedLlama: return "ncatmedllama";
            case OllamaModel.Meditron: return "meditron";
            case OllamaModel.MedLlama2: return "medllama2";
            case OllamaModel.Llama3_2: return "llama3.2";
            case OllamaModel.Llama3_2_1B: return "llama3.2:1b";
            case OllamaModel.Qwen2_5_3B: return "qwen2.5:3b";
            case OllamaModel.Custom: return customOllamaModel;
            default: return "anatomy-tutor-fast";
        }
    }

    private IEnumerator SendPromptMediaRequestToGemini(string promptText, string mediaPath)
    {
        if (!HasUserPrompt(promptText))
        {
            yield break;
        }
        // promptText = WithEducationalContext(promptText);
        promptText = "This is a test prompt. Respond with a short answer to ensure that the model is working correctly.";
        Debug.Log("Prompt text: " + promptText);
        // Read video file and convert to base64
        byte[] mediaBytes = File.ReadAllBytes(mediaPath);
        string base64Media = System.Convert.ToBase64String(mediaBytes);

        string url = $"{apiEndpoint}?key={apiKey}";

        string mimeTypeMedia = GetMimeTypeString();



        string jsonBody = $@"
        {{
        ""contents"": [
            {{
            ""parts"": [
                {{
                ""text"": ""{promptText}""
                }},
                {{
                ""inline_data"": {{
                    ""mime_type"": ""{mimeTypeMedia}"",
                    ""data"": ""{base64Media}""
                }}
                }}
            ]
            }}
        ]
        }}";


        // Serialize the request into JSON
        // string jsonData = JsonUtility.ToJson(jsonBody);

        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonBody);

        LogGeminiRequest(
            "Media",
            url,
            jsonBody,
            $"Prompt: {promptText}\nMedia path: {mediaPath}\nMIME type: {mimeTypeMedia}\nMedia bytes: {mediaBytes.Length}");

        // Create and send the request
        using (UnityWebRequest www = new UnityWebRequest(url, "POST"))
        {
            www.uploadHandler = new UploadHandlerRaw(jsonToSend);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success) 
            {
                Debug.LogError(www.error);
                Debug.LogError("Response: " + www.downloadHandler.text);
            } 
            else 
            {
                Debug.Log("Request complete!");
                TextResponse response = JsonUtility.FromJson<TextResponse>(www.downloadHandler.text);
                if (response.candidates.Length > 0 && response.candidates[0].content.parts.Length > 0)
                {
                    string text = response.candidates[0].content.parts[0].text;
                    Debug.Log(text);
                }
                else
                {
                    Debug.Log("No text found.");
                }
            }
        }
    }

    private bool HasUserPrompt(string text)
    {
        return !string.IsNullOrWhiteSpace(text);
    }

    private void LogGeminiRequest(string requestKind, string url, string jsonBody, string notes = null)
    {
        if (!logGeminiRequests)
        {
            return;
        }

        string message =
            $"[Gemini Request: {requestKind}]\n" +
            "  Method: POST\n" +
            "  Content-Type: application/json\n" +
            $"  URL: {RedactApiKeyFromUrl(url)}\n" +
            $"  Payload size: {jsonBody.Length} characters\n";

        if (!string.IsNullOrEmpty(notes))
        {
            message += "  Notes:\n";
            foreach (string line in notes.Split('\n'))
            {
                message += $"    {line}\n";
            }
        }

        message += "  JSON body:\n" + SanitizeRequestBodyForLog(jsonBody);
        Debug.Log(message);
    }

    private string RedactApiKeyFromUrl(string url)
    {
        return string.IsNullOrEmpty(apiKey) ? url : url.Replace(apiKey, "***REDACTED***");
    }

    private static string SanitizeRequestBodyForLog(string jsonBody)
    {
        const string dataMarker = "\"data\":\"";
        int searchFrom = 0;

        while (searchFrom < jsonBody.Length)
        {
            int markerIndex = jsonBody.IndexOf(dataMarker, searchFrom, StringComparison.Ordinal);
            if (markerIndex < 0)
            {
                break;
            }

            int valueStart = markerIndex + dataMarker.Length;
            int valueEnd = jsonBody.IndexOf('"', valueStart);
            if (valueEnd < 0)
            {
                break;
            }

            int valueLength = valueEnd - valueStart;
            if (valueLength > 120)
            {
                string preview = jsonBody.Substring(valueStart, 80);
                string replacement = dataMarker + preview + $"...[base64 truncated, {valueLength} chars total]\"";
                jsonBody = jsonBody.Substring(0, markerIndex) + replacement + jsonBody.Substring(valueEnd + 1);
                searchFrom = markerIndex + replacement.Length;
            }
            else
            {
                searchFrom = valueEnd + 1;
            }
        }

        return jsonBody;
    }

    // Log diagnotics to CSV

    [Header("AI Data Logging")]
    [Tooltip("Full path to python.exe, or \"python\" if it is on PATH when Unity starts.")]
    public string pythonExecutable = "python";
    [Tooltip("CSV filename under Assets/Data/. Set automatically on Start if empty.")]
    public string ollamaLogCsvFileName = "";

    private static string DataDirectory =>
        Path.Combine(Application.dataPath, "Data");

    private static void EnsureDataDirectory()
    {
        Directory.CreateDirectory(DataDirectory);
    }

    public void EnsureOllamaLogCsvFileName()
    {
        if (string.IsNullOrWhiteSpace(ollamaLogCsvFileName))
            ollamaLogCsvFileName = "ai_data_" + DateTime.UtcNow.ToString("yyyyMMdd_HHmmss") + ".csv";
    }

    private string OllamaLogCsvPath =>
        Path.Combine(DataDirectory, ollamaLogCsvFileName);

    void LogOllamaDiagnosticsToCsv(OllamaDiagnostics diagnostics)
    {
        if (diagnostics == null)
            return;

        EnsureOllamaLogCsvFileName();
        EnsureDataDirectory();

        string scriptsDir = Path.Combine(Application.dataPath, "Scripts");
        string scriptPath = Path.Combine(scriptsDir, "logAIData.py");

        if (!File.Exists(scriptPath))
        {
            Debug.LogError("[logAIData] Script not found: " + scriptPath);
            return;
        }

        // Match append_test_results argument order (InvariantCulture keeps decimals as 1.23 not 1,23)
        string arguments = string.Format(
            System.Globalization.CultureInfo.InvariantCulture,
            "\"{0}\" {1} {2} {3} \"{4}\" {5} {6} \"{7}\"",
            scriptPath,
            diagnostics.totalSeconds,
            diagnostics.loadSeconds,
            diagnostics.evalTokensPerSecond,
            diagnostics.model ?? "",
            diagnostics.promptEvalCount,
            diagnostics.evalCount,
            OllamaLogCsvPath
        );

        var startInfo = new System.Diagnostics.ProcessStartInfo
        {
            FileName = pythonExecutable,
            Arguments = "-u " + arguments,
            WorkingDirectory = scriptsDir,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
        };

        Debug.Log($"[logAIData] Running: {pythonExecutable} {startInfo.Arguments}");

        try
        {
            using (System.Diagnostics.Process process = System.Diagnostics.Process.Start(startInfo))
            {
                if (process == null)
                {
                    Debug.LogError("[logAIData] Failed to start Python process (Process.Start returned null).");
                    return;
                }

                string stdout = process.StandardOutput.ReadToEnd();
                string stderr = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (!string.IsNullOrEmpty(stdout))
                    Debug.Log("[logAIData] " + stdout.Trim());

                if (process.ExitCode != 0)
                {
                    Debug.LogError(
                        "[logAIData] Python failed (exit " + process.ExitCode + ").\n" +
                        (string.IsNullOrEmpty(stderr) ? "(no stderr output)" : stderr.Trim()));
                }
                else if (!string.IsNullOrEmpty(stderr))
                {
                    Debug.LogWarning("[logAIData] stderr: " + stderr.Trim());
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("[logAIData] Failed to run Python: " + ex);
        }
    }
    


}



