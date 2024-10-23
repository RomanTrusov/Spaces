using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillGrabber : MonoBehaviour
{
    //check if skill is available
    public bool isSkillAvailable;
    private bool isSkillActivated;

    //skill cooldown
    public float skillCD;
    private float skillCDTimer;

    //player animator
    public Animator playerAnimator;

    //key for skill
    [Header("Input")]
    public KeyCode skillKey = KeyCode.Q;

    private void Start()
    {
        //reset timer
        skillCDTimer = skillCD;
    }

    private void Update()
    {
        //activate skill if possible by pressing button
        if (isSkillAvailable && !isSkillActivated && Input.GetKeyDown(skillKey))
        {
            //play anim
            playerAnimator.SetBool("SkillGrabber", true);
            //skill activated
            isSkillActivated = true;
            //set cooldown
            skillCDTimer = 0;
        }

        //TODO: skill cooldown - rework it with timer or IEnumerator
        if (skillCDTimer < skillCD)
        {
            skillCDTimer += Time.deltaTime;
        }
        else if (isSkillActivated)
        {
            skillCDTimer = skillCD;
            playerAnimator.SetBool("SkillGrabber", false);
            isSkillActivated = false;
        }

    }

}
