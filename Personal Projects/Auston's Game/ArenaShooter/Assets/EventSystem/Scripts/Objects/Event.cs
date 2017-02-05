using UnityEngine;
using System.Collections.Generic;

public class TypeData
{
    public System.Type type = null;
    public string name = null; //the name of the type
    public bool isArray = false;

    public TypeData parent = null; //the generic type that encloses this type (if any)
    public List<TypeData> children = new List<TypeData>(); //the children that this generic type enloses
    public int argumentCount { get { return children.Count; } } //the number of children that this generic type enloses

    public TypeData(string _name, TypeData _parentTypeData)
    {
        name = _name;
        parent = _parentTypeData;
    } //constructor
    public string GetFullName()
    {
        string _argumentTag = "";

        if (argumentCount != 0)
            _argumentTag = "`" + argumentCount;

        return name + _argumentTag;
    } //returns the name of the type with the argument count suffix (if necessary)
    public string GetNameAndChildNames(bool _fullname = true)
    {
        string _nameAndChildNames = "";
        if (type != null)
        {            
            string _typeName = _fullname ? type.FullName : type.Name; //if "_fullname" then (?) include namespace, else (:) don't
            int _graveIndex = _typeName.LastIndexOf("`");
            if (_graveIndex == -1)
            {
                _nameAndChildNames = _typeName;
                if (isArray)
                    _nameAndChildNames += "[]";
            }
            else
            {
                _typeName = _typeName.Remove(_graveIndex);
                _nameAndChildNames += _typeName + "<";
                for (int i = 0; i < children.Count; i++)
                {
                    _nameAndChildNames += children[i].GetNameAndChildNames(_fullname);
                    if (i + 1 < children.Count)
                        _nameAndChildNames += ", ";
                }
                _nameAndChildNames += ">";

                if (isArray)
                    _nameAndChildNames += "[]";
            }
        }
        else
        {
            _nameAndChildNames = name;
            if (isArray)
                _nameAndChildNames += "[]";
        }

        return _nameAndChildNames;
    }
}
[System.Serializable] public class ParameterInfo
{
    public string name = "";
    public string typeName = "";
    public string fullTypeName = "";

    public List<TypeData> typeData = new List<TypeData>(); //this parameter's type parsed into individual type data (for more complex generic types)
    public List<int> typeMatchIndexes = new List<int>();
    public List<string[]> typeMatchNamespaces = new List<string[]>();

    public string paramNameErrorMessage = "";
    public string paramTypeErrorMessage = "";
    //public bool beingEdited = false;
    //public ParameterInfo uneditedVersion = null;

    public ParameterInfo Copy()
    {
        ParameterInfo _copy = new ParameterInfo();
        _copy.name = name;
        _copy.typeName = typeName;
        _copy.fullTypeName = fullTypeName;
        _copy.typeData = new List<TypeData>(typeData);
        _copy.typeMatchIndexes = new List<int>(typeMatchIndexes);
        _copy.typeMatchNamespaces = new List<string[]>(typeMatchNamespaces);

        return _copy;
    }
}

public class Event : ScriptableObject
{
    public string codeFriendlyName { get { return name.Replace(" ", ""); } }
    public List<ParameterInfo> parameters = new List<ParameterInfo>();
    public bool validParamNames = false;
    public bool validParamTypes = false;
    public int paramEditIndex = -1;
    public bool setControlToNewParam = false;
    public bool setControlToName = false;

    public string GetParamNameList()
    {
        string _nameList = "";

        for (int i = 0; i < parameters.Count; i++)
        {
            _nameList += parameters[i].name;
            if (i + 1 == parameters.Count)
                break;
            else
                _nameList += ", "; 
        }

        return _nameList;
    }
    public string GetParamTypeNameList(bool _fullTypeName = true)
    {
        string _typeNameList = "";

        for (int i = 0; i < parameters.Count; i++)
        {
            //if "_fullTypeName" then (?) include namespace, else (:) don't
            string _typeName = _fullTypeName ? parameters[i].fullTypeName : parameters[i].typeName;
            _typeNameList += _typeName + " " + parameters[i].name;

            if (i + 1 == parameters.Count)
                break;
            else
                _typeNameList += ", ";
        }

        return _typeNameList;
    }
}

