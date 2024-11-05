using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CameraShake;

public class PlayerSkillRaiseUp : MonoBehaviour
{
    //check if skill is available
    public bool isSkillAvailable;
    private bool isSkillActivated;

    //skill cooldown
    public float skillCD;
    private float skillCDTimer;

    //particle effect
    public ParticleSystem dustEffect;

    //sphere object
    public GameObject sphere;
    //triggerzone
    public GameObject triggerZone;
    private float triggerDelay = 1.6f;

    //player animator
    public Animator playerAnimator;

    //key for skill
    [Header("Input")]
    public KeyCode skillKey = KeyCode.Q;

    private void Start()
    {
        //reset timer
        skillCDTimer = skillCD;
        //check if trigger is off
        if (triggerZone.activeSelf) triggerZone.SetActive(false);
    }

    private void Update()
    {
        //activate skill if possible by pressing button
        if (isSkillAvailable && !isSkillActivated && Input.GetKeyDown(skillKey))
        {
            //activate sphere
            if (!sphere.activeSelf) sphere.SetActive(true);
            //play anim
            playerAnimator.SetBool("SkillRaiseUp", true);
            //skill activated
            isSkillActivated = true;
            //set cooldown
            skillCDTimer = 0;
            //start coroutine for trigger activation
            StartCoroutine(EnableTriggerAfterDelay());
        }

        if (skillCDTimer < skillCD)
        {
            skillCDTimer += Time.deltaTime;
        } else if (isSkillActivated)
        {
            //deactivate sphere
            if (sphere.activeSelf) sphere.SetActive(false);
            //reset timer
            skillCDTimer = skillCD;
            //hand to Idle
            playerAnimator.SetBool("SkillRaiseUp", false);
            //skill is not activated
            isSkillActivated = false;
        }

    }

    private IEnumerator EnableTriggerAfterDelay()
    {
        //wait for delay
        yield return new WaitForSeconds(triggerDelay);
        //activate trigger after delay
        triggerZone.SetActive(true);
        //play vfx
        ParticleSystem clone = Instantiate(dustEffect, dustEffect.gameObject.transform.position, dustEffect.gameObject.transform.rotation);
        if (!clone.gameObject.activeSelf) clone.gameObject.SetActive(true);
        //shake camera
        CameraShaker.Instance.ShakePresets.ShortShake3D(2f, 25, 5);

        //wait to deactivate trigger
        yield return new WaitForSeconds(0.2f);
        triggerZone.SetActive(false);

        

    }
}
