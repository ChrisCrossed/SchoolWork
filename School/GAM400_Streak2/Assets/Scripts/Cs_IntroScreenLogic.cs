using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using XInputDotNetPure;

/*********************************
 * 
 * Copyright DigiPen Institute of Technology 2016
 * 
 * Streak 2 by Christopher Christensen
 * 
 * *******************************/

public class Cs_IntroScreenLogic : MonoBehaviour
{
    [SerializeField] GameObject go_Graphic;
    [SerializeField] int i_LevelToGoTo;

    float f_LevelTimer = -1f;
    [SerializeField] float f_LevelTimer_Max = 8.0f; // The overall amount of time until the next scene loads

    float f_LerpPerc;
    static float f_LerpPerc_Max = 2.0f; // The amount of seconds it takes to lerp between 0% and 100%

    // Controller Input
    PlayerIndex pad_PlayerOne = PlayerIndex.One;
    GamePadState state_p1;
    GamePadState prevState_p1;

    // Update is called once per frame
    void Update ()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        #region Update Controller State
        prevState_p1 = state_p1;
        state_p1 = GamePad.GetState(pad_PlayerOne);
        #endregion

        if( Input.GetKeyDown(KeyCode.Space) ||
            Input.GetKeyDown(KeyCode.Escape) ||
            state_p1.Buttons.A == ButtonState.Pressed || 
            state_p1.Buttons.Start == ButtonState.Pressed ||
            state_p1.Buttons.Back == ButtonState.Pressed)
        {
            f_LevelTimer = f_LevelTimer_Max + 1f;
        }

        UpdateLerpTimer();

        // Lerp from the previous graphic's alpha value to its new alpha
        if(go_Graphic != null)
        {
            Color clr_Curr = go_Graphic.GetComponent<Image>().color;
            clr_Curr.a = f_LerpPerc;
            go_Graphic.GetComponent<Image>().color = clr_Curr;
        }
	}

    void UpdateLerpTimer()
    {
        f_LevelTimer += Time.deltaTime;

        if (f_LevelTimer < 0) return;

        // If the timer is less than the time needed to reach 100%...
        if (f_LevelTimer < f_LerpPerc_Max)
        {
            // Store that percent
            f_LerpPerc = f_LevelTimer / f_LerpPerc_Max;
        }
        // Otherwise, if we're greater than the minimum threshold, we begin to revert from 100% back to 0%
        else if (f_LevelTimer > f_LevelTimer_Max - f_LerpPerc_Max && f_LevelTimer < f_LevelTimer_Max)
        {
            // Store that percent
            f_LerpPerc = (f_LevelTimer_Max - f_LevelTimer) / f_LerpPerc_Max;
        }
        // Just switch scenes
        else if(f_LevelTimer >= f_LevelTimer_Max + 1f)
        {
            SceneManager.LoadScene(i_LevelToGoTo);
        }
    }

    void ApplyNewAlpha()
    {

    }

    public float Get_SceneMaxTime()
    {
        return f_LevelTimer_Max;
    }
}
