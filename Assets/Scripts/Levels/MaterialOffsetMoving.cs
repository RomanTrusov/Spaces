using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MaterialOffsetMoving : MonoBehaviour
{
    public Material material;  // Assign the material in the Inspector

    // Define the ranges for X and Y speed
    public Vector2 xSpeedRange = new Vector2(0.05f, 0.2f); // Min and Max X speed
    public Vector2 ySpeedRange = new Vector2(0.05f, 0.2f); // Min and Max Y speed

    private Vector2 scrollSpeed;
    private Vector2 newScrollSpeed;
    private Vector2 currentOffset = Vector2.zero;

    private float randomizeInterval = 1f;  // Time interval (1 second) to randomize the speed
    private float timeSinceLastRandomization = 0f;

    void Start()
    {
        // Initialize the scroll speed
        RandomizeScrollSpeed();
    }

    void Update()
    {
        // Update the offset over time
        currentOffset += scrollSpeed * Time.deltaTime;
        // Apply the updated offset to the material
        material.mainTextureOffset = currentOffset;
        // Update the timer
        timeSinceLastRandomization += Time.deltaTime;

        // Check if 1 second has passed
        if (timeSinceLastRandomization >= randomizeInterval)
        {
            // Randomize the speed again
            RandomizeScrollSpeed();
            // Reset the timer
            timeSinceLastRandomization = 0f;
        }

        // Increase the offset over time
        currentOffset += scrollSpeed * Time.deltaTime * 0.1f;
        // Apply the updated offset to the material
        material.mainTextureOffset = currentOffset;

        if (scrollSpeed != newScrollSpeed)
        {
            scrollSpeed = Vector2.Lerp(scrollSpeed,newScrollSpeed,Time.deltaTime);
        }

    }

    // Randomize scroll speed within the specified ranges
    void RandomizeScrollSpeed()
    {
        float randomXSpeed = Random.Range(xSpeedRange.x, xSpeedRange.y);
        float randomYSpeed = Random.Range(ySpeedRange.x, ySpeedRange.y);

        // Set the scroll speed to the random values
        newScrollSpeed = new Vector2(randomXSpeed, randomYSpeed);
    }
}
