using UnityEngine;
using UnityEngine.UI;

public class ToggleIcon : MonoBehaviour
{
    public Sprite spriteOn;
    public Sprite spriteOff;

    public void ToggleSprite() {
        if (GetComponent<Image>().sprite == spriteOff) {
            GetComponent<Image>().sprite = spriteOn;
        } else {
            GetComponent<Image>().sprite = spriteOff;
        }
    }

    public void SetSpriteOff() {
        GetComponent<Image>().sprite = spriteOff;
    }
}
