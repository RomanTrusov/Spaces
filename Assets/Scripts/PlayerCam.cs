using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    // setting mouse sens
    public float sensX;
    public float sensY;

    // for player's orientation
    public Transform orientation;

    // for camera rotation
    float xRotation;
    float yRotation;


    private void Start()
    {
        // locked and invisible cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        // get mouse input
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        //calculate rotations
        yRotation += mouseX;
        xRotation -= mouseY;

        // clamp vertical rotations
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        //rotate camera and player
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);

    }

}
