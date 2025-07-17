using UnityEngine;
using UnityEngine.UI;

/* *** SUMMARY *** 

*/


public class buttonTest : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(testMessage);
    }

    void OnMouseDown()
    {
        Debug.Log("Mouse Button clicked down");
    }


    public void testMessage()
    {
        Debug.Log("Test button was clicked");
    }
}
