using UnityEngine;
using System.Collections;

public class Cs_ControlPanel : MonoBehaviour
{
    public bool b_IsControlPanel;
    public bool b_PresetButtonsCorrect;
    
    [SerializeField]
    bool[] b_PresetButtons = new bool[16];

    bool[] b_Buttons;

	// Use this for initialization
	void Start ()
    {
        b_Buttons = new bool[16];

        InitializeButtons();

        if (!b_IsControlPanel) DisableButtonRaycast();
	}

    void InitializeButtons()
    {
        for(int i_ = 0; i_ < 16; ++i_)
        {
            // Find each child button and set them to 'off'
            gameObject.transform.Find(i_.ToString()).GetComponent<Cs_ButtonLogic>().SetActive(b_PresetButtons[i_]);

            b_Buttons[i_] = b_PresetButtons[i_];
        }
    }

    public void SetLight(int i_Pos_, bool b_IsOn_)
    {
        if (i_Pos_ >= 16) i_Pos_ = 15;
        if (i_Pos_ <= -1) i_Pos_ = 0;

        SetLight(i_Pos_.ToString(), b_IsOn_);
    }

    [SerializeField] GameObject go_SFX;
    [SerializeField] AudioClip sfx_ButtonOn;
    [SerializeField] AudioClip sfx_ButtonOff;
    public void SetLight(string s_Pos_, bool b_IsOn_)
    {
        gameObject.transform.Find(s_Pos_).GetComponent<Cs_ButtonLogic>().SetActive(b_IsOn_);

        b_Buttons[int.Parse(s_Pos_)] = b_IsOn_;

        if(go_SFX != null)
        {
            if (b_IsOn_) go_SFX.GetComponent<AudioSource>().PlayOneShot(sfx_ButtonOn);
            else if (!b_IsOn_) go_SFX.GetComponent<AudioSource>().PlayOneShot(sfx_ButtonOff);
        }
    }

    public bool[] GetBoolArray()
    {
        if(!b_IsControlPanel && b_PresetButtonsCorrect)
        {
            return b_PresetButtons;
        }

        return b_Buttons;
    }

    void DisableButtonRaycast()
    {
        for (int i_ = 0; i_ < 16; ++i_)
        {
            // Find each child button and set them to 'off'
            gameObject.transform.Find(i_.ToString()).gameObject.layer = 2;
        }
    }

	// Update is called once per frame
	void Update ()
    {
	
	}
}
