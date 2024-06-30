using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using DG.Tweening;

public class BulletsUI : MonoBehaviour
{
    [SerializeField] private GunController _gunController;
    [SerializeField] private RectTransform _bulletContainer;
    [SerializeField] private CanvasGroup _noAmmoIndicator;
    [SerializeField] private CanvasGroup _ammoAddedIndicator;

    private List<BulletElement> _bulletElements = new List<BulletElement>(16);
    private List<IDisposable> _disposables = new List<IDisposable>();

    private Tween _noAmmoTween;
    private Tween _ammoAddedTween;
    
    private void Start()
    {
        _ammoAddedIndicator.DOFade(0f, 0f);
    }

    private void OnEnable()
    {
        _gunController.OnSwapGun += InitBullets;
        _gunController.OnSwapGun += KillAnimations;
    }    
    
    private void OnDisable()
    {
        _gunController.OnSwapGun -= InitBullets;
        _gunController.OnSwapGun -= KillAnimations;
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
        currentGun.OnShootNoAmmo = NoAmmoIndication;

        UpdateBullets(currentGun.RemainingBullets.Value);
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
        int activeBullets = 0;
        foreach (var bullet in _bulletElements)
        {
            if (bullet.IsVisible)
                activeBullets++;
        }
        
        if (amount > activeBullets)
            AmmoAddedIndication();
        
        for (int i = 0; i < _bulletElements.Count; i++)
        {
            _bulletElements[i].SetVisible(amount >= i + 1);
        }
    }

    private void NoAmmoIndication()
    {
        if (_noAmmoTween != null)
            _noAmmoTween.Kill();
        
        var duration = 0.25f;
        _noAmmoIndicator.alpha = 0f;
        _noAmmoTween = _noAmmoIndicator.DOFade(1f, duration).SetLoops(4, LoopType.Yoyo);
    }    
    
    private void AmmoAddedIndication()
    {
        if (_ammoAddedTween != null)
            _ammoAddedTween.Kill();
        
        var duration = 0.5f;
        _ammoAddedIndicator.alpha = 0f;
        _ammoAddedTween = _ammoAddedIndicator.DOFade(1f, duration).SetLoops(2, LoopType.Yoyo);
    }
    
    private void KillAnimations(Gun currentGun)
    {
        if (_noAmmoTween != null)
            _noAmmoTween.Kill(true);
        
        if (_ammoAddedTween != null)
            _ammoAddedTween.Kill(true);
    }
}
