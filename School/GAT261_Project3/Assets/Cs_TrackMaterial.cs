using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cs_TrackMaterial : MonoBehaviour
{
    Material this_Mat;
    Color this_Color;

	// Use this for initialization
	void Start ()
    {
        
	}

    // Update is called once per frame
    bool b_IsGoingUp;
	void Update ()
    {
        this_Mat = gameObject.GetComponent<MeshRenderer>().material;
        this_Color = this_Mat.color;

        float f_Timer = Time.deltaTime / 2f;

        if (b_IsGoingUp)
        {
            if(this_Color.r < 1.0f)
            {
                this_Color.r += f_Timer;
                if (this_Color.r > 1.0f) this_Color.r = 1.0f;
            }
            else
            {
                this_Color.b += f_Timer;
                if (this_Color.b > 1.0f)
                {
                    this_Color.b = 1.0f;

                    b_IsGoingUp = false;
                }
            }
        }
        else
        {
            if (this_Color.r > 0f)
            {
                this_Color.r -= f_Timer;
                if (this_Color.r < 0f) this_Color.r = 0f;
            }
            else
            {
                this_Color.b -= f_Timer;
                if (this_Color.b < 0f)
                {
                    this_Color.b = 0f;

                    b_IsGoingUp = true;
                }
            }
        }

        gameObject.GetComponent<MeshRenderer>().material.color = this_Color;
	}
}
