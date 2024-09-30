using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    public PlayerMovement pm; // get Player movements script
    public Rigidbody playerRb; // get player rigid body
    public Transform playerPosition; 
    public float alertDistance; // distance to spot the player
    public LayerMask playerLayer; 
    public float enemySpeed; 
    public EnemyStates state;
    public float pushForce; // force of the enemy's attack

    private Vector3 playerDirection;
    private RaycastHit hit; //will be used to punch player

    private Transform selfPosition;
    private Rigidbody selfRb;

    private float attackCD; //cooldown before next attack
    private float attackCDtimer;

    public enum EnemyStates // states of the enemy
    {
        await,
        follow
    }

    private void Start()
    {
        selfPosition = GetComponent<Transform>();
        selfRb = GetComponent<Rigidbody>();
        state = EnemyStates.await;
        attackCD = 2f; //time before attack
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


    private bool NoticedPlayer()
    { // check if player close enough to the enemy
        playerDirection = playerPosition.position - selfPosition.position; // get which direction to throw raycast
        if (Physics.Raycast(selfPosition.position, playerDirection, out hit, alertDistance, playerLayer))
        {
            return true;
        }
        else {
            return false;
        }

    }

    private void FollowPlayer()
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
        if (attackCDtimer >= 0)
        {
            attackCDtimer -= Time.deltaTime;
        } else if (Physics.Raycast(selfPosition.position, playerDirection, out hit, 1f, playerLayer))
        {
            AttackPlayer();
            attackCDtimer = attackCD;
        }

    }

    private void AttackPlayer ()
    {
        pm.attacked = true; //set player to the condition
        Invoke(nameof(StopPlayerBeenAttacked), 0.8f); //cancel this condition later
        Vector3 flatDirection = new Vector3(playerDirection.x,0f,playerDirection.z);
        Vector3 playerDirectionY = new Vector3(0f, playerDirection.y + 5f, 0f);
        playerRb.AddForce(flatDirection.normalized * pushForce * 10f + playerDirectionY.normalized * pushForce * 5f, ForceMode.Impulse);
        //playerRb.AddForce((playerDirection.normalized + new Vector3(0f, 1f, 0f)) * pushForce * 10 + new Vector3(0f, -2f, 0f), ForceMode.Impulse);
    }

    private void StopPlayerBeenAttacked()
    {
        pm.attacked = false;
    }
         
} 
