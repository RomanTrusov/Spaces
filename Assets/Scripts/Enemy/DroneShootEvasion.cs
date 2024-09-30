using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DroneShootEvasion : MonoBehaviour
{
    [SerializeField] private float _turnOnChance = 0.5f;
    [SerializeField] private float _duration = 5f;
    [SerializeField] private EnemyBehaviourDrone _drone;
    [SerializeField] private GameObject _indicator;
    
    private bool _turnedOn = false;

    private void Start()
    {
        TurnOff();
    }

    private void OnTriggerEnter(Collider other)
    {
        var projectile = other.GetComponent<Projectile>();
        if (projectile != null && projectile.source == ProjectileSource.Player)
            TryTurnOn();
    }

    public void TryEvade()
    {
        if (!_turnedOn)
            return;

        var randomSide = Random.Range(0, 2) <= 1 ? _drone.transform.right : -_drone.transform.right;
        _drone.Push(randomSide, 15f);
    }

    private void TryTurnOn()
    {
        var random = Random.Range(0f, 1f);
        if (random > _turnOnChance)
            return;

        TurnOn();
    }

    private void TurnOn()
    {
        _turnedOn = true;
        _indicator.SetActive(true);
        
        StartCoroutine(TurnOffCoroutine());
    }
    
    private void TurnOff()
    {
        _turnedOn = false;
        _indicator.SetActive(false);
    }

    private IEnumerator TurnOffCoroutine()
    {
        yield return new WaitForSeconds(_duration);

        TurnOff();
    }
}
