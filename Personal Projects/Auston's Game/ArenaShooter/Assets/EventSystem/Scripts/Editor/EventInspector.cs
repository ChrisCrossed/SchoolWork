using UnityEditor;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;

[CustomEditor(typeof(Event))]
public class EventInspector : Editor
{
    private string newName;

    private Vector2 scrollPos;
    private ParameterInfo currentUnedited = null;
    private bool wasCompiling;

    private double lastClipboardCopy = 0;

    EventInspector()
    {
        Selection.selectionChanged += OnSelectionChanged;
        EditorApplication.update += OnEditorUpdate;
    }

    public void OnEditorUpdate()
    {
        if (wasCompiling && !EditorApplication.isCompiling)
            OnFinishCompile();

        wasCompiling = EditorApplication.isCompiling;
    }

    private void OnFinishCompile()
    {
        //Debug.Log("Finished compile");

        Event _event = (Event)target; //the scriptable object as an Event
        if (_event.paramEditIndex == -1/* || _event.paramEditIndex > _event.parameters.Count - 1*/) return;
        //Debug.Log(_event.paramEditIndex);
        ParameterInfo _parameterInfo = _event.parameters[_event.paramEditIndex];
        _parameterInfo.paramTypeErrorMessage = CheckIfParameterTypeIsValid(_parameterInfo);
    }

    public void OnSelectionChanged()
    {
        Event _event = (Event)target; //the scriptable object as an Event
        CancelEdit(_event);
    }

    public void DrawNameField(Event _event)
    {
        GUI.SetNextControlName("Event Name");
        newName = EditorGUILayout.TextField("Event Name", newName);

        if (_event.setControlToName)
        {
            EditorGUI.FocusTextInControl("Event Name");
            Debug.Log("Doing it!");
            _event.setControlToName = false;
        }

        bool _lostFocus = GUI.GetNameOfFocusedControl() != "Event Name";

        UnityEngine.Event _unityEvent = UnityEngine.Event.current;
        bool _hitEnter = _unityEvent.type == EventType.KeyUp && _unityEvent.keyCode == KeyCode.Return;

        if (newName != null && newName != target.name && (_lostFocus || _hitEnter))
        {
            string _assetPath = AssetDatabase.GetAssetPath(_event); //the location of the event in the project
            AssetDatabase.RenameAsset(_assetPath, newName); //returns error message if fail
            AssetDatabase.Refresh();

            RepaintEventWindow();
            EventEditor.WriteEvents(true);

            newName = target.name;
        }
        else
        {
            if(newName == null)
                newName = target.name;
        }
    }

    public void ClipboardButton(Event _event)
    {
        if (GUILayout.Button("Copy Declaration", EditorStyles.miniButton, GUILayout.Width(130), GUILayout.Height(19)))
        {
            lastClipboardCopy = EditorApplication.timeSinceStartup;
            EditorGUIUtility.systemCopyBuffer = "private void On" + _event.codeFriendlyName + " (" + _event.GetParamTypeNameList(false) + ") \n{\n            \n}";
        }
    }

    private static void RepaintEventWindow()
    {
        EditorWindow _previouslyFocused = EditorWindow.focusedWindow;
        EditorWindow.GetWindow<EventEditor>().Repaint();
        if (_previouslyFocused != null)
            _previouslyFocused.Focus();
    }

