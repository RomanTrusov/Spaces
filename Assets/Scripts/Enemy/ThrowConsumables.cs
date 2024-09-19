using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ThrowConsumables : MonoBehaviour
{
    [SerializeField] private GameObject consumableEmitter;
    [SerializeField] int amountOfItems;
    [SerializeField] float force;
 

    public void AddEmitter()
    {
        GameObject consEmitter = (GameObject)Instantiate(consumableEmitter,transform);
        if (consEmitter.GetComponent<ConsEmitter>() != null)
        {
            consEmitter.GetComponent<ConsEmitter>().EmitItem(amountOfItems,force);
        }
    }
}
