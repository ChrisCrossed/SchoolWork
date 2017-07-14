using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cs_LookAtPlayer : MonoBehaviour
{
    GameObject go_Player;

	// Use this for initialization
	void Start ()
    {
        go_Player = GameObject.Find("Player");
	}
	
	// Update is called once per frame
	void Update ()
    {
        transform.LookAt(go_Player.transform.position);
	}
}
