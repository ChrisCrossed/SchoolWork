using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum Enum_Tutorial
{
    Startup,
    Jump,
    Jetpack,
    LookHoriz
}

public enum Enum_SFX
{
    Jetpack,
    Grass,
    Wind
}

public class Cs_SkiingPlayerController : MonoBehaviour
{
    GameObject go_RaycastPoint_1;
    GameObject go_RaycastPoint_2;
    GameObject go_RaycastPoint_3;
    GameObject go_RaycastPoint_4;

    // Physics Materials
    PhysicMaterial physMat_Ski;
    PhysicMaterial physMat_Walk;

    // Jump bool. Resets on collision with ground
    bool b_CanJump;

    [SerializeField] float f_MaxSpeed;

    // Tutorial locks
    bool b_Startup_Tutorial = false;
    bool b_JumpAllowed_Tutorial = false;
    bool b_JetpackAllowed_Tutorial = false;
    bool b_LookHorizontalAllowed_Tutorial = false;

    // Voiceovers
    public GameObject go_SFX_Wind;
    AudioSource sfx_Wind;
    public GameObject go_SFX_Jetpack;
    AudioSource sfx_Jetpack;
    public GameObject go_SFX_Grass;
    AudioSource sfx_Grass;
    float f_VolumeWind = 0.0f;
    float f_VolumeWind_Max = 0.5f;
    float f_VolumeJetpack = 0.0f;
    float f_VolumeJetpack_Max = 0.5f;
    float f_VolumeGrass = 0.0f;
    float f_VolumeGrass_Max = 0.5f;

    AudioSource audioSource;
    public AudioClip ac_Intro_1 = new AudioClip();
    public AudioClip ac_SuitLocked_2 = new AudioClip();
    public AudioClip ac_Spacebar_3 = new AudioClip();
    public AudioClip ac_HoldButton_4 = new AudioClip();
    public AudioClip ac_NotBad_5 = new AudioClip();
    public AudioClip ac_SuitReleased_6 = new AudioClip();
    public AudioClip ac_SpaceJumps_7 = new AudioClip();
    public AudioClip ac_HighWall_8 = new AudioClip();
    public AudioClip ac_WindowContinue_9 = new AudioClip();

    // Use this for initialization
    void Start ()
    {
        go_JetpackUI = GameObject.Find("Jetpack");

        if (go_JetpackUI.GetComponent<Cs_JetpackHud>())
        {
            go_JetpackUI.GetComponent<Cs_JetpackHud>().Set_HUDPercentage(f_Jetpack_Curr / f_Jetpack_Max);
        }

        f_xRot = transform.eulerAngles.y;
        f_xRot_Curr = f_xRot;

        go_RaycastPoint_1 = transform.Find("RaycastPoint_1").gameObject;
        go_RaycastPoint_2 = transform.Find("RaycastPoint_2").gameObject;
        go_RaycastPoint_3 = transform.Find("RaycastPoint_3").gameObject;
        go_RaycastPoint_4 = transform.Find("RaycastPoint_4").gameObject;

        physMat_Ski  = (PhysicMaterial)Resources.Load("PhysMat_Ski");
        physMat_Walk = (PhysicMaterial)Resources.Load("PhysMat_Walk");

        go_Camera = transform.Find("MainCamera").gameObject;

        Cursor.lockState = CursorLockMode.Locked;

        // Set Audio Sources
        audioSource = gameObject.GetComponent<AudioSource>(); // Music
        sfx_Grass = go_SFX_Grass.GetComponent<AudioSource>(); // Grass
        sfx_Jetpack = go_SFX_Jetpack.GetComponent<AudioSource>(); // Jetpack
        sfx_Wind = go_SFX_Wind.GetComponent<AudioSource>(); // Wind

        // TODO: Remove
        sfx_Wind.volume = 0.0f;
        sfx_Jetpack.volume = 0.0f;
        sfx_Grass.volume = 0.0f;
    }

    Vector3 v3_Velocity;
    float f_JumpTimer;

