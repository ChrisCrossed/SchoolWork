using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cs_WeaponObjects : MonoBehaviour
{
    GameObject go_Player;
    GameObject go_PlayerCamera;

	// Use this for initialization
	void Start ()
    {
        go_Player = GameObject.Find("Player");
        go_PlayerCamera = go_Player.transform.Find("WeaponPosition").gameObject;

        // Remove this object from having a parent object so we can follow the player independently
        transform.SetParent(null);
	}
	
	// Update is called once per frame
	void LateUpdate ()
    {
        gameObject.transform.position = go_PlayerCamera.transform.position;
	}
}
