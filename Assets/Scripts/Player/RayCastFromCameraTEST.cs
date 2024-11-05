using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCastFromCameraTEST : MonoBehaviour
{
    public Camera playerCamera;
    public float maxDistance = 100f;
    public LayerMask raycastLayerMask;
    private Transform targetPoint; // following point
    private float fixedDistanceToCamera; // distance to the camera

    void Update()
    {
        //raycast method
        CastRayAndCreateTargetPoint();
        //point follows the camera
        UpdateTargetPointPosition();
    }

    void CastRayAndCreateTargetPoint()
    {
        // ray from the middle of the screen
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        //check ray collisions
        if (Physics.Raycast(ray, out RaycastHit hitInfo, maxDistance, raycastLayerMask))
        {
            //if point was not created
            if (targetPoint == null)
            {
                //create gameobject
                GameObject pointObject = new GameObject("TargetPoint");
                //target point equals new object positions
                targetPoint = pointObject.transform;
            }

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
            //get forward direction
            Vector3 direction = playerCamera.transform.forward;

            //set new position for target position
            targetPoint.position = playerCamera.transform.position + direction * fixedDistanceToCamera;
        }
    }
}
