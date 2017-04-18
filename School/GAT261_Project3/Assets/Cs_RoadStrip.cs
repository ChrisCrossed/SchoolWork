using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cs_RoadStrip : MonoBehaviour
{
    [SerializeField] float LastZPos = 570f;
    [SerializeField] float Speed;

	// Update is called once per frame
	void Update ()
    {
        Vector3 v3_Pos = gameObject.transform.position;
        v3_Pos.z -= Speed * Time.deltaTime;

        if (v3_Pos.z < -10f) v3_Pos.z = LastZPos;

        gameObject.transform.position = v3_Pos;
	}
}
