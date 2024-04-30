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
    //private float swayForce; // force of the camera sway - not used - sway off
    [SerializeField]
    private bool swayInWalk; //to turn off sway in walking

    //FOV variables
    private float fovDefault;
    private float fovGoal;

    // for player's orientation
    public Transform orientation;

    // for camera rotation
    float xRotation;
    float yRotation;

    //to limit fps
    public int target = 30;

    void Awake()
    {
        //set vsync off and set the limit fps
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = target;
    }



    private void Start()
    {
        //get default FOV
        fovDefault = GetComponent<Camera>().fieldOfView;


        // locked and invisible cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        //swayForce = 0; // default camera sway 0  - not used - sway off
    }

    private void Update()
    {

        //limit fps every frame
        if (Application.targetFrameRate != target)
            Application.targetFrameRate = target;


        //============= get mouse input
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        //calculate rotations
        yRotation += mouseX;
        xRotation -= mouseY;

        // clamp vertical rotations
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        //============= Adding camera shaking while walking
        /*if (player.GetComponent<PlayerMovement>().grounded && swayInWalk) //if grounded - shake camera while moving
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
        else // if !grounded - do not shake camera
        {
            swayForce = 0;
            transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
            orientation.rotation = Quaternion.Euler(0, yRotation, 0);
        }*/
        // moving camera with falt lerp
        transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.Euler(xRotation, yRotation, 0),Time.deltaTime*50);
        orientation.rotation = Quaternion.Lerp(orientation.rotation,Quaternion.Euler(0, yRotation, 0),Time.deltaTime*50);

        //============= Changing FOV
        // while falling
        if (!player.GetComponent<PlayerMovement>().grounded && player.GetComponent<Rigidbody>().velocity.y < -0.2f)
        {
            // getting FOV goal
            fovGoal = GetFOVGoal(fovDefault, player.GetComponent<Rigidbody>().velocity.y, out fovGoal);
            //lerping the FOV
            GetComponent<Camera>().fieldOfView = Mathf.Lerp(GetComponent<Camera>().fieldOfView, fovGoal, Time.deltaTime * 5f);
        }// if not falling - and FOV is not default - return it back
        else if (GetComponent<Camera>().fieldOfView != fovDefault) 
            GetComponent<Camera>().fieldOfView = Mathf.Lerp(GetComponent<Camera>().fieldOfView, fovDefault, Time.deltaTime*5f);
    }

    //===============
    private float GetFOVGoal (float fovDefalut,float yPlayerVelocity ,out float fovGoal)
    {
        if (yPlayerVelocity < -30) yPlayerVelocity = -30; // limit the Y velocity

        fovGoal = fovDefault - yPlayerVelocity * 0.75f;

        return fovGoal;
    }
    
}
