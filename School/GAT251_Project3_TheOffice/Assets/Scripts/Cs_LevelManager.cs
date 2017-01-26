using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Cs_LevelManager : MonoBehaviour
{
    bool b_Time_AM;
    int i_Time_Hours;
    int i_Time_Minutes;
    float f_Time_Seconds;

    float f_DecrementingTimer = 0.85f;

    Text txt_ClockText;
    TextMesh txt_TimeClockText;

    Cs_ObjectiveManager ObjectiveManager;

    public bool b_AllowedToContinue;

	// Use this for initialization
	void Start ()
    {
        // Connect items
        txt_ClockText = GameObject.Find("Text_Clock").GetComponent<Text>();
        txt_TimeClockText = GameObject.Find("PunchClockText").GetComponent<TextMesh>();

        // 8 AM, start of work day
        i_Time_Hours = 7;
        i_Time_Minutes = 45;
        b_Time_AM = true;

        // Objective Manager
        ObjectiveManager = GameObject.Find("LevelManager").GetComponent<Cs_ObjectiveManager>();
    }

    public void Set_AllowedToContinue()
    {
        b_AllowedToContinue = true;
    }
	
    void ManageClock()
    {
        if(b_AllowedToContinue || i_Time_Minutes < 59)
        {
            f_Time_Seconds += Time.deltaTime;
        }

        if(f_Time_Seconds > f_DecrementingTimer)
        {
            ++i_Time_Minutes;
            f_Time_Seconds = 0f;

            if(i_Time_Minutes >= 60)
            {
                i_Time_Minutes = 0;

                ++i_Time_Hours;

                if(i_Time_Hours > 12)
                {
                    i_Time_Hours = 1;

                    b_Time_AM = !b_Time_AM;
                }

                if(!gameObject.GetComponent<Cs_ObjectiveManager>().ClockOutStatus)
                {
                    AudioClip ac_Beep = Resources.Load("sfx_Beep") as AudioClip;
                    GameObject.Find("Player").GetComponent<AudioSource>().PlayOneShot(ac_Beep);
                }
            }

            // On appropriate times, give the player a task
            if (i_Time_Minutes == 0 || i_Time_Minutes == 15 || i_Time_Minutes == 30 || i_Time_Minutes == 45)
            {
                if(ObjectiveManager.ClockIn && !ObjectiveManager.b_JobTestEnvironment)
                {
                    ObjectiveManager.CreateNewJob();
                }
            }

            if (gameObject.GetComponent<Cs_ObjectiveManager>().ClockOutStatus)
            {
                if ((i_Time_Hours == 5 && i_Time_Minutes >= 14) || (i_Time_Hours > 5))
                {
                    if (i_Time_Minutes % 15 == 0)
                    {
                        GameObject.Find("TutorialText").GetComponent<Text>().text = "America does not pay overtime, Comrade!";
                    }
                }
            }

            if(i_Time_Minutes % 10 == 0)
            {
                if(gameObject.GetComponent<Cs_ObjectiveManager>().ClockOutStatus)
                {
                    f_DecrementingTimer = 0.65f;

                }
                else
                {
                    f_DecrementingTimer -= 0.025f;
                    if (f_DecrementingTimer < .4f) f_DecrementingTimer = 0.4f;
                }
            }

            txt_ClockText.text = string.Format("{0:0}:{1:00}", i_Time_Hours, i_Time_Minutes);
            txt_TimeClockText.text = string.Format("{0:0}:{1:00}", i_Time_Hours, i_Time_Minutes);
        }
    }

    public int GetTime_Minutes
    {
        get { return i_Time_Minutes; }
    }

    public int GetTime_Hours
    {
        get { return i_Time_Hours; }
    }

	// Update is called once per frame
	void Update ()
    {
        ManageClock();

        if(Input.GetKeyDown(KeyCode.P))
        {
            i_Time_Hours = 3;
            i_Time_Minutes = 50;
            b_Time_AM = false;
        }
	}
}
