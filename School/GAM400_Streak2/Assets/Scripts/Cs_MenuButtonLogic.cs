using UnityEngine;
using System.Collections;

/*********************************
 * 
 * Copyright DigiPen Institute of Technology 2016
 * 
 * Streak 2 by Christopher Christensen
 * 
 * *******************************/

public class Cs_MenuButtonLogic : MonoBehaviour
{
    [SerializeField] Material mat_Deselected;
    [SerializeField] Material mat_Selected;

    Material mat_ThisMat;

    Color clr_Deselected;
    Color clr_Selected;

    float f_ColorLerpTimer;

    bool b_IsSelected;

	// Use this for initialization
	void Start ()
    {
        clr_Deselected = mat_Deselected.color;
        clr_Selected = mat_Selected.color;

        mat_ThisMat = gameObject.GetComponent<MeshRenderer>().materials[1];
        
        
    }

    public void Init_Button( bool b_IsEnabled_ = false )
    {
        IsSelected = b_IsSelected;

        if (b_IsSelected)
        {
            f_ColorLerpTimer = .99f;
        }
        else
        {
            f_ColorLerpTimer = 0.01f;
        }
    }

    // Update is called once per frame
    Color clr_CurrColor;
	void Update ()
    {
        if ( b_IsSelected )
        {
            if(f_ColorLerpTimer < 1.0f)
            {
                #region Increase/Cap Timer
                f_ColorLerpTimer += Time.deltaTime * 7.5f;

                if (f_ColorLerpTimer > 1.0f) f_ColorLerpTimer = 1.0f;
                #endregion

                clr_CurrColor = Color.Lerp(clr_Deselected, clr_Selected, f_ColorLerpTimer / 1.0f);

                #region Set new material color
                SetMaterial();
                #endregion
            }
        }
        else // Not selected
        {
            if( f_ColorLerpTimer > 0.0f )
            {
                #region Decrease/Cap Timer
                f_ColorLerpTimer -= Time.deltaTime * 7.5f;
                if (f_ColorLerpTimer < 0.0f) f_ColorLerpTimer = 0.0f;
                #endregion

                clr_CurrColor = Color.Lerp(clr_Selected, clr_Deselected, 1f - (f_ColorLerpTimer / 1.0f));

                #region Set new material color
                SetMaterial();
                #endregion
            }
        }
    }

    void SetMaterial()
    {
        // Set current material
        mat_ThisMat.color = clr_CurrColor;
    }

    public bool IsSelected
    {
        set
        {
            b_IsSelected = value;
        }
        get { return b_IsSelected; }
    }
}
