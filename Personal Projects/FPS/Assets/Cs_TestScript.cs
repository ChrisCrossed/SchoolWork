using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cs_TestScript : MonoBehaviour
{

	// Use this for initialization
	void Start ()
    {
        
    }

    float f_Timer;
	// Update is called once per frame
	void FixedUpdate ()
    {
        f_Timer += Time.fixedDeltaTime;

        Vector3 v3_Position = gameObject.GetComponent<Rigidbody>().position;
        v3_Position.x = Mathf.Sin(f_Timer / 2) * 7f;
        gameObject.GetComponent<Rigidbody>().MovePosition(v3_Position);

        gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
    }
}
