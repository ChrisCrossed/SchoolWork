using UnityEngine;
using System.Collections;

public abstract class AnimateVector3 : Animate
{
    public Vector3 startValue = Vector3.zero;
    public Vector3 endValue = Vector3.zero;

    // Use this for initialization
    protected override void Start ()
    {
        base.Start();
	}

    protected override void SetValueFromRatio(float _ratio)
    {
        _ratio = curve.Evaluate(_ratio);

        Vector3 _newValue = startValue + (endValue - startValue) * _ratio;
        SetNewValue(_newValue);
    }

    protected virtual void SetNewValue(Vector3 _newValue)
    {

    }
}
