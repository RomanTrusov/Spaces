using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleAttack : MonoBehaviour
{
    [Header("References")]
    public Animator playerAnimator;
    public GameObject meleTrigger;

    [Header("Debug")]
    //mele states: 0 - inactive, 1 - simple punch, 2 - combo kick
    public int meleState = 0;
    private int currentMeleState = 0;
    public int meleDirection = 0;


    [Header("Settings")]
    //delay to track the combo
    public float timeForCombo;
    // cooldown for attacks to avoid button mashing
    private float attackCD = 0.5f;
    private float attackCDtimer = 0;

    [Header("Input")]
    public KeyCode meleKey = KeyCode.Mouse0;


    private void Update()
    {

        //reduce meleCD timer to 0
        if (attackCDtimer >= 0) attackCDtimer -= Time.deltaTime;

        //if meleCD 0 allow attack and reset meleCD
        if (attackCDtimer <= 0 && Input.GetKeyDown(meleKey) && meleState != 1) // if press mele key during nothing - do simple punch
        {
            MeleAttacking();
            ActivateMeleTrigger();

            attackCDtimer = attackCD;

        } else if (attackCDtimer <= 0 && Input.GetKeyDown(meleKey) && meleState == 1) // if press mele key during combo delay - do combo
        {
            MeleCombo();
            ActivateMeleTrigger();

            attackCDtimer = attackCD;
        }
    }


    private void ActivateMeleTrigger ()
    {
        // check is it simple punch or a combo attack
        if (meleState == 1)
        {
            meleTrigger.GetComponent<MeleAttackTrigger>().meleState = 1;
            meleTrigger.GetComponent<MeleAttackTrigger>().meleDirection = meleDirection;
        } else if (meleState == 2)
        {
            if (meleTrigger.activeSelf) meleTrigger.SetActive(false);
            meleTrigger.GetComponent<MeleAttackTrigger>().meleState = 2;
            meleTrigger.GetComponent<MeleAttackTrigger>().meleDirection = meleDirection;
        }
        meleTrigger.SetActive(true);
        Invoke(nameof(DeactivateMeleTrigger), 0.35f);
    }

    private void DeactivateMeleTrigger()
    {
        meleTrigger.SetActive(false);
    }

    private void MeleAttacking()
    {
        meleDirection = CheckMeleDirection(); //get mele direction
        playerAnimator.SetBool("MelePunch", true);
        meleState = 1; // state - simple punch
        currentMeleState = meleState;
        // if mele button was not pressed during combo delay - state 0
        Invoke(nameof(MeleAttackingOff), timeForCombo); // check if we need to make state to 0 or not
    }

    private void MeleCombo()
    {
        meleDirection = CheckMeleDirection(); //get mele direction
        playerAnimator.SetBool("MelePunch", false); //deactivate simple mele attack
        meleState = 2;
        currentMeleState = meleState;
        switch (meleDirection)
        {
            case 0:
                playerAnimator.SetBool("KickForward", true);
                break;
            case 1:
                playerAnimator.SetBool("KickForward", true);
                break;
            case 2:
                playerAnimator.SetBool("KickRight", true);
                break;
            case 3:
                playerAnimator.SetBool("KickBack", true);
                break;
            case 4:
                playerAnimator.SetBool("KickLeft", true);
                break;
        }
        Invoke(nameof(MeleAttackingOff), timeForCombo/2); // check if we need to make state to 0 or not
    }

    private int CheckMeleDirection() //for directional combo
    {
        // forward - if only go forward (no horizontal)
        if (Input.GetAxisRaw("Vertical") == 1 && Input.GetAxisRaw("Horizontal") == 0)
        {
            return 1; // forward
        }
        else //right - if right or forward+right
        if ((Input.GetAxisRaw("Horizontal") == 1 && Input.GetAxisRaw("Vertical") != -1) || (Input.GetAxisRaw("Horizontal") == 1 && Input.GetAxisRaw("Vertical") == 1))
        {
            return 2; // right
        } else 
        if ((Input.GetAxisRaw("Horizontal") == -1 && Input.GetAxisRaw("Vertical") != -1) || (Input.GetAxisRaw("Horizontal") == 1 && Input.GetAxisRaw("Vertical") == 1))
        {
            return 4; //left
        }
        else //back - if back and horizontal
        if (Input.GetAxisRaw("Vertical") == -1)
        {
            return 3; //back
        }
        else return 0;
    }

    //if mele state didn't changed - turn state to 0. It happens when you stopped first attack or finished the combo
    private void MeleAttackingOff()
    {
        if (currentMeleState == meleState)
        {
            playerAnimator.SetBool("MelePunch", false);
            playerAnimator.SetBool("KickForward", false);
            playerAnimator.SetBool("KickBack", false);
            playerAnimator.SetBool("KickLeft", false);
            playerAnimator.SetBool("KickRight", false);
            meleTrigger.gameObject.SetActive(false); // deactivate mele trigger
            meleState = 0; // state - nothing
            meleDirection = 0; // reset mele direction
        }
    }

    

}
