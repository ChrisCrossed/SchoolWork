using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Modifier
{
    [SerializeField]
    private List<float> multipliers = new List<float>();
    public float value { get; private set; }

    public Modifier()
    {
        value = 1;
    }

    public void AddMultiplier(float _multiplier)
    {
        multipliers.Add(_multiplier);
        value *= _multiplier;
    }
    public bool RemoveMultiplier(float _multiplier)
    {
        bool _removed = multipliers.Remove(_multiplier);
        if (_removed) value /= _multiplier;
        return _removed;
    }
    public float[] GetMultipliers()
    {
        float[] _multipliers = new float[multipliers.Count];
        multipliers.CopyTo(_multipliers);
        return _multipliers;
    }
    public void LogMultipliers()
    {
        string _logString = "";
        for (int i = 0; i < multipliers.Count; ++i)
        {
            if (_logString != "")
                _logString += ", ";

            _logString += multipliers[i].ToString();
        }

        Debug.Log(_logString);
    }
}