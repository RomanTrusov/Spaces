using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragObjectBySelf : MonoBehaviour
{
    [SerializeField]
    private Transform obj;

    // Update is called once per frame
    void FixedUpdate()
    {
        obj.position = transform.position;
        obj.rotation = transform.rotation;
    }
}
