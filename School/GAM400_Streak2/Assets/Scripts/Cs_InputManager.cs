using UnityEngine;
using System.Collections;
using XInputDotNetPure;
using UnityEngine.SceneManagement;

/*********************************
 * 
 * Copyright DigiPen Institute of Technology 2016
 * 
 * Streak 2 by Christopher Christensen
 * 
 * *******************************/

public class Cs_InputManager : MonoBehaviour
{
    Cs_BoardLogic BoardLogic;
    Cs_BoardLogic_Tutorial BoardLogic_Tut;

    // Keyboard Input
    public KeyCode key_Down;
    public KeyCode key_Down_2;
    public KeyCode key_Left;
    public KeyCode key_Left_2;
    public KeyCode key_Right;
    public KeyCode key_Right_2;
    public KeyCode key_RotClock;
    public KeyCode key_RotCounter;
    public KeyCode key_Drop;
    public KeyCode key_Quit;
    float f_KeyTimer;
    float f_KeyTimer_TimeToReactivate = 0.5f;
    float f_KeyTimer_TimeToReduce = 0.1f;

    // Controller Input
    PlayerIndex pad_PlayerOne = PlayerIndex.One;
    GamePadState state_p1;
    GamePadState prevState_p1;

    bool b_KeyboardUsedLast;

    // Use this for initialization
    void Start ()
    {
        if(GameObject.Find("BoardLogic").GetComponent<Cs_BoardLogic>())
        {
            BoardLogic = GameObject.Find("BoardLogic").GetComponent<Cs_BoardLogic>();
        }
        else if(GameObject.Find("BoardLogic").GetComponent<Cs_BoardLogic_Tutorial>())
        {
            BoardLogic_Tut = GameObject.Find("BoardLogic").GetComponent<Cs_BoardLogic_Tutorial>();
        }
	}

    bool KeyboardInputCheck()
    {
        f_ControllerInputTimer += Time.deltaTime;

        if ( Input.GetKey(key_Down) ||
            Input.GetKey(key_Left) ||
            Input.GetKey(key_Right) ||
            Input.GetKey(key_Down_2) ||
            Input.GetKey(key_Left_2) ||
            Input.GetKey(key_Right_2) ||
            Input.GetKey(key_RotClock) ||
            Input.GetKey(key_RotCounter) ||
            Input.GetKey(key_Drop) ||
            Input.GetKey(key_Quit))
        {
            return true;
        }

        return false;
    }

    void KeyboardInput()
    {
        if(Input.GetKey(key_Quit)) SceneManager.LoadScene(2);

        #region Left Input
        if ( (Input.GetKeyDown(key_Left) || Input.GetKeyDown(key_Left_2)) &&
           !(Input.GetKey(key_Right) || Input.GetKey(key_Right_2)) )
        {
            f_KeyTimer = 0.0f;

            if (BoardLogic) BoardLogic.Input_MoveLeft(); else if (BoardLogic_Tut) BoardLogic_Tut.Input_MoveLeft();
        }
        else if(Input.GetKeyUp(key_Left) || Input.GetKeyUp(key_Left_2))
        {
            f_KeyTimer = 0.0f;
        }

        if( (Input.GetKey(key_Left) || Input.GetKey(key_Left_2)) &&
           !(Input.GetKey(key_Right) || Input.GetKey(key_Right_2)) )
        {
            f_KeyTimer += Time.deltaTime;

            if(f_KeyTimer >= f_KeyTimer_TimeToReactivate)
            {
                if (BoardLogic) BoardLogic.Input_MoveLeft(); else if (BoardLogic_Tut) BoardLogic_Tut.Input_MoveLeft();

                f_KeyTimer -= f_KeyTimer_TimeToReduce;
            }
        }
        #endregion

        #region Right Input
        if ((Input.GetKeyDown(key_Right) || Input.GetKeyDown(key_Right_2)) &&
            !(Input.GetKey(key_Left) || Input.GetKey(key_Left_2)) )
        {
            f_KeyTimer = 0.0f;
            
            if (BoardLogic) BoardLogic.Input_MoveRight(); else if (BoardLogic_Tut) BoardLogic_Tut.Input_MoveRight();
        }
        else if (Input.GetKeyUp(key_Right) || Input.GetKeyUp(key_Right_2))
        {
            f_KeyTimer = 0.0f;
        }

        if ( (Input.GetKey(key_Right) || Input.GetKey(key_Right_2)) &&
            !(Input.GetKey(key_Left) || Input.GetKey(key_Left_2)) )
        {
            f_KeyTimer += Time.deltaTime;

            if (f_KeyTimer >= f_KeyTimer_TimeToReactivate)
            {
                if (BoardLogic) BoardLogic.Input_MoveRight(); else if (BoardLogic_Tut) BoardLogic_Tut.Input_MoveRight();

                f_KeyTimer -= f_KeyTimer_TimeToReduce;
            }
        }
        #endregion

        #region Down Input
        if(Input.GetKeyDown(key_Down) || Input.GetKeyDown(key_Down_2))
        {
            if (BoardLogic) BoardLogic.Input_MoveDown(); else if (BoardLogic_Tut) BoardLogic_Tut.Input_MoveDown();
        }
        #endregion

        #region Drop Input
        if(Input.GetKeyDown(key_Drop))
        {
            if (BoardLogic) BoardLogic.Input_Drop(); else if (BoardLogic_Tut) BoardLogic_Tut.Input_Drop();
        }
        #endregion

        #region Clockwise
        if(Input.GetKeyDown(key_RotClock))
        {
            if (BoardLogic) BoardLogic.Input_RotateClockwise(); else if (BoardLogic_Tut) BoardLogic_Tut.Input_RotateClockwise();
        }
        #endregion

        #region CounterClockwise
        if(Input.GetKeyDown(key_RotCounter))
        {
            if (BoardLogic) BoardLogic.Input_RotateCounterclock(); else if (BoardLogic_Tut) BoardLogic_Tut.Input_RotateCounterclock();
        }
        #endregion
    }

