using UnityEngine;
using System.Collections;

public class Cs_ObsButtonLogic : MonoBehaviour
{
    float f_ButtonTimer;
    float f_MAX_BUTTON_TIMER = 4.0f;

    float f_ButtonModelTimer;
    GameObject go_ButtonModel;

    public GameObject go_Observatory;
    public int i_ButtonNumber;

    // Use this for initialization
    void Start ()
    {
        f_ButtonTimer = f_MAX_BUTTON_TIMER;
        go_ButtonModel = transform.Find("ButtonModel").gameObject;
    }

    public void UseButton()
    {
        if (f_ButtonTimer == f_MAX_BUTTON_TIMER)
        {
            f_ButtonModelTimer = 2.0f;

            f_ButtonTimer = 0.0f;

            go_Observatory.GetComponent<Cs_ObservatoryLogic>().SetPositionToLookAt(i_ButtonNumber);
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

    // Update is called once per frame
    void Update ()
    {
        UpdateButtonModel();
	}
}
