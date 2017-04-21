using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cs_VehicleLogic : MonoBehaviour
{
    [SerializeField] GameObject go_LookAtBox;
    Cs_LookAtBox lookAtBox;
    GameObject go_CameraController;
    Cs_CameraController camController;

    [SerializeField] GameObject go_LeftPos;
    [SerializeField] GameObject go_RightPos;

	// Use this for initialization
	void Start ()
    {
        go_CameraController = GameObject.Find("CameraController");
        camController = go_CameraController.GetComponent<Cs_CameraController>();

        lookAtBox = go_LookAtBox.GetComponent<Cs_LookAtBox>();

        IsLeftSide = false;

        if (IsLeftSide) f_LerpTimer = f_LerpTimer_Max;
    }

    void UpdateLookRotation()
    {
        Quaternion q_StartRot = gameObject.transform.rotation;

        Vector3 v3_From = go_LookAtBox.transform.position;
        Vector3 v3_To = gameObject.transform.position;
        Quaternion q_NewRot = Quaternion.LookRotation(v3_From - v3_To);

        gameObject.transform.rotation = Quaternion.Lerp(q_StartRot, q_NewRot, 1f);
    }

    void UpdatePosition()
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

            gameObject.transform.position = Vector3.Lerp(go_LeftPos.transform.position, go_RightPos.transform.position, f_Perc_);
        }
        #endregion

        #region Update 'Done Moving' Bool
        if (b_IsLeftSide && f_LerpTimer == 0f) b_DoneMoving = true;
        else if (!b_IsLeftSide && f_LerpTimer == f_LerpTimer_Max) b_DoneMoving = true;
        #endregion
    }

    bool b_IsLeftSide;
    bool b_DoneMoving;
    float f_LerpTimer;
    [SerializeField]
    float f_LerpTimer_Max = 0.45f;
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
        UpdateLookRotation();
        UpdatePosition();
	}
}
