using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR;
using UnityEngine.XR.Management;

public enum InputMode
{
    VR,
    MouseAndKeyboard,
    GetReal3D
}

public class ToggleVR : MonoBehaviour
{
    [Header("VR")]
    public GameObject[] vrEnabled;

    [Header("Mouse And Keyboard")]
    [FormerlySerializedAs("vrDisabled")]
    public GameObject[] mouseAndKeyboardEnabled;

    [Header("GetReal3D")]
    public GameObject genericPlayer;

    [Tooltip("Select what input/interaction mode you're using.")]
    public InputMode mode = InputMode.VR;

    void Awake()
    {
        bool vrOn = mode == InputMode.VR;
        bool mouseOn = mode == InputMode.MouseAndKeyboard;
        bool getReal3DOn = mode == InputMode.GetReal3D;

        SetActiveForAll(vrEnabled, vrOn);
        SetActiveForAll(mouseAndKeyboardEnabled, mouseOn);

        if (genericPlayer != null)
            genericPlayer.SetActive(getReal3DOn);

        if (!vrOn)
            XRSettings.gameViewRenderMode = GameViewRenderMode.None;
    }

    void Start()
    {
        if (mode != InputMode.VR)
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
