using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    public Transform playerPosition;
    public float alertDistance;
    public LayerMask playerLayer;
    public float enemySpeed;

    private Vector3 playerDirection;
    private RaycastHit hit; //will be used to punch player

    private Transform selfPosition;
    private Rigidbody selfRb;
    public EnemyStates state;

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
} 
