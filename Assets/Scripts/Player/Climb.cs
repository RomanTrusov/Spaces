using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Climb : MonoBehaviour
{

    //====================
    [Header("Refrences")]
    public Rigidbody playerRigidBody;
    public Collider playerCollider;
    public PlayerMovement playerMovement;
    public LayerMask whatIsClimable;
    public Transform orientation;
    public Transform climbPoint;

    [Header("Setting")]
    public float climbForce;
    public float climbCd;
    float climbCdTimer;

    RaycastHit rayTop;
    RaycastHit rayBottom;


    //====================
    private void FixedUpdate()
    {
        //if player not grounded
        if(!playerMovement.grounded)
        {

            //=== V2.0 Check if climb is needed by two Raycast Hit Vector anngles
            if (Physics.Raycast(playerRigidBody.position + new Vector3(0f, -0.5f, 0f), orientation.forward, out rayBottom, 1f, whatIsClimable) &&
                Physics.Raycast(playerRigidBody.position + new Vector3(0f, 0.5f, 0f) + orientation.forward, -orientation.up, out rayTop, 1f, whatIsClimable) && 
                Vector3.Angle(orientation.forward, rayTop.normal) <= 110) // to avoid climbing while jumping on upward stairs
            {
                StartClimb();
            }
        }

        if (climbCdTimer > 0) climbCdTimer -= Time.deltaTime;
    }

    //====================
    private void StartClimb() //Climbing V2 - without stopping player while and after climbing
    {
        if(climbCdTimer <= 0)
        {
            // activate climbing: no collision
            playerMovement.climbing = true;
            // zero to Y current velocity
            playerRigidBody.velocity = Vector3.Scale(playerRigidBody.velocity, new Vector3(0f,0f,0f));
            // add velocity to climb
            playerRigidBody.AddForce(orientation.up * climbForce * 2.5f + orientation.forward * climbForce * 0.5f, ForceMode.Impulse);
            // stop climb after delay
            Invoke(nameof(EndClimb), climbCd);
        }
        
    }

    //====================
    private void EndClimb()
    {
        // decrease Y velocity on exit climbing
        playerRigidBody.velocity = Vector3.Scale(playerRigidBody.velocity,new Vector3(1f,0f,1f));
        //reset climb timer
        climbCdTimer = climbCd;
        // end climb
        playerMovement.climbing = false;
        //activate collider
        playerMovement.collider.enabled = true;

    }

}
