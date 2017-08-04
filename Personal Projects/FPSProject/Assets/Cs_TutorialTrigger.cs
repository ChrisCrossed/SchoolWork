using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cs_TutorialTrigger : MonoBehaviour
{
    GameObject go_Player;

    [SerializeField] GameObject HUDElement;

	// Use this for initialization
	void Start ()
    {
        go_Player = GameObject.Find("Player");
	}

    private void OnTriggerEnter(Collider collider_)
    {
        if(collider_.gameObject == go_Player.gameObject)
        {
            HUDElement.GetComponent<Cs_ControlsTutorial>().Activate();

            gameObject.GetComponent<BoxCollider>().enabled = false;
        }
    }

    // Update is called once per frame
    void Update ()
    {
		
	}
}
