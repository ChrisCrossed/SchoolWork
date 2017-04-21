using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cs_Particle : MonoBehaviour
{
    [SerializeField] float Speed;

	// Use this for initialization
	void Start ()
    {
		
	}

    // Update is called once per frame
    float f_Timer;
	void Update ()
    {
        Vector3 v3_Pos = gameObject.transform.position;
        v3_Pos += gameObject.transform.forward * Speed * Time.deltaTime;
        gameObject.transform.position = v3_Pos;

        f_Timer += Time.deltaTime;
        if (f_Timer > 2.0f) Destroy(gameObject);
	}
}
