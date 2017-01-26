using UnityEngine;
using System.Collections;
using XInputDotNetPure; // Controller input

public class Cs_MechBaseController : MonoBehaviour
{
    // Player Gamepad Information
    GamePadState state;
    GamePadState prevState;
    public PlayerIndex playerIndex = PlayerIndex.One;

    // Speed & Acceleration
    [SerializeField]
    private float READ_ONLY_CurrSpeed;
    public float f_MaxSpeed;
    public float f_MaxAcceleration;
    public float f_RotationRate;
    float f_CurrSpeed;
    float f_CurrRot;

    // Camera Object
    Camera go_camera;
    GameObject camera_InternalPos;
    GameObject camera_ExternalPos;
    float cameraSpeed = 0.25f;

    // Positional constants
    public float f_YPos = 0.5f;

    // Boolean States
    bool b_IsPaused = false;
    bool b_ToggleStop;
    bool b_DriveMode;

    // UI Information
    GameObject ui_SpeedGuide;
    GameObject ui_Speed_Top;
    GameObject ui_Speed_Bottom;
    GameObject ui_Rotation;
    GameObject ui_Reticle;
    public GameObject go_PauseMenu;

    // Use this for initialization
    void Start ()
    {
        // Tell the Turret what state we're in
        gameObject.GetComponentInChildren<Cs_MechTurretController>().SetTurretState(!b_DriveMode);

        // Set our Y Position
        f_YPos = gameObject.transform.position.y;

        // Camera Information
        go_camera = GameObject.Find("Camera").GetComponent<Camera>();
        camera_InternalPos = transform.FindChild("Camera_InternalPos").gameObject;
        camera_ExternalPos = transform.FindChild("Camera_ExternalPos").gameObject;

        // UI Information
        ui_SpeedGuide = GameObject.Find("Speed_Guide");
        ui_Speed_Top = GameObject.Find("Speed_Top");
        ui_Speed_Bottom = GameObject.Find("Speed_Bot");
        ui_Reticle = GameObject.Find("UI_Reticle");
        ui_Rotation = GameObject.Find("Rotation_Base");
    }

    void LateUpdate()
    {
        SetUIGuidePosition();
    }

    public void EndGame()
    {
        b_ToggleStop = true;
        // state = GamePad.GetState(PlayerIndex.Three);
    }
	
	// Update is called once per frame
	void Update ()
    {
        print(b_IsPaused);

        if(gameObject.GetComponent<Rigidbody>().velocity == Vector3.zero)
        {
            f_CurrSpeed = 0;
        }

        prevState = state;
        state = GamePad.GetState(playerIndex);

        #region Set Y Position
        // Reset the Y Position
        var ResetYPos = gameObject.transform.position;
        ResetYPos.y = f_YPos;
        gameObject.transform.position = ResetYPos;
        #endregion

        if (state.Buttons.Start == ButtonState.Pressed && prevState.Buttons.Start == ButtonState.Released)
        {
            go_PauseMenu.GetComponent<Cs_PauseMenuLogic>().TogglePause();
        }

        if (!b_IsPaused)
        {
            MoveMech();
        }
        else
        {
            if (state.ThumbSticks.Left.Y >= 0.4f && prevState.ThumbSticks.Left.Y < 0.4f) go_PauseMenu.GetComponent<Cs_PauseMenuLogic>().ChangePauseOption(true);
            if (state.ThumbSticks.Left.Y <= -0.4f && prevState.ThumbSticks.Left.Y > -0.4f) go_PauseMenu.GetComponent<Cs_PauseMenuLogic>().ChangePauseOption(false);

            if (state.Buttons.A == ButtonState.Pressed && prevState.Buttons.A == ButtonState.Released) go_PauseMenu.GetComponent<Cs_PauseMenuLogic>().ConfirmPauseOption(true);
            if (state.Buttons.X == ButtonState.Pressed && prevState.Buttons.X == ButtonState.Released) go_PauseMenu.GetComponent<Cs_PauseMenuLogic>().ConfirmPauseOption(false);
        }

        // Set new Camera position
        if (b_DriveMode) // If we're in drive mode, move the camera outside
        {
            // Lerp the camera to the correct position/rotation
            go_camera.transform.position = Vector3.Lerp(go_camera.transform.position, camera_ExternalPos.transform.position, cameraSpeed);
            go_camera.transform.rotation = Quaternion.Lerp(go_camera.transform.rotation, camera_ExternalPos.transform.rotation, cameraSpeed);

            // Rotate the Turret to face forward
        }
        else // Otherwise, we're fighting. Move the camera inside.
        {
            // Lerp the camera to the correct position/rotation
            go_camera.transform.position = Vector3.Lerp(go_camera.transform.position, camera_InternalPos.transform.position, cameraSpeed);

            // Reset the X offset of the Drive Mode camera
            var newRot = go_camera.transform.eulerAngles;
            newRot.x = 0;
            go_camera.transform.rotation = Quaternion.Lerp(go_camera.transform.rotation, Quaternion.Euler(newRot), cameraSpeed);
        }
    }

