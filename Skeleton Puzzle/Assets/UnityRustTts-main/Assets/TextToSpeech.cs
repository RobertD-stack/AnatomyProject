using System.Runtime.InteropServices;
using UnityEngine;
using TMPro;

sealed class TextToSpeech : MonoBehaviour
{
    public TMP_Text textField; // Assign in Inspector
    private string lastText = "";

    void Update()
    {
        if (textField != null && textField.text != lastText)
        {
            lastText = textField.text;
            StartSpeech(lastText);
        }
    }

    void StartSpeech(string newText)
    {
        if (!string.IsNullOrEmpty(newText))
        {
            ttsrust_say(newText);
        }
    }

#if !UNITY_EDITOR && (UNITY_IOS || UNITY_WEBGL)
    const string _dll = "__Internal";
#else
    const string _dll = "ttsrust";
#endif

    [DllImport(_dll)] static extern void ttsrust_say(string text);
}
