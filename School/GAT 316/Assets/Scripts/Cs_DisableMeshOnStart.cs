using UnityEngine;
using System.Collections;

public class Cs_DisableMeshOnStart : MonoBehaviour
{

	// Use this for initialization
	void Start ()
    {
        gameObject.GetComponent<MeshRenderer>().enabled = false;
	}
}
