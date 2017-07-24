using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cs_TeleportTrigger : MonoBehaviour
{
    GameObject go_Player;
    Cs_PlayerController playerController;

    [SerializeField] GameObject TeleportObject;

	// Use this for initialization
	void Start ()
    {
        go_Player = GameObject.Find("Player");
        playerController = go_Player.GetComponent<Cs_PlayerController>();
	}

    private void OnTriggerEnter(Collider collider_)
    {
        if(collider_.gameObject == go_Player)
        {
            if(TeleportObject != null) playerController.TeleportPlayer = TeleportObject;
        }
    }
}
