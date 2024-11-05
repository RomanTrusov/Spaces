using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragObjectBySelf : MonoBehaviour
{
    [SerializeField]
    private Transform obj;
    public Vector3 offset;

    // Update is called once per frame
    void FixedUpdate()
    {
        obj.position = transform.position + offset;
        obj.rotation = transform.rotation;
    }
}
