using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SetTextToRootName : MonoBehaviour
{
	// Use this for initialization
	void Start ()
    {
        GetComponent<Text>().text = transform.root.name;
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}
}
