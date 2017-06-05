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

    // Current waypoint counter
    int i_CurrentWaypoint;

	// Use this for initialization
	void Start ()
    {
        Initialize();

        navAgent = gameObject.GetComponent<NavMeshAgent>();

        SetDestination( WaypointList[i_CurrentWaypoint] );

        SetEnemyState = Enum_EnemyState.Patrol;

        WaypointList.Add(null);
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
    void SetDestination(GameObject go_NewPosition)
    {
        SetDestination(go_NewPosition.transform.position);
    }
	
    void BehaviorState()
    {
        if(e_State_ == Enum_EnemyState.Patrol)
        {
            // If the enemy is close enough to the waypoint, increment the waypoint counter
            if (CalculateDistancetoWaypoint(1.0f)) ++i_CurrentWaypoint;

            // Ensure the current Waypoint is populated
            if (WaypointList[i_CurrentWaypoint] == null) ++i_CurrentWaypoint;

            // If we have exceeded the count, reset the counter
            if (i_CurrentWaypoint >= WaypointList.Count) i_CurrentWaypoint = 0;

            // Send enemy to new position
            SetDestination(WaypointList[i_CurrentWaypoint]);
        }

        else if(e_State_ == Enum_EnemyState.ContemplateLife)
        {

        }

        else if(e_State_ == Enum_EnemyState.AttackPlayer)
        {

        }
    }

    bool CalculateDistancetoWaypoint(float f_DistanceThreshhold)
    {
        // Evaluate if enemy is close enough to waypoint
        if (Vector3.Distance(gameObject.transform.position, WaypointList[i_CurrentWaypoint].transform.position) < f_DistanceThreshhold) return true;

        return false;
    }
    float CalculateDistancetoWaypoint()
    {
        return Vector3.Distance(gameObject.transform.position, WaypointList[i_CurrentWaypoint].transform.position);
    }

	// Update is called once per frame
	void Update ()
    {
        base.SkinUpdate();

        BehaviorState();

        print("Checkpoint: " + i_CurrentWaypoint);
        print("Max: " + (WaypointList.Count - 1));
	}
}
