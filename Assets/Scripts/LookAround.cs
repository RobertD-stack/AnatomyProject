using UnityEngine;
/* *** SUMMARY *** 

*/

public class LookAround : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    float xRotation = 0f;
    float yRotation = 0f;

    public GameObject globalVariables;

    void Start()
    {
        Vector3 euler = transform.localEulerAngles;
        xRotation = euler.x > 180 ? euler.x - 360 : euler.x;
        yRotation = euler.y;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        if (Cursor.lockState != CursorLockMode.Locked)
            return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        yRotation += mouseX;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);

        // Reset Camera to initial position
        // if (Input.GetKeyDown(KeyCode.C) && !globalVariables.GetComponent<IsTyping>().typing)
        // {
        //     StartCoroutine(ResetCameraRotation());
        // }
    }
    // System.Collections.IEnumerator ResetCameraRotation()
    // {

    //     while (Quaternion.Angle(transform.rotation, initialRotation) > 0.1f)
    //     {
    //         transform.rotation = Quaternion.Lerp(transform.rotation, initialRotation, Time.deltaTime * 20f);
    //         yield return null; // Wait for the next frame
    //     }

    //     transform.rotation = initialRotation; // Snap to exact rotation
    // }
}
