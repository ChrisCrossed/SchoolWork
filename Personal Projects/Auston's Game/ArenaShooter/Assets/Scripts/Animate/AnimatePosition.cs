using UnityEngine;
using System.Collections;

public class AnimatePosition : AnimateVector3
{
    public bool usesScreenRatio = false;
    private RectTransform rectTransform = null;
    private RectTransform parentRectTransform = null;
    private new Transform transform = null;

    protected override void Start()
    {
        transform = base.transform;

        rectTransform = GetComponent<RectTransform>();
        if (transform.parent != null) parentRectTransform = transform.parent.GetComponent<RectTransform>();

        base.Start();
    }

    protected override void SetStartValue()
    {
        if (usesScreenRatio)
            startValue = ConvertToScreenRatio(transform.localPosition, parentRectTransform.rect);
        else
            startValue = transform.localPosition;
    }
    protected override void SetNewValue(Vector3 _newValue)
    {
        if (usesScreenRatio)
            transform.localPosition = ConvertFromScreenRatio(_newValue, parentRectTransform.rect);
        else
            transform.localPosition = _newValue;
    }
    
    private static Vector3 ConvertFromScreenRatio(Vector3 _vector, Rect _parentRect)
    {
        Vector3 _localPosition = new Vector3(_vector.x * _parentRect.x, _vector.y * _parentRect.y, _vector.z);
        _localPosition.x += 1;
        _localPosition.y += 1;
        _localPosition.z += 1;
        _localPosition *= -1;
        return _localPosition;
    }
    private static Vector3 ConvertToScreenRatio(Vector3 _vector, Rect _parentRect)
    {
        _vector /= -1;
        _vector.x -= 1;
        _vector.y -= 1;
        _vector.z -= 1;
        Vector3 _screenRatio = new Vector3(_vector.x / _parentRect.x, _vector.y / _parentRect.y, _vector.z);
        return _screenRatio;
    }
}
