using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cs_DashToObject : MonoBehaviour
{
    GameObject go_Player;
    Cs_PlayerController go_PlayerController;
    GameObject go_Connection;

	// Use this for initialization
	void Start ()
    {
        go_Player = GameObject.Find("Player");
        go_PlayerController = go_Player.GetComponent<Cs_PlayerController>();
	}

    void DashToObject()
    {
        go_PlayerController.DashToEnemyPosition();
    }
	
	// Update is called once per frame
	void Update ()
    {
        /*
		if(Input.GetKeyDown(KeyCode.F))
        {
            DashToObject();
        }
        */
	}
}
