using UnityEngine;
using System.Collections;
using XInputDotNetPure;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor.Rendering;
#endif


public enum Enum_CameraState
{
    Lerp_ToPlayer,
    Lerp_FromPlayer,
    OnPlayer,
    OnTempPoint
}

public class Cs_PlayerController : MonoBehaviour
{
    // PLAYER STATS & INFORMATION
    public float MAX_PLAYER_SPEED;
    public float ACCELERATION;
    [Range(0, 1)]
    public float f_Magnitude_Sneak;
    [Range(0, 1)]
    public float f_Magnitude_Brisk;
    [Range(0, 2)]
    public float f_Magnitude_Sprint;

    [SerializeField]
    public float currSpeedReadOnly;

    // Player variables
    Vector3 v3_CurrentVelocity;
    bool b_IsSprinting = false;

    // Controller vs. Keyboard - Last Used
    bool b_KeyboardUsedLast;
    float f_DoubleTapForSprintTimer;

    // Controller Input
    GamePadState state;
    GamePadState prevState;
    public PlayerIndex playerOne = PlayerIndex.One;

    // Raycast Objects
    GameObject go_SlopeRaycast;

    // Camera information
    GameObject go_Camera;
    GameObject go_Camera_DefaultPos;
    GameObject go_Camera_TempPos;
    Enum_CameraState cameraState;
    float cameraLerpTime = 1.5f;
    float cameraLerpTime_Curr;

    // Abilities/Projectile
    public GameObject go_FireLocation;
    public GameObject prefab_Rock;
    public GameObject go_TargetObject;
    Vector3 v3_TargetLocation;
    public float f_FiringAngle = 45.0f;
    public float f_Gravity = 9.8f;

    // Camera Trigger variables
    float f_DisableTimer;

    // Game End bool
    bool b_GameOver;

    // Rock Timer
    float f_RockTimer = 10.0f;
    static float f_RockTimer_Max = 10.0f;
    GameObject go_RockIcon;

    // Use this for initialization
    void Start()
    {
        // Remove mouse cursor
        Cursor.visible = false;

        // Load resources to reduce hiccups
        Resources.Load("Icosphere");

        GameObject.Find("Canvas").GetComponent<Canvas>().enabled = true;

        go_RockIcon = GameObject.Find("RockIcon");

        go_SlopeRaycast = transform.FindChild("SlopeRaycast").gameObject;

        // Define Camera Information
        go_Camera = GameObject.Find("Main Camera");
        go_Camera_DefaultPos = transform.Find("Camera_Player").gameObject;

        // Abilities/Projectile
        v3_TargetLocation = go_TargetObject.transform.position;

        // Sound Effects
        as_SFXSource = gameObject.GetComponent<AudioSource>();
        ac_Grass_Light = Resources.Load("SFX_Step_Light") as AudioClip;
        ac_Grass_Heavy = Resources.Load("SFX_Step_Heavy") as AudioClip;
        ac_Gravel = Resources.Load("SFX_Gravel") as AudioClip;
        ac_Gravel_2 = Resources.Load("SFX_Gravel_2") as AudioClip;
        ac_Gravel_3 = Resources.Load("SFX_Gravel_3") as AudioClip;

        Set_CameraPosition();
    }

