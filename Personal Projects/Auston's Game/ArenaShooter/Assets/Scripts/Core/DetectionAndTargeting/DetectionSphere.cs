using UnityEngine;
using System.Collections;

public class DetectionSphere : MonoBehaviour
{
    public Detection owner;

    public void OnTriggerEnter(Collider _collider)
    {
        Detectable _detectable = _collider.GetComponent<Detectable>();
        if (_detectable != null)
            owner.OnDetectableEnter(_detectable);
    }
    public void OnTriggerExit(Collider _collider)
    {
        Detectable _detectable = _collider.GetComponent<Detectable>();
        if (_detectable != null)
            owner.OnDetectableExit(_detectable);
    }
}
