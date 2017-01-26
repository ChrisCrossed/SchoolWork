using UnityEngine;
using System.Collections;

public class Cs_ButtonLogic : MonoBehaviour
{
    float f_yStartPos;
    float f_PosModifier = 0.2f;

    bool b_IsActive;
    float f_ButtonTimer;
    float f_ButtonTimer_Max = 0.5f;

    Material mat_Active;
    Material mat_Inactive;

    Vector3 v3_newPos;

    // Use this for initialization
    void Start ()
    {
        f_yStartPos = gameObject.transform.localPosition.y;

        mat_Active = Resources.Load("Mat_Active", typeof(Material)) as Material;
        mat_Inactive = Resources.Load("Mat_Inactive", typeof(Material)) as Material;

        f_ButtonTimer = f_ButtonTimer_Max;
    }

    public void SetActive(bool b_IsActive_)
    {
        b_IsActive = b_IsActive_;
    }

    public bool GetState()
    {
        return b_IsActive;
    }

    public void UseButton()
    {
        if(f_ButtonTimer == 0.0f)
        {
            transform.parent.gameObject.GetComponent<Cs_ControlPanel>().SetLight(gameObject.name, !b_IsActive);

            f_ButtonTimer = f_ButtonTimer_Max;
        }
    }

    void UpdateButtonModel()
    {
        if (f_ButtonTimer > 0.0f)
        {
            f_ButtonTimer -= Time.deltaTime;

            if (f_ButtonTimer < 0.0f) f_ButtonTimer = 0.0f;
        }

        v3_newPos = gameObject.transform.localPosition;

        if (b_IsActive) v3_newPos.y = f_yStartPos + f_PosModifier; else v3_newPos.y = f_yStartPos - f_PosModifier;

        gameObject.transform.localPosition = Vector3.Lerp(gameObject.transform.localPosition, v3_newPos, 5 * Time.deltaTime);

        if (b_IsActive)
        {
            gameObject.GetComponent<MeshRenderer>().material.Lerp(gameObject.GetComponent<MeshRenderer>().material, mat_Active, 5 * Time.deltaTime);
        }
        else
        {
            gameObject.GetComponent<MeshRenderer>().material.Lerp(gameObject.GetComponent<MeshRenderer>().material, mat_Inactive, 5 * Time.deltaTime);
        }
    }

    // Update is called once per frame
    void Update ()
    {
        UpdateButtonModel();
    }
}
