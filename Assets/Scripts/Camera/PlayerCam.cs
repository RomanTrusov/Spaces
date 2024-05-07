using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    // setting mouse sens
    public float sensX;
    public float sensY;
    public GameObject player; // to get Grounded bool

    //FOV variables
    private float fovDefault;
    private float fovGoal;

    // for player's orientation
    public Transform orientation;

    // for camera rotation
    float xRotation;
    float yRotation;

    //to limit fps
    public int target = 60;

    //tilt camera Y
    float yTilt = 0;
    [SerializeField]
    private float yTiltGoal;
    [SerializeField]
    private float yTiltSpeed;


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

        // tilt camera sideways
        if (Input.GetAxisRaw("Horizontal") > 0)
        {
            yTilt = Mathf.Lerp(yTilt, yTiltGoal * -Input.GetAxisRaw("Horizontal"), Time.deltaTime * yTiltSpeed);
        } else if (Input.GetAxisRaw("Horizontal") < 0)
        {
            yTilt = Mathf.Lerp(yTilt, yTiltGoal * -Input.GetAxisRaw("Horizontal"), Time.deltaTime * yTiltSpeed);
        } else if (Input.GetAxisRaw("Horizontal") == 0)
            yTilt = Mathf.Lerp(yTilt, 0, Time.deltaTime * yTiltSpeed);

        // moving camera with falt lerp
        transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.Euler(xRotation, yRotation, yTilt),Time.deltaTime*50);
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
