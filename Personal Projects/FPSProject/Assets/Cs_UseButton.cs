using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cs_UseButton : MonoBehaviour
{
    // Object Connection
    [SerializeField] GameObject ConnectedObject;
    [SerializeField] bool UseOnce;

    // Child objects
    GameObject go_Button_Top;
    GameObject go_Button_Bottom;

    // Boolean for Resetting Activation
    bool b_ReadyToUse;

    // Use this for initialization
    void Start ()
    {
        // Object connection
		if(ConnectedObject == new GameObject() || ConnectedObject == null ) print(gameObject.name + " is missing a Connected Object!");

        // Child objects
        go_Button_Bottom = transform.FindChild("Button_Bottom").gameObject;
        go_Button_Top = transform.FindChild("Button_Top").gameObject;

        // Boolean for Resetting Activation
        b_ReadyToUse = true;
    }

    bool b_Active;
    public bool ButtonState
    {
        get { return b_Active; }
        set
        {
            if( (value == true || !UseOnce ) && b_ReadyToUse )
            {
                b_Active = value;
                b_ReadyToUse = false;
            }
        }
    }

    public void Use_Button()
    {
        ButtonState = !ButtonState;
    }

	
	// Update is called once per frame
	void Update ()
    {
        Vector3 v3_ButtonPos_Top = go_Button_Top.transform.localPosition;
        Vector3 v3_ButtonPos_Bottom = go_Button_Bottom.transform.localPosition;

        if (b_Active)
        {
            if( go_Button_Top.transform.localPosition.x < 0.125f )
            {
                v3_ButtonPos_Top.x += Time.fixedDeltaTime * 2f;
                if (v3_ButtonPos_Top.x > 0.125f)
                {
                    v3_ButtonPos_Top.x = 0.125f;
                    b_ReadyToUse = true;
                }

                // Gets a percent between 0.5f and 1.0f
                float f_Perc = (v3_ButtonPos_Top.x / 0.125f) / 2f + 0.5f;

                Color v3_Color = go_Button_Top.GetComponent<Renderer>().material.color;
                v3_Color.a = f_Perc;
                go_Button_Top.GetComponent<Renderer>().material.color = v3_Color;
            }

            if( go_Button_Bottom.transform.localPosition.x > 0f)
            {
                v3_ButtonPos_Bottom.x -= Time.fixedDeltaTime * 2f;
                if (v3_ButtonPos_Bottom.x < 0f) v3_ButtonPos_Bottom.x = 0f;

                // Gets a percent between 0.5f and 1.0f
                float f_Perc = (v3_ButtonPos_Bottom.x / 0.125f) / 2f + 0.5f;

                Color v3_Color = go_Button_Bottom.GetComponent<Renderer>().material.color;
                v3_Color.a = f_Perc;
                go_Button_Bottom.GetComponent<Renderer>().material.color = v3_Color;
            }
        }
        else
        {
            if (go_Button_Top.transform.localPosition.x > 0f)
            {
                v3_ButtonPos_Top.x -= Time.fixedDeltaTime * 2f;
                if (v3_ButtonPos_Top.x < 0f)
                {
                    v3_ButtonPos_Top.x = 0f;
                    b_ReadyToUse = true;
                }

                // Gets a percent between 0.5f and 1.0f
                float f_Perc = (v3_ButtonPos_Top.x / 0.125f) / 2f + 0.5f;

                Color v3_Color = go_Button_Top.GetComponent<Renderer>().material.color;
                v3_Color.a = f_Perc;
                go_Button_Top.GetComponent<Renderer>().material.color = v3_Color;
            }

            if (go_Button_Bottom.transform.localPosition.x < 0.125f)
            {
                v3_ButtonPos_Bottom.x += Time.fixedDeltaTime * 2f;
                if (v3_ButtonPos_Bottom.x > 0.125f) v3_ButtonPos_Bottom.x = 0.125f;

                // Gets a percent between 0.5f and 1.0f
                float f_Perc = (v3_ButtonPos_Bottom.x / 0.125f) / 2f + 0.5f;

                Color v3_Color = go_Button_Bottom.GetComponent<Renderer>().material.color;
                v3_Color.a = f_Perc;
                go_Button_Bottom.GetComponent<Renderer>().material.color = v3_Color;
            }
        }

        go_Button_Top.transform.localPosition = v3_ButtonPos_Top;
        go_Button_Bottom.transform.localPosition = v3_ButtonPos_Bottom;
	}
}
