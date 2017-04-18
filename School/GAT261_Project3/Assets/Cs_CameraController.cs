using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cs_CameraController : MonoBehaviour
{
    // Camera Objects and Positions
    [SerializeField] GameObject go_CamPos_Left;
    [SerializeField] GameObject go_CamPos_Right;
    [SerializeField] GameObject go_CameraPoint;
    GameObject go_MainCamera;

    // Smooth Lerp
    [SerializeField] AnimationCurve ac_Curve;

    // Use this for initialization
    void Start ()
    {
        go_MainCamera = GameObject.Find("Main Camera");

        go_CamPos_Left.transform.LookAt(go_CameraPoint.transform.position);
        go_CamPos_Right.transform.LookAt(go_CameraPoint.transform.position);

        Vector3 v3_NewRot = go_CamPos_Left.transform.eulerAngles;
        v3_NewRot.z = -6f;
        go_CamPos_Left.transform.eulerAngles = v3_NewRot;

        v3_NewRot = go_CamPos_Right.transform.eulerAngles;
        v3_NewRot.z = 6f;
        go_CamPos_Right.transform.eulerAngles = v3_NewRot;

        IsLeftSide = true;
        f_LerpTimer = 0f;
	}

    bool b_IsLeftSide;
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
    bool b_DoneMoving;
	void FixedUpdate ()
    {
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

        #region Update Camera Position
        if(!b_DoneMoving)
        {
            float f_Perc_ = ac_Curve.Evaluate( f_LerpTimer / f_LerpTimer_Max );

            go_MainCamera.transform.position = Vector3.Lerp( go_CamPos_Left.transform.position, go_CamPos_Right.transform.position, f_Perc_ );
            go_MainCamera.transform.rotation = Quaternion.Lerp( go_CamPos_Left.transform.rotation, go_CamPos_Right.transform.rotation, f_Perc_ );
        }
        #endregion

        #region Update 'Done Moving' Bool
        if (b_IsLeftSide && f_LerpTimer == 0f) b_DoneMoving = true;
        else if (!b_IsLeftSide && f_LerpTimer == f_LerpTimer_Max) b_DoneMoving = true;
        #endregion
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) IsLeftSide = !IsLeftSide;
    }
}
