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

    GameObject go_AButton;
    [SerializeField] AnimationCurve lerpCurve_AButton;
    [SerializeField] AnimationCurve lerpCurve_Position;
    float f_LerpTimer;
    float f_ZPos = 5f;

	// Use this for initialization
	void Start ()
    {
        clr_Deselected = mat_Deselected.color;
        clr_Selected = mat_Selected.color;

        mat_ThisMat = gameObject.GetComponent<MeshRenderer>().materials[1];

        go_AButton = transform.Find("Button").gameObject;

        Vector3 v3_ButtonPos = gameObject.transform.position;
        v3_ButtonPos.z = f_ZPos;
        go_AButton.transform.position = v3_ButtonPos;
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
                f_LerpTimer += Time.deltaTime * 10f;
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
                f_LerpTimer -= Time.deltaTime * 10f;
                if (f_LerpTimer < 0.0f) f_LerpTimer = 0.0f;
            }
        }

        #region Lerp Menu Button Position
        Vector3 v3_ButtonPosition_Start = gameObject.transform.localPosition;
        v3_ButtonPosition_Start.x = 0f;

        Vector3 v3_ButtonPosition_Final = v3_ButtonPosition_Start;
        v3_ButtonPosition_Final.x = 25f;

        float f_Evaluate_Menu = lerpCurve_Position.Evaluate(f_LerpTimer);
        Vector3 v3_ButtonPosition_New = Vector3.Lerp(v3_ButtonPosition_Start, v3_ButtonPosition_Final, f_Evaluate_Menu);
        gameObject.transform.localPosition = v3_ButtonPosition_New;
        #endregion

        #region Lerp 'A' button position
        // Evaluate lerp timer
        /*
        float f_Evaluate = lerpCurve_AButton.Evaluate(f_LerpTimer);

        // Set positions
        Vector3 v3_StartPos = gameObject.transform.localPosition;
        // v3_StartPos.z = f_ZPos;

        Vector3 v3_FinalPos = v3_StartPos;
        v3_FinalPos.x += 90f;

        Vector3 v3_NewPos = Vector3.Lerp(v3_StartPos, v3_FinalPos, f_Evaluate);
        go_AButton.transform.localPosition = v3_NewPos;
        */
        #endregion
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
