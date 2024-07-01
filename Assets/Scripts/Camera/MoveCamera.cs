using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public Transform cameraPosition;
    //Editor variables, you can customize these
    public float _tiltAmount = 5;


    // Update is called once per frame
    private void Update()
    {
        transform.position = cameraPosition.position;
    }
}
