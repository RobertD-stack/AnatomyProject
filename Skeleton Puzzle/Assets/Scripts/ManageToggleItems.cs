using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
/* *** SUMMARY *** 

*/

public class ManageToggleItems : MonoBehaviour
{
    //We can just get a list of the slot objects
    public GameObject[] objects;
    public GameObject togglePrefab;
    public GameObject listParent;

    //We need a dictionary for the image icons
    public List<Sprite> partImages = new List<Sprite>();

    public Dictionary<string, Sprite> imageDictionary = new Dictionary<string, Sprite>();

    public List<GameObject> menuItemList = new List<GameObject>(); // List of menu items

    // Toggle all to toggle all slots off
    public Toggle toggleAll;

    public GameObject inputField; // Search field

    string searchText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        objects = GameObject.FindGameObjectsWithTag("Slots");
        // Instantiate a toggle for each slot object
        int i = 0;
        foreach (GameObject obj in objects)
        {
            imageDictionary.Add(obj.name, partImages[i]);

            GameObject toggleObj = Instantiate(togglePrefab);
            toggleObj.name = obj.name; // We have to do this for when search for the object
            // set the object's parent to list parent so that it gets added to the list
            toggleObj.transform.SetParent(listParent.transform, false);
            toggleObj.GetComponentInChildren<TextMeshPro>().text = obj.name;
            Image img = toggleObj.transform.Find("Icon").GetComponentInChildren<Image>();

            img.sprite = imageDictionary[obj.name];

            Toggle m_Toggle = toggleObj.GetComponentInChildren<Toggle>();
            m_Toggle.onValueChanged.AddListener(delegate
            {
                ToggleValueChanged(obj);
            });

            menuItemList.Add(toggleObj);

            i++;
        }

        //Set toggleAll to false at the start - if triggered turn all the checks off

        toggleAll.onValueChanged.AddListener(delegate
        {
            ToggleAll(toggleAll);

        });

        // Listen if the search field changes
        inputField.GetComponent<TMP_InputField>().onValueChanged.AddListener(delegate { ValueChangeCheck(); });
    }


    void searchMenuItems(string searchValue)
    {

        // Then we need to build the new filtered list
        // Build filtered list
        List<GameObject> filteredParts = new List<GameObject>();
        for (int i = 0; i < menuItemList.Count; i++)

        {
            if (!menuItemList[i].name.ToLower().Contains(searchValue.ToLower()))
            {
                menuItemList[i].SetActive(false);
            }
            else
            {
                menuItemList[i].SetActive(true);
            }
        }

    }

    // Executes when the search event listener is called
    void ValueChangeCheck()
    {
        searchText = inputField.GetComponent<TMP_InputField>().text;

        // Debug.Log(searchText);

        searchMenuItems(searchText);
    }



    void ToggleValueChanged(GameObject obj)
    {
        // If the object is active then just toggle it inactive
        if (obj.activeSelf)
        {
            obj.SetActive(false);
        }
        else
        {
            obj.SetActive(true);
        }
    }

void ToggleAll(Toggle toggleAll)
{
    bool newState = toggleAll.isOn;

    // Set to opposite state
    foreach (GameObject obj in objects)
    {
        obj.SetActive(newState);
    }


    foreach (GameObject togObj in menuItemList)
    {
        Toggle m_Toggle = togObj.GetComponentInChildren<Toggle>();
        m_Toggle.SetIsOnWithoutNotify(newState); // Avoid triggering listeners when the individual toggles are toggled
    }
}
    
}
