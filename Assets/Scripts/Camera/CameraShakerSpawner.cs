using System;
using System.Collections;
using System.Collections.Generic;
using CameraShake;
//using UnityEditor.SceneManagement;
using UnityEngine;

public class CameraShakerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _shakeObject;

    private void Start()
    {
        _shakeObject.AddComponent<CameraShaker>();
        Destroy(this);
    }
}
