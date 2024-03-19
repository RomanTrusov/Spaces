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
            //if bottom + and top -
            if (Physics.Raycast(playerRigidBody.position + new Vector3(0f,-0.5f,0f), orientation.forward, out rayBottom, 1f, whatIsClimable) &&
                !Physics.Raycast(playerRigidBody.position + new Vector3(0f, 1.5f, 0f), orientation.forward, out rayTop, 2f, whatIsClimable))
            {
                StartClimb();
            }
        }

        if (climbCdTimer > 0) climbCdTimer -= Time.deltaTime;
    }

    //====================
    private void StartClimb() //Climbing V2
    {
        if(climbCdTimer <= 0)
        {
            // activate climbing
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
    /*private void StartClimb() //CLimbing via adding force to player's rigid body
    {
        if(climbCdTimer <= 0)
        {
            // activate climbing
            playerMovement.climbing = true;
            // zero to current velocity
            playerRigidBody.velocity = Vector3.zero;
            // add velocity to climb
            playerRigidBody.AddForce(orientation.up * climbForce * 2f + orientation.forward * climbForce * 0.7f, ForceMode.Impulse);
            // stop climb after delay
            Invoke(nameof(EndClimb), climbCd);
        }
        
    }*/


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
