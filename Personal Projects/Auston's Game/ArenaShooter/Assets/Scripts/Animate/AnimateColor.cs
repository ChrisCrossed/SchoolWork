using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public abstract class AnimateColor : Animate
{
    public Color startColor = Color.white;
    public Color endColor = Color.white;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
    }

    protected override void SetValueFromRatio(float _ratio)
    {
        _ratio = curve.Evaluate(_ratio);

        Color _newColor = startColor + (endColor - startColor) * _ratio;
        SetColor(_newColor);
    }

    protected virtual void SetColor(Color _newColor)
    {
        
    }
}
