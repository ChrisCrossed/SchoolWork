using UnityEngine;
using XInputDotNetPure;

public static class GamePadInput
{
    public enum Button { None, A, B, Back, Guide, LeftShoulder, LeftStick, RightShoulder, RightStick, Start, X, Y, DpadUp, DpadDown, DpadLeft, DpadRight }
    public enum Axis { None, LeftTrigger, RightTrigger, LeftStickUp, LeftStickDown, LeftStickLeft, LeftStickRight, RightStickUp, RightStickDown, RightStickLeft, RightStickRight }
    public enum Sticks { None, Left, Right }

    //REGULAR BUTTONS
    private static ButtonState[][] GamePadButtonStatesThisFrame = { new ButtonState[15], new ButtonState[15], new ButtonState[15], new ButtonState[15] };
    private static ButtonState[][] GamePadButtonStatesLastFrame = { new ButtonState[15], new ButtonState[15], new ButtonState[15], new ButtonState[15] };

    //AXES
    private static float[][] GamePadAxisStatesThisFrame = { new float[10], new float[10], new float[10], new float[10] };
    private static float[][] GamePadAxisStatesLastFrame = { new float[10], new float[10], new float[10], new float[10] };

    //GAMEPAD STATES
    private static GamePadState[] States = new GamePadState[4];
    private static GamePadState[] StatesLastFrame = new GamePadState[4];

    // Updates states for all gamepads
    public static void UpdateStates()
    {
        //Loop through each player index and update all necessary info
        for (int i = 0; i < 4; ++i)
        {
            PlayerIndex currentPlayerIndex = (PlayerIndex)i;

            StatesLastFrame[i] = States[i];
            States[i] = GamePad.GetState(currentPlayerIndex);

            StoreButtonStates(i);
            StoreAxisStates(i);
        }
    }

    //All ways to check an INPUT STATE for a REGULAR BUTTON
    public static bool GetInputTriggered(int gamePadIndex, Button button)
    {
        if (button == Button.None) return false;
        return GamePadInput.GetInputState(gamePadIndex, button) == InputState.Triggered;
    }
    public static bool GetInputReleased(int gamePadIndex, Button button)
    {
        if (button == Button.None) return false;
        return GamePadInput.GetInputState(gamePadIndex, button) == InputState.Released;
    }
    public static bool GetInputInactive(int gamePadIndex, Button button)
    {
        if (button == Button.None) return false;
        return GamePadInput.GetInputState(gamePadIndex, button) == InputState.Inactive;
    }
    public static bool GetInputActive(int gamePadIndex, Button button)
    {
        if (button == Button.None) return false;
        return GetInputState(gamePadIndex, button) == InputState.Active;
    }

    //All ways to check an INPUT STATE for an AXIS
    public static bool GetInputTriggered(int gamePadIndex, Axis axis, Conditional condition, float value)
    {
        if (axis == Axis.None) return false;
        return GamePadInput.GetInputState(gamePadIndex, axis, condition, value) == InputState.Triggered;
    }
    public static bool GetInputReleased(int gamePadIndex, Axis axis, Conditional condition, float value)
    {
        if (axis == Axis.None) return false;
        return GamePadInput.GetInputState(gamePadIndex, axis, condition, value) == InputState.Released;
    }
    public static bool GetInputInactive(int gamePadIndex, Axis axis, Conditional condition, float value)
    {
        if (axis == Axis.None) return false;
        return GamePadInput.GetInputState(gamePadIndex, axis, condition, value) == InputState.Inactive;
    }
    public static bool GetInputActive(int gamePadIndex, Axis axis, Conditional condition, float value)
    {
        if (axis == Axis.None) return false;
        return GamePadInput.GetInputState(gamePadIndex, axis, condition, value) == InputState.Active;
    }

    //Gets an INPUT STATE from a REGULAR BUTTON
    public static InputState GetInputState(int gamePadIndex, Button button)
    {
        if (button == Button.None) return InputState.Inactive;

        ButtonState buttonStateThisFrame = GamePadButtonStatesThisFrame[gamePadIndex][(int)button - 1];
        ButtonState buttonStateLastFrame = GamePadButtonStatesLastFrame[gamePadIndex][(int)button - 1];

        return GamePadInput.ToInputState(buttonStateThisFrame, buttonStateLastFrame);
    }

