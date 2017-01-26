using UnityEngine;
using System.Collections;

public class Cs_DoorLogic : MonoBehaviour
{
    float f_OpenYPos;
    bool b_DoorOpen;

	// Use this for initialization
	void Start ()
    {
        f_OpenYPos = (-gameObject.transform.position.y) - 0.5f;
	}
	
	// Update is called once per frame
	void Update ()
    {
	    if(b_DoorOpen)
        {
            Vector3 newPos = gameObject.transform.position;
            newPos.y -= Time.deltaTime * 5;
            gameObject.transform.position = newPos;

            if(newPos.y <= f_OpenYPos)
            {
                GameObject.Destroy(gameObject);
            }
        }
	}

    public void OpenDoor()
    {
        b_DoorOpen = true;
    }
}
