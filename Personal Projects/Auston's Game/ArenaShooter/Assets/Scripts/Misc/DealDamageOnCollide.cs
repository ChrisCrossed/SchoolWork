using UnityEngine;
using System.Collections;

public class DealDamageOnCollide : MonoBehaviour
{
    public bool applyForces = false;

    void OnTriggerEnter(Collider _collider)
    {
        Health _health = _collider.GetComponent<Health>();
        if (_health == null) return;

        _health.DealDamage(int.MaxValue, null, -1, transform.position);
    }

    void OnTriggerStay(Collider _collider)
    {
        Health _health = _collider.GetComponent<Health>();
        if (_health != null)
            _health.DealDamage(int.MaxValue, null, -1, transform.position);

        if (!applyForces) return;

        Rigidbody _rigidbody = _collider.GetComponent<Rigidbody>();
        if (_rigidbody == null) return;

        _rigidbody.AddForce(Vector3.up * 35, ForceMode.Force);
        _rigidbody.drag = Mathf.Lerp(_rigidbody.drag, 2, Time.deltaTime * 0.25f);
        _rigidbody.angularDrag = Mathf.Lerp(_rigidbody.drag, 2, Time.deltaTime * 0.25f);
    }
}
