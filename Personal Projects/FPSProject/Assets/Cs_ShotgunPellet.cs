using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cs_ShotgunPellet : Cs_BULLET
{
    // Use this for initialization
	void Start ()
    {
        Initialize();
    }

    public void SetDirection(Vector3 _forward, float _intensity = 5f )
    {
        Vector3 _direction = _forward + Random.onUnitSphere * (_intensity / 90);
        transform.LookAt(transform.position + _direction);
    }
}
