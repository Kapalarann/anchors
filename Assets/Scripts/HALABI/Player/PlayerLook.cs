using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
     public float mouseSensitivity = 100f;
    public Camera cam;
     float xRotation = 0f;
     float yRotation = 0f;

     public float topClamp = -90f;
     public float bottomClamp = 90f;

    public float xSensitivity = 30f;
    public float ySensitivity = 30f;
   
   void Start()
   {
     Cursor.lockState = CursorLockMode.Locked;
   }
   void Update()
   {


   }
   public void ProcessLook(Vector2 input)
   {
        float mouseX = input.x;
        float mouseY = input.y;

        xRotation -= (mouseY * Time.deltaTime) * mouseSensitivity;
        yRotation = Mathf.Clamp(xRotation, topClamp, bottomClamp);

        cam.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);

        transform.Rotate(Vector3.up * (mouseX * Time.deltaTime) * xSensitivity);
   }

}
