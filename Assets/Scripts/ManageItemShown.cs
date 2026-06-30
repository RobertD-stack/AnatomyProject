using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using System.IO;
using UnityEngine.AddressableAssets;
using System.Linq;


public class ManageItemShown : MonoBehaviour
{
    public GameObject menuIcon;
    public GameObject menuItem;
    public GameObject[] slotObjects;
    public GameObject spawnParent;

    public AssetLoadMethod assetLoadMethod;
    public Items itemList;

    public GameObject globalVariables;


    public int currentIndex = 0;

    const string ObjectIconsPath = "C:/Users/rgdewitty/Documents/GitHub/AnatomyProject/Skeleton Puzzle/Assets/Resources/ObjectIcons/";


    public SpawnMenuItem smi;
    public ToggleVRMenuItems tvmi;
    public GameObject previousSlotObject;

    public Material defaultSlotMaterial;
    public Material highlightedSlotMaterial;

    public TextMeshProUGUI itemNameText;
    public Sprite defaultSprite;
    
    


    void Awake()
    {
        smi = menuItem.AddComponent<SpawnMenuItem>();
        tvmi = menuItem.AddComponent<ToggleVRMenuItems>();
    }

    void Start()
    {

        slotObjects = GameObject.FindGameObjectsWithTag("Slots");
        Debug.Log($"ManageItemShown: Found {slotObjects?.Length ?? 0} objects with tag 'Slots'");

        if (assetLoadMethod == AssetLoadMethod.FilePath)
        {
            string path = "Assets/Resources/Edited Human Parts/itemDescriptions.json";
            if (File.Exists(path))
            {
                string existingJson = File.ReadAllText(path);
                itemList = JsonUtility.FromJson<Items>(existingJson);
            }
        }

        // Initial Instantiation — start at -1 so first increment lands on slot 0
        currentIndex = -1;
        NextMenuItem();
    }

    public void ShowNextItem(bool increment = true)
    {
        if (slotObjects == null || slotObjects.Length == 0)
        {
            Debug.LogWarning("ManageItemShown: No slot objects to advance.");
            return;
        }

        Debug.Log($"ManageItemShown: showing next item");
        NextMenuItem(increment);
    }

    public void NextMenuItem(bool increment = true)
    {
        if (previousSlotObject != null && defaultSlotMaterial != null)
        {
            Renderer previousRenderer = previousSlotObject.GetComponent<Renderer>();
            if (previousRenderer != null)
                previousRenderer.material = defaultSlotMaterial;
        }

        if (increment)
        {
            currentIndex++;
            if (currentIndex >= slotObjects.Length)
                currentIndex = 0;
        }
        else
        {
            currentIndex--;
            if (currentIndex < 0)
                currentIndex = slotObjects.Length - 1;
        }

        GameObject slot = slotObjects[currentIndex];
        Renderer slotRenderer = slot.GetComponent<Renderer>();
        if (slotRenderer != null && highlightedSlotMaterial != null)
            slotRenderer.material = highlightedSlotMaterial;

        previousSlotObject = slot;
        Item item = itemList.bodyParts.FirstOrDefault(x => x.name == slot.name);
        string itemName = item == null ? "No item" : item.name;
        if (item == null){
            Debug.LogError("No Item: An entry was not found for " + slot.name);
        }
        else{
            Debug.Log("Found Item: " + itemName);
        }
        // menuItem.GetComponentInChildren<TextMeshProUGUI>().text = itemName;
        menuItem.name = itemName;


        smi.menuItemName = menuItem.name;

        itemNameText.text = itemName;
        smi.slot = slot;
        smi.spawnParent = spawnParent;
        BodyPartGroupType group = globalVariables.GetComponent<BodyGroup>().selectedGroup;
        string prefix = group == BodyPartGroupType.Skeleton ? "Skeleton Combined" : group == BodyPartGroupType.Muscles ? "Muscles Combined" : "Veins";

        Addressables.LoadAssetAsync<GameObject>(prefix + "[" + itemName + "]").Completed += handle =>
        {
            if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
                smi.menuItem = handle.Result;
            else
                Debug.Log("Failed to load" + itemName + " via Addressables.");
        };

        LoadMenuImage(itemName);
    }

    void LoadMenuImage(string itemName)
    {
        Image img = menuIcon.GetComponentInChildren<Image>();
        if (img == null)
        {
            Debug.LogWarning($"No Image component found on menu item for {itemName}");
            return;
        }

        string filepath = ObjectIconsPath + itemName.ToLower() + ".JPG";
        if (!File.Exists(filepath))
        {
            Debug.Log("The image at " + filepath + " does not exist!");
            menuIcon.GetComponentInChildren<Image>().sprite = defaultSprite;
            return;
        }

        byte[] fileData = File.ReadAllBytes(filepath);
        Texture2D tex = new Texture2D(2, 2);
        bool loadSuccess = tex.LoadImage(fileData);
        bool isValid = loadSuccess && tex.width > 0 && tex.height > 0;

        if (isValid)
        {
            Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            img.sprite = sprite;
        }
        else if (!loadSuccess)
            Debug.LogWarning($"Invalid image (LoadImage failed): {filepath}");
        else
            Debug.LogWarning($"Invalid image (bad dimensions {tex.width}x{tex.height}): {filepath}");
    }
}
