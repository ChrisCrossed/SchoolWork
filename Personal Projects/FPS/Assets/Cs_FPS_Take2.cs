using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cs_FPS_Take2 : Cs_InputManager
{
    // Player Connections
    GameObject this_Player;
    Rigidbody this_RigidBody;

    // Speed & Movement
    float f_Speed;
    static float f_Acceleration = 10f;
    static float f_Acceleration_Max = 50f;

	// Use this for initialization
	void Start ()
    {
        this_Player = gameObject;
        this_RigidBody = this_Player.GetComponent<Rigidbody>();
	}

    void Movement()
    {
        if (playerInput.zDir != 0f || playerInput.xDir != 0f)
        {
            print("Here");

            // Increase acceleration
            f_Speed += f_Acceleration * Time.fixedDeltaTime;
            if (f_Speed > f_Acceleration_Max) f_Speed = f_Acceleration_Max;

            this_RigidBody.MovePosition((this_RigidBody.position * f_Speed));
        }
    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        base.InputUpdate();

        Movement();
	}
}
