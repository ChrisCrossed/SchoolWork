using UnityEngine;
using System.Collections.Generic;

//determines which detected object should the the target of the next attack, block, heal, or other effect
public class Targeting : MonoBehaviour
{
    private AffiliatedObject affiliatedObject;
    [System.NonSerialized] public List<Detectable> detectedObjects = new List<Detectable>();
    public Transform currentTarget { get { if (targetOverride) return targetOverride; else return closestTarget; } }
    private Transform closestTarget;
    public Transform targetOverride;

    void Start()
    {
        affiliatedObject = GetComponent<AffiliatedObject>();
    }

	// Update is called once per frame
	void Update ()
    {
        UpdateTarget();
    }

    void UpdateTarget()
    {
        closestTarget = GetClosest();
    }

    protected virtual Transform GetClosest()
    {
        Transform _closestObject = null;
        float _shortestDistanceSq = float.PositiveInfinity;

        Transform _transform = transform;

        foreach (Detectable _detectedObject in detectedObjects)
        {
            if (_detectedObject.affiliatedObject != null && affiliatedObject != null && _detectedObject.affiliatedObject.team == affiliatedObject.team)
                continue; //affiliatedObject is on the same team

            Vector3 _vecToObject = (_detectedObject.transform.position - _transform.position);
            float _distToObject = _vecToObject.magnitude;
            Vector3 _vecToObjectNormalized = _vecToObject / _distToObject;

            //RaycastHit _hitInfo;
            //if (Physics.Raycast(_transform.position + _transform.up * 0.5f, _vecToObjectNormalized, out _hitInfo, _distToObject, ~0, QueryTriggerInteraction.Ignore))
            //    if (_hitInfo.collider.transform.root.GetComponent<Detectable>() != _detectedObject)
            //        continue; //can't see object

            Transform _detectedTransform = _detectedObject.transform;
            float distanceToTargetSq = (_detectedTransform.position - _transform.position).sqrMagnitude;

            if (distanceToTargetSq > _shortestDistanceSq)
                continue;

            _closestObject = _detectedTransform;
            _shortestDistanceSq = distanceToTargetSq;
        }

        if (_closestObject == null)
            return null;

        return _closestObject;
    }
}
