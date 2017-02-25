using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cs_BULLET : MonoBehaviour
{
    Rigidbody this_Rigidbody;
    Vector3 v3_StartRotation;

    float f_Speed = 10f;
    float f_Rotation = 0f;
    float f_Slope = 0f;

	// Use this for initialization
	protected void Initialize ()
    {
        this_Rigidbody = gameObject.GetComponent<Rigidbody>();
        v3_StartRotation = gameObject.transform.eulerAngles;
	}
    
    protected float BulletSpeed
    {
        set { f_Speed = value; }
        get { return f_Speed; }
    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        this_Rigidbody.velocity = gameObject.transform.forward * f_Speed;
	}
}
