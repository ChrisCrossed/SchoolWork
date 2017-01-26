using UnityEngine;
using System.Collections;

public class Cs_TurretAxel : MonoBehaviour
{
    GameObject player;

    bool b_IsEnabled = true;

	// Use this for initialization
	void Start ()
    {
        player = GameObject.Find("Mech");
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(b_IsEnabled)
        {
            transform.LookAt(player.transform.position);

            Vector3 newRot = transform.eulerAngles;
            newRot.x = 0;
            newRot.z = 0;

            transform.eulerAngles = newRot;
        }
	}

    public void SetState(bool b_Status_)
    {
        b_IsEnabled = b_Status_;
    }
}