    // Update is called once per frame
    float f_WalkSFX_Timer = 0.5f;
    static float f_WalkSFX_Max = 0.6f;
    float f_WalkSFX_Multiplier;
    Vector3 v3_PreviousLocation_Speed;
    Vector3 v3_CurrLocation_Speed;
    void Update()
    {
        // Grab current location for speed evaluation
        v3_CurrLocation_Speed = gameObject.transform.position;

        // Quit with appropriate input
        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
        // if(state.Buttons.Back == ButtonState.Pressed && prevState.Buttons.Back == ButtonState.Released) Application.Quit();

        // Restart with appropriate input
        if (state.Buttons.Start == ButtonState.Pressed && prevState.Buttons.Start == ButtonState.Released) SceneManager.LoadScene(3);
        if (state.Buttons.Back == ButtonState.Pressed && prevState.Buttons.Back == ButtonState.Released) SceneManager.LoadScene(2);

        // Rock Update
        if (f_RockTimer < f_RockTimer_Max)
        {
            f_RockTimer += Time.deltaTime;

            // go_RockIcon.GetComponent<Image>().enabled = false;

            if (f_RockTimer > f_RockTimer_Max)
            {
                f_RockTimer = f_RockTimer_Max;

                // go_RockIcon.GetComponent<Image>().enabled = true;

                GameObject.Find("RockBackdrop").GetComponent<Image>().color = new Color(0.5f, 1f, 0.5f, 1f);
            }
            else
            {
                GameObject.Find("RockBackdrop").GetComponent<Image>().color = new Color(0.5f, 1f, 0.5f, 0f);
            }
        }

        if(!b_GameOver)
        {
            #region Play Movement SFX
            f_WalkSFX_Timer += Time.deltaTime * f_WalkSFX_Multiplier;

            // Evaluate distance travelled
            if(Vector3.Distance(v3_PreviousLocation_Speed, v3_CurrLocation_Speed) > 0.01f)
            {
                if (f_WalkSFX_Timer >= f_WalkSFX_Max)
                {
                    // Only play if the player is indeed moving
                    if(gameObject.GetComponent<Rigidbody>().velocity.magnitude > 1f)
                    {
                        Play_WalkSFX(f_WalkSFX_Multiplier);
                        f_WalkSFX_Timer = 0.0f;
                    }
                }
            }
            #endregion

            // Stop player movement while touching specific triggers
            if (f_DisableTimer > 0)
            {
                // Reduce the timer back down to 0
                f_DisableTimer -= Time.deltaTime;

                // Clamp
                if (f_DisableTimer < 0) f_DisableTimer = 0;
            }
            else
            {
                b_KeyboardUsedLast = KeyboardCheck(b_KeyboardUsedLast);

                if (b_KeyboardUsedLast)
                {
                    // Input_Keyboard();
                }
                else Input_Controller();
            }

            currSpeedReadOnly = gameObject.GetComponent<Rigidbody>().velocity.magnitude;
        }

        v3_PreviousLocation_Speed = v3_CurrLocation_Speed;
    }

    void LateUpdate()
    {
        // Update fade out/in
        FadeState();

        // Update Camera position/rotation
        UpdateCameraPosition();
    }

    bool KeyboardCheck(bool b_KeyboardPressed_)
    {
        if (Input.GetKey(KeyCode.W) ||
            Input.GetKey(KeyCode.S) ||
            Input.GetKey(KeyCode.A) ||
            Input.GetKey(KeyCode.D) ||
            Input.GetKey(KeyCode.LeftControl) ||
            Input.GetKey(KeyCode.Space))
        {
            return true;
        }

        return false;
    }

    void PlayerMovement(Vector3 v3_InputVector_, float f_Magnitude_)
    {
        // Grab previous velocity to compare against
        Vector3 v3_PreviousVelocity = gameObject.GetComponent<Rigidbody>().velocity;
        Vector3 v3_NewVelocity = v3_InputVector_ * MAX_PLAYER_SPEED * f_Magnitude_;

        v3_CurrentVelocity = Vector3.Lerp(v3_PreviousVelocity, v3_NewVelocity, ACCELERATION * Time.deltaTime);

        // Receive the ramp angle below the player
        RaycastHit rayHit = EvaluateGroundVector(LayerMask.GetMask("Ground", "Wall"));

        Vector3 v3_FinalVelocity = Vector3.ProjectOnPlane(v3_CurrentVelocity, rayHit.normal);

        if (rayHit.distance >= 0.265f)
        {
            v3_FinalVelocity.y = v3_PreviousVelocity.y - (Time.deltaTime * 50);
        }

        gameObject.GetComponent<Rigidbody>().velocity = v3_FinalVelocity;
    }

