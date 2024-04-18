using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CrackOnDamage : MonoBehaviour
{
    public bool gotHit;

    [SerializeField]
    private PlayerMovement pm;

    //crackAlphaCD
    float crackFadeCD = 1f;
    float crackFadeCDTimer = 0;

    // Start is called before the first frame update
    void Start()
    {

        //make transparent
        GetComponent<RawImage>().color = new Color(0,0,0,0);
    }

    // Update is called once per frame
    void Update()
    {

        if (pm.attacked) gotHit = true;

        if (gotHit) //activate crack and reset timer and gothit
        {
            //make cracks visible
            GetComponent<RawImage>().color = new Color(1, 1, 1, 1);
            // reset timer
            crackFadeCDTimer = crackFadeCD;
            gotHit = false;
        } //reduce alpha crack in timer
        else if (crackFadeCDTimer > 0)
        {
            crackFadeCDTimer -= Time.deltaTime;
            GetComponent<RawImage>().color = new Color(1, 1, 1, crackFadeCDTimer);
        } else
        {
            crackFadeCDTimer = 0;
            GetComponent<RawImage>().color = new Color(0, 0, 0, 0);
        }

    }
}
