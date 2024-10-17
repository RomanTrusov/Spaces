using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LasersFromTheSky : MonoBehaviour
{
    [Header("Set top and bottom fields where lasers will startand end")]
    public float yTop;
    public float yBottom;

    [Header("Scale of the top and bottom fields as half-sqaures")]
    public float filedScale;

    [Header("Set delay betweeb laser creations")]
    public float laserCreateEverySec;
    private float laserCreateEverySecTimer;

    [Header("Sample of the laser")]
    public GameObject laserSample;

    //will be set auto in start
    private GameObject player;

    private void Start()
    {
        //find the player object
        player = GameObject.Find("Player");

        //reset the timer
        laserCreateEverySecTimer = laserCreateEverySec;

        //check for disable the sample
        if (laserSample.activeSelf) laserSample.SetActive(false);

    }

    private void Update()
    {
        laserCreateEverySecTimer -= Time.deltaTime;
        if (laserCreateEverySecTimer <= 0)
        {
            //TODO Instantiate laser
            InstantiateLaser();
            //reset the timer
            laserCreateEverySecTimer = laserCreateEverySec;
        }
    }

    private void InstantiateLaser()
    {
        //--DO ONCE
        //get random top point
        float xRandom = Random.Range(-filedScale,filedScale);
        float zRandom = Random.Range(-filedScale,filedScale);
        Vector3 topPoint = new Vector3(xRandom,yTop,zRandom);

        //get random bottom point
        xRandom = Random.Range(-filedScale, filedScale);
        zRandom = Random.Range(-filedScale, filedScale);
        Vector3 bottomPoint = new Vector3(xRandom, yBottom, zRandom);

        //get direction from top to bottom
        Vector3 direction = bottomPoint - topPoint;
        // normilize direction
        Vector3 directionNormalized = direction.normalized;
        //get distance
        float distance = direction.magnitude;

        //create laser with right rotation
        GameObject laser = Instantiate(laserSample,topPoint, Quaternion.LookRotation(directionNormalized),transform);

        //set Z scale as distance between points
        Vector3 newScale = new Vector3(laser.transform.localScale.x, laser.transform.localScale.y,distance);
        laser.transform.localScale = newScale;
        if (!laser.activeSelf) laser.SetActive(true);

    }


}
