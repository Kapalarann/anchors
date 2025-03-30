using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerMotor : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool isGrounded;
    public float speed = 5f;
    public float jumpHeight = 3f;

    private bool lerpCrouch = false;
    private bool crouching = false;
    private float crouchTimer = 0f;
    private bool sprinting = false;
    private Vector3 moveDirection = Vector3.zero;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        isGrounded = controller.isGrounded;

        // Handle crouch animation
        if (lerpCrouch)
        {
            crouchTimer += Time.deltaTime;
            float p = crouchTimer / 1f; // Assuming crouch transition takes 1 second
            p *= p;
            controller.height = Mathf.Lerp(controller.height, crouching ? 1f : 2f, p);

            if (p > 1f)
            {
                lerpCrouch = false;
                crouchTimer = 0f;
            }
        }

        //movement
        controller.Move(transform.TransformDirection(moveDirection) * speed * Time.deltaTime);

        playerVelocity.y += Physics.gravity.y * Time.deltaTime;

        if (isGrounded && playerVelocity.y < 0)
            playerVelocity.y = -2f;

        controller.Move(playerVelocity * Time.deltaTime);
    }

    public void OnMovement(InputValue value)
    {
        moveDirection = new Vector3(value.Get<Vector2>().x, 0, value.Get<Vector2>().y);
    }

    public void OnJump()
    {
        if (isGrounded)
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * 2f * -Physics.gravity.y);
        }
    }

    public void OnCrouch()
    {
        if (!crouching && Physics.Raycast(transform.position, Vector3.up, 2f))
        {
            // Prevent standing up if there's an obstacle above
            return;
        }

        crouching = !crouching;
        crouchTimer = 0f;
        lerpCrouch = true;
    }

    public void OnSprint()
    {
        sprinting = !sprinting;
        speed = sprinting ? 8f : 5f;
    }
}
