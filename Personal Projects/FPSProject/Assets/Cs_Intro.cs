using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cs_Intro : MonoBehaviour
{


	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        Color clr_ = gameObject.GetComponent<Image>().color;
        clr_.a -= Time.deltaTime / 3.0f;
        if (clr_.a < 0f) clr_.a = 0f;
        gameObject.GetComponent<Image>().color = clr_;

        if (clr_.a == 0f) gameObject.GetComponent<Image>().enabled = false;
	}
}
