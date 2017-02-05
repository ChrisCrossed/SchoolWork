using UnityEngine;
using System.Collections;

public class FaceMovement : MonoBehaviour
{
    private new Rigidbody rigidbody = null;
    private new Transform transform = null;
    private RotateTopTowardVelocity parentRotationInfo;
    private Vector3 previousHorizontal;

	// Use this for initialization
	void Start ()
    {
        transform = base.transform;
        rigidbody = GetComponentInParent<Rigidbody>();
        parentRotationInfo = transform.parent.GetComponent<RotateTopTowardVelocity>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (transform.parent == null) return;

        Vector3 _horizontalMovement = rigidbody.velocity;
        _horizontalMovement.y = 0;
        if (_horizontalMovement.sqrMagnitude < 0.001f)
            _horizontalMovement = previousHorizontal;

        previousHorizontal = _horizontalMovement;

        _horizontalMovement = Quaternion.AngleAxis(-parentRotationInfo.currentRotation, Vector3.Cross(_horizontalMovement, Vector3.up)) * _horizontalMovement;
        transform.LookAt(transform.position + _horizontalMovement);
    }
}
