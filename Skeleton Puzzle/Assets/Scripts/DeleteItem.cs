using UnityEngine;

/* *** SUMMARY *** 

*/


public class DeleteItem : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {

        if (gameObject.GetComponent<Highlight>().clicked && Input.GetKeyDown(KeyCode.Delete))
        {
            Destroy(gameObject);
        }
    }
}
