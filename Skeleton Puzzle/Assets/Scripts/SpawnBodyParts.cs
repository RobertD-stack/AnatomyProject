// As of June 6 this is a deprecated feature as we switch to individual part spawning on a different script

using UnityEngine;
using System.Collections.Generic;

public class SpawnBodyParts : MonoBehaviour
{
    public GameObject[] editedBodyParts;
    public GameObject[] bodyParts;
    public BoxCollider spawnBounds;
    public GameObject spawnBox;
    public Material defaultMaterial;
    public List<GameObject> displacedParts;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        // spawnAllParts();

    }

    public static Vector3 RandomPointInBounds(Bounds bounds, float start, float end, float fixedY)
    {
        // Mathf.Lerp returns the X position that is start% of the way through the bounding range
        // This is necessary to allow x values to be negative
        float xMin = Mathf.Lerp(bounds.min.x, bounds.max.x, start);
        float xMax = Mathf.Lerp(bounds.min.x, bounds.max.x, end);

        return new Vector3(
            Random.Range(xMin, xMax),
            fixedY,
            1.89f
        );
    }

    void spawnAllParts()
    {
        float fixedY = spawnBox.transform.position.y;
        float compartments = 100 / bodyParts.Length;
        for (int i = 0; i < bodyParts.Length; i++)
        {
            Vector3 startPos = RandomPointInBounds(spawnBounds.bounds, (i * compartments) / 100, ((i + 1) * compartments) / 100, fixedY);
            GameObject temp = Instantiate(bodyParts[i], startPos, bodyParts[i].transform.rotation);
            temp.GetComponent<Renderer>().material = defaultMaterial;
            temp.AddComponent<Draggable>(); // Add Draggable Property
            temp.AddComponent<Matched>(); // Add Matched Property


            Highlight label = temp.AddComponent<Highlight>(); // Add Label
            label.label = bodyParts[i].name;

            temp.AddComponent<BoxCollider>(); // Add Box Collider
            SnapToPlace snapComponent = temp.AddComponent<SnapToPlace>(); // Add Snap to Place Component Onto Spawned Objects
            snapComponent.slotObject = bodyParts[i]; // Add the slot object as an object to snap to
            displacedParts.Add(temp); // This list is accessed by JoinParts.cs




        }
        spawnBounds.enabled = false;
    }



}
