using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    // setting mouse sens
    public float sensX;
    public float sensY;
    public GameObject player; // to get Grounded bool

    [SerializeField]
    private float playerVelocity;

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

        //adding camera shaking while walking
        if (player.GetComponent<PlayerMovement>().grounded) //if grounded - shake camera while moving
        {
            playerVelocity = player.GetComponent<Rigidbody>().velocity.magnitude / 10f; //get speed of walking to set the camera shaking by it's scale
            Quaternion x_Shake = Quaternion.AngleAxis(playerVelocity * Mathf.Sin(Time.time * 5), Vector3.right); //calculate shake force
            Quaternion y_Shake = Quaternion.AngleAxis(playerVelocity/2f * Mathf.Sin(Time.time * 3), Vector3.up);
            Quaternion addShake = x_Shake * y_Shake;

            //apply velocity scale to camera shake
            transform.rotation = Quaternion.Euler(xRotation, yRotation, 0) * addShake;
            orientation.rotation = Quaternion.Euler(0, yRotation, 0);

        } else // ig !grounded - do not shake camera
        {
            transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
            orientation.rotation = Quaternion.Euler(0, yRotation, 0);
        }
        

    }

}
