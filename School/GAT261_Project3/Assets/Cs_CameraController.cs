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
    Camera mainCamera;

    // Smooth Lerp
    [SerializeField] AnimationCurve ac_Curve;

    // Use this for initialization
    void Awake ()
    {
        go_MainCamera = GameObject.Find("Main Camera");
        mainCamera = go_MainCamera.GetComponent<Camera>();

        go_CamPos_Left.transform.LookAt(go_CameraPoint.transform.position);
        go_CamPos_Right.transform.LookAt(go_CameraPoint.transform.position);

        Vector3 v3_NewRot = go_CamPos_Left.transform.eulerAngles;
        v3_NewRot.z = -5f;
        go_CamPos_Left.transform.eulerAngles = v3_NewRot;

        v3_NewRot = go_CamPos_Right.transform.eulerAngles;
        v3_NewRot.z = 5f;
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

    float f_CameraFOVScale = 45f;
    void Update()
    {
        // TEMP CODE
        if (Input.GetKeyDown(KeyCode.Space)) IsLeftSide = !IsLeftSide;

        // Slow down time
        if(Input.GetKey(KeyCode.LeftShift))
        {
            Time.timeScale -= Time.deltaTime * 2;
            if (Time.timeScale < 0.5f) Time.timeScale = 0.5f;

            mainCamera.fieldOfView -= Time.deltaTime * f_CameraFOVScale;
            if (mainCamera.fieldOfView < 45f) mainCamera.fieldOfView = 45f;
        }
        // Speed up time
        else if(Input.GetKey(KeyCode.LeftControl))
        {
            Time.timeScale += Time.deltaTime * 2;
            if (Time.timeScale > 2f) Time.timeScale = 2f;

            mainCamera.fieldOfView += Time.deltaTime * f_CameraFOVScale;
            if (mainCamera.fieldOfView > 75f) mainCamera.fieldOfView = 75f;
        }
        // Normalize time
        else
        {
            if(Time.timeScale > 1.0f)
            {
                Time.timeScale -= Time.deltaTime;
                if (Time.timeScale < 1.0f) Time.timeScale = 1.0f;
            }
            else if(Time.timeScale < 1.0f)
            {
                Time.timeScale += Time.deltaTime;
                if (Time.timeScale > 1.0f) Time.timeScale = 1.0f;
            }

            if(mainCamera.fieldOfView > 60f)
            {
                mainCamera.fieldOfView -= Time.deltaTime * f_CameraFOVScale;
                if (mainCamera.fieldOfView < 60f) mainCamera.fieldOfView = 60f;
            }
            else if(mainCamera.fieldOfView < 60f)
            {
                mainCamera.fieldOfView += Time.deltaTime * f_CameraFOVScale;
                if (mainCamera.fieldOfView > 60f) mainCamera.fieldOfView = 60f;
            }
        }

        // Set fixedDeltaTime according to deltaTime
        Time.fixedDeltaTime = 0.02F * Time.timeScale;
    }
}
