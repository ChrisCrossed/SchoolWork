using UnityEngine;
using System.Collections.Generic;
using InputEvents;

[System.Serializable] public class InputMethod
{
    public string name;
    private bool editingName_;
    public bool editingName { get { return editingName_; } set { if (value == false) controlSet = false; editingName_ = value; } }
    public bool controlSet = false;
    public enum InputType { None, Key, MouseButton, GamepadButton, GamepadTrigger, GamepadStick, MouseScrollDelta, MouseMove };
    [SerializeField] private InputType inputType_ = InputType.None;
    public InputType inputType
    {
        get { return inputType_; }
        set
        {
            if (inputType_ != value)
            {
                InputTypeChanged(value);
                inputType_ = value;
            }
        }
    }

    public virtual void InputTypeChanged(InputType _newInputType)
    {
        //implemented in children
    }

    public List<bool> sendOnChannels;

    public bool expanded = true;

    [System.NonSerialized] public float lastTriggeredTimestamp;
    [System.NonSerialized] public float lastReleasedTimestamp;

    [System.NonSerialized] public float[] gamepadTriggeredTime;
    [System.NonSerialized] public float[] gamepadReleasedTime;

    [System.NonSerialized] public bool activeLastFrame = false;
    [System.NonSerialized] public int timesTriggeredSinceRelease = 0;
    [System.NonSerialized] public float timeActive = 0;
    [System.NonSerialized] public float timeInactive = 0;

    [System.NonSerialized] public bool[] gamepadChannelWasActive;
    [System.NonSerialized] public int[] gamepadChannelTimesTriggered;
    [System.NonSerialized] public float[] gamepadChannelTimeActive;
    [System.NonSerialized] public float[] gamepadChannelTimeInactive;

    public float repeatDelay { get { return parentEvent.repeatDelay; } }

    public InputEvent parentEvent = null;

    public void GameStart()
    {
        if(inputType == InputType.GamepadButton || inputType == InputType.GamepadTrigger || inputType == InputType.GamepadStick)
        {
            gamepadChannelWasActive = new bool[sendOnChannels.Count];
            gamepadChannelTimesTriggered = new int[sendOnChannels.Count];
            gamepadChannelTimeActive = new float[sendOnChannels.Count];
            gamepadChannelTimeInactive = new float[sendOnChannels.Count];
            
            gamepadTriggeredTime = new float[sendOnChannels.Count];
            gamepadReleasedTime = new float[sendOnChannels.Count];
        }
    }

    public virtual void ResetAll()
    {
        //implemented in children
    }
    public virtual bool Evaluate(int _playerIndex = 0)
    {
        //implemented in children
        return false;
    }
    public virtual void Send(InputState _state, SubscriptionData _subscriptionData, int _gamepad = -1)
    {
        //implemented in children
    }
}
[System.Serializable] public class BinaryInputMethod : InputMethod
{
    //only one of the following are used (more than one should never be set)
    public KeyCode keyCode = KeyCode.None;
    public GamePadInput.Button gamePadButton = GamePadInput.Button.None;
    public GamePadInput.Axis gamePadAxis = GamePadInput.Axis.None;
    public Direction mouseDirection = Direction.None; //for the mouse delta or mouse position
    public float threshold = 0; //the value that must be met before this binaryInput evaluates to true

    public override void InputTypeChanged(InputType _newInputType)
    {
        sendOnChannels = new List<bool>(parentEvent.channelCount);

        if (_newInputType == InputType.GamepadButton || _newInputType == InputType.GamepadStick || _newInputType == InputType.GamepadTrigger)
            for (int i = 0; i < parentEvent.channelCount; i++)
                sendOnChannels.Add(true);
        else
            for (int i = 0; i < parentEvent.channelCount; i++)
                sendOnChannels.Add(false);
    }

    public override void ResetAll()
    {
        keyCode = KeyCode.None;
        gamePadButton = GamePadInput.Button.None;
        gamePadAxis = GamePadInput.Axis.None;
        mouseDirection = Direction.None;

        threshold = 0;
    }
    public override bool Evaluate(int _playerIndex = 0)
    {
        switch (inputType)
        {
            case InputType.Key:
            case InputType.MouseButton:
                return Input.GetKey(keyCode);

            case InputType.GamepadButton:
                return GamePadInput.GetInputActive(_playerIndex, gamePadButton);

            case InputType.GamepadTrigger:
            case InputType.GamepadStick:
                return GamePadInput.GetInputActive(_playerIndex, gamePadAxis, Conditional.Greater, threshold);

            case InputType.MouseScrollDelta:
                switch (mouseDirection)
                {
                    case Direction.Up:
                        return Input.mouseScrollDelta.y > threshold;
                    case Direction.Down:
                        return -Input.mouseScrollDelta.y > threshold;
                    default:
                        return false;
                }

            case InputType.MouseMove:
                switch (mouseDirection)
                {
                    case Direction.Up:
                        return InputSystem.mousePositionDelta.y > threshold;
                    case Direction.Down:
                        return -InputSystem.mousePositionDelta.y > threshold;
                    case Direction.Left:
                        return -InputSystem.mousePositionDelta.x > threshold;
                    case Direction.Right:
                        return InputSystem.mousePositionDelta.x > threshold;
                    default:
                        return false;
                }

            default:
                return false;
        }
    }
    public override void Send(InputState _state, SubscriptionData _subscriptionData, int _gamepad = -1)
    {
        InputEventInfo _inputEventInfo = new InputEventInfo();
        _inputEventInfo.axisType = InputEvent.AxisType.Binary;
        _inputEventInfo.inputMethodName = name;
        _inputEventInfo.inputType = inputType;

        _inputEventInfo.gamepad = _gamepad;
        _inputEventInfo.inputState = _state;
        if(_gamepad == -1)
        {
            _inputEventInfo.timeHeld = timeActive;
            _inputEventInfo.timeReleased = timeInactive;
        }
        else
        {
            _inputEventInfo.timeHeld = gamepadChannelTimeActive[_gamepad];
            _inputEventInfo.timeReleased = gamepadChannelTimeInactive[_gamepad];
        }

        if (_gamepad == -1) //send to default and all
        {
            _subscriptionData.Send(_inputEventInfo);
            for (int i = 0; i < sendOnChannels.Count; i++)
            {
                if (sendOnChannels[i])
                    _subscriptionData.Send(_inputEventInfo, i);
            }
        }
        else //send to default and channel (based on the gamepad)
        {
            _subscriptionData.Send(_inputEventInfo);
            _subscriptionData.Send(_inputEventInfo, _gamepad);
        }
    }
}
[System.Serializable] public class SingleAxisInputMethod : InputMethod
{
    //only one set of the following are used (more than one set should never be set)
    public KeyCode positiveKeyCode = KeyCode.None; //used for input types: Key, MouseButton
    public KeyCode negativeKeyCode = KeyCode.None;//used for input types: Key, MouseButton

    public GamePadInput.Button positiveGamePadButton = GamePadInput.Button.None; //used for input types: GamePadButton
    public GamePadInput.Button negativeGamePadButton = GamePadInput.Button.None; //used for input types: GamePadButton

    public GamePadInput.Axis positiveGamePadAxis = GamePadInput.Axis.None; //used for input types: GamePadTrigger, GamePadStick
    public GamePadInput.Axis negativeGamePadAxis = GamePadInput.Axis.None; //used for input types: GamePadTrigger, GamePadStick

    public Direction positiveMouseDirection = Direction.None;
    public Direction negativeMouseDirection = Direction.None;

    public float currentValue;
    public float delay = 0; //(if acceleration is less than or equal to 0, acceleration will be instant)

    public override void InputTypeChanged(InputType _newInputType)
    {
        sendOnChannels = new List<bool>(parentEvent.channelCount);

        if (_newInputType == InputType.GamepadButton || _newInputType == InputType.GamepadStick || _newInputType == InputType.GamepadTrigger)
            for (int i = 0; i < parentEvent.channelCount; i++)
                sendOnChannels.Add(true);
        else
            for (int i = 0; i < parentEvent.channelCount; i++)
                sendOnChannels.Add(false);
    }

    public void ResetPositives()
    {
        positiveKeyCode = KeyCode.None;
        positiveGamePadButton = GamePadInput.Button.None;
        positiveGamePadAxis = GamePadInput.Axis.None;
        positiveMouseDirection = Direction.None;
    }
    public void ResetNegatives()
    {
        negativeKeyCode = KeyCode.None;
        negativeGamePadButton = GamePadInput.Button.None;
        negativeGamePadAxis = GamePadInput.Axis.None;
        negativeMouseDirection = Direction.None;
    }
    public override void ResetAll()
    {
        ResetPositives();
        ResetNegatives();

        currentValue = 0;
        delay = 0;
    }    
    public override bool Evaluate(int _playerIndex = 0)
    {
        float _inputValue = GetInputValue(_playerIndex);
        return _inputValue < -parentEvent.threshold || _inputValue > parentEvent.threshold;
    }
    public float GetInputValue(int _playerIndex = 0)
    {
        float _positiveValue = 0;
        float _negativeValue = 0;

        switch (inputType)
        {
            case InputType.Key:
            case InputType.MouseButton:
                {
                    _positiveValue = System.Convert.ToSingle(Input.GetKey(positiveKeyCode));
                    _negativeValue = parentEvent.hasNegativeInput ? System.Convert.ToSingle(Input.GetKey(negativeKeyCode)) : 0;

                    if (delay > 0)
                    {
                        if (currentValue < _positiveValue - _negativeValue)
                        {
                            currentValue += Time.unscaledDeltaTime * (1 / delay);
                            if (currentValue > _positiveValue - _negativeValue)
                                currentValue = _positiveValue - _negativeValue;
                        }

                        else if (currentValue > _positiveValue - _negativeValue)
                        {
                            currentValue -= Time.unscaledDeltaTime * (1 / delay);
                            if (currentValue < _positiveValue - _negativeValue)
                                currentValue = _positiveValue - _negativeValue;
                        }

                        currentValue = Mathf.Clamp(currentValue, -1, 1);
                    }
                    else
                        currentValue = _positiveValue - _negativeValue;
                    currentValue = Mathf.Clamp(currentValue, -1, 1);
                    return currentValue;
                }
            case InputType.GamepadButton:
                {
                    bool _positiveActive = GamePadInput.GetInputActive(_playerIndex, positiveGamePadButton);
                    bool _negativeActive = GamePadInput.GetInputActive(_playerIndex, negativeGamePadButton);

                    _positiveValue = System.Convert.ToSingle(_positiveActive);
                    _negativeValue = parentEvent.hasNegativeInput ? System.Convert.ToSingle(_negativeActive) : 0;

                    if (delay > 0)
                    {
                        if(currentValue < _positiveValue - _negativeValue)
                            currentValue += Time.unscaledDeltaTime * (1 / delay);

                        else if (currentValue > _positiveValue - _negativeValue)
                            currentValue -= Time.unscaledDeltaTime * (1 / delay);

                        currentValue = Mathf.Clamp(currentValue, -1, 1);
                    }
                    else
                        currentValue = _positiveValue - _negativeValue;

                    return currentValue;
                }
            case InputType.GamepadTrigger:
            case InputType.GamepadStick:
                _positiveValue = GamePadInput.GetInputValue(_playerIndex, positiveGamePadAxis);
                _negativeValue = GamePadInput.GetInputValue(_playerIndex, negativeGamePadAxis);

                currentValue = _positiveValue - _negativeValue;
                return currentValue;

            case InputType.MouseScrollDelta:
                switch (positiveMouseDirection)
                {
                    case Direction.Up:
                        _positiveValue = Input.mouseScrollDelta.y;
                        break;
                    case Direction.Down:
                        _positiveValue = -Input.mouseScrollDelta.y;
                        break;
                    default:
                        _positiveValue = 0;
                        break;
                }
                if(parentEvent.hasNegativeInput)
                {
                    switch (negativeMouseDirection)
                    {
                        case Direction.Up:
                            _negativeValue = Input.mouseScrollDelta.y;
                            break;
                        case Direction.Down:
                            _negativeValue = -Input.mouseScrollDelta.y;
                            break;
                        default:
                            _negativeValue = 0;
                            break;
                    }
                }
                else
                {
                    _negativeValue = 0;
                }
                    
                currentValue = (_positiveValue - _negativeValue) / 2;
                return currentValue;

            case InputType.MouseMove:
                switch (positiveMouseDirection)
                {
                    case Direction.Up:
                        _positiveValue = InputSystem.mousePositionDelta.y;
                        break;
                    case Direction.Down:
                        _positiveValue = -InputSystem.mousePositionDelta.y;
                        break;
                    case Direction.Left:
                        _positiveValue = -InputSystem.mousePositionDelta.x;
                        break;
                    case Direction.Right:
                        _positiveValue = InputSystem.mousePositionDelta.x;
                        break;
                    default:
                        _positiveValue = 0;
                        break;
                }
                
                if (parentEvent.hasNegativeInput)
                {
                    switch (negativeMouseDirection)
                    {
                        case Direction.Up:
                            _negativeValue = InputSystem.mousePositionDelta.y;
                            break;
                        case Direction.Down:
                            _negativeValue = -InputSystem.mousePositionDelta.y;
                            break;
                        case Direction.Left:
                            _negativeValue = -InputSystem.mousePositionDelta.x;
                            break;
                        case Direction.Right:
                            _negativeValue = InputSystem.mousePositionDelta.x;
                            break;
                        default:
                            _negativeValue = 0;
                            break;
                    }
                }
                else
                {
                    _negativeValue = 0;
                }

                currentValue = (_positiveValue - _negativeValue) / 2;
                return currentValue;

            default:
                return 0;
        }
    }
    public override void Send(InputState _state, SubscriptionData _subscriptionData, int _gamepad = -1)
    {
        InputEventInfo _inputEventInfo = new InputEventInfo();
        _inputEventInfo.axisType = InputEvent.AxisType.SingleAxis;
        _inputEventInfo.inputMethodName = name;
        _inputEventInfo.inputType = inputType;

        _inputEventInfo.gamepad = _gamepad;
        _inputEventInfo.inputState = _state;
        _inputEventInfo.timeHeld = timeActive;
        _inputEventInfo.timeReleased = timeInactive;
        _inputEventInfo.singleAxisValue = currentValue;

        if (_gamepad == -1) //send to default and all
        {
            _subscriptionData.Send(_inputEventInfo);
            for (int i = 0; i < sendOnChannels.Count; i++)
            {
                if (sendOnChannels[i])
                    _subscriptionData.Send(_inputEventInfo, i);
            }
        }
        else //send to default and channel (based on the gamepad)
        {
            _subscriptionData.Send(_inputEventInfo);
            _subscriptionData.Send(_inputEventInfo, _gamepad);
        }
    }
}
[System.Serializable] public class DualAxisInputMethod : InputMethod
{
    public SingleAxisInputMethod horizontal = new SingleAxisInputMethod();
    public SingleAxisInputMethod vertical = new SingleAxisInputMethod();

    public override void InputTypeChanged(InputType _newInputType)
    {
        if(horizontal.parentEvent == null)
            horizontal.parentEvent = parentEvent;

        if (vertical.parentEvent == null)
            vertical.parentEvent = parentEvent;

        horizontal.inputType = _newInputType;
        vertical.inputType = _newInputType;

        sendOnChannels = new List<bool>(parentEvent.channelCount);

        if (_newInputType == InputType.GamepadButton || _newInputType == InputType.GamepadStick || _newInputType == InputType.GamepadTrigger)
            for (int i = 0; i < parentEvent.channelCount; i++)
                sendOnChannels.Add(true);
        else
            for (int i = 0; i < parentEvent.channelCount; i++)
                sendOnChannels.Add(false);
    }

    public Vector2 currentValue
    {
        get { return new Vector2(horizontal.currentValue, vertical.currentValue); }
    }
    public float delay //(if acceleration is less than or equal to 0, acceleration will be instant)
    {
        get { return horizontal.delay; }
        set { horizontal.delay = value; vertical.delay = value; }
    }

    public override void ResetAll()
    {
        horizontal.ResetAll();
        vertical.ResetAll();
    }
    public override bool Evaluate(int _playerIndex = 0)
    {
        Vector2 _inputValue = GetInputValue(_playerIndex);
        switch(parentEvent.thresholdType)
        {
            case InputEvent.ThresholdType.Square:
                return _inputValue.x > parentEvent.threshold || _inputValue.x < parentEvent.threshold
                    || _inputValue.y > parentEvent.threshold || _inputValue.y < parentEvent.threshold;

            default:
                return _inputValue.magnitude > parentEvent.threshold;
        }
    }
    public Vector2 GetInputValue(int _playerIndex = 0)
    {
        return new Vector2(horizontal.GetInputValue(_playerIndex), vertical.GetInputValue(_playerIndex));
    }
    public override void Send(InputState _state, SubscriptionData _subscriptionData, int _gamepad = -1)
    {
        InputEventInfo _inputEventInfo = new InputEventInfo();
        _inputEventInfo.axisType = InputEvent.AxisType.DualAxis;
        _inputEventInfo.inputMethodName = name;
        _inputEventInfo.inputType = inputType;

        _inputEventInfo.gamepad = _gamepad;
        _inputEventInfo.inputState = _state;
        _inputEventInfo.timeHeld = timeActive;
        _inputEventInfo.timeReleased = timeInactive;
        _inputEventInfo.dualAxisValue = currentValue;

        if (_gamepad == -1) //send to default and all
        {
            _subscriptionData.Send(_inputEventInfo);
            for (int i = 0; i < sendOnChannels.Count; i++)
            {
                if (sendOnChannels[i])
                    _subscriptionData.Send(_inputEventInfo, i);
            }
        }
        else //send to default and channel (based on the gamepad)
        {
            _subscriptionData.Send(_inputEventInfo);
            _subscriptionData.Send(_inputEventInfo, _gamepad);
        }
    }
}

