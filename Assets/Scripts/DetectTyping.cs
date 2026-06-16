using UnityEngine;
/* *** SUMMARY *** 

*/

// On the search component, we detect whether we are typing or not
public class DetectTyping : MonoBehaviour
{

    public GameObject globalVariables;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        globalVariables = GameObject.FindWithTag("GlobalVariables"); ;

    }

    //We use update() to determine whether the mouse clicks elsewhere and to determine whether to update item visual
    void Update()
    {



        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            // Check if clicked on different object
            if (Physics.Raycast(ray, out hit))
            {
                if (!hit.collider.CompareTag("SearchBar"))
                {
                    // Debug.Log("Not a search bar!");
                    globalVariables.GetComponent<IsTyping>().setTyping(false);
                }
            }
            else
            {
                // Debug.Log("Not a search bar!");
                globalVariables.GetComponent<IsTyping>().setTyping(false);


            }
        }


    }


    void OnMouseDown()
    {
        globalVariables.GetComponent<IsTyping>().setTyping(true);
    }
    
}
