using UnityEngine;
using System.Collections;

public enum Enum_EnemyState
{
    Patrol,
    InvestigateLocation,
    ChasePlayer
}

public class Cs_EnemyLogic_Grunt : MonoBehaviour
{
    [SerializeField] GameObject[] go_PatrolPath = new GameObject[20];

    [SerializeField] Enum_EnemyState e_EnemyState = Enum_EnemyState.Patrol;

    Vector3 v3_LastKnownLocation;

    GameObject go_ExclamationMark;
    GameObject go_QuestionMark;

    GameObject go_LevelLogic;

    [SerializeField] int i_StartPatrolPoint = 0;

    // Audio & SFX
    AudioSource as_SFXSource;
    AudioClip ac_Grass_Light;
    AudioClip ac_Grass_Heavy;

    // Use this for initialization
    void Start ()
    {
        go_LevelLogic = GameObject.Find("LevelLogic");

        // Load SFX
        as_SFXSource = gameObject.GetComponent<AudioSource>();
        ac_Grass_Light = Resources.Load("SFX_Step_Light") as AudioClip;
        ac_Grass_Heavy = Resources.Load("SFX_Step_Heavy") as AudioClip;

        // If there's a custom start position...
        if (i_StartPatrolPoint > 0)
        {
            // Check to see the patrol position exists
            if(go_PatrolPath[i_StartPatrolPoint] != null)
            {
                // Set the new default position
                i_PatrolPoint = i_StartPatrolPoint;
            }
        }

        // Set the first patrol point for the guard
        if(gameObject.GetComponent<NavMeshAgent>().enabled)
        {
            NavMeshAgent agent = GetComponent<NavMeshAgent>();
            if(go_PatrolPath[i_PatrolPoint] != null)
            {
                agent.destination = go_PatrolPath[i_PatrolPoint].transform.position;
            }
            else { print(gameObject.name + " NEEDS A PATROL PATH "); }
        }

        // Set the wait time for the first patrol point
        if(go_PatrolPath[i_PatrolPoint] != null)
        {
            f_MAX_WAIT_TIME = go_PatrolPath[i_PatrolPoint].GetComponent<Cs_PatrolPointLogic>().GetWaitTime();
        }

        // Set the models above the player
        go_ExclamationMark = transform.Find("Mdl_ExclamationMark").gameObject;
        go_QuestionMark = transform.Find("Mdl_QuestionMark").gameObject;

        // Grab timers from the LevelLogic system
        Cs_LevelLogic[] lvlLogic = GameObject.Find("LevelLogic").GetComponents<Cs_LevelLogic>();
        for (int i_ = 0; i_ < lvlLogic.Length; ++i_)
        {
            // Call their 'Get Current Timer' functions, which also determine if this script contains this enemy
            float f_TimeCheck = lvlLogic[i_].Get_CurrentTimer(Enum_EnemyState.InvestigateLocation, true, gameObject);
            if(f_TimeCheck != -1) f_MAX_INVESTIGATE_TIME = f_TimeCheck;
            
            f_TimeCheck = lvlLogic[i_].Get_CurrentTimer(Enum_EnemyState.InvestigateLocation, false, gameObject);
            if (f_TimeCheck != -1) f_InvestigateTimer = f_TimeCheck;
        }

        GoToState_Patrol();
    }

    float f_PatrolWaitTimer;
    float f_MAX_WAIT_TIME = 0.0f;
    int i_PatrolPoint = 0;

    float f_InvestigateTimer;
    float f_MAX_INVESTIGATE_TIME = 5.0f;

    float f_BasicMoveSpeed = 4f;
    float f_StoppingDistance = 0.1f;
    float f_Acceleration = 6f;
    public void GoToState_Patrol()
    {
        #region Reset Basic Details
        gameObject.GetComponent<NavMeshAgent>().stoppingDistance = f_StoppingDistance;
        gameObject.GetComponent<NavMeshAgent>().speed = f_BasicMoveSpeed;
        gameObject.GetComponent<NavMeshAgent>().acceleration = f_Acceleration;
        gameObject.GetComponent<NavMeshAgent>().updateRotation = true;

        // Reset timer
        f_PatrolWaitTimer = 0f;

        // Set next wait timer
        if(go_PatrolPath[i_PatrolPoint])
        {
            f_MAX_WAIT_TIME = go_PatrolPath[i_PatrolPoint].GetComponent<Cs_PatrolPointLogic>().GetWaitTime();
        }
        #endregion

        // Go To State
        e_EnemyState = Enum_EnemyState.Patrol;
    }

