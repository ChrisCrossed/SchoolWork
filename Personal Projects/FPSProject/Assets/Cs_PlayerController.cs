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

        // Raycast information
        go_RaycastPoint = new GameObject[5];
        go_RaycastPoint[0] = transform.FindChild("RaycastPoints").FindChild("RaycastPoint_Center").gameObject;
        go_RaycastPoint[1] = transform.FindChild("RaycastPoints").FindChild("RaycastPoint_0").gameObject;
        go_RaycastPoint[2] = transform.FindChild("RaycastPoints").FindChild("RaycastPoint_1").gameObject;
        go_RaycastPoint[3] = transform.FindChild("RaycastPoints").FindChild("RaycastPoint_2").gameObject;
        go_RaycastPoint[4] = transform.FindChild("RaycastPoints").FindChild("RaycastPoint_3").gameObject;
    }

    Vector3 v3_PushDirection_Old;
    float f_FallVelocity;
    void Movement()
    {
        // Store previous downward velocity
        float f_yVel = this_Rigidbody.velocity.y;

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
        v3_PushDirection = Vector3.Lerp( v3_PushDirection_Old, v3_PushDirection, 0.3f );

        // Apply new push direction based on ground normal
        Vector3 v3_GroundNormal = FindRaycastHit().normal;
        if( v3_GroundNormal != new Vector3() )
        {
            // Convert normals
            v3_PushDirection = Vector3.ProjectOnPlane( v3_PushDirection, v3_GroundNormal );
        }

        // Set player velocity based on (SPEED) & (PUSH DIRECTION)
        this_Rigidbody.velocity = 10.0f * v3_PushDirection;

        #region Gravity
        // Check if the ground is close below us.
        float f_TerminalVelocity = 15f;
        RaycastHit hitToGround = FindRaycastHit();
        
        // Run a Raycast to see if we're touching any ground or not
        if (RaycastHit.Equals(hitToGround, new RaycastHit()))
        {
            // Not touching the ground. Add gravity/velocity
            if (f_FallVelocity < f_TerminalVelocity)
            {
                f_FallVelocity += Time.fixedDeltaTime + f_FallVelocity / 2f;
                if (f_FallVelocity > f_TerminalVelocity) f_FallVelocity = f_TerminalVelocity;
            }

            // We are not touching the ground. Apply gravity.
            Vector3 v3_Gravity = this_Rigidbody.velocity;
            v3_Gravity.y -= f_FallVelocity;
            this_Rigidbody.velocity = v3_Gravity;
        }
        else
        {
            f_FallVelocity = 0;

            /*
            if(Input.GetKeyDown(KeyCode.Space))
            {
                Vector3 v3_JumpVelocity = this_Rigidbody.velocity;
                v3_JumpVelocity.y = 10f;
                this_Rigidbody.velocity = v3_JumpVelocity;
            }
            */
        }
        #endregion

        // Store the old push direction
        v3_PushDirection_Old = v3_PushDirection;
    }

    GameObject[] go_RaycastPoint;
    RaycastHit FindRaycastHit(float f_Distance = 0.35f)
    {
        // Raycast downward to find ground plane
        RaycastHit hit;
        RaycastHit tempHit;
        int i_LayerMask = LayerMask.GetMask("Ground");

        // Store first normal of ground plane
        Physics.Raycast(go_RaycastPoint[0].transform.position, -Vector3.up, out hit, f_Distance, i_LayerMask);

        for (int i_ = 0; i_ < go_RaycastPoint.Length; ++i_)
        if (Physics.Raycast(go_RaycastPoint[i_].transform.position, -Vector3.up, out tempHit, f_Distance, i_LayerMask))
        {
            if(tempHit.distance < hit.distance) hit = tempHit;
        }

        // Return normal
        return hit;
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