public class InputEventInfo
{
    public InputEvent.AxisType axisType;
    public string inputMethodName;
    public InputMethod.InputType inputType;

    public InputState inputState;
    public float timeHeld; //the time that the input has been held (down) for
    public float timeReleased; //the time that the input has been released (up) for
    public int gamepad;
    public float singleAxisValue;
    public Vector2 dualAxisValue;
    internal bool used = false;

    public void Use()
    {
        //when looping through and sending the event, we should stop if the event is used...
    }
}

public class InputEvent : ScriptableObject
{
    public string codeFriendlyName;
    public SubscriptionData subscriptionData;

    public enum AxisType { Binary, SingleAxis, DualAxis };
    public AxisType axisType;

    public List<BinaryInputMethod> binaryInputMethods = new List<BinaryInputMethod>();
    public List<SingleAxisInputMethod> singleAxisInputMethods = new List<SingleAxisInputMethod>();
    public List<DualAxisInputMethod> dualAxisInputMethods = new List<DualAxisInputMethod>();

    public int channelCount = 4;
    public bool hasNegativeInput = true;

    public float threshold = 0.2f; //uses only with axis types (Single is simple: if greater than threshold or less than -threshold)
    public enum ThresholdType { Radial, Square }; //Dual (Radial: if magnitude is greater than threshold)(Square: works like basic threshold but on both axes)
    public ThresholdType thresholdType = ThresholdType.Radial;
    
