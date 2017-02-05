using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Targeting))]
public class Detection : MonoBehaviour
{
    private GameObject detectionObject;
    private SphereCollider detectionCollider;
    private DetectionSphere detectionSphere;
    //public Vector3
    public float detectionRadius;
    //public float detectionStrength;

    public LayerMask layersToDetect = ~0;

    private Targeting targeting;
    public List<Detectable> detectedObjects { get { if (targeting == null) return null; else return targeting.detectedObjects; } }

    public delegate void DetectionDelegate(Detectable _detectable);
    public DetectionDelegate onDetectObject = delegate { };
    public DetectionDelegate onLoseObject = delegate { };

    // Use this for initialization
    void Start ()
    {
        targeting = GetComponent<Targeting>();

        detectionObject = new GameObject();
        detectionObject.transform.parent = transform;
        detectionObject.transform.localPosition = Vector3.zero;
        detectionCollider = detectionObject.AddComponent<SphereCollider>();
        detectionSphere = detectionObject.AddComponent<DetectionSphere>();
        detectionSphere.owner = this;

        Rigidbody _rigidbody = detectionObject.AddComponent<Rigidbody>();
        _rigidbody.isKinematic = true;

        detectionCollider.radius = detectionRadius;
        detectionCollider.isTrigger = true;
	}

    void Update()
    {
        //for (int i = 0; i < detectedObjects.Count; i++)
        //{
        //    Detectable _detectable = detectedObjects[i];
        //    CapsuleCollider _targetCapsuleCollider = _detectable.GetComponent<CapsuleCollider>();
        //    Vector3 _targetCenter = Creation.GetCapsuleColliderCenter(_targetCapsuleCollider);

        //    Vector3 _vecToTarget = _targetCenter - transform.position;

        //    if (Physics.Raycast(transform.position, _vecToTarget, _vecToTarget.magnitude, 1 << 11, QueryTriggerInteraction.Ignore))
        //        return;
        //}
    }

    public void OnDetectableEnter(Detectable _detectable)
    {
        //Debug.Log(_detectable.gameObject.name);
        if ((layersToDetect & (1 << _detectable.gameObject.layer)) == 0 || _detectable.gameObject == gameObject || _detectable.transform.root == transform.root)
            return;

        onDetectObject.Invoke(_detectable);

        detectedObjects.Add(_detectable);
        _detectable.detectedBy.Add(this);
    }
    public void OnDetectableExit(Detectable _detectable)
    {
        onLoseObject.Invoke(_detectable);

        detectedObjects.Remove(_detectable);
        _detectable.detectedBy.Remove(this);
    }

    public void OnDestroy()
    {
        if (detectedObjects == null)
            return;

        for (int i = 0; i < detectedObjects.Count; i++)
            detectedObjects[i].detectedBy.Remove(this);

        Destroy(detectionObject);
    }
}
