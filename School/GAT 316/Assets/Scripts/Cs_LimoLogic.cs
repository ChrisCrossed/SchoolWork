using UnityEngine;
using System.Collections;

public class Cs_LimoLogic : MonoBehaviour
{
    GameObject go_LimoStop;
    GameObject go_LimoGoal;
    NavMeshAgent navMeshAgent;

    bool b_IsArriving = true;

	// Use this for initialization
	public void Init_Limo()
    {
        go_LimoGoal = GameObject.Find("CarGoal_2");
        go_LimoStop = GameObject.Find("LimoStop");

        navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
        navMeshAgent.speed = 15f;

        gameObject.transform.position = GameObject.Find("LimoStart").transform.position;

        Set_LimoStop();
    }

    void Set_LimoStop()
    {
        navMeshAgent.SetDestination(go_LimoStop.transform.position);
    }

    public void Set_LimoGoal()
    {
        navMeshAgent.SetDestination(go_LimoGoal.transform.position);
    }

    void Update()
    {
        if(b_IsArriving && navMeshAgent.remainingDistance < 0.15f)
        {
            // Enable the visual marker to leave
            transform.Find("tgr_Limo").GetComponent<Cs_LimoTrigger>().Set_MeshRenderer = true;

            // Turn off b_IsArriving
            b_IsArriving = false;
        }
    }
}
