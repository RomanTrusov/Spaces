using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaiseUpTrigger : MonoBehaviour
{

    public float RaiseUpDamage;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            var enemyTakeDamage = other.GetComponent<EnemyTakeHit>();
            if (enemyTakeDamage != null)
            {
                enemyTakeDamage.TakeHit(10, other.transform.up * 3);
            }
        }
    }
}
