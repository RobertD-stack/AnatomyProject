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
        EnsureButtonComponents();
    }

    void EnsureButtonComponents()
    {
        if (GetComponent<BoxCollider>() == null)
        {
            BoxCollider collider = gameObject.AddComponent<BoxCollider>();
            collider.size = new Vector3(100f, 100f, 1f);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
     void Start()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonPressed);

        if (globalVariables != null)
        {
            ToggleVR toggleVR = globalVariables.GetComponent<ToggleVR>();
            if (toggleVR != null && toggleVR.VRToggle != VR.On)
                button.enabled = false;
        }
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
