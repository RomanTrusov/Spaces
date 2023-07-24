using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    public PlayerMovement pm;
    public Rigidbody playerRb;
    public Transform playerPosition;
    public float alertDistance;
    public LayerMask playerLayer;
    public float enemySpeed;
    public EnemyStates state;
    public float pushForce;

    private Vector3 playerDirection;
    private RaycastHit hit; //will be used to punch player

    private Transform selfPosition;
    private Rigidbody selfRb;

    private float attackCD; //cooldown before next attack
    private float attackCDtimer;

    public enum EnemyStates
    {
        await,
        follow
    }

    private void Start()
    {
        selfPosition = GetComponent<Transform>();
        selfRb = GetComponent<Rigidbody>();
        state = EnemyStates.await;
        attackCD = 0.5f; //time before attack
        attackCDtimer = attackCD;
    }

    private void FixedUpdate()
    {
        
        if (state == EnemyStates.await && !NoticedPlayer())
        {
            
        }
        else if (NoticedPlayer())
        {
            state = EnemyStates.follow;
            FollowPlayer();
            CheckForAttack();
        }
    }


    private bool NoticedPlayer ()
    { // check if player close to the enemy
        playerDirection = playerPosition.position - selfPosition.position;
        if (Physics.Raycast(selfPosition.position, playerDirection, out hit, alertDistance, playerLayer))
        {
            return true;
        }
        else {
            return false;
        }

    }

    private void FollowPlayer ()
    { //apply movement to the player direction
        playerDirection = playerPosition.position - selfPosition.position;
        // check for loosing player in larger distance
        if (Physics.Raycast(selfPosition.position, playerDirection, out hit, alertDistance, playerLayer))
        {
            selfRb.AddForce(playerDirection.normalized * enemySpeed);
        } else
        {
            state = EnemyStates.await;
        }
    }

    private void CheckForAttack()
    {
        if (Physics.Raycast(selfPosition.position, playerDirection, out hit, 1.5f, playerLayer) && attackCDtimer >= 0)
        {
            attackCDtimer -= Time.deltaTime;
        } else if (Physics.Raycast(selfPosition.position, playerDirection, out hit, 1.5f, playerLayer) && attackCDtimer < 0)
        {
            AttackPlayer();
            attackCDtimer = attackCD;
        } else attackCDtimer = attackCD;

    }

    private void AttackPlayer ()
    {
        pm.attacked = true;
        Invoke(nameof(StopPlayerBeenAttacked), 0.5f);
        playerRb.AddForce((playerDirection.normalized * pushForce * 10) + new Vector3(0f,10f,0f),ForceMode.Impulse);
    }

    private void StopPlayerBeenAttacked()
    {
        pm.attacked = false;
    }
         
} 
