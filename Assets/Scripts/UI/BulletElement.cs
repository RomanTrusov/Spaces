using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BulletElement : MonoBehaviour
{
    [SerializeField] private Image _image;

    public bool IsVisible => _image.enabled;
    
    private void Start()
    {
        SetVisible(true);
    }

    public void SetVisible(bool visible)
    {
        _image.enabled = visible;
    }
}