    void Input_Controller()
    {
        // Capture latest input
        prevState = state;
        state = GamePad.GetState(playerOne);

        #region Movement
        // Create new temporary Vector3 to apply Controller input
        Vector3 v3_InputVector = new Vector3();

        // Accept Left Analog Stick input, apply into Vector3
        v3_InputVector.x = state.ThumbSticks.Left.X;
        v3_InputVector.z = state.ThumbSticks.Left.Y;

        #endregion

        #region Sprinting
        float f_Magnitude = 0f;

        if (!b_IsSprinting)
        {
            if (state.Buttons.LeftStick == ButtonState.Pressed && prevState.Buttons.LeftStick == ButtonState.Released) b_IsSprinting = true;
        }
        else
        {
            if (v3_InputVector.magnitude < 0.1f) b_IsSprinting = false;
        }

        // If the player speed isn't 0, apply preset speeds
        if (v3_InputVector.magnitude != float.Epsilon)
        {
            if (v3_InputVector.magnitude < 0.15f) f_Magnitude = 0;
            else if (v3_InputVector.magnitude < 0.82f) f_Magnitude = f_Magnitude_Sneak; // 0.82 is the minimum magnitude reachable when the analog stick is pushed in one direction
            else f_Magnitude = f_Magnitude_Brisk;

            if (b_IsSprinting) f_Magnitude = f_Magnitude_Sprint;

        }
        #endregion

        #region Play Walk/Run SFX
        if (f_Magnitude == f_Magnitude_Sneak) f_WalkSFX_Multiplier = 1.0f;
        else if (f_Magnitude == f_Magnitude_Brisk) f_WalkSFX_Multiplier = 1.5f;
        else if (f_Magnitude == f_Magnitude_Sprint) f_WalkSFX_Multiplier = 2.0f;
        else if (f_Magnitude == 0) f_WalkSFX_Multiplier = 0.0f;
        #endregion

        #region Update Aim/Fire
        bool b_AllowedToFire_ReticleMagnitude = false;

        // Get the currect Vector of the right analog stick
        Vector2 v2_RightStickVector;
        v2_RightStickVector.x = state.ThumbSticks.Right.X;
        v2_RightStickVector.y = state.ThumbSticks.Right.Y;
        v2_RightStickVector.Normalize();

        // Get the 'magnitude' of the stick being pressed in
        //float f_RightStickMagnitude = Mathf.Sqrt(Vector2.SqrMagnitude(new Vector2(state.ThumbSticks.Right.X, state.ThumbSticks.Right.Y)));
        float f_RightStickMagnitude = new Vector2(state.ThumbSticks.Right.X, state.ThumbSticks.Right.Y).magnitude;

        // Pass information in to the UpdateReticle function
        v3_TargetLocation = go_TargetObject.transform.position;

        if( UpdateReticle(v2_RightStickVector, f_RightStickMagnitude) )
        {
            if ( f_RightStickMagnitude >= 0.4f )
            {
                b_AllowedToFire_ReticleMagnitude = true;
            }
        }
        #endregion

        #region Use Ability
        // Throw Rock
        if(f_RockTimer == f_RockTimer_Max)
        {
            if (state.Buttons.RightShoulder == ButtonState.Pressed && prevState.Buttons.RightShoulder == ButtonState.Released &&
                b_AllowedToFire_ReticleMagnitude)
            {
                float f_StickMagnitude = Vector2.SqrMagnitude(new Vector2(state.ThumbSticks.Right.X, state.ThumbSticks.Right.Y));
                Vector3 v3_ThrowVector = CalculateThrow(f_StickMagnitude);

                ThrowRockAtLocation(v3_ThrowVector);

                f_RockTimer = 0f;
            }
        }
        #endregion

        // Normalize
        v3_InputVector.Normalize();

        // Pass information into PlayerMovement()
        PlayerMovement(v3_InputVector, f_Magnitude);
    }

