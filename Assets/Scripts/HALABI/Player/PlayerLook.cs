using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
    public Camera cam;
    public Transform hand;

    private float mouseX;

    public float xSensitivity = 5f;
   
   void OnEnable()
   {
        if(cam == null) cam = GameStateManager.Instance.cameraInstances[StateType.FPS].GetComponent<Camera>();
        GetComponentInChildren<Bow>().playerCamera = cam;
        Cursor.lockState = CursorLockMode.Locked;
   }

    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
    }
    public void OnLook(InputValue value)
    {
        mouseX = value.Get<Vector2>().x;
    }

    private void Update()
    {
        transform.Rotate(0, mouseX * xSensitivity, 0);
        hand.rotation = cam.transform.rotation;
    }

}
