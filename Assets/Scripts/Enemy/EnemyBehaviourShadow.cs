using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEditor;

public class EnemyBehaviourShadow : MonoBehaviour, IDamageable
{
    //[SerializeField]
    //GameObject avoidIstantDestroy;

    [Header("Player object will be set automacally")]
    [SerializeField]
    private GameObject player;
    [Header("Particles for preattack state")]
    [SerializeField]
    private ParticleSystem circleSparkles;
    [SerializeField]
    private ParticleSystem centerSegment;

    [Header("Params for laser attack")]
    [SerializeField]
    private GameObject laserBeam;
    //Timer for laser attck
    private float laserAttackTimer = 0;
    //bools for laser attck states
    private bool[] laserAttackStates = new bool[6];
    private float laserZScale;
    private float defaultLaserBeam = 500f;
    // coordinates for start and end laser
    private Vector3 pointAForlaser;
    private Vector3 pointBForlaser;
    private float laserIncreaseSize = 0;
    private float lerpTime = 0;


    public int enemyHealth;

    [SerializeField]
    float noticeDistance;
    [SerializeField]
    float alertMovingDistance; //desirable distance from player while alert
    [SerializeField]
    float enemySpeed;
    [SerializeField]
    float pushForce;
    [SerializeField]
    LayerMask playerLayer;

    RaycastHit hit;

    [SerializeField]
    public EnemyStates state;

    [SerializeField]
    float attackCD = 2f;
    float attackCDTimer;

    float damagedCD = 0.5f;
    float damagedCDTimer;

    float decideToAttackCD = 0.7f;
    float decideToAttackCDTimer = 0;

    private Coroutine enemyAttackCoroutine;
    
    public enum EnemyStates
    {
        idle,
        alert,
        preattack,
        attack,
        grapped,
        damaged,
        dead
    }

    private void Awake()
    {
        //avoid self destrustioc if broken drone is active by accident
        //if (avoidIstantDestroy.activeSelf) avoidIstantDestroy.SetActive(false);
    }

    void Start()
    {

        

        //find the player object
        player = GameObject.Find("Player");

        //default idle state
        state = EnemyStates.idle;

        //reset timers
        decideToAttackCDTimer = decideToAttackCD;
        attackCDTimer = attackCD;
        damagedCDTimer = damagedCD; // !!change to damaged timer
    }


    void FixedUpdate()
    {
        if (enemyHealth <= 0)
        {
            // !! change to dead state
            state = EnemyStates.dead;
        }

        //activate smoke particles if 1 HP
        /*if (enemyHealth == 1)
        {
            lowHP.gameObject.SetActive(true);
        }*/

        //limit the speed
        if (GetComponent<Rigidbody>().velocity.magnitude > 10f && state == EnemyStates.alert)
        {
            GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity.normalized * 10f;
        }

        //if enemy is not alerted - stop invoke
        if (state != EnemyStates.alert) CancelInvoke("DecidingToAttack");

        //if (state != EnemyStates.grapped) grapplingParticles.gameObject.SetActive(false);

        //=================STATES MACHINE
        //if stay still
        if (state == EnemyStates.idle)
        {
            //stay still with no gravity
            GetComponent<Rigidbody>().useGravity = false;
            //tyr to nitice player
            TryToNoticePlayer();
            //if the velocity is high enough - decrease it
            if (GetComponent<Rigidbody>().velocity.magnitude > 0)
            {
                GetComponent<Rigidbody>().velocity = Vector3.Lerp(GetComponent<Rigidbody>().velocity, Vector3.zero, Time.deltaTime * 2f);
            }
            //if noticed player
        }
        else if (state == EnemyStates.alert)
        {
            //set lamp to blue in alert state
            //lamp.GetComponent<Renderer>().material.SetColor("_Color", alertLamp);
            //little air up force
            GetComponent<Rigidbody>().AddForce(Vector3.up * 5f, ForceMode.Force);
            //lift drone if its below player
            if (player.transform.position.y > transform.position.y) GetComponent<Rigidbody>().AddForce(Vector3.up * 10f, ForceMode.Force);
            //turn on gravity
            GetComponent<Rigidbody>().useGravity = true;
            //rotate to the player exclude Y
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.Scale(PlayerDirection(), 
                new Vector3(1, 0, 1))), Time.deltaTime * 5);
            //get lightly random distance to player to follow
            float alertMovingDistanceRand = Random.Range(alertMovingDistance - 2f, alertMovingDistance + 2f);

