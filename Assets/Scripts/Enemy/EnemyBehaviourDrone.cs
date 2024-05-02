using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviourDrone : MonoBehaviour
{
    [SerializeField]
    GameObject avoidIstantDestroy;

    [SerializeField]
    private GameObject player;
    [SerializeField]
    private ParticleSystem parts;
    [SerializeField]
    private ParticleSystem dust;
    [SerializeField]
    private ParticleSystem lowHP;
    [SerializeField]
    private GameObject lamp;

    [ColorUsage(true, true)]
    public Color alertLamp;
    [ColorUsage(true, true)]
    public Color attacktLamp;
    [ColorUsage(true, true)]
    public Color damagedtLamp;

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

    [SerializeField]
    private GameObject wholeDrone;
    [SerializeField]
    private GameObject brokenDrone;

    float damagedCD = 1f;
    float damagedCDTimer;

    float decideToAttackCD = 1f;
    float decideToAttackCDTimer = 0;

    public AudioSource sfxBeforeAttack;
    public AudioSource sfxGetHit;

    public enum EnemyStates
    {
        idle,
        alert,
        attack,
        grapped,
        damaged,
        dead
    }

    private void Awake()
    {
        //avoid self destrustioc if broken drone is active by accident
        if (avoidIstantDestroy.activeSelf) avoidIstantDestroy.SetActive(false);
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
        if (enemyHealth == 1)
        {
            lowHP.gameObject.SetActive(true);
        }

        

        //limit the speed
        if (GetComponent<Rigidbody>().velocity.magnitude > 10f && state == EnemyStates.alert)
        {
            GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity.normalized * 10f;
        }

        //try to notice player every step
        if (state != EnemyStates.damaged) TryToNoticePlayer();

        //if enemy is not alerted - stop invoke
        if (state != EnemyStates.alert) CancelInvoke("DecidingToAttack");

        //=================STATES MACHINE
        //if stay still
        if (state == EnemyStates.idle)
        {
            //stay still with no gravity
            GetComponent<Rigidbody>().useGravity = false;
            //if noticed player
        } 
        else if (state == EnemyStates.alert)
        {
            //set lamp to blue in alert state
            lamp.GetComponent<Renderer>().material.SetColor("_Color", alertLamp);
            //little air up force
            GetComponent<Rigidbody>().AddForce(Vector3.up * 9f, ForceMode.Force);
            //turn on gravity
            GetComponent<Rigidbody>().useGravity = true;
            //rotate to the player
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(PlayerDirection()), Time.deltaTime * 5);
            //get lightly random distance to player to follow
            float alertMovingDistanceRand = Random.Range(alertMovingDistance-1f, alertMovingDistance+1f);

            //stay at the distance
            if (Vector3.Distance(transform.position,player.transform.position) < alertMovingDistanceRand)
                {
                    GetComponent<Rigidbody>().AddForce(PlayerDirection().normalized * -enemySpeed);
                } else if (Vector3.Distance(transform.position, player.transform.position) > alertMovingDistanceRand)
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

        //if decided to attack
        } 
        else if (state == EnemyStates.attack)
        {
            // red light indocation
            lamp.GetComponent<Renderer>().material.SetColor("_Color", attacktLamp);
            //calculate players'face position
            Vector3 PlayerFace = PlayerDirection() + new Vector3(0,0.5f,0);
            //little air up force
            GetComponent<Rigidbody>().AddForce(Vector3.up * 9f, ForceMode.Force);
            //rush on player with more speed
            GetComponent<Rigidbody>().AddForce(PlayerFace.normalized * enemySpeed * 3f);
            //rotate to the player
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(PlayerDirection()), Time.deltaTime * 5);
            //if raycasted player - hit him
            if (Physics.Raycast(transform.position, PlayerDirection(), out hit, 2f, playerLayer))
            {
                AttackPlayer();
            }
        }
        else if (state == EnemyStates.damaged)
        {
            //set lamp to damaged
            lamp.GetComponent<Renderer>().material.SetColor("_Color", damagedtLamp);

            //stop the velocity and off gravity
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            GetComponent<Rigidbody>().useGravity = false;


            //activate particle effect
            if (damagedCDTimer == damagedCD)
            {
                ParticleSystem clone = Instantiate(parts, transform.position, transform.rotation, transform);
                clone.gameObject.SetActive(true);
            }

            //reduce timer to set effect once
            damagedCDTimer -= Time.deltaTime;
            if (damagedCDTimer < 0)
            {
                state = EnemyStates.alert;
            }
            else state = EnemyStates.damaged;
            //if (enemyHealth <= 0) state = EnemyStates.dead;
        }
        else if (state == EnemyStates.grapped)
        {
            //set lamp to damaged
            lamp.GetComponent<Renderer>().material.SetColor("_Color", damagedtLamp);

            //stop the velocity and off gravity
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            GetComponent<Rigidbody>().useGravity = false;

        }
        else if (state == EnemyStates.dead)
        {
            StopPlayerBeenAttacked();
            DestroyEnemy();
        }
    }

    private void OnDestroy()
    {// healpp player on destroy (if to avoid errors on closing the game)
        if (player != null) player.GetComponent<PlayerMovement>().HealPlayer();
    }

    private void TryToNoticePlayer()
    {
        //if idle and player close enemy is alerted
        if (state == EnemyStates.idle && Physics.Raycast(transform.position,PlayerDirection(), out hit, noticeDistance, playerLayer))
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
            sfxBeforeAttack.Play();
            Invoke(nameof(ActivateAttackState),0.5f);
            
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

    public void GetDamage(int damage)
    {
        sfxGetHit.Play();
        enemyHealth -= damage;
        state = EnemyBehaviourDrone.EnemyStates.damaged;
        ResetAttackedTimer();
    }

    private void ActivateAttackState()
    {
        //stop velocity
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        //change state to attack
        state = EnemyStates.attack;
        // back to alert state after two seconds of rushing
        Invoke(nameof(AlertAfterAttack), 2f);
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

    private void DestroyEnemy()
    {
        /*
        //make a lot of patricles
        ParticleSystem clone = Instantiate(parts, transform.position, transform.rotation);
        clone.gameObject.SetActive(true);
        //scale down enemy
        transform.localScale = Vector3.Scale(transform.localScale, new Vector3(0.85f, 0.85f, 0.85f));

        //destroy parent after second
        Destroy(transform.parent.gameObject,0.5f);
        */

        if (wholeDrone.activeSelf) wholeDrone.SetActive(false);
        if (!brokenDrone.activeSelf) brokenDrone.SetActive(true);
    }


}
