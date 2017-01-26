using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;
using UnityEngine.UI;

public struct PlayerInput
{
    // Game Settings
    private float _f_zDir; // Forward or Backward
    private float _f_xDir; // Strafe Left or Right
    private float _f_MouseHoriz;
    private float _f_MouseVert;
    private bool _b_JumpPressed;
    private bool _b_JumpHeld;

    public float zDir
    {
        set
        {
            float f_Temp = value;

            // Cap the value between -1 and 1
            if (f_Temp < -1.0f) f_Temp = -1.0f;
            if (f_Temp > 1.0f) f_Temp = 1.0f;

            // If very close to 0f, set to 0f
            if (f_Temp < .05f && f_Temp > -0.05f) f_Temp = 0f;

            _f_zDir = f_Temp;
        }
        get
        {
            return _f_zDir;
        }
    }
    public float xDir
    {
        set
        {
            float f_Temp = value;

            // Cap the value between -1 and 1
            if (f_Temp < -1.0f) f_Temp = -1.0f;
            if (f_Temp > 1.0f) f_Temp = 1.0f;

            // If very close to 0f, set to 0f
            if (f_Temp < .05f && f_Temp > -0.05f) f_Temp = 0f;

            _f_xDir = f_Temp;
        }
        get
        {
            return _f_xDir;
        }
    }

    public float mouseHoriz
    {
        set
        {
            _f_MouseHoriz = value;
        }
        get { return _f_MouseHoriz; }
    }
    public float mouseVert
    {
        set
        {
            _f_MouseVert = value;
        }
        get { return _f_MouseVert; }
    }
    
    public bool JumpPressed
    {
        set { _b_JumpPressed = value; }
        get { return _b_JumpPressed; }
    }
    public bool JumpHeld
    {
        set { _b_JumpHeld = value; }
        get { return _b_JumpHeld; }
    }
}

public enum KeyCodeChoices
{
    Forward,
    Backward,
    StrafeLeft,
    StrafeRight,
    Jump,
    Sprint,
    Crouch,
    Use,
    WeaponOne,
    WeaponTwo,
    Grenade,
    Reload,
    Reset
}

public class Cs_InputManager : MonoBehaviour
{
    // Input Manager
    internal PlayerInput playerInput;

    // Scripts
    Cs_PlayerController_Infantry PlayerCont_Infantry;

    // Controller scripts
    GamePadState p1_State;
    GamePadState p1_PrevState;
    PlayerIndex p1 = PlayerIndex.One;

    // Keyboard Button Presets
    KeyCode kc_Forward;
    KeyCode kc_Backward;
    KeyCode kc_StrafeLeft;
    KeyCode kc_StrafeRight;
    KeyCode kc_Jump;
    KeyCode kc_Sprint;
    KeyCode kc_Crouch;
    KeyCode kc_Use;
    KeyCode kc_WeaponOne;
    KeyCode kc_WeaponTwo;
    KeyCode kc_Grenade;
    KeyCode kc_Reload;

    // Twitch Keyboard Viewing
    Image img_But_W;
    Image img_But_A;
    Image img_But_S;
    Image img_But_D;

    // Use this for initialization
    void Start()
    {
        PlayerCont_Infantry = gameObject.GetComponent<Cs_PlayerController_Infantry>();

        // Initialization
        playerInput = new PlayerInput();
        Init_ResetControls();

        PlayerCont_Infantry.Initialize();

        // Twitch Keyboard Viewing
        img_But_W = GameObject.Find("But_W").GetComponent<Image>();
        img_But_A = GameObject.Find("But_A").GetComponent<Image>();
        img_But_S = GameObject.Find("But_S").GetComponent<Image>();
        img_But_D = GameObject.Find("But_D").GetComponent<Image>();
    }

