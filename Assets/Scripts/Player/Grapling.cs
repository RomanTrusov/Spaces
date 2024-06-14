using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] private Slider _cooldownSlider; //line component


    [Header("Grappling")]
    public float maxGrapplingDistance;
    public float grappleDelayTime; // for the start function, not a cooldown
    public float overshootYaxis;
    public float sphereCastRadius;
    public bool isGrappleOnHold; // is grappling requres holding ab utton
    public float grapplingOnHoldForce = 1f;
    private int yVelocityMoodifier = 1; // modifier that goes 0 when grapple enemy to not uplift player
    [SerializeField] private float _cooldown = 3f;
    
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
    private float _cooldownTimer;

    //if player was grappling the enemy
    //private bool isGrappedEnemy;
    private GameObject grappedEnemy;

    public GrapplingStates state;

    public bool CooldownFinished => _cooldownTimer >= _cooldown;

    public enum GrapplingStates
    {
        nothing,
        wall,
        enemy
    }

    private void Start()
    {
        // get access to the player movement script
        pm = GetComponent<PlayerMovement>();
        // get player rigid body
        PlayerRB = GetComponent<Rigidbody>();
        
        _cooldownTimer = _cooldown;
    }

    private void Update()
    {
        if (!grappling)
            IterateTimer();
        
        if (!CooldownFinished)
            return;
        
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

    private void IterateTimer()
    {
        _cooldownTimer += Time.deltaTime;

        var normalizedValue = _cooldownTimer / _cooldown;
        if (normalizedValue < 1)
            _cooldownSlider.value = normalizedValue;
        else
            _cooldownSlider.value = 0;
    }

    private void FixedUpdate()
    {
        //grappling on holdind a button
        if (isGrappleOnHold)
        {
            //add force
            if (Input.GetKey(grapplingKey) && grapplePoint != Vector3.zero) ExecuteGrappleWhlieHolding();
        }
    }

    private void LateUpdate()
    {
        //if grapple - update start position of the line
        if (grappling)
        {
            lr.SetPosition(0, gunTip.position);
        }

        //grapped enemy !!change to state enemy
        if (state == GrapplingStates.enemy)
        {
            //follow grappling hook to the enemy
            grapplePoint = grappedEnemy.transform.position;
            lr.SetPosition(1, grapplePoint);
        }

        //activate draw line in later update
        if (isDrawLineNeeded)
        {
            DrawLine(lr);
        }
    }

    //--------------------------------------------------
    //GRAPPLE FUNCTIONS START HERE
    //--------------------------------------------------

    //Start Grapple whlie pressing function
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


    //Start Grapple whlie holding function
    private void StartGrappleWhileHolding()
    {
        //if cooldown - return
        if (grapplingCdTimer > 0) return;

        //grappling starts
        grappling = true;
        yVelocityMoodifier = 1;

        //--------GRAPPLE STATES
        //throw raycast to get grappling point and state
        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, maxGrapplingDistance))
        {
            //if hitted enemy
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                state = GrapplingStates.enemy;
                grappedEnemy = hit.transform.gameObject;
                grappedEnemy.GetComponent<EnemyBehaviourDrone>().state = EnemyBehaviourDrone.EnemyStates.grapped;
                pm.grappleSpeedModifier = 0; //do not let to move player to get hin to the enemy directly
                yVelocityMoodifier = 0; // 0 to not uplift player above enemy
                pm.GetComponent<Rigidbody>().velocity = Vector3.zero; // stop player
            }
            // if hitted ground
            else if (hit.transform.gameObject.layer == LayerMask.NameToLayer("whatIsGround"))
            {
                pm.grappleSpeedModifier = 0.2f;
                state = GrapplingStates.wall;
            }
        } else
        // if raycast hit nothing - throw spherecast
        {
            if (Physics.SphereCast(cam.position, sphereCastRadius, cam.forward, out hit, maxGrapplingDistance))
            {
                //if hitted enemy
                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                {
                    state = GrapplingStates.enemy;
                    grappedEnemy = hit.transform.gameObject;
                    grappedEnemy.GetComponent<EnemyBehaviourDrone>().state = EnemyBehaviourDrone.EnemyStates.grapped;
                    pm.grappleSpeedModifier = 0; //do not let to move player to get hin to the enemy directly
                    yVelocityMoodifier = 0; // 0 to not uplift player above enemy
                    pm.GetComponent<Rigidbody>().velocity = Vector3.zero; // stop player
                }
                // if hitted ground
                else if (hit.transform.gameObject.layer == LayerMask.NameToLayer("whatIsGround"))
                {
                    pm.grappleSpeedModifier = 0.2f;
                    state = GrapplingStates.wall;
                }
            }
            else
            { //if hitted nothing
                state = GrapplingStates.nothing;
                // if missed make grapple point 000
                grapplePoint = Vector3.zero;
                //stop grapple function
                Invoke(nameof(StopGrapple), grappleDelayTime);
            }
        }

        if (state != GrapplingStates.nothing)
        {
            //save grapple point from raycast
            grapplePoint = hit.point;
            // set signal to draw the line
            isDrawLineNeeded = true;
            // Y velocity to 0 in grappling start
            PlayerRB.velocity = Vector3.Scale(PlayerRB.velocity, new Vector3(1f, 0, 1f));

            lr.SetPosition(1, grapplePoint);
        }

        
    }


    //This method draws the line in time
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

    //Execute grapple while press once
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

    //Execute grapple while holding
    private void ExecuteGrappleWhlieHolding()
    {
        pm.activeGrapple = true;// ++set it while holding

        //get direction to the grapple point
        Vector3 destination = (grapplePoint - transform.position).normalized;
        //move player to the grapple point
        PlayerRB.AddForce(destination * grapplingOnHoldForce,ForceMode.Force);
        //adding more force to grappled enemy
        if (state == GrapplingStates.enemy) PlayerRB.AddForce((destination * grapplingOnHoldForce) / 3, ForceMode.Force);
        //regulate upward force depends on Y of destination // yVelocityMoodifier 0 if grapped enemy
        if (destination.y > -0.5f)
        {
            PlayerRB.AddForce(Vector3.up * grapplingOnHoldForce / 3 * yVelocityMoodifier, ForceMode.Force);
        }
        else
        {
            PlayerRB.AddForce(Vector3.up * grapplingOnHoldForce / -3 * yVelocityMoodifier, ForceMode.Force);
        }

        //if enemy grappled enemy dead - stop grapple
        if (grappedEnemy && grappedEnemy.GetComponent<EnemyBehaviourDrone>().state == EnemyBehaviourDrone.EnemyStates.dead)
        {
            StopGrapple();
        }
    }

    //Stop Grapple
    public void StopGrapple()
    {
        pm.activeGrapple = false;
        // stop grappling
        grappling = false;
        //reset enemy grapple
        state = GrapplingStates.nothing;
        //reset enemy state
        if (grappedEnemy && grappedEnemy.GetComponent<EnemyBehaviourDrone>().enemyHealth != 0) grappedEnemy.GetComponent<EnemyBehaviourDrone>().state = EnemyBehaviourDrone.EnemyStates.alert;
        //reset enemygrapple object
        grappedEnemy = null;
        // reset cooldown
        grapplingCdTimer = grapplingCd;
        //deactivate line
        lr.enabled = false;
        //reset grapple point
        grapplePoint = Vector3.zero;
        //reset control speed
        pm.grappleSpeedModifier = 1f;
        // reset Y velocity while grapple
        yVelocityMoodifier = 1;
        // reset point 1 position
        lr.SetPosition(1, lr.GetPosition(0));

        _cooldownTimer = 0;
    }
}
