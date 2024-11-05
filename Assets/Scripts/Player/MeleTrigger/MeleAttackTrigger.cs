using System.Collections;
using System.Collections.Generic;
using CameraShake;
using UnityEngine;

public class MeleAttackTrigger : MonoBehaviour
{
    public MeleAttack playerMeleAttack;
    private GameObject player;
    private PlayerMovement playerMove;
    private Grapling playerGr;
    private Rigidbody playerRB;

    public Transform orientation;
    public float forwardPush;
    public float upwardPush;
    public float meleAttackModifier;

    [Header("Vars for push from attack while in grapple")]
    public float pushFromEnemyForce;
    public float yPush;
    public float secWithoutSpeedLimit;

    [SerializeField] private GameObject punchEffect;
    [SerializeField] private Grapling _grapling;
    [SerializeField] private GunController _gunController;

    private Transform enemyTransform;

    public int meleState = 0;
    public int meleDirection = 0;

    private float _grapplingDamageMult = 2;
    private int _ammoAddingPerHit = 2;

    private void Start()
    {
        //get player obj and components
        player = playerMeleAttack.gameObject;
        playerMove = player.GetComponent<PlayerMovement>();
        playerRB = player.GetComponent<Rigidbody>();
        playerGr = player.GetComponent<Grapling>();
    }

    // on trigger enter
    private void OnTriggerEnter(Collider other)
    {
        // in it's enemy and simple punch - punch enemy
        if (other.gameObject.layer == 8 && meleState == 1)
        {
            // get enemy GO
            enemyTransform = other.gameObject.transform;

            InitiatePunchEffect();
            InitiateAttackOnEnemy(other);

            //if player grappled while attacking
            if (playerMove.activeGrapple)
            {
                //stop grapple
                playerGr.StopGrapple();
                //push direction
                Vector3 pushDirection = (playerRB.position - other.transform.position).normalized;
                pushDirection.y = yPush;
                //stop speed limit for a short time
                playerMove.DisableMoveSpeed(secWithoutSpeedLimit);
                //push
                playerRB.AddForce(pushDirection * pushFromEnemyForce, ForceMode.Impulse);
            }

            switch (meleDirection)
            {
                case 0:
                    other.gameObject.GetComponent<Rigidbody>().AddForce((orientation.forward * forwardPush + new Vector3(0f, upwardPush, 0f)), ForceMode.Impulse);
                    break;
                case 1:
                    other.gameObject.GetComponent<Rigidbody>().AddForce((orientation.forward * forwardPush + new Vector3(0f, upwardPush, 0f)), ForceMode.Impulse);
                    break;
                case 2:
                    other.gameObject.GetComponent<Rigidbody>().AddForce((orientation.right * forwardPush + new Vector3(0f, upwardPush, 0f)), ForceMode.Impulse);
                    break;
                case 3:
                    other.gameObject.GetComponent<Rigidbody>().AddForce((orientation.up * forwardPush + new Vector3(0f, upwardPush, 0f)), ForceMode.Impulse);
                    break;
                case 4:
                    other.gameObject.GetComponent<Rigidbody>().AddForce((orientation.right * -forwardPush + new Vector3(0f, upwardPush, 0f)), ForceMode.Impulse);
                    break;
            }
            gameObject.SetActive(false);
        }
        //if it's enemy and it's a combo attack - punch harder
        else if (other.gameObject.layer == 8 && meleState == 2)
        {
            InitiatePunchEffect();
            InitiateAttackOnEnemy(other);

            switch (meleDirection)
            {
                case 0:
                    other.gameObject.GetComponent<Rigidbody>().AddForce((orientation.forward * forwardPush + new Vector3(0f, upwardPush, 0f)) * meleAttackModifier, ForceMode.Impulse);
                    break;
                case 1: 
                    other.gameObject.GetComponent<Rigidbody>().AddForce((orientation.forward * forwardPush + new Vector3(0f, upwardPush, 0f)) * meleAttackModifier, ForceMode.Impulse);
                    break;
                case 2:
                    other.gameObject.GetComponent<Rigidbody>().AddForce((orientation.right * forwardPush + new Vector3(0f, upwardPush, 0f)) * meleAttackModifier, ForceMode.Impulse);
                    break;
                case 3:
                    other.gameObject.GetComponent<Rigidbody>().AddForce((orientation.up * forwardPush + new Vector3(0f, upwardPush, 0f)) * meleAttackModifier, ForceMode.Impulse);
                    break;
                case 4:
                    other.gameObject.GetComponent<Rigidbody>().AddForce((orientation.right * -forwardPush + new Vector3(0f, upwardPush, 0f)) * meleAttackModifier, ForceMode.Impulse);
                    break;
            }

            meleState = 0;
            meleDirection = 0;
            gameObject.SetActive(false);
        }
    }

    private void InitiateAttackOnEnemy(Collider other)
    {
        float damageMultiplier = _grapling.Grappling ? _grapplingDamageMult : 1;
        var enemyTakeDamage = other.GetComponent<EnemyTakeHit>();
        if (enemyTakeDamage != null)
        {
            enemyTakeDamage.TakeHit(playerMeleAttack.playerDamage * (int)damageMultiplier, other.transform.position - transform.position);
            _gunController.AddAmmo(_ammoAddingPerHit);
        }

        //this code works with drone only
        /*var droneBehaviour = other.GetComponent<EnemyBehaviourDrone>();
        if (droneBehaviour != null)
        {
            droneBehaviour.TakeHit(playerMeleAttack.playerDamage * (int)damageMultiplier);
            droneBehaviour.Push(other.transform.position - transform.position, 5f);
            _gunController.AddAmmo(_ammoAddingPerHit);
        }*/


    }

    private void InitiatePunchEffect()
    {
        //get player's position
        Vector3 playerPosition = orientation.parent.position;
        //get effect position
        Vector3 punchEffectPosition = Vector3.Lerp(playerPosition,enemyTransform.position,0.80f);
        //instace on effect
        Instantiate(punchEffect, punchEffectPosition, Quaternion.identity);
    }

}
