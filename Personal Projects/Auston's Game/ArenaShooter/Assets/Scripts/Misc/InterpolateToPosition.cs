using UnityEngine;
using System.Collections;

public class InterpolateToPosition : MonoBehaviour
{
    public Transform transformToFollow;
    public bool useStartingLocal = true;
    public float speed = 5;
    private Vector3 positionLastFrame;
    private new Transform transform;
    private Transform parentTransform;
    [System.NonSerialized]
    public Vector3 startingLocal;

	// Use this for initialization
	void Start ()
    {
        transform = base.transform;
        parentTransform = transform.parent;
        startingLocal = transform.localPosition;

        positionLastFrame = transform.position;

        //if (!useStartingLocal) return;

        //transformToFollow = new GameObject().transform;
        //transformToFollow.parent = transform.parent;
        //transformToFollow.localPosition = transform.localPosition;
    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        Vector3 _targetPosition = Vector3.zero;
        if (useStartingLocal)
            _targetPosition = parentTransform.TransformPoint(startingLocal);
        else if (transformToFollow != null)
            _targetPosition = transformToFollow.position;

        transform.position = Vector3.Lerp(positionLastFrame, _targetPosition, Time.fixedDeltaTime * speed);
        positionLastFrame = transform.position;
	}
}
