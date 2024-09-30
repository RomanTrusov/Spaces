using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGrappleOffset : MonoBehaviour
{
    [Header("Offset is needed because of the different ancor point on different enemies.\n")]
    //offset to make grapple point settable for every enemy type
    public Vector3 grappleOffset;

}
