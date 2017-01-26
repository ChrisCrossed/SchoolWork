using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cs_TestRotation : MonoBehaviour
{
    [SerializeField] float f_RotationsPerSecond = 3f;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        Vector3 v3_CurrRot = gameObject.transform.eulerAngles;
        
        v3_CurrRot.y = Mathf.LerpAngle(v3_CurrRot.y, v3_CurrRot.y + Time.deltaTime * 360f * f_RotationsPerSecond, Time.deltaTime);

        gameObject.transform.rotation = Quaternion.Euler(v3_CurrRot);
    }
}
