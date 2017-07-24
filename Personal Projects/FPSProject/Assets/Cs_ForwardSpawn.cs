using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cs_ForwardSpawn : MonoBehaviour
{
    [SerializeField] bool Active = false;

    // Player Connection
    GameObject go_Player;
    GameObject go_Child;

	// Use this for initialization
	void Start ()
    {
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        gameObject.GetComponent<Collider>().enabled = false;

        go_Child = transform.Find("Cube").gameObject;
        go_Child.GetComponent<MeshRenderer>().enabled = false;
        go_Child.GetComponent<Collider>().enabled = false;

        if (Active)
        {
            go_Player = GameObject.Find("Player");

            go_Player.transform.position = gameObject.transform.position;
            go_Player.transform.rotation = gameObject.transform.rotation;
        }

        new WaitForEndOfFrame();

        // GameObject.Destroy(gameObject);
	}
}
