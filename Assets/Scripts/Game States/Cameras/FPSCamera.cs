using UnityEngine;

public class FPSCamera : MonoBehaviour
{
    public float mouseSensitivity = 2f;
    public float maxVerticalAngle = 80f;

    private float verticalRotation = 0f;

    void Update()
    {
        // Vertical rotation (camera)
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -maxVerticalAngle, maxVerticalAngle);

        // Apply vertical rotation to the camera
        transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }

    void OnEnable()
    {
        // Hide and lock the cursor when the camera becomes active
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void OnDisable()
    {
        // Optionally, restore the cursor when the camera is disabled
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
