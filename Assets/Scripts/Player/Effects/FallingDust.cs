using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingDust : MonoBehaviour
{
    [SerializeField]
    private GameObject player;

    [SerializeField]
    private ParticleSystem fallingDustPS;

    // to avoid dusting in the air
    private bool isDustNeeded;

    void Start()
    {
        // deactivate dust by default
        fallingDustPS.gameObject.SetActive(false);
    }

   
    void Update()
    {
        //if player falling with high speed
        if (!player.GetComponent<PlayerMovement>().grounded && 
            player.GetComponent<Rigidbody>().velocity.y < -15f)
        {
            isDustNeeded = true;
        } 

        if (player.GetComponent<PlayerMovement>().grounded && isDustNeeded)
        {
            //sctivate PS
            fallingDustPS.gameObject.SetActive(true);
            //deactivate PS over time
            Invoke(nameof(deactivatePS), 1f);
            
            isDustNeeded = false;
        }

    }


    private void deactivatePS ()
    {
        fallingDustPS.gameObject.SetActive(false);
    }
}
