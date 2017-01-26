using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Cs_HUDController : MonoBehaviour
{
    float f_Timer;
    float f_HangTime;
    int i_NumTimesCaught;
    bool b_IsCaught;

    bool b_FadeInText = true;
    GameObject txt_Timer;

    float f_TotalGameTimer;

	// Use this for initialization
	void Start ()
    {
        f_HangTime = 0f;

        i_NumTimesCaught = 0;

        b_IsCaught = false;

        txt_Timer = transform.Find("Text_Timer").gameObject;
        txt_CaughtText = transform.Find("Text_CaughtCounter").gameObject;

        Set_CaughtText();
    }

    void FadeText( bool b_FadeIn_ )
    {
        Color clr_CurrAlpha = txt_Timer.GetComponent<Text>().color;

        if( b_FadeIn_ )
        {
            // Just turn on the text since the player was caught
            clr_CurrAlpha.a = 1.0f;
        }
        else
        {
            clr_CurrAlpha.a -= Time.deltaTime;

            if (clr_CurrAlpha.a < 0.0f) clr_CurrAlpha.a = 0.0f;
        }

        txt_Timer.GetComponent<Text>().color = clr_CurrAlpha;
    }

    public bool Set_ScreenTimer
    {
        set
        {
            // If value is true (Activated)
            if( value )
            { 
                // If the player wasn't previously caught, increment the timer and set the bool to true
                if(!b_IsCaught)
                {
                    b_IsCaught = true;

                    ++i_NumTimesCaught;
                    Set_CaughtText();
                }

                // Set the text object to show 10:00 Countdown
                f_Timer = 10f;
                
                // Set the Hangtime to 3.0f for when the player successfully escapes (Fades out the screen text)
                f_HangTime = 3.0f;
            }
        }
    }

    void Set_ScreenText( float f_Time_ )
    {
        // Set the text's color
        if(f_Time_ == 10f)
        {
            txt_Timer.GetComponent<Text>().color = Color.red;
        }
        else if( f_Time_ > 0f )
        {
            txt_Timer.GetComponent<Text>().color = Color.yellow;
        }
        else // If f_Time == 0f
        {
            if(f_HangTime > 0f)
            {
                f_HangTime -= Time.deltaTime;

                if (f_HangTime < 0f) f_HangTime = 0f;
            }

            if(f_HangTime == 0f)
            {
                // Get the text's alpha and reduce it
                Color clr_Alpha = txt_Timer.GetComponent<Text>().color;
                clr_Alpha.a -= Time.deltaTime;
                if (clr_Alpha.a < 0f) clr_Alpha.a = 0f;
                txt_Timer.GetComponent<Text>().color = clr_Alpha;
            }
            else // Otherwise we're still green
            {
                txt_Timer.GetComponent<Text>().color = Color.green;
            }
        }

        int i_Seconds;
        float f_Milliseconds;
        if (f_Time_ > 0f)
        {
            i_Seconds = (int)f_Time_;
            f_Milliseconds = (f_Time_ - i_Seconds) * 100f;
        }
        else
        {
            i_Seconds = 0;
            f_Milliseconds = 0f;
        }

        txt_Timer.GetComponent<Text>().text = string.Format("{0:00}:{1:00}", i_Seconds, f_Milliseconds);
    }

    void Set_CaughtText()
    {
        if (i_NumTimesCaught == 0)
        {
            txt_CaughtText.GetComponent<Text>().color = Color.blue;
        }
        else if( i_NumTimesCaught < 4 )
        {
            txt_CaughtText.GetComponent<Text>().color = Color.green;
        }
        else if (i_NumTimesCaught < 7)
        {
            txt_CaughtText.GetComponent<Text>().color = Color.yellow;
        }
        else
        {
            txt_CaughtText.GetComponent<Text>().color = Color.red;
        }

        txt_CaughtText.GetComponent<Text>().text = i_NumTimesCaught.ToString();
    }

    public int TimesCaught
    {
        get { return i_NumTimesCaught; }
    }

    public float TotalGameTime
    {
        get { return f_TotalGameTimer; }
    }

    // Update is called once per frame
    GameObject txt_CaughtText;
	void Update ()
    {
        f_TotalGameTimer += Time.deltaTime;

        Set_ScreenText(f_Timer);
        
        if (b_IsCaught)
        {
            f_Timer -= Time.deltaTime;

            if(f_Timer < 0f)
            {
                b_IsCaught = false;
            }
        }
    }
}
