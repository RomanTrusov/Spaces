using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleAttackTrigger : MonoBehaviour
{
    public MeleAttack playerMeleAttack;

    public Transform orientation;
    public float forwardPush;
    public float upwardPush;
    public float meleAttackModifier;

    [SerializeField]
    private GameObject punchEffect;

    private Transform enemyTransform;

    public int meleState = 0;
    public int meleDirection = 0;

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
        other.GetComponent<EnemyBehaviourDrone>().GetDamage(playerMeleAttack.playerDamage);
        /*other.GetComponent<EnemyBehaviourDrone>().enemyHealth -= 1;
        other.GetComponent<EnemyBehaviourDrone>().state = EnemyBehaviourDrone.EnemyStates.damaged;
        other.GetComponent<EnemyBehaviourDrone>().ResetAttackedTimer();*/
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