    // Update is called once per frame
    bool b_JumpUnlocked;
    bool b_UI_Jump;
    bool b_UI_Jetpack;
    bool b_UI_LookHoriz;
    void Update ()
    {
        #region Cheat Codes
        if(Input.GetKeyDown(KeyCode.P))
        {
            Set_TutorialState(Enum_Tutorial.Jetpack);
            Set_TutorialState(Enum_Tutorial.Jump);
            Set_TutorialState(Enum_Tutorial.LookHoriz);
            Set_TutorialState(Enum_Tutorial.Startup);
            
        }
        if(Input.GetKeyDown(KeyCode.Y))
        {
            Set_ResetVelocity(GameObject.Find("Checkpoint_1"));
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            Set_ResetVelocity(GameObject.Find("Camera_Render_1"));
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            Set_ResetVelocity(GameObject.Find("RespawnPoint_3"));
        }
        #endregion

        UnlockSuit_Tutorial();

        if(b_JumpUnlocked)
        {
            UnlockSuit_Jump();
        }

        #region UI Lerping
        if(b_UI_Jetpack)
        {
            GameObject go_LookJetpack = GameObject.Find("Jetpack_Color");

            Color clr_Curr = go_LookJetpack.GetComponent<Image>().color;

            if (clr_Curr.r > 0)
            {
                clr_Curr.r -= Time.deltaTime;
                clr_Curr.g += Time.deltaTime;

                if (clr_Curr.r < 0)
                {
                    clr_Curr.r = 0;
                    clr_Curr.g = 1;

                    b_UI_Jetpack = false;
                }
            }

            go_LookJetpack.GetComponent<Image>().color = clr_Curr;
        }
        if(b_UI_Jump)
        {
            GameObject go_LookJump = GameObject.Find("Jump_Color");

            Color clr_Curr = go_LookJump.GetComponent<Image>().color;

            if (clr_Curr.r > 0)
            {
                clr_Curr.r -= Time.deltaTime;
                clr_Curr.g += Time.deltaTime;

                if (clr_Curr.r < 0)
                {
                    clr_Curr.r = 0;
                    clr_Curr.g = 1;

                    b_UI_Jump = false;
                }
            }

            go_LookJump.GetComponent<Image>().color = clr_Curr;
        }
        if(b_UI_LookHoriz)
        {
            GameObject go_LookHoriz = GameObject.Find("Look_Color");

            Color clr_Curr = go_LookHoriz.GetComponent<Image>().color;
            
            if(clr_Curr.r > 0)
            {
                clr_Curr.r -= Time.deltaTime;
                clr_Curr.g += Time.deltaTime;

                if(clr_Curr.r < 0)
                {
                    clr_Curr.r = 0;
                    clr_Curr.g = 1;

                    b_UI_LookHoriz = false;
                }
            }

            go_LookHoriz.GetComponent<Image>().color = clr_Curr;
        }
        #endregion

        //print("Current speed: " + gameObject.GetComponent<Rigidbody>().velocity.magnitude);
        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();

        if(b_Startup_Tutorial)
        {
            // Update mouse look
            MouseInput();

            Jetpack();
            
            #region PlayerSliding

            // On the first moment the spacebar is pressed, jump
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Jump();
            }
            // Otherwise, if the button is held down, the player is skiing
            else if (Input.GetKey(KeyCode.Space))
            {
                // Increment a timer to be sure the whole jump can be fulfilled
                if(f_JumpTimer < 0.5f) f_JumpTimer += Time.deltaTime;

                if(f_JumpTimer > 0.5f) Ski();
            }
            // If player is not skiing
            else
            {
                // Set PhysicsMaterial
                gameObject.GetComponent<Collider>().material = physMat_Walk;

                PlayerInput();

                // Decrease skiing volume
                SetVolume( Enum_SFX.Grass, false );
            }

            // GrappleHook();
            #endregion
        }

        // Get player speed, lerp camera's FOV between 60 & upper limit
        GameObject go_Particle = GameObject.Find("Particle");
        float f_PlayerSpeed = gameObject.GetComponent<Rigidbody>().velocity.magnitude;
        float f_Percent = f_PlayerSpeed / (f_MaxSpeed * 2);
        
        if(f_PlayerSpeed >= 15.0f)
        {
            if (f_Percent > 1.0f) f_Percent = 1.0f;

            // Particle Size: 0.0f to 0.2f
            go_Particle.GetComponent<ParticleSystem>().startSize = f_Percent * 0.15f;

            // Particle Speed: 5.0f to 20.0f
            go_Particle.GetComponent<ParticleSystem>().startSpeed = (f_Percent * 12f) + 3.0f;

            // Set wind sfx volume
            if(f_VolumeWind < 0.15f)
            {
                SetVolume( Enum_SFX.Wind, true );
            }
            else
            {
                SetVolume( Enum_SFX.Wind, f_Percent * 0.75f );
            }
        }
        else
        {
            // Particle Size: 0.0f to 0.2f
            go_Particle.GetComponent<ParticleSystem>().startSize = 0f;

            // Particle Speed: 5.0f to 20.0f
            go_Particle.GetComponent<ParticleSystem>().startSpeed = 0f;

            // Set wind sfx volume
            SetVolume(Enum_SFX.Wind, false);
        }

        // Camera FOV: 60f to 75f
        float f_FOV = (f_Percent * 15f) + 60f;
        if (f_FOV > 80) f_FOV = 80f;
        go_Camera.GetComponent<Camera>().fieldOfView = f_FOV;

