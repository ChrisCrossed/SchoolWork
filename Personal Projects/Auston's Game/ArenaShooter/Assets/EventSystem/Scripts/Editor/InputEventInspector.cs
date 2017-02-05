using UnityEditor;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using XInputDotNetPure;

//[InitializeOnLoad]
[CustomEditor(typeof(InputEvent))]
public class InputEventInspector : Editor
{
    private string oldName = "";

    private static Vector2 scrollPos;
    private static bool foundUnsetInput;

    private static double lastClipboardCopy = 0;
    private static double lastInputSet = 0;
    private static bool editMode;

    private static GamePadState[] previousState = new GamePadState[4];
    private static GamePadState[] currentState = new GamePadState[4];

    private static void RepaintEventWindow()
    {
        EditorWindow _previouslyFocused = EditorWindow.focusedWindow;
        EditorWindow.GetWindow<EventEditor>().Repaint();
        if (_previouslyFocused != null)
            _previouslyFocused.Focus();
    }

    public override void OnInspectorGUI()
    {
        EditorUtility.SetDirty(target);

        for (int i = 0; i < 4; i++)
        {
            PlayerIndex _playerIndex = (PlayerIndex)i;
            GamePadState _state = GamePad.GetState(_playerIndex);
            previousState[i] = currentState[i];
            currentState[i] = _state;
        }

        InputEvent _event = (InputEvent)target; //the scriptable object as an Event

        //if (EditorWindow.focusedWindow.GetType().Name != "InspectorWindow")
        //    CancelEdit(_event);

        DrawNameField(_event); //Draws the text field for the name of the Event
        _event.axisType = (InputEvent.AxisType)EditorGUILayout.EnumPopup("Axis Type", _event.axisType);

        _event.channelCount = Mathf.Clamp(EditorGUILayout.IntField("Channel Count", _event.channelCount), 0, 1000);
        
        //oldName = EditorGUILayout.DelayedTextField(oldName);
        //Debug.Log(oldName);

        if (_event.axisType == InputEvent.AxisType.SingleAxis)
        {
            GUILayout.Space(5);
            _event.hasNegativeInput = EditorGUILayout.Toggle("Has Negative Input", _event.hasNegativeInput);
        }

        switch (_event.axisType)
        {
            case InputEvent.AxisType.Binary:
                DrawInputMethods(_event, _event.binaryInputMethods);
                break;
            case InputEvent.AxisType.SingleAxis:
                DrawInputMethods(_event, _event.singleAxisInputMethods);
                DisplaySingleAxisThreshold(_event);
                break;
            case InputEvent.AxisType.DualAxis:
                DrawInputMethods(_event, _event.dualAxisInputMethods);
                DisplayDualAxisThreshold(_event);
                break;
        }

        #region REPEAT DELAY
        GUILayout.Space(8);
        GUILayout.BeginHorizontal();
        _event.hasRepeatDelay = EditorGUILayout.ToggleLeft("", _event.hasRepeatDelay, GUILayout.Width(20));
        GUILayout.Label("Repeats If Held");
        GUILayout.Space(10);
        GUILayout.Label("|");
        GUILayout.Space(10);
        EditorGUI.BeginDisabledGroup(!_event.hasRepeatDelay);
        if(EditorGUIUtility.currentViewWidth > 315)
            GUILayout.Label("Repeat Delay");
        else
            GUILayout.Label("Delay");
        GUILayout.Space(10);
        _event.repeatDelay = EditorGUILayout.FloatField(_event.repeatDelay, GUILayout.MinWidth(10));
        EditorGUI.EndDisabledGroup();
        GUILayout.Space(5);
        GUILayout.EndHorizontal();
        #endregion

        #region SEND EVENT ON SELECTIONS
        GUILayout.Space(8);

        GUILayout.BeginHorizontal();
        GUILayout.Label("Send On ");

        GUIStyle _warningStyle = new GUIStyle();
        _warningStyle.normal.textColor = Color.red;
        _warningStyle.contentOffset = new Vector2(0, 1);
        _warningStyle.fontSize = 10;
        if (!_event.sendOnActive && !_event.sendOnInactive && !_event.sendOnTriggered && !_event.sendOnReleased)
        {

            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField("[Event will never be sent]", _warningStyle);

        }
        GUILayout.EndHorizontal();
        
        GUILayout.BeginHorizontal();
        GUIContent _activeContent = new GUIContent("Active", "Sends every frame the input is valid (active)");
        GUIContent _inactiveContent = new GUIContent("Inactive", "Sends every frame the input is NOT valid (inactive)");
        GUIContent _triggeredContent = new GUIContent("Triggered", "Sends on the frame the input becomes valid (triggered)");
        GUIContent _releasedContent = new GUIContent("Released", "Sends on the frame the input becomes invalid (released)");
        float _height = 20;
        _event.sendOnActive = GUILayout.Toggle(_event.sendOnActive, _activeContent, "ButtonLeft", GUILayout.Height(_height));
        _event.sendOnInactive = GUILayout.Toggle(_event.sendOnInactive, _inactiveContent, "ButtonMid", GUILayout.Height(_height));
        _event.sendOnTriggered = GUILayout.Toggle(_event.sendOnTriggered, _triggeredContent, "ButtonMid", GUILayout.Height(_height));
        _event.sendOnReleased = GUILayout.Toggle(_event.sendOnReleased, _releasedContent, "ButtonRight", GUILayout.Height(_height));
        GUILayout.EndHorizontal();
        #endregion

        #region BOTTOM BUTTONS ("COPY DECLARATION" AND "WRITE CHANGES")
        GUIStyle _bottomHorizontalStyle = new GUIStyle();
        _bottomHorizontalStyle.margin = new RectOffset(0, 0, 11, 0);
        GUILayout.BeginHorizontal(_bottomHorizontalStyle);

        double timeSinceClipboardCopy = EditorApplication.timeSinceStartup - lastClipboardCopy;
        if (timeSinceClipboardCopy < 0.4d)
        {
            GUIStyle _declarationTextStyle = new GUIStyle(EditorStyles.label);
            _declarationTextStyle.normal.textColor = new Color(0.05f, 0.5f, 0.08f);

            GUILayout.FlexibleSpace();
            GUILayout.Label("Declaration copied to clipboard.", _declarationTextStyle);
            GUILayout.FlexibleSpace();
        }

        ClipboardButton(_event);
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Write Changes", EditorStyles.miniButton, GUILayout.Width((EditorGUIUtility.currentViewWidth * 0.3f) + 35), GUILayout.Height(19)))
        {
            EventEditor.WriteEvents(false);
        }

        GUILayout.EndHorizontal();
        #endregion
    }

