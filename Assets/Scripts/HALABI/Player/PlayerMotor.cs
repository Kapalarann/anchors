using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool isGrounded;
    public float gravity = 9.8f;
    public float speed = 5f;
    public float jumpHeight = 3f;

    private bool lerpCrouch = false;
    private bool crouching = false;
    private float crouchTimer = 0f;
    private bool sprinting = false;

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
    }

    public void ProcessMove(Vector2 input)
    {
        Vector3 moveDirection = new Vector3(input.x, 0, input.y);
        controller.Move(transform.TransformDirection(moveDirection) * speed * Time.deltaTime);

        playerVelocity.y += -gravity * Time.deltaTime;

        if (isGrounded && playerVelocity.y < 0)
            playerVelocity.y = -2f;

        controller.Move(playerVelocity * Time.deltaTime);
    }

    public void Jump()
    {
        if (isGrounded)
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * 2f * gravity);
        }
    }

    public void Crouch()
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

    public void Sprint()
    {
        sprinting = !sprinting;
        speed = sprinting ? 8f : 5f;
    }
}
