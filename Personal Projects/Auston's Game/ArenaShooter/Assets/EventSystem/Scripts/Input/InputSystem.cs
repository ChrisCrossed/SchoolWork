using System.Diagnostics;
using UnityEngine;

public enum InputState { Active, Inactive, Triggered, Released }
public enum Conditional { Greater, Less, GreaterOrEqual, LessOrEqual, Equal, NotEqual }
public enum Direction { None, Up, Right, Down, Left }

public delegate void MethodDelegate();
public class InputSystem : MonoBehaviour
{
    private static Vector3 mousePositionLastFrame = Vector3.zero;
    public static Vector3 mousePositionDelta = Vector3.zero;

    public static InputEvent[] inputEvents;

    public static Stopwatch stopWatch = new Stopwatch();

    // Use updates the gamepad input before anything happens
    InputSystem()
    {
        GamePadInput.UpdateStates();
    }

    void Start()
    {
        mousePositionLastFrame = Input.mousePosition;
        inputEvents = GetInputEvents();
        CallStartOnAll();
    }

    private static InputEvent[] GetInputEvents()
    {
        InputEvent[] _inputEvents = Resources.LoadAll<InputEvent>("Events");
        for (int i = 0; i < _inputEvents.Length; i++)
        {
            System.Type _evaluatorType = System.Type.GetType("InputEvents." + _inputEvents[i].codeFriendlyName);
            if (_evaluatorType == null)
            {
                UnityEngine.Debug.Log("InputEvent Type '" + _inputEvents[i].codeFriendlyName + "' Not Found: Try writing changes and restarting the game.", _inputEvents[i]);
                continue;
            }
            _inputEvents[i].subscriptionData = (InputEvents.SubscriptionData)System.Activator.CreateInstance(_evaluatorType);
        }

        return _inputEvents;
    }
    private static void CallStartOnAll()
    {
        foreach (InputEvent _inputEvent in inputEvents)
        {
            switch (_inputEvent.axisType)
            {
                case InputEvent.AxisType.Binary:
                    foreach (BinaryInputMethod _binaryMethod in _inputEvent.binaryInputMethods)
                        _binaryMethod.GameStart();
                    break;

                case InputEvent.AxisType.SingleAxis:
                    foreach (SingleAxisInputMethod _singleAxisMethod in _inputEvent.singleAxisInputMethods)
                        _singleAxisMethod.GameStart();
                    break;

                case InputEvent.AxisType.DualAxis:
                    foreach (DualAxisInputMethod _dualAxisMethod in _inputEvent.dualAxisInputMethods)
                        _dualAxisMethod.GameStart();
                    break;

                default:
                    UnityEngine.Debug.Log("Invalid Axis Type");
                    break;
            }
        }
    }

    // Updates mouse and gamepad information every frame
    void Update()
    {
        if (inputEvents == null)
        {
            UnityEngine.Debug.Log("Scripts have been reloaded and subscriptions have been lost. Try restarting the game or resubscribing.");
            inputEvents = GetInputEvents();
        }

        mousePositionDelta = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0);//Input.mousePosition - mousePositionLastFrame;
        mousePositionLastFrame = Input.mousePosition;

        GamePadInput.UpdateStates();

        for (int i = 0; i < inputEvents.Length; i++)
        {
            inputEvents[i].EvaluateAndSend();
        }
    }

    //Get an input state with pressed/released information from a boolean value
    public static InputState ToInputState(bool activeThisFrame, bool activeLastFrame)
    {
        //If we had the button up last frame and this frame it is down, we just "Triggered" the button
        if (!activeLastFrame && activeThisFrame)
            return InputState.Triggered;

        //If we had the button down last frame and this frame it is up, we just "Released" the button
        else if (activeLastFrame && !activeThisFrame)
            return InputState.Released;

        //Otherwise, if the button is down, the button is "Active"
        else if (activeThisFrame)
            return InputState.Active;

        //Otherwise the button is not in use at all and is "Inactive"
        else
            return InputState.Inactive;
    }
}