    //Gets an AXIS value as an INPUT STATE by passing a CONDITION and a VALUE
    public static InputState GetInputState(int gamePadIndex, Axis axis, Conditional condition, float value)//Stick
    {
        if (axis == Axis.None) return InputState.Inactive;

        float axisStateThisFrame = GamePadAxisStatesThisFrame[gamePadIndex][(int)axis - 1];
        float axisStateLastFrame = GamePadAxisStatesLastFrame[gamePadIndex][(int)axis - 1];

        bool axisConditionMetThisFrame = EvaluateFloatCondition(axisStateThisFrame, condition, value);
        bool axisConditionMetLastFrame = EvaluateFloatCondition(axisStateLastFrame, condition, value);

        return ToInputState(axisConditionMetThisFrame, axisConditionMetLastFrame);
    }

    //Gets an AXIS value as a FLOAT (between 0 and 1)
    public static float GetInputValue(int gamePadIndex, Axis axis)
    {
        if (axis == Axis.None) return 0;
        return GamePadInput.GamePadAxisStatesThisFrame[gamePadIndex][(int)axis - 1];
    }

    //Gets an AXIS value as a VECTOR2 (from -1 to 1 on both the X and Y axes)
    public static Vector2 GetInputValue(int gamePadIndex, Sticks stick)
    {
        if(stick == Sticks.Left)
        {
            float axisY = GamePadAxisStatesThisFrame[gamePadIndex][2] + GamePadAxisStatesThisFrame[gamePadIndex][3];
            float axisX = GamePadAxisStatesThisFrame[gamePadIndex][4] + GamePadAxisStatesThisFrame[gamePadIndex][5];
            return new Vector2(axisX, axisY);
        }
        else if(stick == Sticks.Right)
        {
            float axisY = GamePadAxisStatesThisFrame[gamePadIndex][6] + GamePadAxisStatesThisFrame[gamePadIndex][7];
            float axisX = GamePadAxisStatesThisFrame[gamePadIndex][8] + GamePadAxisStatesThisFrame[gamePadIndex][9];
            return new Vector2(axisX, axisY);
        }
        else
        {
            return Vector2.zero;
        }
    }

    //Evaluate a float condition using a conditional enum
    private static bool EvaluateFloatCondition(float leftValue, Conditional condition, float rightValue)
    {
        switch (condition)
        {
            case Conditional.Equal:
                return leftValue == rightValue;
            case Conditional.NotEqual:
                return leftValue != rightValue;
            case Conditional.Greater:
                return leftValue > rightValue;
            case Conditional.Less:
                return leftValue < rightValue;
            case Conditional.GreaterOrEqual:
                return leftValue >= rightValue;
            case Conditional.LessOrEqual:
                return leftValue <= rightValue;
        }

        return false;
    }

    //Get an input state with pressed/released information from a regular Button state
    private static InputState ToInputState(ButtonState stateThisFrame, ButtonState stateLastFrame)
    {
        //If we had the button up last frame and this frame it is down, we just "Pressed" the button
        if (stateLastFrame == ButtonState.Released && stateThisFrame == ButtonState.Pressed)
            return InputState.Triggered;

        //If we had the button down last frame and this frame it is up, we just "Released" the button
        else if (stateLastFrame == ButtonState.Pressed && stateThisFrame == ButtonState.Released)
            return InputState.Released;

        //Otherwise, if the normal button state is "Pressed", the button is up
        else if (stateThisFrame == ButtonState.Pressed)
            return InputState.Active;

        //Otherwise the button is not in use at all
        else
            return InputState.Inactive;
    }

    //Get an input state with pressed/released information from a boolean value
    private static InputState ToInputState(bool stateThisFrame, bool stateLastFrame)
    {
        //If we had the button up last frame and this frame it is down, we just "Pressed" the button
        if (!stateLastFrame && stateThisFrame)
            return InputState.Triggered;

        //If we had the button down last frame and this frame it is up, we just "Released" the button
        else if (stateLastFrame && !stateThisFrame)
            return InputState.Released;

        //Otherwise, if the normal button state is "Pressed", the button is active
        else if (stateThisFrame)
            return InputState.Active;

        //Otherwise the button is not in use at all
        else
            return InputState.Inactive;
    }

    /*************************************************************************************************************/
    /*       Below are functions which store values for various types of buttons and axes on the gamepad         */

