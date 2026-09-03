using UnityEngine;
using UnityEngine.UI;

public class SpawnItemButton : MonoBehaviour
{

    public SpawnMenuItem SpawnMenuItem;

    GameObject globalVariables;

    public bool increment = true;

    void Awake()
    {
        if (SpawnMenuItem == null)
            SpawnMenuItem = FindFirstObjectByType<SpawnMenuItem>();

        globalVariables = GameObject.FindGameObjectWithTag("GlobalVariables");
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
     void Start()
    {
        // SpawnMenuItem onSameObject = GetComponent<SpawnMenuItem>();
        // if (onSameObject != null)
        // {
        //     SpawnMenuItem = onSameObject;
        //     return;
        // }

        // Button button = GetComponent<Button>();
        // button.onClick.AddListener(OnButtonPressed);

        // if (globalVariables != null)
        // {
        //     ToggleVR toggleVR = globalVariables.GetComponent<ToggleVR>();
        //     if (toggleVR != null && toggleVR.mode != InputMode.VR)
        //         button.enabled = false;
        // }
    }

    public void OnButtonPressed()
    {
        if (SpawnMenuItem == null)
        {
            SpawnMenuItem = FindFirstObjectByType<SpawnMenuItem>();
            if (SpawnMenuItem == null)
            {
                Debug.LogWarning("SpawnItemButton: SpawnMenuItem not found in scene.");
                return;
            }
        }

        Debug.Log("SpawnItemButton: spawning item.");
        SpawnMenuItem.spawnItem();
    }
}
