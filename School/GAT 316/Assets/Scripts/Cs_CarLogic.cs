using UnityEngine;
using System.Collections;

public class Cs_CarLogic : MonoBehaviour
{
    // Choice between 1st or 2nd goal at end of road
    float f_LaneSwitchTimer;
    float f_LaneSwitchTimer_Max;
    bool b_CarGoal;
    NavMeshAgent navAgent;

    Vector3 go_CarGoal_1;
    Vector3 go_CarGoal_2;

    Vector3 go_CarStart_1;
    Vector3 go_CarStart_2;

    int i_NumLanes = 2;

    // Use this for initialization
    void Start ()
    {
        go_CarGoal_1 = GameObject.Find("CarGoal_1").transform.position;
        go_CarGoal_2 = GameObject.Find("CarGoal_2").transform.position;
        go_CarStart_1 = GameObject.Find("CarStart_1").transform.position;
        go_CarStart_2 = GameObject.Find("CarStart_2").transform.position;

        navAgent = gameObject.GetComponent<NavMeshAgent>();

        Reset();
    }

    void Reset()
    {
        // Set random speed
        navAgent.speed = Random.Range(10f, 25f);

        // Set random end position
        if (Random.Range(0, i_NumLanes) == 0)
        {
            gameObject.transform.position = go_CarStart_1;
            navAgent.SetDestination(go_CarGoal_1);
        }
        else
        {
            gameObject.transform.position = go_CarStart_2;
            navAgent.SetDestination(go_CarGoal_2);
        }
    }

    public void Set_BottomLaneOnly()
    {
        i_NumLanes = 1;
    }

	// Update is called once per frame
	void Update ()
    {
        if (navAgent.remainingDistance < 0.5f)
        {
            Reset();
        }
    }
}