    Vector3 v3_InvestigateLocation;
    float f_SprintMoveSpeed = 6f;
    public void GoToState_InvestigateLocation()
    {
        if(v3_InvestigateLocation != new Vector3())
        {
            GoToState_InvestigateLocation(v3_InvestigateLocation);
        }
        else
        {
            print("Enemy " + gameObject.name + " has no investigate point! Going to 'patrol'");

            GoToState_Patrol();
        }
    }
    public void GoToState_InvestigateLocation( Vector3 v3_InvestigateLocation_ )
    {
        #region Reset Basic Details
        // f_InvestigateTimer = 0.0f;
        f_InvestigateTimer = f_MAX_INVESTIGATE_TIME;

        // Adds 0.5f to the height position so the enemy can reach it
        Vector3 v3_InvestigateLocation = v3_InvestigateLocation_;
        v3_InvestigateLocation.y += 0.5f;

        gameObject.GetComponent<NavMeshAgent>().destination = v3_InvestigateLocation;
        gameObject.GetComponent<NavMeshAgent>().stoppingDistance = f_StoppingDistance;
        gameObject.GetComponent<NavMeshAgent>().speed = f_SprintMoveSpeed;
        gameObject.GetComponent<NavMeshAgent>().acceleration = 5.0f;
        gameObject.GetComponent<NavMeshAgent>().updateRotation = true;
        #endregion

        // Go To State
        e_EnemyState = Enum_EnemyState.InvestigateLocation;
    }
    
    public void GoToState_ChasePlayer( Vector3 v3_PlayerLastKnownLocation_, bool b_SeeThePlayer_ = false)
    {
        #region Reset basic details
        // Go To State
        e_EnemyState = Enum_EnemyState.ChasePlayer;

        v3_LastKnownLocation = v3_PlayerLastKnownLocation_;

        gameObject.GetComponent<NavMeshAgent>().destination = v3_LastKnownLocation;
        gameObject.GetComponent<NavMeshAgent>().stoppingDistance = f_StoppingDistance;
        gameObject.GetComponent<NavMeshAgent>().speed = f_SprintMoveSpeed * 1.5f;
        gameObject.GetComponent<NavMeshAgent>().acceleration = 8;
        gameObject.GetComponent<NavMeshAgent>().updateRotation = false;
        #endregion

    }

