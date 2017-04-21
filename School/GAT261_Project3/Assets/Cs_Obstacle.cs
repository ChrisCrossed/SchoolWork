using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cs_Obstacle : MonoBehaviour
{
    float f_Speed = 10f;

	// Update is called once per frame
	void Update ()
    {
        Vector3 v3_Pos = gameObject.transform.position;
        v3_Pos.z -= f_Speed * Time.deltaTime;
	}
}
