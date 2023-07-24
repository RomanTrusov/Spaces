using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    public Transform playerPosition;
    public float alertDistance;
    public LayerMask playerLayer;

    private Vector3 playerDirection;
    private RaycastHit hit;

    Transform enemyPosition;
    public float distance;


    private void Start()
    {
        enemyPosition = GetComponent<Transform>();
    }

    private void FixedUpdate()
    {

        if (!NoticedPlayer())
        {
            Debug.Log("NotGotcha");
            //try to nootice player
        }
        else
        {
            Debug.Log("Gotcha!");
            //follow player until losing him again
        }
    }


    private bool NoticedPlayer ()
    { // check if player close to the enemy
        playerDirection = playerPosition.position - enemyPosition.position;
        if (Physics.Raycast(enemyPosition.position, playerDirection, out hit, alertDistance, playerLayer))
        {
            return true;
        }
        else {
            return false;
        }

    }

    private void FollowPlayer ()
    { //apply movement to the player direction

    }
} 
