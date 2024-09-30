using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRotation : MonoBehaviour
{

    [SerializeField]
    private Vector3 XYZRotation;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(XYZRotation.x/100, XYZRotation.y/100, XYZRotation.z/100, Space.Self);
    }
}
