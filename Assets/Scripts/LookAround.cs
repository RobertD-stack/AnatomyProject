using UnityEngine;
/* *** SUMMARY *** 

*/

public class LookAround : MonoBehaviour
{
    Quaternion initialRotation;
    float xRotation = 0f;
    float yRotation = 0f;

    public float sensitivity = 15f;
    public GameObject globalVariables;

    void Start()
    {
        initialRotation = gameObject.transform.rotation;
        globalVariables = GameObject.FindWithTag("GlobalVariables");

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            // Sync rotation with the current transform at the moment of clicking
            // Convert Euler X to signed angle (-180 to 180) to clamp correctly

            Vector3 euler = transform.localEulerAngles;
            xRotation = euler.x > 180 ? euler.x - 360 : euler.x;
            yRotation = euler.y;
            xRotation -= Input.GetAxis("Mouse Y") * Time.deltaTime * sensitivity;
            yRotation += Input.GetAxis("Mouse X") * Time.deltaTime * sensitivity;

            xRotation = Mathf.Clamp(xRotation, -90f, 90f);             // to stop the player from looking above/below 90

            transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0);

        }


        // Reset Camera to initial position
        if (Input.GetKeyDown(KeyCode.C) && !globalVariables.GetComponent<IsTyping>().typing)
        {
            StartCoroutine(ResetCameraRotation());
        }
    }
    System.Collections.IEnumerator ResetCameraRotation()
    {

        while (Quaternion.Angle(transform.rotation, initialRotation) > 0.1f)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, initialRotation, Time.deltaTime * 20f);
            yield return null; // Wait for the next frame
        }

        transform.rotation = initialRotation; // Snap to exact rotation
    }
}
