using UnityEngine;
using System.Collections;

public class AnimateScale : AnimateVector3
{
    protected override void SetStartValue()
    {
        startValue = transform.localScale;
    }
    protected override void SetNewValue(Vector3 _newValue)
    {
        transform.localScale = _newValue;
    }
}
