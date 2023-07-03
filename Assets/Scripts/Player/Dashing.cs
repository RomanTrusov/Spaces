using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dashing : MonoBehaviour
{
    // resources needed for dash
    [Header("References")]
    public Transform orientation;
    public Transform playerCam;
    private Rigidbody rb;
    private PlayerMovement pm;

    // dahs stats
    [Header("Dashing")]
    public float dashForce;
    public float dashUpwardForce;
    public float dashDuration;

    //dash cooldown
    [Header("Cooldown")]
    public float dashCd;
    private float dashCdTimer;

    //keycode
    [Header("Keycode")]
    public KeyCode dashKey = KeyCode.E;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();

    }


    private void Update()
    {
        // dash if pressed a key
        if (Input.GetKeyDown(dashKey))
            Dash();

        //reduce dash timer
        if (dashCdTimer > 0)
            dashCdTimer -= Time.deltaTime;

    }

    private void Dash()
    {
        // if cooldown - do nothing
        if (dashCdTimer > 0) return;
        // restart dash cooldown if it was used
        else dashCdTimer = dashCd;

        //activate state dashing
        pm.dashing = true;

        //GetDirection direction for the dash
        Vector3 direction = GetDirection(orientation);

        // calculate dash force
        Vector3 forceToApply = direction * dashForce + orientation.up * dashUpwardForce;
        // apply force woith a little delay
        delayForceToApply = forceToApply;
        Invoke(nameof(DelayedDashForce), 0.025f);
        // reset the dash
        Invoke(nameof(ResetDash), dashDuration);

    }

    // vector for delayed dash force
    private Vector3 delayForceToApply;

    //method for delayed dashing
    private void DelayedDashForce()
    {
        //apply it
        rb.AddForce(delayForceToApply, ForceMode.Impulse);
    }

    private void ResetDash()
    {
        pm.dashing = false;
    }

    // get directiob for the dash from inputs
    private Vector3 GetDirection(Transform orientation)
    {
        float horiontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3();
        direction = orientation.forward * verticalInput + orientation.right * horiontalInput;

        return direction.normalized;

    }

}
