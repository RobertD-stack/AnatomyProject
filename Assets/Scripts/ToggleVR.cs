using System.Collections;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Management;

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

        if (!vrOn)
            XRSettings.gameViewRenderMode = GameViewRenderMode.None;
    }

    void Start()
    {
        if (VRToggle != VR.On)
            StartCoroutine(StopXRAndUseDesktopCamera());
    }

    IEnumerator StopXRAndUseDesktopCamera()
    {
        XRManagerSettings manager = XRGeneralSettings.Instance != null
            ? XRGeneralSettings.Instance.Manager
            : null;

        if (manager != null)
        {
            float elapsed = 0f;
            while (elapsed < 8f && !manager.isInitializationComplete)
            {
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            if (manager.activeLoader != null)
            {
                manager.StopSubsystems();
                manager.DeinitializeLoader();
            }
        }

        XRSettings.enabled = false;
        XRSettings.gameViewRenderMode = GameViewRenderMode.None;
        ConfigureDesktopCameras();
    }

    static void ConfigureDesktopCameras()
    {
        Camera[] cameras = Camera.allCameras;
        for (int i = 0; i < cameras.Length; i++)
        {
            Camera cam = cameras[i];
            cam.stereoTargetEye = StereoTargetEyeMask.None;
            cam.enabled = true;
        }
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