    #region Controls Initialization
    protected void Init_InputControls( KeyCodeChoices kc_Choice_ )
    {
        // Cardinal movement
        if (kc_Choice_ == KeyCodeChoices.Forward) Init_InputControls(kc_Choice_, KeyCode.W);
        else if (kc_Choice_ == KeyCodeChoices.Backward) Init_InputControls(kc_Choice_, KeyCode.S);
        else if (kc_Choice_ == KeyCodeChoices.StrafeLeft) Init_InputControls(kc_Choice_, KeyCode.A);
        else if (kc_Choice_ == KeyCodeChoices.StrafeRight) Init_InputControls(kc_Choice_, KeyCode.D);

        // Additional Movement
        else if (kc_Choice_ == KeyCodeChoices.Jump) Init_InputControls(kc_Choice_, KeyCode.Space);
        else if (kc_Choice_ == KeyCodeChoices.Sprint) Init_InputControls(kc_Choice_, KeyCode.LeftShift);
        else if (kc_Choice_ == KeyCodeChoices.Crouch) Init_InputControls(kc_Choice_, KeyCode.LeftControl);
        else if (kc_Choice_ == KeyCodeChoices.Use) Init_InputControls(kc_Choice_, KeyCode.E);

        // Weapons
        else if (kc_Choice_ == KeyCodeChoices.WeaponOne) Init_InputControls(kc_Choice_, KeyCode.Alpha1);
        else if (kc_Choice_ == KeyCodeChoices.WeaponTwo) Init_InputControls(kc_Choice_, KeyCode.Alpha2);
        else if (kc_Choice_ == KeyCodeChoices.Grenade) Init_InputControls(kc_Choice_, KeyCode.F);
        else if (kc_Choice_ == KeyCodeChoices.Reload) Init_InputControls(kc_Choice_, KeyCode.R );
    }
    protected void Init_InputControls( KeyCodeChoices kc_Choice_, KeyCode kc_Button_ )
    {
        // Cardinal movement
        if (kc_Choice_ == KeyCodeChoices.Forward) kc_Forward = kc_Button_;
        else if (kc_Choice_ == KeyCodeChoices.Backward) kc_Backward = kc_Button_;
        else if (kc_Choice_ == KeyCodeChoices.StrafeLeft) kc_StrafeLeft = kc_Button_;
        else if (kc_Choice_ == KeyCodeChoices.StrafeRight) kc_StrafeRight = kc_Button_;

        // Additional Movement
        else if (kc_Choice_ == KeyCodeChoices.Jump) kc_Jump = kc_Button_;
        else if (kc_Choice_ == KeyCodeChoices.Sprint) kc_Sprint = kc_Button_;
        else if (kc_Choice_ == KeyCodeChoices.Crouch) kc_Crouch = kc_Button_;
        else if (kc_Choice_ == KeyCodeChoices.Use) kc_Use = kc_Button_;

        // Weapons
        else if (kc_Choice_ == KeyCodeChoices.WeaponOne) kc_WeaponOne = kc_Button_;
        else if (kc_Choice_ == KeyCodeChoices.WeaponTwo) kc_WeaponTwo = kc_Button_;
        else if (kc_Choice_ == KeyCodeChoices.Grenade) kc_Grenade = kc_Button_;
        else if (kc_Choice_ == KeyCodeChoices.Reload) kc_Reload = kc_Button_;
    }
    protected void Init_ResetControls()
    {
        // Cardinal Movement
        Init_InputControls(KeyCodeChoices.Forward);
        Init_InputControls(KeyCodeChoices.Backward);
        Init_InputControls(KeyCodeChoices.StrafeLeft);
        Init_InputControls(KeyCodeChoices.StrafeRight);

        // Additional Movement
        Init_InputControls(KeyCodeChoices.Jump);
        Init_InputControls(KeyCodeChoices.Sprint);
        Init_InputControls(KeyCodeChoices.Crouch);
        Init_InputControls(KeyCodeChoices.Use);

        // Weapons
        Init_InputControls(KeyCodeChoices.WeaponOne);
        Init_InputControls(KeyCodeChoices.WeaponTwo);
        Init_InputControls(KeyCodeChoices.Grenade);
        Init_InputControls(KeyCodeChoices.Reload);
    }
    #endregion

