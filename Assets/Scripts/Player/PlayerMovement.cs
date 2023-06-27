using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;

    // reduce sliding
    public float groundDrag;

    // for jumps
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    [SerializeField]
    bool readyToJump = true;

    //jump button
    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Ground check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    private void Start()
    {
        // get rigidbody + free rotation
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update()
    {

        //ground check with raycast down to the ground
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        MyInput();
        SpeedControl();

        // make drag
        if (grounded) rb.drag = groundDrag; else rb.drag = 0;

    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    // function for keyboard inputs

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        //if ready to jump
        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();
            // cooldown over time
            Invoke(nameof(ResetJump), jumpCooldown);
        }

    }

    //Move player function
    private void MovePlayer()
    {
        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        //add force to the rigid body in the direction with speed times 10. ForceMode.Force for better movement
        if (grounded)
        {
            rb.AddForce(moveDirection * moveSpeed * 10f, ForceMode.Force);
        }

        else if (!grounded)
        {
            rb.AddForce(moveDirection * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }
       
    }

    // limit velocity of rigid body
    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitVel.x, rb.velocity.y, limitVel.z);
        }
    }

    private void Jump()
    {
        // sure that Y velocity is 0
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        // Impulse for one time force
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

}
