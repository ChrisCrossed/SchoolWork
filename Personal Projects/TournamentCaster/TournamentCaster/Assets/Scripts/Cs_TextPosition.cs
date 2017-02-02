using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Cs_TextPosition : MonoBehaviour
{
    public GameObject ObjectToFollow;

	// Use this for initialization
	void Start ()
    {

	}
	
	// Update is called once per frame
	void Update ()
    {
        // guiText.transform.position = Camera.main.WorldToViewportPoint(ObjectToFollow.transform.position);
        transform.position = ObjectToFollow.gameObject.transform.position;
    }
}
