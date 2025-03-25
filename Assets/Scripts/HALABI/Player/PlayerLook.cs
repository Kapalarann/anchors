using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
    public Camera cam;
    public Transform hand;

    Vector2 mouse;
    float VerticalRotation;

    public float maxVerticalAngle = 90f;

    public float xSensitivity = 5f;
    public float ySensitivity = 1f;
   
   void OnEnable()
   {
        cam = GameStateManager.Instance.cameraInstances[StateType.FPS].GetComponent<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
   }

    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
    }
    public void OnLook(InputValue value)
   {
        mouse = value.Get<Vector2>();
    }

    private void Update()
    {
        VerticalRotation -= mouse.y * ySensitivity;
        VerticalRotation = Mathf.Clamp(VerticalRotation, -maxVerticalAngle, maxVerticalAngle);

        cam.transform.localRotation = Quaternion.Euler(VerticalRotation, 0, 0);
        hand.localRotation = cam.transform.localRotation;

        transform.Rotate(0, mouse.x * xSensitivity * Time.deltaTime, 0);
    }

}
