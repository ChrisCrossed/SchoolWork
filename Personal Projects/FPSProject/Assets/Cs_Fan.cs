using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cs_Fan : MonoBehaviour
{
    [SerializeField] float RotationsPerMinute = 10f;
    
	// Update is called once per frame
	void Update ()
    {
        Vector3 v3_CurrRot_ = gameObject.transform.eulerAngles;
        v3_CurrRot_.y += Time.deltaTime * RotationsPerMinute;
        gameObject.transform.eulerAngles = v3_CurrRot_;
	}
}
