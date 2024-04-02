using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grapling : MonoBehaviour
{
    [Header("References")]
    private PlayerMovement pm;
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

    //===============

    private void Start()
    {
        // get access to the player movement script
        pm = GetComponent<PlayerMovement>();
    }

    //===============
    private void Update()
    {
        //wait for input
        if (Input.GetKeyDown(grapplingKey)) StartGrapple();

        //reduce cooldown by time
        if (grapplingCdTimer > 0) grapplingCdTimer -= Time.deltaTime;

        //stop grapple with Space
        if (pm.activeGrapple == true && Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("chucha");
            StopGrapple();
        }

    }

    //===============
    private void LateUpdate()
    {
        //if grapple - update start position of the line
        if (grappling)
        {
            lr.SetPosition(0, gunTip.position);
        }

        //acticate draw line in later update
        if (isDrawLineNeeded)
        {
            DrawLine(lr);
        }
    }

    //--------------------------------------------------
    //THREE GRAPPLE FUNCTIONS START HERE
    //--------------------------------------------------

    private void StartGrapple()
    {
        if (grapplingCdTimer > 0) return; //if cooldown

        grappling = true; //grappling starts

        //throw spherecast to get grappling point
        RaycastHit hit;
        if (Physics.SphereCast(cam.position,sphereCastRadius, cam.forward, out hit, maxGrapplingDistance, whatIsGrappable))
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
        else
        {
            // if hit is not grappable or far away - store the max distance point
            grapplePoint = cam.position + cam.forward * maxGrapplingDistance;
            //stop grapple function
            Invoke(nameof(StopGrapple), grappleDelayTime);
        }
        //old method with no movement
        lr.SetPosition(1, grapplePoint);
    }

    //=============== this method draws the line in time
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

    //===============
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
    //===============
    private void StopGrapple()
    {
        pm.activeGrapple = false;
        // stop grappling
        grappling = false;
        // reset cooldown
        grapplingCdTimer = grapplingCd;
        //deactivate line
        lr.enabled = false;
        // reset point 1 position
        lr.SetPosition(1, lr.GetPosition(0));
    }

}
