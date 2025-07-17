using UnityEngine;
/* *** SUMMARY *** 

*/

public class RotateItem : MonoBehaviour
{
    public float rotationSpeed = 90f; // degrees per second
    public GameObject globalVariables;

    // This script should work
    void Start()
    {
        globalVariables = GameObject.FindWithTag("GlobalVariables");
    }
    void Update()
    {
        if (globalVariables.GetComponent<AllMatched>().checkAllMatched)
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                Debug.Log("All matched, rotating");

                transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                Debug.Log("All matched, rotating");

                transform.Rotate(0f, -rotationSpeed * Time.deltaTime, 0f);
            }
        }

    }
}
