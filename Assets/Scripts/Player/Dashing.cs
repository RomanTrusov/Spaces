using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dashing : MonoBehaviour
{
    //=====================
    // resources needed for dash
    [Header("References")]
    public Camera playerCameraForFOV;
    public Transform orientation;
    public Transform playerCam;
    private Rigidbody rb;
    private PlayerMovement pm;

    // dahs stats
    [Header("Dashing")]
    public float dashForce;
    public float dashUpwardForce;
    public float dashDuration;
    public bool readyForDash;

    //dash cooldown
    [Header("Cooldown")]
    public float dashCd;
    private float dashCdTimer;

    //keycode
    [Header("Keycode")]
    public KeyCode dashKey = KeyCode.E;

    //PS for wind effects
    [Header("Dash Effects")]
    public ParticleSystem windLeft;
    public ParticleSystem windRight;
    public ParticleSystem windForward;
    public ParticleSystem windBackward;
    private ParticleSystem currentWindEffect;

    //=====================
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();

    }

    //=====================
    private void Update()
    {

        //setting ready for dash while grounded with coolsown delay to avoid double dash from ground
        if ((pm.grounded && dashCdTimer < 0.1f) || pm.activeGrapple) readyForDash = true;

        // dash if pressed a key
        if (Input.GetKeyDown(dashKey) && readyForDash) Dash();

        //reduce dash timer
        if (dashCdTimer > 0)
            dashCdTimer -= Time.deltaTime;

        

    }

    //=====================
    private void Dash()
    {
        // if cooldown - do nothing
        if (dashCdTimer > 0) return;
        // restart dash cooldown if it was used
        else dashCdTimer = dashCd;

        //make player not ready for dash while in air
        readyForDash = false;

        //activate state dashing
        pm.dashing = true;

        //GetDirection for the dash from inputs
        Vector3 direction = GetDirection(orientation);
        
        //if player stands still, direction = camera forward
        if (direction == Vector3.zero)
        {
            direction = playerCam.forward;
        }

        // calculate dash force
        Vector3 forceToApply = direction * dashForce + orientation.up * dashUpwardForce;
        
        // apply force woith a little delay
        delayForceToApply = forceToApply;
        Invoke(nameof(DelayedDashForce), 0.025f);

        //dash wind effect
        ActivateDashWind();

        // reset the dash
        Invoke(nameof(ResetDash), dashDuration);

    }

    //=====================
    // vector for delayed dash force
    private Vector3 delayForceToApply;

    //method for delayed dashing
    private void DelayedDashForce()
    {
        rb.velocity = Vector3.Scale(rb.velocity, new Vector3(1f,0f,1f));
        //apply it
        rb.AddForce(delayForceToApply, ForceMode.Impulse);
    }

    //=====================
    private void ResetDash()
    {
        pm.dashing = false;
    }

    //=====================
    // get direction for the dash from inputs
    private Vector3 GetDirection(Transform orientation)
    {
        float horiontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3();

        //if moving forward, dash towards camera, else - moving direction
        if (verticalInput == 1)
        {
            direction = playerCam.forward;
        } else direction = orientation.forward * verticalInput + orientation.right * horiontalInput;

        return direction.normalized;

    }

    private void ActivateDashWind()
    {

        // set Current wind direction
        if (Input.GetKey(KeyCode.W))
        {
            currentWindEffect = windForward;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            currentWindEffect = windLeft;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            currentWindEffect = windBackward;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            currentWindEffect = windRight;
        } else
        {
            currentWindEffect = windForward;
        }

        //enable PS and disable it with time
        currentWindEffect.gameObject.SetActive(true);

        ParticleSystem.TrailModule trails = currentWindEffect.trails;
        trails.colorOverTrail = new Color(1, 1, 1, 0.1f);
        StartCoroutine(DeactivateDashWind(currentWindEffect, trails));
    }

    IEnumerator DeactivateDashWind(ParticleSystem activePS, ParticleSystem.TrailModule trails)
    {
        Color currentColor = activePS.trails.colorOverTrail.color;
        yield return new WaitForSeconds(0.2f);
        for (float i = 1f; i >= 0; i -= 0.05f)
        {
            Color lerpColor = Color.Lerp(currentColor, new Color(1, 1, 1, 0), Time.deltaTime * 8f);
            currentColor = lerpColor;
            trails.colorOverTrail = lerpColor;
            yield return null;
        }
        //yield return new WaitForSeconds(0.2f);
        trails.colorOverTrail = new Color(1,1,1,0.1f);
        activePS.gameObject.SetActive(false);
        yield return null;
    }

}
