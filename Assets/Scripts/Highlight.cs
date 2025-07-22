using TMPro;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;

/* *** SUMMARY *** 

*/

public enum DescriptionMethod
{
    JSON, 
    Dictionary
}

public class Highlight : MonoBehaviour
{

    public bool highlighted = false;

    public bool clicked = false;

    // On object label
    public TextMeshPro textLabel;

    public string label = "default";


    // Materials
    public Material highlightMaterial;
    public Material defaultMaterial;

    // Canvas UI Description text
    public TextMeshPro descriptionText;
    public TextMeshPro descriptionHeader;

    public Dictionary<string, string> descriptions = new Dictionary<string, string>();

    public bool hovering;
    public DescriptionMethod descriptionMethod;

    void Start()
    {
        hovering = false;
        if (descriptionMethod == DescriptionMethod.Dictionary)
        {
            // Old way of pulling descriptions is using a hard-coded dictionary
            descriptions.Add("torso", "The human torso houses and protects vital organs including the heart, lungs, and digestive system. It supports breathing, circulation, and digestion, while also serving as the central structure connecting the upper and lower body, allowing for movement and stability.");
            descriptions.Add("head", "The human head contains the brain, which controls the body's functions, thoughts, and emotions. It also houses the sensory organs—eyes, ears, nose, and mouth—enabling vision, hearing, smell, taste, and speech. The skull protects the brain and supports facial structure.");
            descriptions.Add("hands", "The human hands are versatile tools used for grasping, holding, and manipulating objects. They contain muscles, tendons, and joints that allow for precise movement and coordination. Hands play a key role in touch, communication, and performing everyday tasks.");
            descriptions.Add("left leg", "The human leg supports the body’s weight and enables movement such as walking, running, and jumping. It consists of strong bones, muscles, and joints that provide stability, balance, and mobility. The leg also aids in shock absorption and helps maintain posture.");
            descriptions.Add("right leg", "The human leg supports the body’s weight and enables movement such as walking, running, and jumping. It consists of strong bones, muscles, and joints that provide stability, balance, and mobility. The leg also aids in shock absorption and helps maintain posture.");
            descriptions.Add("head 1", "Head 1");
        }
        else if (descriptionMethod == DescriptionMethod.JSON)
        {
            // Read itemDescriptions data and add it to a dictionary 
            // TODO: Make this more effiicent
            string path = "Assets/Resources/Edited Human Parts/itemDescriptions.json";
            if (System.IO.File.Exists(path))
            {
                string existingJson = System.IO.File.ReadAllText(path);
                Debug.Log(existingJson);
                Items itemList = JsonUtility.FromJson<Items>(existingJson);

                foreach (Item bodyPart in itemList.bodyParts)
                {
                    Debug.Log("Found " + bodyPart.name + " in " + path);
                    descriptions.Add(bodyPart.name, bodyPart.description);
                }
            }
            else
            {
                Debug.Log(path + " is not a valid path!");
            }
            
        }



        //Assign materials
        highlightMaterial = Resources.Load<Material>("Highlight");
        defaultMaterial = Resources.Load<Material>("Default");



        GameObject labelObj = new GameObject(label);

        // Make it a child of this GameObject
        labelObj.transform.SetParent(transform);

        // Offset it above the object
        labelObj.transform.position = transform.position + new Vector3(1f, 1f, 0);
        // Debug.Log(labelObj.transform.position);

        // Set Rotation always forward
        labelObj.transform.rotation = Quaternion.LookRotation(Vector3.forward);

        //Add the TextMeshPro component
        TextMeshPro textLabel = labelObj.AddComponent<TextMeshPro>();

        // Set label text and appearance
        textLabel.text = label;
        textLabel.fontSize = 3;
        textLabel.alignment = TextAlignmentOptions.Center;
        textLabel.color = Color.white;

        textLabel.enabled = false;


    }
    //We use update() to determine whether the mouse clicks elsewhere and to determine whether to update item visual
    void Update()
    {

        // Check if hovering

        hovering = GetMousePosition();

        if (highlighted && Input.GetMouseButtonDown(0))
        {
            bool onObj = GetMousePosition();
            highlighted = onObj;
            clicked = onObj;
            ToggleMaterial(highlighted);
            // Debug.Log("Toggling false material");
        }


    }

    bool GetMousePosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        // Check if clicked on different object
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform != transform)
            {
                return false;
            }
        }
        else
        {
            return false;

        }
        return true;
    }

    void OnMouseEnter()
    {
        HighlightObject();
    }
    void OnMouseExit()
    {


        if (clicked)
        {
            return;
        }


        UnHighlightObject();


    }

    void OnMouseDown()
    {
        FocusObject();
    }

    //Highlight changes the object texture
    public void ToggleMaterial(bool highlighted)
    {
        if (highlighted)
        {
            gameObject.GetComponent<Renderer>().material = highlightMaterial;

        }
        else
        {
            gameObject.GetComponent<Renderer>().material = defaultMaterial;

        }
    }

    public void HighlightObject()
    {
        highlighted = true;
        ToggleMaterial(highlighted);
        Debug.Log("Hovering");
    }


    public void UnHighlightObject()
    {
        Debug.Log("Unhighlighting " + gameObject.name);
        highlighted = false;
        ToggleMaterial(highlighted);

    }

    public void FocusObject()
    {
        // If clicked we change focus for description objects
        descriptionText = GameObject.FindWithTag("DescriptionText").GetComponent<TextMeshPro>();
        descriptionText.text = descriptions[label];

        descriptionHeader = GameObject.FindWithTag("DescriptionHeader").GetComponent<TextMeshPro>();
        descriptionHeader.text = label;

        highlighted = true;
        clicked = true;
        ToggleMaterial(highlighted);
        Debug.Log("Highlighted");
    }







}

[System.Serializable]
public class Item
{
    public string name;
    public string description;
}
[System.Serializable]
public class Items
{
    public List<Item> bodyParts;
}