using UnityEngine;
using System.Collections;

public class LookAt : MonoBehaviour
{
    public Transform targetTransform;

	// Use this for initialization
	void Start ()
    {
	    
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(targetTransform != null)
            transform.LookAt(transform.forward, Vector3.forward);
	}
}
