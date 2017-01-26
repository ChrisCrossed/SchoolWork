using UnityEngine;
using System.Collections;
using XInputDotNetPure; // Controller Input

public class Cs_PlayerController : MonoBehaviour
{
    GamePadState state;
    GamePadState prevState;
    public PlayerIndex playerIndex = PlayerIndex.One;

    // Use this for initialization
    void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        gameObject.GetComponent<Rigidbody>().velocity = transform.forward * 5;
	}
}
