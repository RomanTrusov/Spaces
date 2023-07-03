using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    // movespeed calculates inside, other speeds - puts manually
    private float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;
    public float dashSpeed;

    // reduce sliding
    public float groundDrag;

    // for jumps
    [Header("Jumps")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    [SerializeField]
    bool readyToJump = true;

    [Header("DoubleJump")]
    public bool readyForDoubleJump;

    //jump, sprint button
    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode reserKey = KeyCode.R;

    [Header("Ground check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    public bool grounded;

    //for sloping
    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;


    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    // states creation
    public MovementStates state;
    public enum MovementStates
    {
        walking,
        sprinting,
        dashing,
        air
    }


    public bool dashing;
    public bool activeGrapple;

    private void Start()
    {
        // get rigidbody + free rotation
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update()
    {

        //ground check with raycast down to the ground
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.15f, whatIsGround);

        MyInput();
        SpeedControl();
        StateHandler();

        // make drag
        if (grounded && !activeGrapple)
        {
            if (moveDirection != new Vector3(0,0,0))
                rb.drag = groundDrag;
            else rb.drag = 2 * groundDrag;
            rb.useGravity = true;
            ResetDoubleJump();
        }
        
        else rb.drag = 0;

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
        if (Input.GetKeyDown(jumpKey) && readyToJump)
        {
            if (grounded)
            {
                readyToJump = false;

                Jump();
                // cooldown over time
                Invoke(nameof(ResetJump), jumpCooldown);
            }
            else if (readyForDoubleJump) DoubleJump();
            

        }

        // reset players position and velocity
        if (Input.GetKey(reserKey))
        {
            rb.position = new Vector3(0, 2.5f, 0);
            rb.velocity = new Vector3(0, 0, 0);
        }

    }


    //State handler
    private void StateHandler ()
    {



        // if dashinng
         if (dashing)
        {
            state = MovementStates.dashing;
            moveSpeed = dashSpeed;
        }
        else 
        // if sprinting
        if (grounded && Input.GetKey(sprintKey))
        {
            state = MovementStates.sprinting;
            moveSpeed = sprintSpeed;
        }
        // if walking
        else if (grounded)
        {
            state = MovementStates.walking;
            moveSpeed = walkSpeed;
        }
        else
        {
            state = MovementStates.air;
            moveSpeed = walkSpeed;
        }
    }


    //Move player function
    private void MovePlayer()
    {
        if (activeGrapple) return;

        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        //If player on slope add slope speeed
        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 4f, ForceMode.Force);

            // fix for little jumps on slopees
            if (rb.velocity.y > 0)
            {
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
        } 

        //add force to the rigid body in the direction with speed times 10. ForceMode.Force for better movement
        if (grounded)
        {
            rb.AddForce(moveDirection * moveSpeed * 10f, ForceMode.Force);
        }

        //TEST reduce Y velocity while falling
        else if (!grounded)
        {
            if (rb.velocity.y < 0)
            {
                rb.AddForce(moveDirection * moveSpeed * 10f * airMultiplier - new Vector3(0, 15f, 0), ForceMode.Force);
            } else
            rb.AddForce(moveDirection * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }

        //turn off gravity when slope
        rb.useGravity = !OnSlope();


    }


    // limit velocity of rigid body
    private void SpeedControl()
    {
        //limit speed on slopes
        if (activeGrapple) return;
 

            if (OnSlope() && !exitingSlope)
            {

                if (rb.velocity.magnitude > moveSpeed)
                {
                    rb.velocity = rb.velocity.normalized * moveSpeed;
                }
            }

            else
            {
                Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

                if (flatVel.magnitude > moveSpeed)
                {
                    Vector3 limitVel = flatVel.normalized * moveSpeed;
                    rb.velocity = new Vector3(limitVel.x, rb.velocity.y, limitVel.z);
                }
            }

            
        
        
    }

    private void Jump()
    {

        exitingSlope = true;

        // sure that Y velocity is 0
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        // Impulse for one time force
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }


    private void ResetJump()
    {
        readyToJump = true;

        exitingSlope = false;
    }

    private void DoubleJump()
    {
        readyForDoubleJump = false;
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        
    }

    private void ResetDoubleJump ()
    {
        readyForDoubleJump = true;
    }


    public void JumpToPosition(Vector3 targetPosition, float trajectoryHeight)
    {
        activeGrapple = true; // activate grapple to stop motion and off speed limit
        //apply jump velocity

        velocityToSet = CalculateJumpVelocity(transform.position, targetPosition, trajectoryHeight);
        Invoke(nameof(SetVelocity), 0.1f);
    }

    // apply grappling jump after some delay
    private Vector3 velocityToSet;
    private void SetVelocity()
    {
        rb.velocity = velocityToSet;
    }

    // check if we are on slope
    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            // slope angle
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            // true if angle less that max slope angle
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    //to get slope
    private Vector3 GetSlopeMoveDirection()
    {
        // get slope move deirection normalized vi projection on the slope
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }

    //calculating grapple jump
    public Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight)
    {
        float gravity = Physics.gravity.y;
        float displacementY = endPoint.y - startPoint.y;
        Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x, 0f, endPoint.z - startPoint.z);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);
        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * trajectoryHeight / gravity) + Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity));

        return (velocityXZ + velocityY) + new Vector3(0f, 3f, 0f);

    }

}
