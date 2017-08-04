using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cs_Outro : MonoBehaviour
{
    GameObject go_One;
    GameObject go_Two;

	// Use this for initialization
	void Start ()
    {
        go_One = GameObject.Find("1");
        go_Two = GameObject.Find("2");
	}

    float f_Timer = 0f;
	// Update is called once per frame
	void Update ()
    {
        f_Timer += Time.deltaTime;

        if(f_Timer > 1.0f && f_Timer < 5.0f) go_One.GetComponent<Image>().enabled = true;
        else go_One.GetComponent<Image>().enabled = false;

        if (f_Timer > 7.0f && f_Timer < 12.0f) go_Two.GetComponent<Image>().enabled = true;
        else go_Two.GetComponent<Image>().enabled = false;

        if (f_Timer > 15f) { Application.Quit(); print("Quit"); }
    }
}
