using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cs_ControlsTutorial : MonoBehaviour
{
    float f_Timer;
    float f_Timer_Max = 8.0f;

    [SerializeField] bool Initial;

    Image img_;
	// Use this for initialization
	void Start ()
    {
        img_ = gameObject.GetComponent<Image>();

        img_.enabled = true;

        Color clr_ = img_.color;
        clr_.a = 0f;
        img_.color = clr_;

        f_Timer = f_Timer_Max;
	}

    bool b_CanActivate = true;
    public void Activate()
    {
        if(b_CanActivate)
        {
            f_Timer = 0f;

            b_CanActivate = false;
        }
    }

    // Update is called once per frame
    float f_Timer_Init;
    [SerializeField] float InitialTimer = 4.0f;
    void Update ()
    {
        if(Initial)
        {
            f_Timer_Init += Time.deltaTime;

            if (f_Timer_Init > InitialTimer)
            {
                Activate();
                Initial = false;
            }
        }

        if (f_Timer < f_Timer_Max) f_Timer += Time.deltaTime;

		if(f_Timer < 1.5f)
        {
            // Increase alpha
            float f_Perc = f_Timer / 1.5f;

            Color clr_ = img_.color;
            clr_.a = f_Perc;
            img_.color = clr_;
        }
        else if(f_Timer >= 1.5 && f_Timer < 6.5f)
        {
            // Sustain Alpha
            Color clr_ = img_.color;
            clr_.a = 1.0f;
            img_.color = clr_;
        }
        else if(f_Timer >= 6.5f && f_Timer < f_Timer_Max)
        {
            // Decrease alpha
            float f_Perc = f_Timer;
            f_Perc -= 6.5f;

            f_Perc = 1.0f - (f_Perc / (f_Timer_Max - 6.5f));

            Color clr_ = img_.color;
            clr_.a = f_Perc;
            img_.color = clr_;
        }
        else
        {
            Color clr_ = img_.color;
            clr_.a = 0f;
            img_.color = clr_;

            f_Timer = f_Timer_Max;

            b_CanActivate = true;
        }
	}
}
