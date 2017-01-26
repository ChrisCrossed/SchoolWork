using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Runtime.InteropServices;

public class Cs_TextHint : MonoBehaviour
{
    float f_Timer;
    float f_Timer_Max = 5;
    string s_TextHint;
    GameObject go_TextHint;

	// Use this for initialization
	void Start ()
    {
        f_Timer = f_Timer_Max;

        go_TextHint = GameObject.Find("Text_Hint");
	}
	
	// Update is called once per frame
	void Update ()
    {
	    if(f_Timer < f_Timer_Max)
        {
            f_Timer += Time.deltaTime;

            if (f_Timer > f_Timer_Max) f_Timer = f_Timer_Max;
        }
        else
        {
            go_TextHint.GetComponent<Text>().text = "";
        }
	}

    public void Set_TextHint( string s_TextHint_ )
    {
        // Reset timer
        f_Timer = 0f;

        s_TextHint = s_TextHint_.Replace("//", "\n");

        go_TextHint.GetComponent<Text>().text = s_TextHint;
    }
}
