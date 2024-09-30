using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CursorOpacity : MonoBehaviour
{
    public Transform playerCamera;
    public Grapling playerGrapple;

    public Color fullOpacity;
    public Color halfOpacity;

    public LayerMask whatIsGrappable;
    public LayerMask enemy;

    private void LateUpdate()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, playerGrapple.maxGrapplingDistance, whatIsGrappable))
            GetComponent<RawImage>().color = fullOpacity;
        else if (Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, playerGrapple.maxGrapplingDistance, enemy))
            GetComponent<RawImage>().color = fullOpacity;
        else GetComponent<RawImage>().color = halfOpacity;
    }

}
