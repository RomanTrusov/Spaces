using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBehaviour : MonoBehaviour
{
    private float laserLifeTime;
    private float laserLifeTimeTimer;

    private bool canAttack;
    private bool wasPlayerAttacked;

    //color of the laser
    [ColorUsage(true, true)]
    public Color laserColor;
    private float alpha;
    private MaterialPropertyBlock block;
    private Renderer renderer;

    //vars to scale down
    private float xScale;
    private float yScale;

    private void Start()
    {
        //get laser lifetime from other script
        laserLifeTime = gameObject.GetComponent<DestroyAfterDelay>().destroyTimer;

        //set life time timer to 0 aat start
        laserLifeTimeTimer = 0;

        //get material vars
        renderer = GetComponent<Renderer>();
        block = new MaterialPropertyBlock();
        renderer.GetPropertyBlock(block);
        //set transparent laser at the beginning
        alpha = 0f;
        laserColor.a = alpha;
        block.SetColor("_Color", laserColor);
        renderer.SetPropertyBlock(block);

    }
    private void Update()
    {
        //start the timer
        laserLifeTimeTimer += Time.deltaTime;

        if (laserLifeTimeTimer < laserLifeTime / 5)
        {
            //lerp the alpha from 0 to 1
            alpha = Mathf.Lerp(alpha,1,Time.deltaTime / (laserLifeTime / 10));
            laserColor.a = alpha;
            block.SetColor("_Color", laserColor);
            renderer.SetPropertyBlock(block);

        } else if (laserLifeTimeTimer >= laserLifeTime / 5 && (laserLifeTimeTimer < (laserLifeTime - laserLifeTime / 5)))
        {
            //if player was not attacked - try to on trigger enter
            if (!wasPlayerAttacked) canAttack = true;
            //if it was attacker once - do not attack again
            else canAttack = false;


        } else if (laserLifeTimeTimer >= (laserLifeTime - laserLifeTime / 5))
        {
            //do not attack on disappear state
            canAttack = false;
            //slpah to 0
            alpha = Mathf.Lerp(alpha, 0, Time.deltaTime / (laserLifeTime / 15));
            laserColor.a = alpha;
            block.SetColor("_Color", laserColor);
            renderer.SetPropertyBlock(block);
            //reduce the diameter
            xScale = Mathf.Lerp(transform.localScale.x,0, Time.deltaTime / (laserLifeTime / 15));
            yScale = Mathf.Lerp(transform.localScale.y,0, Time.deltaTime / (laserLifeTime / 15));
            Vector3 newScale = new Vector3(xScale,yScale,transform.localScale.z);
            transform.localScale = newScale;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        //sttack player once on trigger enter
        if (other.gameObject.layer.Equals(3) && canAttack)
        {
            gameObject.GetComponent<HitThePlayer>().HitPlayerOnce();
            wasPlayerAttacked = true;
        }
    }

}
