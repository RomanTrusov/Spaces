using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grapling : MonoBehaviour
{
    [Header("References")]
    private PlayerMovement pm;
    private Rigidbody PlayerRB;
    public Transform cam;
    public Transform gunTip;
    public LayerMask whatIsGrappable;
    public LayerMask enemy;
    public LineRenderer lr; //line component


    [Header("Grappling")]
    public float maxGrapplingDistance;
    public float grappleDelayTime; // for the start function, not a cooldown
    public float overshootYaxis;
    public float sphereCastRadius;
    public bool isGrappleOnHold; // is grappling requres holding ab utton
    public float grapplingOnHoldForce = 1f;

    //====== vars for drawing grapling line
    private bool isDrawLineNeeded; //check if the line should be drawn
    private bool isPredrawFinished; //is predraw actions were finished
    private Vector3 endPointStore = Vector3.zero; //to store the end point
    private float lrLength = 0; // length of the line
    private float t = 0; // timer for lerping the line

    //where to grab
    private Vector3 grapplePoint;

    [Header("Cooldown")]
    public float grapplingCd;
    private float grapplingCdTimer;

    [Header("Input")]
    public KeyCode grapplingKey = KeyCode.Mouse1;

    //is grapple
    private bool grappling;

    //if player was grappling the enemy
    private bool isGrappedEnemy;
    private GameObject grappedEnemy;

    #region Start

    private void Start()
    {
        // get access to the player movement script
        pm = GetComponent<PlayerMovement>();
        // get player rigid body
        PlayerRB = GetComponent<Rigidbody>();

    }
    #endregion Start

    #region Update
    private void Update()
    {
        // grappling on pressing a button
        if (!isGrappleOnHold)
        {
            //wait for input
            if (Input.GetKeyDown(grapplingKey)) StartGrapple();

            //reduce cooldown by time
            if (grapplingCdTimer > 0) grapplingCdTimer -= Time.deltaTime;

            //stop grapple with Space or Grappling key UP
            if (pm.activeGrapple == true && Input.GetKeyDown(KeyCode.Space))
            {
                StopGrapple();
            }
        }
        
        //grappling on holdind a button
        if (isGrappleOnHold)
        {
            //wait for input
            if (Input.GetKeyDown(grapplingKey)) StartGrappleWhileHolding();
            //reduce cooldown by time
            if (grapplingCdTimer > 0) grapplingCdTimer -= Time.deltaTime;
            //stop grapple with Space or Grappling key UP
            if (pm.activeGrapple == true && Input.GetKeyUp(grapplingKey))
            {
                StopGrapple();
            }
        }

    }
    #endregion Update

    private void FixedUpdate()
    {
        //grappling on holdind a button
        if (isGrappleOnHold)
        {
            //add force
            if (Input.GetKey(grapplingKey) && grapplePoint != Vector3.zero) ExecuteGrappleWhlieHolding();
        }
    }

    #region Late Update functions
    private void LateUpdate()
    {
        //if grapple - update start position of the line
        if (grappling)
        {
            lr.SetPosition(0, gunTip.position);
        }

        //grapped enemy
        if (isGrappedEnemy)
        {
            //follow grappling hook to the enemy
            grapplePoint = grappedEnemy.transform.position;
            lr.SetPosition(1, grapplePoint);
        }

        //acticate draw line in later update
        if (isDrawLineNeeded)
        {
            DrawLine(lr);
        }
    }
    #endregion Late Update functions

    //--------------------------------------------------
    //THREE GRAPPLE FUNCTIONS START HERE
    //--------------------------------------------------

    #region Start Grapple whlie pressing function
    private void StartGrapple()
    {
        //if cooldown
        if (grapplingCdTimer > 0) return; 

        //grappling starts
        grappling = true; 

        //throw spherecast to get grappling point
        RaycastHit hit;
        //raycast throw first
        if (Physics.Raycast(cam.position,cam.forward, out hit, maxGrapplingDistance, whatIsGrappable))
        {
            //save grapple point from raycast
            grapplePoint = hit.point;
            //execute grapple function
            Invoke(nameof(ExecuteGrapple), grappleDelayTime);
            isDrawLineNeeded = true; // set signal to draw the line

        }
        else if (Physics.Raycast(cam.position, cam.forward, out hit, maxGrapplingDistance, enemy))
        {
            //save grapple point from raycast
            grapplePoint = hit.point;
            //execute grapple function
            Invoke(nameof(ExecuteGrapple), grappleDelayTime);
            isDrawLineNeeded = true; // set signal to draw the line
        }
        else 
        {
            //spherecast throw then
            if (Physics.SphereCast(cam.position, sphereCastRadius, cam.forward, out hit, maxGrapplingDistance, whatIsGrappable))
            {
                //save grapple point from raycast
                grapplePoint = hit.point;
                //execute grapple function
                Invoke(nameof(ExecuteGrapple), grappleDelayTime);
                isDrawLineNeeded = true; // set signal to draw the line

            }
            else if (Physics.SphereCast(cam.position, sphereCastRadius, cam.forward, out hit, maxGrapplingDistance, enemy))
            {
                //save grapple point from raycast
                grapplePoint = hit.point;
                //execute grapple function
                Invoke(nameof(ExecuteGrapple), grappleDelayTime);
                isDrawLineNeeded = true; // set signal to draw the line
            }
            else //all above must be a raycast, all below must be a spherecast
            {
                // if hit is not grappable or far away - store the max distance point
                grapplePoint = cam.position + cam.forward * maxGrapplingDistance;
                //stop grapple function
                Invoke(nameof(StopGrapple), grappleDelayTime);
            }
        }

        lr.SetPosition(1, grapplePoint);
    }

    #endregion

    #region Start Grapple whlie holding function
    private void StartGrappleWhileHolding()
    {
        //if cooldown
        if (grapplingCdTimer > 0) return;

        //grappling starts
        grappling = true;

        //throw spherecast to get grappling point
        RaycastHit hit;
        //raycast throw first
        if (Physics.Raycast(cam.position, cam.forward, out hit, maxGrapplingDistance, enemy))
        {
            //save grapple point from raycast
            grapplePoint = hit.point;
            isDrawLineNeeded = true; // set signal to draw the line
            // Y velocity to 0 in grappling start
            PlayerRB.velocity = Vector3.Scale(PlayerRB.velocity, new Vector3(1f,0,1f));
            //grapped to the enemy
            isGrappedEnemy = true;
            //get enemy object on hit
            grappedEnemy = hit.transform.gameObject;

        }
        else if (Physics.Raycast(cam.position, cam.forward, out hit, maxGrapplingDistance, whatIsGrappable))
        {
            //save grapple point from raycast
            grapplePoint = hit.point;
            isDrawLineNeeded = true; // set signal to draw the line
            // Y velocity to 0 in grappling start
            PlayerRB.velocity = Vector3.Scale(PlayerRB.velocity, new Vector3(1f, 0, 1f));
        }
        else
        {
            //spherecast throw then
            if (Physics.SphereCast(cam.position, sphereCastRadius, cam.forward, out hit, maxGrapplingDistance, enemy))
            {
                //save grapple point from raycast
                grapplePoint = hit.point;
                isDrawLineNeeded = true; // set signal to draw the line
                // Y velocity to 0 in grappling start
                PlayerRB.velocity = Vector3.Scale(PlayerRB.velocity, new Vector3(1f, 0, 1f));
                //grapped to the enemy
                isGrappedEnemy = true;
                //get enemy object on hit
                grappedEnemy = hit.transform.gameObject;


            }
            else if (Physics.SphereCast(cam.position, sphereCastRadius, cam.forward, out hit, maxGrapplingDistance, whatIsGrappable))
            {
                //save grapple point from raycast
                grapplePoint = hit.point;
                isDrawLineNeeded = true; // set signal to draw the line
                // Y velocity to 0 in grappling start
                PlayerRB.velocity = Vector3.Scale(PlayerRB.velocity, new Vector3(1f, 0, 1f));
            }
            else //all above must be a raycast, all below must be a spherecast
            {
                // if missed make grapple point 000
                grapplePoint = Vector3.zero;
                //stop grapple function
                Invoke(nameof(StopGrapple), grappleDelayTime);
            }
        }

        lr.SetPosition(1, grapplePoint);
    }

    #endregion

    #region This method draws the line in time
    private void DrawLine(LineRenderer lr)
    {
        if (!isPredrawFinished) //do predraw
        {
            endPointStore = lr.GetPosition(1); // store end point
            lrLength = Vector3.Distance(lr.GetPosition(0),lr.GetPosition(1)); //get length of the line
            lr.SetPosition(1, lr.GetPosition(0)); // move 1 point to 0 position
            lr.enabled = true; // enable the line
            isPredrawFinished = true;
        } else // do after predraw
        {
            t += 2 / lrLength; //increase lerp t
            // temporary position for poinnt 1 of the line during the Lerp function
            Vector3 tempEndPosition = Vector3.Lerp(lr.GetPosition(0),endPointStore,t);
            lr.SetPosition(1,tempEndPosition);
            if (t > 1) //if lerp end
            {
                // reload all vars
                endPointStore = lr.GetPosition(0);
                isDrawLineNeeded = false;
                lrLength = 0;
                isPredrawFinished = false;
                t = 0;
            }
        }
    }
    #endregion

    #region Execute grapple while press once
    private void ExecuteGrapple()
    {
        pm.activeGrapple = false;

        Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);

        float grapplePointRelativeYPos = grapplePoint.y - lowestPoint.y;
        float highestPointOnArc = grapplePointRelativeYPos + overshootYaxis;

        if (grapplePointRelativeYPos < 0) highestPointOnArc = overshootYaxis;

        pm.JumpToPosition(grapplePoint, highestPointOnArc);

        if (pm.grounded && grapplePoint.y < transform.position.y)
            Invoke(nameof(StopGrapple), 1f);
        else
            Invoke(nameof(StopGrapple), 1f);

    }
    #endregion Execute grapple while press once

    #region Execute grapple while holding
    private void ExecuteGrappleWhlieHolding()
    {
        pm.activeGrapple = true;// ++set it while holding

        //get direction to the grapple point
        Vector3 destination = Vector3.Normalize(grapplePoint - transform.position);

        PlayerRB.AddForce(destination * grapplingOnHoldForce,ForceMode.Force);
        PlayerRB.AddForce(Vector3.up * grapplingOnHoldForce/3,ForceMode.Force);

    }
    #endregion Execute grapple while holding

    #region Stop Grapple
    private void StopGrapple()
    {
        pm.activeGrapple = false;
        // stop grappling
        grappling = false;
        //reset enemy grapple
        isGrappedEnemy = false;
        //reset enemygrapple object
        grappedEnemy = null;
        // reset cooldown
        grapplingCdTimer = grapplingCd;
        //deactivate line
        lr.enabled = false;
        // reset point 1 position
        lr.SetPosition(1, lr.GetPosition(0));
    }
    #endregion 
}
