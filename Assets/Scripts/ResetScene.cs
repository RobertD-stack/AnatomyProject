using UnityEngine;
using UnityEngine.SceneManagement;
/* *** SUMMARY *** 

*/

public class ResetScene : MonoBehaviour
{

    public GameObject globalVariables;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        globalVariables = GameObject.FindWithTag("GlobalVariables"); ;

    }

    // Listen for the reset key(r)
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && !globalVariables.GetComponent<IsTyping>().typing)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
