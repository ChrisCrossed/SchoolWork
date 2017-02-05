using UnityEngine;
using System.Collections;

public class ManualTargeting : Targeting
{

    Player player;

    // Use this for initialization
    void Start()
    {
        player = GetComponent<Player>();
    }

    // Update is called once per frame
    protected override Transform GetClosest()
    {
        float _smallestAngle = 360;
        int _targetObjectIndex = -1;

        //getting the actual object
        for (int i = 0; i < AffiliatedObject.activeObjects.Count; i++)
        {
            AffiliatedObject _activeObject = AffiliatedObject.activeObjects[i];

            if (_activeObject == null || _activeObject.gameObject.layer == 1 << 9 || _activeObject.team == player.team)
                continue; //don't check against null, things that can't be detected, or objects of the same team

            Vector3 _targetPosition = Creation.GetCapsuleColliderCenter(_activeObject.GetComponent<CapsuleCollider>());
            Vector3 _vecToOtherObject = _targetPosition - transform.position;

            Rigidbody _targetRigidbody = _activeObject.GetComponent<Rigidbody>();

            //if (_targetRigidbody != null)
            //{
            //    float _distanceToTarg = _vecToOtherObject.magnitude;
            //    float _estimatedTimeToCollide = _distanceToTarg / projectileSpeed;

            //    Vector3 _estimatedNewPosition = _targetPosition + _targetRigidbody.velocity * _estimatedTimeToCollide;
            //    _vecToOtherObject = _estimatedNewPosition - transform.position;

            //    debugLastEstimatedPosition = _estimatedNewPosition;
            //}

            float _angleToOtherObject = Vector3.Angle(_vecToOtherObject, player.camera.forward);
            if (_angleToOtherObject < _smallestAngle)
            {
                _smallestAngle = _angleToOtherObject;
                _targetObjectIndex = i;
            }
        }

        if (_targetObjectIndex != -1)
            return AffiliatedObject.activeObjects[_targetObjectIndex].transform;
        else
            return null;
    }
}
