using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cs_StopLight : MonoBehaviour
{

	// Use this for initialization
	void Start ()
    {
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        gameObject.GetComponent<BoxCollider>().enabled = false;
	}
}
