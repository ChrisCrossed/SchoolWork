using UnityEngine;
using System.Collections;

public class Cs_Platform : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        Vector3 currPos = gameObject.transform.position;
        gameObject.GetComponent<Rigidbody>().MovePosition(currPos + (transform.forward * Time.deltaTime));
	}
}
