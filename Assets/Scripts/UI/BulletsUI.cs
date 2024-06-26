using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class BulletsUI : MonoBehaviour
{
    [SerializeField] private GunController _gunController;
    [SerializeField] private RectTransform _bulletContainer;

    private List<BulletElement> _bulletElements = new List<BulletElement>(16);
    private List<IDisposable> _disposables = new List<IDisposable>();
    
    private void OnEnable()
    {
        _gunController.OnSwapGun += InitBullets;
    }    
    
    private void OnDisable()
    {
        _gunController.OnSwapGun -= InitBullets;
    }

    private void OnDestroy()
    {
        foreach (var disposable in _disposables)
        {
            disposable.Dispose();
        }
        
        _disposables.Clear();
    }

    private void InitBullets(Gun currentGun)
    {
        DestroyBullets();
        
        for (int i = 0; i < currentGun.ProjectilesPerMag; i++)
        {
            var bullet = Instantiate(currentGun.bulletUIPrefab, _bulletContainer);
            _bulletElements.Add(bullet);
        }
        
        _bulletElements.Reverse();
        currentGun.RemainingBullets.Subscribe(UpdateBullets).AddTo(_disposables);
    }

    private void DestroyBullets()
    {
        foreach (var bullet in _bulletElements)
        {
            if (bullet != null)
                Destroy(bullet.gameObject);
        }
        
        _bulletElements.Clear();
    }

    private void UpdateBullets(int amount)
    {
        for (int i = 0; i < _bulletElements.Count; i++)
        {
            _bulletElements[i].SetVisible(amount >= i + 1);
        }
    }
}
