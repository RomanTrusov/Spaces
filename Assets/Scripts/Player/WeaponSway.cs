using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WeaponSway : MonoBehaviour
{

    public float intensity; // speed of rotating
    public float smooth; // speed of resetting rotation
    public float jumpSwayForce;
    public PlayerMovement player;

    private Quaternion origin_rotation;

    //=======================
    void Start()
    {
        origin_rotation = Quaternion.AngleAxis(0,Vector3.zero); // hard coding the origin rotation

        intensity = 4;
        smooth = 3;
        jumpSwayForce = 20;

    }

    //=======================
    void Update()
    {
        // sway from camera movement
        UpdateSway();

        // walking sway
        if (player.grounded) WalkingSway();

        //jumping sway
        // if (player.grounded && Input.GetKeyDown(KeyCode.Space)) JumpSway();

    }

    //=======================
    private void UpdateSway()
    {
        //get mouse axis
        float t_x_mouse = Input.GetAxis("Mouse X");
        float t_y_mouse = Input.GetAxis("Mouse Y");

        //calculate target rotation
        Quaternion t_x_adj = Quaternion.AngleAxis(-intensity * t_x_mouse, Vector3.up);
        Quaternion t_y_adj = Quaternion.AngleAxis(intensity * 3f * t_y_mouse, Vector3.right); //*3f to increase the poser of sway on Y axis
        Quaternion t_z_adj = Quaternion.AngleAxis(intensity * t_x_mouse, Vector3.forward); // adding Z rotation from X mouse rotaion
        Quaternion target_rotation = origin_rotation * t_x_adj * t_y_adj * t_z_adj;

        //rotate towards target rotation - in local space
        transform.localRotation = Quaternion.Lerp(transform.localRotation, target_rotation, Time.deltaTime * smooth);
    }

    //=======================
    private void WalkingSway()
    {
        
        //get inputs
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        //calculate target rotation
        Quaternion t_x_adj = Quaternion.AngleAxis(intensity * horizontalInput * Mathf.Sin(Time.time * 2) * 1f, Vector3.up);
        Quaternion t_y_adj = Quaternion.AngleAxis(intensity * verticalInput * Mathf.Sin(Time.time * 10) * 0.5f, Vector3.right);
        Quaternion target_rotation = origin_rotation * t_x_adj * t_y_adj;

        //rotate towards target rotation - in local space
        transform.localRotation = Quaternion.Lerp(transform.localRotation, target_rotation, Time.deltaTime * smooth);
    }


    //=======================
    public void JumpSway()
    {
        //calculate target rotation
        Quaternion t_y_adj = Quaternion.AngleAxis(intensity * jumpSwayForce, Vector3.right);

        //rotate towards target rotation - in local space
        transform.localRotation = Quaternion.Lerp(transform.localRotation, t_y_adj, Time.deltaTime * smooth);
    }


    //=======================
    public void flySway()
    {
        //calculate target rotation
        Quaternion t_y_adj = Quaternion.AngleAxis(-intensity* 0.4f * jumpSwayForce, Vector3.right);

        //rotate towards target rotation - in local space
        transform.localRotation = Quaternion.Lerp(transform.localRotation, t_y_adj, Time.deltaTime * smooth);
    }



}