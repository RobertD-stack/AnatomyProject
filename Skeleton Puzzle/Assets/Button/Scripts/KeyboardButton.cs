using UnityEngine;
using TMPro;

public class NewMonoBehaviourScript : MonoBehaviour
{
    Keyboard keyboard;
    TextMeshProUGUI buttonText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        keyboard = GetComponentInParent<Keyboard>();
        buttonText = GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText.text.Length == 1)
        {
            NameToButtonText();
            GetComponentInChildren<ButtonVR>().onRelease.AddListener(delegate { keyboard.InsertChar(buttonText.text); });

        }


    }
    public void NameToButtonText() {
        buttonText.text = gameObject.name;
    }
}