    void SetIconState( Enum_EnemyState e_EnemyState_ )
    {
        // Set the exclamation mark
        if(e_EnemyState_ == Enum_EnemyState.ChasePlayer)
        {
            go_ExclamationMark.GetComponent<MeshRenderer>().enabled = true;
            go_QuestionMark.GetComponent<MeshRenderer>().enabled = false;
        }
        else if(e_EnemyState_ == Enum_EnemyState.InvestigateLocation)
        {
            go_ExclamationMark.GetComponent<MeshRenderer>().enabled = false;
            go_QuestionMark.GetComponent<MeshRenderer>().enabled = true;
        }
        else // Default state
        {
            go_ExclamationMark.GetComponent<MeshRenderer>().enabled = false;
            go_QuestionMark.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    float f_VolumeZeroAtDistance = 25.0f;
    float f_VolumeFullAtDistance = 5.0f;
    Vector3 v3_CurrentLocation;
    Vector3 v3_PreviousLocation;
    void SetVolume()
    {
        // Evaluate distance right now. If not within range, cut out.
        Vector3 v3_EnemyLocation = gameObject.transform.position;
        Vector3 v3_PlayerLocation = GameObject.Find("Player").transform.position;
        float f_DistanceToPlayer = Vector3.Distance(v3_EnemyLocation, v3_PlayerLocation);

        // Used to determine amount of distance moved
        v3_CurrentLocation = gameObject.transform.position;

        // If we're not even within range, set volume to 0f and move on OR if we haven't moved enough
        if(f_DistanceToPlayer > f_VolumeZeroAtDistance || Vector3.Distance(v3_CurrentLocation, v3_PreviousLocation) < 0.01f)
        {
            as_SFXSource.volume = 0f;

            return;
        }
        else
        {
            // If this enemy has moved far enough this frame, begin evaluating distance & set volume
            {
                // Evaluate distance away and increase volume based on distance
                if( f_DistanceToPlayer < f_VolumeZeroAtDistance && f_DistanceToPlayer < f_VolumeFullAtDistance )
                {
                    float f_Perc = (f_DistanceToPlayer - f_VolumeFullAtDistance) / (f_VolumeZeroAtDistance - f_VolumeFullAtDistance);

                    // Minimum volume
                    if (f_Perc < 0.3f) f_Perc = 0.3f;

                    // Set volume
                    as_SFXSource.volume = f_Perc;
                }
                else
                {
                    // Cap volume
                    as_SFXSource.volume = 1f;
                }
            }
        }

        // Used to determine amount of distance moved
        v3_PreviousLocation = v3_CurrentLocation;
    }

    // Update is called once per frame
    float f_WalkSFX_Timer = 0.5f;
    static float f_WalkSFX_Max = 0.75f;
	void Update ()
    {
        SetVolume();

        #region SFX
        f_WalkSFX_Timer += Time.deltaTime * (gameObject.GetComponent<NavMeshAgent>().speed / f_BasicMoveSpeed);

        if(f_WalkSFX_Timer > 0.75f)
        {
            // Reset the timer
            f_WalkSFX_Timer = 0.0f;

            if(gameObject.GetComponent<NavMeshAgent>().speed == 4)
            {
                as_SFXSource.PlayOneShot(ac_Grass_Light);
            }
            else
            {
                as_SFXSource.PlayOneShot(ac_Grass_Heavy);
            }
        }
        #endregion

        #region Patrol
        if (e_EnemyState == Enum_EnemyState.Patrol)
        {
            // If within a set distance of the patrol point, increment the Wait Timer
            if (gameObject.GetComponent<NavMeshAgent>().enabled)
            {
                Vector3 v3_PatrolPos = gameObject.transform.position;

                if (go_PatrolPath[i_PatrolPoint])
                {
                    v3_PatrolPos = go_PatrolPath[i_PatrolPoint].transform.position;
                }

                gameObject.GetComponent<NavMeshAgent>().destination = v3_PatrolPos;
                gameObject.GetComponent<NavMeshAgent>().stoppingDistance = 0.1f;

                // if (gameObject.GetComponent<NavMeshAgent>().remainingDistance <= gameObject.GetComponent<NavMeshAgent>().radius + 0.15f)
                if (Vector3.Distance(gameObject.transform.position, v3_PatrolPos) <= gameObject.GetComponent<NavMeshAgent>().radius + 0.15f)
                {
                    f_PatrolWaitTimer += Time.deltaTime;

                    // Make the enemy rotate to match the angle of the patrol point
                    if(f_MAX_WAIT_TIME != 0)
                    {
                        // Disable the enemies ability to rotate naturally
                        gameObject.GetComponent<NavMeshAgent>().updateRotation = false;

                        // Set the manual rotation
                        Quaternion q_CurrRot = gameObject.transform.rotation;
                        q_CurrRot = Quaternion.Lerp(gameObject.transform.rotation, go_PatrolPath[i_PatrolPoint].transform.rotation, 3 * Time.deltaTime);
                        gameObject.transform.rotation = q_CurrRot;
                    }

                    // If the Wait Timer reaches a certain point, go to the next point & reset the timer
                    if (f_PatrolWaitTimer >= f_MAX_WAIT_TIME && f_MAX_WAIT_TIME >= 0.0f)
                    {
                        gameObject.GetComponent<NavMeshAgent>().updateRotation = true;

                        // Increment/Reset
                        if (go_PatrolPath[i_PatrolPoint + 1] != null && (i_PatrolPoint + 1) < go_PatrolPath.Length)
                        {
                            ++i_PatrolPoint;
                        }
                        else
                        {
                            i_PatrolPoint = 0;
                        }

                        // Set next wait timer
                        if(go_PatrolPath[i_PatrolPoint])
                        {
                            f_MAX_WAIT_TIME = go_PatrolPath[i_PatrolPoint].GetComponent<Cs_PatrolPointLogic>().GetWaitTime();
                        }

                        // Reset timer
                        f_PatrolWaitTimer = 0f;
                    }
                }
            }
        }
        #endregion

        #region Investigate
        else if (e_EnemyState == Enum_EnemyState.InvestigateLocation)
        {
            // if (Vector3.Distance(gameObject.transform.position, v3_InvestigateLocation) <= gameObject.GetComponent<NavMeshAgent>().radius + 0.15f)
            if (gameObject.GetComponent<NavMeshAgent>().remainingDistance <= gameObject.GetComponent<NavMeshAgent>().radius + 0.15f)
            {
                // f_InvestigateTimer += Time.deltaTime;
                f_InvestigateTimer -= Time.deltaTime;

                // If the Wait Timer reaches a certain point, go to the next point & reset the timer
                if (f_InvestigateTimer <= 0)
                {
                    // GoToState_Patrol();
                    if(gameObject != null)
                    {
                        Cs_LevelLogic[] levelLogic = go_LevelLogic.GetComponents<Cs_LevelLogic>();
                        for(int i_ = 0; i_ < levelLogic.Length; ++i_)
                        {
                            levelLogic[i_].Set_PatrolState(gameObject);
                        }
                    }
                }
            }
        }
        #endregion

        #region Chase
        else if (e_EnemyState == Enum_EnemyState.ChasePlayer)
        {
            // Lerp between the enemies current look rotation and where the player's position is
            var targetRotation = Quaternion.LookRotation(v3_LastKnownLocation - gameObject.transform.position);
            
            gameObject.transform.rotation = Quaternion.Slerp(gameObject.transform.rotation, targetRotation, 10 * Time.deltaTime);
            
            gameObject.GetComponent<NavMeshAgent>().destination = v3_LastKnownLocation;
        }
        #endregion

        SetIconState(e_EnemyState);
    } // End Update
}
