using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
 
    public float speed = 12f;
    public float gravity = -9.81f * 2;
    public float jumpHeight = 1.5f;
    
    // Sprint fields: hold Left Shift to add this amount to `speed` while held
    [Tooltip("Hold Left Shift to sprint (adds this amount to `speed`).")]
    public float sprintAdd = 10f;
    
    // Light toggle fields
    [Tooltip("Assign the Spot Light here (optional). If left empty, the script will try to find a GameObject named 'Spot Light'.")]
    public Light spotLight;
    public string spotLightName = "Spot Light";
 
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
 
    Vector3 velocity;
 
    bool isGrounded;

    void Start()
    {
        // If no light assigned in the Inspector, try to find the light by name
        if (spotLight == null)
        {
            GameObject found = GameObject.Find(spotLightName);
            if (found != null)
            {
                spotLight = found.GetComponent<Light>();
            }
        }
    }
 
    // Update is called once per frame
    void Update()
    {
        // Toggle the player's spot light with F
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (spotLight != null)
            {
                spotLight.enabled = !spotLight.enabled;
            }
            else
            {
                Debug.LogWarning("PlayerMovement: No spotLight assigned and GameObject named '" + spotLightName + "' not found.");
            }
        }

        //checking if we hit the ground to reset our falling velocity, otherwise we will fall faster the next time
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
 
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
 
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // right is the red Axis, forward is the blue axis
        Vector3 move = transform.right * x + transform.forward * z;

        // Sprint: hold Left Shift to temporarily add sprintAdd to speed
        float currentSpeed = speed;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed += sprintAdd;
        }

        controller.Move(move * currentSpeed * Time.deltaTime);
 
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