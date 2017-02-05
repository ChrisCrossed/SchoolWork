using UnityEngine;
using System.Collections;

public class AutomaticAiming : Aiming
{
    private Targeting targeting;
    private Transform currentTarget { get { if (targetOverride != null) return targetOverride; else if (targeting != null) return targeting.currentTarget; else return null; } }
    public Transform targetOverride;

	// Use this for initialization
	override protected void Start()
    {
        base.Start();
        targeting = GetComponent<Targeting>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (currentTarget == null)
        {
            aimInput = Vector3.zero;
            return;
        }

        Vector3 _colliderCenter = Vector3.zero;

        CapsuleCollider _collider = currentTarget.GetComponent<CapsuleCollider>();
        if (_collider != null)
            _colliderCenter = _collider.center;

        Vector3 _localTargetPosition = verticalObject.InverseTransformPoint(currentTarget.position + currentTarget.TransformVector(_colliderCenter));
        if (_localTargetPosition.z < 0)
            _localTargetPosition.y = 0;

        aimInput = ((Vector2)_localTargetPosition.normalized) * 25 * aimSpeed;
    }
}
