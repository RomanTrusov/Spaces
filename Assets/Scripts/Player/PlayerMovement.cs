using System.Collections;
using System.Collections.Generic;
using CameraShake;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{

    public int playerHP;
    public int playerHPMax;
    public bool isNeedToHeal;
    private bool isInvincible;
    [SerializeField]
    private Animator healAnimator;
    [SerializeField]
    private Animator damageAnimator;

    [Header("Movement")]
    // movespeed calculates inside, other speeds - puts manually
    private float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;
    public float dashSpeed;
    public float grappleSpeedModifier; //modifier that reduces speed during the grapple

    [SerializeField]
    private Vector3 velocity;

    // reduce sliding
    public float groundDrag;
    public float groundDragStopper;
    //to track grappling down from air
    public bool grapplingDown = false;

    // for jumps
    [Header("Jumps")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    [SerializeField]
    bool readyToJump = true;

    [Header("DoubleJump")]
    public bool readyForDoubleJump;
    public float doubleJumpForce;

    //jump, sprint, reset button
    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode resetKey = KeyCode.R;

    [Header("Ground check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    public bool grounded;

    //for sloping
    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    //for restart
    private Vector3 restartPosition;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    public GameObject deadSign;

    public new Collider collider;

    Rigidbody rb;

    private ICameraShake _movingShake = null;
    private Vector3 _prevMovingDirection;

    // states creation
    public MovementStates state;
    public enum MovementStates
    {
        climbing,
        walking,
        sprinting,
        dashing,
        air,
        attacked,
        dead
    }

    public bool attacked;
    public bool onSlope;
    public bool climbing;
    public bool dashing;
    public bool activeGrapple;

    private void Start()
    {

        //set restart position
        restartPosition = transform.position;

        // get rigidbody + free rotation
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        //set the default value
        grappleSpeedModifier = 1f;
}

    private void Update()
    {

        if (playerHP <= 0)
        {
            state = MovementStates.dead;
            moveSpeed = 0f;
            rb.drag = 0;
            deadSign.SetActive(true);
        }

        //was used for debugging, afraid to delete
        velocity = GetComponent<Rigidbody>().velocity;

        //check if player on slope or not
        onSlope = OnSlope();
        //ground check with raycast down to the ground
        if (readyToJump && grounded) 
            grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight + 0.15f, whatIsGround);
        else if (readyToJump && !grounded)
        {
            //sway the weapon from Sway script
            WeaponSway activateSway = GameObject.FindObjectOfType(typeof(WeaponSway)) as WeaponSway;
            activateSway.FlySway();

            //old
            //grounded = Physics.Raycast(transform.position + new Vector3(0.5f, 0f, 0f), Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
            grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight + 0.15f, whatIsGround);
        }
        
        MyInput();
        StateHandler();
        SpeedControl();
        CheckMaxHP();

        //invinvible while dashing
        if (dashing) isInvincible = true; else isInvincible = false;

        //stop grappling if grounded and graaple down
        if (grounded && grapplingDown)
        {
            grapplingDown = false;
            activeGrapple = false;
        }

        // make drag 
        
        if (attacked) rb.drag = 4;
        else if (grounded && !activeGrapple && state != MovementStates.air)
        {
            if (moveDirection != Vector3.zero)
                rb.drag = groundDrag;
            else rb.drag = groundDragStopper;
            rb.useGravity = true;
            ResetDoubleJump();
        }
        else rb.drag = 0;
        if (dashing) rb.drag = 0;

        //turn off gravity if grapple
        if (activeGrapple) rb.useGravity = false;
        else rb.useGravity = true;

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
        if (Input.GetKey(resetKey))
        {
            Scene scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.name);
        }

    }


    //State handler
    private void StateHandler()
    {
        var prevState = state;
        
        //if climb
        if (attacked)
        {
            state = MovementStates.attacked;
            grounded = false;
        }
        else if (climbing)
        {
            state = MovementStates.climbing;
            collider.enabled = false;
        }
        // if dashinng
        else if (dashing)
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
        
        CheckMovementShake(prevState);
    }

    private void CheckMovementShake(MovementStates prevState)
    {
        bool wasMoving = (prevState == MovementStates.walking || prevState == MovementStates.sprinting) && _prevMovingDirection != Vector3.zero;
        bool nowMoving = (state == MovementStates.walking || state == MovementStates.sprinting) && moveDirection != Vector3.zero;
        _prevMovingDirection = moveDirection;

        if (!wasMoving && nowMoving && _movingShake == null)
        {
            _movingShake = CameraShaker.Instance.ShakePresets.Walking();
        }
        else if (wasMoving && !nowMoving)
        {
            CameraShaker.Instance.StopPerlinShake(_movingShake);
            _movingShake = null;
        }
    }

    [ContextMenu("Walking shake")]
    public void WalkShake()
    {
        _movingShake = CameraShaker.Instance.ShakePresets.Walking();
    }
    
    [ContextMenu("Stop walking shake")]
    public void StopWalkShake()
    {
        CameraShaker.Instance.StopPerlinShake(_movingShake);
        _movingShake = null;
    }


    //Move player function
    private void MovePlayer()
    {
        float speedMultiplier = 30f; //10f default
        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        //If player on slope add slope is not negative
        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection() * moveSpeed * grappleSpeedModifier * speedMultiplier, ForceMode.Force);

        } 

        //add force to the rigid body in the direction with speed times 10. ForceMode.Force for better movement
        else if (grounded)
        {
            rb.AddForce(moveDirection * moveSpeed * grappleSpeedModifier * speedMultiplier, ForceMode.Force);
        } 
        

        //reduce Y velocity while falling
        else if (!grounded && rb.useGravity)
        {
            if (rb.velocity.y < 0)
            {
                rb.AddForce(moveDirection * moveSpeed * grappleSpeedModifier * speedMultiplier * airMultiplier - new Vector3(0, 15f, 0), ForceMode.Force);
            } else
            rb.AddForce(moveDirection * moveSpeed * grappleSpeedModifier * speedMultiplier * airMultiplier, ForceMode.Force);
        }

        //TEST if air - move down
        if (state == MovementStates.air && rb.useGravity)
        {
            rb.AddForce(orientation.up * -7f, ForceMode.Force);
        }

        //turn off gravity when slope
        if (!activeGrapple) rb.useGravity = !OnSlope();
    }


    //============================================
    // limit velocity of rigid body
    
    private void SpeedControl()
       
    {
        //limit speed on slopes
        if (activeGrapple && !attacked) return;

        if (attacked)
        {
            //rb.velocity = rb.velocity.normalized * walkSpeed * 2f;
            rb.velocity = rb.velocity.normalized * walkSpeed;
        }
        else if (OnSlope() && !exitingSlope)
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
        grounded = false;

        // sure that Y velocity is 0
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        // Impulse for one time force
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

        //sway the weapon from WeaponSway script
        WeaponSway activateSway = GameObject.FindObjectOfType(typeof(WeaponSway)) as WeaponSway;
        activateSway.JumpSway();
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
        rb.AddForce(transform.up * jumpForce * doubleJumpForce, ForceMode.Impulse);

        //sway the weapon from WeaponSway script
        WeaponSway activateSway = GameObject.FindObjectOfType(typeof(WeaponSway)) as WeaponSway;
        activateSway.JumpSway();
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
        Invoke(nameof(SetVelocity), 0.05f);
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
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight + 0.5f))
        {
            // slope angle
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            // true if angle less that max slope angle
            if (angle < maxSlopeAngle && angle != 0) return true;
        }

        return false;
    }

    //to get slope
    private Vector3 GetSlopeMoveDirection()
    {
        // get slope move deirection normalized via projection on the slope
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }

    //calculating grapple jump
    public Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight)
    {
        float gravity = Physics.gravity.y;
        float displacementY = endPoint.y - startPoint.y;

        if (endPoint.y > startPoint.y)
        {
            Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x, 0f, endPoint.z - startPoint.z);

            Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);
            Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * trajectoryHeight / gravity) + Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity));

            return (velocityXZ + velocityY) * 1.15f + new Vector3(0f, 0f, 0f);
        } else if (grounded)
        {
            Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x, 0f, endPoint.z - startPoint.z);

            Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);
            Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * trajectoryHeight / gravity) + Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity));

            return (velocityXZ * 1f + velocityY) + new Vector3(0f, 0f, 0f);
        } else
        {
            grapplingDown = true; // set this state of grappling to cancel grappling on ground
            Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x, 0f, endPoint.z - startPoint.z);

            Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);
            Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * trajectoryHeight / gravity) + Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity));

            return (velocityXZ * 1.2f + velocityY) + new Vector3(0f, (endPoint.y - startPoint.y)*0.5f, 0f);
        }
        

    }

    public void PlayerGetHit()
    {
        if (!isInvincible)
        {
            playerHP -= 1;
            damageAnimator.Play("Base Layer.Cracks",0,0);
            CameraShaker.Instance.ShakePresets.ShortShake3D(7f, 6f, 2);
        }
            
    }

    private void CheckMaxHP()
    {
        if (playerHP > 100) playerHP = 100;
    }

    //heal player
    public void HealPlayer ()
    {
        //plater no more need to heal
        isNeedToHeal = false;
        //restore hp if it's not maxed
        if (playerHP < playerHPMax)
        {
            playerHP += 1;
            // play heal animation
            healAnimator.Play("Base Layer.Heal", 0, 0);
        }
        
    }

}
