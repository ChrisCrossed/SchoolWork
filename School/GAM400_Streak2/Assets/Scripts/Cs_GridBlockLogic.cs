using UnityEngine;
using System.Collections;

/*********************************
 * 
 * Copyright DigiPen Institute of Technology 2016
 * 
 * Streak 2 by Christopher Christensen
 * 
 * *******************************/

public enum Enum_FadeState
{
    FadeIn,
    Stay,
    FadeOut
}

public enum Enum_ColorState
{
    Original,
    Red,
    Blue,
    White
}

public class Cs_GridBlockLogic : MonoBehaviour
{
    float f_FadeTimer = 0.0f;
    float f_FadeTimer_Max = 0.5f;
    float f_FadePercent = 0.0f;

    // Store original colors
    Color clr_OriginalColor;
    Color clr_Red;
    Color clr_Blue;
    Color clr_White;

    // The color the block will lerp to when set within Set_ColorState
    GameObject go_Backdrop;
    Color clr_CurrentColor;
    float f_ColorTimer;
    float f_ColorTimer_Max = 0.5f;
    float f_ColorPercent = 0.0f;
    
    Enum_FadeState e_FadeState;
    Enum_ColorState e_ColorState;

	// Use this for initialization
	void Start ()
    {
        if(transform.Find("Grid_Backdrop"))
        {
            go_Backdrop = transform.Find("Grid_Backdrop").gameObject;
        }

        Color clr_CurrMat = gameObject.GetComponent<MeshRenderer>().material.color;
        clr_CurrMat.a = 0;
        gameObject.GetComponent<MeshRenderer>().material.color = clr_CurrMat;

        // Tells the block to stay invisible so external forces can fade them in manually
        Set_FadeState( Enum_FadeState.Stay );

        // Set colors for the block
        clr_OriginalColor = new Color(0, 0, 0, 0);
        clr_Blue = new Color(0, 0, 1, 0.15f);
        clr_Red = new Color(1, 0, 0, 0.15f);
        clr_White = new Color(1, 1, 1, 0.15f);
    }

    public void Set_FadeState( Enum_FadeState e_FadeState_ )
    {
        e_FadeState = e_FadeState_;
    }

    public void Set_ColorState( Enum_ColorState e_ColorState_, bool b_IsInstant_ )
    {
        if( e_ColorState_ == Enum_ColorState.Original )
        {
            // Set the current color to be original. Gets lerped to within Update
            clr_CurrentColor = clr_OriginalColor;
        }
        else if( e_ColorState_ == Enum_ColorState.Red )
        {
            clr_CurrentColor = clr_Red;
        }
        else if( e_ColorState_ == Enum_ColorState.Blue )
        {
            clr_CurrentColor = clr_Blue;
        }
        else if( e_ColorState_ == Enum_ColorState.White )
        {
            clr_CurrentColor = clr_White;
        }

        if(b_IsInstant_)
        {
            f_ColorTimer = f_ColorTimer_Max;
            f_ColorPercent = 1.0f;
        }
        else
        {
            f_ColorTimer = 0.0f;
            f_ColorPercent = 0.0f;
        }
    }

    void UpdateFadeTimers()
    {
        if (e_FadeState == Enum_FadeState.FadeIn)
        {
            // Increment Fade Timer
            if (f_FadeTimer < f_FadeTimer_Max)
            {
                f_FadeTimer += Time.deltaTime;

                if (f_FadeTimer > f_FadeTimer_Max) f_FadeTimer = f_FadeTimer_Max;
            }

            // Set Fade Percent
            f_FadePercent = f_FadeTimer / f_FadeTimer_Max;

            if (f_FadePercent == 1.0f)
            {
                e_FadeState = Enum_FadeState.Stay;
            }
        }
        else if (e_FadeState == Enum_FadeState.FadeOut)
        {
            // Decrement Fade Timer
            if (f_FadeTimer > 0)
            {
                f_FadeTimer -= Time.deltaTime;

                if (f_FadeTimer < 0) f_FadeTimer = 0;
            }

            // Set Fade Percent
            f_FadePercent = f_FadeTimer / f_FadeTimer_Max;

            if (f_FadePercent == 0f) e_FadeState = Enum_FadeState.Stay;
        }
    }

    void UpdateBlockAlpha()
    {
        if(e_FadeState == Enum_FadeState.FadeIn || e_FadeState == Enum_FadeState.FadeOut)
        {
            Color clr_CurrMat = gameObject.GetComponent<MeshRenderer>().material.color;
            clr_CurrMat.a = f_FadePercent;
            gameObject.GetComponent<MeshRenderer>().material.color = clr_CurrMat;
        }
    }

    void UpdateBlockColor()
    {
        if(f_ColorTimer < f_ColorTimer_Max)
        {
            f_ColorTimer += Time.deltaTime;

            if (f_ColorTimer > f_ColorTimer_Max) f_ColorTimer = f_ColorTimer_Max;

            f_ColorPercent = f_ColorTimer / f_ColorTimer_Max;
        }

        // Fade from old color into current color
        if(go_Backdrop)
        {
            Color clr_OldColor = go_Backdrop.GetComponent<MeshRenderer>().material.color;
            clr_OldColor = Color.Lerp(clr_OldColor, clr_CurrentColor, f_ColorPercent);
            go_Backdrop.GetComponent<MeshRenderer>().material.color = clr_OldColor;
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        UpdateBlockAlpha();

        UpdateFadeTimers();

        UpdateBlockColor();
	}
}
