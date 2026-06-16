using UnityEngine;
using UnityEngine.UI;

public class ToggleVRMenuItems : MonoBehaviour
{

    public GameObject globalGameObject;
    void Awake()
    {
        globalGameObject = GameObject.FindGameObjectWithTag("GlobalVariables");
    }
    void Start()
    {
        if (globalGameObject.GetComponent<ToggleVR>() != null)
        {
            if (globalGameObject.GetComponent<ToggleVR>().VRToggle == VR.On)
            {

                gameObject.GetComponent<Button>().enabled = true;

            }
            else
            {
                gameObject.GetComponent<Button>().enabled = false;
            }
        }
        else
        {
            Debug.LogWarning("toggle vr not found");
        }

    }


}
