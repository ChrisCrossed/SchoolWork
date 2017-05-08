using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Cs_PlayerController : NetworkBehaviour
{
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        gameObject.GetComponent<MeshRenderer>().material.color = Color.blue;
    }

    // Update is called once per frame
    void Update ()
    {
        if (!isLocalPlayer) return;

        float x = Input.GetAxis("Horizontal") * Time.deltaTime * 150f;
        float z = Input.GetAxis("Vertical") * Time.deltaTime * 3.0f;

        gameObject.transform.Rotate(0, x, 0);
        gameObject.transform.Translate(0, 0, z);
	}
}