    void SetUIGuidePosition()
    {
        var speedPos = ui_SpeedGuide.GetComponent<RectTransform>().transform.position;

        float speedMaxPoint = ui_Speed_Top.GetComponent<RectTransform>().transform.position.y;
        float speedMinPoint = ui_Speed_Bottom.GetComponent<RectTransform>().transform.position.y;

        float speedCenterPoint = (speedMaxPoint + speedMinPoint) / 2;
        float speedHalfLength = speedMaxPoint - speedCenterPoint; // Half-Length is far right edge - center point

        float speedCurrYPos = speedCenterPoint + (speedHalfLength * (f_CurrSpeed / f_MaxSpeed));

        // Hard-set top & bot values (prevents icon shaking)
        if (f_CurrSpeed / f_MaxSpeed >= 0.94f) speedCurrYPos = speedMaxPoint;
        if (f_CurrSpeed / f_MaxSpeed <= -0.94f) speedCurrYPos = speedMinPoint;

        speedPos.y = speedCurrYPos;
        ui_SpeedGuide.GetComponent<RectTransform>().transform.position = speedPos;
    }

    public float GetCurrRot()
    {
        return f_CurrRot;
    }

    public void ToggleDriveMode()
    {
        b_DriveMode = !b_DriveMode;

        // If we're in drive mode, the turret is *not* in turret mode.
        gameObject.GetComponentInChildren<Cs_MechTurretController>().SetTurretState(!b_DriveMode);

        // Set the new Max Acceleration amounts
        if (b_DriveMode) f_MaxSpeed *= 3; else f_MaxSpeed /= 3;

        // Set the new Max Rotation Rate amounts
        if (b_DriveMode) f_MaxAcceleration *= 3; else f_MaxAcceleration /= 3;

        // If we're in drive mode, disable specific UI elements
        ui_Rotation.gameObject.SetActive(!b_DriveMode);
        ui_Reticle.gameObject.SetActive(!b_DriveMode);
    }

