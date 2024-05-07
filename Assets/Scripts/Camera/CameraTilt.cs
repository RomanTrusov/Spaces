using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTilt : MonoBehaviour
{
    //Editor variables, you can customize these
    public float _tiltAmount = 5;
    //public KeyCode _leftBtn = KeyCode.A; //A is default
    //public KeyCode _rightBtn = KeyCode.D; //D is default
    float hInput;

    // Update is called once per frame
    void Update()
    {
        hInput = -Input.GetAxisRaw("Horizontal");
        // If _leftBtn key is hit, rotate Z axis of camera by _tiltAmount
        if (hInput < 0)
        {
            this.transform.Rotate(0, 0, _tiltAmount);
        }

        // Same as above, but inverted values
        if (hInput > 0)
        {
            this.transform.Rotate(0, 0, -_tiltAmount);
        }

    }
}
