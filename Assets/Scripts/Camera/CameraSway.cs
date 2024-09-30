using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSway : MonoBehaviour
{

    public float intensity; // speed of rotating
    public float smooth; // speed of resetting rotation

    public PlayerMovement player; // to het Grounded for player

    private Quaternion origin_rotation;

    // Start is called before the first frame update
    void Start()
    {
        origin_rotation = Quaternion.AngleAxis(0, Vector3.zero); // hard coding the origin rotation
    }

    // Update is called once per frame
    void Update()
    {
        /*if (player.grounded)*/ UpdateSway();
    }

    private void UpdateSway ()
    {
        //get inputs
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        //calculate target rotation
        Quaternion t_x_adj = Quaternion.AngleAxis(intensity * horizontalInput * Mathf.Sin(Time.time * 3) * 1f, Vector3.up);
        Quaternion t_y_adj = Quaternion.AngleAxis(intensity * verticalInput * Mathf.Sin(Time.time * 3) * 1f, Vector3.right);
        Quaternion target_rotation = origin_rotation * t_x_adj * t_y_adj;

        //rotate towards target rotation - in local space
        transform.localRotation = Quaternion.Lerp(transform.localRotation, target_rotation, Time.deltaTime * smooth);
    }

}