    public bool sendOnActive = false;
    public bool sendOnInactive = false;
    public bool sendOnTriggered = false;
    public bool sendOnReleased = false;

    public bool hasRepeatDelay
    {
        get { return repeatDelay != float.PositiveInfinity; }
        set
        {
            if(value != hasRepeatDelay)
            {
                if (value) repeatDelay = 0;
                else repeatDelay = float.PositiveInfinity;
            }
        }
    }
    public float repeatDelay = float.PositiveInfinity;

    public void EvaluateAndSend()
    {
        switch (axisType)
        {
            case AxisType.Binary:
                CheckInputMethods(binaryInputMethods);
                break;
            case AxisType.SingleAxis:
                CheckInputMethods(singleAxisInputMethods);
                break;
            case AxisType.DualAxis:
                CheckInputMethods(dualAxisInputMethods);
                break;
        }
    }

    private void CheckInputMethods<InputMethodType>(List<InputMethodType> _inputMethods) where InputMethodType : InputMethod, new()
    {
        for (int i = 0; i < _inputMethods.Count; i++)
        {
            if(_inputMethods[i].gamepadChannelWasActive == null)
                CheckInputMethodNormal(_inputMethods[i]);
            else
                CheckInputMethodGamepad(_inputMethods[i]);
        }
    }
    private void CheckInputMethodNormal<InputMethodType>(InputMethodType _inputMethod) where InputMethodType : InputMethod, new()
    {
        bool _activeThisFrame = _inputMethod.Evaluate();

        float _timesTriggered = _inputMethod.timesTriggeredSinceRelease + 1;
        float _altRepeatDelay = _inputMethod.repeatDelay * _timesTriggered;
        if (_activeThisFrame && _inputMethod.timeActive >= _altRepeatDelay)
        {
            _inputMethod.activeLastFrame = false;
            _inputMethod.timesTriggeredSinceRelease++;
        }

        InputState _inputState = InputSystem.ToInputState(_activeThisFrame, _inputMethod.activeLastFrame);
        _inputMethod.activeLastFrame = _activeThisFrame;

        switch (_inputState)
        {
            case InputState.Active:
                _inputMethod.timeActive = Time.realtimeSinceStartup - _inputMethod.lastTriggeredTimestamp;
                _inputMethod.timeInactive = 0;
                if (sendOnActive)
                    _inputMethod.Send(_inputState, subscriptionData);
                break;
            case InputState.Inactive:
                _inputMethod.timeInactive = Time.realtimeSinceStartup - _inputMethod.lastReleasedTimestamp;
                _inputMethod.timeActive = 0;
                if (sendOnInactive)
                    _inputMethod.Send(_inputState, subscriptionData);
                break;
            case InputState.Triggered:
                if (_inputMethod.timesTriggeredSinceRelease == 0)
                    _inputMethod.lastTriggeredTimestamp = Time.realtimeSinceStartup;
                if (sendOnTriggered)
                    _inputMethod.Send(_inputState, subscriptionData);
                break;
            case InputState.Released:
                _inputMethod.lastReleasedTimestamp = Time.realtimeSinceStartup;
                _inputMethod.timesTriggeredSinceRelease = 0;
                if (sendOnReleased)
                    _inputMethod.Send(_inputState, subscriptionData);
                break;
        }
    }
    private void CheckInputMethodGamepad<InputMethodType>(InputMethodType _inputMethod) where InputMethodType : InputMethod, new()
    {
        InputState _stateToSendToAll = InputState.Inactive;

        for (int i = 0; i < _inputMethod.sendOnChannels.Count; i++) //check all channels
        {
            if (!_inputMethod.sendOnChannels[i]) //continue if we don't send on this channel
                continue;

            bool _activeThisFrame = _inputMethod.Evaluate(i); //evaluate the state of the gamepad channel (player index)

            float _timesTriggered = _inputMethod.gamepadChannelTimesTriggered[i] + 1;
            float _altRepeatDelay = _inputMethod.repeatDelay * _timesTriggered;
            if (_activeThisFrame && _inputMethod.gamepadChannelTimeActive[i] >= _altRepeatDelay)
            {
                _inputMethod.gamepadChannelWasActive[i] = false;
                _inputMethod.gamepadChannelTimesTriggered[i]++;
            }

            InputState _inputState = InputSystem.ToInputState(_activeThisFrame, _inputMethod.gamepadChannelWasActive[i]);
            _inputMethod.gamepadChannelWasActive[i] = _activeThisFrame;

            switch (_inputState)
            {
                case InputState.Active:
                    _inputMethod.gamepadChannelTimeActive[i] = Time.realtimeSinceStartup - _inputMethod.gamepadTriggeredTime[i];
                    _inputMethod.gamepadChannelTimeInactive[i] = 0;
                    if (_stateToSendToAll != InputState.Triggered) _stateToSendToAll = InputState.Active;
                    if (sendOnActive)
                        _inputMethod.Send(_inputState, subscriptionData, i);
                    break;
                case InputState.Inactive:
                    _inputMethod.gamepadChannelTimeInactive[i] = Time.realtimeSinceStartup - _inputMethod.gamepadReleasedTime[i];
                    _inputMethod.gamepadChannelTimeActive[i] = 0;
                    if (sendOnInactive)
                        _inputMethod.Send(_inputState, subscriptionData, i);
                    break;
                case InputState.Triggered:
                    if (_inputMethod.gamepadChannelTimesTriggered[i] == 0)
                        _inputMethod.gamepadTriggeredTime[i] = Time.realtimeSinceStartup;
                    _stateToSendToAll = InputState.Triggered;
                    if (sendOnTriggered)
                        _inputMethod.Send(_inputState, subscriptionData, i);
                    break;
                case InputState.Released:
                    _inputMethod.gamepadReleasedTime[i] = Time.realtimeSinceStartup;
                    _inputMethod.gamepadChannelTimesTriggered[i] = 0;
                    if (_stateToSendToAll != InputState.Triggered) _stateToSendToAll = InputState.Released;
                    if (sendOnReleased)
                        _inputMethod.Send(_inputState, subscriptionData, i);
                    break;
            }
        }
    }
}