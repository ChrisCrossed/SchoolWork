using UnityEngine;
using System.Collections;

public class Cs_LockPosRot : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        Vector3 v3_Pos = transform.root.gameObject.transform.position;
        v3_Pos.y = v3_Pos.y + 1;
        gameObject.transform.position = v3_Pos;
	}
}
