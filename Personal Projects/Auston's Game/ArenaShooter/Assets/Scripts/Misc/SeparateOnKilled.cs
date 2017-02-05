using UnityEngine;
using System.Collections;

public class SeparateOnKilled : MonoBehaviour
{
    public Transform[] transformsToSeparate;
    public GameObject[] gameObjectsToToggle;
    public Behaviour[] behaviorsToToggle;

    public float destroyDelay = -1;

    public void Separate(GameObject _killedBy)
    {
        for (int i = 0; i < transformsToSeparate.Length; i++)
        {
            transformsToSeparate[i].parent = null;
            InterpolateToPosition _interpolator = transformsToSeparate[i].GetComponent<InterpolateToPosition>();

            if (destroyDelay > 0)
            {
                DestroyAfterTime _newDestroyAfterTime = transformsToSeparate[i].gameObject.AddComponent<DestroyAfterTime>();
                _newDestroyAfterTime.lifetime = destroyDelay;
            }

            if (_interpolator != null)
                Destroy(_interpolator);

            Rigidbody _rigidbody = transformsToSeparate[i].gameObject.AddComponent<Rigidbody>();
            if (_rigidbody != null)
            {
                _rigidbody.mass = 3;
                _rigidbody.velocity = _rigidbody.velocity;

                _rigidbody.velocity += Vector3.ProjectOnPlane(Random.onUnitSphere, transform.up) * 8;
                _rigidbody.angularVelocity = Random.rotation.eulerAngles;
                if (_killedBy != null)
                {
                    _rigidbody.velocity += transform.up * 5;
                    _rigidbody.velocity += (transformsToSeparate[i].position - _killedBy.transform.position).normalized * 6;
                }
            }

            //_rigidbody.velocity *= Time.fixedDeltaTime * 60;

            Collider _collider = transformsToSeparate[i].GetComponent<Collider>();
            if (_collider != null)
                _collider.enabled = true;

            DestroyAfterTime _destroyAfterTime = transformsToSeparate[i].GetComponent<DestroyAfterTime>();
            if (_destroyAfterTime != null)
                _destroyAfterTime.enabled = true;
        }

        for (int i = 0; i < gameObjectsToToggle.Length; i++)
            gameObjectsToToggle[i].SetActive(!gameObjectsToToggle[i].activeInHierarchy);

        for (int i = 0; i < behaviorsToToggle.Length; i++)
            behaviorsToToggle[i].enabled = !behaviorsToToggle[i].enabled;
    }
}
