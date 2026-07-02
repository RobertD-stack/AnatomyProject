using UnityEngine;

public enum VR
{
    On,
    Off
}

public class ToggleVR : MonoBehaviour
{
    [Header("VR Enabled")]
    public GameObject[] vrEnabled;

    [Header("VR Disabled")]
    public GameObject[] vrDisabled;

    public VR VRToggle;

    void Awake()
    {
        bool vrOn = VRToggle == VR.On;
        SetActiveForAll(vrEnabled, vrOn);
        SetActiveForAll(vrDisabled, !vrOn);
    }

    static void SetActiveForAll(GameObject[] objects, bool active)
    {
        if (objects == null)
            return;

        foreach (GameObject obj in objects)
        {
            if (obj != null)
                obj.SetActive(active);
        }
    }
}
