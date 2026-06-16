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


public class UnityAndGeminiV3: MonoBehaviour
{
    private string lastText = "";
    private const string educationalSuffix = " Explain this in three short sentences in an educational human anatomy context for high school children. Do not acknowledge directly that we are catering our responses to high school anatomy students.";

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


    // Constantly Check if the textfield is updated
    void Update()
    {
        if (inputField != null && inputField.text != lastText)
        {
            Debug.Log("Input field text updated");
            lastText = inputField.text;
            if (HasUserPrompt(lastText))
            {
                StartCoroutine(SendPromptRequestToGemini(lastText));
            }
        }
    }
    void OnInputChanged(string newText)
    {
        if (HasUserPrompt(newText))
        {
            StartCoroutine(SendPromptRequestToGemini(newText));
        }
    }
    public IEnumerator SendPromptRequestToGemini(string promptText)
    {
        if (!HasUserPrompt(promptText))
        {
            yield break;
        }
        promptText = WithEducationalContext(promptText);
        string url = $"{apiEndpoint}?key={apiKey}";
     
        // Send the prompt as plain text. Escape for JSON so quotes in speech don't break the request.
        string escapedText = promptText.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "\\r");
        // System instruction tells Gemini not to echo the user - answer the question directly.
        string jsonData = "{\"systemInstruction\": {\"parts\": [{\"text\": \"You are a helpful anatomy educator. Do not repeat or echo the user's words. Answer the question or request directly with your own explanation. Never start by restating what the user said.\"}]}, \"contents\": [{\"parts\": [{\"text\": \"" + escapedText + "\"}]}]}";

        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);

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

    private IEnumerator SendPromptMediaRequestToGemini(string promptText, string mediaPath)
    {
        if (!HasUserPrompt(promptText))
        {
            yield break;
        }
        promptText = WithEducationalContext(promptText);
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
        Debug.Log("Sending JSON: " + jsonBody); // For debugging

        // byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);

        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonBody);


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

}



