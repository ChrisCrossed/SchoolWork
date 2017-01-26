using UnityEngine;
using System.Collections;
using XInputDotNetPure; // Controller input

public class Cs_MechTurretController : MonoBehaviour
{
    // Player Gamepad Information
    GamePadState state;
    GamePadState prevState;
    public PlayerIndex playerIndex = PlayerIndex.Two;
    GameObject go_PauseMenu;

    // Rotation information
    float f_CurrRot = 0f;
    public float f_MaxRotation;
    public float f_RotationRate;
    GameObject mechBase;

    // UI Information
    GameObject ui_Reticle;
    GameObject ui_Camera;
    GameObject ui_PlayerGuide; // Green center guide UI object
    GameObject ui_Guide_Left; // Left side Red UI object
    GameObject ui_Guide_Right; // Right side Red UI object

    // Turret Game Objects
    GameObject go_TurretHinge_Left;
    GameObject go_TurretHinge_Right;
    GameObject go_LaserBeam_Left;
    GameObject go_LaserBeam_Right;
    GameObject go_laserHinge_Left;
    GameObject go_laserHinge_Right;

    // Shield Game Object
    GameObject go_Shield;

    // Fire Timers
    bool b_FireLeftGun = true;
    float f_FireTimer;
    public float f_TimeToFire = 0.5f;

    // Turret Enabled State
    bool b_IsTurretActive;
    bool b_ResetTurretRotation;

    // Game Paused State
    bool b_IsPaused = false;

    // SFX
    public AudioClip sfx_FireLaser;
    public AudioClip sfx_LaserHit;
    public AudioSource audioSource;

    // Use this for initialization
    void Start ()
    {
        // SFX
        audioSource = GetComponent<AudioSource>();

        // Set the Mech
        mechBase = GameObject.Find("Mech");

        // Set the UI Information
        ui_Reticle = GameObject.Find("UI_Reticle");
        ui_Camera = GameObject.Find("Camera");
        ui_PlayerGuide = GameObject.Find("Guide");
        ui_Guide_Left = GameObject.Find("Rotation_Left");
        ui_Guide_Right = GameObject.Find("Rotation_Right");
        go_PauseMenu = GameObject.Find("PauseMenu");

        // Set the TurretJoint Objects
        go_TurretHinge_Left = GameObject.Find("TurretJoint_Left");
        go_TurretHinge_Right = GameObject.Find("TurretJoint_Right");
        go_LaserBeam_Left = GameObject.Find("TurretLaser_Left");
        go_LaserBeam_Right = GameObject.Find("TurretLaser_Right");
        go_laserHinge_Left = GameObject.Find("LaserJoint_Left");
        go_laserHinge_Right = GameObject.Find("LaserJoint_Right");

        // Set the Shield Object
        go_Shield = GameObject.Find("Shield_Object");

        // Max out the shoot timer so they can fire right away
        f_FireTimer = f_TimeToFire;

        audioSource.Play();
    }
	
