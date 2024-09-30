using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetEnemyType : MonoBehaviour
{
    

    public enum Types
    {
        None,
        Drone,
        Shadow
    }
    [Header("Select enemy type to let player's scripts interact with it properly\n")]
    public Types EnemyType;

}
