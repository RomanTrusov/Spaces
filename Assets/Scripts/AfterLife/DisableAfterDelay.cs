using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableAfterDelay : MonoBehaviour
{
    //var for the delay time
    public float disableDelay;
    
    void Start()
    {
        Invoke(nameof(Deactivate), disableDelay);
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }

}
