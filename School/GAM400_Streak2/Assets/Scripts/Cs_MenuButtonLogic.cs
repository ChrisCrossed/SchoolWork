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

    GameObject go_Button;
    [SerializeField] AnimationCurve lerpCurve;
    float f_LerpTimer;
    float f_ZPos = 5f;

	// Use this for initialization
	void Start ()
    {
        clr_Deselected = mat_Deselected.color;
        clr_Selected = mat_Selected.color;

        mat_ThisMat = gameObject.GetComponent<MeshRenderer>().materials[1];

        go_Button = transform.Find("Button").gameObject;

        Vector3 v3_ButtonPos = gameObject.transform.position;
        v3_ButtonPos.z = f_ZPos;
        go_Button.transform.position = v3_ButtonPos;
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

            // Lerp Button Position outward
            if(f_LerpTimer < 1.0f)
            {
                // Increase lerp timer
                f_LerpTimer += Time.deltaTime * 3f;
                if (f_LerpTimer > 1.0f) f_LerpTimer = 1.0f;
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

            // Lerp Button Position outward
            if (f_LerpTimer > 0.0f)
            {
                // Increase lerp timer
                f_LerpTimer -= Time.deltaTime * 3f;
                if (f_LerpTimer < 0.0f) f_LerpTimer = 0.0f;
            }
        }

        // Evaluate lerp timer
        float f_Evaluate = lerpCurve.Evaluate(f_LerpTimer);

        // Set positions
        Vector3 v3_StartPos = gameObject.transform.position;
        v3_StartPos.z = f_ZPos;

        Vector3 v3_FinalPos = v3_StartPos;
        v3_FinalPos.x += 100f;

        Vector3 v3_NewPos = Vector3.Lerp(v3_StartPos, v3_FinalPos, f_Evaluate);
        go_Button.transform.position = v3_NewPos;
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
            if( b_IsSelected != value )
            {
                // f_LerpTimer = 0f;
            }

            b_IsSelected = value;
        }
        get { return b_IsSelected; }
    }
}
