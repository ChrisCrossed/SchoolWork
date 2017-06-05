using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public enum Enum_EnemyState
{
    Patrol,
    ContemplateLife,
    AttackPlayer
}

public class Cs_Enemy_Basic : Cs_ENEMY
{
    NavMeshAgent navAgent;
    Enum_EnemyState e_State_;
    [SerializeField] List<GameObject> WaypointList = new List<GameObject>(5);

	// Use this for initialization
	void Start ()
    {
        Initialize();

        navAgent = gameObject.GetComponent<NavMeshAgent>();

        SetDestination( new Vector3(0, 0, 0) );
	}

    public Enum_EnemyState SetEnemyState
    {
        set { e_State_ = value; }
        get { return e_State_; }
    }

    void SetDestination(Vector3 v3_NewPosition_)
    {
        navAgent.SetDestination( v3_NewPosition_ );
    }
	
    void BehaviorState()
    {
        if(e_State_ == Enum_EnemyState.Patrol)
        {

        }

        else if(e_State_ == Enum_EnemyState.ContemplateLife)
        {

        }

        else if(e_State_ == Enum_EnemyState.AttackPlayer)
        {

        }
    }

	// Update is called once per frame
	void Update ()
    {
        base.SkinUpdate();
	}
}
