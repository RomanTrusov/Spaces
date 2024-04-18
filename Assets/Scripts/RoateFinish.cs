using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoateFinish : MonoBehaviour
{


    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.one * 4 * Time.deltaTime);
    }
}
