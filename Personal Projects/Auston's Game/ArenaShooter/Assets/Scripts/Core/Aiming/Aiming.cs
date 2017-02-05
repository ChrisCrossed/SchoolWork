using UnityEngine;
using System.Collections;

public class Aiming : MonoBehaviour
{
    [Range(0, 10)]
    public float sensitivityOverride = 0;

    [Range(5, 40)]
    public float aimSpeed = 15; //the normal speed in which the look vector will be updated
    [Range(20, 90)]
    public float maxAimSpeed = 35; //the maximum look speed aquired after acceleration is applied when input is "pegged"
    public float aimAcceleration = 8; //the rate at which the aim speed will go to max aim speed after input is "pegged"
    [Range(0, 1)] public float verticalRatio = 0.8f; //the ratio of aim speed for the vertical look (so vertical can be slower)
    
    public AnimationCurve sensitivityCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); //the curve that is evaluated to determine the change in aim speed based on input

    public Transform verticalObject = null; //the object that will rotate around its local x axis when vertical aim input is applied
    public Transform horizontalObject = null; //the object that will rotate around its local y axis when vertical aim input is applied
    public Vector3 verticalObjectOffset = new Vector3(0, 0, 0); //the offset of the vertical object when aiming down (offset while aiming down will be the negative)

    protected Vector3 verticalObjectStartingPosition = Vector3.zero;

    protected float currentAimSpeed = 0; //the current aim speed (multiplier) to be applied while rotating objects
    protected Vector3 aimInput = new Vector3(0, 0, 0); //the current aim input to be applied while rotating objects

    protected InterpolateToPosition verticalObjectInterpolator = null; //the component that objects sometimes use to interpolate their positions

    // Use this for initialization
    virtual protected void Start ()
    {
        if(sensitivityOverride > 0)
        {
            aimSpeed = sensitivityOverride * 2;
            maxAimSpeed = sensitivityOverride * 4;
            aimAcceleration = sensitivityOverride;
        }

        if (verticalObject == null)
            verticalObject = transform;

        if (horizontalObject== null)
            horizontalObject = transform;

        verticalObjectInterpolator = verticalObject.GetComponent<InterpolateToPosition>();

        if(verticalObjectInterpolator == null)
            verticalObjectStartingPosition = verticalObject.localPosition;
        else
            verticalObjectStartingPosition = verticalObjectInterpolator.transformToFollow.localPosition;
    }

    protected void FixedUpdate()
    {
        Vector3 _aimInput = aimInput;
        _aimInput *= Time.fixedDeltaTime;

        //horizontal rotation application
        Vector3 _currentHorizontalRotation = horizontalObject.localEulerAngles;
        float _newHorizontal = _currentHorizontalRotation.x + _aimInput.x;
        horizontalObject.localRotation = Quaternion.Euler(_currentHorizontalRotation.x, _currentHorizontalRotation.y + _aimInput.x, _currentHorizontalRotation.z);

        //vertical rotation application
        Vector3 _currentVerticalRotation = verticalObject.localEulerAngles;
        float _verticalRatio = _currentVerticalRotation.x >= 270 ? 1 - ((_currentVerticalRotation.x - 270) / 90) : -_currentVerticalRotation.x / 90;

        if (verticalObjectOffset != Vector3.zero)
        {
            if (verticalObjectInterpolator == null) //check to see if we can just directly move the object or if we need to update the interpolators position
                verticalObject.localPosition = verticalObjectStartingPosition + (_verticalRatio * -verticalObjectOffset);
            else
                verticalObjectInterpolator.transformToFollow.localPosition = verticalObjectStartingPosition + (_verticalRatio * -verticalObjectOffset);
        }

        float _newVertical = _currentVerticalRotation.x - _aimInput.y;
        if (_newVertical > 90 && _newVertical < 180)
            _newVertical = 90;
        if (_newVertical < 270 && _newVertical > 180)
            _newVertical = 270;

        verticalObject.localRotation = Quaternion.Euler(_newVertical, _currentVerticalRotation.y, _currentVerticalRotation.z);
    }
}