    void MoveMech()
    {
        // Turn off the Toggle Stop if the player presses an acceleration button
        if (state.Triggers.Left >= 0.8f || state.Triggers.Right >= 0.8f) b_ToggleStop = false;
        if (state.ThumbSticks.Right.Y >= 0.1f || state.ThumbSticks.Right.Y <= -0.1f) b_ToggleStop = false;

        // If the player presses 'X', then coast the Mech to a stop   
        if (state.Buttons.X == ButtonState.Pressed && prevState.Buttons.X == ButtonState.Released) b_ToggleStop = true;

        // If the player presses 'A', then Toggle Drive Mode   
        if (state.Buttons.A == ButtonState.Pressed && prevState.Buttons.A == ButtonState.Released) ToggleDriveMode();

        #region Rotation
        // Rotate the Mech left
        if (state.ThumbSticks.Left.X <= -0.1f)
        {
            // Get the rotation
            f_CurrRot = gameObject.transform.eulerAngles.y;
            if (f_CurrSpeed >= 0) f_CurrRot -= f_RotationRate * Time.deltaTime;
            if (f_CurrSpeed < 0)  f_CurrRot += f_RotationRate * Time.deltaTime;
            gameObject.transform.rotation = Quaternion.Euler(0, f_CurrRot, 0);
        }

        if(state.ThumbSticks.Left.X >= 0.1f)
        {
            // Get the rotation
            f_CurrRot = gameObject.transform.eulerAngles.y;

            // If moving in reverse, turn the other way
            if (f_CurrSpeed >= 0) f_CurrRot += f_RotationRate * Time.deltaTime;
            if (f_CurrSpeed < 0)  f_CurrRot -= f_RotationRate * Time.deltaTime;

            gameObject.transform.rotation = Quaternion.Euler(0, f_CurrRot, 0);
        }
        #endregion

        // Normal movement
        if (!b_ToggleStop)
        {
            #region Acceleration & Deacceleration

            #region Trigger Acceleration
            // If the right trigger is held, increase the speed of the mech
            if (state.Triggers.Right >= 0.8f && state.Triggers.Left <= 0.8f)
            {
                f_CurrSpeed += f_MaxAcceleration * Time.deltaTime;
            }

            // If the left trigger is held, decrease the speed of the mech
            if (state.Triggers.Left >= 0.8f && state.Triggers.Right <= 0.8f)
            {
                f_CurrSpeed -= f_MaxAcceleration * Time.deltaTime;
            }
            #endregion

            #region Thumbstick Acceleration
            // Accelerate
            if (state.ThumbSticks.Right.Y >= 0.1f) f_CurrSpeed += (f_MaxAcceleration * Time.deltaTime * state.ThumbSticks.Right.Y);

            // Deaccelerate
            if (state.ThumbSticks.Right.Y <= -0.1f) f_CurrSpeed -= (f_MaxAcceleration * Time.deltaTime * -state.ThumbSticks.Right.Y);
            #endregion

            // Cap the speed of the player. Considers switching in/out of Drive Mode 
            if (f_CurrSpeed >= f_MaxSpeed)  f_CurrSpeed -= f_MaxAcceleration * 3 * Time.deltaTime;
            if (f_CurrSpeed <= -f_MaxSpeed) f_CurrSpeed += f_MaxAcceleration * 3 * Time.deltaTime;

            // Set the speed of the Mech
            gameObject.GetComponent<Rigidbody>().velocity = transform.forward * f_CurrSpeed;
            #endregion
        }
        else // Toggle Stop
        {
            #region
            // If the current speed is greater than 0.5f, reduce the speed;
            if (f_CurrSpeed >= 0.25f) f_CurrSpeed -= f_MaxAcceleration * Time.deltaTime;
            if(f_CurrSpeed <= -0.25f) f_CurrSpeed += f_MaxAcceleration * Time.deltaTime;

            // Cap the speed of the player based on switching in/out of Drive Mode 
            if (f_CurrSpeed >= f_MaxSpeed) f_CurrSpeed -= f_MaxAcceleration * 3 * Time.deltaTime;
            if (f_CurrSpeed <= -f_MaxSpeed) f_CurrSpeed += f_MaxAcceleration * 3 * Time.deltaTime;

            // If we're within -0.25 to 0.25, just stop the mech.
            if (f_CurrSpeed >= -0.25f && f_CurrSpeed <= 0.25f) f_CurrSpeed = 0;

            if(gameObject.GetComponent<Rigidbody>().velocity.magnitude >= -0.1f && gameObject.GetComponent<Rigidbody>().velocity.magnitude <= 0.1f)
            {
                f_CurrSpeed = 0;
            }

            gameObject.GetComponent<Rigidbody>().velocity = transform.forward * f_CurrSpeed;
            #endregion Toggle Stop
        }
    }

    public void SetPausedState(bool b_IsPaused_)
    {
        b_IsPaused = b_IsPaused_;
    }
}
