using UnityEngine;
using System.Collections;

public class CS_ImageParameters : MonoBehaviour
{
    private Vector3 V3_LocalScale = new Vector3();

	// Use this for initialization
	void Start ()
    {
        //V3_LocalScale = gameObject.transform.position;

        //V3_LocalScale.x = 1;
        // V3_LocalScale.y = 1;

        // gameObject.transform.localScale = V3_LocalScale;

        gameObject.transform.localScale = new Vector3(1, 1, 1);
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}
}