	// Update is called once per frame
	void Update ()
    {
        // Update the player controller
        prevState = state;
        state = GamePad.GetState(playerIndex);

        SetUIGuidePosition();

        // If the player presses Start, pause the game
        if (state.Buttons.Start == ButtonState.Pressed && prevState.Buttons.Start == ButtonState.Released)
        {
            GameObject.Find("PauseMenu").GetComponent<Cs_PauseMenuLogic>().TogglePause();
        }

        if (!b_IsPaused)
        {
            #region Turret Active
            if (b_IsTurretActive)
            {
                go_Shield.SetActive(false);

                // If the player presses 'X' or 'A', reset the rotation until they move the stick
                if (state.Buttons.X == ButtonState.Pressed && prevState.Buttons.X == ButtonState.Released)
                {
                    b_ResetTurretRotation = true;
                }

                UpdateMechBaseRotation();

                // Increment fire timer
                if(f_FireTimer < f_TimeToFire) f_FireTimer += Time.deltaTime;

                if (state.Triggers.Right >= 0.5f)
                {
                    if(f_FireTimer >= f_TimeToFire)
                    {
                        FireLasers();
                    }
                }

                // Reset the lasers size
                // Turns off both lasers after 30% of the fire rate has elapsed.
                if (f_FireTimer >= (f_TimeToFire * 0.3f))
                {
                    // Reset position
                    go_LaserBeam_Left.transform.position = go_TurretHinge_Left.transform.position;
                    go_LaserBeam_Right.transform.position = go_TurretHinge_Right.transform.position;

                    // Turn off the laser
                    go_LaserBeam_Left.GetComponent<MeshRenderer>().enabled = false;
                    go_LaserBeam_Right.GetComponent<MeshRenderer>().enabled = false;

                    // Change the laser scale
                    var newScale = go_LaserBeam_Left.transform.localScale;
                    newScale = new Vector3(0.5f, 0.5f, 0.5f);
                    go_LaserBeam_Left.transform.localScale = newScale;

                    newScale = go_LaserBeam_Right.transform.localScale;
                    newScale = new Vector3(0.5f, 0.5f, 0.5f);
                    go_LaserBeam_Right.transform.localScale = newScale;
                }
            }
            else
            {
                ResetMechRotation();

                go_Shield.SetActive(true);

                SetShieldRotation();
            }
            #endregion
        }
        else
        {
            if (state.ThumbSticks.Left.Y >= 0.4f && prevState.ThumbSticks.Left.Y < 0.4f) go_PauseMenu.GetComponent<Cs_PauseMenuLogic>().ChangePauseOption(true);
            if (state.ThumbSticks.Left.Y <= -0.4f && prevState.ThumbSticks.Left.Y > -0.4f) go_PauseMenu.GetComponent<Cs_PauseMenuLogic>().ChangePauseOption(false);

            if (state.Buttons.A == ButtonState.Pressed && prevState.Buttons.A == ButtonState.Released) go_PauseMenu.GetComponent<Cs_PauseMenuLogic>().ConfirmPauseOption(true);
            if (state.Buttons.X == ButtonState.Pressed && prevState.Buttons.X == ButtonState.Released) go_PauseMenu.GetComponent<Cs_PauseMenuLogic>().ConfirmPauseOption(false);
        }
    }

    void SetShieldRotation()
    {
        if((state.ThumbSticks.Right.X >= -0.3f &&
            state.ThumbSticks.Right.X <= 0.3f &&
            state.ThumbSticks.Right.Y >= -0.3f &&
            state.ThumbSticks.Right.Y <= 0.3f))
        {
            go_Shield.SetActive(false);
        }
        else
        {
            go_Shield.SetActive(true);

            float rotation = Mathf.Atan2(state.ThumbSticks.Right.Y, state.ThumbSticks.Right.X) * 180 / Mathf.PI;

            // Subtract 90 degrees to convert 'right' into 'forward'
            rotation -= 90;

            // Multiply by -1 due to maths
            rotation *= -1;

            // Set the new degree to match that of the mech base
            rotation += mechBase.GetComponent<Cs_MechBaseController>().GetCurrRot();

            go_Shield.transform.eulerAngles = new Vector3(0, rotation, 0);

            // var currRot = gameObject.transform.eulerAngles;
        }

    }

