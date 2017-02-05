using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class ProjectileTrackers : MonoBehaviour
{
    public Tracker trackerObject;
    public Detection detection;

    public List<Tracker> trackers = new List<Tracker>();

    public void Start()
    {
        detection.onDetectObject += OnDetectProjectile;
        detection.onLoseObject += OnLoseProjectile;
    }

    private void OnDetectProjectile(Detectable _detectable)
    {
        if (_detectable.GetComponent<Projectile>().targeting != null && _detectable.GetComponent<Projectile>().targeting.currentTarget != transform.root)
            return;

        Tracker _newTracker = (Tracker)Instantiate(trackerObject, transform.parent);
        trackers.Add(_newTracker);
        _newTracker.targetOverride = _detectable.transform;

        //Debug.Log("Creating tracker");
    }

    private void OnLoseProjectile(Detectable _detectable)
    {
        for (int i = 0; i < trackers.Count; i++)
        {
            if(trackers[i].targetOverride == _detectable.transform || trackers[i] == null)
            {
                Destroy(trackers[i].gameObject);
                trackers.RemoveAt(i);
                --i;
                //Debug.Log("Destroying tracker");
            }
        }
    }
}
