using UnityEngine;
using System.Collections;

public abstract class AnimateFloat : Animate
{
    public float startValue = 0f;
    public float endValue = 0f;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
    }

    protected override void SetValueFromRatio(float _ratio)
    {
        _ratio = curve.Evaluate(_ratio);

        float _newValue = startValue + (endValue - startValue) * _ratio;
        SetNewValue(_newValue);
    }

    protected virtual void SetNewValue(float _newValue)
    {

    }
}
