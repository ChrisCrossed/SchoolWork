﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum Enum_PlayerState
{
    Movement,
    DashToObject,
    Grenade
}
public class Cs_PlayerController : MonoBehaviour
{
    Enum_PlayerState e_PlayerState;

    // Object Connections
    GameObject this_Player;
    Rigidbody this_Rigidbody;
    Collider this_Collider;
    GameObject this_Camera;
    Cs_Crosshair this_Crosshair;

    // Floats
    float MAX_MOVESPEED_FORWARD = 10f;
    float ACCELERATION = 5f;
    float f_RayCast_DownwardDistance = 0.32f;
    float JUMP_HEIGHT = 12.5f;

    // Use this for initialization
    void Start ()
    {
        // Lock mouse cursor
        Cursor.lockState = CursorLockMode.Locked;

        // Object Connections
        this_Player = gameObject;
        this_Rigidbody = this_Player.GetComponent<Rigidbody>();
        this_Collider = this_Player.GetComponent<Collider>();
        this_Camera = transform.FindChild("Main Camera").gameObject;
        this_Crosshair = this_Player.GetComponent<Cs_Crosshair>();

        // Raycast information
        go_RaycastPoint = new GameObject[5];
        go_RaycastPoint[0] = transform.FindChild("RaycastPoints").FindChild("RaycastPoint_Center").gameObject;
        go_RaycastPoint[1] = transform.FindChild("RaycastPoints").FindChild("RaycastPoint_0").gameObject;
        go_RaycastPoint[2] = transform.FindChild("RaycastPoints").FindChild("RaycastPoint_1").gameObject;
        go_RaycastPoint[3] = transform.FindChild("RaycastPoints").FindChild("RaycastPoint_2").gameObject;
        go_RaycastPoint[4] = transform.FindChild("RaycastPoints").FindChild("RaycastPoint_3").gameObject;

        // Set Player State
        e_PlayerState = Enum_PlayerState.Movement;
    }

