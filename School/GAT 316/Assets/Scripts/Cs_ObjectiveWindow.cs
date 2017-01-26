using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Cs_ObjectiveWindow : MonoBehaviour
{
    GameObject txt_EnterCompound;
    GameObject txt_DeactivateGenerator;
    GameObject txt_GrabBriefcase;
    GameObject txt_Escape;

	// Use this for initialization
	void Start ()
    {
        txt_EnterCompound = transform.Find("Text_EnterCompound").gameObject;
        txt_DeactivateGenerator = transform.Find("Text_DeactivateGenerator").gameObject;
        txt_GrabBriefcase = transform.Find("Text_GrabBriefcase").gameObject;
        txt_Escape = transform.Find("Text_Escape").gameObject;

        Set_EnterCompound = false;
        Set_DeactivateGenerator = false;
        Set_GrabBriefcase = false;
        Set_Escape = false;
    }

    public bool Set_EnterCompound
    {
        set
        {
            string s_Text = "Enter Compound ";

            if(value)
            {
                txt_EnterCompound.GetComponent<Text>().text = s_Text + "[✓]";
                txt_EnterCompound.GetComponent<Text>().color = Color.green;
            }
            else
            {
                txt_EnterCompound.GetComponent<Text>().text = s_Text + "[X]";
                txt_EnterCompound.GetComponent<Text>().color = Color.yellow;
            }
        }
    }

    public bool Set_DeactivateGenerator
    {
        set
        {
            string s_Text = "Deactivate Generator ";

            if (value)
            {
                txt_DeactivateGenerator.GetComponent<Text>().text = s_Text + "[✓]";
                txt_DeactivateGenerator.GetComponent<Text>().color = Color.green;
            }
            else
            {
                txt_DeactivateGenerator.GetComponent<Text>().text = s_Text + "[X]";
                txt_DeactivateGenerator.GetComponent<Text>().color = Color.yellow;
            }
        }
    }

    bool b_BriefcaseGrabbed;
    public bool Set_GrabBriefcase
    {
        set
        {
            string s_Text = "Grab Briefcase ";

            if (value)
            {
                txt_GrabBriefcase.GetComponent<Text>().text = s_Text + "[✓]";
                txt_GrabBriefcase.GetComponent<Text>().color = Color.green;
            }
            else
            {
                txt_GrabBriefcase.GetComponent<Text>().text = s_Text + "[X]";
                txt_GrabBriefcase.GetComponent<Text>().color = Color.yellow;
            }

            b_BriefcaseGrabbed = value;
        }

        get { return b_BriefcaseGrabbed; }
    }

    public bool Set_Escape
    {
        set
        {
            string s_Text = "Escape ";

            if (value)
            {
                txt_Escape.GetComponent<Text>().text = s_Text + "[✓]";
                txt_Escape.GetComponent<Text>().color = Color.green;
            }
            else
            {
                txt_Escape.GetComponent<Text>().text = s_Text + "[X]";
                txt_Escape.GetComponent<Text>().color = Color.yellow;
            }
        }
    }

    // Update is called once per frame
    void Update ()
    {
	
	}
}
