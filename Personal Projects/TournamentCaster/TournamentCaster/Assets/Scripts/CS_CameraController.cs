/*!************************************************************************
\file    CS_CameraController
\author  Christopher Christensen (C.Christensen)
\date    10/27/2015

\brief   A camera controller for the TournamentCaster app

**************************************************************************/

using UnityEngine;
using System.Collections;

public class CS_CameraController : MonoBehaviour
{
	public GameObject cam_MainCamera;
	private Vector3 V3_currPos = new Vector3();
    // private Bounds bounds = new Bounds();

	// Use this for initialization
	void Start()
	{
        /*
		var bounds = GetComponent<SpriteRenderer>().sprite.bounds;
        var xSize = bounds.size.x;
        var ySize = bounds.size.y;
        gameObject.transform.localScale = new Vector3(1920 / xSize, 1080 / ySize, 1);
        */
	}

	// FixedUpdate relates to camera information
    void FixedUpdate()
    {
        
    }
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}
