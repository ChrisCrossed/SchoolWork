using UnityEngine;
using System.Collections;

public class Cs_LeverLogic : MonoBehaviour
{
    float f_ButtonTimer;
    float f_MAX_BUTTON_TIMER = 4.0f;

    float f_ButtonModelTimer;
    GameObject go_ButtonModel;

    public GameObject go_Door;

    public GameObject go_CorrectAnswerSource;
    public GameObject go_ControlPanel;
    bool[] b_LightArray = new bool[16];
    bool[] b_CorrectAnswer = new bool[16];

    public bool b_HasSecondPanel = false;
    public GameObject go_CorrectAnswerSourceTwo;
    public GameObject go_ControlPanelTwo;
    bool[] b_LightArrayTwo = new bool[16];
    bool[] b_CorrectAnswerTwo = new bool[16];

    public Material mat_SuccessFail;
    Color color_Fail;
    Color color_Success;
    bool b_ChangeColor;

    // Use this for initialization
    void Start ()
    {
        f_ButtonTimer = f_MAX_BUTTON_TIMER;

        for(int i_ = 0; i_ < 16; ++i_)
        {
            b_CorrectAnswer[i_] = false;
        }

        SetCorrectAnswer();

        #region Set Colors
        color_Fail = new Color(1, (float)(100 / 255), 0);
        color_Success = new Color((float)100 / 255, 1, 0);

        if (mat_SuccessFail != null) mat_SuccessFail.color = color_Fail;
        #endregion
    }

    void SetCorrectAnswer()
    {
        if ( go_CorrectAnswerSource.GetComponent<Cs_ControlPanel>() )
        {
            b_CorrectAnswer = go_CorrectAnswerSource.GetComponent<Cs_ControlPanel>().GetBoolArray();
        }
        else if( go_CorrectAnswerSource.GetComponent<Cs_HintButtonLogic>() )
        {
            b_CorrectAnswer = go_CorrectAnswerSource.GetComponent<Cs_HintButtonLogic>().GetBoolArray();
        }
        
        if(b_HasSecondPanel)
        {
            if (go_CorrectAnswerSourceTwo.GetComponent<Cs_ControlPanel>())
            {
                b_CorrectAnswerTwo = go_CorrectAnswerSourceTwo.GetComponent<Cs_ControlPanel>().GetBoolArray();
            }
            else if (go_CorrectAnswerSourceTwo.GetComponent<Cs_HintButtonLogic>())
            {
                b_CorrectAnswerTwo = go_CorrectAnswerSourceTwo.GetComponent<Cs_HintButtonLogic>().GetBoolArray();
            }
        }
    }

    [SerializeField] GameObject go_SFX;
    public void UseButton()
    {
        if (f_ButtonTimer == f_MAX_BUTTON_TIMER)
        {
            f_ButtonModelTimer = 2.0f;

            f_ButtonTimer = 0.0f;

            b_LightArray = go_ControlPanel.GetComponent<Cs_ControlPanel>().GetBoolArray();

            if (b_HasSecondPanel) b_LightArrayTwo = go_ControlPanelTwo.GetComponent<Cs_ControlPanel>().GetBoolArray();

            if (CheckCorrectAnswer())
            {
                if(go_Door.GetComponent<Cs_Door>())
                {
                    go_Door.GetComponent<Cs_Door>().MoveDoor();
                }
                else if(go_Door.GetComponent<Cs_FakeGround>())
                {
                    go_Door.GetComponent<Cs_FakeGround>().OpenUnderground();
                }

                b_ChangeColor = true;
            }

            go_SFX.GetComponent<AudioSource>().Play();
        }
    }

    bool CheckCorrectAnswer()
    {
        for(int i_ = 0; i_ < 16; ++i_)
        {
            if (b_LightArray[i_] != b_CorrectAnswer[i_])
            {
                return false;
            }
        }

        if(b_HasSecondPanel)
        {
            for (int i_ = 0; i_ < 16; ++i_)
            {
                if (b_LightArrayTwo[i_] != b_CorrectAnswerTwo[i_])
                {
                    return false;
                }
            }
        }

        return true;
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
            float f_LerpAngle = Mathf.LerpAngle(gameObject.transform.eulerAngles.z, gameObject.transform.eulerAngles.z - 15f, 0.05f);
            gameObject.transform.eulerAngles = new Vector3(gameObject.transform.eulerAngles.x, gameObject.transform.eulerAngles.y, f_LerpAngle);
        }
        else if (f_ButtonModelTimer >= 0.5f)
        {
            float f_LerpAngle = Mathf.LerpAngle(gameObject.transform.eulerAngles.z, gameObject.transform.eulerAngles.z + 15f, 0.05f);

            gameObject.transform.eulerAngles = new Vector3(gameObject.transform.eulerAngles.x, gameObject.transform.eulerAngles.y, f_LerpAngle);
        }

        if (f_ButtonModelTimer != 0.0f)
        {
            f_ButtonModelTimer -= Time.deltaTime;

            if (f_ButtonModelTimer < 0.0f) f_ButtonModelTimer = 0.0f;
        }
    }

    // Update is called once per frame
    void Update ()
    {
        UpdateButtonModel();

        if (b_ChangeColor && mat_SuccessFail != null)
        {
            Color currentColor = mat_SuccessFail.color;

            currentColor = Color.Lerp(currentColor, color_Success, Time.deltaTime * 5);

            mat_SuccessFail.color = currentColor;
        }
    }
}