    public float f_AimingDistance = 5.0f;
    float f_ReticleFadeTimer;
    private Vector3 velocity = Vector3.zero;
    Vector3 v3_OldPosition;
    bool UpdateReticle(Vector2 v2_Vector_, float f_Magnitude)
    {
        // Reposition the Reticle's X/Z in comparison to the player's position
        Vector3 v3_ReticlePosition = gameObject.transform.position;
        Vector3 v3_ConvertedVector = new Vector3(v2_Vector_.x, 0, v2_Vector_.y);
        v3_ReticlePosition += v3_ConvertedVector * f_Magnitude * f_AimingDistance;

        // int layer_mask = LayerMask.GetMask("Player", "Enemy");
        int layer_mask = LayerMask.GetMask("Ground", "Wall");

        // Raycast down from the Reticle's current X/Z position to find ground to apply on to
        RaycastHit hit;
        Physics.Raycast(new Vector3(v3_ReticlePosition.x, v3_ReticlePosition.y + 5, v3_ReticlePosition.z), -transform.up, out hit, 10.0f, layer_mask);

        if(hit.collider)
        {
            Vector3 v3_NewPosition = hit.point;
            v3_NewPosition.y += 0.1f;

            // Rotate the reticle to match the surface it hits
            Vector3 v3_FinalRotation = Vector3.ProjectOnPlane(-transform.up + new Vector3(90, 0, 0), hit.normal);

            // Apply final position
            // go_TargetObject.transform.position = v3_NewPosition;
            v3_NewPosition = Vector3.Lerp(v3_OldPosition, v3_NewPosition, Time.deltaTime * 10f);
            go_TargetObject.transform.position = v3_NewPosition;

            go_TargetObject.transform.eulerAngles = v3_FinalRotation;

            if (f_Magnitude >= 0.4f)
            {
                if (f_ReticleFadeTimer < 1.0f)
                {
                    f_ReticleFadeTimer += Time.deltaTime * 10;

                    if (f_ReticleFadeTimer > 1.0f) f_ReticleFadeTimer = 1.0f;
                }
            }
            else
            {
                if (f_ReticleFadeTimer > 0)
                {
                    if (f_ReticleFadeTimer > 0.5f) f_ReticleFadeTimer = 0.5f;

                    f_ReticleFadeTimer -= Time.deltaTime;

                    if (f_ReticleFadeTimer < 0.0f) f_ReticleFadeTimer = 0.0f;
                }
            }

            Color clr_Fade = go_TargetObject.GetComponent<MeshRenderer>().material.color;
            clr_Fade.a = f_ReticleFadeTimer;
            go_TargetObject.GetComponent<MeshRenderer>().material.color = clr_Fade;

            v3_OldPosition = go_TargetObject.transform.position;
        }
        // The raycast did not hit the wall or ground. Pull the target back toward the player until we find solid ground
        else
        {
            // Reset details
            bool b_FoundSpot = false;
            float f_NewMagnitude = f_Magnitude;

            while(!b_FoundSpot)
            {
                if(f_NewMagnitude > 0.41f)
                {
                    // Resetting basic details.
                    v3_ReticlePosition = gameObject.transform.position;

                    // Decrementing the magnitude to test with
                    if(f_NewMagnitude > 0.415f)
                    {
                        f_NewMagnitude -= 0.015f;

                        // Setting new position
                        v3_ReticlePosition += v3_ConvertedVector * f_NewMagnitude * f_AimingDistance;

                        // Perform a raycast to see if a new position exists
                        if(Physics.Raycast(new Vector3(v3_ReticlePosition.x, v3_ReticlePosition.y + 5, v3_ReticlePosition.z), -transform.up, out hit, 10.0f, layer_mask))
                        {
                            #region Raycast - Same process as above
                            Vector3 v3_NewPosition = hit.point;
                            v3_NewPosition.y += 0.1f;

                            // Rotate the reticle to match the surface it hits
                            Vector3 v3_FinalRotation = Vector3.ProjectOnPlane(-transform.up + new Vector3(90, 0, 0), hit.normal);

                            // Apply final position
                            v3_NewPosition = Vector3.Lerp(v3_OldPosition, v3_NewPosition, Time.deltaTime * 10f);
                            go_TargetObject.transform.position = v3_NewPosition;

                            go_TargetObject.transform.eulerAngles = v3_FinalRotation;

                            if (f_Magnitude >= 0.4f)
                            {
                                if (f_ReticleFadeTimer < 1.0f)
                                {
                                    f_ReticleFadeTimer += Time.deltaTime * 10;

                                    if (f_ReticleFadeTimer > 1.0f) f_ReticleFadeTimer = 1.0f;
                                }
                            }

                            Color clr_Fade = go_TargetObject.GetComponent<MeshRenderer>().material.color;
                            clr_Fade.a = f_ReticleFadeTimer;
                            go_TargetObject.GetComponent<MeshRenderer>().material.color = clr_Fade;

                            v3_OldPosition = go_TargetObject.transform.position;
                            #endregion

                            // End while loop
                            b_FoundSpot = true;
                            return true;
                        }
                    }
                    // Magnitude is less than the minimum. Set position and break out.
                    else
                    {
                        if (f_ReticleFadeTimer > 0)
                        {
                            if (f_ReticleFadeTimer > 0.5f) f_ReticleFadeTimer = 0.5f;

                            f_ReticleFadeTimer -= Time.deltaTime;

                            if (f_ReticleFadeTimer < 0.0f) f_ReticleFadeTimer = 0.0f;
                        }

                        Color clr_Fade = go_TargetObject.GetComponent<MeshRenderer>().material.color;
                        clr_Fade.a = f_ReticleFadeTimer;
                        go_TargetObject.GetComponent<MeshRenderer>().material.color = clr_Fade;

                        // v3_OldPosition = go_TargetObject.transform.position;
                        go_TargetObject.transform.position = gameObject.transform.position;

                        // Break out so we don't crash
                        b_FoundSpot = true;
                        // return false;
                    }
                }
                // Magnitude is less than the minimum. Set position & break out.
                else
                {
                    if (f_ReticleFadeTimer > 0)
                    {
                        if (f_ReticleFadeTimer > 0.5f) f_ReticleFadeTimer = 0.5f;

                        f_ReticleFadeTimer -= Time.deltaTime;

                        if (f_ReticleFadeTimer < 0.0f) f_ReticleFadeTimer = 0.0f;
                    }

                    Color clr_Fade = go_TargetObject.GetComponent<MeshRenderer>().material.color;
                    clr_Fade.a = f_ReticleFadeTimer;
                    go_TargetObject.GetComponent<MeshRenderer>().material.color = clr_Fade;

                    // v3_OldPosition = go_TargetObject.transform.position;
                    go_TargetObject.transform.position = gameObject.transform.position;

                    // Break out so we don't crash
                    b_FoundSpot = true;
                    return false;
                }
            }
        }

        return true;
    }

