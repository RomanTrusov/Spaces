using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviourDrone : MonoBehaviour
{
    [SerializeField]
    private GameObject player;

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
        attacked
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

    void Update()
    {
        //check for 0 heath
        if (enemyHealth == 0)
        {
            StopPlayerBeenAttacked();
            Destroy(gameObject);
        }

        //little air up force
        GetComponent<Rigidbody>().AddForce(Vector3.up * 7f, ForceMode.Force);

        //if enemy attacker - cuntdown with attacked state, at the end change state and false attacked
        if (attacked)
        {
            attackedCDTimer -= Time.deltaTime;
            if (attackedCDTimer < 0)
            {
                attacked = false;
                state = EnemyStates.alert;
            } else state = EnemyStates.attacked;
        }
        
        //limit the speed
        if (GetComponent<Rigidbody>().velocity.magnitude > 4f && state == EnemyStates.alert)
        {
            GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity.normalized * 4f;
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
            GetComponent<Rigidbody>().AddForce(PlayerDirection().normalized * enemySpeed * 2f);

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
        } 
    }

    private Vector3 PlayerDirection()
    {
        return player.transform.position - transform.position;
    }

        
    private void DecidingToAttack()
    {
        //50/50 to attack every second of alerting
        float rnd = Random.Range(0, 1f);
        if (rnd >= 0.75f)
        {
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            state = EnemyStates.attack;
            Invoke(nameof(AlertAfterAttack), 2f);
        } else
        {
            return;
        }
    }

    private void AlertAfterAttack()
    {
        state = EnemyStates.alert;
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

    public void ResetAttackedTimer()
    {
        attackedCDTimer = attackedCD;
    }

}
