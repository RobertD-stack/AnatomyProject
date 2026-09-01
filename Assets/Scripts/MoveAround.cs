using UnityEngine;
/* *** SUMMARY *** 

*/

public class MoveAround : MonoBehaviour
{
    Rigidbody rb;
    public float speed = 200f;

    Vector3 initialPosition;
    public GameObject globalVariables;

    void Start()
    {
        initialPosition = gameObject.transform.position;
        globalVariables = GameObject.FindWithTag("GlobalVariables");

    }
    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.position;
        pos.y = 14.14f;
        transform.position = pos;
        Rigidbody rb = GetComponent<Rigidbody>();
        if (!globalVariables.GetComponent<IsTyping>().typing)
        {
            if (Input.GetKey(KeyCode.A))
            {
                rb.linearVelocity = Vector3.zero;
                rb.AddForce(gameObject.transform.right * -speed * Time.deltaTime, ForceMode.Impulse);
                
            }
            if (Input.GetKey(KeyCode.D))
            {
                rb.linearVelocity = Vector3.zero;

                rb.AddForce(gameObject.transform.right * speed * Time.deltaTime, ForceMode.Impulse);
                
            }
            if (Input.GetKey(KeyCode.W))
            {
                rb.linearVelocity = Vector3.zero;

                rb.AddForce(gameObject.transform.forward * speed * Time.deltaTime, ForceMode.Impulse);
                
            }
            if (Input.GetKey(KeyCode.S))
            {
                rb.linearVelocity = Vector3.zero;


                rb.AddForce(gameObject.transform.forward * -speed * Time.deltaTime, ForceMode.Impulse);
                

            }
            if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.S) &&!Input.GetKey(KeyCode.D)) 
            {
                rb.linearVelocity = Vector3.zero;
            }  
        }


        // Reset Camera to initial position
        if (Input.GetKeyDown(KeyCode.C) && !globalVariables.GetComponent<IsTyping>().typing)
        {
            StartCoroutine(ResetPosition());
        }
        System.Collections.IEnumerator ResetPosition()
        {

            while (Vector3.Distance(gameObject.transform.position, initialPosition) > 0.1f)
            {
                gameObject.transform.position = Vector3.Lerp(transform.position, initialPosition, Time.deltaTime * 20f);
                yield return null; // Wait for the next frame
            }

            transform.position = initialPosition; // Snap to exact rotation
        }
    }
}
