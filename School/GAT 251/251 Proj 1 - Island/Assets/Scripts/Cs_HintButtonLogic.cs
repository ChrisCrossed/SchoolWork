using UnityEngine;
using System.Collections;

public class Cs_HintButtonLogic : MonoBehaviour
{
    float f_ButtonTimer;
    float f_MAX_BUTTON_TIMER = 4.0f;

    float f_ButtonModelTimer;
    GameObject go_ButtonModel;

    [SerializeField]
    Material mat_SuccessFail;
    Color color_Fail;
    Color color_Success;
    bool b_ChangeColor;

    public GameObject go_HintPanel;
    public bool[] TopRow = new bool[4];
    public bool[] SecondRow = new bool[4];
    public bool[] ThirdRow = new bool[4];
    public bool[] BottomRow = new bool[4];
    bool[] PatternList = new bool[16];

    // Use this for initialization
    void Start ()
    {
        f_ButtonTimer = f_MAX_BUTTON_TIMER;
        go_ButtonModel = transform.Find("ButtonModel").gameObject;

        InitializeBoolPattern();

        #region Set Colors
        if (mat_SuccessFail != null)
        {
            color_Fail = new Color(1, (float)(100 / 255), 0);
            color_Success = new Color((float)100 / 255, 1, 0);

            mat_SuccessFail.color = color_Fail;
        }
        #endregion
    }

    [SerializeField] GameObject go_SFX;
    public void UseButton()
    {
        if(f_ButtonTimer == f_MAX_BUTTON_TIMER)
        {
            f_ButtonModelTimer = 2.0f;

            f_ButtonTimer = 0.0f;

            if(go_HintPanel.GetComponent<Cs_ControlPanel>())
            {
                for(int i_ = 0; i_ < 16; ++i_)
                {
                    go_HintPanel.GetComponent<Cs_ControlPanel>().SetLight(i_, PatternList[i_]);
                }
            }
            else
            {
                print("No Control Panel Script attached!");
            }

            if(mat_SuccessFail != null)
            {
                b_ChangeColor = true;
            }

            go_SFX.GetComponent<AudioSource>().Play();
        }
    }

    void UpdateButtonModel()
    {
        if (f_ButtonTimer < f_MAX_BUTTON_TIMER)
        {
            f_ButtonTimer += Time.deltaTime;

            if (f_ButtonTimer > f_MAX_BUTTON_TIMER) f_ButtonTimer = f_MAX_BUTTON_TIMER;
        }

        // Button Model Movement
        if (f_ButtonModelTimer >= 1.25f)
        {
            Vector3 v3_newPos = go_ButtonModel.transform.localPosition;

            go_ButtonModel.transform.localPosition = new Vector3(0, 0, Mathf.Lerp(v3_newPos.z, 0.4f, 0.1f));
        }
        else if (f_ButtonModelTimer >= 0.5f)
        {
            Vector3 v3_newPos = go_ButtonModel.transform.localPosition;

            go_ButtonModel.transform.localPosition = new Vector3(0, 0, Mathf.Lerp(v3_newPos.z, 0.6f, 0.1f));
        }

        if (f_ButtonModelTimer != 0.0f)
        {
            f_ButtonModelTimer -= Time.deltaTime;

            if (f_ButtonModelTimer < 0.0f) f_ButtonModelTimer = 0.0f;
        }
    }

    void InitializeBoolPattern()
    {
        PatternList[0] = TopRow[0];
        PatternList[1] = TopRow[1];
        PatternList[2] = TopRow[2];
        PatternList[3] = TopRow[3];

        PatternList[4] = SecondRow[0];
        PatternList[5] = SecondRow[1];
        PatternList[6] = SecondRow[2];
        PatternList[7] = SecondRow[3];

        PatternList[8]  = ThirdRow[0];
        PatternList[9]  = ThirdRow[1];
        PatternList[10] = ThirdRow[2];
        PatternList[11] = ThirdRow[3];

        PatternList[12] = BottomRow[0];
        PatternList[13] = BottomRow[1];
        PatternList[14] = BottomRow[2];
        PatternList[15] = BottomRow[3];
    }

    public bool[] GetBoolArray()
    {
        return PatternList;
    }
	
	// Update is called once per frame
	void Update ()
    {
        UpdateButtonModel();

        if (b_ChangeColor)
        {
            Color currentColor = mat_SuccessFail.color;

            currentColor = Color.Lerp(currentColor, color_Success, Time.deltaTime * 5);

            mat_SuccessFail.color = currentColor;
        }
    }
}
