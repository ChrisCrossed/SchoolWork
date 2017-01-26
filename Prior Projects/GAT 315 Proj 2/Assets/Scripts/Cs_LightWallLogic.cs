using UnityEngine;
using System.Collections;

public class Cs_LightWallLogic : MonoBehaviour
{
    public GameObject LightWallPos;
	
	// Update is called once per frame
	void Update ()
    {
        if(LightWallPos != null)
        {
            Vector3 newPos = Vector3.Lerp(gameObject.transform.position, LightWallPos.transform.position, 1.0f);
            gameObject.transform.position = newPos;
        }
	}
}