        if(b_GameOver)
        {
            UnlockSuit_GameOver();

            f_GameOverTimer += Time.deltaTime;

            if(f_GameOverTimer >= 10.0f)
            {
                Application.Quit();

                print("WE QUIT");
            }

            // Increment alpha of overlay
            #region Alpha Overlay
            Color clr_FadeInOut = GameObject.Find("UI_FadeInOut").GetComponent<Image>().color;

            if(clr_FadeInOut.a < 1.0f)
            {
                clr_FadeInOut.a += Time.deltaTime * 0.25f;

                if (clr_FadeInOut.a > 1.0f) clr_FadeInOut.a = 1.0f;

                GameObject.Find("UI_FadeInOut").GetComponent<Image>().color = clr_FadeInOut;
            }
            #endregion
        }
    }

    float f_TutTimer_Jump;
    float f_TutTimer_Jump_Minimum = 2f;
    float f_TutTimer_Jump_Difference = 0.5f;
    int i_TutCounter_Jump;
    const int i_TutCounter_Max = 20;
    bool b_TutCompleted_Jump = false;
    void UnlockSuit_Jump()
    {
        if (!b_TutCompleted_Jump)
        {
            string s_TextToPrint = "";

            if (f_TutTimer_Jump < f_TutTimer_Jump_Minimum)
            {
                f_TutTimer_Jump += Time.deltaTime;

                if (f_TutTimer_Jump > f_TutTimer_Jump_Minimum)
                {
                    // Increment counter
                    ++i_TutCounter_Jump;

                    // Decrement timer by difference
                    f_TutTimer_Jump -= f_TutTimer_Jump_Difference;

                    #region Switch
                    // Set text to print
                    switch (i_TutCounter_Jump)
                    {
                        case 1:
                            s_TextToPrint = "***BIOS_ACTIVE***\n";
                            break;

                        case 2:
                            s_TextToPrint = "\n";
                            break;

                        case 3:
                            s_TextToPrint = "INIT_UNLOCK() ";
                            break;

                        case 4:
                            s_TextToPrint = ". ";
                            PlayAudioClip(6);
                            break;

                        case 5:
                            s_TextToPrint = ". ";
                            break;

                        case 6:
                            s_TextToPrint = ". ";
                            // PlayAudioClip(2);
                            break;

                        case 7:
                            s_TextToPrint = " ";
                            break;

                        case 8:
                            s_TextToPrint = "SUCCESS";

                            b_JumpAllowed_Tutorial = true;
                            b_LookHorizontalAllowed_Tutorial = true;

                            b_UI_Jump = true;
                            b_UI_LookHoriz = true;
                            break;

                        case 9:
                            s_TextToPrint = "\n";
                            gameObject.GetComponent<Cs_TextHint>().Set_TextHint("Press Spacebar to Jump\nPress & Hold Spacebar to slide\nW/A/S/D to Move");
                            b_CanJump = true;
                            break;

                        case 10:
                            s_TextToPrint = "\nHEV ACTIVE...";
                            break;

                        case 13:
                            // PlayAudioClip(3);
                            break;

                        case 11:
                            s_TextToPrint = "";
                            break;

                        case i_Counter_Max:
                            s_TextToPrint = " ";
                            break;

                        default:
                            break;
                    }
                    #endregion

                    // Call Set_HUDTest
                    if (i_TutCounter_Jump < i_TutCounter_Max)
                    {
                        Set_HUDTest(s_TextToPrint);
                    }
                    else
                    {
                        b_TutCompleted_Jump = true;

                        Set_HUDTest(s_TextToPrint, true);
                    }
                }
            }
        }
    }

    float f_TutTimer_GameOver;
    float f_TutTimer_GameOver_Minimum = 0f;
    float f_TutTimer_GameOver_Difference = 0.5f;
    int i_TutCounter_GameOver;
    const int i_GameOverCounter_Max = 20;
    bool b_TutCompleted_GameOver = false;
    void UnlockSuit_GameOver()
    {
        if (!b_TutCompleted_GameOver)
        {
            string s_TextToPrint = "";

            if (f_TutTimer_GameOver <= f_TutTimer_GameOver_Minimum)
            {
                f_TutTimer_GameOver += Time.deltaTime;

                if (f_TutTimer_GameOver > f_TutTimer_GameOver_Minimum)
                {
                    // Increment counter
                    ++i_TutCounter_GameOver;

                    // Decrement timer by difference
                    f_TutTimer_GameOver -= f_TutTimer_GameOver_Difference;

                    #region Switch
                    // Set text to print
                    switch (i_TutCounter_GameOver)
                    {
                        case 1:
                            s_TextToPrint = "***BIOS_ACTIVE***\n";
                            break;

                        case 2:
                            s_TextToPrint = "\n";
                            break;

                        case 3:
                            s_TextToPrint = "INIT_STASIS() ";
                            break;

                        case 4:
                            s_TextToPrint = ". ";
                            break;

                        case 5:
                            s_TextToPrint = ". ";
                            break;

                        case 6:
                            s_TextToPrint = ". ";
                            break;

                        case 7:
                            s_TextToPrint = " ";
                            break;

                        case 8:
                            s_TextToPrint = "SUCCESS";
                            break;

                        case 9:
                            s_TextToPrint = "\n";
                            b_CanJump = true;
                            break;

                        case 10:
                            s_TextToPrint = "\nHEV DISABLED...";
                            break;

                        case 13:
                            break;

                        case 11:
                            s_TextToPrint = "";
                            break;

                        case i_Counter_Max:
                            s_TextToPrint = " ";
                            break;

                        default:
                            break;
                    }
                    #endregion

                    // Call Set_HUDTest
                    if (i_TutCounter_GameOver < i_GameOverCounter_Max)
                    {
                        Set_HUDTest(s_TextToPrint);
                    }
                    else
                    {
                        b_TutCompleted_GameOver = true;

                        Set_HUDTest(s_TextToPrint, true);
                    }
                }
            }
        }
    }

    float f_Timer;
    float f_Timer_Minimum = 2f;
    float f_Timer_Difference = 0.5f;
    int i_Counter;
    const int i_Counter_Max = 20;
    bool b_TextCompleted = false;
    void UnlockSuit_Tutorial()
    {
        if(!b_TextCompleted)
        {
            string s_TextToPrint = "";

            if(f_Timer < f_Timer_Minimum)
            {
                f_Timer += Time.deltaTime;

                if(f_Timer > f_Timer_Minimum)
                {
                    // Increment counter
                    ++i_Counter;

                    // Decrement timer by difference
                    f_Timer -= f_Timer_Difference;

                    #region Switch
                    // Set text to print
                    switch (i_Counter)
                    {
                        case 1:
                            s_TextToPrint = "***BIOS_STARTUP***\n";
                            PlayAudioClip(1);
                            break;

                        case 2:
                            s_TextToPrint = "\n";
                            break;

                        case 3:
                            s_TextToPrint = "INIT_LOCK() ";
                            break;

                        case 4:
                            s_TextToPrint = ". ";
                            break;

                        case 5:
                            s_TextToPrint = ". ";
                            break;

                        case 6:
                            s_TextToPrint = ". ";
                            PlayAudioClip(2);
                            break;

                        case 7:
                            s_TextToPrint = " ";
                            break;

                        case 8:
                            s_TextToPrint = "SUCCESS";
                            break;

                        case 9:
                            s_TextToPrint = "\n";
                            break;

                        case 10:
                            s_TextToPrint = "\nWaiting for user input...";
                            Set_TutorialState(Enum_Tutorial.Startup);
                            break;

                        case 11:
                            gameObject.GetComponent<Cs_TextHint>().Set_TextHint("Hold Spacebar to Slide\nUse Mouse to Look");
                            break;

                        case 13:
                            PlayAudioClip(3);
                            break;

                        case i_Counter_Max:
                            s_TextToPrint = " ";
                            break;

                        default:
                            break;
                    }
                    #endregion

                    // Call Set_HUDTest
                    if (i_Counter < i_Counter_Max)
                    {
                        Set_HUDTest(s_TextToPrint);
                    }
                    else
                    {
                        b_TextCompleted = true;

                        Set_HUDTest(s_TextToPrint, true);
                    }
                }
            }
        }
    }

    public void Set_TutorialState(Enum_Tutorial e_Tutorial_, bool b_State_ = true)
    {
        if( e_Tutorial_ == Enum_Tutorial.Jetpack )
        {
            b_JetpackAllowed_Tutorial = b_State_;

            b_UI_Jetpack = b_State_;

            if(b_State_)
            {
                if(gameObject.GetComponent<Cs_TextHint>())
                {
                    gameObject.GetComponent<Cs_TextHint>().Set_TextHint("Press 'Spacebar' to jump and immediately hold 'Right Mouse' to jumpjet\nPress W/A/S/D to push yourself in that direction");
                }
            }
        }
        else if( e_Tutorial_ == Enum_Tutorial.Jump )
        {
            b_JumpUnlocked = b_State_;
        }
        else if( e_Tutorial_ == Enum_Tutorial.Startup)
        {
            b_Startup_Tutorial = b_State_;
        }
    }

    float f_LookSensitivity = 5f;
    float f_lookSmoothDamp = 0.1f;
    float f_yRot;
    float f_yRot_Curr;
    float f_yRot_Vel;
    float f_xRot;
    float f_xRot_Curr;
    float f_xRot_Vel;
    GameObject go_Camera;
    void MouseInput()
    {
        #region Mouse Horizontal

        if(b_LookHorizontalAllowed_Tutorial)
        {
            // Update Mouse State
            f_xRot += Input.GetAxis("Mouse X") * f_LookSensitivity;

            // Smooth it out
            f_xRot_Curr = Mathf.SmoothDamp(f_xRot_Curr, f_xRot, ref f_xRot_Vel, f_lookSmoothDamp);
        }

        #endregion

        #region Mouse Vertical

        // Update Mouse State
        f_yRot += Input.GetAxis("Mouse Y") * f_LookSensitivity;

        // Clamp the angles (Vertical only)
        f_yRot = Mathf.Clamp(f_yRot, -90, 90);

        // Smooth it out
        f_yRot_Curr = Mathf.SmoothDamp(f_yRot_Curr, f_yRot, ref f_yRot_Vel, f_lookSmoothDamp);

        #endregion

        // Apply vertical rotation to Camera
        go_Camera.transform.rotation = Quaternion.Euler(-f_yRot_Curr, f_xRot_Curr, 0);

        // Apply horizontal rotation to gameObject
        gameObject.transform.rotation = Quaternion.Euler(0, f_xRot_Curr, 0);
    }

    float f_Speed = 0.0f;
    float f_MaxRunSpeed = 10f;
    float f_Acceleration = 25f;
    void PlayerInput()
    {
        Vector3 v3_OldVelocity = gameObject.GetComponent<Rigidbody>().velocity;

        // If the player is in the air, do not manipulate their movement velocity
        RaycastHit hit = CheckRaycasts();
        //print(hit.distance);

        if(hit.distance < 2f)
        {
            float f_JumpVelocity = v3_OldVelocity.y;

            Vector3 v3_InputVelocity = new Vector3();

            if(b_LookHorizontalAllowed_Tutorial)
            {
                if(Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
                {
                    v3_InputVelocity.z = 1;
                }
                else if(!Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.S))
                {
                    v3_InputVelocity.z = -1;
                }

                if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
                {
                    v3_InputVelocity.x = -1;
                }
                else if (!Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D))
                {
                    v3_InputVelocity.x = 1;
                }
            }

            v3_InputVelocity.Normalize();
        
            if (Input.GetKey(KeyCode.W) ||
                Input.GetKey(KeyCode.S) ||
                Input.GetKey(KeyCode.A) ||
                Input.GetKey(KeyCode.D))
            {
                f_Speed += Time.deltaTime * f_Acceleration;

                if (f_Speed > f_MaxRunSpeed) f_Speed = f_MaxRunSpeed;
            }
            else
            {
                f_Speed -= Time.deltaTime * f_Acceleration;

                if (f_Speed < 0) f_Speed = 0;
            }

            // Aligning vector to that of the player's rotation
            Vector3 v3_FinalVelocity = Vector3.Lerp(v3_OldVelocity, gameObject.transform.rotation * v3_InputVelocity * f_Speed, 0.1f);

            // Restore y velocity
            v3_FinalVelocity.y = f_JumpVelocity;

            // Project upon a plane
            v3_NewVelocity = Vector3.ProjectOnPlane(v3_NewVelocity, -hit.normal);

            // Set final rotation
            gameObject.GetComponent<Rigidbody>().velocity = v3_FinalVelocity;
        }
        else
        {
            // We're above the ground. Disable the ability to jump.
            b_CanJump = false;
        }
    }

    float f_JumpMagnitude_Curr;
    [SerializeField] float f_MaxJumpMagnitude;
    Vector3 v3_NewVelocity;
    void Jump()
    {
        if(b_CanJump)
        {
            Vector3 v3_CurrVelocity = gameObject.GetComponent<Rigidbody>().velocity;
            v3_CurrVelocity.y = 5;

            gameObject.GetComponent<Rigidbody>().velocity = v3_CurrVelocity;

            b_CanJump = false;

            f_JumpTimer = 0;
        }
    }

    void SetVolume( Enum_SFX e_SFX_, float f_Percent_ )
    {
        // Cap volume percent
        if (f_Percent_ > 1.0f) f_Percent_ = 1.0f;
        else if (f_Percent_ < 0.0f) f_Percent_ = 0.0f;

        if (e_SFX_ == Enum_SFX.Grass)
        {
            f_VolumeGrass = f_Percent_ * f_VolumeGrass_Max;

            // Connect to the AudioSource and set the volume
            sfx_Grass.volume = f_VolumeGrass;
        }
        else if (e_SFX_ == Enum_SFX.Jetpack)
        {
            f_VolumeJetpack = f_Percent_ * f_VolumeJetpack_Max;

            // Connect to the AudioSource and set the volume
            sfx_Jetpack.volume = f_VolumeJetpack;
        }
        else if (e_SFX_ == Enum_SFX.Wind)
        {
            f_VolumeWind = f_Percent_;
            
            // Connect to the AudioSource and set the volume
            sfx_Wind.volume = f_VolumeWind * f_VolumeWind_Max;
        }
    }
    void SetVolume( Enum_SFX e_SFX_, bool b_VolumeUp_ )
    {
        if(e_SFX_ == Enum_SFX.Grass)
        {
            if( b_VolumeUp_ )
            {
                f_VolumeGrass += Time.deltaTime;

                if ( f_VolumeGrass > f_VolumeGrass_Max ) f_VolumeGrass = f_VolumeGrass_Max;
            }
            else
            {
                f_VolumeGrass -= Time.deltaTime;

                if ( f_VolumeGrass < 0 ) f_VolumeGrass = 0.0f;
            }

            // Connect to the AudioSource and set the volume
            sfx_Grass.volume = f_VolumeGrass;
        }
        else if(e_SFX_ == Enum_SFX.Jetpack)
        {
            if (b_VolumeUp_)
            {
                f_VolumeJetpack += Time.deltaTime;

                if (f_VolumeJetpack > f_VolumeJetpack_Max) f_VolumeJetpack = f_VolumeJetpack_Max;
            }
            else
            {
                f_VolumeJetpack -= Time.deltaTime;

                if (f_VolumeJetpack < 0) f_VolumeJetpack = 0.0f;
            }

            // Connect to the AudioSource and set the volume
            sfx_Jetpack.volume = f_VolumeJetpack;
        }
        else if(e_SFX_ == Enum_SFX.Wind)
        {
            if (b_VolumeUp_)
            {
                f_VolumeWind += Time.deltaTime;

                if (f_VolumeWind > f_VolumeWind_Max) f_VolumeWind = f_VolumeWind_Max;
            }
            else
            {
                f_VolumeWind -= Time.deltaTime;

                if (f_VolumeWind < 0) f_VolumeWind = 0.0f;
            }

            // Connect to the AudioSource and set the volume
            sfx_Wind.volume = f_VolumeWind;
        }
    }

    float f_Jetpack_Curr = 0;
    float f_Jetpack_Max = 10f;
    GameObject go_JetpackUI;
    void Jetpack()
    {
        if (b_JetpackAllowed_Tutorial)
        {
            if (Input.GetMouseButton(1))
            {
                if(f_Jetpack_Curr >= 0.1f)
                {
                    // Increase Jetpack volume
                    SetVolume(Enum_SFX.Jetpack, true);

                    f_Jetpack_Curr -= Time.deltaTime * 2;

                    Vector3 v3_CurrVelocity = gameObject.GetComponent<Rigidbody>().velocity;
                    v3_CurrVelocity.y += (f_MaxJumpMagnitude * Time.deltaTime * 2) / 5;

                    if (v3_CurrVelocity.y < 1.5f)
                    {
                        v3_CurrVelocity.y += 1.5f * Time.deltaTime;
                    }

                    if(b_LookHorizontalAllowed_Tutorial)
                    {
                        AirPush();
                    }

                    gameObject.GetComponent<Rigidbody>().velocity = v3_CurrVelocity;
                }
                else
                {
                    // Decrease Jetpack volume
                    SetVolume(Enum_SFX.Jetpack, false);
                }
            }
            else
            {
                // Decrease Jetpack volume
                SetVolume(Enum_SFX.Jetpack, false);

                if (b_JetpackAllowed_Tutorial)
                {
                    f_Jetpack_Curr += Time.deltaTime;

                    if (f_Jetpack_Curr > f_Jetpack_Max) f_Jetpack_Curr = f_Jetpack_Max;
                }
            }
        }

        if(go_JetpackUI.GetComponent<Cs_JetpackHud>())
        {
            go_JetpackUI.GetComponent<Cs_JetpackHud>().Set_HUDPercentage(f_Jetpack_Curr / f_Jetpack_Max);
        }
    }

    bool b_UseGrapple;
    Vector3 v3_GrapplePoint;
    [SerializeField] float f_GrappleDistance = 25f;
    [SerializeField] GameObject go_Reticle;
    void GrappleHook()
    {
        #region Update Reticle
        RaycastHit hit_reticle;
        int i_LayerMask_Reticle = LayerMask.GetMask("Ground", "Grapple");
        if(Physics.Raycast(go_Camera.transform.position, go_Camera.transform.forward, out hit_reticle, f_GrappleDistance, i_LayerMask_Reticle))
        {
            if(hit_reticle.collider.gameObject.layer == LayerMask.NameToLayer("Grapple"))
            {
                go_Reticle.GetComponent<Image>().color = new Color(1, 0, 0);
            }
            else // First thing we *did* hit wasn't a grapple. Set to white.
            {
                go_Reticle.GetComponent<Image>().color = new Color(1, 1, 1);
            }
        }
        else // We hit nothing. Set to white.
        {
            go_Reticle.GetComponent<Image>().color = new Color(1, 1, 1);
        }
        #endregion

        #region Mouse Input
        if (Input.GetMouseButton(0))
        {
            if(!b_UseGrapple)
            {
                int i_LayerMask = LayerMask.GetMask("Ground", "Grapple");

                RaycastHit hit;

                if(Physics.Raycast(go_Camera.transform.position, go_Camera.transform.forward, out hit, f_GrappleDistance, i_LayerMask))
                {
                    if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Grapple"))
                    {
                        // Set b_UseGrapple to true
                        b_UseGrapple = true;

                        // Draw 'grapple rope' to grapple point
                        gameObject.GetComponent<LineRenderer>().enabled = true;
                        gameObject.GetComponent<LineRenderer>().SetPosition(0, gameObject.transform.position + (-gameObject.transform.up * 2));
                        gameObject.GetComponent<LineRenderer>().SetPosition(1, hit.collider.gameObject.transform.position);

                        // Find vector between player and grapple object's point
                        v3_GrapplePoint = hit.collider.gameObject.transform.position;

                        // Set PhysicsMaterial
                        gameObject.GetComponent<Collider>().material = physMat_Walk;
                    }
                }
            }
        }
        else
        {
            b_UseGrapple = false;

            gameObject.GetComponent<LineRenderer>().enabled = false;
        }

        if(b_UseGrapple)
        {
            // Set PhysicsMaterial
            gameObject.GetComponent<Collider>().material = physMat_Walk;

            Vector3 v3_Vector = gameObject.transform.position - v3_GrapplePoint;

            /*v3_Velocity = new Vector3(v3_Velocity.x, 0, v3_Velocity.z);
            v3_Velocity = Vector3.ProjectOnPlane(v3_Velocity, hit.normal);
            v3_Velocity.Normalize();
            v3_Velocity *= f_MaxSpeed;*/


            // Project a plane under the player based on the vector
            Vector3 v3_GrappleVelocity = Vector3.ProjectOnPlane(gameObject.GetComponent<Rigidbody>().velocity, v3_Vector);
            v3_GrappleVelocity.Normalize();
            v3_GrappleVelocity *= f_Speed;
            gameObject.GetComponent<Rigidbody>().velocity = v3_GrappleVelocity;

            // Reset line renderer positions
            gameObject.GetComponent<LineRenderer>().SetPosition(0, gameObject.transform.position + (-gameObject.transform.up * 2));
            gameObject.GetComponent<LineRenderer>().SetPosition(1, v3_GrapplePoint);
        }
        #endregion
    }

    void Ski()
    {
        #region Ski (if in the air)
        // Raycast down and grab the angle of the terrain
        RaycastHit hit = CheckRaycasts();

        // This checks to be sure there is ground below us & it is within a certain distance
        if (hit.distance < 1.1f && (hit.normal != new Vector3()))
        {
            if (v3_Velocity == new Vector3())
            {
                // Set PhysicsMaterial
                gameObject.GetComponent<Collider>().material = physMat_Ski;

                // If f_MaxSpeed > 0 (Why did I do this?!)
                if (!(f_MaxSpeed <= 0))
                {
                    // v3_Velocity = gameObject.GetComponent<Rigidbody>().velocity;
                    v3_Velocity = new Vector3(v3_Velocity.x, 0, v3_Velocity.z);
                    v3_Velocity = Vector3.ProjectOnPlane(v3_Velocity, hit.normal);
                    v3_Velocity.Normalize();
                    v3_Velocity *= f_MaxSpeed;
                }
            }
            else
            {
                v3_Velocity = gameObject.GetComponent<Rigidbody>().velocity;
            }

            // Increase grass skiing volume
            SetVolume( Enum_SFX.Grass, true );

            if (gameObject.GetComponent<Rigidbody>().velocity.magnitude <= v3_Velocity.magnitude)
            {
                gameObject.GetComponent<Rigidbody>().velocity = v3_Velocity;
            }
        }
        else
        {
            // Checks the player's current speed while flying. Soft-caps & reduces their horizontal speed
            if( gameObject.GetComponent<Rigidbody>().velocity.magnitude > 50f)
            {
                float f_yVel = gameObject.GetComponent<Rigidbody>().velocity.y;

                Vector3 v3_CurrVelocity_ = gameObject.GetComponent<Rigidbody>().velocity;
                v3_CurrVelocity_.Normalize();
                v3_CurrVelocity_ *= 50f;
                gameObject.GetComponent<Rigidbody>().velocity = v3_CurrVelocity_;
            }
            else if (gameObject.GetComponent<Rigidbody>().velocity.magnitude > f_MaxSpeed)
            {
                Vector3 v3_CurrVelocity_ = gameObject.GetComponent<Rigidbody>().velocity;
                v3_CurrVelocity_.x *= 0.98f;
                v3_CurrVelocity_.z *= 0.98f;
                gameObject.GetComponent<Rigidbody>().velocity = v3_CurrVelocity_;
            }

            // Decrease grass skiing volume
            SetVolume( Enum_SFX.Grass, false );
        }

        print("Speed: " + gameObject.GetComponent<Rigidbody>().velocity.magnitude);
        #endregion
    }

    void AirPush()
    {
        Vector3 v3_AirPush = new Vector3();
        // Apply basic velocities based on player input
        if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        {
            // Apply left movement
            v3_AirPush.x = -1;
        }
        else if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A))
        {
            // Apply right movement
            v3_AirPush.x = 1;
        }

        if (Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W))
        {
            v3_AirPush.z = -2;
        }
        else if(Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
        {
            v3_AirPush.z = 1.0f;
        }

        if (v3_AirPush != new Vector3())
        {
            v3_AirPush.Normalize();

            Vector3 v3_FinalAirPush = gameObject.transform.rotation * v3_AirPush;

            gameObject.GetComponent<Rigidbody>().AddForce(v3_FinalAirPush * 7.5f);
        }
    }

    RaycastHit CheckRaycasts()
    {
        // outHit is what we'll be sending out from the function
        RaycastHit outHit;

        // tempHit is what we'll compare against
        RaycastHit tempHit;

        int i_LayerMask = LayerMask.GetMask("Ground");

        // Set the default as outHit automatically
        Physics.Raycast(go_RaycastPoint_1.transform.position, -transform.up, out outHit, float.PositiveInfinity, i_LayerMask);

        // Begin comparing against the other three. Find the shortest distance
        if (Physics.Raycast(go_RaycastPoint_2.transform.position, -transform.up, out tempHit, float.PositiveInfinity, i_LayerMask))
        {
            if (tempHit.distance < outHit.distance) outHit = tempHit;
        }

        if (Physics.Raycast(go_RaycastPoint_3.transform.position, -transform.up, out tempHit, float.PositiveInfinity, i_LayerMask))
        {
            if (tempHit.distance < outHit.distance) outHit = tempHit;
        }

        if (Physics.Raycast(go_RaycastPoint_4.transform.position, -transform.up, out tempHit, float.PositiveInfinity, i_LayerMask))
        {
            if (tempHit.distance < outHit.distance) outHit = tempHit;
        }

        // Return the shortest hit distance
        return outHit;
    }

    public void Set_ResetVelocity( GameObject go_TeleportObj_ )
    {
        gameObject.GetComponent<Rigidbody>().velocity = new Vector3();

        gameObject.transform.position = go_TeleportObj_.transform.position;

        f_xRot_Curr = go_TeleportObj_.transform.eulerAngles.y;
        f_xRot = go_TeleportObj_.transform.eulerAngles.y;
        f_yRot_Curr = 0;
        f_yRot = 0;
    }

    void OnCollisionEnter( Collision collision_ )
    {
        if(!b_CanJump)
        {
            int i_LayerMask = LayerMask.GetMask("Ground");

            float f_RaycastDistance = 0.1f;

            GameObject go_RaycastPoint_Jump = GameObject.Find("RaycastPoint_Jump");

            RaycastHit hit;

            Physics.Raycast(go_RaycastPoint_Jump.transform.position, -gameObject.transform.up, out hit, f_RaycastDistance, i_LayerMask);

            if(hit.distance <= f_RaycastDistance)
            {
                if(b_JumpAllowed_Tutorial)
                {
                    // Reset jump capabilities
                    b_CanJump = true;
                }

                f_JumpMagnitude_Curr = f_MaxJumpMagnitude;
            }
        }
    }

    string s_Text;
    void Set_HUDTest( string s_Text_, bool b_Reset_ = false )
    {
        if(b_Reset_)
        {
            s_Text = "";
        }

        s_Text += s_Text_;

        GameObject.Find("HUD_FlavorText").GetComponent<Text>().text = s_Text;
    }

    public void PlayAudioClip( int i_ )
    {
        switch (i_)
        {
            case 1:
                audioSource.PlayOneShot(ac_Intro_1);
                break;

            case 2:
                audioSource.PlayOneShot(ac_SuitLocked_2);
                break;

            case 3:
                audioSource.PlayOneShot(ac_Spacebar_3);
                break;

            case 4:
                audioSource.PlayOneShot(ac_HoldButton_4);
                break;

            case 5:
                audioSource.PlayOneShot(ac_NotBad_5);
                break;

            case 6:
                audioSource.PlayOneShot(ac_SuitReleased_6);
                break;

            case 7:
                audioSource.PlayOneShot(ac_SpaceJumps_7);
                break;

            case 8:
                audioSource.PlayOneShot(ac_HighWall_8);
                break;

            case 9:
                audioSource.PlayOneShot(ac_WindowContinue_9);
                break;

            default:
                // audioSource.PlayOneShot(ac_Intro_1);
                break;
        }
    }

    bool b_GameOver;
    float f_GameOverTimer;
    public void Set_EndGame()
    {
        b_GameOver = true;

        // Take control away from player
        Set_TutorialState(Enum_Tutorial.Jetpack, false);
        Set_TutorialState(Enum_Tutorial.Jump, false);
        Set_TutorialState(Enum_Tutorial.LookHoriz, false);
        Set_TutorialState(Enum_Tutorial.Startup, false);

        // Run HUD text
        UnlockSuit_GameOver();
    }
}
