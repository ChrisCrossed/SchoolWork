using UnityEngine;
using System.Collections;

public class AnimateLightIntensity : AnimateFloat
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
        light.intensity = _newValue;
        //Debug.Log(_newValue);
    }

    protected override void SetStartValue()
    {
        startValue = light.intensity;
    }
}
