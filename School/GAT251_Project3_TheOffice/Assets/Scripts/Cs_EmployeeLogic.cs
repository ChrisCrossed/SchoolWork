using UnityEngine;
using System.Collections;

public class Cs_EmployeeLogic : MonoBehaviour
{
    [SerializeField] int i_StartPosition = 0;
    int i_CurrPosition;
    [SerializeField] GameObject[] go_Positions = new GameObject[5];
    Vector3[] v3_Positions = new Vector3[5];

    Cs_Objective objectiveObj;
    
    NavMeshAgent navAgent;
    bool b_TestPosition;

	// Use this for initialization
	void Start ()
    {
        objectiveObj = transform.Find("BackMessage").GetComponent<Cs_Objective>();
        navAgent = gameObject.GetComponent<NavMeshAgent>();

        for(int i_ = 0; i_ < go_Positions.Length; ++i_)
        {
            if(go_Positions[i_])
            {
                v3_Positions[i_] = go_Positions[i_].transform.position;
            }
        }

        navAgent.SetDestination(v3_Positions[i_StartPosition]);

        i_CurrPosition = i_StartPosition;
    }
	
	// Update is called once per frame
	void Update ()
    {
	    if(navAgent.remainingDistance < .15f)
        {
            ++i_CurrPosition;
            if (i_CurrPosition >= 5) i_CurrPosition = 0;
            if (go_Positions[i_CurrPosition] == null) i_CurrPosition = 0;

            navAgent.SetDestination(v3_Positions[i_CurrPosition]);

            if(i_CurrPosition == 1 || i_CurrPosition == 3)
            {
                if(objectiveObj.Set_State != Enum_ObjectiveState.Disabled)
                {
                    objectiveObj.Set_State = Enum_ObjectiveState.InProgress;
                }
            }
        }
	}
}
