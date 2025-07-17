using UnityEngine;

/* *** SUMMARY *** 

*/


public class DeleteItem : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        if (gameObject.GetComponent<Highlight>().hovering)
        {
            return;
        }
        if (gameObject.GetComponent<Highlight>().highlighted && Input.GetKeyDown(KeyCode.Delete))
        {
            Destroy(gameObject);
        }
    }
}
