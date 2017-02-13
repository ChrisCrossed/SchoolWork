using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cs_PlayerController : MonoBehaviour
{
    // Object Connections
    GameObject this_Player;
    Rigidbody this_Rigidbody;
    Collider this_Collider;
    GameObject this_Camera;

	// Use this for initialization
	void Start ()
    {
        // Object Connections
        this_Player = gameObject;
        this_Rigidbody = this_Player.GetComponent<Rigidbody>();
        this_Collider = this_Player.GetComponent<Collider>();
        this_Camera = transform.FindChild("Main Camera").gameObject;
	}

    Vector3 v3_PushDirection_Old;
    void Movement()
    {
        // Create new directional vector
        Vector3 v3_InputVector = new Vector3();

        #region Create Directional vector based on input
        if(Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
        {
            v3_InputVector.z = 1.0f;
        }
        else if(Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W))
        {
            v3_InputVector.z = -1.0f;
        }

        if(Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        {
            v3_InputVector.x = -1.0f;
        }
        else if(Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A))
        {
            v3_InputVector.x = 1.0f;
        }
        #endregion

        // Normalize vector
        v3_InputVector.Normalize();

        // Find player orientation and push in that direction in accordance to the input vector
        Vector3 v3_PushDirection = this_Player.transform.rotation * v3_InputVector;

        // Lerp between the previous direction and the new direction (Makes direction switching a bit smoother)
        v3_PushDirection = Vector3.Lerp(v3_PushDirection_Old, v3_PushDirection, 0.3f);

        // Set player velocity based on (SPEED) & (PUSH DIRECTION)
        this_Rigidbody.velocity = 10.0f * v3_PushDirection;

        // Store the old push direction
        v3_PushDirection_Old = v3_PushDirection;
    }

    float f_VertAngle;
    void MouseLook()
    {
        #region Mouse Horizontal
        // Store the player's current Euler angle rotation
        Vector3 v3_PlayerEuler = this_Player.transform.eulerAngles;

        // Store Mouse horizontal input
        float f_MouseX = Input.GetAxis("Mouse X");

        // Change player rotation based on mouse horiz
        v3_PlayerEuler.y += f_MouseX;

        // Update player's new rotation
        this_Player.transform.eulerAngles = v3_PlayerEuler;
        #endregion

        #region Mouse Vertical
        // Store mouse Y input
        f_VertAngle -= Input.GetAxis("Mouse Y");
        f_VertAngle = Mathf.Clamp(f_VertAngle, -85f, 85f);

        Vector3 v3_CamEuler = this_Camera.transform.eulerAngles;
        v3_CamEuler.x = f_VertAngle;
        this_Camera.transform.eulerAngles = v3_CamEuler;
        #endregion
    }

    // FixedUpdate is called at the same points in time.
    void FixedUpdate ()
    {
        Movement();
        MouseLook();
	}
}
