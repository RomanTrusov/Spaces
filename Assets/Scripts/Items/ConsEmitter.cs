using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ConsEmitter : MonoBehaviour
{

    [SerializeField] private GameObject itemToEmit;

    public void EmitItem(int count, float force)
    {
        // create amount of coins
        for (int i = 0; i < count; i++)
        {
            //Set random direction for coins
            Vector3 throwDirection = new Vector3(Random.Range(-1,1), Random.Range(-1, 1), Random.Range(-1, 1));
            //create coin object
            GameObject clone = (GameObject)Instantiate(itemToEmit,transform.position,transform.rotation);
            //set the direction of the coin
            clone.transform.Translate(new Vector3(throwDirection.x * force, throwDirection.y * force, throwDirection.z * force),Space.World);
        }
    }


}
