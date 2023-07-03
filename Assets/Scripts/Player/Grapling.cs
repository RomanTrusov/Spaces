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
    public LineRenderer lr; //line component


    [Header("Grappling")]
    public float maxGrapplingDistance;
    public float grappleDelayTime; // for the start function, not a cooldown
    public float overshootYaxis;

    //where to grab
    private Vector3 grapplePoint;

    [Header("Cooldown")]
    public float grapplingCd;
    private float grapplingCdTimer;

    [Header("Input")]
    public KeyCode grapplingKey = KeyCode.Mouse1;

    //is grapple
    private bool grappling;

    //--------------------------------------------------

    private void Start()
    {
        // get access to the player movement script
        pm = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        //wait for input
        if (Input.GetKeyDown(grapplingKey)) StartGrapple();

        //reduce cooldown by time
        if (grapplingCdTimer > 0) grapplingCdTimer -= Time.deltaTime;
    }

    private void LateUpdate()
    {
        //if grapple - update start position of the line
        if (grappling)
        {
            lr.SetPosition(0, gunTip.position);
        }
    }

    //--------------------------------------------------
    //THREE GRAPPLE FUNCTIONS START HERE
    //--------------------------------------------------

    private void StartGrapple()
    {
        if (grapplingCdTimer > 0) return; //if cooldown

        grappling = true; //grappling starts


        //throw raycast
        RaycastHit hit;
        if(Physics.Raycast(cam.position, cam.forward,out hit, maxGrapplingDistance, whatIsGrappable))
        {
            //save grapple point from raycast
            grapplePoint = hit.point;
            //execute grapple function
            Invoke(nameof(ExecuteGrapple), grappleDelayTime);

        } else
        {
            // if hit i snot grappable or far away - store the max distance point
            grapplePoint = cam.position + cam.forward * maxGrapplingDistance;
            //stop grapple function
            Invoke(nameof(StopGrapple), grappleDelayTime);
        }
        //activate line and set end position
        lr.enabled = true;
        lr.SetPosition(1, grapplePoint);

    }

    private void ExecuteGrapple()
    {
        pm.activeGrapple = false;

        Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);

        float grapplePointRelativeYPos = grapplePoint.y - lowestPoint.y;
        float highestPointOnArc = grapplePointRelativeYPos + overshootYaxis;

        if (grapplePointRelativeYPos < 0) highestPointOnArc = overshootYaxis;

        pm.JumpToPosition(grapplePoint, highestPointOnArc);

        Invoke(nameof(StopGrapple), 1f);

    }

    private void StopGrapple()
    {
        pm.activeGrapple = false;
        // stop grappling
        grappling = false;
        // reset cooldown
        grapplingCdTimer = grapplingCd;
        //deactivate line
        lr.enabled = false;
    }

}
