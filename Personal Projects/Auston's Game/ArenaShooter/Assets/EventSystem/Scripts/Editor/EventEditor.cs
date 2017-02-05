using UnityEngine;
using UnityEditor;
//using System.Collections.Generic;
using System.IO;

public class EventEditor : EditorWindow
{
    private int selectedEvent = 0;
    private int selectedInputEvent = 0;
    private Vector2 eventScrollPos = Vector2.zero;
    private Vector2 inputEventScrollPos = Vector2.zero;
    private static bool autoWriteChanges = false;
    private static bool eventsNeedWritten = false;
    public bool autoWrite; //serialized
    public bool needsWrite; //serialized

    #region MENU ITEMS
    [MenuItem("Window/Events")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        EventEditor window = (EventEditor)EditorWindow.GetWindow(typeof(EventEditor), false, "Events");
        window.Show();
    }

    [MenuItem("Events/Create Event")]
    static void AddEvent() { CreateNewEvent(); }

    [MenuItem("Events/Create Input Event")]
    static void AddInputEvent() { CreateNewInputEvent(); }

    [MenuItem("Events/Find Thing")]
    static void FindThing()
    {
        Debug.Log(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("TestFolder")[0]));
    }
    #endregion

    public EventEditor()
    {
        //EditorApplication.update -= OnUpdate;
        //EditorApplication.update += OnUpdate;
        Selection.selectionChanged += OnSelectionChanged;
        autoWriteChanges = autoWrite;
        eventsNeedWritten = needsWrite;
    }
    void Update()
    {
    }

    //bool testBool = false;

    void OnSelectionChanged()
    {
        Event[] _eventList = GetObjectList<Event>();
        bool _eventIsSelected = false;
        for (int i = 0; i < _eventList.Length; i++)
        {
            if (Selection.activeObject == _eventList[i])
            {
                selectedEvent = i;
                _eventIsSelected = true;
            }
        }
        if (!_eventIsSelected)
        {
            selectedEvent = -1;
        }

        InputEvent[] _inputEventList = GetObjectList<InputEvent>();
        bool _inputEventIsSelected = false;
        for (int i = 0; i < _inputEventList.Length; i++)
        {
            if (Selection.activeObject == _inputEventList[i])
            {
                selectedInputEvent = i;
                _inputEventIsSelected = true;
            }
        }
        if (!_inputEventIsSelected)
        {
            selectedInputEvent = -1;
        }

        Repaint();
    }

    void OnGUI()
    {
        CheckDeleteEvent();
        Toolbar();

        GUILayout.Label("Generic Events", EditorStyles.boldLabel);
        EventList();
        GUILayout.Label("Input Events", EditorStyles.boldLabel);
        InputEventList();
    }

    /// <summary> Creates the Generic Event List inside the window. </summary>
    private void EventList()
    {
        GUIStyle _normalStyle = GetNormalStyle();
        GUIStyle _selectedStyle = GetSelectedStyle();

        GUILayout.BeginVertical(EditorStyles.helpBox);
        eventScrollPos = GUILayout.BeginScrollView(eventScrollPos, false, false, new GUILayoutOption[] { });
        
        Event[] _eventList = GetObjectList<Event>();

        for (int i = 0; i < _eventList.Length; i++)
        {
            string _eventLabel = _eventList[i].name;
            if (_eventList[i].parameters.Count > 0)
                _eventLabel += "<color=#4A4A4AFF>" + " - (" + _eventList[i].GetParamTypeNameList(false) + ")</color>";

            if (GUILayout.Button(_eventLabel, i == selectedEvent ? _selectedStyle : _normalStyle))
            {
                //event number 'i' selected
                selectedEvent = i;
                Selection.activeObject = _eventList[i];//AssetDatabase.LoadAssetAtPath(_pathList[selectedEvent], typeof(Event));
            }
        }

        #region KEY CONTROLS (UP/DOWN + F2)
        if (selectedEvent != -1)
        {
            UnityEngine.Event _e = UnityEngine.Event.current;

            if (_e.type == EventType.KeyDown && _e.keyCode == KeyCode.UpArrow)
            {
                --selectedEvent;
                if (selectedEvent < 0)
                {
                    InputEvent[] _inputEventList = GetObjectList<InputEvent>();
                    selectedInputEvent = _inputEventList.Length - 1;
                    Selection.activeObject = _inputEventList[selectedInputEvent];
                }
                else
                    Selection.activeObject = _eventList[selectedEvent];

                _e.Use();
            }
            if (_e.type == EventType.KeyDown && _e.keyCode == KeyCode.DownArrow)
            {
                ++selectedEvent;
                if (selectedEvent > _eventList.Length - 1)
                {
                    InputEvent[] _inputEventList = GetObjectList<InputEvent>();
                    selectedInputEvent = 0;
                    Selection.activeObject = _inputEventList[selectedInputEvent];
                }
                else
                    Selection.activeObject = _eventList[selectedEvent];

                _e.Use();
            }

            if (_e.type == EventType.KeyDown && _e.keyCode == KeyCode.F2)
            {
                Debug.Log("Set");
                _eventList[selectedEvent].setControlToName = true;
            }
        }
        #endregion

        GUILayout.EndScrollView();
        GUILayout.EndVertical();
    }

    private GUIStyle GetNormalStyle()
    {
        GUIStyle _normalStyle = new GUIStyle();
        _normalStyle.padding = new RectOffset(11, 0, 2, 1);
        return _normalStyle;
    }
    private GUIStyle GetSelectedStyle()
    {
        Texture2D _bgTexture = new Texture2D(1, 1);
        if (focusedWindow == this) _bgTexture.SetPixel(0, 0, new Color(0.243f, 0.490f, 0.905f, 1)); //blue if window is focused
        else _bgTexture.SetPixel(0, 0, new Color(0.561f, 0.561f, 0.561f, 1)); //gray if window is out of focus
        _bgTexture.Apply(); //applying the color change to the texture

        GUIStyle _selectedStyle = new GUIStyle();
        _selectedStyle.normal.background = _bgTexture;
        _selectedStyle.normal.textColor = Color.white;
        _selectedStyle.richText = true;
        _selectedStyle.padding = new RectOffset(11, 0, 2, 1);

        return _selectedStyle;
    }

    private void InputEventList()
    {
        GUIStyle _normalStyle = GetNormalStyle();
        GUIStyle _selectedStyle = GetSelectedStyle();

        GUILayout.BeginVertical(EditorStyles.helpBox);
        inputEventScrollPos = GUILayout.BeginScrollView(inputEventScrollPos, false, false, new GUILayoutOption[] { });

        InputEvent[] _inputEventList = GetObjectList<InputEvent>();

        for (int i = 0; i < _inputEventList.Length; i++)
        {
            if (_inputEventList[i] == null) continue;
            string _eventLabel = _inputEventList[i].name;
            if (GUILayout.Button(_eventLabel, i == selectedInputEvent ? _selectedStyle : _normalStyle))
            {
                //event number 'i' selected
                selectedInputEvent = i;
                Selection.activeObject = _inputEventList[i];//AssetDatabase.LoadAssetAtPath(_pathList[selectedEvent], typeof(Event));
            }
        }

        #region KEY UP/DOWN
        if (selectedInputEvent != -1)
        {
            UnityEngine.Event _e = UnityEngine.Event.current;
            if (_e.type == EventType.KeyDown && _e.keyCode == KeyCode.UpArrow)
            {
                --selectedInputEvent;
                if(selectedInputEvent < 0)
                {
                    Event[] _eventList = GetObjectList<Event>();
                    selectedEvent = _eventList.Length - 1;
                    Selection.activeObject = _eventList[selectedEvent];
                }
                else
                    Selection.activeObject = _inputEventList[selectedInputEvent];
            }
            if (_e.type == EventType.KeyDown && _e.keyCode == KeyCode.DownArrow)
            {
                ++selectedInputEvent;
                if (selectedInputEvent > _inputEventList.Length - 1)
                {
                    Event[] _eventList = GetObjectList<Event>();
                    selectedEvent = 0;
                    Selection.activeObject = _eventList[selectedEvent];
                }
                else
                    Selection.activeObject = _inputEventList[selectedInputEvent];
            }
        }
        #endregion

        GUILayout.EndScrollView();
        GUILayout.EndVertical();
    }

    /// <summary> Creates the toolbar at the top of the window. </summary>
    private static void Toolbar()
    {
        GUILayout.BeginHorizontal();

        GUILayout.Label("", EditorStyles.toolbar, GUILayout.Width(7)); //Left margin

        if (GUILayout.Button("New Event", EditorStyles.toolbarButton, GUILayout.Width(66))) //Create event button
            CreateNewEvent();

        GUILayout.Label("", EditorStyles.toolbarButton, GUILayout.ExpandWidth(true)); //Middle filler

        GUIStyle _writeChangesStyle = new GUIStyle(EditorStyles.toolbarButton);
        if(eventsNeedWritten)
        {
            //_writeChangesStyle.normal.textColor = Color.red;
            _writeChangesStyle.fontStyle = FontStyle.Bold;
        }

        if (GUILayout.Button(eventsNeedWritten ? "Write Changes*" : "Write Changes", _writeChangesStyle, GUILayout.Width(88))) //Write events button
            WriteEvents(false);

        if (GUILayout.Button("Open Script", EditorStyles.toolbarButton, GUILayout.Width(70))) //Open script button
        {
            string[] _GUIDs = AssetDatabase.FindAssets("Events t:MonoScript");
            if (_GUIDs.Length == 0)
            {
                Debug.Log("Cannot find event script. Script named \"Events\" must exist.");
                return;
            }
            string _path = AssetDatabase.GUIDToAssetPath(_GUIDs[0]);
            Object _object = AssetDatabase.LoadAssetAtPath<Object>(_path);
            AssetDatabase.OpenAsset(_object);
        }

        GUILayout.Label("", EditorStyles.toolbarButton, GUILayout.Width(7)); //Right margin

        GUILayout.EndHorizontal();
    }

    /// <summary> Checks if the user is attempting to delete the event. </summary>
    private void CheckDeleteEvent()
    {
        UnityEngine.Event _e = UnityEngine.Event.current;
        if (_e != null && _e.type == EventType.keyDown && _e.keyCode == KeyCode.Delete)
        {
            string _title = "Delete selected event?";
            string _body = "Are you sure you want to delete this event? This cannot be undone.";
            string _confirmation = "Delete Event";
            string _cancelation = "Cancel";

            bool _continue = EditorUtility.DisplayDialog(_title, _body, _confirmation, _cancelation);
            if (_continue)
            {
                string _path = AssetDatabase.GetAssetPath(Selection.activeObject);
                AssetDatabase.DeleteAsset(_path);
                AssetDatabase.Refresh();
                WriteEvents(true);
            }
        }
    }

    private static T[] GetObjectList<T>() where T : Object
    {
        string _typeName = typeof(T).ToString();
        string[] _GUIDs = AssetDatabase.FindAssets("t:" + _typeName);
        T[] _objectList = new T[_GUIDs.Length];
        string[] _pathList = new string[_GUIDs.Length];

        for (int i = 0; i < _GUIDs.Length; i++)
        {
            string _assetPath = AssetDatabase.GUIDToAssetPath(_GUIDs[i]);
            _pathList[i] = _assetPath;

            T _object = AssetDatabase.LoadAssetAtPath<T>(_assetPath);
            _objectList[i] = _object;
        }

        return _objectList;
    } //returns a list of the type objects and the paths to them
    private static int GetObjectCount<T>()
    {
        string _typeName = typeof(T).ToString();
        string[] _GUIDs = AssetDatabase.FindAssets("t:" + _typeName);
        return _GUIDs.Length;
    }
    
    private static void CreateNewEvent()
    {
        Event _event = ScriptableObject.CreateInstance<Event>();

        string _path = "Assets/EventSystem/Resources/Events";
        string _assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(_path + "/New " + typeof(Event).ToString() + ".asset");

        AssetDatabase.CreateAsset(_event, _assetPathAndName);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Selection.activeObject = _event;

        WriteEvents(true);
    }
    private static void CreateNewInputEvent()
    {
        InputEvent _event = ScriptableObject.CreateInstance<InputEvent>();

        string _path = "Assets/EventSystem/Resources/Events";
        string _assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(_path + "/New InputEvent.asset");

        AssetDatabase.CreateAsset(_event, _assetPathAndName);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Selection.activeObject = _event;

        WriteEvents(true);
    }
    public static void WriteEvents(bool _isAutoWrite)
    {
        if (_isAutoWrite && !autoWriteChanges)
        {
            eventsNeedWritten = true;
            return;
        }

        try
        {
            StreamWriter _writer = new StreamWriter(Application.dataPath + @"\EventSystem\Scripts\Events.cs");
            Event[] _eventList = GetObjectList<Event>();
            InputEvent[] _inputEventList = GetObjectList<InputEvent>();

            _writer.WriteLine("namespace Events");
            _writer.WriteLine("{");

            foreach (Event _event in _eventList)
            {
                if (!_event.validParamNames || !_event.validParamTypes) //if invalid param names or types, do not write
                    continue;

                _writer.WriteLine("    public static class " + _event.codeFriendlyName);
                _writer.WriteLine("    {");
                _writer.WriteLine("        public delegate void SubscripionDelegate(" + _event.GetParamTypeNameList() + ");");
                _writer.WriteLine("        private static SubscripionDelegate subscriptions = delegate{ };");
                _writer.WriteLine("");
                _writer.WriteLine("        /// <summary>");
                _writer.WriteLine("        /// Subscribes to this event so that when the event is sent the supplied function is called.");
                _writer.WriteLine("        /// Requires a function which takes" + (_event.parameters.Count > 0 ? (": " + _event.GetParamTypeNameList()) : " no parameters."));
                _writer.WriteLine("        /// </summary>");
                _writer.WriteLine("        /// <param name=\"_eventFunction\"></param>");
                _writer.WriteLine("        public static void Subscribe(SubscripionDelegate _eventFunction)");
                _writer.WriteLine("        {");
                _writer.WriteLine("            subscriptions += _eventFunction;");
                _writer.WriteLine("        }");
                _writer.WriteLine("        /// <summary>");
                _writer.WriteLine("        /// Unsubscribes from this event so that when the event is sent the supplied function is no longer called.");
                _writer.WriteLine("        /// Requires a function which takes" + (_event.parameters.Count > 0 ? (": " + _event.GetParamTypeNameList()) : " no parameters."));
                _writer.WriteLine("        /// </summary>");
                _writer.WriteLine("        /// <param name=\"_eventFunction\"></param>");
                _writer.WriteLine("        public static void Unsubscribe(SubscripionDelegate _eventFunction)");
                _writer.WriteLine("        {");
                _writer.WriteLine("            subscriptions -= _eventFunction;");
                _writer.WriteLine("        }");
                _writer.WriteLine("        /// <summary>");
                _writer.WriteLine("        /// Sends the event, calling all functions which are subscribe to this event.");
                _writer.WriteLine("        /// </summary>");
                _writer.WriteLine("        /// <param name=\"_eventFunction\"></param>");
                _writer.WriteLine("        public static void Send(" + _event.GetParamTypeNameList() + ")");
                _writer.WriteLine("        {");
                _writer.WriteLine("            subscriptions.Invoke(" + _event.GetParamNameList() + ");");
                _writer.WriteLine("        }");
                _writer.WriteLine("    }");
            }

            _writer.WriteLine("}"); //closing the namespace

            _writer.WriteLine("namespace InputEvents");
            _writer.WriteLine("{");

            _writer.WriteLine(@"    public class SubscriptionData
    {
        public virtual void Send(InputEventInfo _inputEventInfo) { }
        public virtual void Send(InputEventInfo _inputEventInfo, int _channel) { }

        internal static bool ChannelIsInvalid(int _channel, int _channelSubscriptionsCount)
        {
            if (_channel < 0 || _channel > _channelSubscriptionsCount - 1)
            {
                UnityEngine.Debug.Log(""Channel '"" + _channel.ToString() + ""' is not a valid channel."");
                return true;
            }
            return false;
        }
    }");

            foreach (InputEvent _inputEvent in _inputEventList)
            {
                string _delegateList = "";
                for (int i = 0; i < _inputEvent.channelCount; i++)
                {
                    _delegateList += " delegate { }";
                    if (i != _inputEvent.channelCount - 1)
                        _delegateList += ",";
                }

                _writer.WriteLine(@"    public class " + _inputEvent.name + @" : SubscriptionData
    {
        public delegate void SubscripionDelegate(InputEventInfo _inputEventInfo);
        private static SubscripionDelegate[] channelSubscriptions = new SubscripionDelegate[] { " + _delegateList + @" }; //for channel-specific Subscriptions
        private static SubscripionDelegate subscriptions = delegate { }; //for all other subscriptions (that don't care about channel)
        private static int channelCount { get { return channelSubscriptions.Length; } }

        /// <summary> Subscribes to this event so that when the event is sent the supplied function is called. Requires a function which takes: float deltaTime </summary>
        /// <param name=""_eventFunction"">The function that will be called when the event is sent.</param>
        public static void Subscribe(SubscripionDelegate _eventFunction) { subscriptions += _eventFunction; }
        /// <summary> Subscribes to this event so that when the event is sent the supplied function is called. </summary>
        /// <param name=""_eventFunction"">The function that will be called when the event is sent. The function must take an 'InputEventInfo'</param>
        /// <param name=""_channel"">The channel that the event will need to be called on for the function to be called.</param>
        public static void Subscribe(SubscripionDelegate _eventFunction, int _channel) { if (ChannelIsInvalid(_channel, channelCount)) return; channelSubscriptions[_channel] += _eventFunction; }

        /// <summary> Unsubscribes from this event so that when the event is sent the supplied function is no longer called. Requires a function which takes: float deltaTime </summary>
        /// <param name=""_eventFunction"">The function that would be called when the event was sent.</param>
        public static void Unsubscribe(SubscripionDelegate _eventFunction) { subscriptions -= _eventFunction; }
        /// <summary> Unsubscribes from this event so that when the event is sent the supplied function is no longer called. Requires a function which takes: float deltaTime </summary>
        /// <param name=""_eventFunction"">The function that would be called when the event was sent. The function must take an 'InputEventInfo'</param>
        /// <param name=""_channel"">The channel that the function was subscribed on.</param>
        public static void Unsubscribe(SubscripionDelegate _eventFunction, int _channel) { if (ChannelIsInvalid(_channel, channelCount)) return; channelSubscriptions[_channel] -= _eventFunction; }

        /// <summary> Sends the event, calling all functions which are subscribe to this event. </summary>
        /// <param name=""_inputEventInfo"">The information that will be sent with the input event.</param>
        public override void Send(InputEventInfo _inputEventInfo) { subscriptions.Invoke(_inputEventInfo); }
        /// <summary> Sends the event, calling all functions which are subscribe to this event. </summary>
        /// <param name=""_inputEventInfo"">The information that will be sent with the input event.</param>
        /// /// <param name=""_channel"">The channel that the event will be sent on.</param>
        public override void Send(InputEventInfo _inputEventInfo, int _channel) { if (ChannelIsInvalid(_channel, channelCount)) return; channelSubscriptions[_channel].Invoke(_inputEventInfo); }
    } ");
            }

            _writer.WriteLine("}"); //closing the namespace

            //Close the file
            _writer.Close();

            string[] _GUIDs = AssetDatabase.FindAssets("Events t:MonoScript");
            if (_GUIDs.Length == 0)
            {
                Debug.Log("Cannot find event script. Script named \"Events\" must exist.");
                return;
            }
            string _path = AssetDatabase.GUIDToAssetPath(_GUIDs[0]);
            AssetDatabase.ImportAsset(_path);
        }
        catch (System.Exception e)
        {
            Debug.Log("Exception: " + e.Message);
        }
    }
}

/// <summary> Used to update the event list if an event is deleted through the project window. </summary>
public class DeletionPostProcessor : AssetPostprocessor
{
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        if (deletedAssets.Length > 0)
        {
            EditorWindow.GetWindow<EventEditor>().Repaint();
            EventEditor.WriteEvents(true);
        }
    }
}