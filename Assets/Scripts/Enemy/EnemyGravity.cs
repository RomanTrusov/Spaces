using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGravity : MonoBehaviour
{
    private Rigidbody rb;
    private Vector3 gravityForce;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        gravityForce = new Vector3(0f,-5f,0f);
    }
    private void FixedUpdate()
    {
        rb.AddForce(gravityForce, ForceMode.Force);
    }
}
