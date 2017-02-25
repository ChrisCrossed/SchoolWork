using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cs_ShotgunPellet : Cs_BULLET
{
    [Range(0, 90)]
    public float intensity = 2f;

	// Use this for initialization
	void Start ()
    {
        Initialize();

        //BulletRotation = 15f;
        SetDirection(transform.forward);
    }

    void SetDirection(Vector3 _forward)
    {
        Vector3 _direction = _forward + Random.onUnitSphere * (intensity / 90);
        transform.LookAt(transform.position + _forward);
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
