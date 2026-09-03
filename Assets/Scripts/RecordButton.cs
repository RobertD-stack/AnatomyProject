using UnityEngine;
using UnityEngine.UI;
using Whisper.Samples;

/* Mouse/keyboard: OnMouseDown + BoxCollider (add collider manually in Editor).
   VR: UI Button onClick via MyMicrophoneDemo. */

public class RecordButton : MonoBehaviour
{
    public MyMicrophoneDemo microphoneDemo;

    GameObject globalVariables;

    void Awake()
    {
        if (microphoneDemo == null)
            microphoneDemo = FindFirstObjectByType<MyMicrophoneDemo>();

        globalVariables = GameObject.FindGameObjectWithTag("GlobalVariables");
    }

    void Start()
    {
        Button button = GetComponent<Button>();
        if (button == null || globalVariables == null)
            return;

        ToggleVR toggleVR = globalVariables.GetComponent<ToggleVR>();
        if (toggleVR != null && toggleVR.mode != InputMode.VR)
            button.enabled = false;
    }

    void OnMouseDown()
    {
        if (IsDragging() || microphoneDemo == null)
            return;

        microphoneDemo.OnButtonPressed();
    }

    bool IsDragging()
    {
        if (globalVariables == null)
            return false;

        IsDragging dragging = globalVariables.GetComponent<IsDragging>();
        return dragging != null && dragging.isDragging;
    }
}
