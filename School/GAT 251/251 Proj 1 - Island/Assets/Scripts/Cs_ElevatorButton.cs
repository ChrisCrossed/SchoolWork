using UnityEngine;
using System.Collections;

public enum Enum_ElevatorButtonType
{
    Bottom,
    Top,
    Both
}

public class Cs_ElevatorButton : MonoBehaviour
{
    public Enum_ElevatorButtonType ButtonPosition;
    public GameObject go_Elevator;

    float f_ButtonTimer;
    float f_MAX_BUTTON_TIMER = 4.0f;

    float f_ButtonModelTimer;
    GameObject go_ButtonModel;

	// Use this for initialization
	void Start ()
    {
        f_ButtonTimer = f_MAX_BUTTON_TIMER;
        go_ButtonModel = transform.Find("ButtonModel").gameObject;
	}

    public void UseButton()
    {
        if(f_ButtonTimer == f_MAX_BUTTON_TIMER)
        {
            Enum_ElevatorStatus elevatorStatus = go_Elevator.GetComponent<Cs_Elevator>().GetState();

            if(ButtonPosition == Enum_ElevatorButtonType.Top && elevatorStatus == Enum_ElevatorStatus.Bottom_Stall)
            {
                go_Elevator.GetComponent<Cs_Elevator>().CycleElevator();

                f_ButtonTimer = 0.0f;
            }
            else if(ButtonPosition == Enum_ElevatorButtonType.Bottom && elevatorStatus == Enum_ElevatorStatus.Top_Stall)
            {
                go_Elevator.GetComponent<Cs_Elevator>().CycleElevator();

                f_ButtonTimer = 0.0f;
            }
            else if( ButtonPosition == Enum_ElevatorButtonType.Both && 
                    (elevatorStatus == Enum_ElevatorStatus.Top_Stall || elevatorStatus == Enum_ElevatorStatus.Bottom_Stall) )
            {
                go_Elevator.GetComponent<Cs_Elevator>().CycleElevator();

                f_ButtonTimer = 0.0f;
            }

            f_ButtonModelTimer = 2.0f;
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
	    if(f_ButtonTimer < f_MAX_BUTTON_TIMER)
        {
            f_ButtonTimer += Time.deltaTime;

            if (f_ButtonTimer > f_MAX_BUTTON_TIMER) f_ButtonTimer = f_MAX_BUTTON_TIMER;
        }

        // Button Model Movement
        if(f_ButtonModelTimer >= 1.25f)
        {
            Vector3 v3_newPos = go_ButtonModel.transform.localPosition;

            go_ButtonModel.transform.localPosition = new Vector3(Mathf.Lerp(v3_newPos.x, 0.4f, 0.1f), 0, 0);
        }
        else if(f_ButtonModelTimer >= 0.5f)
        {
            Vector3 v3_newPos = go_ButtonModel.transform.localPosition;

            go_ButtonModel.transform.localPosition = new Vector3(Mathf.Lerp(v3_newPos.x, 0.6f, 0.1f), 0, 0);
        }

        if(f_ButtonModelTimer != 0.0f)
        {
            f_ButtonModelTimer -= Time.deltaTime;

            if (f_ButtonModelTimer < 0.0f) f_ButtonModelTimer = 0.0f;
        }
	}
}
