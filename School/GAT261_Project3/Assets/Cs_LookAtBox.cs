using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cs_LookAtBox : MonoBehaviour
{
    [SerializeField] GameObject go_LeftPos;
    [SerializeField] GameObject go_RightPos;

    [SerializeField] AnimationCurve ac_Curve;

    // Use this for initialization
    void Start ()
    {
        IsLeftSide = false;

        if (IsLeftSide) f_LerpTimer = f_LerpTimer_Max;
	}

    bool b_IsLeftSide;
    bool b_DoneMoving;
    float f_LerpTimer;
    [SerializeField] float f_LerpTimer_Max = 0.15f;
    public bool IsLeftSide
    {
        set
        {
            if (value != b_IsLeftSide)
            {
                // Change 'is moving' bool state
                b_DoneMoving = false;
            }

            b_IsLeftSide = value;
        }
        get { return b_IsLeftSide; }
    }

    // Update is called once per frame
    void Update ()
    {
        // TEMP CODE
        if (Input.GetKeyDown(KeyCode.Space)) IsLeftSide = !IsLeftSide;

        #region Lerp Timer
        if (b_IsLeftSide && f_LerpTimer > 0f)
        {
            f_LerpTimer -= Time.fixedDeltaTime;
            if (f_LerpTimer < 0f) f_LerpTimer = 0f;
        }
        else if (!b_IsLeftSide && f_LerpTimer < f_LerpTimer_Max)
        {
            f_LerpTimer += Time.fixedDeltaTime;
            if (f_LerpTimer > f_LerpTimer_Max) f_LerpTimer = f_LerpTimer_Max;
        }
        #endregion

        #region Update Box Position
        if (!b_DoneMoving)
        {
            float f_Perc_ = f_LerpTimer / f_LerpTimer_Max;

            float f_New = ac_Curve.Evaluate(f_Perc_);

            gameObject.transform.position = Vector3.Lerp(go_LeftPos.transform.position, go_RightPos.transform.position, f_New);
        }
        #endregion

        #region Update 'Done Moving' Bool
        if (b_IsLeftSide && f_LerpTimer == 0f) b_DoneMoving = true;
        else if (!b_IsLeftSide && f_LerpTimer == f_LerpTimer_Max) b_DoneMoving = true;
        #endregion
    }
}
