using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumableObject : MonoBehaviour
{
    public Transform player;  // Assign the player in the Inspector
    public float moveSpeed = 5f;  // Speed at which the object moves to the player
    public float shrinkSpeed = 0.5f;  // Speed at which the object shrinks
    public float collectionDistance = 1f;  // Distance at which the item starts moving to the player

    private bool isMovingToPlayer = false;  // Flag to check if the object should start moving

    private void Start()
    {
        //find the player object
        player = GameObject.Find("Player").transform;
    }

    void Update()
    {
        // Check the distance between the player and the object
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // If the player is within the collection distance, start moving and shrinking
        if (distanceToPlayer <= collectionDistance)
        {
            isMovingToPlayer = true;
        }

        // If the object should move towards the player
        if (isMovingToPlayer)
        {
            // Move the object towards the player
            transform.position = Vector3.MoveTowards(transform.position, player.position + new Vector3(0,0.3f,0), moveSpeed * Time.deltaTime);

            // Shrink the object over time
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, shrinkSpeed * Time.deltaTime);

            // Destroy the object when it's small enough
            if (transform.localScale.magnitude < 0.05f)
            {
                
                Destroy(gameObject);
            }
        }
    }
}
