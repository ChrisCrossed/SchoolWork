using UnityEngine;
using System.Collections;

public class Cs_EnemyVisionLogic : MonoBehaviour
{
    GameObject go_Root;
    GameObject go_RaycastPoint;

    GameObject go_Player;

    int i_LayerMask;
    int i_LayerMask_NotPlayer;

    // Use this for initialization
    void Start ()
    {
        go_Player = GameObject.Find("Player");
        go_Root = gameObject.transform.root.gameObject;
        if(go_Root.transform.Find("VisionRaycast"))
        {
            go_RaycastPoint = go_Root.transform.Find("VisionRaycast").gameObject;
        }

        #region PRESENTATION STUFF
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        #endregion

        // LayerMask info
        i_LayerMask = LayerMask.NameToLayer("Player");
        i_LayerMask_NotPlayer = 9;  // Kinda have to hardcode the ground here. The player is '8', so anything greater than that is NOT the player. You can't use LayerMask.GetMask to return an int.
    }

    float f_SeePlayerTimer;
    bool b_PlayerInCollider;
    Vector3 v3_LastKnownLocation;
    // Update is called once per frame
    void Update ()
    {
        /*
        if (Input.GetKeyDown(KeyCode.I))
        {
            gameObject.GetComponent<MeshRenderer>().enabled = false;

            go_Root.GetComponent<Cs_EnemyLogic_Grunt>().GoToState_Patrol();
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            gameObject.GetComponent<MeshRenderer>().enabled = true;
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
        */
    }

    float f_ChasePlayerTimer;
    float f_ChasePlayerTimer_Max = 3.0f;
    void CheckToSeePlayer( Collider collider_ )
    {
        // Clarify which objects we want to cast to, specifically the player & objects they can hide behind.
        int i_LayerMask = LayerMask.GetMask("Player", "Wall", "Trigger", "Ground");

        // Find the vector between the raycast point & the player
        if(go_RaycastPoint != null)
        {
            Vector3 v3_Vector = new Vector3(go_Player.transform.position.x - go_RaycastPoint.transform.position.x,
                                            go_Player.transform.position.y - go_RaycastPoint.transform.position.y,
                                            go_Player.transform.position.z - go_RaycastPoint.transform.position.z);

            // Normalize the vector
            v3_Vector.Normalize();

            // Store the raycast information
            RaycastHit hit;

            // If a ray from the enemy, toward the player, hits *any* object specified, continue through
            if (Physics.Raycast(go_RaycastPoint.transform.position, v3_Vector, out hit, float.PositiveInfinity, i_LayerMask))
            {
                // Show the angle in the editor
                Debug.DrawRay(go_RaycastPoint.transform.position, v3_Vector * hit.distance, Color.red);

                // This intentionally blocks here so that objects (like walls) can interfere with spotting the player.
                if (hit.collider.gameObject.layer != LayerMask.NameToLayer("Player"))
                {
                    f_ChasePlayerTimer += Time.deltaTime;
                    if(f_ChasePlayerTimer >= f_ChasePlayerTimer_Max && b_PlayerInCollider)
                    {
                        b_PlayerInCollider = false;

                        go_Root.GetComponent<Cs_EnemyLogic_Grunt>().GoToState_InvestigateLocation(v3_LastKnownLocation);

                        // Find the 'LevelLogic' object and grab all the 'LevelLogic' scripts within it
                        Cs_LevelLogic[] lvlLogic = GameObject.Find("LevelLogic").GetComponents<Cs_LevelLogic>();
                        for(int i_ = 0; i_ < lvlLogic.Length; ++i_)
                        {
                            // Call each script's Investigate State
                            lvlLogic[i_].Set_InvestigateState(go_Root);
                        }
                    }
                
                    return;
                }
                // Otherwise, we hit the player.
                else
                {
                    // Reset the timer 
                    f_ChasePlayerTimer = 0.0f;

                    b_PlayerInCollider = true;

                    v3_LastKnownLocation = go_Player.transform.position;

                    go_Root.GetComponent<Cs_EnemyLogic_Grunt>().GoToState_ChasePlayer(v3_LastKnownLocation, true);

                    // print("Calling state: CHASE");

                    // Find the 'LevelLogic' object and grab all the 'LevelLogic' scripts within it
                    Cs_LevelLogic[] lvlLogic = GameObject.Find("LevelLogic").GetComponents<Cs_LevelLogic>();
                    for (int i_ = 0; i_ < lvlLogic.Length; ++i_)
                    {
                        // Call each script's Chase State
                        lvlLogic[i_].Set_ChaseState(go_Root);
                    }
                    // GameObject.Find("LevelLogic").GetComponent<Cs_LevelLogic>().Set_ChaseState(go_Root);
                }
            }
        }
    }

    void OnTriggerEnter( Collider collider_ )
    {
        #region Working Vision Code
        // If this is the VisionCone, we just see the player.
        if (gameObject.name == "VisionCone")
        {
            if (collider_.transform.root.gameObject.tag == "Player")
            {
                CheckToSeePlayer( collider_ );
            }
        }
        #endregion
    }

    void OnTriggerStay( Collider collider_ )
    {
        // print("Touching: " + collider_.gameObject.name);
        // print("Trigger: " + gameObject.name);

        // Check to see if this is the Radius Trigger (and not the Vision Trigger)
        if (gameObject.name == "RadiusTrigger")
        {
            // If this is the player...
            if (collider_.transform.root.gameObject.tag == "Player")
            {
                // Make sure the player is 'making noise' before trying to see the player
                if(go_Player.GetComponent<Rigidbody>().velocity.magnitude > float.Epsilon)
                {
                    // The player is 'making noise', so check to find the player
                    CheckToSeePlayer( collider_ );
                }
            }

            // if (b_PlayerInCollider) print("Collider touched, we see the player"); else print("Collider touched, we DO NOT see the player");
        }
        else if(gameObject.name == "VisionCone")
        {
            if (collider_.transform.root.gameObject.tag == "Player")
            {
                #region Working Vision Code
                // If this is the VisionCone, we just see the player.
                CheckToSeePlayer(collider_);
                #endregion
            }

            // if (b_PlayerInCollider) print("Collider touched, we see the player"); else print("Collider touched, we DO NOT see the player");
        }

    }

    void OnTriggerExit( Collider collider_ )
    {
        // Only tries to go to 'InvestigateLocation' if we saw the player in the first place. Otherwise, we just continue.
        if(b_PlayerInCollider)
        {
            if (collider_.transform.root.gameObject.tag == "Player")
            {
                b_PlayerInCollider = false;

                if (v3_LastKnownLocation != new Vector3())
                {
                    Cs_LevelLogic[] lvlLogic = GameObject.Find("LevelLogic").GetComponents<Cs_LevelLogic>();
                    for (int i_ = 0; i_ < lvlLogic.Length; ++i_)
                    {
                        // Call each script's Chase State
                        lvlLogic[i_].Set_InvestigateState( go_Root );
                    }

                    go_Root.GetComponent<Cs_EnemyLogic_Grunt>().GoToState_InvestigateLocation(v3_LastKnownLocation);
                }
            }
        }
    }
}
