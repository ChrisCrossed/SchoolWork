using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Cs_MissionReport : MonoBehaviour
{
    // The grading on screen that will change based on player's behavior
    GameObject go_MR_CaughtCounter;
    GameObject go_MR_Timer;
    GameObject go_MR_GradeEvaluation;

    // The static text
    GameObject go_MR_CaughtCounter_Text;
    GameObject go_MR_Timer_Text;
    GameObject go_MR_GradeEvaluation_Text;

    Color clr_WhiteOff;

    bool b_IsGrading;

    Cs_HUDController gameplayStats;
    float f_TotalPlayTime;
    int i_NumTimesCaught;

    // Use this for initialization
    void Start ()
    {
        gameplayStats = gameObject.GetComponent<Cs_HUDController>();

        go_MR_CaughtCounter = transform.Find("MR_CaughtCounter").gameObject;
        go_MR_Timer = transform.Find("MR_Timer").gameObject;
        go_MR_GradeEvaluation = transform.Find("MR_Ranking").gameObject;

        go_MR_CaughtCounter_Text = transform.Find("MR_CaughtCounter_Text").gameObject;
        go_MR_Timer_Text = transform.Find("MR_Timer_Text").gameObject;
        go_MR_GradeEvaluation_Text = transform.Find("MR_Ranking_Text").gameObject;

        // Set all text to White and turn off their alpha
        clr_WhiteOff = new Color(1, 1, 1, 0);

        go_MR_CaughtCounter.GetComponent<Text>().color = clr_WhiteOff;
        go_MR_Timer.GetComponent<Text>().color = clr_WhiteOff;
        go_MR_GradeEvaluation.GetComponent<Text>().color = clr_WhiteOff;
        go_MR_CaughtCounter_Text.GetComponent<Text>().color = clr_WhiteOff;
        go_MR_Timer_Text.GetComponent<Text>().color = clr_WhiteOff;
        go_MR_GradeEvaluation_Text.GetComponent<Text>().color = clr_WhiteOff;
    }

    public void Set_ActivateGrading()
    {
        // Activate bool to begin timer
        b_IsGrading = true;

        i_NumTimesCaught = gameplayStats.TimesCaught;
        f_TotalPlayTime = gameplayStats.TotalGameTime;

        print("Time: " + f_TotalPlayTime);
    }

    // Update is called once per frame
    float f_Timer;
    float f_Increment_Delay = 0.35f;
    float f_Increment_Delay_Max = 0.35f;
    int i_NumTimesCaught_Counter;
    float f_TotalPlayTime_Counter;
    char c_Grade;
    float f_Alpha = 1.0f;
    void Update ()
    {
	    if(b_IsGrading)
        {
            f_Timer += Time.deltaTime;

            Color clr_WhiteOn = new Color(1, 1, 1, 1);

            #region Caught Counter
            if (f_Timer > 1.0f)
            {
                // If the text is 'Off'
                if(go_MR_CaughtCounter_Text.GetComponent<Text>().color.a == 0f)
                {
                    // Turn on the text
                    go_MR_CaughtCounter_Text.GetComponent<Text>().color = clr_WhiteOn;

                    // TODO: Play SFX

                }
            }
            if (f_Timer > 2.0f)
            {
                // If the text is 'Off'
                if (go_MR_CaughtCounter.GetComponent<Text>().color.a == 0f)
                {
                    // Turn on the text
                    go_MR_CaughtCounter.GetComponent<Text>().color = clr_WhiteOn;
                }

                // Increment the number of times caught
                if(i_NumTimesCaught_Counter < i_NumTimesCaught)
                {
                    // Count down for 0.1f seconds
                    if(f_Increment_Delay > 0f)
                    {
                        f_Increment_Delay -= Time.deltaTime;
                    }
                    
                    // After 0.1f have passed, increment the counter of times caught by one until we reach what the player hit
                    if(f_Increment_Delay < 0)
                    {
                        // Every time we increment the counter, decrement the f_Timer to keep times accurate
                        f_Timer -= f_Increment_Delay_Max;

                        // Reset the delay timer
                        f_Increment_Delay = f_Increment_Delay_Max;

                        // Increment the counter
                        ++i_NumTimesCaught_Counter;
                        
                        // TODO: Play SFX
                    }
                }

                #region Set color of text
                if (i_NumTimesCaught_Counter == 0)
                {
                    // Blue
                    go_MR_CaughtCounter.GetComponent<Text>().color = Color.cyan;
                }
                else if(i_NumTimesCaught_Counter < 4)
                {
                    // Green
                    go_MR_CaughtCounter.GetComponent<Text>().color = Color.green;
                }
                else if (i_NumTimesCaught_Counter < 7)
                {
                    // Green
                    go_MR_CaughtCounter.GetComponent<Text>().color = Color.yellow;
                }
                else
                {
                    go_MR_CaughtCounter.GetComponent<Text>().color = Color.red;
                }
                #endregion

                go_MR_CaughtCounter.GetComponent<Text>().text = i_NumTimesCaught_Counter.ToString();
            }
            #endregion

            #region Game Timer
            if (f_Timer > 4.0f)
            {
                // If the text is 'Off'
                if (go_MR_Timer_Text.GetComponent<Text>().color.a == 0f)
                {
                    // Turn on the text
                    go_MR_Timer_Text.GetComponent<Text>().color = clr_WhiteOn;

                    // TODO: Play SFX


                }
            }
            if (f_Timer > 5.0f)
            {
                // If the text is 'Off'
                if (go_MR_Timer.GetComponent<Text>().color.a == 0f)
                {
                    // Turn on the text
                    go_MR_Timer.GetComponent<Text>().color = clr_WhiteOn;

                    // TODO: Play SFX

                }
                
                // Increment the number of times caught
                if (f_TotalPlayTime_Counter < f_TotalPlayTime)
                {
                    // Increment counter at faster speed
                    f_TotalPlayTime_Counter += Time.deltaTime * 100f;

                    // Compensate for time increase
                    f_Timer -= Time.deltaTime;

                    // Cap
                    if (f_TotalPlayTime_Counter > f_TotalPlayTime) f_TotalPlayTime_Counter = f_TotalPlayTime;
                    
                    // TODO: Play SFX

                }

                #region Set color of text

                // Under 5 minutes
                if (f_TotalPlayTime_Counter <= 2.0f * 60f)
                {
                    // Blue
                    go_MR_Timer.GetComponent<Text>().color = Color.cyan;

                    if (i_NumTimesCaught == 0) c_Grade = 'S';
                    else if (i_NumTimesCaught < 4) c_Grade = 'A';
                    else if (i_NumTimesCaught < 7) c_Grade = 'B';
                    else c_Grade = 'C';
                }
                else if (f_TotalPlayTime_Counter <= 3.0f * 60f)
                {
                    // Green
                    go_MR_Timer.GetComponent<Text>().color = Color.green;

                    if (i_NumTimesCaught < 4) c_Grade = 'A';
                    else if (i_NumTimesCaught < 7) c_Grade = 'B';
                    else c_Grade = 'C';
                }
                else if (f_TotalPlayTime_Counter <= 3.5f * 60f)
                {
                    // Green
                    go_MR_Timer.GetComponent<Text>().color = Color.yellow;

                    if (i_NumTimesCaught < 7) c_Grade = 'B';
                    else c_Grade = 'C';
                }
                else
                {
                    go_MR_Timer.GetComponent<Text>().color = Color.red;

                    c_Grade = 'C';
                }
                #endregion

                #region Set minutes/seconds/milliseconds
                int i_Minutes = (int)(f_TotalPlayTime_Counter / 60f);
                if(f_TotalPlayTime_Counter < 60f)
                {
                    i_Minutes = 0;
                }
                int i_Seconds = (int)(f_TotalPlayTime_Counter - (i_Minutes * 60));
                float f_Milliseconds = ((f_TotalPlayTime_Counter % 60f) * 100f) - (i_Seconds * 100);
                #endregion

                string s_TextTimer = string.Format("{0:00}:{1:00}:{2:00}", i_Minutes, i_Seconds, f_Milliseconds);
                go_MR_Timer.GetComponent<Text>().text = s_TextTimer;
            }
            #endregion

            #region Mission Rating
            if (f_Timer > 8.0f)
            {
                // If the text is 'Off'
                if (go_MR_GradeEvaluation_Text.GetComponent<Text>().color.a == 0f)
                {
                    // Turn on the text
                    go_MR_GradeEvaluation_Text.GetComponent<Text>().color = clr_WhiteOn;

                    // TODO: Play SFX


                }
            }
            if (f_Timer > 10.0f)
            {
                // If the text is 'Off'
                if (go_MR_GradeEvaluation.GetComponent<Text>().color.a == 0f)
                {
                    // Turn on the text
                    if(c_Grade == 'S') go_MR_GradeEvaluation.GetComponent<Text>().color = Color.cyan;
                    else if(c_Grade == 'A') go_MR_GradeEvaluation.GetComponent<Text>().color = Color.green;
                    else if (c_Grade == 'B') go_MR_GradeEvaluation.GetComponent<Text>().color = Color.yellow;
                    else if (c_Grade == 'C') go_MR_GradeEvaluation.GetComponent<Text>().color = Color.red;

                    // Set text
                    go_MR_GradeEvaluation.GetComponent<Text>().text = c_Grade.ToString();

                    // TODO: Play SFX
                    
                }
            }

            // End Mission Rating.
            if(f_Timer > 15f)
            {
                // Go through all text and fade them out
                // Reduce/Cap Alpha
                if(f_Alpha > 0f)
                {
                    f_Alpha -= Time.deltaTime / 2f;
                    if(f_Alpha < 0f) f_Alpha = 0f;
                }

                // Reduce volume based on alpha
                GameObject.Find("MusicManager").GetComponent<AudioSource>().volume = f_Alpha / 2f;

                #region Set Alpha on all text
                Color clr_CurrColor = go_MR_CaughtCounter.GetComponent<Text>().color;
                clr_CurrColor.a = f_Alpha;
                go_MR_CaughtCounter.GetComponent<Text>().color = clr_CurrColor;

                clr_CurrColor = go_MR_Timer.GetComponent<Text>().color;
                clr_CurrColor.a = f_Alpha;
                go_MR_Timer.GetComponent<Text>().color = clr_CurrColor;

                clr_CurrColor = go_MR_GradeEvaluation.GetComponent<Text>().color;
                clr_CurrColor.a = f_Alpha;
                go_MR_GradeEvaluation.GetComponent<Text>().color = clr_CurrColor;

                clr_CurrColor = go_MR_CaughtCounter_Text.GetComponent<Text>().color;
                clr_CurrColor.a = f_Alpha;
                go_MR_CaughtCounter_Text.GetComponent<Text>().color = clr_CurrColor;

                clr_CurrColor = go_MR_Timer_Text.GetComponent<Text>().color;
                clr_CurrColor.a = f_Alpha;
                go_MR_Timer_Text.GetComponent<Text>().color = clr_CurrColor;

                clr_CurrColor = go_MR_GradeEvaluation_Text.GetComponent<Text>().color;
                clr_CurrColor.a = f_Alpha;
                go_MR_GradeEvaluation_Text.GetComponent<Text>().color = clr_CurrColor;
                #endregion

                if (f_Timer > 18f)
                {
                    // Return to menu
                    SceneManager.LoadScene(2);
                }
            }
            #endregion
        }
    }
}