    void FireLasers()
    {
        Ray ray = ui_Camera.GetComponent<Camera>().ScreenPointToRay(ui_Reticle.GetComponent<RectTransform>().transform.position);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit) && hit.transform.tag == "Wall")
        {
            // Play Laser SFX
            audioSource.PlayOneShot(sfx_FireLaser, 0.4f);
            // audioSource.PlayOneShot(sfx_LaserHit, 0.4f);
            audioSource.PlayDelayed(0.1f);

            if (b_FireLeftGun)
            {
                // Enable the visualization
                go_LaserBeam_Left.GetComponent<MeshRenderer>().enabled = true;

                // Midpoint formula (Mid point between the hit point and the laser hinge)
                go_LaserBeam_Left.transform.position = new Vector3((hit.point.x + go_laserHinge_Left.transform.position.x) / 2, (hit.point.y + go_laserHinge_Left.transform.position.y) / 2, (hit.point.z + go_laserHinge_Left.transform.position.z) / 2);

                // Change the laser scale
                var newScale = go_LaserBeam_Left.transform.lossyScale;
                // Reset scale
                newScale.x = .09f;
                // Distance between the joint and the hit position
                float sqrX = (hit.point.x - go_laserHinge_Left.transform.position.x) * (hit.point.x - go_laserHinge_Left.transform.position.x);
                float sqrY = (hit.point.y - go_laserHinge_Left.transform.position.y) * (hit.point.y - go_laserHinge_Left.transform.position.y);
                float sqrZ = (hit.point.z - go_laserHinge_Left.transform.position.z) * (hit.point.z - go_laserHinge_Left.transform.position.z);
                newScale.y = Mathf.Sqrt(sqrX + sqrY + sqrZ) / 2;
                newScale.z = .09f;

                // Apply the scale
                go_LaserBeam_Left.transform.localScale = newScale;

                // Set rotation
                go_LaserBeam_Left.transform.rotation = go_laserHinge_Left.transform.rotation;

                // Switch guns
                b_FireLeftGun = false;
            }
            else
            {
                go_LaserBeam_Right.GetComponent<MeshRenderer>().enabled = true;

                // Midpoint formula
                go_LaserBeam_Right.transform.position = (hit.point + go_laserHinge_Right.transform.position) / 2;

                // Change the laser scale
                var newScale = go_LaserBeam_Right.transform.lossyScale;
                // Reset scale
                newScale.x = .09f;
                // Distance between the joint and the hit position
                float sqrX = (hit.point.x - go_laserHinge_Right.transform.position.x) * (hit.point.x - go_laserHinge_Right.transform.position.x);
                float sqrY = (hit.point.y - go_laserHinge_Right.transform.position.y) * (hit.point.y - go_laserHinge_Right.transform.position.y);
                float sqrZ = (hit.point.z - go_laserHinge_Right.transform.position.z) * (hit.point.z - go_laserHinge_Right.transform.position.z);
                newScale.y = Mathf.Sqrt(sqrX + sqrY + sqrZ) / 2; // No idea why I need to divide by 2, but it's correct in-game
                newScale.z = .09f;

                // Apply the scale
                go_LaserBeam_Right.transform.localScale = newScale;

                // Set rotation
                go_LaserBeam_Right.transform.rotation = go_laserHinge_Right.transform.rotation;

                // Switch guns
                b_FireLeftGun = true;
            }

            // Reset the Laser Timer
            f_FireTimer = 0;
        }
    }

    public void UpdateMechBaseRotation()
    {
        // Receive the mech base rotation, Add to my rotation.
        float baseRot = mechBase.GetComponent<Cs_MechBaseController>().GetCurrRot();

        ControllerInputRotation();

        if (b_ResetTurretRotation) ResetMechRotation();

        // Rotation input
        var currRot = gameObject.transform.eulerAngles;
        currRot.y = baseRot + f_CurrRot;
        gameObject.transform.eulerAngles = currRot;

        // Set the camera rotation to match the turret rotation
        // ui_Camera.transform.rotation = gameObject.transform.rotation; // Quaternion.Lerp(camera.transform.rotation, camera_InternalPos.transform.rotation, cameraSpeed);
    }

    void ResetMechRotation()
    {
        float baseRot = mechBase.GetComponent<Cs_MechBaseController>().GetCurrRot();

        // Receive the mech base rotation, Add to my rotation.
        if (f_CurrRot < -0.51f) f_CurrRot += Time.deltaTime * f_RotationRate;
        if (f_CurrRot > 0.51f) f_CurrRot -= Time.deltaTime * f_RotationRate;
        if (f_CurrRot > -0.51f && f_CurrRot < 0.51f) f_CurrRot = 0;

        var currRot = gameObject.transform.eulerAngles;
        currRot.y = baseRot + f_CurrRot;
        gameObject.transform.eulerAngles = currRot;
    }

    void ControllerInputRotation()
    {
        // Rotate Mech Turret Left/Right
        if (state.ThumbSticks.Left.X <= -0.05f && f_CurrRot > -f_MaxRotation)
        {
            f_CurrRot -= Time.deltaTime * 15;
            b_ResetTurretRotation = false;
        }
        if (state.ThumbSticks.Left.X >= 0.05f && f_CurrRot < f_MaxRotation)
        {
            f_CurrRot += Time.deltaTime * 15;
            b_ResetTurretRotation = false;
        }

        SetReticlePosition();
    }

    void SetReticlePosition()
    {
        // Update Aiming Reticle
        var pos = ui_Reticle.GetComponent<RectTransform>().transform.position;

        // Set reticle X pos based on Right Thumbstick (Math works out to 63% of the screen is within the UI)
        float halfScreenWidth = (Screen.width * .63f) / 2f; // * .815f
        pos.x = (halfScreenWidth + (state.ThumbSticks.Right.X * halfScreenWidth)) + (Screen.width * (.37f / 2));

        // Set reticle Y pos based on Right Thumbstick
        float halfScreenHeight = (Screen.height * .69f) / 2;
        pos.y = (halfScreenHeight + (state.ThumbSticks.Right.Y * halfScreenHeight)) + (Screen.height * (.31f / 2));

        // Reposition the reticle
        ui_Reticle.GetComponent<RectTransform>().transform.position = pos;

        AimTurrets();
    }

    void SetUIGuidePosition()
    {
        var pos = ui_PlayerGuide.GetComponent<RectTransform>().transform.position;

        float maxPoint = ui_Guide_Right.GetComponent<RectTransform>().transform.position.x;
        float minPoint = ui_Guide_Left.GetComponent<RectTransform>().transform.position.x;
        
        float centerPoint = (maxPoint + minPoint) / 2;
        float halfLength = maxPoint - centerPoint; // Half-Length is far right edge - center point

        float currXPos = centerPoint + (halfLength * (f_CurrRot / f_MaxRotation));
        
        pos.x = currXPos;
        ui_PlayerGuide.GetComponent<RectTransform>().transform.position = pos;
    }

    void AimTurrets()
    {
        // Raycast through the UI to the point in the world
        Ray ray = ui_Camera.GetComponent<Camera>().ScreenPointToRay(ui_Reticle.GetComponent<RectTransform>().transform.position);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Debug.DrawRay(ray.origin, ray.direction, Color.red);

            // go_Laser_Left.transform.LookAt(hit.point, go_Laser_Left.gameObject.transform.parent.InverseTransformDirection(go_Laser_Left.transform.up));
            go_TurretHinge_Left.transform.LookAt(hit.point, go_TurretHinge_Left.gameObject.transform.InverseTransformDirection(go_TurretHinge_Left.transform.up));
            go_TurretHinge_Right.transform.LookAt(hit.point, go_TurretHinge_Right.gameObject.transform.InverseTransformDirection(go_TurretHinge_Right.transform.up));
        }
    }

    public void SetTurretState(bool b_IsTurretActive_)
    {
        b_IsTurretActive = b_IsTurretActive_;

        // Hard-Set the shield to be turned off if the turret is active
        // if (b_IsTurretActive) go_Shield.SetActive(false);
    }

    public void SetPausedState(bool b_IsPaused_)
    {
        b_IsPaused = b_IsPaused_;
    }
}
