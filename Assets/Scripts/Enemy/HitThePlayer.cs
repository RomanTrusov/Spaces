using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitThePlayer : MonoBehaviour
{

    //player obj
    private GameObject player;
    public bool wasPlayerDamaged;

    private void Start()
    {
        //find the player object
        player = GameObject.Find("Player");
    }

    public void HitPlayerOnce()
    {
        if (!wasPlayerDamaged)
        {
            //hit player once if delay is near zero
            player.GetComponent<PlayerMovement>().PlayerGetHit();
            wasPlayerDamaged = true;
        }
    }


}
