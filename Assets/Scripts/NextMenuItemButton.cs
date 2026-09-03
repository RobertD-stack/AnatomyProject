using UnityEngine;
using UnityEngine.UI;

/* *** SUMMARY ***
    Adds the same button components used by spawn menu items, then advances ManageItemShown.
    Mouse/keyboard: BoxCollider + OnMouseDown. VR: UI Button onClick.
*/

public class NextMenuItemButton : MonoBehaviour
{
    public ManageItemShown manageItemShown;

    GameObject globalVariables;

    public bool increment = true;

    void Awake()
    {
        if (manageItemShown == null)
            manageItemShown = FindFirstObjectByType<ManageItemShown>();

        globalVariables = GameObject.FindGameObjectWithTag("GlobalVariables");
        EnsureButtonComponents();
    }

    void Start()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonPressed);

        if (globalVariables != null)
        {
            ToggleVR toggleVR = globalVariables.GetComponent<ToggleVR>();
            if (toggleVR != null && toggleVR.mode != InputMode.VR)
                button.enabled = false;
        }
    }

    void EnsureButtonComponents()
    {
        if (GetComponent<BoxCollider>() == null)
        {
            BoxCollider collider = gameObject.AddComponent<BoxCollider>();
            collider.size = new Vector3(100f, 100f, 1f);
        }

        if (GetComponent<ToggleVRMenuItems>() == null)
            gameObject.AddComponent<ToggleVRMenuItems>();

        if (GetComponent<Button>() == null)
            gameObject.AddComponent<Button>();
    }

    void OnMouseDown()
    {
        if (IsDragging())
            return;

        OnButtonPressed();
    }

    public void OnButtonPressed()
    {
        if (manageItemShown == null)
        {
            manageItemShown = FindFirstObjectByType<ManageItemShown>();
            if (manageItemShown == null)
            {
                Debug.LogWarning("NextMenuItemButton: ManageItemShown not found in scene.");
                return;
            }
        }

        Debug.Log("NextMenuItemButton: advancing to next item.");
        manageItemShown.ShowNextItem(increment);
    }

    bool IsDragging()
    {
        if (globalVariables == null)
            return false;

        IsDragging dragging = globalVariables.GetComponent<IsDragging>();
        return dragging != null && dragging.isDragging;
    }
}
