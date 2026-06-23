using System.Runtime.InteropServices;
using UnityEngine;
using TMPro;

public sealed class TextToSpeech : MonoBehaviour
{
    public TMP_Text textField; // Assign in Inspector
    private string lastText = "";
    public Animator animator;
    [Tooltip("How long the speaking animation runs (seconds).")]
    public float animationDuration = 15f;
    private float animationEndTime;
    public ToggleIcon toggleIcon;

    void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
        if (animator != null) {
            animator.speed=0;
            toggleIcon.SetSpriteOff();
        }
    }

    void Update()
    {
        if (animator != null && animator.speed > 0f && Time.time >= animationEndTime)
        {
            animator.speed = 0f;
            toggleIcon.SetSpriteOff();
        }
    }

    void StartSpeech()
    {
        string newText = textField.text;
        Debug.Log($"StartSpeech entered. textNull={(newText == null)}, textLen={(newText == null ? -1 : newText.Length)}");
        if (string.IsNullOrWhiteSpace(newText))
        {
            Debug.LogWarning("StartSpeech skipped: empty/whitespace text.");
            return;
        }

        ttsrust_say(newText);
        toggleIcon.ToggleSprite();
        Debug.Log("Animation started");

        // Animator might be on a child depending on the rig setup.
        if (animator == null) animator = GetComponent<Animator>();
        if (animator == null) animator = GetComponentInChildren<Animator>();
        if (animator != null)
        {
            animator.speed = 1f;
            animationEndTime = Time.time + animationDuration;
        }
        else
        {
            Debug.LogWarning("StartSpeech: Animator reference is null; cannot start animation.");
        }
    }

    // Called by UnityAndGeminiV3 for every completed model reply.
    // This avoids relying on text changes (AI can return identical strings).
    public void NotifyModelResponse(string text)
    {
        Debug.Log($"NotifyModelResponse called. textNull={(text == null)}, textLen={(text == null ? -1 : text.Length)}");
        lastText = text ?? "";
        StartSpeech();
    }



#if !UNITY_EDITOR && (UNITY_IOS || UNITY_WEBGL)
    const string _dll = "__Internal";
#else
    const string _dll = "ttsrust";
#endif

    [DllImport(_dll)] static extern void ttsrust_say(string text);
}
