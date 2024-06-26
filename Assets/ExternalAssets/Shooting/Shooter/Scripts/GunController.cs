using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GunController : MonoBehaviour
{
    public Action<Gun> OnSwapGun;
    
    public Transform weaponHold;
    public List<Gun> Guns;
    public Gun equippedGun;
    private int _currentGunIndex;
    
    //TFG
    [SerializeField] private Transform _gunContainer;

    public void Start()
    {
        if (Guns[0] != null)
            EquipGun(0);
    }

    public void Update()
    {
        // This stuff should be in the player class caliing the methods of the same name in this class
        if (Input.GetMouseButton(0))
        {
            OnTriggerHold();
        }
        if (Input.GetMouseButtonUp(0))
        {
            OnTriggerReleased();
        }
        
        if (Input.GetKeyDown(KeyCode.G))
        {
            EquipNextWeapon();
        }
    }

    private void EquipNextWeapon()
    {
        var nextWeaponIndex = _currentGunIndex + 1;
        
        if (nextWeaponIndex > Guns.Count - 1)
            nextWeaponIndex = 0;
        
        EquipGun(nextWeaponIndex);
    }

    public void EquipGun(int weaponIndex)
    {
        if (weaponIndex >= 0 && weaponIndex <= Guns.Count)
        {
            EquipGun(Guns[weaponIndex]);
            _currentGunIndex = weaponIndex;
        }
    }

	public void EquipGun(Gun gunToEquip)
    {
        if (equippedGun != null)
            Destroy(equippedGun.gameObject);

        equippedGun = Instantiate(gunToEquip, weaponHold.position, weaponHold.rotation) as Gun;
        equippedGun.transform.parent = weaponHold;
        equippedGun.gunContainer = _gunContainer;
        
        OnSwapGun?.Invoke(equippedGun);
    }

    public void OnTriggerHold()
    {
        if (equippedGun != null)
        {
            equippedGun.OnTriggerHold();
        }
    }

    public void OnTriggerReleased()
    {
        if (equippedGun != null)
        {
            equippedGun.OnTriggerReleased();
        }
    }

    public void Reload()
    {
        if (equippedGun != null)
        {
            equippedGun.Reload();
        }
    }

    public void AddAmmo(int amount)
    {
        equippedGun.AddAmmo(amount);
    }
}
