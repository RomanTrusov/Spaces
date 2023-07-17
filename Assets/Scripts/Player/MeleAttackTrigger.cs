using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleAttackTrigger : MonoBehaviour
{
    public Transform orientation;
    public float forwardPush;
    public float upwardPush;
    public int meleDir = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8 && meleDir == 1)
        {
            other.gameObject.GetComponent<Rigidbody>().AddForce(orientation.forward * forwardPush + new Vector3(0f, upwardPush, 0f),ForceMode.Impulse);
            gameObject.SetActive(false);
        }
    }



}
