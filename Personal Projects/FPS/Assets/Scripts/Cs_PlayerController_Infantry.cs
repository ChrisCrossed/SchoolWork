
#define DEBUG_PLATFORMS

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cs_PlayerController_Infantry : Cs_InputManager
{
    // Object Connection
    Cs_InputManager inputManager;
    GameObject go_Camera;
    GameObject go_Raycast_Front;
    GameObject go_Raycast_Back;
    GameObject go_Raycast_Left;
    GameObject go_Raycast_Right;

    // Player Variables
    GameObject this_Player;
    Rigidbody this_Rigidbody;

    // Jumping
    bool b_OnGround;
    bool b_JumpJetAllowed;

    // Speed Variables
    float f_GroundAcceleration;
    static float f_GroundAcceleration_Max = 5.0f;

    // Mouse Look Variables
    static float f_VertRotation_Max = 60f;

    internal void Initialize()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Object Connection
        go_Camera = transform.root.FindChild("Main Camera").gameObject;
        go_Raycast_Front = transform.Find("RaycastObjects").Find("Raycast_Front").gameObject;
        go_Raycast_Back = transform.Find("RaycastObjects").Find("Raycast_Back").gameObject;
        go_Raycast_Left = transform.Find("RaycastObjects").Find("Raycast_Left").gameObject;
        go_Raycast_Right = transform.Find("RaycastObjects").Find("Raycast_Right").gameObject;

        // Player variables
        this_Player = gameObject;
        this_Rigidbody = this_Player.GetComponent<Rigidbody>();
    }

    GameObject go_Platform;
    Vector3 v3_PlatformPreviousPos;
    Vector3 v3_PlatformVel;
    Vector3 PlatformVel { get { return go_Platform != null ? go_Platform.GetComponent<Rigidbody>().velocity : Vector3.zero; } }
    float MaxSpeed
    {
        get
        {
            return MaxSpeedBase + PlatformVel.magnitude;
        }
    }
    [SerializeField]
    public float MaxSpeedBase;

    // Finds the object the player is standing on, checks to see if it is moving, and applies that velocity to the player
    void ReceiveExternalVelocities()
    {
        #region Khan, here's the 'new' code: Applies the difference the object has moved. Both formats have been removed by latest verion.
        /*
        // Capture the gameobject we're standing on.
        RaycastHit hit = CheckRaycasts(LayerMask.GetMask("MovingPlatform"));

        if (hit.rigidbody)
        {
            #if DEBUG_PLATFORMS
            foreach (Renderer r in GetComponentsInChildren<Renderer>())
                r.material.color = Color.red;
            #endif

            // If it's not the same platform we were in contact with last frame
            if (go_Platform != hit.collider.gameObject)
            {
                // Store game object & position
                go_Platform = hit.collider.gameObject;
                v3_PlatformPreviousPos = hit.transform.position;
            }
            // Same platform as last frame.
            else
            {
                // Calculate differences and apply to player.
                Vector3 v3_Difference = hit.transform.position - v3_PlatformPreviousPos;
                transform.position += v3_Difference;

                // Store data
                v3_PlatformPreviousPos = hit.transform.position;
                v3_PlatformVel = v3_Difference;

                // Remove angular velocity
                //gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            }
        }
        else
        {
            #if DEBUG_PLATFORMS
            foreach (Renderer r in GetComponentsInChildren<Renderer>())
                r.material.color = Color.green;
            #endif

            // Not the same game object. Clear data.
            go_Platform = null;
            v3_PlatformPreviousPos = new Vector3();
        }
        */
        #endregion
        #region Khan, here's the old code. In various formats, the code would remain the same: Find the velocity of the touching object and add to player's velocity
        
        // Raycast downward to find the velocity of whatever we're standing on. Apply that velocity to the player.
        RaycastHit hit = CheckRaycasts();

        // Ground objects
        if(hit.collider != null)
        {
            // Get object's velocity if it has one
            Vector3 v3_TouchingVelocity = new Vector3();
            if(hit.rigidbody)
            {
                v3_TouchingVelocity = hit.rigidbody.velocity;
                // v3_TouchingVelocity = v3_TouchingVelocity.normalized;
                // return v3_TouchingVelocity;
                v3_PlatformVel = v3_TouchingVelocity.normalized;
            }
        }

        // return new Vector3();
        #endregion
    }

    void Movement()
    {
        // Receive input vector
        Vector3 v3_InputVector = new Vector3(playerInput.xDir, 0, playerInput.zDir);
        v3_InputVector.Normalize();

        // Combine input vector with player rotation. Done before we manipulate player's velocity
        v3_InputVector = this_Player.transform.rotation * v3_InputVector;

        Vector3 v3_EvaluateVelocity = this_Player.transform.rotation * this_Rigidbody.velocity;

        float f_ForwardVel = v3_EvaluateVelocity.z;
        float f_HorizVel = this_Rigidbody.velocity.x;

        print(v3_EvaluateVelocity);

        // If player isn't moving forward/backward, reduce their forward/backward velocity
        if (playerInput.zDir == 0f)
        {
            // Find their forward velocity
            float f_zDir = this_Rigidbody.velocity.z;

            // Multiply by -1 which pushes in the reverse direction
            f_zDir *= -1f;

            // Multiply by 0.3f which slowly deaccelerates the player in this direction
            f_zDir *= 0.3f; 

            // Apply velocity to player
            v3_InputVector.z = f_zDir;
        }

        // If player isn't moving horizontally, reduce their horizontal velocity
        if(playerInput.xDir == 0f)
        {
            // Find their horizontal velocity
            float f_xDir = this_Rigidbody.velocity.x;

            // Multiply by -1 which pushes in the reverse direction
            f_xDir *= -1f;

            // Multiply by 0.3f which slowly deaccelerates the player in this direction
            f_xDir *= 0.3f;

            // Apply velocity to player
            v3_InputVector.x = f_xDir;
        }
        
        // Multiply input vector by speed
        Vector3 v3_FrameVelocity = v3_InputVector * 30.0f * Time.fixedDeltaTime;

        // Speed check
        if(this_Rigidbody.velocity.magnitude < 20f)
        {
            // Add velocity to player input
            this_Rigidbody.velocity += v3_FrameVelocity;
        }

        #region Old Attempt
        /*
        Vector3 v3_OldVelocity = gameObject.GetComponent<Rigidbody>().velocity - v3_PlatformVel;

        // Find directional vector
        Vector3 inputVec = new Vector3(playerInput.xDir, 0, playerInput.zDir);
        inputVec.Normalize();

        // Moving, ignoring platform
        Rigidbody body = GetComponent<Rigidbody>();
        if (inputVec != Vector3.zero)
        {
            // Transform input direction from global space to local space
            Vector3 moveVec = gameObject.transform.rotation * inputVec;

            // Move in direction
            float acceleration = 30.0f;
            body.velocity += moveVec * acceleration * Time.fixedDeltaTime; // adding to the velocity instead of directly setting it

            // Cap velocity
            if (body.velocity.sqrMagnitude > MaxSpeedBase * MaxSpeedBase)
            {
                body.velocity = body.velocity.normalized * MaxSpeedBase;
            }
        }
        // Not moving, decelerate
        else
        {
            float decelerationPerSecond = 10.0f; // not 100% accurate since the capsule's friction will also slow it down
            float playerWithPlatformSpeed = body.velocity.magnitude; // need to take in platform's speed so that we're always at least going the speed of the platform
            float deceleratedSpeed = playerWithPlatformSpeed - (decelerationPerSecond * Time.fixedDeltaTime);
            float newMagnitude = Mathf.Max(deceleratedSpeed, 0.0f);
            body.velocity = (body.velocity.normalized * newMagnitude);
        }
        */
        #endregion

        #region Khan's notes
        /*
          Hey Chris! 
          tl;dr: The issue was that you were directly setting the velocity instead of adding to it, and overwriting physics as you did so.

          Whenever you directly set something, you're overwriting what was previously there.
          This caused issues because both you and physics were trying to affect the player's velocity, so neither of you resolved correctly
            -Remember slapping a Rigidbody onto a cube and placing it on top of a moving platform will get you an object that moves with the platform
             under it. Since this is already done (and better than either of can write, since it's done by Unity), the goal is to preserve this behavior.
              -Letting physics resolve itself normally will allow the player to move with moving platforms
            -The way we solve this is by adding to the velocity instead of directly setting it. 
              -As long as you have something like:
                                         rigidbody.velocity += val;
               instead of...
                                         rigidbody = val;
               ...you should hypothetically be fine.

          The solution I implemented here is basically what I have implemented for my AI in Night of the Living Bread: Dough Rising.
          -Add to the velocity every fixedUpdate
            -This (kind of) mimics how things actually move, which is they add a force to themselves
            -POP QUIZ: What's it called when we add to velocity?
              -ANSWER: *ding ding* YOU GOT IT! It's acceleration. We're just accelerating the player.
          -Cap the velocity after a while
            -Acceleration/deceleration doesn't feel good unless it's very high, so we need to limit it after a while
          -Manually apply deceleration if we don't have input
            -If you think about it, the way a capsule moves isn't similar to how a person moves. Capsules slide, while people
             will stick their foot out to stop.

          Possible issues with this approach
          -You're handling the velocity pretty tightly at this point, you might want to consider switching to a swept controller, or
           at least giving it a shot if you haven't already

          Why your thing wasn't actually interpolation
          -You know what, I don't really know.

          Issues in ReceiveExternalInput()
          -Directly setting the position didn't work because doing so affected GetComponent<Rigidbody>().velocity
            -Your other calcuations (the ones in Movement()) were done as if the player's velocity was not affected by what they are standing on, 
             so that threw off calculations
              -Remember that while standing still on a moving platform, the player's velocity is the platform's velocity, not (0,0,0)
        */

        //Vector3 v3_NewVelocity = Vector3.Lerp(v3_OldVelocity, v3_FinalRotation * 10, 1f / 10f);
        //gameObject.GetComponent<Rigidbody>().velocity = v3_NewVelocity;
        #endregion
    }

    float f_CamRot;
    float f_CamRot_Vel;
    float f_lookSmoothDamp = 0.05f;
    float f_CamRot_Curr;
    void MouseRotations()
    {
        #region Horizontal rotation (y axis)
        Vector3 v3_CurrRot = gameObject.transform.eulerAngles;
        v3_CurrRot.y += playerInput.mouseHoriz;
        gameObject.transform.eulerAngles = v3_CurrRot;
        #endregion

        #region Vertical (x axis)
        f_CamRot -= playerInput.mouseVert; // Inverted

        // Clamp
        f_CamRot = Mathf.Clamp(f_CamRot, -89, 89);
        f_CamRot_Curr = Mathf.SmoothDamp(f_CamRot_Curr, f_CamRot, ref f_CamRot_Vel, f_lookSmoothDamp);

        go_Camera.transform.eulerAngles = new Vector3(f_CamRot_Curr, v3_CurrRot.y, 0);
        #endregion
    }

    // Raycasts down through four points on the player, finds the closes distance, and returns that RaycastHit.
    RaycastHit CheckRaycasts(int i_LayerMask_ = -1)
    {
        // outHit is what we'll be sending out from the function
        RaycastHit outHit;

        // tempHit is what we'll compare against
        RaycastHit tempHit;

        int i_LayerMask;
        if (i_LayerMask_ == -1)
        {
            i_LayerMask = LayerMask.GetMask("Ground");
        }
        else
        {
            i_LayerMask = i_LayerMask_;
        }

        // Set the default as outHit automatically
        Physics.Raycast(go_Raycast_Front.transform.position, -transform.up, out outHit, 0.15f, i_LayerMask);

        // Begin comparing against the other three. Find the shortest distance
        if (Physics.Raycast(go_Raycast_Back.transform.position, -transform.up, out tempHit, 0.15f, i_LayerMask))
        {
            if (tempHit.distance < outHit.distance) outHit = tempHit;
        }

        if (Physics.Raycast(go_Raycast_Left.transform.position, -transform.up, out tempHit, 0.15f, i_LayerMask))
        {
            if (tempHit.distance < outHit.distance) outHit = tempHit;
        }

        if (Physics.Raycast(go_Raycast_Right.transform.position, -transform.up, out tempHit, 0.15f, i_LayerMask))
        {
            if (tempHit.distance < outHit.distance) outHit = tempHit;
        }

        // Return the shortest hit distance
        return outHit;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            DestroyImmediate(sphere.GetComponent<Collider>());
            sphere.transform.position = transform.position;
        }
    }

    // Update is called once per frame
    bool movementEnabled = true;
    void FixedUpdate()
    {
        //Call's the input manager's Update
        base.InputUpdate();

        MouseRotations();
        // ReceiveExternalVelocities();
        Movement();
    }

    #region Not Used
    void OnCollisionEnter(Collision collider_)
    {
        /*
        // If the object we're touching is the object directly below the player, we're touching the ground
        RaycastHit hit;
        int i_LayerMask = collider_.gameObject.layer;
        print(LayerMask.LayerToName(i_LayerMask));
        Physics.Raycast(gameObject.transform.position, Vector3.down, out hit, Mathf.Infinity, i_LayerMask);

        if(hit.collider)
        {
            print("Touching: " + hit.collider.gameObject.layer);
        }

        // If the player is now touching a moving platform, parent it
        if (collider_.gameObject.layer == LayerMask.NameToLayer("MovingPlatform"))
        {
            gameObject.transform.SetParent(collider_.transform.root);
        }*/
    }

    void OnCollisionExit(Collision collider_)
    {
        /*
        // If the player's parent is the same as the parent of this moving platform...
        if (gameObject.transform.parent == collider_.transform.root)
        {
            // Release it
            gameObject.transform.parent = null;
        }
        */
    }
    #endregion
}
