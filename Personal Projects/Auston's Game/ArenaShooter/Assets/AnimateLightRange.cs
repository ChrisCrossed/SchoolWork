using UnityEngine;
using System.Collections;

public class AnimateLightRange : AnimateFloat
{
    private new Light light = null;

	// Use this for initialization
	protected override void Start ()
    {
        light = GetComponent<Light>();

        base.Start();
	}

    protected override void SetNewValue(float _newValue)
    {
        light.range = _newValue;
    }

    protected override void SetStartValue()
    {
        startValue = light.range;
    }
}
