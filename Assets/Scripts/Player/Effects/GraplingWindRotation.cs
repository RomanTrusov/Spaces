using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraplingWindRotation : MonoBehaviour
{
    public LineRenderer playerLR;
    public ParticleSystem graplingWind;
    public float rotationSpeed;
    private float lrLength;
    ParticleSystem.TrailModule effectTrailModule;

    void Start()
    {
        effectTrailModule = graplingWind.trails;
    }

    
    void LateUpdate()
    {
        // find rotation to the end point of the grapling
        if (playerLR.enabled)
        {
            Invoke(nameof(ActivateEffect), 0.3f); //activate effect with delay
            lrLength = Vector3.Distance(playerLR.GetPosition(0),playerLR.GetPosition(1)); //get length to calcilate lerping color to zero
            Vector3 lookPos = transform.position - playerLR.GetPosition(1); // position o fthe point 1
            Quaternion rotation = Quaternion.LookRotation(lookPos); // set rotation goal
            transform.rotation = Quaternion.Slerp(transform.rotation,rotation,Time.deltaTime * rotationSpeed); //lerp angle

            //lerping color to zero during the effect
            if (graplingWind.gameObject.activeSelf)
            {
                Color curentColor = effectTrailModule.colorOverTrail.color;
                Color lerpColor = Color.Lerp(curentColor, new Color(0, 0, 0, 0), Time.deltaTime);
                effectTrailModule.colorOverTrail = lerpColor;
            }
        } else
        {
            effectTrailModule.colorOverTrail = new Color(1, 1, 1, 0.2f);
            graplingWind.gameObject.SetActive(false);
        }
            

    }


    private void ActivateEffect ()
    {
        graplingWind.gameObject.SetActive(true);
    }

 
}