    public override void OnInspectorGUI()
    {
        //if(currentUnedited != null)
        //    Debug.Log(currentUnedited.name == "");
        //else
        //    Debug.Log(currentUnedited);

        Event _event = (Event)target; //the scriptable object as an Event
        
        //if (EditorWindow.focusedWindow.GetType().Name != "InspectorWindow")
        //    CancelEdit(_event);

        _event.validParamNames = true;
        _event.validParamTypes = true;

        DrawNameField(_event); //Draws the text field for the name of the Event
        
        //Parameter header
        GUIStyle _parameterHeaderStyle = new GUIStyle(EditorStyles.label);
        _parameterHeaderStyle.margin = new RectOffset(0, 0, 6, 8);
        //_parameterHeaderStyle.fixedWidth = 100;
        _parameterHeaderStyle.contentOffset = new Vector2(3, 8);
        GUILayout.Label("Parameters", _parameterHeaderStyle);
        //end parameter header

        scrollPos = GUILayout.BeginScrollView(scrollPos, false, false);

        GUIStyle _verticalStyle = new GUIStyle(EditorStyles.helpBox);
        _verticalStyle.padding = new RectOffset(0, 0, 6, 8);

        if (_event.parameters.Count == 0) //start "no parameters"
        {
            GUILayout.BeginHorizontal(EditorStyles.helpBox);
            GUILayout.FlexibleSpace();
            GUIStyle _noParamsWarnStyle = new GUIStyle(EditorStyles.boldLabel);
            _noParamsWarnStyle.fontSize = 10;
            _noParamsWarnStyle.normal.textColor = new Color(0.2f, 0.2f, 0.2f);
            GUILayout.Label("No Parameters", _noParamsWarnStyle);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        } //end "no parameters"

        for (int i = 0; i < _event.parameters.Count; i++) //looping through each parameter
        {
            GUILayout.BeginVertical(_verticalStyle);
            GUILayout.BeginHorizontal();

            if (_event.paramEditIndex == i)
                DrawParameterEdit(i, _event);
            else
                DrawParameterNormal(i, _event);

            GUILayout.EndVertical();
        }

        //end looping through all parameters

        if (_event.setControlToNewParam)
        {
            EditorGUI.FocusTextInControl("ParameterName" + (_event.parameters.Count - 1));
            _event.setControlToNewParam = false;
        }

        GUIStyle _topVerticalStyle = new GUIStyle(); //add parameter button start
        _topVerticalStyle.margin = new RectOffset(0, 0, 11, 11);
        GUILayout.BeginHorizontal(_topVerticalStyle);
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Add Parameter", EditorStyles.miniButton, GUILayout.Width(130), GUILayout.Height(19)))
        {
            CancelEdit(_event);

            _event.parameters.Add(new ParameterInfo());
            _event.paramEditIndex = _event.parameters.Count - 1;
            _event.setControlToNewParam = true;
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal(); //add parameter button end

        GUILayout.FlexibleSpace(); //space to bottom of window
        GUILayout.EndScrollView();

        double timeSinceClipboardCopy = EditorApplication.timeSinceStartup - lastClipboardCopy;
        if(timeSinceClipboardCopy < 0.4d)
        {
            GUIStyle _declarationTextStyle = new GUIStyle(EditorStyles.label); //add parameter button start
            _declarationTextStyle.normal.textColor = new Color(0.05f, 0.5f, 0.08f);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Declaration copied to clipboard.", _declarationTextStyle);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        GUIStyle _bottomHorizontalStyle = new GUIStyle(); //start write changes button
        _bottomHorizontalStyle.margin = new RectOffset(0, 0, 11, 0);
        GUILayout.BeginHorizontal(_bottomHorizontalStyle);
        ClipboardButton(_event);
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Write Changes", EditorStyles.miniButton, GUILayout.Width(130), GUILayout.Height(19)))
        {
            EventEditor.WriteEvents(false);
        }
        GUILayout.EndHorizontal(); //end write changes button
        
    }
    private void DrawParameterNormal(int _parameterIndex, Event _event)
    {
        ParameterInfo _parameterInfo = _event.parameters[_parameterIndex];

        if (_event == null || _parameterInfo == null)
            return;

        GUIStyle _nameStyle = new GUIStyle(EditorStyles.label);
        _nameStyle.contentOffset = new Vector2(0, -2);
        _nameStyle.richText = true;
        GUILayout.Label("<b><color=black>" + _parameterInfo.name + "</color></b> " + "<size=9> <color=#4A4A4AFF>of type</color> </size> " + "<color=blue>" + _parameterInfo.typeName + "</color>", _nameStyle);

        //start edit button
        GUIStyle _editButtonStyle = new GUIStyle(EditorStyles.miniButton);
        _editButtonStyle.margin = new RectOffset(10, 0, 0, 20);
        _editButtonStyle.padding = new RectOffset(0, 0, 2, 2);
        _editButtonStyle.alignment = TextAnchor.MiddleCenter;
        _editButtonStyle.fixedWidth = 50;

        if (GUILayout.Button("Edit", _editButtonStyle, GUILayout.Width(55)))
        {
            CancelEdit(_event);

            _parameterInfo.paramTypeErrorMessage = CheckIfParameterTypeIsValid(_parameterInfo);
            currentUnedited = _parameterInfo.Copy();
            _event.paramEditIndex = _parameterIndex;
        }
        //end edit button

        GUILayout.EndHorizontal();
    }
    private void DrawParameterEdit(int _parameterIndex, Event _event)
    {
        //if (_event.paramEditIndex >= _event.parameters.Count)
        //{
        //    _event.paramEditIndex = -1;
        //    return;
        //}

        ParameterInfo _parameterInfo = _event.parameters[_parameterIndex];

        GUIStyle _headerStyle = new GUIStyle(EditorStyles.boldLabel);
        _headerStyle.contentOffset = new Vector2(0, -7);
        GUILayout.Label("Editing Parameter " + (_parameterIndex + 1) + "", _headerStyle);

        //start delete button
        GUIStyle _deleteButtonStyle = new GUIStyle(EditorStyles.miniButton);
        _deleteButtonStyle.margin = new RectOffset(10, 0, 0, 20);
        _deleteButtonStyle.padding = new RectOffset(0, 0, 2, 2);
        _deleteButtonStyle.alignment = TextAnchor.MiddleCenter;
        _deleteButtonStyle.fixedWidth = 50;

        //GUIStyleState _deleteButtonState = _deleteButtonStyle.normal;
        //_deleteButtonState.textColor = Color.black;
        //_deleteButtonStyle.normal = _deleteButtonState;

        if (GUILayout.Button("Delete", _deleteButtonStyle, GUILayout.Width(55)))
        {
            _event.parameters.RemoveAt(_parameterIndex);
            CancelEdit(_event);
            RepaintEventWindow();
            EventEditor.WriteEvents(true);
            return;
        }
        //end delete button

        GUILayout.EndHorizontal();

        GUIStyle _warningStyle = new GUIStyle(EditorStyles.label);
        _warningStyle.normal.textColor = Color.red;

        GUI.SetNextControlName("ParameterName" + _parameterIndex);
        _parameterInfo.name = EditorGUILayout.TextField("Parameter Name", _parameterInfo.name);

        if (_parameterInfo.paramNameErrorMessage != "")
        {
            _event.validParamTypes = false;

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Invalid name: " + _parameterInfo.paramNameErrorMessage, _warningStyle);
            GUILayout.EndHorizontal();
        }

        _parameterInfo.typeName = EditorGUILayout.TextField("Parameter Type", _parameterInfo.typeName);

        if (_parameterInfo.paramTypeErrorMessage != "")
        {
            _event.validParamTypes = false;

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Invalid type: " + _parameterInfo.paramTypeErrorMessage, _warningStyle);
            GUILayout.EndHorizontal();
        }

        GUIStyle _matchStyle = new GUIStyle();
        _matchStyle.margin = new RectOffset(5, 0, 8, 3);
        _matchStyle.padding = new RectOffset(0, 0, 0, 0);
        _matchStyle.contentOffset = new Vector2(0, -4);
        _matchStyle.richText = true;
        _matchStyle.alignment = TextAnchor.MiddleCenter;

        for (int _typeIndex = 0; _typeIndex < _parameterInfo.typeData.Count; _typeIndex++)
        {
            //if (_parameterInfo.typeMatchNamespaces[_typeIndex] == null)
            //    Debug.Log("Null");
            //else
            //    Debug.Log(_parameterInfo.typeMatchNamespaces[_typeIndex].Length);

            if (_parameterInfo.typeMatchNamespaces[_typeIndex] == null || _parameterInfo.typeMatchNamespaces[_typeIndex].Length == 0)
                continue;

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Namespace for " + _parameterInfo.typeData[_typeIndex].name + ":", _matchStyle);
            GUILayout.FlexibleSpace();
            if (_parameterInfo.typeMatchNamespaces[_typeIndex] != null)
                _parameterInfo.typeMatchIndexes[_typeIndex] = EditorGUILayout.Popup(_parameterInfo.typeMatchIndexes[_typeIndex], _parameterInfo.typeMatchNamespaces[_typeIndex]);//, GUILayout.Width(230));
            EditorGUILayout.EndHorizontal();
        }

        GUIStyle _confirmButtonStyle = new GUIStyle(EditorStyles.miniButton);
        _confirmButtonStyle.margin = new RectOffset(5, 5, 5, 5);
        _confirmButtonStyle.padding = new RectOffset(0, 0, 2, 2);
        _confirmButtonStyle.alignment = TextAnchor.MiddleCenter;

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Cancel", _confirmButtonStyle))
        {
            CancelEdit(_event);
        }

        if (GUILayout.Button("Confirm", _confirmButtonStyle))
        {
            _parameterInfo.paramNameErrorMessage = CheckIfParameterNameIsValid(_parameterIndex, _event, _parameterInfo);
            _parameterInfo.paramTypeErrorMessage = CheckIfParameterTypeIsValid(_parameterInfo);
            
            if(_parameterInfo.paramNameErrorMessage == "" && _parameterInfo.paramTypeErrorMessage == "")
            {
                _event.validParamNames = true;
                _event.validParamTypes = true;

                EditorUtility.SetDirty(_event); //saves changes to the Event
                _event.paramEditIndex = -1;
                EventEditor.WriteEvents(true);
                RepaintEventWindow();
                
                currentUnedited = null;
            }
        }

        EditorGUILayout.EndHorizontal();
    }

    private void CancelEdit(Event _event)
    {
        if (_event.paramEditIndex == -1)
            return;

        EditorGUI.FocusTextInControl("");

        if ((currentUnedited == null || currentUnedited.name == "") && _event.paramEditIndex < _event.parameters.Count)
        {
            _event.parameters.RemoveAt(_event.paramEditIndex);
            RepaintEventWindow();
            EventEditor.WriteEvents(true);

            _event.paramEditIndex = -1;
            currentUnedited = null;
            return;
        }

        if (_event.paramEditIndex >= _event.parameters.Count)
        {
            _event.paramEditIndex = -1;
            currentUnedited = null;
            return;
        }

        _event.parameters[_event.paramEditIndex] = currentUnedited;

        _event.paramEditIndex = -1;
        currentUnedited = null;
    }

    private static string CheckIfParameterNameIsValid(int _parameterIndex, Event _event, ParameterInfo _parameterInfo)
    {
        if (_parameterInfo.name == "")
            return "No name entered.";

        if (!System.CodeDom.Compiler.CodeGenerator.IsValidLanguageIndependentIdentifier(_parameterInfo.name))
        {
            return "Name contains invalid characters.";
        }
        for (int i = _parameterIndex - 1; i >= 0; --i)
        {
            if (_parameterInfo.name == _event.parameters[i].name)
            {
                return "Name is duplicate.";
            }
        }

        return "";
    }
    private static string CheckIfParameterTypeIsValid(ParameterInfo _parameterInfo)
    {
        //_parameterInfo.typeNamespace = ""; //Resetting the namespace variable incase it has changed
        string _userTypeName = _parameterInfo.typeName.Replace(" ", ""); //Removing spaces (the type name as provided by the user)

        if (_userTypeName.Length < 2)
            return "No type provided";

        int _openCount = _userTypeName.Length - _userTypeName.Replace("<", "").Length;
        int _closeCount = _userTypeName.Length - _userTypeName.Replace(">", "").Length;

        if(_openCount != _closeCount)
            return "Generic type is formatted incorrectly.";

        _parameterInfo.typeData = new List<TypeData>();
        TypeData _currentGeneric = null;

        for (int i = 0; i < _userTypeName.Length; i++)
        {
            if (_userTypeName[i] == '<' || _userTypeName[i] == ',' || _userTypeName[i] == '>' || i + 1 == _userTypeName.Length)
            {
                if(i == 0)
                {
                    _userTypeName = _userTypeName.Remove(0, 1); //Removing the extra thing, but ignoring its effects
                    continue;
                }

                /////////////////////////////////////////
                //Creating and storing the new typeData//
                
                string _name = null;

                if (i == _userTypeName.Length - 1 && _userTypeName[i] != '>')
                    _name = _userTypeName.Substring(0, i + 1);
                else
                    _name = _userTypeName.Substring(0, i);

                TypeData _newType = new TypeData(_name, _currentGeneric);

                if (_currentGeneric != null)
                    _currentGeneric.children.Add(_newType);

                _parameterInfo.typeData.Add(_newType);

                /////////////////////////////////////////

                if (_userTypeName[i] == '<')
                {
                    _newType.parent = _currentGeneric;
                    _currentGeneric = _newType;
                }

                else if (_userTypeName[i] == '>')
                {
                    if (i + 1 != _userTypeName.Length && _userTypeName[i + 1] == '[')
                    {
                        if (i + 1 == _userTypeName.Length - 1 || _userTypeName[i + 2] != ']')
                            return "Array type is formatted incorrectly.";
                        else
                        {
                            _newType.parent.isArray = true;
                            _userTypeName = _userTypeName.Remove(i + 1, 2);
                        }
                    }

                    if (_currentGeneric != null)
                        _currentGeneric = _currentGeneric.parent;
                }

                if(i - 2 >= 0 && _userTypeName[i - 2] == '[')
                    {
                    if (_userTypeName[i - 1] != ']')
                        return "Array type is formatted incorrectly.";
                    else
                    {
                        _newType.isArray = true;
                        _userTypeName = _userTypeName.Remove(i - 2, 2);
                        _newType.name = _newType.name.Remove(_newType.name.Length - 2, 2);
                        i -= 2;
                    }
                }
                if (i + 1 == _userTypeName.Length && _userTypeName[i] == ']')
                {
                    if (_userTypeName[i - 1] != '[')
                        return "Array type is formatted incorrectly.";
                    else
                    {
                        _newType.isArray = true;
                        _userTypeName = _userTypeName.Remove(i - 2, 2);
                        _newType.name = _newType.name.Remove(_newType.name.Length - 2, 2);
                        i -= 2;
                    }
                }

                _userTypeName = _userTypeName.Remove(0, i + 1); //removing the type segment from the user provided string
                i = -1; //resetting i back at the begining (because everything up to that point was removed)
            }
        }

        //make sure that there are the same number of type match indexes as there are typeData
        while (_parameterInfo.typeMatchIndexes.Count < _parameterInfo.typeData.Count)
            _parameterInfo.typeMatchIndexes.Add(0);
        while (_parameterInfo.typeMatchIndexes.Count > _parameterInfo.typeData.Count)
            _parameterInfo.typeMatchIndexes.RemoveAt(_parameterInfo.typeMatchIndexes.Count - 1); //remove at last index

        while (_parameterInfo.typeMatchNamespaces.Count < _parameterInfo.typeData.Count)
            _parameterInfo.typeMatchNamespaces.Add(null);
        while (_parameterInfo.typeMatchNamespaces.Count > _parameterInfo.typeData.Count)
            _parameterInfo.typeMatchNamespaces.RemoveAt(_parameterInfo.typeMatchNamespaces.Count - 1); //remove at last index

        //Check if each type is valid
        for (int i = 0; i < _parameterInfo.typeData.Count; i++)
        {
            string _error = CheckIfValidType(i, _parameterInfo);

            if (_error != null)
                return _error;
        }

        if (_parameterInfo.typeData.Count > 0)
        {
            _parameterInfo.fullTypeName = _parameterInfo.typeData[0].GetNameAndChildNames();
            _parameterInfo.typeName = _parameterInfo.typeData[0].GetNameAndChildNames(false);
        }
        else
            _parameterInfo.fullTypeName = "";

        return ("");
    }
    ///<summary> Returns error messages for invalid types and null if type is valid. </summary>
    private static string CheckIfValidType(int _typeIndex, ParameterInfo _parameterInfo)
    {
        TypeData _typeData = _parameterInfo.typeData[_typeIndex];
        string _name = _typeData.GetFullName();
        
        bool _typeIsKeyword = CheckTypeIsKeyword(_name);

        List<System.Type> _typeList = null;
        if (!_typeIsKeyword) //if the type wasn't a keyword, look in the assemblies for it
        {
            _typeList = GetListOfTypesFromString(_name);
            _typeData.type = GetTypeFromListOfTypes(_typeList, _typeIndex, _parameterInfo); //getting the type to evaluate
        }

        if (!_typeIsKeyword && _typeData.type == null)
        {
            if (_typeList.Count > 1 && _parameterInfo.typeMatchNamespaces[_typeIndex].Length > 0 && _parameterInfo.typeMatchIndexes[_typeIndex] == 0)
                return "Some namespaces cannot be inferred.";

            if (_typeData.argumentCount == 0)
                return "Type '" + _typeData.name + "' does not exist.";
            else
                return "Type '" + _typeData.name + "' that takes " + _typeData.argumentCount + " argument(s) does not exist.";
        }

        if (!_typeIsKeyword && _typeData.type.IsInterface)
            return "Type '" + _typeData.name + "' cannot be an interface.";

        if (!_typeIsKeyword && _typeData.type.IsAbstract)
            return "Type '" + _typeData.name + "' cannot be static or abstract.";

        //Storing the namespace for later
        //if (_type != null && _type.FullName.Contains("."))
        //{
        //    int lastDotIndex = _type.FullName.LastIndexOf(".");
        //    _parameterInfo.typeNamespaces.Add(_type.FullName.Substring(0, lastDotIndex));
        //}

        return null; //Success: no error message
    }
    private static System.Type GetTypeFromListOfTypes(List<System.Type> _matchingTypes, int _typeIndex, ParameterInfo _parameterInfo)
    {
        System.Type _type = null;

        if (_matchingTypes == null)
            return _type;

        if (_matchingTypes.Count == 1)
            return _matchingTypes[0];

        if (_parameterInfo.typeMatchIndexes[_typeIndex] > _matchingTypes.Count)
            _parameterInfo.typeMatchIndexes[_typeIndex] = 0; //the type match index for that type index is out of range so reset it

        if (_matchingTypes.Count > 1)
        {
            string[] _namespaces = new string[_matchingTypes.Count + 1];
            _namespaces[0] = " -- Select namespace -- ";
            for (int i = 1; i < _namespaces.Length; i++)
                _namespaces[i] = _matchingTypes[i - 1].FullName;

            for (int i = 1; i < _namespaces.Length; i++)
            {
                int _lastDot = _namespaces[i].LastIndexOf(".");
                
                if (_lastDot != -1)
                    _namespaces[i] = _namespaces[i].Remove(_lastDot);
            }

            _parameterInfo.typeMatchNamespaces[_typeIndex] = _namespaces;
        }

        if (_parameterInfo.typeMatchIndexes[_typeIndex] != 0) //if an index has been chosen
        {
            int _matchIndex = _parameterInfo.typeMatchIndexes[_typeIndex];
            _type = _matchingTypes[_matchIndex - 1]; //subtract 1 so that a 0 match index is actually -1 and therefore invalid and all others are valid
        }

        return _type;
    }
    private static bool CheckTypeIsKeyword(string _typeName)
    {
        return _typeName == "bool" || _typeName == "byte" || _typeName == "char" || _typeName == "decimal" || _typeName == "double" ||
            _typeName == "float" || _typeName == "int" || _typeName == "long" || _typeName == "sbyte" || _typeName == "short" ||
            _typeName == "string" || _typeName == "uint" || _typeName == "ulong" || _typeName == "ushort";
    }
    private static List<System.Type> GetListOfTypesFromString(string _typeName)
    {
        List<System.Type> _matchingTypes = new List<System.Type>();

        Assembly[] _assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
        foreach (Assembly _assembly in _assemblies) //Loop through all assemblies
        {
            System.Type[] _typesInAssemly = _assembly.GetTypes(); //Get the types for the current assembly
            foreach (System.Type _type in _typesInAssemly) //Loop through the types in this assembly
                if (_type.Name == _typeName || _type.FullName == _typeName) //Check if the type name matchs the supplied parameter name
                    _matchingTypes.Add(_type); //If match is made, the type of the parameter is meant to be that type
        }

        //System.Type.GetType();

        return _matchingTypes;
    }
}