    void Input_Keyboard()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //Vector3 v3_ThrowVector = CalculateThrow();

            //ThrowRockAtLocation(v3_ThrowVector);
        }

        #region Movement
        // Create new temporary Vector3 to apply keyboard input
        Vector3 v3_InputVector = new Vector3();

        // Accept keyboard input, apply into Vector3
        if (Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S)) v3_InputVector.z = 1;
        else if (Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W)) v3_InputVector.z = -1;

        if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D)) v3_InputVector.x = -1;
        else if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A)) v3_InputVector.x = 1;
        #endregion

        #region Sprinting
        float f_Magnitude = 0f;

        #region Shift Key
        if (!b_IsSprinting)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift)) b_IsSprinting = true;
        }
        else
        {
            if (v3_InputVector.magnitude < 0.1f) b_IsSprinting = false;
        }
        #endregion

        #region Double Tap Direction

        // Reference Variable
        float f_TIME_ALLOWANCE = 0.4f;

        // Decrement the timer if it's currently 'active'
        if (f_DoubleTapForSprintTimer > 0.0f)
        {
            // Decrement
            f_DoubleTapForSprintTimer -= Time.deltaTime;

            // Clamp
            if (f_DoubleTapForSprintTimer < 0.0f) f_DoubleTapForSprintTimer = 0.0f;
        }

        // If the player pressed a button, evaluate if we BEGIN sprinting
        if (Input.GetKeyDown(KeyCode.W) ||
            Input.GetKeyDown(KeyCode.S) ||
            Input.GetKeyDown(KeyCode.A) ||
            Input.GetKeyDown(KeyCode.D))
        {
            // First press
            if (f_DoubleTapForSprintTimer == 0.0f)
            {
                // Set timer to the Alloted Time to wait
                f_DoubleTapForSprintTimer = f_TIME_ALLOWANCE;
            }
            else
            {
                // This is the second (or up) time we pressed a movement button within the time limit. Sprint.
                b_IsSprinting = true;
            }
        }

        // If we're already sprinting and are holding a directional key down, keep the sprint timer up. Allows walking into sprinting.
        if (b_IsSprinting)
        {
            if (Input.GetKey(KeyCode.W) ||
                 Input.GetKey(KeyCode.S) ||
                 Input.GetKey(KeyCode.A) ||
                 Input.GetKey(KeyCode.D))
            {
                f_DoubleTapForSprintTimer = f_TIME_ALLOWANCE;
            }
        }

        // print("Sprinting: " + b_IsSprinting + ", Timer: " + f_DoubleTapForSprintTimer);
        #endregion

        // If the player speed isn't 0, apply preset speeds
        if (v3_InputVector.magnitude != float.Epsilon)
        {
            if (v3_InputVector.magnitude < 0.15f) f_Magnitude = 0;
            else if (v3_InputVector.magnitude < 0.5f) f_Magnitude = f_Magnitude_Sneak;
            else f_Magnitude = f_Magnitude_Brisk;

            if (b_IsSprinting) f_Magnitude = f_Magnitude_Sprint;
        }
        #endregion

        // Normalize
        v3_InputVector.Normalize();

        // Pass information into PlayerMovement()
        PlayerMovement(v3_InputVector, f_Magnitude);
    }

    void UpdateCameraPosition()
    {
        if (cameraState == Enum_CameraState.Lerp_FromPlayer || cameraState == Enum_CameraState.Lerp_ToPlayer)
        {
            // Camera timer increments as it travels to the temp location
            if (cameraState == Enum_CameraState.Lerp_FromPlayer)
            {
                cameraLerpTime_Curr += Time.deltaTime;

                if (cameraLerpTime_Curr > cameraLerpTime)
                {
                    cameraLerpTime_Curr = cameraLerpTime;
                }
            }
            // Camera timer decrements as it travels back to the player
            else
            {
                cameraLerpTime_Curr -= (Time.deltaTime * 2);

                if (cameraLerpTime_Curr <= 0)
                {
                    cameraLerpTime_Curr = 0;
                }
            }

            // Lerp calculations
            float perc = cameraLerpTime_Curr / cameraLerpTime;

            if (go_Camera_TempPos != null && go_Camera_DefaultPos != null)
            {
                Vector3 v3_Vector = go_Camera_TempPos.transform.position - go_Camera_DefaultPos.transform.position;
                // go_Camera_TempPos.transform.rotation - go_Camera_DefaultPos.transform.rotation;
                // Quaternion q_Rot = Quaternion.FromToRotation(go_Camera_TempPos.transform.eulerAngles, go_Camera_DefaultPos.transform.position);
                // Vector3 v3_Rotation = go_Camera_TempPos.transform.eulerAngles - go_Camera_DefaultPos.transform.eulerAngles;

                go_Camera.transform.position = go_Camera_DefaultPos.transform.position + (v3_Vector * perc);
                // go_Camera.transform.eulerAngles = go_Camera_DefaultPos.transform.eulerAngles + (v3_Rotation * perc);
                // go_Camera.transform.position = go_Camera_DefaultPos.transform.rotation + (q_Rot * perc);
                go_Camera.transform.rotation = Quaternion.Lerp(go_Camera_DefaultPos.transform.rotation, go_Camera_TempPos.transform.rotation, perc);
            }
        }
    }

    GameObject go_PreviousCameraPos;
    public void Set_CameraPosition(GameObject go_CameraPos_ = null)
    {
        // Only change away from the previous camera once we've already changed to the new one (Stops camera errors from differing, close-by triggers)
        if (go_PreviousCameraPos != go_Camera_TempPos)
        {
            go_PreviousCameraPos = go_Camera_TempPos;
        }

        if (go_CameraPos_ == null)
        {
            cameraState = Enum_CameraState.Lerp_ToPlayer;
        }
        else
        {
            if (cameraLerpTime_Curr == 0)
            {
                go_Camera_TempPos = go_CameraPos_;
                cameraState = Enum_CameraState.Lerp_FromPlayer;
            }
        }
    }

    RaycastHit EvaluateGroundVector(LayerMask layerMask_)
    {
        RaycastHit hit;

        if (layerMask_ != new LayerMask())
        {
            Physics.Raycast(go_SlopeRaycast.transform.position, -transform.up, out hit, float.PositiveInfinity, layerMask_);
        }
        else
        {
            Physics.Raycast(go_SlopeRaycast.transform.position, -transform.up, out hit);
        }

        return hit;
    }

    Vector3 CalculateThrow(float f_AnalogStickMagnitude_)
    {
        f_Gravity = Physics.gravity.magnitude;
        float f_Angle = (f_FiringAngle * f_AnalogStickMagnitude_) * Mathf.Deg2Rad;

        Vector3 v3_HorizontalTarget = new Vector3(v3_TargetLocation.x, 0, v3_TargetLocation.z);
        Vector3 v3_HorizontalPosition = new Vector3(gameObject.transform.position.x, 0, gameObject.transform.position.z);

        float f_Distance = Vector3.Distance(v3_HorizontalTarget, v3_HorizontalPosition);
        float f_yOffset = gameObject.transform.position.y - v3_TargetLocation.y;

        float f_InitialVelocity = (1 / Mathf.Cos(f_Angle)) * Mathf.Sqrt((0.5f * f_Gravity * Mathf.Pow(f_Distance, 2)) / (f_Distance * Mathf.Tan(f_Angle) + f_yOffset));

        Vector3 v3_Velocity = new Vector3(0, f_InitialVelocity * Mathf.Sin(f_Angle), f_InitialVelocity * Mathf.Cos(f_Angle));

        float f_AngleBetweenObjects = Vector3.Angle(Vector3.forward, v3_HorizontalTarget - v3_HorizontalPosition);

        Vector3 v3_FinalVelocity = Quaternion.AngleAxis(f_AngleBetweenObjects, Vector3.up) * v3_Velocity;


        if (v3_HorizontalTarget.x < v3_HorizontalPosition.x)
        {
            v3_FinalVelocity.x *= -1;
        }

        return v3_FinalVelocity;
    }

    void ThrowRockAtLocation(Vector3 v3_Velocity_)
    {
        GameObject go_Rock = (GameObject)Instantiate(prefab_Rock, go_FireLocation.transform.position, gameObject.transform.rotation);

        if (v3_Velocity_ != new Vector3())
        {
            // Applies a velocity on the object as it's thrown
            go_Rock.GetComponent<Rigidbody>().velocity = v3_Velocity_;

            // Applies a spin for one frame as it's thrown, like a grenade
            go_Rock.GetComponent<Rigidbody>().AddRelativeTorque(v3_Velocity_ * 100, ForceMode.Force);
        }
    }

    public void Set_PlayerDisableTimer(float f_DisableTimer_)
    {
        f_DisableTimer = f_DisableTimer_;
    }

    public bool GameOver
    {
        set
        {
            if(value == true)
            {
                gameObject.GetComponent<CapsuleCollider>().enabled = false;
            }

            b_GameOver = value;
        }
        get { return b_GameOver; }
    }

    bool b_FadeToBlack = false;
    public void Set_FadeState( bool b_FadeToBlack_ )
    {
        if(b_FadeToBlack_)
        {
            f_FadeTimer = -1.0f;
        }

        b_FadeToBlack = b_FadeToBlack_;
    }

    float f_FadeTimer = 3.0f;
    float f_FadeTimer_Max = 1.5f;
    float f_Transparency;
    [SerializeField] GameObject go_FadeInOutObj;
    void FadeState()
    {
        // 0 timer = 0 opacity (completely transparent). full timer = full opacity.
        // Not fading to black, we go transparent.
        if (b_FadeToBlack)
        {
            if (f_FadeTimer < f_FadeTimer_Max) f_FadeTimer += Time.deltaTime;

            if (f_FadeTimer > f_FadeTimer_Max) f_FadeTimer = f_FadeTimer_Max;
        }
        else // Go transparent
        {
            if (f_FadeTimer > 0) f_FadeTimer -= Time.deltaTime / 2f;

            if (f_FadeTimer < 0) f_FadeTimer = 0;

        }

        f_Transparency = f_FadeTimer / f_FadeTimer_Max;
        
        if (f_Transparency > 1)
        {
            f_Transparency = 1;
        }

        if(f_Transparency == 1f)
        {
            if (b_GameOver)
            {
                // Activate Mission Report
                print("Mission Report");
                GameObject.Find("Canvas").GetComponent<Cs_MissionReport>().Set_ActivateGrading();

                gameObject.GetComponent<Cs_PlayerController>().enabled = false;
            }
        }

        // Set fade object's transparency
        Color clr_CurrColor = go_FadeInOutObj.GetComponent<CanvasRenderer>().GetColor();
        clr_CurrColor.a = f_Transparency;
        go_FadeInOutObj.GetComponent<CanvasRenderer>().SetColor(clr_CurrColor);
    }

    AudioClip ac_Grass_Light;
    AudioClip ac_Grass_Heavy;
    AudioClip ac_Gravel;
    AudioClip ac_Gravel_2;
    AudioClip ac_Gravel_3;
    AudioSource as_SFXSource;
    float f_PrevMult;
    void Play_WalkSFX(float f_SFXMultiplier_)
    {
        if (f_SFXMultiplier_ != f_PrevMult)
        {
            f_WalkSFX_Timer = f_WalkSFX_Max - 0.05f;
        }

        LayerMask i_LayerMask = LayerMask.GetMask("Ground", "Wall");
        RaycastHit hit;
        Physics.Raycast(gameObject.transform.position, -transform.up, out hit, float.PositiveInfinity, i_LayerMask);

        if (f_SFXMultiplier_ == 1.0f)
        {
            as_SFXSource.volume = 0.2f;
            as_SFXSource.pitch = 1.0f;

            if (hit.collider.tag == "Gravel")
            {
                as_SFXSource.pitch = Random.Range(0.8f, 0.9f);
                int i_RandomPick = Random.Range(0, 3);

                if (i_RandomPick == 0) gameObject.GetComponent<AudioSource>().PlayOneShot(ac_Gravel);
                else if (i_RandomPick == 1) gameObject.GetComponent<AudioSource>().PlayOneShot(ac_Gravel_2);
                else if (i_RandomPick == 2) gameObject.GetComponent<AudioSource>().PlayOneShot(ac_Gravel_3);
            }
            else
            {
                gameObject.GetComponent<AudioSource>().PlayOneShot(ac_Grass_Light);
            }
        }
        else if (f_SFXMultiplier_ == 1.5f)
        {
            as_SFXSource.volume = 0.4f;
            as_SFXSource.pitch = 1.0f;

            if (hit.collider.tag == "Gravel")
            {
                as_SFXSource.pitch = Random.Range(0.9f, 1.0f);
                int i_RandomPick = Random.Range(0, 3);

                if (i_RandomPick == 0) gameObject.GetComponent<AudioSource>().PlayOneShot(ac_Gravel);
                else if (i_RandomPick == 1) gameObject.GetComponent<AudioSource>().PlayOneShot(ac_Gravel_2);
                else if (i_RandomPick == 2) gameObject.GetComponent<AudioSource>().PlayOneShot(ac_Gravel_3);
            }
            else
            {
                gameObject.GetComponent<AudioSource>().PlayOneShot(ac_Grass_Heavy);
            }
        }
        else if (f_SFXMultiplier_ == 2.0f)
        {
            as_SFXSource.volume = 1.0f;
            as_SFXSource.pitch = 1.1f;

            if (hit.collider.tag == "Gravel")
            {
                as_SFXSource.pitch = Random.Range(1.0f, 1.1f);
                int i_RandomPick = Random.Range(0, 3);

                if (i_RandomPick == 0) gameObject.GetComponent<AudioSource>().PlayOneShot(ac_Gravel);
                else if (i_RandomPick == 1) gameObject.GetComponent<AudioSource>().PlayOneShot(ac_Gravel_2);
                else if (i_RandomPick == 2) gameObject.GetComponent<AudioSource>().PlayOneShot(ac_Gravel_3);
            }
            else
            {
                gameObject.GetComponent<AudioSource>().PlayOneShot(ac_Grass_Heavy);
            }
        }

        f_PrevMult = f_SFXMultiplier_;
    }
}
