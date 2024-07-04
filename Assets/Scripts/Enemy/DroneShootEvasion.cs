using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DroneShootEvasion : MonoBehaviour
{
    private float _turnOnChance = 0.5f;
    
    private void OnTriggerEnter(Collider other)
    {
        var projectile = other.GetComponent<Projectile>();
        if (projectile != null && projectile.source == ProjectileSource.Player)
            TryTurnOn();
    }

    private void TryTurnOn()
    {
        var random = Random.Range(0f, 1f);
        if (random > _turnOnChance)
            return;
        
        Debug.Log("!!! Turn on evasion!");
    }
}