    Vector3 InputVector()
    {
        Vector3 v3_InputVector = new Vector3();

        if (Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
        {
            v3_InputVector.z = 1.0f;
        }
        else if (Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W))
        {
            v3_InputVector.z = -1.0f;
        }

        if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        {
            v3_InputVector.x = -1.0f;
        }
        else if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A))
        {
            v3_InputVector.x = 1.0f;
        }

        v3_InputVector.Normalize();

        return v3_InputVector;
    }

    Vector3 v3_PushDirection_Old;
    float f_FallVelocity;
    void Movement()
    {
        // Create new directional vector
        Vector3 v3_InputVector = new Vector3();

        v3_InputVector = InputVector();

        #region Jump
        bool b_Jump = false;
        if (b_CanJump)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                b_Jump = true;
                b_CanJump = false;
            }
        }
        #endregion

        PlayerMovement( v3_InputVector, b_Jump );
    }

    GameObject[] go_RaycastPoint;
    RaycastHit FindRaycastHit(float f_Distance = 0.35f)
    {
        // Raycast downward to find ground plane
        RaycastHit hit;
        // RaycastHit tempHit;
        int i_LayerMask = LayerMask.GetMask("Ground");

        // Store first normal of ground plane
        Physics.Raycast(go_RaycastPoint[0].transform.position, -Vector3.up, out hit, f_Distance, i_LayerMask);

        // Return normal
        return hit;
    }

    void PlayerMovement(Vector3 v3_Direction_, bool b_Jump_, float f_Magnitude_ = 1)
    {
        // Old velocity
        Vector3 v3_oldVelocity = gameObject.GetComponent<Rigidbody>().velocity;

        // Reset the new Push Direction for the player
        Vector3 v3_PushDirection = new Vector3();

        // Combine (not multiply) the player's current rotation (Quat) into the input vector (Vec3)
        Vector3 v3_FinalRotation = gameObject.transform.rotation * v3_Direction_;

        // Lerp prior velocity into new velocity
        Vector3 v3_newVelocity = Vector3.Lerp(v3_oldVelocity, v3_FinalRotation * MAX_MOVESPEED_FORWARD * f_Magnitude_, 1 / ACCELERATION);

        // Return gravity
        if (!b_Jump_)
        {
            // Synthetic terminal velocity
            RaycastHit hit;

            // Raycast straight down 
            Physics.Raycast(go_RaycastPoint[0].transform.position, -transform.up, out hit);

            // Apply fake gravity (synthetic Terminal Velocity) - Note: RigidBody gravity is OFF
            if (hit.distance > f_RayCast_DownwardDistance) v3_newVelocity.y = v3_oldVelocity.y - (Time.deltaTime * 50);

            // Determine direction to push against ramp
            v3_PushDirection = -FindRaycastHit().normal;

            // Apply velocity to player
            v3_newVelocity = Vector3.ProjectOnPlane(v3_newVelocity, v3_PushDirection);
        }
        else
        {
            // Apply a jump
            v3_newVelocity.y = JUMP_HEIGHT;

            ResetJump();
        }

        /*
        if(Vector3.Magnitude(v3_newVelocity) <= Vector3.Magnitude(v3_oldVelocity))
        {
            gameObject.GetComponent<Rigidbody>().velocity = v3_newVelocity;
        }
        else
        {
            gameObject.GetComponent<Rigidbody>().velocity = v3_oldVelocity;
        }
        */

        gameObject.GetComponent<Rigidbody>().velocity = v3_newVelocity;
        gameObject.GetComponent<Rigidbody>().AddForce(v3_PushDirection);
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

    bool b_CanJump;
    float f_JumpTimer;
    void UpdateJump()
    {
        // Raycast Out object
        RaycastHit hit;

        b_CanJump = true;

        if (f_JumpTimer < 0.1f)
        {
            f_JumpTimer += Time.deltaTime;

            if (f_JumpTimer > 0.1f) f_JumpTimer = 0.1f;

            // Disable the ability to jump during this initial window
            b_CanJump = false;
        }
        else
        {
            // If the player isn't touching the ground, disable the ability to jump.
            if (!Physics.Raycast(go_RaycastPoint[0].transform.position, -Vector3.up, out hit, 0.35f, LayerMask.GetMask("Ground")))
            {
                b_CanJump = false;
            }
        }
    }
    void ResetJump()
    {
        f_JumpTimer = 0.0f;

        b_CanJump = false;
    }

    public void ApplyGrenadeBounce( Vector3 v3_Vector_, float f_GrenadeStrength_ )
    {
        // Store initial velocity
        Vector3 v3_Velocity = this_Rigidbody.velocity;

        // Determine impulse vector
        v3_Vector_.Normalize();

        // Apply grenade strength
        v3_Vector_ *= f_GrenadeStrength_;

        // Apply grenade velocity to player velocity
        v3_Velocity += v3_Vector_;

        // Apply new total velocity to player
        this_Rigidbody.velocity = v3_Velocity;

        // Set state
        e_PlayerState = Enum_PlayerState.Grenade;
    }

    GameObject go_LastKnownConnection;
    float f_LastKnownBulletHit_Timer;
    static float f_LastKnownBulletHit_Timer_Max = 0.25f;
    public GameObject Set_BulletHit
    {
        set
        {
            // If enough time has passed since the last bullet connection, store the connection
            if(f_LastKnownBulletHit_Timer == f_LastKnownBulletHit_Timer_Max)
            {
                // Reset the timer
                f_LastKnownBulletHit_Timer = 0f;

                print("Received: " + value.name);

                // Store the connection
                go_LastKnownConnection = value;
            }
        }
        get
        {
            if (go_LastKnownConnection != null) return go_LastKnownConnection;
            else return null;
        }
    }

    float f_TimeToPosition = 0f;
    float f_TimeToPosition_Max = 0.35f;
    Vector3 v3_StartPos;
    Vector3 v3_EndPos;
    [SerializeField] AnimationCurve ac_PullCurve;
    public void DashToEnemyPosition()
    {
        // If no enemy object is stored, cancel the action
        if (go_LastKnownConnection == null) return;

        // Set positions
        v3_StartPos = gameObject.transform.position;
        v3_EndPos = go_LastKnownConnection.transform.position;

        // Find distance between points. Subtract player width from distance.
        float f_Distance = Vector3.Distance(v3_StartPos, v3_EndPos);
        f_Distance -= gameObject.transform.lossyScale.z + go_LastKnownConnection.transform.lossyScale.z;

        // Find vector between points
        Vector3 v3_Vector = v3_EndPos - v3_StartPos;
        v3_Vector.Normalize();

        #region Determine if something other than the enemy hit is in the way
        int i_LayerMask = LayerMask.GetMask("Ground", "Enemy", "Wall");
        RaycastHit hit;
        Physics.Raycast(v3_StartPos, v3_Vector, out hit, float.PositiveInfinity, i_LayerMask);

        if (hit.collider.gameObject.layer != LayerMask.NameToLayer("Enemy")) { print("Something in the way"); return; }
        if (hit.collider.gameObject != go_LastKnownConnection) { print("Different Enemy in the way"); return; }
        #endregion

        // Set new final position
        v3_EndPos = v3_Vector * f_Distance;

        // Add player's position to vector * distance
        v3_EndPos += v3_StartPos;

        // Reset timer
        f_TimeToPosition = 0.0f;

        // Set state
        e_PlayerState = Enum_PlayerState.DashToObject;
    }

    // FixedUpdate is called at the same points in time.
    void FixedUpdate ()
    {
        if(e_PlayerState == Enum_PlayerState.Movement)
        {
            Movement();
            UpdateJump();
            MouseLook();

            if(Input.GetKeyDown(KeyCode.Q))
            {
                gameObject.GetComponent<Cs_ThrowGrenade>().GrenadeButtonPressed();
            }
        }
        else if(e_PlayerState == Enum_PlayerState.DashToObject)
        {
            // Increase timer
            f_TimeToPosition += Time.fixedDeltaTime;
            if (f_TimeToPosition > f_TimeToPosition_Max) f_TimeToPosition = f_TimeToPosition_Max;

            // Find position of player
            float f_Perc = ac_PullCurve.Evaluate(f_TimeToPosition / f_TimeToPosition_Max);
            Vector3 v3_NewPosition = Vector3.Lerp(v3_StartPos, v3_EndPos, f_Perc);

            // Set new position
            this_Rigidbody.MovePosition(v3_NewPosition);

            // Lerp player rotation to face the object being moved to

            if(f_TimeToPosition == f_TimeToPosition_Max)
            {
                // Allow movement
                e_PlayerState = Enum_PlayerState.Movement;
            }
        }
        else if(e_PlayerState == Enum_PlayerState.Grenade)
        {
            // Allow mouse look
            MouseLook();

            // Old velocity
            Vector3 v3_OldVelocity = this_Rigidbody.velocity;

            Vector3 v3_NewVelocity = v3_OldVelocity;
            v3_NewVelocity.y = v3_OldVelocity.y - (Time.deltaTime * 50);

            // Receive player input and apply velocity to it, correcting for player rotation
            Vector3 v3_InputVector = this_Player.transform.rotation * InputVector();

            // Add air-strafing velocity
            v3_InputVector *= 25.0f * Time.fixedDeltaTime;

            // Add player input velocity to current velocity
            v3_NewVelocity += v3_InputVector;

            // Set new velocity
            this_Rigidbody.velocity = v3_NewVelocity;

            // Determine if the player is touching the ground
            RaycastHit hit;
            int i_LayerMask = LayerMask.GetMask("Ground", "Enemy", "Wall");
            if(Physics.Raycast(go_RaycastPoint[0].transform.position, Vector3.down, out hit, f_RayCast_DownwardDistance, i_LayerMask))
            {
                e_PlayerState = Enum_PlayerState.Movement;
            }
        }

        #region Bullet Connection Timer
        if( f_LastKnownBulletHit_Timer < f_LastKnownBulletHit_Timer_Max )
        {
            f_LastKnownBulletHit_Timer += Time.fixedDeltaTime;

            if( f_LastKnownBulletHit_Timer > f_LastKnownBulletHit_Timer_Max ) f_LastKnownBulletHit_Timer = f_LastKnownBulletHit_Timer_Max;
        }
        #endregion

        if ( Input.GetKeyDown( KeyCode.E ) )
        {
            if( this_Crosshair.Get_CrosshairObject != null )
            {
                if(this_Crosshair.Get_CrosshairObject.layer == LayerMask.NameToLayer("Use"))
                {
                    // If button
                    if(this_Crosshair.Get_CrosshairObject.GetComponent<Cs_UseButton>())
                    {
                        this_Crosshair.Get_CrosshairObject.GetComponent<Cs_UseButton>().Use_Button();
                    }

                    if(this_Crosshair.Get_CrosshairObject.GetComponent<Cs_Door>())
                    {
                        this_Crosshair.Get_CrosshairObject.GetComponent<Cs_Door>().Use_OpenDoor();
                    }
                }
            }
        }
        else if( Input.GetMouseButtonDown(0) )
        {
            if( this_Crosshair.Get_CrosshairObject != null )
            {
                if( this_Crosshair.Get_CrosshairObject.layer == LayerMask.NameToLayer("Enemy") )
                {
                    print( this_Crosshair.Get_CrosshairObject );
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
	}
}
