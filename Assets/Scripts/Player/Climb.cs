using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Climb : MonoBehaviour
{
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

    RaycastHit rayBottom;

    RaycastHit rayTop;

    private void Update()
    {
        //if player not grounded
        if(!playerMovement.grounded)
        {
            //if bottom + and top -
            if (Physics.Raycast(playerRigidBody.position - new Vector3(0f,0f,0f), orientation.forward, out rayBottom, 1f, whatIsClimable) &&
                !Physics.Raycast(playerRigidBody.position + new Vector3(0f, 1f, 0f), orientation.forward, out rayTop, 2f, whatIsClimable))
            {
                StartClimb();
            }
        }

        if (climbCdTimer > 0) climbCdTimer -= Time.deltaTime;
    }

    private void StartClimb()
    {
        if(climbCdTimer <= 0)
        {
            // activate climbing
            playerMovement.climbing = true;
            // zero to current velocity
            playerRigidBody.velocity = Vector3.zero;
            // add velocity to climb
            playerRigidBody.AddForce(orientation.up * climbForce * 1.5f + orientation.forward * climbForce, ForceMode.Impulse);
            // stop climb after delay
            Invoke(nameof(EndClimb), climbCd);
        }
        
    }


    private void EndClimb()
    {
        // zero to current velocity
        playerRigidBody.velocity = Vector3.zero;
        //reset climb timer
        climbCdTimer = climbCd;
        // end climb
        playerMovement.climbing = false;
        //activate collider
        playerMovement.collider.enabled = true;

    }

}
