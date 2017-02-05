/*/////////////////////////////////////////////////////////////////////////
//SCRIPT - RotateTopTowardVelocity.cs
//AUTHOR - Auston Lindsay
//COPYRIGHT - © 2016 DigiPen Institute of Technology
/////////////////////////////////////////////////////////////////////////*/

using UnityEngine;
using System.Collections;

public class RotateTopTowardVelocity : MonoBehaviour
{
    [SerializeField]
    public float maxRotation = 20;
    [SerializeField]
    private float speed = 10;
    [SerializeField]
    private float maxVelocityMagnitude = 3;
    [System.NonSerialized]
    public float currentRotation;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Rigidbody _rootRigidbody = this.transform.root.GetComponent<Rigidbody>();
        if (_rootRigidbody == null)
            return;

        Vector3 _rootHorizontalVelocity = _rootRigidbody.velocity;
        _rootHorizontalVelocity.y = 0;
        float _rootSpeed = _rootHorizontalVelocity.magnitude;
        if (_rootSpeed == 0) return;

        Vector3 _targetVector = _rootRigidbody.velocity / _rootSpeed;
        float _maxRotation = Mathf.Clamp01(_rootSpeed / maxVelocityMagnitude) * Mathf.Deg2Rad * maxRotation;
        Vector3 _desiredUp = Vector3.RotateTowards(transform.root.up, _targetVector, _maxRotation, 0);
        Vector3 _desiredDirection = Vector3.Cross(transform.root.right, _desiredUp);
        if (_desiredDirection == Vector3.zero)
            return;

        if(_desiredDirection != Vector3.zero)
        {
            Quaternion _desiredRotation = Quaternion.LookRotation(_desiredDirection, _desiredUp);
            this.transform.rotation = Quaternion.Lerp(transform.rotation, _desiredRotation, Time.deltaTime * speed);
            currentRotation = Vector3.Angle(transform.root.up, _desiredUp);
        }
    }
}
