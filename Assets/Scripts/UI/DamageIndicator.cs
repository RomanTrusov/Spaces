using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageIndicator : MonoBehaviour
{
    //damage position
    public Vector3 damageLocation;
    // player position
    public Transform orientation;
    // Pivot o fthe aarow
    public Transform damageImagePivot;

    // canvas to decrease alpha
    public CanvasGroup damageImageCanvas;
    // alpha timer vars
    public float fadeStartTime, fadeTime;
    float maxFadeTime;

    public Color alertColor; //alert color
    public Color attackColor; //attack color after 0.8f time
    private float colorChangeCD = 0.8f;

    Transform childImage;

    //enemyObject
    public GameObject enemy;

    // Start is called before the first frame update
    void Start()
    {
        // reset timer var
        maxFadeTime = fadeTime;
        //get child image
        childImage = this.gameObject.transform.GetChild(0);
        //set alert color of it
        childImage.gameObject.GetComponent<Image>().color = alertColor;
    }

    // Update is called once per frame
    void Update()
    {
        // timer for alpha
        if (fadeStartTime > 0)
        {
            fadeStartTime -= Time.deltaTime;
        } else
        {
            fadeTime -= Time.deltaTime;
            damageImageCanvas.alpha = fadeTime / maxFadeTime;
            if (fadeTime <= 0)
            {
                Destroy(this.gameObject);
            }
        }

        damageLocation = enemy.transform.position;

        // fix arrow clamp on top and bottom
        damageLocation.y = orientation.position.y;
        // get direction for the arrow
        Vector3 direction = (damageLocation - orientation.position).normalized;
        // get amgle for the pivot
        float angle = (Vector3.SignedAngle(direction, orientation.forward, Vector3.up));
        // set angle for the pivot
        damageImagePivot.transform.localEulerAngles = new Vector3(0,0, angle);

        //change color by timer
        if (colorChangeCD < 0) childImage.gameObject.GetComponent<Image>().color = attackColor;
        else colorChangeCD -= Time.deltaTime;
    }
}
