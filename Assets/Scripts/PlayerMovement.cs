using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;

    public float speed = 10f;
    public float crouchSpeed = 6f;
    public float gravity = -9.81f * 2;
    public float jumpHeight = 1.5f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    Vector3 velocity;
    bool isGrounded;

    private float originalHeight;
    public float crouchHeight = 1f;

    void Start()
    {
        originalHeight = controller.height; // Store the original standing height
    }

    // Update is called once per frame
    void Update()
    {
        //checking if we hit the ground to reset our falling velocity, otherwise we will fall faster the next time
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        //right is the red Axis, foward is the blue axis
        Vector3 move = transform.right * x + transform.forward * z;

        // **Crouch Handling (Hold Shift to crouch)**
        if (Input.GetKey(KeyCode.LeftShift)) // hold Shift to crouch
        {
            controller.height = crouchHeight;
            controller.Move(move * crouchSpeed * Time.deltaTime); // move at reduced speed
        }
        else
        {
            controller.height = originalHeight;
            controller.Move(move * speed * Time.deltaTime);
        }

        //check if the player is on the ground so he can jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            //the equation for jumping
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
