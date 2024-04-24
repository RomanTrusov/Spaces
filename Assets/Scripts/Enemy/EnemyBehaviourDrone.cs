using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviourDrone : MonoBehaviour
{
    [SerializeField]
    private GameObject player;
    [SerializeField]
    private ParticleSystem parts;
    [SerializeField]
    private ParticleSystem dust;
    [SerializeField]
    private ParticleSystem lowHP;

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

    public bool attacked;

    [SerializeField]
    EnemyStates state;

    [SerializeField]
    float attackCD = 2f;
    float attackCDTimer;

    float attackedCD = 1f;
    float attackedCDTimer;

    float decideToAttackCD = 1f;
    float decideToAttackCDTimer = 0;


    enum EnemyStates
    {
        idle,
        alert,
        attack,
        attacked,
    }

    void Start()
    {

        player = GameObject.Find("Player");

        //default idle state
        state = EnemyStates.idle;

        //reset timers
        decideToAttackCDTimer = decideToAttackCD;
        attackCDTimer = attackCD;
        attackedCDTimer = attackedCD;
    }

    void FixedUpdate()
    {
        //check for 0 heath
        if (enemyHealth <= 0)
        {
            StopPlayerBeenAttacked();
            state = EnemyStates.attacked;
            GetComponent<Rigidbody>().mass = 20f;
        }

        //activate smoke if 1 HP
        if (enemyHealth == 1)
        {
            lowHP.gameObject.SetActive(true);
        }

        //little air up force
        if (state != EnemyStates.idle && state != EnemyStates.attacked) GetComponent<Rigidbody>().AddForce(Vector3.up * 9f, ForceMode.Force);

        //if enemy attacker - cuntdown with attacked state, at the end change state and false attacked
        if (attacked)
        {
            //activate effect
            if (attackedCDTimer == attackedCD)
            {
                ParticleSystem clone = Instantiate(parts, transform.position, transform.rotation, transform);
                clone.gameObject.SetActive(true);

            }
            attackedCDTimer -= Time.deltaTime;
            if (attackedCDTimer < 0)
            {
                attacked = false;
                state = EnemyStates.alert;
            } else state = EnemyStates.attacked;
            if (enemyHealth <= 0) Invoke(nameof(DestroyEnemy), 1f);
        }
        
        //limit the speed
        if (GetComponent<Rigidbody>().velocity.magnitude > 10f && state == EnemyStates.alert)
        {
            GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity.normalized * 10f;
        }

        //try to notice player every step
        if (state != EnemyStates.attacked) TryToNoticePlayer();

        //if enemy is not alerted - stop invoke
        if (state != EnemyStates.alert) CancelInvoke("DecidingToAttack");

        //rotate to the player if not idle
        if (state != EnemyStates.idle && state != EnemyStates.attacked) transform.LookAt(player.transform, Vector3.up);


        //=================STATES BEHAVIOUR
        //if stay still
        if (state == EnemyStates.idle)
        {
            //stay still
            GetComponent<Rigidbody>().useGravity = false;
            //if noticed player
        } else if (state == EnemyStates.alert)
        {
            GetComponent<Rigidbody>().useGravity = true;
            //move around
            float alertMovingDistanceRand = Random.Range(alertMovingDistance-1f, alertMovingDistance+1f);

            //move sideways
            float rnd = Random.Range(0,1f);
            if (rnd < 0.005f)
            {
                GetComponent<Rigidbody>().AddForce(Vector3.left * 40f,ForceMode.Impulse);
            }
            else if (rnd > 0.995f)
            {
                GetComponent<Rigidbody>().AddForce(Vector3.left * -40f, ForceMode.Impulse);
            }
            else

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
            //calculate players'face position
            Vector3 PlayerFace = PlayerDirection() + new Vector3(0,0.5f,0);
            //rush on player with more speed
            GetComponent<Rigidbody>().AddForce(PlayerFace.normalized * enemySpeed * 3f);

            //if raycasted player - hit him
            if (Physics.Raycast(transform.position, PlayerDirection(), out hit, 1.5f, playerLayer))
            {
                AttackPlayer();
            }
        }
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
            //stop Y velocity
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            //change state to attack
            state = EnemyStates.attack;
            // back to alert state after two seconds of rushing
            Invoke(nameof(AlertAfterAttack), 2f);
            // start red light animation
            GetComponent<Animator>().Play("Base Layer.RedLight",0,0);
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
        attackedCDTimer = attackedCD;
    }

    private void DestroyEnemy()
    {
        //make a lot of patricles
        ParticleSystem clone = Instantiate(parts, transform.position, transform.rotation);
        clone.gameObject.SetActive(true);
        //scale down enemy
        transform.localScale = Vector3.Scale(transform.localScale, new Vector3(0.85f, 0.85f, 0.85f));

        //restore player's health
        player.GetComponent<PlayerMovement>().playerHP += 1;

        //destroy parent after second
        Destroy(transform.parent.gameObject,0.5f);
    }

}
