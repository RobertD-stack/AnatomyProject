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
            Debug.Log("toggle vr found");
            if (globalGameObject.GetComponent<ToggleVR>().VRToggle == VR.On)
            {

                Debug.Log("VR is on");
                gameObject.GetComponent<Button>().enabled = true;

            }
            else
            {
                gameObject.GetComponent<Button>().enabled = false;
            }
        }
        else
        {
            Debug.Log("toggle vr not found");
        }

    }


}
