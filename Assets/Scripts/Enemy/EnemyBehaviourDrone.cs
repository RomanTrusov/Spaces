using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviourDrone : MonoBehaviour
{
    [SerializeField]
    private GameObject player;

    public int enemyHealth = 1;

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
    EnemyStates state;

    [SerializeField]
    float attackCD = 2f;
    float attackCDTimer;

    float decideToAttackCD = 1f;
    float decideToAttackCDTimer;


    enum EnemyStates
    {
        idle,
        alert,
        attack
    }

    void Start()
    {

        player = GameObject.Find("Player");

        //default idle state
        state = EnemyStates.idle;

        //reset timers
        decideToAttackCDTimer = decideToAttackCD;
        attackCDTimer = attackCD;
    }

    void Update()
    {
        //check for 0 heath
        if (enemyHealth == 0)
        {
            StopPlayerBeenAttacked();
            Destroy(gameObject);
        }
        
        //limit the speed
        if (GetComponent<Rigidbody>().velocity.magnitude > 2f && state != EnemyStates.attack)
        {
            GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity.normalized * 2f;
        }

        //try to notice player every step
        TryToNoticePlayer();

        //if enemy is not alerted - stop invoke
        if (state != EnemyStates.alert) CancelInvoke("DecidingToAttack");

        //rotate to the player if not idle
        if (state != EnemyStates.idle) transform.LookAt(player.transform, Vector3.up);


        //=================STATES BEHAVIOUR
        //if stay still
        if (state == EnemyStates.idle)
        {
            //stay still
        //if noticed player
        } else if (state == EnemyStates.alert)
        {
            //move around

            //stay at the distance
            if (Vector3.Distance(transform.position,player.transform.position) < alertMovingDistance)
            {
                GetComponent<Rigidbody>().AddForce(PlayerDirection().normalized * -enemySpeed);
            } else if (Vector3.Distance(transform.position, player.transform.position) > alertMovingDistance)
            {
                GetComponent<Rigidbody>().AddForce(PlayerDirection().normalized * enemySpeed);
            } else
            {
                GetComponent<Rigidbody>().velocity = Vector3.zero;
            }
            
            //decide to attack every second
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
            //going to attack
            GetComponent<Rigidbody>().AddForce(PlayerDirection().normalized * enemySpeed * 5f);

            //try to attack player after cooldown if raycast works
            /*if (attackCDTimer > 0)
            {
                attackCDTimer -= Time.deltaTime;
            } else
            {
                if (Physics.Raycast(transform.position, PlayerDirection(), out hit, 1.2f, playerLayer))
                {
                    AttackPlayer();
                }
            }*/
            if (Physics.Raycast(transform.position, PlayerDirection(), out hit, 1.5f, playerLayer))
            {
                AttackPlayer();
            }
        }
    }


    private void TryToNoticePlayer()
    {
        //getting player direction
        //if idle and player close
        if (state == EnemyStates.idle && Physics.Raycast(transform.position,PlayerDirection(), out hit, noticeDistance, playerLayer))
        {
            state = EnemyStates.alert;
        //if not idle and not see me - idle
        } /*else if (state != EnemyStates.idle && !Physics.Raycast(transform.position, PlayerDirection(), out hit, noticeDistance * 1.5f, playerLayer))
        {
            state = EnemyStates.idle;
        }*/
    }

    private Vector3 PlayerDirection()
    {
        return player.transform.position - transform.position;
    }

        
    private void DecidingToAttack()
    {
        //50/50 to attack every second of alerting
        float rnd = Random.Range(0, 1f);
        if (rnd >= 0.85f)
        {
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            state = EnemyStates.attack;
        } else
        {
            return;
        }
    }

    
    private void AttackPlayer()
    {
        attackCDTimer = attackCD; //reset timer
        player.GetComponent<PlayerMovement>().attacked = true; //set player to the condition
        Invoke(nameof(StopPlayerBeenAttacked), 0.8f); //cancel this condition later
        Vector3 flatDirection = new Vector3(PlayerDirection().x, 0f, PlayerDirection().z);
        Vector3 playerDirectionY = new Vector3(0f, PlayerDirection().y + 5f, 0f);
        player.GetComponent<Rigidbody>().AddForce(flatDirection.normalized * pushForce * 10f + playerDirectionY.normalized * pushForce * 5f, ForceMode.Impulse);

        state = EnemyStates.alert;
    }

    private void StopPlayerBeenAttacked()
    {
        player.GetComponent<PlayerMovement>().attacked = false;
    }

}
