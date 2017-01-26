using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/*********************************
 * 
 * Copyright DigiPen Institute of Technology 2016
 * 
 * Streak 2 by Christopher Christensen
 * 
 * *******************************/

public class Cs_Intro_TextLogic : MonoBehaviour
{
    float f_FadeTimer;
    [SerializeField] float f_FadeTimer_Start = 2.5f;
    [SerializeField] float f_FadeTimer_End = 6.0f;
    float f_TotalFadeTime;

    float f_MaxSceneTime;

    // Use this for initialization
    void Start ()
    {
        // Set Alpha to 0
        Color clr_CurrColor = gameObject.GetComponent<Text>().color;
        clr_CurrColor.a = 0f;
        gameObject.GetComponent<Text>().color = clr_CurrColor;

        f_TotalFadeTime = f_FadeTimer_End - f_FadeTimer_Start;

        f_MaxSceneTime = GameObject.Find("Canvas").GetComponent<Cs_IntroScreenLogic>().Get_SceneMaxTime();
    }

    // Update is called once per frame
    float f_Timer_Fade;
    void Update ()
    {
        f_FadeTimer += Time.deltaTime;

        if(f_FadeTimer >= f_FadeTimer_Start && f_FadeTimer < f_FadeTimer_End)
        {
            f_Timer_Fade += Time.deltaTime;
            if (f_Timer_Fade > 1.0f) f_Timer_Fade = 1.0f;
        }
        else if(f_FadeTimer > f_FadeTimer_End)
        {
            f_Timer_Fade -= Time.deltaTime * 2f;
            if (f_Timer_Fade < 0.0f) f_Timer_Fade = 0.0f;
        }

        gameObject.GetComponent<Text>().color = Color.Lerp(new Color(1, 1, 1, 0), new Color(1, 1, 1, 1), f_Timer_Fade);
	}
}