    private static void StoreButtonStates(int gamePadIndex)
    {
        GamePadInput.GamePadButtonStatesThisFrame[gamePadIndex][0] = GamePadInput.States[gamePadIndex].Buttons.A;
        GamePadInput.GamePadButtonStatesThisFrame[gamePadIndex][1] = GamePadInput.States[gamePadIndex].Buttons.B;
        GamePadInput.GamePadButtonStatesThisFrame[gamePadIndex][2] = GamePadInput.States[gamePadIndex].Buttons.Back;
        GamePadInput.GamePadButtonStatesThisFrame[gamePadIndex][3] = GamePadInput.States[gamePadIndex].Buttons.Guide;
        GamePadInput.GamePadButtonStatesThisFrame[gamePadIndex][4] = GamePadInput.States[gamePadIndex].Buttons.LeftShoulder;
        GamePadInput.GamePadButtonStatesThisFrame[gamePadIndex][5] = GamePadInput.States[gamePadIndex].Buttons.LeftStick;
        GamePadInput.GamePadButtonStatesThisFrame[gamePadIndex][6] = GamePadInput.States[gamePadIndex].Buttons.RightShoulder;
        GamePadInput.GamePadButtonStatesThisFrame[gamePadIndex][7] = GamePadInput.States[gamePadIndex].Buttons.RightStick;
        GamePadInput.GamePadButtonStatesThisFrame[gamePadIndex][8] = GamePadInput.States[gamePadIndex].Buttons.Start;
        GamePadInput.GamePadButtonStatesThisFrame[gamePadIndex][9] = GamePadInput.States[gamePadIndex].Buttons.X;
        GamePadInput.GamePadButtonStatesThisFrame[gamePadIndex][10] = GamePadInput.States[gamePadIndex].Buttons.Y;
        GamePadInput.GamePadButtonStatesThisFrame[gamePadIndex][11] = GamePadInput.States[gamePadIndex].DPad.Up;
        GamePadInput.GamePadButtonStatesThisFrame[gamePadIndex][12] = GamePadInput.States[gamePadIndex].DPad.Down;
        GamePadInput.GamePadButtonStatesThisFrame[gamePadIndex][13] = GamePadInput.States[gamePadIndex].DPad.Left;
        GamePadInput.GamePadButtonStatesThisFrame[gamePadIndex][14] = GamePadInput.States[gamePadIndex].DPad.Right;

        //--------------------------------------------------------- Begin Last Frame -----------------------------------------------------------//

        GamePadInput.GamePadButtonStatesLastFrame[gamePadIndex][0] = GamePadInput.StatesLastFrame[gamePadIndex].Buttons.A;
        GamePadInput.GamePadButtonStatesLastFrame[gamePadIndex][1] = GamePadInput.StatesLastFrame[gamePadIndex].Buttons.B;
        GamePadInput.GamePadButtonStatesLastFrame[gamePadIndex][2] = GamePadInput.StatesLastFrame[gamePadIndex].Buttons.Back;
        GamePadInput.GamePadButtonStatesLastFrame[gamePadIndex][3] = GamePadInput.StatesLastFrame[gamePadIndex].Buttons.Guide;
        GamePadInput.GamePadButtonStatesLastFrame[gamePadIndex][4] = GamePadInput.StatesLastFrame[gamePadIndex].Buttons.LeftShoulder;
        GamePadInput.GamePadButtonStatesLastFrame[gamePadIndex][5] = GamePadInput.StatesLastFrame[gamePadIndex].Buttons.LeftStick;
        GamePadInput.GamePadButtonStatesLastFrame[gamePadIndex][6] = GamePadInput.StatesLastFrame[gamePadIndex].Buttons.RightShoulder;
        GamePadInput.GamePadButtonStatesLastFrame[gamePadIndex][7] = GamePadInput.StatesLastFrame[gamePadIndex].Buttons.RightStick;
        GamePadInput.GamePadButtonStatesLastFrame[gamePadIndex][8] = GamePadInput.StatesLastFrame[gamePadIndex].Buttons.Start;
        GamePadInput.GamePadButtonStatesLastFrame[gamePadIndex][9] = GamePadInput.StatesLastFrame[gamePadIndex].Buttons.X;
        GamePadInput.GamePadButtonStatesLastFrame[gamePadIndex][10] = GamePadInput.StatesLastFrame[gamePadIndex].Buttons.Y;
        GamePadInput.GamePadButtonStatesLastFrame[gamePadIndex][11] = GamePadInput.StatesLastFrame[gamePadIndex].DPad.Up;
        GamePadInput.GamePadButtonStatesLastFrame[gamePadIndex][12] = GamePadInput.StatesLastFrame[gamePadIndex].DPad.Down;
        GamePadInput.GamePadButtonStatesLastFrame[gamePadIndex][13] = GamePadInput.StatesLastFrame[gamePadIndex].DPad.Left;
        GamePadInput.GamePadButtonStatesLastFrame[gamePadIndex][14] = GamePadInput.StatesLastFrame[gamePadIndex].DPad.Right;
    }
    private static void StoreAxisStates(int gamePadIndex)
    {
        GamePadInput.GamePadAxisStatesThisFrame[gamePadIndex][0] = GamePadInput.States[gamePadIndex].Triggers.Left;
        GamePadInput.GamePadAxisStatesThisFrame[gamePadIndex][1] = GamePadInput.States[gamePadIndex].Triggers.Right;

        GamePadInput.GamePadAxisStatesThisFrame[gamePadIndex][2] = Mathf.Clamp01(GamePadInput.States[gamePadIndex].ThumbSticks.Left.Y); //up
        GamePadInput.GamePadAxisStatesThisFrame[gamePadIndex][3] = Mathf.Clamp01(-GamePadInput.States[gamePadIndex].ThumbSticks.Left.Y); //down
        GamePadInput.GamePadAxisStatesThisFrame[gamePadIndex][4] = Mathf.Clamp01(-GamePadInput.States[gamePadIndex].ThumbSticks.Left.X); //left
        GamePadInput.GamePadAxisStatesThisFrame[gamePadIndex][5] = Mathf.Clamp01(GamePadInput.States[gamePadIndex].ThumbSticks.Left.X); //right

        GamePadInput.GamePadAxisStatesThisFrame[gamePadIndex][6] = Mathf.Clamp01(GamePadInput.States[gamePadIndex].ThumbSticks.Right.Y); //up
        GamePadInput.GamePadAxisStatesThisFrame[gamePadIndex][7] = Mathf.Clamp01(-GamePadInput.States[gamePadIndex].ThumbSticks.Right.Y); //down
        GamePadInput.GamePadAxisStatesThisFrame[gamePadIndex][8] = Mathf.Clamp01(-GamePadInput.States[gamePadIndex].ThumbSticks.Right.X); //left
        GamePadInput.GamePadAxisStatesThisFrame[gamePadIndex][9] = Mathf.Clamp01(GamePadInput.States[gamePadIndex].ThumbSticks.Right.X); //right

        //--------------------------------------------------------- Begin Last Frame -----------------------------------------------------------//

        GamePadInput.GamePadAxisStatesLastFrame[gamePadIndex][0] = GamePadInput.StatesLastFrame[gamePadIndex].Triggers.Left;
        GamePadInput.GamePadAxisStatesLastFrame[gamePadIndex][1] = GamePadInput.StatesLastFrame[gamePadIndex].Triggers.Right;

        GamePadInput.GamePadAxisStatesLastFrame[gamePadIndex][2] = Mathf.Clamp01(GamePadInput.StatesLastFrame[gamePadIndex].ThumbSticks.Left.Y); //up
        GamePadInput.GamePadAxisStatesLastFrame[gamePadIndex][3] = Mathf.Clamp01(-GamePadInput.StatesLastFrame[gamePadIndex].ThumbSticks.Left.Y); //down
        GamePadInput.GamePadAxisStatesLastFrame[gamePadIndex][4] = Mathf.Clamp01(-GamePadInput.StatesLastFrame[gamePadIndex].ThumbSticks.Left.X); //left
        GamePadInput.GamePadAxisStatesLastFrame[gamePadIndex][5] = Mathf.Clamp01(GamePadInput.StatesLastFrame[gamePadIndex].ThumbSticks.Left.X); //right

        GamePadInput.GamePadAxisStatesLastFrame[gamePadIndex][6] = Mathf.Clamp01(GamePadInput.StatesLastFrame[gamePadIndex].ThumbSticks.Right.Y); //up
        GamePadInput.GamePadAxisStatesLastFrame[gamePadIndex][7] = Mathf.Clamp01(-GamePadInput.StatesLastFrame[gamePadIndex].ThumbSticks.Right.Y); //down
        GamePadInput.GamePadAxisStatesLastFrame[gamePadIndex][8] = Mathf.Clamp01(-GamePadInput.StatesLastFrame[gamePadIndex].ThumbSticks.Right.X); //left
        GamePadInput.GamePadAxisStatesLastFrame[gamePadIndex][9] = Mathf.Clamp01(GamePadInput.StatesLastFrame[gamePadIndex].ThumbSticks.Right.X); //right
    }
}