    protected void ControllerInput()
    {
        p1_PrevState = p1_State;
        p1_State = GamePad.GetState(p1);

        #region Left Analog Stick
        playerInput.xDir = p1_State.ThumbSticks.Left.X;
        playerInput.zDir = p1_State.ThumbSticks.Left.Y;
        #endregion

        #region Right Analog Stick
        playerInput.mouseHoriz = p1_State.ThumbSticks.Right.X;
        playerInput.mouseVert = -p1_State.ThumbSticks.Right.Y;
        #endregion

        #region Jumping
        if (p1_State.Buttons.A == ButtonState.Pressed && p1_PrevState.Buttons.A == ButtonState.Released) playerInput.JumpPressed = true;
        else if (p1_PrevState.Buttons.A == ButtonState.Pressed) playerInput.JumpHeld = true;
        #endregion
    }

    float f_yRot;
    float f_xRot;
    protected bool KeyboardCheck()
    {
        #region Keyboard Input
        if ( Input.GetKey(kc_Forward) ||
             Input.GetKey(kc_Backward) ||
             Input.GetKey(kc_StrafeLeft) ||
             Input.GetKey(kc_StrafeRight) ||
             Input.GetKey(kc_Jump) ||
             Input.GetKey(kc_Sprint) ||
             Input.GetKey(kc_Crouch) ||
             Input.GetKey(kc_Use) ||
             Input.GetKey(kc_WeaponOne) ||
             Input.GetKey(kc_WeaponTwo) ||
             Input.GetKey(kc_Grenade) ||
             Input.GetKey(kc_Reload))
        {
            return true;
        }
        #endregion

        #region Mouse Input
        float f_yRot_Prev = f_yRot;
        float f_xRot_Prev = f_xRot;

        f_yRot += Input.GetAxis("Mouse Y");
        f_xRot += Input.GetAxis("Mouse X");

        if ((f_xRot_Prev != f_xRot) || (f_yRot_Prev != f_yRot)) return true;

        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) return true;
        #endregion

        return false;
    }
    protected void KeyboardInput()
    {
        #region Forward & Backward
        if (Input.GetKey(kc_Forward) && !Input.GetKey(kc_Backward))
        {
            print("Set");
            playerInput.zDir = 1.0f;
        }
        else if(Input.GetKey(kc_Backward) && !Input.GetKey(kc_Forward))
        {
            playerInput.zDir = -1.0f;
        }
        #endregion

        #region Strafing
        if(Input.GetKey(kc_StrafeLeft) && !Input.GetKey(kc_StrafeRight))
        {
            playerInput.xDir = -1.0f;
        }
        else if(Input.GetKey(kc_StrafeRight) && !Input.GetKey(kc_StrafeLeft))
        {
            playerInput.xDir = 1.0f;
        }
        #endregion

        #region Mouse Input
        playerInput.mouseHoriz = Input.GetAxis("Mouse X");
        playerInput.mouseVert = Input.GetAxis("Mouse Y");
        #endregion

        #region Jumping
        playerInput.JumpPressed = Input.GetKeyDown(kc_Jump);
        playerInput.JumpHeld = Input.GetKey(kc_Jump);
        #endregion
    }

    internal void InputUpdate()
    {
        // Reset input values
        playerInput.xDir = 0;
        playerInput.zDir = 0;
        playerInput.mouseHoriz = 0f;
        playerInput.mouseVert = 0f;

        // If the player has used the keyboard or mouse this frame, switch to Keyboard input. Otherwise, Controller.
        if (KeyboardCheck()) KeyboardInput(); else ControllerInput();

        // Twitch Keyboard viewing
        if (playerInput.zDir > 0) img_But_W.color = Color.grey; else img_But_W.color = Color.white;
        if (playerInput.zDir < 0) img_But_S.color = Color.grey; else img_But_S.color = Color.white;
        if (playerInput.xDir < 0) img_But_A.color = Color.grey; else img_But_A.color = Color.white;
        if (playerInput.xDir > 0) img_But_D.color = Color.grey; else img_But_D.color = Color.white;
    }
    
}
