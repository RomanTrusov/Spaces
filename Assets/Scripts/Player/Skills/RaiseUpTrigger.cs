using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaiseUpTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            var enemyTakeDamage = other.GetComponent<EnemyTakeHit>();
            if (enemyTakeDamage != null)
            {
                enemyTakeDamage.TakeHit(1, other.transform.up * 10);
            }
        }
    }
}
