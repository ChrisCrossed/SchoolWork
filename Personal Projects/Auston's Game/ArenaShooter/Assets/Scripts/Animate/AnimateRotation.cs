using UnityEngine;
using System.Collections;

public class AnimateRotation : AnimateVector3
{
    protected override void SetStartValue()
    {
        startValue = transform.localRotation.eulerAngles;
    }
    protected override void SetNewValue(Vector3 _newValue)
    {
        transform.localRotation = Quaternion.Euler(_newValue);
    }
}
