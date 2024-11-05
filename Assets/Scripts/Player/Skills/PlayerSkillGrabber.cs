using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillGrabber : MonoBehaviour
{
    //check if skill is available and activated
    public bool isSkillAvailable;
    private bool isSkillActivated;

    //skill cooldown
    public float skillCD;
    private float skillCDTimer;

    //player animator
    public Animator playerAnimator;

    //key for skill
    [Header("Input")]
    public KeyCode skillKey = KeyCode.E;

    [Header("Visual Effects")]
    public ParticleSystem grabberEffect;
    private ParticleSystem.EmissionModule grabberEffectEmmission;

    [Header("Vars for raycasting")]
    public Camera playerCamera;
    public float maxDistance = 100f;
    public LayerMask raycastLayerMask; //enemy mask
    private Transform targetPoint; // following point
    public GameObject pointObject; //object with target position
    private float fixedDistanceToCamera; // distance to the camera
    private GameObject enemy; //GO for enemy

    private void Start()
    {
        //reset timer
        skillCDTimer = skillCD;

        //check for active pointObject
        if (pointObject.activeSelf) pointObject.SetActive(false);

        //check if effect is on
        if (grabberEffect.gameObject.activeSelf) grabberEffect.gameObject.SetActive(false);
        grabberEffectEmmission = grabberEffect.emission;

    }



    private void Update()
    {
        //STATE SETTER
        //activate skill if possible by pressing button
        if (isSkillAvailable && !isSkillActivated && Input.GetKeyDown(skillKey))
        {
            //anim params to tart the animation
            playerAnimator.SetBool("SkillGrabber", true);
            playerAnimator.SetBool("SkillGrabberEnd", false);
            //skill activated
            isSkillActivated = true;

            //throw raycast to find an enemy
            CastRayAndCreateTargetPoint();

            //activate vfx after delay
            Invoke(nameof(ActivateVFX),0.3f);
        }


        if (isSkillActivated && pointObject.activeSelf)
        {
            //point follows the camera if grab the enemy and skill is activated
            UpdateTargetPointPosition();
        }

        //if skill activated and button relesaed
        if (isSkillAvailable && isSkillActivated && Input.GetKeyUp(skillKey))
        {
            //anim params to finish the animation
            playerAnimator.SetBool("SkillGrabber", false);
            playerAnimator.SetBool("SkillGrabberEnd", true);
            isSkillActivated = false;

            //if pointObject active - off it
            if (pointObject.activeSelf) pointObject.SetActive(false);

            //deactivate VFX
            DeactivateVFXEmission();

            //return enemy to alert and clear the var
            if (enemy != null)
            {
                enemy.GetComponent<EnemyBehaviourDrone>().state = EnemyBehaviourDrone.EnemyStates.alert;
                enemy = null;
            }

        }

    }

    private void ActivateVFX()
    {
        //activate object and set Emission
        grabberEffect.gameObject.SetActive(true);
        grabberEffectEmmission.rateOverTime = 30;
    }

    private void DeactivateVFXEmission()
    {
        //reduce emission and deactivate object after 1 sec
        grabberEffectEmmission.rateOverTime = 0;
        Invoke(nameof(DeactivateVFX),1f);
    }

    private void DeactivateVFX()
    {
        //deactivate object
        grabberEffect.gameObject.SetActive(false);
    }

    void CastRayAndCreateTargetPoint()
    {
        // ray from the middle of the screen
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        //check ray collisions with enemy
        if (Physics.Raycast(ray, out RaycastHit hitInfo, maxDistance, raycastLayerMask))
        {
            //if pointObject is not acive
            if (!pointObject.activeSelf)
            {
                //activate it
                pointObject.SetActive(true);
                //target point equals new object positions
                targetPoint = pointObject.transform;
            }

            //get gameobject and make it Grabbed state
            enemy = hitInfo.transform.gameObject;
            

            //get fixed distance from target point to camera
            fixedDistanceToCamera = Vector3.Distance(playerCamera.transform.position, hitInfo.point);

            //set target point position
            targetPoint.position = hitInfo.point;
        } 
    }

    void UpdateTargetPointPosition()
    {
        //update if exists
        if (targetPoint != null)
        {

            enemy.GetComponent<EnemyBehaviourDrone>().state = EnemyBehaviourDrone.EnemyStates.grabbed;

            //get forward direction
            Vector3 direction = playerCamera.transform.forward;

            //set new position for target position
            targetPoint.position = playerCamera.transform.position + direction * fixedDistanceToCamera;
        }
    }


}
