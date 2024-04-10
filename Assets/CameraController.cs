using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Sensitivity of mouse movement
    public float mouseSensitivity = 100f;
    // Reference to the player body for horizontal rotation
    public Transform playerBody;
    // Vertical rotation limit
    float xRotation = 0f;

    void Start()
    {
        // Hide and lock the cursor to the center of the screen
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Apply vertical rotation (pitch) and clamp it
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Apply rotation to the camera (for looking up and down)
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Rotate the player body horizontally (yaw)
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
