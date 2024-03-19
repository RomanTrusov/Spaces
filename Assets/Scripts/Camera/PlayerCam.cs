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
    private float swayForce; // force of the camera sway

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
        swayForce = 0; // default camera sway 0
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
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        //adding camera shaking while walking
        if (player.GetComponent<PlayerMovement>().grounded) //if grounded - shake camera while moving
        {

            //trying to make sway start slowly
            if (swayForce < 1) swayForce += 0.02f;

            playerVelocity = player.GetComponent<Rigidbody>().velocity.magnitude / 10f; //get speed of walking to set the camera shaking by it's scale
            if (playerVelocity > 1) playerVelocity = 1; // limit player velocity to avoid camera shift while dashing
            Quaternion x_Shake = Quaternion.AngleAxis(swayForce * playerVelocity * Mathf.Sin(Time.time * 5), Vector3.right); //calculate shake force
            Quaternion y_Shake = Quaternion.AngleAxis(swayForce * playerVelocity / 2f * Mathf.Sin(Time.time * 3), Vector3.up);
            Quaternion addShake = Quaternion.AngleAxis(0, Vector3.zero) * x_Shake * y_Shake;
            
            //apply velocity scale to camera shake
            transform.rotation = Quaternion.Euler(xRotation, yRotation, 0) * addShake;
            orientation.rotation = Quaternion.Euler(0, yRotation, 0);

        }
        else // ig !grounded - do not shake camera
        {
            swayForce = 0;
            transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
            orientation.rotation = Quaternion.Euler(0, yRotation, 0);
        }

    }


}