    public void DrawNameField(InputEvent _event)
    {
        GUI.SetNextControlName("Event Name");
        string _newName = EditorGUILayout.DelayedTextField("Event Name", _event.name);
        string _newCodeFriendly = _newName.Replace(" ", "");

        if (_newName != _event.name || _event.codeFriendlyName != _newCodeFriendly)
        {
            string _assetPath = AssetDatabase.GetAssetPath(_event); //the location of the event in the project
            AssetDatabase.RenameAsset(_assetPath, _newName); //returns error message if fails
            AssetDatabase.Refresh();

            RepaintEventWindow();
            EventEditor.WriteEvents(true);

            _event.codeFriendlyName = _newCodeFriendly;
        }
    }
    public void ClipboardButton(InputEvent _event)
    {
        if (GUILayout.Button("Copy Declaration", EditorStyles.miniButton, GUILayout.Width((EditorGUIUtility.currentViewWidth * 0.3f) + 35), GUILayout.Height(19)))
        {
            lastClipboardCopy = EditorApplication.timeSinceStartup;
            EditorGUIUtility.systemCopyBuffer = "Still in development! Will create function which listens for the event.";
            //EditorGUIUtility.systemCopyBuffer = "private void On" + _event.codeFriendlyName + " (" + _event.GetParamTypeNameList(false) + ") \n{\n            \n}";
        }
    }
    private void DrawInputMethods<InputMethodType>(InputEvent _event, List<InputMethodType> _inputMethods) where InputMethodType : InputMethod, new()
    {
        GUILayout.Space(8);
        GUILayout.Label("Input Methods");

        scrollPos = GUILayout.BeginScrollView(scrollPos, false, false);

        if (_inputMethods == null || _inputMethods.Count == 0) //no input methods
        {
            GUILayout.BeginHorizontal(EditorStyles.helpBox);
            GUILayout.FlexibleSpace();
            GUIStyle _noInputMethodsWarnStyle = new GUIStyle(EditorStyles.boldLabel);
            _noInputMethodsWarnStyle.fontSize = 10;
            _noInputMethodsWarnStyle.normal.textColor = new Color(0.2f, 0.2f, 0.2f);
            GUILayout.Label("No Input Methods", _noInputMethodsWarnStyle);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        else
        {

            //GUIStyle _verticalStyle = new GUIStyle("RegionBg");
            //GUIStyle _verticalStyle = new GUIStyle("flow overlay box");
            GUIStyle _verticalStyle = new GUIStyle(EditorStyles.helpBox);
            //GUIStyle _verticalStyle = new GUIStyle("RL Background");
            _verticalStyle.padding = new RectOffset(10, 10, 6, 13);
            //_verticalStyle.margin = new RectOffset(0, 0, 0, 0);
            _verticalStyle.padding = new RectOffset(0, 0, 6, 8);
            foundUnsetInput = false;

            int _verticalDistanceTraveled = 0;
            Rect _testRect = new Rect(0, 0, 0, 0);
            UnityEngine.Event _unityEvent = UnityEngine.Event.current;

            for (int i = 0; i < _inputMethods.Count; i++) //looping through each parameter
            {
                //int _verticalArea = i == 0 ? 20 : 40;
                //_testRect = new Rect(0, _verticalDistanceTraveled, EditorGUIUtility.currentViewWidth, _verticalArea);
                //_verticalDistanceTraveled += _verticalArea;
                ////GUI.Button(_testRect, "Test");
                ////EditorGUI.DrawRect(_testRect, Color.black);

                //bool _testBool = _unityEvent.type == EventType.MouseMove && _testRect.Contains(_unityEvent.mousePosition, false);
                //Debug.Log(_testBool);

                //if (_testBool)
                //{
                //    if (i == 0)
                //        GUILayout.Space(3);

                //    GUIStyle _testStyle = new GUIStyle("PR Insertion Above");
                //    _testStyle.hover.background = _testStyle.normal.background;
                //    _testStyle.stretchWidth = true;
                //    _testStyle.imagePosition = ImagePosition.ImageOnly;
                //    _testStyle.padding = new RectOffset(0, 0, 0, 0);
                //    _testStyle.margin = new RectOffset(5, 0, 0, 0);

                //    GUILayout.Label("", _testStyle, GUILayout.Height(0));
                //}

                //GUIStyle _buttonStyle = new GUIStyle(EditorStyles.miniButtonLeft);
                ////_buttonStyle.margin = new RectOffset(0, 0, 0, 0);
                //_buttonStyle.normal.background = EditorGUIUtility.FindTexture("overlay header lower right");
                ////GUILayout.BeginHorizontal();
                //GUILayout.Button("?", _buttonStyle);
                GUILayout.BeginVertical(_verticalStyle);

                DrawInputMethod(_event, _inputMethods, i);

                GUILayout.EndVertical();
                //GUILayout.EndHorizontal();
                //GUILayout.Space(2);
            }

            //_testRect = new Rect(0, _verticalDistanceTraveled, EditorGUIUtility.currentViewWidth, 20);
            //_verticalDistanceTraveled += 20;
            //GUI.Button(_testRect, "Test");

            //if (_testRect.Contains(_unityEvent.mousePosition))
            //{
            //    GUIStyle _testStyle = new GUIStyle("PR Insertion Above");
            //    _testStyle.hover.background = _testStyle.normal.background;
            //    _testStyle.stretchWidth = true;
            //    _testStyle.imagePosition = ImagePosition.ImageOnly;
            //    _testStyle.padding = new RectOffset(0, 0, 0, 0);
            //    _testStyle.margin = new RectOffset(5, 0, 0, 0);

            //    GUILayout.Label("", _testStyle, GUILayout.Height(0));
            //}
        }

        #region ADD INPUT METHOD BUTTON
        if(_inputMethods != null)
        {
            GUIStyle _topVerticalStyle = new GUIStyle();
            _topVerticalStyle.margin = new RectOffset(0, 0, 11, 11);
            GUILayout.BeginHorizontal(_topVerticalStyle);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Add Input Method", EditorStyles.miniButton, GUILayout.Width(130), GUILayout.Height(19)))
            {
                InputMethodType _newInputMethod = new InputMethodType();
                _newInputMethod.parentEvent = _event;
                _newInputMethod.name = "Method " + (_inputMethods.Count + 1);
                _inputMethods.Add(_newInputMethod);
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        #endregion

        GUILayout.FlexibleSpace(); //space to bottom of window
        GUILayout.EndScrollView();
    }
    private void DrawInputMethod<InputMethodType>(InputEvent _event, List<InputMethodType> _inputMethods, int _methodIndex) where InputMethodType : InputMethod
    {
        InputMethodType _inputMethod = _inputMethods[_methodIndex];

        GUILayout.BeginHorizontal();

        GUIStyle _foldoutStyle = new GUIStyle(EditorStyles.foldout);
        _foldoutStyle.padding = new RectOffset(14, 0, 0, 0);
        _foldoutStyle.margin = new RectOffset(17, 0, 0, 0);
        _foldoutStyle.border = new RectOffset(14, 0, 13, 0);
        _foldoutStyle.fixedWidth = 0;
        _foldoutStyle.stretchWidth = false;

        GUIContent _foldoutContent = new GUIContent();
        _foldoutContent.text = "";
        //_inputMethod.expanded = EditorGUI.Foldout(new Rect(20,10,0,0), _inputMethod.expanded, _foldoutContent, true, _foldoutStyle);
        if(!_inputMethod.editingName)
            _inputMethod.expanded = EditorGUILayout.Foldout(_inputMethod.expanded, _foldoutContent, _foldoutStyle);

        #region HEADER
        GUIStyle _headerStyle = new GUIStyle(EditorStyles.boldLabel);
        _headerStyle.contentOffset = new Vector2(-63, 0);
        _headerStyle.margin = new RectOffset(15, 0, 0, 0);
        _headerStyle.fixedWidth = 15;
        _headerStyle.stretchWidth = false;
        _headerStyle.clipping = TextClipping.Overflow;

        if (_inputMethod.editingName)
        {
            GUIStyle _nameFieldStyle = new GUIStyle(EditorStyles.textField);
            _nameFieldStyle.margin = new RectOffset(7, 15, 0, 0);

            GUI.SetNextControlName("NameField");
            string _newName = EditorGUILayout.DelayedTextField(_inputMethod.name, _nameFieldStyle);

            if (_newName.Length > 14)
                _newName = _newName.Remove(14);

            if (_newName != _inputMethod.name)
            {
                if(_newName != "")
                    _inputMethod.name = _newName;
                _inputMethod.editingName = false;
            }

            UnityEngine.Event _unityEvent = UnityEngine.Event.current;
            if (_unityEvent.type == EventType.MouseDown)
                _inputMethod.editingName = false;
        }
        else
            GUILayout.Label(_inputMethod.name, _headerStyle);
        #endregion

        GUILayout.FlexibleSpace();

        #region ADD DELAY, RESET, AND DELETE BUTTONS
        GUIStyle _topButtons = new GUIStyle(EditorStyles.miniButton);
        _topButtons.margin = new RectOffset(0, 5, 0, 20);
        _topButtons.padding = new RectOffset(0, 0, 2, 2);
        _topButtons.alignment = TextAnchor.MiddleCenter;

        #region ADD DELAY BUTTON
        if (_event.axisType == InputEvent.AxisType.SingleAxis && _inputMethod.expanded)
        {
            SingleAxisInputMethod _singleAxisMethod = _event.singleAxisInputMethods[_methodIndex];
            if (_singleAxisMethod.inputType == InputMethod.InputType.Key
            || _singleAxisMethod.inputType == InputMethod.InputType.MouseButton
            || _singleAxisMethod.inputType == InputMethod.InputType.GamepadButton)
            {
                if (_singleAxisMethod.delay == 0)
                {
                    _topButtons.fixedWidth = 17;
                    //_topButtons.fontSize = 10;

                    if (GUILayout.Button("+", _topButtons, GUILayout.Width(22)))
                    {
                        _singleAxisMethod.delay = 0.1f;
                    }
                }
            }
        }
        if (_event.axisType == InputEvent.AxisType.DualAxis && _inputMethod.expanded)
        {
            DualAxisInputMethod _dualAxisMethod = _event.dualAxisInputMethods[_methodIndex];
            if (_dualAxisMethod.inputType == InputMethod.InputType.Key
            || _dualAxisMethod.inputType == InputMethod.InputType.MouseButton
            || _dualAxisMethod.inputType == InputMethod.InputType.GamepadButton)
            {
                if (_dualAxisMethod.delay == 0)
                {
                    _topButtons.fixedWidth = 17;
                    //_topButtons.fontSize = 10;

                    if (GUILayout.Button("+", _topButtons, GUILayout.Width(22)))
                    {
                        _dualAxisMethod.delay = 0.1f;
                    }
                }
            }
        }
        #endregion

        _topButtons.fixedWidth = 45;

        if(_inputMethod.expanded)
        {
            if (GUILayout.Button("Reset", _topButtons))
            {
                _inputMethod.ResetAll();
                _inputMethod.expanded = true;
            }
        }
        else
        {
            _topButtons.fixedWidth = 50;

            if (_inputMethod.editingName == false)
            {
                if (GUILayout.Button("Rename", _topButtons))
                {
                    _inputMethod.editingName = true;
                    EditorGUI.FocusTextInControl("NameField");
                }
            }
            else
            {
                if (GUILayout.Button("Cancel", _topButtons))
                {
                    _inputMethod.editingName = false;
                }
            }
            _topButtons.fixedWidth = 45;
        }

        if (GUILayout.Button("Delete", _topButtons))
        {
            _inputMethods.RemoveAt(_methodIndex);
            return;
        }
        #endregion

        GUILayout.EndHorizontal();

        #region WARNING STYLE
        GUIStyle _warningStyle = new GUIStyle(EditorStyles.label);
        _warningStyle.normal.textColor = Color.red;
        #endregion

        if (_inputMethod.expanded == false)
            return;

        InputMethod.InputType _newInputType = (InputMethod.InputType)EditorGUILayout.EnumPopup("Input Type", _inputMethod.inputType);
        bool _typeChanged = _newInputType != _inputMethod.inputType; //if the inputType changed
        _inputMethod.inputType = _newInputType;

        if (_typeChanged) EditorGUI.FocusTextInControl(""); //unfocus the input method selection

        switch (_event.axisType)
        {
            case InputEvent.AxisType.Binary:
                #region BINARY METHOD CHECKS
                BinaryInputMethod _binaryMethod = _event.binaryInputMethods[_methodIndex];

                if (_typeChanged)
                    _binaryMethod.ResetAll();

                switch (_binaryMethod.inputType)
                {
                    #region KEY
                    case InputMethod.InputType.Key:
                        CheckKeyCode("Key", ref _binaryMethod.keyCode);
                        if (_binaryMethod.keyCode == KeyCode.None)
                            Repaint(); //ensures another GUI update next frame
                        break;
                    #endregion
                    #region MOUSE BUTTON
                    case InputMethod.InputType.MouseButton:
                        CheckMouseButton("Mouse Button", ref _binaryMethod.keyCode);
                        if (_binaryMethod.keyCode == KeyCode.None)
                            Repaint(); //ensures another GUI update next frame
                        break;
                    #endregion
                    #region GAMEPAD BUTTON
                    case InputMethod.InputType.GamepadButton:
                        CheckGamepadButton("Gamepad Button", ref _binaryMethod.gamePadButton);
                        if(_binaryMethod.gamePadButton == GamePadInput.Button.None)
                            Repaint(); //ensures another GUI update next frame
                        break;
                    #endregion
                    #region GAMEPAD TRIGGER
                    case InputMethod.InputType.GamepadTrigger:
                        CheckGamepadTrigger("Gamepad Trigger", ref _binaryMethod.gamePadAxis);
                        if (_binaryMethod.gamePadAxis == GamePadInput.Axis.None)
                            Repaint(); //ensures another GUI update next frame
                        else
                            DisplayThreshold(_binaryMethod);
                        break;
                    #endregion
                    #region GAMEPAD STICK
                    case InputMethod.InputType.GamepadStick:
                        CheckGamepadStick("Gamepad Stick", ref _binaryMethod.gamePadAxis);
                        if (_binaryMethod.gamePadAxis == GamePadInput.Axis.None)
                            Repaint(); //ensures another GUI update next frame
                        else
                            DisplayThreshold(_binaryMethod);
                        break;
                    #endregion
                    #region MOUSE SCROLL DELTA
                    case InputMethod.InputType.MouseScrollDelta:
                        CheckMouseScrollDelta("Scroll Direction", ref _binaryMethod.mouseDirection);
                        if (_binaryMethod.mouseDirection == Direction.None)
                            Repaint(); //ensures another GUI update next frame
                        else
                            DisplayThreshold(_binaryMethod);
                        break;
                    #endregion
                    #region MOUSE POSITION DELTA
                    case InputMethod.InputType.MouseMove:
                        CheckMouseMove("Move Direction", ref _binaryMethod.mouseDirection);
                        if (_binaryMethod.mouseDirection == Direction.None)
                            Repaint(); //ensures another GUI update next frame
                        else
                            DisplayThreshold(_binaryMethod);
                        break;
                    #endregion
                    #region DEFAULT
                    default:
                        return;
                    #endregion
                }
                #endregion
                break;

            case InputEvent.AxisType.SingleAxis:
                #region SINGLE AXIS METHOD CHECKS
                SingleAxisInputMethod _singleAxisMethod = _event.singleAxisInputMethods[_methodIndex];

                if (_typeChanged)
                    _singleAxisMethod.ResetAll();

                GUIStyle _horizontal = new GUIStyle();
                _horizontal.margin = new RectOffset(0, 1, 0, 0);

                GUIStyle _toggle = new GUIStyle(EditorStyles.toggle);
                _toggle.margin = new RectOffset(6, 0, 5, 0);

                bool _hasNegative = _singleAxisMethod.parentEvent.hasNegativeInput;

                switch (_singleAxisMethod.inputType)
                {
                    #region KEY
                    case InputMethod.InputType.Key:
                        CheckKeyCode("Positive Key", ref _singleAxisMethod.positiveKeyCode, foundUnsetInput);
                        if (_hasNegative) CheckKeyCode("Negative Key", ref _singleAxisMethod.negativeKeyCode, foundUnsetInput);

                        bool _positiveSet = _singleAxisMethod.positiveKeyCode != KeyCode.None;
                        bool _negativeSet = _singleAxisMethod.negativeKeyCode != KeyCode.None;

                        if (_positiveSet == false || (_hasNegative && _negativeSet == false))
                            Repaint(); //ensures another GUI update next frame to check input again (because they weren't set this frame

                        break;
                    #endregion
                    #region MOUSE BUTTON
                    case InputMethod.InputType.MouseButton:
                        CheckMouseButton("Positive Button", ref _singleAxisMethod.positiveKeyCode, foundUnsetInput);
                        if (_hasNegative) CheckMouseButton("Negative Button", ref _singleAxisMethod.negativeKeyCode, foundUnsetInput);

                        _positiveSet = _singleAxisMethod.positiveKeyCode != KeyCode.None;
                        _negativeSet = _singleAxisMethod.negativeKeyCode != KeyCode.None;

                        if (_positiveSet == false || (_hasNegative && _negativeSet == false))
                            Repaint(); //ensures another GUI update next frame

                        break;
                    #endregion
                    #region GAMEPAD BUTTON
                    case InputMethod.InputType.GamepadButton:
                        CheckGamepadButton("Positive Button", ref _singleAxisMethod.positiveGamePadButton, foundUnsetInput);
                        if (_hasNegative) CheckGamepadButton("Negative Button", ref _singleAxisMethod.negativeGamePadButton, foundUnsetInput);

                        _positiveSet = _singleAxisMethod.positiveGamePadButton != GamePadInput.Button.None;
                        _negativeSet = _singleAxisMethod.negativeGamePadButton != GamePadInput.Button.None;

                        if (_positiveSet == false || (_hasNegative && _negativeSet == false))
                            Repaint(); //ensures another GUI update next frame

                        break;
                    #endregion
                    #region GAMEPAD TRIGGER
                    case InputMethod.InputType.GamepadTrigger:
                        CheckGamepadTrigger("Positive Trigger", ref _singleAxisMethod.positiveGamePadAxis, foundUnsetInput);
                        if (_hasNegative) CheckGamepadTrigger("Negative Trigger", ref _singleAxisMethod.negativeGamePadAxis, foundUnsetInput);

                        _positiveSet = _singleAxisMethod.positiveGamePadAxis != GamePadInput.Axis.None;
                        _negativeSet = _singleAxisMethod.negativeGamePadAxis != GamePadInput.Axis.None;

                        if (_positiveSet == false || (_hasNegative && _negativeSet == false))
                            Repaint(); //ensures another GUI update next frame
                        break;
                    #endregion
                    #region GAMEPAD STICK
                    case InputMethod.InputType.GamepadStick:
                        CheckGamepadStick("Positive Stick", ref _singleAxisMethod.positiveGamePadAxis, foundUnsetInput);
                        if (_hasNegative) CheckGamepadStick("Negative Stick", ref _singleAxisMethod.negativeGamePadAxis, foundUnsetInput);

                        _positiveSet = _singleAxisMethod.positiveGamePadAxis != GamePadInput.Axis.None;
                        _negativeSet = _singleAxisMethod.negativeGamePadAxis != GamePadInput.Axis.None;

                        if (_positiveSet == false || (_hasNegative && _negativeSet == false))
                            Repaint(); //ensures another GUI update next frame
                        break;
                    #endregion
                    #region MOUSE SCROLL DELTA
                    case InputMethod.InputType.MouseScrollDelta:
                        CheckMouseScrollDelta("Positive Direction", ref _singleAxisMethod.positiveMouseDirection, foundUnsetInput);
                        if (_hasNegative) CheckMouseScrollDelta("Negative Direction", ref _singleAxisMethod.negativeMouseDirection, foundUnsetInput);

                        _positiveSet = _singleAxisMethod.positiveMouseDirection != Direction.None;
                        _negativeSet = _singleAxisMethod.negativeMouseDirection != Direction.None;

                        if (_positiveSet == false || (_hasNegative && _negativeSet == false))
                            Repaint(); //ensures another GUI update next frame
                        //else
                        //    DisplayThreshold(_singleAxisMethod); //needs overload
                        break;
                    #endregion
                    #region MOUSE POSITION DELTA
                    case InputMethod.InputType.MouseMove:
                        CheckMouseMove("Positive Direction", ref _singleAxisMethod.positiveMouseDirection, foundUnsetInput);
                        if (_hasNegative) CheckMouseMove("Negative Direction", ref _singleAxisMethod.negativeMouseDirection, foundUnsetInput);

                        _positiveSet = _singleAxisMethod.positiveMouseDirection != Direction.None;
                        _negativeSet = _singleAxisMethod.negativeMouseDirection != Direction.None;

                        if (_positiveSet == false || (_hasNegative && _negativeSet == false))
                            Repaint(); //ensures another GUI update next frame
                        break;
                    #endregion
                    #region DEFAULT

                    default:
                        return;
                        #endregion
                }

                #endregion
                break;
            case InputEvent.AxisType.DualAxis:
                #region DUAL AXIS METHOD CHECKS
                DualAxisInputMethod _dualAxisMethod = _event.dualAxisInputMethods[_methodIndex];

                if (_typeChanged)
                    _dualAxisMethod.ResetAll();

                switch (_dualAxisMethod.inputType)
                {
                    #region KEY
                    case InputMethod.InputType.Key:
                        CheckKeyCode("Up Key", ref _dualAxisMethod.vertical.positiveKeyCode, foundUnsetInput);
                        CheckKeyCode("Down Key", ref _dualAxisMethod.vertical.negativeKeyCode, foundUnsetInput);
                        CheckKeyCode("Left Key", ref _dualAxisMethod.horizontal.negativeKeyCode, foundUnsetInput);
                        CheckKeyCode("Right Key", ref _dualAxisMethod.horizontal.positiveKeyCode, foundUnsetInput);

                        bool _upSet = _dualAxisMethod.vertical.positiveKeyCode != KeyCode.None;
                        bool _downSet = _dualAxisMethod.vertical.negativeKeyCode != KeyCode.None;
                        bool _leftSet = _dualAxisMethod.horizontal.negativeKeyCode != KeyCode.None;
                        bool _rightSet = _dualAxisMethod.horizontal.positiveKeyCode != KeyCode.None;

                        if (_upSet == false || _downSet == false || _leftSet == false || _rightSet == false)
                            Repaint(); //ensures another GUI update next frame to check input again (because they weren't set this frame

                        break;
                    #endregion
                    #region MOUSE BUTTON
                    case InputMethod.InputType.MouseButton:
                        CheckMouseButton("Up Button", ref _dualAxisMethod.vertical.positiveKeyCode, foundUnsetInput);
                        CheckMouseButton("Down Button", ref _dualAxisMethod.vertical.negativeKeyCode, foundUnsetInput);
                        CheckKeyCode("Left Button", ref _dualAxisMethod.horizontal.negativeKeyCode, foundUnsetInput);
                        CheckKeyCode("Right Button", ref _dualAxisMethod.horizontal.positiveKeyCode, foundUnsetInput);

                        _upSet = _dualAxisMethod.vertical.positiveKeyCode != KeyCode.None;
                        _downSet = _dualAxisMethod.vertical.negativeKeyCode != KeyCode.None;
                        _leftSet = _dualAxisMethod.horizontal.negativeKeyCode != KeyCode.None;
                        _rightSet = _dualAxisMethod.horizontal.positiveKeyCode != KeyCode.None;

                        if (_upSet == false || _downSet == false || _leftSet == false || _rightSet == false)
                            Repaint(); //ensures another GUI update next frame to check input again (because they weren't set this frame

                        break;
                    #endregion
                    #region GAMEPAD BUTTON
                    case InputMethod.InputType.GamepadButton:
                        CheckGamepadButton("Up Button", ref _dualAxisMethod.vertical.positiveGamePadButton, foundUnsetInput);
                        CheckGamepadButton("Down Button", ref _dualAxisMethod.vertical.negativeGamePadButton, foundUnsetInput);
                        CheckGamepadButton("Left Button", ref _dualAxisMethod.horizontal.negativeGamePadButton, foundUnsetInput);
                        CheckGamepadButton("Right Button", ref _dualAxisMethod.horizontal.positiveGamePadButton, foundUnsetInput);

                        _upSet = _dualAxisMethod.vertical.positiveGamePadButton != GamePadInput.Button.None;
                        _downSet = _dualAxisMethod.vertical.negativeGamePadButton != GamePadInput.Button.None;
                        _leftSet = _dualAxisMethod.horizontal.negativeGamePadButton != GamePadInput.Button.None;
                        _rightSet = _dualAxisMethod.horizontal.positiveGamePadButton != GamePadInput.Button.None;

                        if (_upSet == false || _downSet == false || _leftSet == false || _rightSet == false)
                            Repaint(); //ensures another GUI update next frame to check input again (because they weren't set this frame

                        break;
                    #endregion
                    #region GAMEPAD TRIGGER
                    case InputMethod.InputType.GamepadTrigger:
                        CheckGamepadTrigger("Up Trigger", ref _dualAxisMethod.vertical.positiveGamePadAxis, foundUnsetInput);
                        CheckGamepadTrigger("Down Trigger", ref _dualAxisMethod.vertical.negativeGamePadAxis, foundUnsetInput);
                        CheckGamepadTrigger("Left Trigger", ref _dualAxisMethod.horizontal.negativeGamePadAxis, foundUnsetInput);
                        CheckGamepadTrigger("Right Trigger", ref _dualAxisMethod.horizontal.positiveGamePadAxis, foundUnsetInput);

                        _upSet = _dualAxisMethod.vertical.positiveGamePadAxis != GamePadInput.Axis.None;
                        _downSet = _dualAxisMethod.vertical.negativeGamePadAxis != GamePadInput.Axis.None;
                        _leftSet = _dualAxisMethod.horizontal.negativeGamePadAxis != GamePadInput.Axis.None;
                        _rightSet = _dualAxisMethod.horizontal.positiveGamePadAxis != GamePadInput.Axis.None;

                        if (_upSet == false || _downSet == false || _leftSet == false || _rightSet == false)
                            Repaint(); //ensures another GUI update next frame to check input again (because they weren't set this frame

                        break;
                    #endregion
                    #region GAMEPAD STICK
                    case InputMethod.InputType.GamepadStick:
                        CheckGamepadStick("Up Stick", ref _dualAxisMethod.vertical.positiveGamePadAxis, foundUnsetInput);
                        CheckGamepadStick("Down Stick", ref _dualAxisMethod.vertical.negativeGamePadAxis, foundUnsetInput);
                        CheckGamepadStick("Left Stick", ref _dualAxisMethod.horizontal.negativeGamePadAxis, foundUnsetInput);
                        CheckGamepadStick("Right Stick", ref _dualAxisMethod.horizontal.positiveGamePadAxis, foundUnsetInput);

                        _upSet = _dualAxisMethod.vertical.positiveGamePadAxis != GamePadInput.Axis.None;
                        _downSet = _dualAxisMethod.vertical.negativeGamePadAxis != GamePadInput.Axis.None;
                        _leftSet = _dualAxisMethod.horizontal.negativeGamePadAxis != GamePadInput.Axis.None;
                        _rightSet = _dualAxisMethod.horizontal.positiveGamePadAxis != GamePadInput.Axis.None;

                        if (_upSet == false || _downSet == false || _leftSet == false || _rightSet == false)
                            Repaint(); //ensures another GUI update next frame to check input again (because they weren't set this frame

                        break;
                    #endregion
                    #region MOUSE SCROLL DELTA
                    case InputMethod.InputType.MouseScrollDelta:
                        CheckMouseScrollDelta("Up Direction", ref _dualAxisMethod.vertical.positiveMouseDirection, foundUnsetInput);
                        CheckMouseScrollDelta("Down Direction", ref _dualAxisMethod.vertical.negativeMouseDirection, foundUnsetInput);
                        CheckMouseScrollDelta("Left Direction", ref _dualAxisMethod.horizontal.negativeMouseDirection, foundUnsetInput);
                        CheckMouseScrollDelta("Right Direction", ref _dualAxisMethod.horizontal.positiveMouseDirection, foundUnsetInput);

                        _upSet = _dualAxisMethod.vertical.positiveMouseDirection != Direction.None;
                        _downSet = _dualAxisMethod.vertical.negativeMouseDirection != Direction.None;
                        _leftSet = _dualAxisMethod.horizontal.negativeMouseDirection != Direction.None;
                        _rightSet = _dualAxisMethod.horizontal.positiveMouseDirection != Direction.None;

                        if (_upSet == false || _downSet == false || _leftSet == false || _rightSet == false)
                            Repaint(); //ensures another GUI update next frame to check input again (because they weren't set this frame

                        break;
                    #endregion
                    #region MOUSE POSITION DELTA
                    case InputMethod.InputType.MouseMove:
                        CheckMouseMove("Up Direction", ref _dualAxisMethod.vertical.positiveMouseDirection, foundUnsetInput);
                        CheckMouseMove("Down Direction", ref _dualAxisMethod.vertical.negativeMouseDirection, foundUnsetInput);
                        CheckMouseMove("Left Direction", ref _dualAxisMethod.horizontal.negativeMouseDirection, foundUnsetInput);
                        CheckMouseMove("Right Direction", ref _dualAxisMethod.horizontal.positiveMouseDirection, foundUnsetInput);

                        _upSet = _dualAxisMethod.vertical.positiveMouseDirection != Direction.None;
                        _downSet = _dualAxisMethod.vertical.negativeMouseDirection != Direction.None;
                        _leftSet = _dualAxisMethod.horizontal.negativeMouseDirection != Direction.None;
                        _rightSet = _dualAxisMethod.horizontal.positiveMouseDirection != Direction.None;

                        if (_upSet == false || _downSet == false || _leftSet == false || _rightSet == false)
                            Repaint(); //ensures another GUI update next frame to check input again (because they weren't set this frame

                        break;
                    #endregion
                    #region DEFAULT

                    default:
                        return;
                        #endregion
                }

                #endregion
                break;
        }

        #region CHANNEL SELECTION
        while (_inputMethod.sendOnChannels.Count != _event.channelCount)
        {
            if (_inputMethod.sendOnChannels.Count < _event.channelCount)
                _inputMethod.sendOnChannels.Add(false);
            else if (_inputMethod.sendOnChannels.Count > _event.channelCount)
                _inputMethod.sendOnChannels.RemoveAt(_inputMethod.sendOnChannels.Count - 1);
        }

        if (_event.channelCount > 1)
        {
            try
            {
                GUILayout.Space(4);
                if (_inputMethod.sendOnChannels.Count < 7)
                    GUILayout.BeginHorizontal();
                GUILayout.Label("Send To Channels ");

                int _additionalRows = (_inputMethod.sendOnChannels.Count / 11) + 1; // the number of additional rows that will be needed
                if (_additionalRows == 0) _additionalRows = 1;
                int _maxPerRow = (_inputMethod.sendOnChannels.Count / _additionalRows) + 1;

                for (int i = 0; i < _inputMethod.sendOnChannels.Count; i++)
                {
                    string _numString = (i + 1).ToString();
                    if ((i) % _maxPerRow == 0 || i == 0)
                    {
                        if (_inputMethod.sendOnChannels.Count >= 7)
                            GUILayout.BeginHorizontal();
                        _inputMethod.sendOnChannels[i] = GUILayout.Toggle(_inputMethod.sendOnChannels[i], _numString, EditorStyles.miniButtonLeft, GUILayout.MinWidth(0));
                    }
                    else if ((i + 1) % _maxPerRow == 0 || i == _inputMethod.sendOnChannels.Count - 1)
                    {
                        _inputMethod.sendOnChannels[i] = GUILayout.Toggle(_inputMethod.sendOnChannels[i], _numString, EditorStyles.miniButtonRight, GUILayout.MinWidth(0));
                        GUILayout.EndHorizontal();
                    }
                    else
                    {
                        _inputMethod.sendOnChannels[i] = GUILayout.Toggle(_inputMethod.sendOnChannels[i], _numString, EditorStyles.miniButtonMid, GUILayout.MinWidth(0));
                    }
                }
            }
            catch { }
        }
        else if(_event.channelCount == 1)
        {
            GUILayout.Space(8); GUILayout.BeginHorizontal();
            GUILayout.Label("Send To Channels ");
            _inputMethod.sendOnChannels[0] = GUILayout.Toggle(_inputMethod.sendOnChannels[0], "1", EditorStyles.miniButton, GUILayout.MinWidth(0));
            GUILayout.EndHorizontal();
        }
        #endregion
        #region DELAY FIELDS
        if (_event.axisType == InputEvent.AxisType.SingleAxis)
        {
            SingleAxisInputMethod _singleAxisMethod = _event.singleAxisInputMethods[_methodIndex];
            _singleAxisMethod.delay = DrawDelayField(_singleAxisMethod.inputType, _singleAxisMethod.delay, _event);
        }
        if (_event.axisType == InputEvent.AxisType.DualAxis)
        {
            DualAxisInputMethod _dualAxisMethod = _event.dualAxisInputMethods[_methodIndex];
            _dualAxisMethod.delay = DrawDelayField(_dualAxisMethod.inputType, _dualAxisMethod.delay, _event);
        }
        #endregion
    }

    private static float DrawDelayField(InputMethod.InputType _inputType, float _delay, InputEvent _event)
    {
        if (_inputType == InputMethod.InputType.Key || _inputType == InputMethod.InputType.MouseButton || _inputType == InputMethod.InputType.GamepadButton)
        {
            if (_delay != 0)
            {
                GUIStyle _explainTextStyle = new GUIStyle(EditorStyles.label);
                _explainTextStyle.normal.textColor = new Color(0f, 0f, 0f, 0.7f);
                _explainTextStyle.fontSize = 9;
                _explainTextStyle.margin = new RectOffset(0, 0, 3, 0);

                GUILayout.Space(4);
                string _timeToMax = "Time from Zero to Max/Min";
                if (_event.axisType == InputEvent.AxisType.SingleAxis && _event.hasNegativeInput == false)
                    _timeToMax = "Time from Zero to Max";
                GUILayout.BeginHorizontal();
                GUILayout.Label("Speed");
                GUILayout.Label("(" + _timeToMax + ")", _explainTextStyle);
                GUILayout.Space(10);
                _delay = EditorGUILayout.DelayedFloatField("", _delay, GUILayout.MinWidth(20));
                GUILayout.EndHorizontal();
                if (_delay < 0)
                    _delay = 0;
            }
        }

        return _delay;
    }

    private static void CheckKeyCode(string _inputName, ref KeyCode _keyCode, bool _disabled = false)
    {
        if (_keyCode != KeyCode.None)
        {
            if (DisplayInputValue(_inputName, FormatWithSpaces(_keyCode.ToString()), true)) //returns true if user wants to change the key
                _keyCode = KeyCode.None;
        }
        else if (_disabled)
        {
            DisplayInputValue(_inputName, "None", false, _disabled);
        }
        else
        {
            foundUnsetInput = true;

            DisplayInputValue(_inputName, "Press to set...", false);
            EditorGUI.FocusTextInControl(""); //unfocus keyboard (so event is not intercepted)

            if (UnityEngine.Event.current.shift && UnityEngine.Event.current.type == EventType.KeyDown) //special case for shift because reasons
                _keyCode = KeyCode.LeftShift;

            else if (UnityEngine.Event.current.keyCode != KeyCode.None && UnityEngine.Event.current.type == EventType.KeyDown)
            {
                _keyCode = UnityEngine.Event.current.keyCode;
                UnityEngine.Event.current.Use();
            }
        }   
    }
    private static void CheckMouseButton(string _inputName, ref KeyCode _keyCode, bool _disabled = false)
    {
        if (_keyCode != KeyCode.None)
        {
            if (DisplayInputValue(_inputName, FormatWithSpaces(_keyCode.ToString()), true)) //returns true if user wants to change the key
                _keyCode = KeyCode.None;
        }
        else if(_disabled)
        {
            DisplayInputValue(_inputName, "None", false, _disabled);
        }
        else
        {
            foundUnsetInput = true;

            DisplayInputValue(_inputName, "Click to set...", false);

            if (UnityEngine.Event.current.isMouse && UnityEngine.Event.current.type == EventType.MouseDown)
            {
                _keyCode = (KeyCode)UnityEngine.Event.current.button + 323;
                UnityEngine.Event.current.Use();
            }
        }
    }
    private static void CheckGamepadButton(string _inputName, ref GamePadInput.Button _gamePadButton, bool _disabled = false)
    {
        if (_gamePadButton != GamePadInput.Button.None)
        {
            if (DisplayInputValue(_inputName, FormatWithSpaces(_gamePadButton.ToString()), true)) //returns true if user wants to change the key
                _gamePadButton = GamePadInput.Button.None;
        }
        else if (_disabled)
        {
            DisplayInputValue(_inputName, "None", false, _disabled);
        }
        else
        {
            foundUnsetInput = true;

            DisplayInputValue(_inputName, "Press to set...", false);

            GamePadInput.Button _button = GamePadInput.Button.None;
            GetFirstActiveButton(out _button);
            _gamePadButton = _button;
        }
    }
    private static void CheckGamepadTrigger(string _inputName, ref GamePadInput.Axis _gamePadAxis, bool _disabled = false)
    {
        if (_gamePadAxis != GamePadInput.Axis.None)
        {
            if (DisplayInputValue(_inputName, FormatWithSpaces(_gamePadAxis.ToString()), true)) //returns true if user wants to change the key
                _gamePadAxis = GamePadInput.Axis.None;
        }
        else if (_disabled)
        {
            DisplayInputValue(_inputName, "None", false, _disabled);
        }
        else
        {
            foundUnsetInput = true;

            DisplayInputValue(_inputName, "Press to set...", false);

            GamePadInput.Axis _axis = GamePadInput.Axis.None;
            GetFirstActiveTrigger(out _axis);
            _gamePadAxis = _axis;
        }
    }
    private static void CheckGamepadStick(string _inputName, ref GamePadInput.Axis _gamePadAxis, bool _disabled = false)
    {
        if (_gamePadAxis != GamePadInput.Axis.None)
        {
            if (DisplayInputValue(_inputName, FormatWithSpaces(_gamePadAxis.ToString()), true)) //returns true if user wants to change the key
                _gamePadAxis = GamePadInput.Axis.None;
        }
        else if (_disabled)
        {
            DisplayInputValue(_inputName, "None", false, _disabled);
        }
        else
        {
            foundUnsetInput = true;

            DisplayInputValue(_inputName, "Move to set...", false);

            GamePadInput.Axis _axis = GamePadInput.Axis.None;
            GetFirstActiveStick(out _axis);
            _gamePadAxis = _axis;
        }
    }
    private static void CheckMouseScrollDelta(string _inputName, ref Direction _mouseDirection, bool _disabled = false)
    {
        if (_mouseDirection != Direction.None)
        {
            if (DisplayInputValue(_inputName, FormatWithSpaces(_mouseDirection.ToString()), true)) //returns true if user wants to change the key
                _mouseDirection = Direction.None;
        }
        else if (_disabled)
        {
            DisplayInputValue(_inputName, "None", false, _disabled);
        }
        else
        {
            foundUnsetInput = true;

            DisplayInputValue(_inputName, "Scroll to set...", false);

            if (UnityEngine.Event.current.type == EventType.ScrollWheel)
                _mouseDirection = UnityEngine.Event.current.delta.y < 0 ? Direction.Up : Direction.Down;
        }
    }
    private static void CheckMouseMove(string _inputName, ref Direction _mouseDirection, bool _disabled = false)
    {
        if (_mouseDirection != Direction.None)
        {
            if (DisplayInputValue(_inputName, FormatWithSpaces(_mouseDirection.ToString()), true)) //returns true if user wants to change the key
                _mouseDirection = Direction.None;
        }
        else if (_disabled)
        {
            DisplayInputValue(_inputName, "None", false, _disabled);
        }
        else
        {
            foundUnsetInput = true;

            DisplayInputValue(_inputName, "Press arrow key...", false, _disabled);
            EditorGUI.FocusTextInControl(""); //unfocus keyboard (so event is not intercepted)

            if (UnityEngine.Event.current.keyCode != KeyCode.None && UnityEngine.Event.current.type == EventType.KeyDown)
            {
                _mouseDirection = KeyToDirection(UnityEngine.Event.current.keyCode);
                UnityEngine.Event.current.Use();
            }
        }
    }

    private static int GetFirstActiveButton(out GamePadInput.Button _button) //returns the player index (-1 if not found) and the first active button
    {
        for (int i = 0; i < 4; i++)
        {
            GamePadState _state = currentState[i];
            GamePadState _previousState = previousState[i];

            if (_state.Buttons.A == ButtonState.Pressed && _previousState.Buttons.A == ButtonState.Released) { _button = GamePadInput.Button.A; return i; }
            if (_state.Buttons.B == ButtonState.Pressed && _previousState.Buttons.B == ButtonState.Released) { _button = GamePadInput.Button.B; return i; }
            if (_state.Buttons.X == ButtonState.Pressed && _previousState.Buttons.X == ButtonState.Released) { _button = GamePadInput.Button.X; return i; }
            if (_state.Buttons.Y == ButtonState.Pressed && _previousState.Buttons.Y == ButtonState.Released) { _button = GamePadInput.Button.Y; return i; }
            if (_state.Buttons.Start == ButtonState.Pressed && _previousState.Buttons.Start == ButtonState.Released) { _button = GamePadInput.Button.Start; return i; }
            if (_state.Buttons.Back == ButtonState.Pressed && _previousState.Buttons.Back == ButtonState.Released) { _button = GamePadInput.Button.Back; return i; }
            if (_state.Buttons.Guide == ButtonState.Pressed && _previousState.Buttons.Guide == ButtonState.Released) { _button = GamePadInput.Button.Guide; return i; }
            if (_state.Buttons.LeftShoulder == ButtonState.Pressed && _previousState.Buttons.LeftShoulder == ButtonState.Released) { _button = GamePadInput.Button.LeftShoulder; return i; }
            if (_state.Buttons.RightShoulder == ButtonState.Pressed && _previousState.Buttons.RightShoulder == ButtonState.Released) { _button = GamePadInput.Button.RightShoulder; return i; }
            if (_state.Buttons.LeftStick == ButtonState.Pressed && _previousState.Buttons.LeftStick == ButtonState.Released) { _button = GamePadInput.Button.LeftStick; return i; }
            if (_state.Buttons.RightStick == ButtonState.Pressed && _previousState.Buttons.RightStick == ButtonState.Released) { _button = GamePadInput.Button.RightStick; return i; }
            if (_state.DPad.Up == ButtonState.Pressed && _previousState.DPad.Up == ButtonState.Released) { _button = GamePadInput.Button.DpadUp; return i; }
            if (_state.DPad.Down == ButtonState.Pressed && _previousState.DPad.Down == ButtonState.Released) { _button = GamePadInput.Button.DpadDown; return i; }
            if (_state.DPad.Left == ButtonState.Pressed && _previousState.DPad.Left == ButtonState.Released) { _button = GamePadInput.Button.DpadLeft; return i; }
            if (_state.DPad.Right == ButtonState.Pressed && _previousState.DPad.Right == ButtonState.Released) { _button = GamePadInput.Button.DpadRight; return i; }
        }

        //if nothing was active
        _button = GamePadInput.Button.None;
        return -1;
    }
    private static int GetFirstActiveTrigger(out GamePadInput.Axis _trigger) //returns the player index (-1 if not found) and the first active trigger
    {
        for (int i = 0; i < 4; i++)
        {
            GamePadState _state = currentState[i];
            GamePadState _previousState = previousState[i];

            if (_state.Triggers.Left > 0.2f && _previousState.Triggers.Left <= 0.2f) { _trigger = GamePadInput.Axis.LeftTrigger; return i; }
            if (_state.Triggers.Right > 0.2f && _previousState.Triggers.Right <= 0.2f) { _trigger = GamePadInput.Axis.RightTrigger; return i; }
        }

        //if nothing was active
        _trigger = GamePadInput.Axis.None;
        return -1;
    }
    private static int GetFirstActiveStick(out GamePadInput.Axis _stick) //returns the player index (-1 if not found) and the first active stick
    {
        for (int i = 0; i < 4; i++)
        {
            GamePadState _state = currentState[i];
            GamePadState _previousState = previousState[i];

            if (_state.ThumbSticks.Left.X < -0.2f && _previousState.ThumbSticks.Left.X >= -0.2f) { _stick = GamePadInput.Axis.LeftStickLeft; return i; }
            if (_state.ThumbSticks.Left.X > 0.2f && _previousState.ThumbSticks.Left.X <= 0.2f) { _stick = GamePadInput.Axis.LeftStickRight; return i; }
            if (_state.ThumbSticks.Left.Y > 0.2f && _previousState.ThumbSticks.Left.Y <= 0.2f) { _stick = GamePadInput.Axis.LeftStickUp; return i; }
            if (_state.ThumbSticks.Left.Y < -0.2f && _previousState.ThumbSticks.Left.Y >= -0.2f) { _stick = GamePadInput.Axis.LeftStickDown; return i; }

            if (_state.ThumbSticks.Right.X < -0.2f && _previousState.ThumbSticks.Right.X >= -0.2f) { _stick = GamePadInput.Axis.RightStickLeft; return i; }
            if (_state.ThumbSticks.Right.X > 0.2f && _previousState.ThumbSticks.Right.X <= 0.2f) { _stick = GamePadInput.Axis.RightStickRight; return i; }
            if (_state.ThumbSticks.Right.Y > 0.2f && _previousState.ThumbSticks.Right.Y <= 0.2f) { _stick = GamePadInput.Axis.RightStickUp; return i; }
            if (_state.ThumbSticks.Right.Y < -0.2f && _previousState.ThumbSticks.Right.Y >= -0.2f) { _stick = GamePadInput.Axis.RightStickDown; return i; }
        }

        //if nothing was active
        _stick = GamePadInput.Axis.None;
        return -1;
    }
    private static Direction KeyToDirection(KeyCode _keyCode) //returns the supplied arrow key as a direction
    {
        switch (_keyCode)
        {
            case KeyCode.LeftArrow: return Direction.Left;
            case KeyCode.RightArrow: return Direction.Right;
            case KeyCode.UpArrow: return Direction.Up;
            case KeyCode.DownArrow: return Direction.Down;
            default: return Direction.None;
        }
    }

    private static string FormatWithSpaces(string _string)
    {
        for (int i = 1; i < _string.Length; i++)
        {
            bool _isUpperOrNum = char.IsUpper(_string[i]) || char.IsNumber(_string[i]);
            bool _previousIsUpperOrNum = char.IsUpper(_string[i - 1]) || char.IsNumber(_string[i - 1]);
            if (_isUpperOrNum && !_previousIsUpperOrNum)
            {
                _string = _string.Insert(i, " ");
                i++;
            }
        }

        return _string;
    }

    private static void DisplayThreshold(BinaryInputMethod _binaryMethod)
    {
        try
        {
            GUILayout.Space(6);

            GUIStyle _labelStyle4 = new GUIStyle(EditorStyles.label);
            _labelStyle4.contentOffset = new Vector2(0, -1);

            GUIStyle _labelStyle = new GUIStyle(EditorStyles.boldLabel);
            _labelStyle.contentOffset = new Vector2(0, -7);
            _labelStyle.fontSize = 12;

            GUIStyle _labelStyle2 = new GUIStyle(EditorStyles.boldLabel);
            _labelStyle2.normal.textColor = new Color(0.2f, 0.2f, 0.2f);
            _labelStyle2.contentOffset = new Vector2(0, -6);
            _labelStyle2.fontSize = 11;

            GUIStyle _labelStyle3 = new GUIStyle(EditorStyles.numberField);
            _labelStyle3.margin = new RectOffset(0, 5, 0, 0);
            
            GUILayout.BeginHorizontal();
            GUILayout.Label("Active If", _labelStyle4, GUILayout.MinWidth(96));
            GUILayout.FlexibleSpace();
            GUILayout.Label("Input Value", _labelStyle2);
            GUILayout.Label(" > ", _labelStyle);
            _binaryMethod.threshold = EditorGUILayout.FloatField(_binaryMethod.threshold, _labelStyle3, GUILayout.MinWidth(24), GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth * 0.4f));

            GUILayout.EndHorizontal();
        }
        catch { }
    }
    private static void DisplaySingleAxisThreshold(InputEvent _event)
    {
        try
        {
            GUILayout.Space(4);

            GUIStyle _box = new GUIStyle("GroupBox");
            _box.padding = new RectOffset(0, 0, 5, 4);
            _box.margin = new RectOffset(0, 0, 0, 0);

            GUIStyle _labelStyle = new GUIStyle(EditorStyles.boldLabel);
            _labelStyle.contentOffset = new Vector2(0, -7);
            _labelStyle.fontSize = 12;

            GUIStyle _labelStyle2 = new GUIStyle(EditorStyles.label);
            _labelStyle2.contentOffset = new Vector2(0, -2);
            _labelStyle2.fontSize = 11;

            GUIStyle _labelStyle3 = new GUIStyle(EditorStyles.numberField);
            _labelStyle3.alignment = TextAnchor.MiddleCenter;
            _labelStyle3.margin = new RectOffset(0, 5, 0, 0);

            GUIStyle _labelStyle4 = new GUIStyle(EditorStyles.label);
            _labelStyle4.normal.textColor = new Color(0.2f, 0.2f, 0.2f);
            _labelStyle4.fontSize = 10;

            GUIStyle _labelStyle5 = new GUIStyle(EditorStyles.boldLabel);
            _labelStyle5.fontSize = 11;

            GUILayout.BeginVertical(_box);
            GUILayout.Label("Active If", _labelStyle5);
            GUILayout.BeginHorizontal();

            GUILayout.Label("Input Value", _labelStyle2);
            GUILayout.Label(" > ", _labelStyle);
            _event.threshold = EditorGUILayout.FloatField(_event.threshold, _labelStyle3, GUILayout.MinWidth(24));

            GUILayout.Label(" OR", _labelStyle2);
            GUILayout.Label(" < ", _labelStyle);
            GUILayout.Label((_event.threshold == 0 ? "" : "-") + _event.threshold + " ", _labelStyle2);

            GUILayout.EndHorizontal();
            GUILayout.Space(2);
            GUILayout.EndVertical();
        }
        catch { }
    }
    private static void DisplayDualAxisThreshold(InputEvent _event)
    {
        try
        {
            GUILayout.Space(4);

            GUIStyle _box = new GUIStyle("GroupBox");
            _box.padding = new RectOffset(0, 0, 5, 4);
            _box.margin = new RectOffset(0, 0, 0, 0);

            GUIStyle _labelStyle = new GUIStyle(EditorStyles.boldLabel);
            _labelStyle.contentOffset = new Vector2(0, -7);
            _labelStyle.fontSize = 12;

            GUIStyle _labelStyle2 = new GUIStyle(EditorStyles.label);
            _labelStyle2.contentOffset = new Vector2(0, -2);
            _labelStyle2.fontSize = 11;

            GUIStyle _labelStyle3 = new GUIStyle(EditorStyles.numberField);
            _labelStyle3.alignment = TextAnchor.MiddleCenter;
            _labelStyle3.margin = new RectOffset(0, 5, 0, 0);

            GUIStyle _labelStyle4 = new GUIStyle(EditorStyles.label);
            _labelStyle4.normal.textColor = new Color(0.2f, 0.2f, 0.2f);
            _labelStyle4.fontSize = 10;

            GUIStyle _labelStyle5 = new GUIStyle(EditorStyles.boldLabel);
            _labelStyle5.fontSize = 11;

            GUILayout.BeginVertical(_box);
            GUILayout.Label("Active If", _labelStyle5);
            GUILayout.BeginHorizontal();

            switch (_event.thresholdType)
            {
                case InputEvent.ThresholdType.Square:
                    GUILayout.Label(EditorGUIUtility.currentViewWidth > 315 ? "Input Value (X or Y)" : "Input (X or Y)", _labelStyle2);
                    break;
                default:
                    GUILayout.Label(EditorGUIUtility.currentViewWidth > 315 ? "Input Value (Magnitude)" : "Input (Magnitude)", _labelStyle2);
                    break;
            }
            GUILayout.Label(" > ", _labelStyle);
            _event.threshold = EditorGUILayout.FloatField(_event.threshold, _labelStyle3, GUILayout.MinWidth(24));

            if(_event.thresholdType == InputEvent.ThresholdType.Square)
            {
                GUILayout.Label(" OR", _labelStyle2);
                GUILayout.Label(" < ", _labelStyle);
                GUILayout.Label((_event.threshold == 0 ? "" : "-") + _event.threshold + " ", _labelStyle2);
            }

            GUILayout.EndHorizontal();

            GUIStyle _typeButtonStyleLeft = new GUIStyle(EditorStyles.miniButtonLeft);
            if (_event.thresholdType == InputEvent.ThresholdType.Square)
                _typeButtonStyleLeft.normal = _typeButtonStyleLeft.onActive;

            GUIStyle _typeButtonStyleRight = new GUIStyle(EditorStyles.miniButtonRight);
            if (_event.thresholdType == InputEvent.ThresholdType.Radial)
                _typeButtonStyleRight.normal = _typeButtonStyleRight.onActive;

            GUILayout.Space(2);

            GUILayout.BeginHorizontal();
            //GUILayout.Label("Threshold Type", _labelStyle4);
            if (GUILayout.Button("Square", _typeButtonStyleLeft))
            {
                _event.thresholdType = InputEvent.ThresholdType.Square;
            }
            if (GUILayout.Button("Radial", _typeButtonStyleRight))
            {
                _event.thresholdType = InputEvent.ThresholdType.Radial;
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(2);
            GUILayout.EndVertical();
        }
        catch { }
    }
    private static bool DisplayInputValue(string _label, string _value, bool _valueSet, bool _disabled = false) //returns whether or not to reset the value
    {
        GUILayout.Space(1);
        GUILayout.BeginHorizontal();
        EditorGUI.BeginDisabledGroup(_disabled);

        if (_disabled)
        {
            _valueSet = true;
            _value = "None";
        }

        GUIStyle _boldLableStyle = new GUIStyle(EditorStyles.boldLabel);
        _boldLableStyle.contentOffset = new Vector2(0, -4);
        GUIStyle _labelStyle = new GUIStyle(EditorStyles.label);
        _labelStyle.contentOffset = new Vector2(0, 3);
        //if(!_valueSet) _lableStyle.normal.textColor = Color.blue;
        
        GUILayout.Label(_label, _labelStyle, GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.35f));

        GUILayout.FlexibleSpace();
        
        GUIStyle _valueLabelStyle = _valueSet ? new GUIStyle(EditorStyles.textArea) : new GUIStyle(EditorStyles.objectFieldThumb);
        _valueLabelStyle.margin = new RectOffset(0, 0, 3, 0);
        _valueLabelStyle.alignment = TextAnchor.MiddleCenter;
        if(!_valueSet) _valueLabelStyle.normal.textColor = Color.white;

        GUILayout.Label(_value, _valueLabelStyle, GUILayout.MinWidth(20), GUILayout.Height(18), GUILayout.MaxWidth(float.MaxValue));
        GUILayout.FlexibleSpace();

        GUIStyle _changeButtonStyle = new GUIStyle(EditorStyles.miniButtonRight);
        _changeButtonStyle.margin = new RectOffset(0, 4, 3, 0);

        if (!_valueSet)
            _changeButtonStyle.normal.background = _changeButtonStyle.active.background;
        
        bool _resetValue =  GUILayout.Button("Change", _changeButtonStyle, GUILayout.Height(18));
        if(_resetValue) EditorGUI.FocusTextInControl(""); //unfocus the input method selection

        EditorGUI.EndDisabledGroup();
        GUILayout.EndHorizontal();
        return _resetValue;
    }
}