    float f_ControllerInputTimer = 0.0f;
    void ControllerInput()
    {
        if(state_p1.Buttons.Start == ButtonState.Pressed && prevState_p1.Buttons.Start == ButtonState.Released)
        {
            SceneManager.LoadScene(2);
        }

        #region Move Left
        if ((state_p1.DPad.Left == ButtonState.Pressed && prevState_p1.DPad.Left == ButtonState.Released) || // DPad Left was Pressed
            (state_p1.ThumbSticks.Left.X < -0.5f && prevState_p1.ThumbSticks.Left.X > -0.5f))                // Analog Left was Pressed
        {
            f_KeyTimer = 0.0f;

            if (BoardLogic) BoardLogic.Input_MoveLeft(); else if (BoardLogic_Tut) BoardLogic_Tut.Input_MoveLeft();

            VibrateController( true );
        }
        else if ((state_p1.DPad.Left == ButtonState.Released && prevState_p1.DPad.Left == ButtonState.Pressed) || // DPad Left was Released
                 (state_p1.ThumbSticks.Left.X > -0.5f && prevState_p1.ThumbSticks.Left.X < -0.5f))                // Analog Left was Released
        {
            f_KeyTimer = 0.0f;
        }

        if ((state_p1.DPad.Left == ButtonState.Pressed && prevState_p1.DPad.Left == ButtonState.Pressed) ||       // DPad Left was Held
            (state_p1.ThumbSticks.Left.X < -0.5f && prevState_p1.ThumbSticks.Left.X < -0.5f))                     // Analog Left was Held
        {
            f_KeyTimer += Time.deltaTime;

            if (f_KeyTimer >= f_KeyTimer_TimeToReactivate)
            {
                if (BoardLogic) BoardLogic.Input_MoveLeft(); else if (BoardLogic_Tut) BoardLogic_Tut.Input_MoveLeft();

                VibrateController(true);

                f_KeyTimer -= f_KeyTimer_TimeToReduce;
            }
        }
        #endregion

        #region Move Right
        if ((state_p1.DPad.Right == ButtonState.Pressed && prevState_p1.DPad.Right == ButtonState.Released) ||  // DPad Right was Pressed
            (state_p1.ThumbSticks.Left.X > 0.5f && prevState_p1.ThumbSticks.Left.X < 0.5f))                     // Analog Left was Pressed
        {
            f_KeyTimer = 0.0f;

            if (BoardLogic) BoardLogic.Input_MoveRight(); else if (BoardLogic_Tut) BoardLogic_Tut.Input_MoveRight();

            VibrateController(true);
        }
        else if ((state_p1.DPad.Right == ButtonState.Released && prevState_p1.DPad.Right == ButtonState.Pressed) || // DPad Right was Released
                 (state_p1.ThumbSticks.Left.X < 0.5f && prevState_p1.ThumbSticks.Left.X > 0.5f))                    // Analog Right was Released
        {
            f_KeyTimer = 0.0f;
        }

        if ((state_p1.DPad.Right == ButtonState.Pressed && prevState_p1.DPad.Right == ButtonState.Pressed) ||   // DPad Right was Held
            (state_p1.ThumbSticks.Left.X > 0.5f && prevState_p1.ThumbSticks.Left.X > 0.5f))                     // Analog Right was Held
        {
            f_KeyTimer += Time.deltaTime;

            if (f_KeyTimer >= f_KeyTimer_TimeToReactivate)
            {
                if (BoardLogic) BoardLogic.Input_MoveRight(); else if (BoardLogic_Tut) BoardLogic_Tut.Input_MoveRight();

                VibrateController(true);

                f_KeyTimer -= f_KeyTimer_TimeToReduce;
            }
        }
        #endregion

        #region Down Input
        if( (state_p1.ThumbSticks.Right.Y < -0.5f && prevState_p1.ThumbSticks.Right.Y > -0.5f) ||           // Right Analog Down Pressed
            (state_p1.ThumbSticks.Left.Y < -0.5f && prevState_p1.ThumbSticks.Left.Y > -0.5f) ||             // Left Analog Down Pressed
            (state_p1.DPad.Down == ButtonState.Pressed && prevState_p1.DPad.Down == ButtonState.Released) ) // DPad Down Pressed
        {
            if (BoardLogic) BoardLogic.Input_MoveDown(); else if (BoardLogic_Tut) BoardLogic_Tut.Input_MoveDown();

            VibrateController(true);
        }
        #endregion

        #region Drop Input
        if (state_p1.Buttons.A == ButtonState.Pressed && prevState_p1.Buttons.A == ButtonState.Released)      // Button 'A' Pressed
        {
            if (BoardLogic) BoardLogic.Input_Drop(); else if (BoardLogic_Tut) BoardLogic_Tut.Input_Drop();

            VibrateController(true);
        }
        #endregion

        #region Clockwise
        if (state_p1.DPad.Up == ButtonState.Pressed && prevState_p1.DPad.Up == ButtonState.Released ||                              // DPad Up Pressed
            state_p1.Buttons.Y == ButtonState.Pressed && prevState_p1.Buttons.Y == ButtonState.Released ||                          // 'Y' Button Pressed
            state_p1.Buttons.B == ButtonState.Pressed && prevState_p1.Buttons.B == ButtonState.Released ||                          // 'B' Button Pressed
            state_p1.Buttons.RightShoulder == ButtonState.Pressed && prevState_p1.Buttons.RightShoulder == ButtonState.Released ||  // Right Shoulder Pressed
            state_p1.Triggers.Right > 0.4f && prevState_p1.Triggers.Right < 0.4f ||                                                 // Right Trigger Pressed
            state_p1.ThumbSticks.Right.X < -0.5f && prevState_p1.ThumbSticks.Right.X > -0.5f)                                       // Right Thumbstick Pressed
        {
            // Ensures multiple presses are not performed
            if(f_ControllerInputTimer > 0.05f)
            {
                if (BoardLogic) BoardLogic.Input_RotateClockwise(); else if (BoardLogic_Tut) BoardLogic_Tut.Input_RotateClockwise();

                VibrateController(true);

                f_ControllerInputTimer = 0.0f;
            }
        }
        #endregion

        #region Counter-Clockwise
        if (state_p1.Buttons.X == ButtonState.Pressed && prevState_p1.Buttons.X == ButtonState.Released ||                          // 'X' Button Pressed
            state_p1.Buttons.LeftShoulder == ButtonState.Pressed && prevState_p1.Buttons.LeftShoulder == ButtonState.Released ||    // Left Shoulder Pressed
            state_p1.Triggers.Left > 0.4f && prevState_p1.Triggers.Left < 0.4f ||                                                   // Left Trigger Pressed
            state_p1.ThumbSticks.Right.X > 0.5f && prevState_p1.ThumbSticks.Right.X < 0.5f)                                         // Right Analog Pushed Right
        {
            // Ensures multiple presses are not performed
            if (f_ControllerInputTimer > 0.05f)
            {
                if (BoardLogic) BoardLogic.Input_RotateCounterclock(); else if (BoardLogic_Tut) BoardLogic_Tut.Input_RotateCounterclock();

                VibrateController(true);

                f_ControllerInputTimer = 0.0f;
            }
        }
        #endregion
    }

    float f_VibrateTimer;
    void VibrateController(bool b_Reset_ = false)
    {
        /*
        if(b_Reset_)
        {
            f_VibrateTimer = 0.15f;
        }
        
        if(f_VibrateTimer > 0.0f)
        {
            f_VibrateTimer -= Time.deltaTime;

            GamePad.SetVibration(pad_PlayerOne, 0.5f, 0.5f);
        }
        else
        {
            GamePad.SetVibration(pad_PlayerOne, 0.0f, 0.0f);
        }
        */
    }

    // Update is called once per frame
    void Update ()
    {
        prevState_p1 = state_p1;
        state_p1 = GamePad.GetState(pad_PlayerOne);

        if(KeyboardInputCheck())
        {
            KeyboardInput();
        }
        else
        {
            ControllerInput();
        }

        VibrateController();
    }
}
