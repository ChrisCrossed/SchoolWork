using UnityEngine;
using System.Collections;

public class Cs_LightWall : MonoBehaviour
{
    float f_Timer;

	// Use this for initialization
	void Start ()
    {
        gameObject.GetComponent<BoxCollider>().enabled = false;
	}
	
	// Update is called once per frame
	void Update ()
    {
	    if(f_Timer <= 0.5f)
        {
            f_Timer += Time.deltaTime;

            if(f_Timer >= 0.5f)
            {
                gameObject.GetComponent<BoxCollider>().enabled = true;
            }
        }
	}
}
