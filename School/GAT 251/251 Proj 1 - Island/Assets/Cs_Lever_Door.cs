using UnityEngine;
using System.Collections;

public class Cs_Lever_Door : MonoBehaviour
{
    float f_ButtonTimer;
    float f_MAX_BUTTON_TIMER = 4.0f;

    float f_LeverModelTimer;

    [SerializeField]
    Material mat_SuccessFail;
    Color color_Fail;
    Color color_Success;
    bool b_ChangeColor;

    public GameObject go_Door;

    // Use this for initialization
    void Start ()
    {
        f_ButtonTimer = f_MAX_BUTTON_TIMER;

        float f_PartialColor = (float)(100 / 255);
        color_Fail =    new Color( 1, f_PartialColor, 0 );
        color_Success = new Color( f_PartialColor, 1, 0 );

        if(mat_SuccessFail != null) mat_SuccessFail.color = color_Fail;
    }
	
	// Update is called once per frame
	void Update ()
    {
        UpdateLeverModel();

        if(b_ChangeColor)
        {
            Color currentColor = mat_SuccessFail.color;

            currentColor = Color.Lerp(currentColor, color_Success, Time.deltaTime * 5);

            mat_SuccessFail.color = currentColor;
        }
    }

    [SerializeField] GameObject go_SFX;
    public void UseButton()
    {
        if (f_ButtonTimer == f_MAX_BUTTON_TIMER)
        {
            f_LeverModelTimer = 2.0f;

            f_ButtonTimer = 0.0f;

            go_Door.GetComponent<Cs_Door>().MoveDoor();

            if(mat_SuccessFail != null) b_ChangeColor = true;

            go_SFX.GetComponent<AudioSource>().Play();
        }
    }


    void UpdateLeverModel()
    {
        if (f_ButtonTimer < f_MAX_BUTTON_TIMER)
        {
            f_ButtonTimer += Time.deltaTime;

            if (f_ButtonTimer > f_MAX_BUTTON_TIMER) f_ButtonTimer = f_MAX_BUTTON_TIMER;
        }

        // Button Model Movement
        if (f_LeverModelTimer >= 1.25f)
        {
            float f_LerpAngle = Mathf.LerpAngle(gameObject.transform.eulerAngles.z, gameObject.transform.eulerAngles.z - 15f, 0.05f);
            
            gameObject.transform.eulerAngles = new Vector3(gameObject.transform.eulerAngles.x, gameObject.transform.eulerAngles.y, f_LerpAngle);
        }
        else if (f_LeverModelTimer >= 0.5f)
        {
            float f_LerpAngle = Mathf.LerpAngle(gameObject.transform.eulerAngles.z, gameObject.transform.eulerAngles.z + 15f, 0.05f);

            gameObject.transform.eulerAngles = new Vector3(gameObject.transform.eulerAngles.x, gameObject.transform.eulerAngles.y, f_LerpAngle);
        }

        if (f_LeverModelTimer != 0.0f)
        {
            f_LeverModelTimer -= Time.deltaTime;

            if (f_LeverModelTimer < 0.0f)
            {
                f_LeverModelTimer = 0.0f;
            }
        }
    }
}
