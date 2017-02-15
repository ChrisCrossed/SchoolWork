using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cs_Door : MonoBehaviour
{
    // Door Options
    bool ControlledByButton = false;
    float f_DistanceToCloseDoor = 10.0f;
    float f_DoorTimer_Max = 0.25f;

    // Position Vectors
    Vector3 v3_StartPosition;
    Vector3 v3_FinalPosition;
    [SerializeField] AnimationCurve ac;

    // Player connector
    GameObject go_Player;

	// Use this for initialization
	void Start ()
    {
        v3_StartPosition = gameObject.transform.position;

        // Default is 'Door Opens Upward'
        v3_FinalPosition = v3_StartPosition;
        if(gameObject.GetComponent<BoxCollider>())
        {
            v3_FinalPosition.y += gameObject.GetComponent<BoxCollider>().size.y + 0.1f;
        }

        // Find and attach Player
        go_Player = GameObject.Find("Player");
    }

    bool b_Open;
    public bool DoorState
    {
        set
        {
            if ( !ControlledByButton || value == true )
            {
                b_Open = value;

                if(b_Open)
                {
                    gameObject.layer = LayerMask.NameToLayer("Default");
                }
                else
                {
                    gameObject.layer = LayerMask.NameToLayer("Use");
                }
            }
        }
        get { return b_Open; }
    }

    public void Use_OpenDoor()
    {
        // Only open door if it isn't already open
        if( !DoorState )
        {
            DoorState = true;
        }
    }

    public bool Set_ControlledByButton
    {
        set { ControlledByButton = value; }
    }

    // Update is called once per frame
    float f_MoveTimer;
	void Update ()
    {
		// If door is open and Not controlled by a button, then close when the player is a certain distance away.
        if( DoorState == true && !ControlledByButton )
        {
            if( Vector3.Distance(go_Player.transform.position, gameObject.transform.position) > f_DistanceToCloseDoor )
            {
                DoorState = false;
            }
        }
        
        // If the door is 'open', move to the Open position
        if( DoorState && f_MoveTimer < f_DoorTimer_Max )
        {
            f_MoveTimer += Time.fixedDeltaTime;
            if( f_MoveTimer > f_DoorTimer_Max ) f_MoveTimer = f_DoorTimer_Max;
        }
        // Otherwise, if the door is 'Closed', move to the Initial position
        else if( !DoorState && f_MoveTimer > 0f )
        {
            f_MoveTimer -= Time.fixedDeltaTime;
            if( f_MoveTimer < 0f ) f_MoveTimer = 0f;
        }

        float f_Perc = ac.Evaluate(f_MoveTimer / f_DoorTimer_Max);

        gameObject.transform.position = Vector3.Lerp( v3_StartPosition, v3_FinalPosition, f_Perc );
	}
}
