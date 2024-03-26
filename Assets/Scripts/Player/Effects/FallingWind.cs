using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingWind : MonoBehaviour
{
    [SerializeField]
    private Rigidbody playerRb;

    [SerializeField]
    private ParticleSystem playerPS;
    private ParticleSystem.EmissionModule playerPSEmissionModule;
    private ParticleSystem.TrailModule playerPSTrailModule;

    private float emissionDesired;

    //=======================

    private void Start()
    {
        // get Emission and Color modules from PS
        playerPSEmissionModule = playerPS.emission;
        playerPSTrailModule = playerPS.trails;
    }
    //=======================
    void Update()
    {
        //if player falling fast
        if (playerRb.velocity.y < -15f)
        {
            // set default white color of the trail
            playerPSTrailModule.colorOverTrail = new Color(1,1,1,0.2f);
            // lerping to desired RoT
            emissionDesired = -playerRb.velocity.y * 2;
            playerPSEmissionModule.rateOverTime = Mathf.Lerp(playerPSEmissionModule.rateOverTime.constant, emissionDesired, Time.deltaTime*50);
        }
        else
        { // if not falling - lerping it to 0
            playerPSEmissionModule.rateOverTime = 0;
            //lerp color to zero
            Color curentColor = playerPSTrailModule.colorOverTrail.color;
            Color lerpColor = Color.Lerp(curentColor, new Color(0,0,0,0),Time.deltaTime * 8);
            playerPSTrailModule.colorOverTrail = lerpColor;

        }
    }
}
