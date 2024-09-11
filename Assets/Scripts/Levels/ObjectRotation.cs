using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRotation : MonoBehaviour
{

    [SerializeField]
    private float XRotation;
    [SerializeField]
    private float YRotation;
    [SerializeField]
    private float ZRotation;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(XRotation/100, YRotation/100, ZRotation/100,Space.Self);
    }
}
