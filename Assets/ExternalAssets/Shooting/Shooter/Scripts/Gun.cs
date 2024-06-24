﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Gun : MonoBehaviour 
{

    public enum FireMode
    {
        Single,
        Burst,
        Automatic
    }

    [Header("Stats")]
    public float MuzzleVelocity;
    public float ChargedMuzzleVelocity;
    public FireMode fireMode = FireMode.Single;
    public int burstCount;
    public float msBetweenShots = 100;
    public int ProjectilesPerMag;
    public float reloadTime = 0.3f;
    public float chargedShootTime = 2f;
    
    [Header("Projectile")]
    public Projectile projectile;
    public Projectile projectileCharged;
    public int mainShotSpawns = 1;
    public int chargedShotSpawns = 1;
    public Vector2 randomShootDeviationX;
    public Vector2 randomShootDeviationY;
    public Transform[] ProjectileSpawns;
    public List<Vector3> InitialProjectileSpawnRotations = new List<Vector3>();

    [Header("Effects")]
    public Transform shell;
    public Transform shellEjection;
    public AudioClip shootAudio;
    public AudioClip reloadAudio;
    public Transform chargedShoot;
    
    AudioSource source;

    [Header("Recoil")]
    public Vector2 kickMinMax = new Vector2(.05f, .2f);
    public Vector2 recoilAngleMinMax = new Vector2(3, 5);
    public float recoilMoveSettleTime = .1f;
    public float recoilRotationSettleTime = .1f;

    Vector3 recoilSmoothDampVelocity;
    float recoilRotSmoothDampVelocity;
    float recoilAngle;

    Muzzleflash muzzleFlash;

    float nextShotTime;
    bool triggerReleasedSinceLastShot = true;
    int shotsRemainingInBurst;
    int projectilesRemainingInMag;

    bool isReloading;
    
    //TFG
    public Transform gunContainer;
    private float _chargedShootTimer;
    private Vector3 _initialPosition;

    // Use this for initialization
    void Start ()
    {
        muzzleFlash = GetComponent<Muzzleflash>();
        shotsRemainingInBurst = burstCount;
        projectilesRemainingInMag = ProjectilesPerMag;
        source = GetComponent<AudioSource>();
        chargedShoot.localScale = Vector3.zero;

        SetInitialPositionsRotations();
    }

    private void SetInitialPositionsRotations()
    {
        for (int i = 0; i < ProjectileSpawns.Length; i++)
        {
            InitialProjectileSpawnRotations.Add(ProjectileSpawns[i].localRotation.eulerAngles);
        }

        _initialPosition = gunContainer.localPosition;
    }

    void LateUpdate()
    {
        // animate recoil
        gunContainer.localPosition = Vector3.SmoothDamp(gunContainer.localPosition, _initialPosition, ref recoilSmoothDampVelocity, recoilMoveSettleTime);
        
        if (!isReloading)
        {
            recoilAngle = Mathf.SmoothDamp(recoilAngle, 0, ref recoilRotSmoothDampVelocity, recoilRotationSettleTime);
            gunContainer.localEulerAngles = Vector3.left * recoilAngle;
            if (projectilesRemainingInMag == 0)
                Reload();
        }
    }

    void Shoot()
    {
        if (!isReloading && Time.time > nextShotTime && projectilesRemainingInMag > 0)
        {
            // Firemodes
            if (fireMode == FireMode.Burst)
            {
                if (shotsRemainingInBurst == 0)
                {
                    return;
                }
                shotsRemainingInBurst--;
            }
            else if (fireMode == FireMode.Single)
            {
                if (!triggerReleasedSinceLastShot) return;
            }


            nextShotTime = Time.time + msBetweenShots / 1000f;
            
            SpawnProjectile(projectile, mainShotSpawns);
            ShootEffect();
        }
    }

    private void ShootEffect()
    {
        // Eject shell
        Instantiate(shell, shellEjection.position, shellEjection.rotation);
        // Muzzleflash
        muzzleFlash.Activate();

        // Initiate Recoil
        gunContainer.localPosition -= Vector3.forward * Random.Range(kickMinMax.x, kickMinMax.y);
        recoilAngle += Random.Range(recoilAngleMinMax.x, recoilAngleMinMax.y);
        recoilAngle = Mathf.Clamp(recoilAngle, 0, 30);

        source.PlayOneShot(shootAudio, 1);
    }

    private void SpawnProjectile(Projectile projectileToSpawn, int limit = 1)
    {
        var spawnsCount = Mathf.Clamp(ProjectileSpawns.Length, 0, limit);
        for (int i = 0; i < spawnsCount; i++)
        {
            if (projectilesRemainingInMag == 0) break;
            projectilesRemainingInMag--;

            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hitInfo,
                    2000f))
            {
                Vector3 direction = hitInfo.point - ProjectileSpawns[i].transform.position;
                ProjectileSpawns[i].transform.rotation = Quaternion.LookRotation(direction);
            }
            else
            {
                ProjectileSpawns[i].transform.localRotation = Quaternion.Euler(InitialProjectileSpawnRotations[i]);
            }

            var finalRotation = ProjectileSpawns[i].rotation;
            if (i > 1)
            {
                float randomXDeviation = UnityEngine.Random.Range(randomShootDeviationX.x, randomShootDeviationX.y);
                float randomYDeviation = UnityEngine.Random.Range(randomShootDeviationY.x, randomShootDeviationY.y);
                var euler = ProjectileSpawns[i].rotation.eulerAngles;
                var randomizedAngle = Quaternion.Euler(euler.x + randomXDeviation, euler.y + randomYDeviation, euler.z);
                finalRotation = randomizedAngle;
            }
            Projectile newProjectile =
                Instantiate(projectileToSpawn, ProjectileSpawns[i].position, finalRotation) as Projectile;
        }
    }

    public void Reload()
    {
        if (!isReloading && projectilesRemainingInMag != ProjectilesPerMag)
        {
            StartCoroutine(AnimateReload());
        }
    }

    IEnumerator AnimateReload()
    {
        isReloading = true;
        yield return new WaitForSeconds(.2f);

        source.PlayOneShot(reloadAudio, 1);

        float reloadSpeed = 1 / reloadTime;
        float percent = 0;

        Vector3 initialRot = transform.localEulerAngles;
        float maxReloadAngle = 30.0f;

        while (percent < 1)
        {
            percent += Time.deltaTime * reloadSpeed;

            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
            float reloadAngle = Mathf.Lerp(0, maxReloadAngle, interpolation);
            transform.localEulerAngles = initialRot + Vector3.left * reloadAngle;

            yield return null;
        }

        isReloading = false;
        projectilesRemainingInMag = ProjectilesPerMag;
    }

    public void OnTriggerHold()
    {
        Shoot();
        HandleChargedShoot();
        
        triggerReleasedSinceLastShot = false;
    }

    public void OnTriggerReleased()
    {
        triggerReleasedSinceLastShot = true;
        shotsRemainingInBurst = burstCount;

        ResetChargedShoot();
    }
    
    private void HandleChargedShoot()
    {
        _chargedShootTimer += Time.deltaTime;
        float normalizedCharge = _chargedShootTimer / chargedShootTime;
        chargedShoot.localScale = Vector3.one * normalizedCharge;

        if (_chargedShootTimer >= chargedShootTime)
        {
            ChargedShoot();
        }
    }

    private void ChargedShoot()
    {
        ResetChargedShoot();
        
        SpawnProjectile(projectileCharged, chargedShotSpawns);
        ShootEffect();
    }

    private void ResetChargedShoot()
    {
        _chargedShootTimer = 0;
        chargedShoot.localScale = Vector3.zero;
    }
}
