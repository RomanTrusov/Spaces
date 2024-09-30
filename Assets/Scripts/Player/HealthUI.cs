using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HealthUI : MonoBehaviour
{

    public Text text;
    public PlayerMovement pm;


    // Update is called once per frame
    void FixedUpdate()
    {
        text.text = "Health: " + pm.playerHP;
    }
}
