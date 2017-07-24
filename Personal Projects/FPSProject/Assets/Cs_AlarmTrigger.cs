using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cs_AlarmTrigger : MonoBehaviour
{
    GameObject go_Player;
    float f_Timer;

    GameObject as_Source;

	// Use this for initialization
	void Start ()
    {
        go_Player = GameObject.Find("Player");
        as_Source = go_Player.transform.Find("AudioSource").gameObject;

        f_Timer = 3.0f;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (f_Timer <= 3.0f) f_Timer += Time.deltaTime;
	}

    private void OnTriggerStay(Collider collider_)
    {
        if(f_Timer >= 3.0f)
        {
            if(collider_.gameObject == go_Player)
            {
                f_Timer = 0.0f;

                as_Source.GetComponent<AudioSource>().Play();
            }
        }
    }
}
