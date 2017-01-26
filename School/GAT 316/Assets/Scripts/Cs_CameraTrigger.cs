using UnityEngine;
using System.Collections;

public class Cs_CameraTrigger : MonoBehaviour
{
    public GameObject go_CameraObj;
    GameObject go_Player;

    [SerializeField] bool b_StartPosition;
    float f_DestroyTimer;

    [SerializeField] float f_TimerDisableOnTouch;
    [SerializeField] float f_DestroyAfterTime = -1;

	// Use this for initialization
	void Start ()
    {
        // Initialize objects
        go_Player = GameObject.Find("Player");

        if (go_CameraObj == null)
        {
            print(gameObject.name + " at " + gameObject.transform.position + " has no camera!");
        }

        // Ensure that if there's a timer assigned to 'DisableOnTouchTimer', that we force 'b_StartPosition' to be enabled
        if (f_TimerDisableOnTouch > 0) b_StartPosition = true;
	}

    void Update()
    {
        if(f_DestroyAfterTime > 0 && f_DestroyTimer > 0)
        {
            f_DestroyTimer -= Time.deltaTime;

            if (f_DestroyTimer <= 0)
            {
                gameObject.GetComponent<Collider>().enabled = false;

                // Tell player's camera to return to default
                go_Player.GetComponent<Cs_PlayerController>().Set_CameraPosition();
            }
        }
    }

    void OnTriggerEnter( Collider collision_ )
    {
        GameObject go_CollisionObj = collision_.transform.root.gameObject;

        if (go_CollisionObj.tag == "Player")
        {
            if( f_TimerDisableOnTouch > 0 && b_StartPosition )
            {
                go_CollisionObj.GetComponent<Cs_PlayerController>().Set_PlayerDisableTimer(f_TimerDisableOnTouch);
            }

            f_DestroyTimer = f_DestroyAfterTime;
        }
    }

    void OnTriggerStay( Collider collision_ )
    {
        GameObject go_CollisionObj = collision_.transform.root.gameObject;

        if(go_CollisionObj.tag == "Player")
        {
            if(go_CameraObj != null)
            {
                // Tell player's camera to lerp to this game object
                go_Player.GetComponent<Cs_PlayerController>().Set_CameraPosition(go_CameraObj);
            }
        }
    }

    void OnTriggerExit( Collider collision_ )
    {
        GameObject go_CollisionObj = collision_.transform.root.gameObject;

        if (go_CollisionObj.tag == "Player")
        {
            if (go_CameraObj != null)
            {
                // Tell player's camera to return to default
                go_Player.GetComponent<Cs_PlayerController>().Set_CameraPosition();
            }

            if (b_StartPosition)
            {
                // Hacky way to keep the object around to refer to while not allowing the player to touch it
                gameObject.GetComponent<Collider>().enabled = false;
            }
        }
    }
}
