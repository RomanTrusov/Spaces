using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTakeHit : MonoBehaviour
{
    [Header("This script initiates take hit and push force actions while enemy's damage")]
    [SerializeField] private float pushForce;

    public void TakeHit(float damage, Vector3 pushAngle)
    {
        switch (GetComponent<GetEnemyType>().EnemyType)
        {
            case GetEnemyType.Types.Drone:
                GetComponent<EnemyBehaviourDrone>().TakeHit(damage);
                GetComponent<EnemyBehaviourDrone>().Push(pushAngle, pushForce);
                break;
            case GetEnemyType.Types.Shadow:
                GetComponent<EnemyBehaviourShadow>().TakeHit(damage);
                GetComponent<EnemyBehaviourShadow>().Push(pushAngle, pushForce);
                break;
            case GetEnemyType.Types.None:
                break;
        }
    }

}
