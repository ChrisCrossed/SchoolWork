using UnityEngine;
using System.Collections;

public class Cs_EndGame : MonoBehaviour
{

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    void OnTriggerEnter( Collider collider_ )
    {
        if(collider_.gameObject.tag == "Player")
        {
            collider_.gameObject.GetComponent<Cs_SkiingPlayerController>().Set_EndGame();

            GameObject.Destroy(gameObject);
        }
    }
}
