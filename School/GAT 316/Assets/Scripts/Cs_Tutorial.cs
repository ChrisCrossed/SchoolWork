using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Cs_Tutorial : MonoBehaviour
{
    GameObject go_Sneak;
    GameObject go_Walk;
    GameObject go_Run;

    float f_Timer_Sneak;
    float f_Timer_Walk;
    float f_Timer_Run;

    float f_TutorialTimer;
    float f_FadeTimer;
    bool b_DeactivateScript;
    [SerializeField] float f_BeginFadeInAt = 3.0f;
    float f_FadeOutAt;

    // Use this for initialization
    void Start ()
    {
        GameObject.Find("FadeInOut").GetComponent<Image>().enabled = true;

        go_Sneak = transform.Find("Hint_Sneak").gameObject;
        go_Walk = transform.Find("Hint_Walk").gameObject;
        go_Run = transform.Find("Hint_Run").gameObject;

        Color clr_AlphaOff = go_Sneak.GetComponent<Image>().color;
        clr_AlphaOff.a = 0f;

        go_Sneak.GetComponent<Image>().color = clr_AlphaOff;
        go_Walk.GetComponent<Image>().color = clr_AlphaOff;
        go_Run.GetComponent<Image>().color = clr_AlphaOff;

        f_FadeOutAt = f_BeginFadeInAt + 180f;
    }

    public void Set_DeactivateTutorial()
    {
        f_TutorialTimer = f_FadeOutAt;
    }
	
	// Update is called once per frame
	void Update ()
    {
        // Becomes deactivated when not needed anymore
        if(!b_DeactivateScript)
        {
            // If we're within the timer to begin fading in
            if(f_TutorialTimer < f_FadeOutAt)
            {
                f_TutorialTimer += Time.deltaTime;  

                // Sneak movement
                if(f_TutorialTimer > f_BeginFadeInAt)
                {
                    f_Timer_Sneak += Time.deltaTime;
                    if (f_Timer_Sneak > 1.0f) f_Timer_Sneak = 1.0f;

                    Color clr_Alpha = go_Sneak.GetComponent<Image>().color;
                    clr_Alpha.a = f_Timer_Sneak;
                    go_Sneak.GetComponent<Image>().color = clr_Alpha;
                }

                // Walk movement
                if (f_TutorialTimer > f_BeginFadeInAt + 1.5f)
                {
                    f_Timer_Walk += Time.deltaTime;
                    if (f_Timer_Walk > 1.0f) f_Timer_Walk = 1.0f;

                    Color clr_Alpha = go_Walk.GetComponent<Image>().color;
                    clr_Alpha.a = f_Timer_Walk;
                    go_Walk.GetComponent<Image>().color = clr_Alpha;
                }

                // Sprint movement
                if (f_TutorialTimer > f_BeginFadeInAt + 3.0f)
                {
                    f_Timer_Run += Time.deltaTime;
                    if (f_Timer_Run > 1.0f) f_Timer_Run = 1.0f;

                    Color clr_Alpha = go_Run.GetComponent<Image>().color;
                    clr_Alpha.a = f_Timer_Run;
                    go_Run.GetComponent<Image>().color = clr_Alpha;
                }
            }
            // Fade out all objects
            else
            {
                f_FadeTimer += Time.deltaTime;

                // Timer is less than 1.0f
                if (f_FadeTimer < 1.0f)
                {
                    // Runs from 1.0f to 0.0f
                    float f_Perc = 1f - f_FadeTimer;

                    // Fade out each object individually
                    Color clr_CurrAlpha = go_Sneak.GetComponent<Image>().color;
                    clr_CurrAlpha.a = f_Perc;

                    go_Sneak.GetComponent<Image>().color = clr_CurrAlpha;
                    go_Walk.GetComponent<Image>().color = clr_CurrAlpha;
                    go_Run.GetComponent<Image>().color = clr_CurrAlpha;
                }
                else // Timer is greater than 1.0f. End the script.
                {
                    go_Sneak.GetComponent<Image>().enabled = false;
                    go_Walk.GetComponent<Image>().enabled = false;
                    go_Run.GetComponent<Image>().enabled = false;

                    b_DeactivateScript = true;
                    return;
                }
            }
        }
    }
}
