using UnityEngine;
using System.Collections;

public class Cs_CenterObjective : MonoBehaviour {

	// Use this for initialization
	void OnTriggerEnter(Collider collider_)
    {
        if(collider_.gameObject.tag == "Enemy")
        {
            GameObject.Find("LevelController").GetComponent<Cs_LevelController>().EndGame();
        }
    }
}
