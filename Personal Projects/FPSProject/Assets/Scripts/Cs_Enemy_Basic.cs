using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class Cs_Enemy_Basic : Cs_ENEMY
{
    NavMeshAgent navAgent;

	// Use this for initialization
	void Start ()
    {
        Initialize();

        navAgent = gameObject.GetComponent<NavMeshAgent>();

        SetDestination( new Vector3(0, 0, 0) );
	}
	
	// Update is called once per frame
	void Update ()
    {
        base.SkinUpdate();
	}

    void SetDestination(Vector3 v3_NewPosition_)
    {
        navAgent.SetDestination( v3_NewPosition_ );
    }
}
