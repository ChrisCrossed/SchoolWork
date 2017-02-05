using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class Detectable : MonoBehaviour
{
    [System.NonSerialized]
    public AffiliatedObject affiliatedObject;

    [Range(0,10)]
    public uint detectionLevel = 5;

    [HideInInspector]
    public List<Detection> detectedBy = new List<Detection>();

    private bool removalHandled = false;

    void Start()
    {
        affiliatedObject = GetComponent<AffiliatedObject>();
    }

    void DestroyWithoutRemoval()
    {
        removalHandled = true;
        Destroy(this);
    }

    void OnDestroy()
    {
        if (removalHandled) return;
        foreach (Detection _detection in detectedBy)
            _detection.detectedObjects.Remove(this);
    }
}
