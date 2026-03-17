using System.Runtime.InteropServices;
using UnityEngine;
using TMPro;

sealed class TextToSpeech : MonoBehaviour
{
    public TMP_Text textField; // Assign in Inspector
    private string lastText = "";
    public Animator animator;
    [Tooltip("How long the speaking animation runs (seconds).")]
    public float animationDuration = 25f;
    private float animationEndTime;

    void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
        if (animator != null) animator.enabled = false;
    }

    void Start()
    {
        if (animator != null) animator.enabled = false;
    }

    void Update()
    {
        if (textField != null && textField.text != lastText)
        {
            lastText = textField.text;
            StartSpeech(lastText);
        }

        if (animator != null && animator.enabled && Time.time >= animationEndTime)
        {
            animator.enabled = false;
        }
    }

    void StartSpeech(string newText)
    {
        if (!string.IsNullOrEmpty(newText))
        {
            ttsrust_say(newText);
            if (animator != null)
            {
                bool wasOff = !animator.enabled;
                animator.enabled = true;
                // Start 15-second run only when speech starts (not reset by later text updates)
                if (wasOff)
                    animationEndTime = Time.time + animationDuration;
            }
        }
    }

#if !UNITY_EDITOR && (UNITY_IOS || UNITY_WEBGL)
    const string _dll = "__Internal";
#else
    const string _dll = "ttsrust";
#endif

    [DllImport(_dll)] static extern void ttsrust_say(string text);
}