            //stay at the distance from player
            if (Vector3.Distance(transform.position, player.transform.position) < alertMovingDistanceRand)
            {
                GetComponent<Rigidbody>().AddForce(PlayerDirection().normalized * -enemySpeed);
            }
            else if (Vector3.Distance(transform.position, player.transform.position) > alertMovingDistanceRand)
            {
                GetComponent<Rigidbody>().AddForce(PlayerDirection().normalized * enemySpeed);
            }

            //deciding to attack or not every second of alerting

            decideToAttackCDTimer -= Time.deltaTime;
            if (decideToAttackCDTimer < 0)
            {
                decideToAttackCDTimer = decideToAttackCD; //reset timer
                DecidingToAttack();
            }

            //check the distance to player to lose or not it
            // DRONE FOLLOW PAYER TO DEATH
            /*if (playerFarawayCD < 0)
            {
                LeavePlayer();
            }
            else playerFarawayCD -= Time.deltaTime;*/

            //if decided to attack - wiggle
        }
        else if (state == EnemyStates.preattack)
        {
            //rotate to the player exclude Y
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.Scale(PlayerDirection(), 
                new Vector3(1, 0, 1))), Time.deltaTime * 5);

        } 
        // while attacking the player
        else if (state == EnemyStates.attack)
        {

            //if coroutine wasn't set - set it
            if (enemyAttackCoroutine == null)
            {
                enemyAttackCoroutine = StartCoroutine(DoLaserAttack());
            }

            //rotate to the player exclude Y
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.Scale(PlayerDirection(), 
                new Vector3(1, 0, 1))), Time.deltaTime * 5);

        }
        else if (state == EnemyStates.damaged)
        {

            //add some random jitter after take damage
            float jitter = Random.Range(-0.1f, 0.1f);
            transform.position = transform.position + new Vector3(jitter, jitter, jitter);

            //set state to alert once
            damagedCDTimer -= Time.deltaTime;
            if (damagedCDTimer < 0) state = EnemyStates.alert;
            else state = EnemyStates.damaged;

        }
        else if (state == EnemyStates.grapped)
        {
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            GetComponent<Rigidbody>().useGravity = false;

        }
        else if (state == EnemyStates.dead)
        {
            
            StopPlayerBeenAttacked();

            StopAttack();

            //turn off gravity
            GetComponent<Rigidbody>().useGravity = false;
            
            //shrink enemy
            if (transform.localScale.x >= 0 && transform.localScale.y >= 0 && transform.localScale.z >= 0)
            {
                float sppedOfShrink = 0.95f;
                transform.localScale = Vector3.Scale(transform.localScale,new Vector3(sppedOfShrink, sppedOfShrink, sppedOfShrink));
            }

            //destroy after 1 second
            Invoke(nameof(DestroyEnemy), 0.7f);
            
        }
    }

    IEnumerator DoLaserAttack()
    {
        //initiate tiers and array states
        laserAttackTimer = 0f;
        lerpTime = 0f;
        laserAttackStates = new bool[6];

        while (laserAttackTimer <= 10f && laserAttackStates[3] == false)
        {
            //laser attack procedure
            laserAttackTimer += Time.deltaTime;
            // Find Point A: if timer and 1st state isn't done
            if (laserAttackTimer > 0.1f && !laserAttackStates[0])
            {
                //DO ONCE
                //calculate player's position for Point A
                pointAForlaser = PlayerDirection();
                laserBeam.transform.rotation = Quaternion.LookRotation(pointAForlaser);
                laserAttackStates[0] = true;
            }
            // Expand the laser to Point A
            if (laserAttackTimer > 0.5f && !laserAttackStates[1])
            {
                //DO WHILE NEXT STATE
                //activate object
                if (!laserBeam.activeSelf) laserBeam.SetActive(true);
                //increase Lazer Z size
                lerpTime += Time.deltaTime;

                //find end length for the lazer
                if (Physics.Raycast(laserBeam.transform.position, laserBeam.transform.TransformDirection(Vector3.forward), out hit, defaultLaserBeam, LayerMask.GetMask("whatIsGround")))
                {
                    //set Z scale if laser meets the ground
                    laserZScale = hit.distance;
                    //else it's defaultLaserBeam
                }
                else laserZScale = defaultLaserBeam;

                laserIncreaseSize = Mathf.Lerp(0, laserZScale, Mathf.Clamp01(lerpTime / 0.5f));
                //apply new scale Z
                Vector3 currentScale = laserBeam.transform.localScale;
                laserBeam.transform.localScale = new Vector3(currentScale.x, currentScale.y, laserIncreaseSize);

            }
            // Rotate laser to the Player
            if (laserAttackTimer > 1f && !laserAttackStates[2])
            {
                //DO WHILE NEXT STATE
                laserAttackStates[1] = true;
                lerpTime = 0;
                // get direction to player position
                Vector3 diresctionToB = player.transform.position - laserBeam.transform.position;
                // get target rotation
                Quaternion targetRotation = Quaternion.LookRotation(diresctionToB);
                // rotate the laser
                float rotationSpeed = 35f;
                laserBeam.transform.rotation = Quaternion.RotateTowards(laserBeam.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

                //cut lazer if collider infront ground
                RaycastHit hit;
                //set the distance to check if raycast hits the player
                float playerDistanceHit;
                //throw raycast along the laser on whole length
                Vector3 currentScale = laserBeam.transform.localScale;
                if (Physics.Raycast(laserBeam.transform.position, laserBeam.transform.TransformDirection(Vector3.forward), out hit, defaultLaserBeam, LayerMask.GetMask("whatIsGround")))
                {
                    //change Z scale if laser meets the ground
                    laserBeam.transform.localScale = new Vector3(currentScale.x, currentScale.y, hit.distance);
                    //set the attack raycast to HIT distance
                    playerDistanceHit = hit.distance;
                }
                else
                {
                    //set Z default defaultLaserBeam
                    laserBeam.transform.localScale = new Vector3(currentScale.x, currentScale.y, defaultLaserBeam);
                    //set the attack raycast to default
                    playerDistanceHit = defaultLaserBeam;
                }

                //if lazer hits player - hit th player once
                if (Physics.Raycast(laserBeam.transform.position, laserBeam.transform.TransformDirection(Vector3.forward), playerDistanceHit, LayerMask.GetMask("Player")))
                {
                    laserBeam.GetComponent<HitThePlayer>().HitPlayerOnce();
                }

            }
            //decrease the laser
            if (laserAttackTimer > 3f && !laserAttackStates[3])
            {
                //DO WHILE NEXT STATE
                laserAttackStates[2] = true;
                lerpTime += Time.deltaTime;
                //find end length for the lazer
                if (Physics.Raycast(laserBeam.transform.position, laserBeam.transform.TransformDirection(Vector3.forward), out hit, defaultLaserBeam, LayerMask.GetMask("whatIsGround")))
                {
                    //set Z scale if laser meets the ground
                    laserZScale = hit.distance;
                    //else it's defaultLaserBeam
                }
                else laserZScale = defaultLaserBeam;
                laserIncreaseSize = Mathf.Lerp(laserZScale, 0, Mathf.Clamp01(lerpTime / 0.2f));
                //apply new scale Z
                Vector3 currentScale = laserBeam.transform.localScale;
                laserBeam.transform.localScale = new Vector3(currentScale.x, currentScale.y, laserIncreaseSize);
                //end decreasing ig lazes Z small
                if (laserBeam.transform.localScale.z < 0.1f)
                {
                    laserAttackStates[3] = true;
                    //deactivate object
                    if (laserBeam.activeSelf) laserBeam.SetActive(false);
                    //reset played damage delay
                    laserBeam.GetComponent<HitThePlayer>().wasPlayerDamaged = false;
                }
            }

            // Wait for the next frame
            yield return null;
        }

        //lereset laser attack vars and states
        laserAttackTimer = 0;
        lerpTime = 0;
        laserAttackStates = new bool[6];
        //reset laser timer and scale
        laserIncreaseSize = 0;
        laserBeam.transform.localScale = Vector3.Scale(laserBeam.transform.localScale, new Vector3(1, 1, 0));
        //reset the state to alert
        state = EnemyStates.alert;
        enemyAttackCoroutine = null;

    }

    private void OnDestroy()
    {// healpp player on destroy (if to avoid errors on closing the game)
        if (player != null) player.GetComponent<PlayerMovement>().HealPlayer();
    }

    private void TryToNoticePlayer()
    {
        //if idle and player close enemy is alerted
        if (Physics.Raycast(transform.position,PlayerDirection(), out hit, noticeDistance, playerLayer))
        {
            state = EnemyStates.alert;
        } 
    }

    private Vector3 PlayerDirection()
    {// find player direction
        return player.transform.position - transform.position;
    }

        
    private void DecidingToAttack()
    {
        //chance to attack every second of alerting
        float rnd = Random.Range(0, 1f);
        if (rnd >= 0.75f)
        {
            //activate sfx
            /*sfxBeforeAttack.pitch = Random.Range(0.5f,1.5f);
            sfxBeforeAttack.Play();*/
            Invoke(nameof(ActivatePreAttackState),0.5f);
            
        } else
        {
            return;
        }
    }

    private void AlertAfterAttack()
    {
        //reset the alert state
        state = EnemyStates.alert;
    }
    
    private void LeavePlayer()
    {
        //set the idle state if player is out of view
        if (!Physics.Raycast(transform.position, PlayerDirection(), out hit, noticeDistance * 1.5f, playerLayer))
        {
            state = EnemyStates.idle;
        }
        //reset timer in any case
        //playerFarawayCD = 1f;
    }

    private void AttackAfterPreAttack()
    {
        
        // back to alert state after two seconds of rushing
        //Invoke(nameof(AlertAfterAttack), 2f);
        //turn on gravity
        //GetComponent<Rigidbody>().useGravity = true;
        //reset the alert state
        state = EnemyStates.attack;
    }


    public void TakeHit(float damage)
    {
        //play danage sound
        /*
        sfxGetHit.pitch = Random.Range(0.5f, 1.5f);
        sfxGetHit.Play();*/
        // reduce health
        enemyHealth -= (int)damage;
        //set state damaged
        state = EnemyStates.damaged;
        //initiate damage
        ResetAttackedTimer();
    }

    public void Push(Vector3 direction, float force)
    {
        GetComponent<Rigidbody>().AddForce(direction * force, ForceMode.Impulse);
    }

    private void ActivatePreAttackState()
    {
        //stop velocity
        GetComponent<Rigidbody>().velocity = Vector3.zero;

        // orange light indocation
        //lamp.GetComponent<Renderer>().material.SetColor("_Color", preAttacktLamp);

        circleSparkles.Play();
        centerSegment.Play();

        //gravity off
        GetComponent<Rigidbody>().useGravity = false;

        //wiggle drone
        //GetComponent<Animator>().Play("Base Layer.Wiggle", 0, 0);

        Invoke(nameof(AttackAfterPreAttack), 1.5f);

        //create a UI arrow if player is near
        if (Vector3.Distance(transform.position, player.transform.position) < 20)
            player.GetComponent<AttackCircleUI>().CreateUIArrow(transform);

        //change state to attack
        state = EnemyStates.preattack;

        

    }

    private void AttackPlayer()
    {
        //reset timer
        attackCDTimer = attackCD;
        //set player attacked condition
        player.GetComponent<PlayerMovement>().attacked = true;
        //cancel this condition later
        Invoke(nameof(StopPlayerBeenAttacked), 0.8f);

        //get force direction for the player
        Vector3 flatDirection = new Vector3(PlayerDirection().x, 0f, PlayerDirection().z);
        Vector3 playerDirectionY = new Vector3(0f, PlayerDirection().y + 5f, 0f);
        //push player
        player.GetComponent<Rigidbody>().AddForce(flatDirection.normalized * pushForce * 10f + playerDirectionY.normalized * pushForce * 5f, ForceMode.Impulse);

        //activate player's get hit method
        player.GetComponent<PlayerMovement>().PlayerGetHit();

        //back to alert state
        state = EnemyStates.alert;
    }

    private void StopPlayerBeenAttacked()
    {//to avoid infinite attacked state for the player after enemty death
        player.GetComponent<PlayerMovement>().attacked = false;
    }

    public void ResetAttackedTimer()
    {//access to other scripts
        damagedCDTimer = damagedCD;
    }

    public void StopAttack()
    {
        //deactivale beam
        if (laserBeam.activeSelf) laserBeam.SetActive(false);
        //make conditions to stop laser attack
        laserAttackTimer = 0;
        lerpTime = 0;
        laserAttackStates = new bool[6];
        //reset laser timer and scale
        laserIncreaseSize = 0;
        laserBeam.transform.localScale = Vector3.Scale(laserBeam.transform.localScale, new Vector3(1, 1, 0));
    }

    private void DestroyEnemy()
    {
        //create emitter for coins
        gameObject.GetComponent<ThrowConsumables>().AddEmitter();
        Destroy(transform.parent.gameObject);
        //vhange model to obstacles
        /*
        if (wholeDrone.activeSelf) wholeDrone.SetActive(false);
        if (!brokenDrone.activeSelf) brokenDrone.SetActive(true);*/
    }
}
