using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class AnimateFOV : AnimateFloat
{
    private new Camera camera = null;

    protected override void Start()
    {
        camera = GetComponent<Camera>();

        base.Start();
    }

    protected override void SetStartValue()
    {
        startValue = camera.fieldOfView;
    }

    protected override void SetNewValue(float _newValue)
    {
        camera.fieldOfView = _newValue;
    }
}
