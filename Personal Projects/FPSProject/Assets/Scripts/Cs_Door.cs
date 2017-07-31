using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cs_Door : MonoBehaviour
{
    // Door Closes after time
    [SerializeField] float DoorClosesAfterTime = 0f;
    float f_DoorCloseTimer;
    GameObject go_Player;

    // Door Options
    bool ControlledByButton = false;
    float f_DoorTimer_Max = 0.25f;

    // Position Vectors
    Vector3 v3_StartPosition;
    Vector3 v3_FinalPosition;
    [SerializeField] AnimationCurve ac;

    // Connected Objects
    [SerializeField] GameObject[] DisableLightsOnOpen;
    [SerializeField] GameObject[] EnableLightsOnOpen;

    // Music On Activation
    [SerializeField] bool PlaysSong = false;
    [SerializeField] Enum_Song SongToPlay = Enum_Song.None;
    Cs_MusicPlayer cs_MusicPlayer;

    // Use this for initialization
    protected void Start ()
    {
        v3_StartPosition = gameObject.transform.position;

        // Default is 'Door Opens Upward'
        v3_FinalPosition = v3_StartPosition;
        if(gameObject.GetComponent<BoxCollider>())
        {
            v3_FinalPosition.y += gameObject.GetComponent<BoxCollider>().size.y + 0.1f;
        }

        // No need to capture player information if Door doesn't close
        if(DoorClosesAfterTime > 0 || PlaysSong)
        {
            go_Player = GameObject.Find("Player");
            cs_MusicPlayer = go_Player.GetComponent<Cs_MusicPlayer>();
        }
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

                    EnableLights();
                    DisableLights();
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

            if(PlaysSong)
            {
                // Change soundtrack
                cs_MusicPlayer.RunningSong = SongToPlay;

                // Disable after first use
                PlaysSong = false;
            }
        }
    }

    public bool Set_ControlledByButton
    {
        set { ControlledByButton = value; }
    }

    float f_MinimumDistanceToPlayer = 5.0f;
    void CloseDoorBasedOnTimer()
    {
        // If door is open, increment the timer
        if (DoorState)
        {
            float f_DoorHeight = gameObject.GetComponent<BoxCollider>().size.y;

            // If the player is too close to the door, do not close
            if(Vector3.Distance(go_Player.transform.position, gameObject.transform.position) < f_MinimumDistanceToPlayer + (f_DoorHeight / 4f))
            {
                // Reset the door timer
                f_DoorCloseTimer = 0f;

                // Quit out without incrementing the timer;
                return;
            }

            // If under the max timer, increment / cap the timer
            if (f_DoorCloseTimer < DoorClosesAfterTime)
            {
                f_DoorCloseTimer += Time.deltaTime;

                // Time limit reached, close door
                if (f_DoorCloseTimer >= DoorClosesAfterTime)
                {
                    // Reset timer
                    f_DoorCloseTimer = 0f;

                    // Close door
                    DoorState = false;
                }
            }
        }
    }

    void EnableLights()
    {
        int i_NumLights_ = EnableLightsOnOpen.Length;

        if(i_NumLights_ > 0)
        {
            for(int i_ = 0; i_ < i_NumLights_; ++i_)
            {
                EnableLightsOnOpen[i_].GetComponent<Light>().enabled = true;
            }
        }
    }

    void DisableLights()
    {
        if (DisableLightsOnOpen.Length > 0)
        {
            int i_NumLights_ = DisableLightsOnOpen.Length;

            if (i_NumLights_ > 0)
            {
                for (int i_ = 0; i_ < i_NumLights_; ++i_)
                {
                    DisableLightsOnOpen[i_].GetComponent<Light>().enabled = false;
                }
            }
        }
    }

    // Update is called once per frame
    float f_MoveTimer;
	protected void Update ()
    {
		// If door is supposed to close after a certain amount of time, close it
        if(DoorClosesAfterTime > 0f)
        {
            CloseDoorBasedOnTimer();
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
