using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleAttack : MonoBehaviour
{
    [Header("References")]
    public Animator playerAnimator;
        // is mele attack acting?
    public bool isMeleOn = false;

    [Header("Setting")]
    public float meleDelay;

    [Header("Input")]
    public KeyCode meleKey = KeyCode.Mouse3;


    private void Update()
    {
        if (Input.GetKeyDown(meleKey) && !isMeleOn)
        {
            MeleAttacking();
        }
    }

    private void MeleAttacking()
    {
        playerAnimator.SetBool("MelePunch", true);
        isMeleOn = true;
        Invoke(nameof(MeleAttackingOff), meleDelay);
    }

    private void MeleAttackingOff()
    {
        playerAnimator.SetBool("MelePunch", false);
        isMeleOn = false;
    }

}
