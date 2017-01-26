using UnityEngine;
using System.Collections;

public class Cs_LevelLogic : MonoBehaviour
{
    [SerializeField] string NameOfRegion_ReadOnly;

    [SerializeField] float f_MaxTimer_FromChaseToInvestigate;
    float f_Timer_FromChaseToInvestigate;

    [SerializeField] float f_MaxTimer_FromInvestigateToPatrol;
    float f_Timer_FromInvestigateToPatrol;

    [SerializeField] GameObject[] DisableObjectsOnChase = new GameObject[5];
    [SerializeField] GameObject[] EnableObjectsOnChase = new GameObject[5];

    [SerializeField] GameObject[] DisableObjectsOnInvestigate = new GameObject[5];
    [SerializeField] GameObject[] EnableObjectsOnInvestigate = new GameObject[5];

    [SerializeField] GameObject[] DisableObjectsOnPatrol = new GameObject[5];
    [SerializeField] GameObject[] EnableObjectsOnPatrol = new GameObject[5];

    [SerializeField] GameObject[] EnemiesToControl = new GameObject[5];
    Enum_EnemyState e_EnemiesState = Enum_EnemyState.Patrol;

    GameObject go_Player;
    GameObject go_MusicManager;

    // Use this for initialization
    void Start ()
    {
        go_Player = GameObject.Find("Player");
        go_MusicManager = GameObject.Find("MusicManager");

        HUD_Object = GameObject.Find("Canvas").GetComponent<Cs_HUDController>();
    }

    bool CheckEnemyInList( GameObject go_EnemyObject_ )
    {
        // If an enemy is provided, check to see if this enemy is within the list of controllable enemies
        if (go_EnemyObject_ != null)
        {
            for( int i_ = 0; i_ < EnemiesToControl.Length; ++i_)
            {
                if( go_EnemyObject_ == EnemiesToControl[i_] )
                {
                    return true;
                }
            }
        }

        return false;
    }

    public void Set_ChaseState( GameObject go_EnemyObject_ = null)
    {
        if(go_EnemyObject_ != null)
        {
            if(CheckEnemyInList( go_EnemyObject_ ) == false)
            {
                return;
            }
        }

        // Player was caught, turn off the Objective Window setting
        if(!GameObject.Find("Canvas").GetComponent<Cs_ObjectiveWindow>().Set_GrabBriefcase)
        {
            GameObject.Find("Canvas").GetComponent<Cs_ObjectiveWindow>().Set_DeactivateGenerator = false;
        }

        // When the player is spotted, reset the f_Timer
        f_Timer_FromChaseToInvestigate = f_MaxTimer_FromChaseToInvestigate;

        // Set the state of the enemies involved
        e_EnemiesState = Enum_EnemyState.ChasePlayer;

        // Run through all appropriate enemies and tell them to chase the player
        for(int i_ = 0; i_ < EnemiesToControl.Length; ++i_)
        {
            if(EnemiesToControl[i_] != null)
            {
                if(EnemiesToControl[i_].GetComponent<Cs_EnemyLogic_Grunt>())
                {
                    EnemiesToControl[i_].GetComponent<Cs_EnemyLogic_Grunt>().GoToState_ChasePlayer(go_Player.transform.position, true);
                }
            }
        }

        // Enable appropriate objects for 'Chase' state
        EnableObjectsInList(EnableObjectsOnChase);

        // Disable appropriate objects for 'Chase' state
        DisableObjectsInList(DisableObjectsOnChase);

        go_MusicManager.GetComponent<Cs_MusicManager>().Set_MusicState(Enum_EnemyState.ChasePlayer);
    }

    public void Set_InvestigateState( GameObject go_EnemyObject_ = null)
    {
        // Check to make sure the enemy calling this is within the list
        if (go_EnemyObject_ != null)
        {
            if (CheckEnemyInList(go_EnemyObject_) == false)
            {
                return;
            }

            // Reset timer
            f_Timer_FromInvestigateToPatrol = f_MaxTimer_FromInvestigateToPatrol;

            // Set the state of the enemies involved
            e_EnemiesState = Enum_EnemyState.InvestigateLocation;

            for (int i_ = 0; i_ < EnemiesToControl.Length; ++i_)
            {
                if(EnemiesToControl[i_] != null)
                {
                    if (EnemiesToControl[i_].GetComponent<Cs_EnemyLogic_Grunt>())
                    {
                        EnemiesToControl[i_].GetComponent<Cs_EnemyLogic_Grunt>().GoToState_InvestigateLocation(go_Player.transform.position);
                    }
                }
            }
        
            // Enable appropriate objects for 'Investigate' state
            EnableObjectsInList( EnableObjectsOnInvestigate );

            // Disable appropriate objects for 'Investigate' state
            DisableObjectsInList( DisableObjectsOnInvestigate );

            go_MusicManager.GetComponent<Cs_MusicManager>().Set_MusicState(Enum_EnemyState.InvestigateLocation);
        }
    }

    public void Set_PatrolState( GameObject go_EnemyObject_ = null )
    {
        // Check that the enemy gameobject exists in *this* list
        if (go_EnemyObject_ != null)
        {
            if (CheckEnemyInList(go_EnemyObject_) == false)
            {
                return;
            }
        }

        // Set appropriate timers
        f_Timer_FromChaseToInvestigate = f_MaxTimer_FromChaseToInvestigate;
        f_Timer_FromInvestigateToPatrol = f_MaxTimer_FromInvestigateToPatrol;

        // Set state
        e_EnemiesState = Enum_EnemyState.Patrol;

        // Set all enemies in *this* list to go patrol
        for (int i_ = 0; i_ < EnemiesToControl.Length; ++i_)
        {
            if(EnemiesToControl[i_] != null)
            {
                if (EnemiesToControl[i_].GetComponent<Cs_EnemyLogic_Grunt>())
                {
                   EnemiesToControl[i_].GetComponent<Cs_EnemyLogic_Grunt>().GoToState_Patrol();
                }
            }
        }
        
        // Enable appropriate objects for 'Patrol' state
        EnableObjectsInList( EnableObjectsOnPatrol );

        // Disable appropriate objects for 'Patrol' state
        DisableObjectsInList( DisableObjectsOnPatrol );

        go_MusicManager.GetComponent<Cs_MusicManager>().Set_MusicState(Enum_EnemyState.Patrol);
    }

    public float Get_CurrentTimer(Enum_EnemyState e_EnemyState_, bool b_GetMaxTimer = false, GameObject go_EnemyObject_ = null )
    {
        if (go_EnemyObject_ != null)
        {
            if (CheckEnemyInList(go_EnemyObject_) == false)
            {
                return -1f;
            }
        }

        if (e_EnemyState_ == Enum_EnemyState.ChasePlayer)
        {
            if (!b_GetMaxTimer) return f_Timer_FromChaseToInvestigate;
            else return f_MaxTimer_FromChaseToInvestigate;
        }
        else if(e_EnemyState_ == Enum_EnemyState.InvestigateLocation)
        {
            if (!b_GetMaxTimer) return f_Timer_FromInvestigateToPatrol;
            else return f_MaxTimer_FromInvestigateToPatrol;
        }

        return 0f;
    }

    void EnableObjectsInList(GameObject[] go_EnableList_)
    {
        for (int i_ = 0; i_ < go_EnableList_.Length; ++i_)
        {
            if(go_EnableList_[i_] != null)
            {
                // Gates
                if(go_EnableList_[i_].GetComponent<Cs_GateScript>())
                {
                    go_EnableList_[i_].GetComponent<Cs_GateScript>().Set_DoorOpen(true);
                }

                // Sirens
                if(go_EnableList_[i_].GetComponent<Cs_Siren>())
                {
                    go_EnableList_[i_].GetComponent<Cs_Siren>().Set_Enabled = true;
                }
            }
        }
    }

    void DisableObjectsInList(GameObject[] go_DisableList_)
    {
        for (int i_ = 0; i_ < go_DisableList_.Length; ++i_)
        {
            if (go_DisableList_[i_] != null)
            {
                // Gates
                if (go_DisableList_[i_].GetComponent<Cs_GateScript>())
                {
                    go_DisableList_[i_].GetComponent<Cs_GateScript>().Set_DoorOpen(false);
                }

                // Sirens
                if (go_DisableList_[i_].GetComponent<Cs_Siren>())
                {
                    go_DisableList_[i_].GetComponent<Cs_Siren>().Set_Enabled = false;
                }
            }
        }
    }

    // Update is called once per frame
    Cs_HUDController HUD_Object;
	void Update ()
    {
	    if(e_EnemiesState == Enum_EnemyState.ChasePlayer)
        {
            f_Timer_FromChaseToInvestigate -= Time.deltaTime;

            // print("Chase->Invest: " + f_Timer_FromChaseToInvestigate);
            HUD_Object.Set_ScreenTimer = true;

            if(f_Timer_FromChaseToInvestigate <= 0)
            {
                // Reset timer
                f_Timer_FromChaseToInvestigate = f_MaxTimer_FromChaseToInvestigate;

                // Change state (Saying we don't know where the player is)
                Set_InvestigateState();
            }
        }
        else if(e_EnemiesState == Enum_EnemyState.InvestigateLocation)
        {
            f_Timer_FromInvestigateToPatrol -= Time.deltaTime;

            // print("Invest->Patrol: " + f_Timer_FromInvestigateToPatrol);

            if(f_Timer_FromInvestigateToPatrol <= 0)
            {
                // Reset timer
                f_Timer_FromInvestigateToPatrol = f_MaxTimer_FromInvestigateToPatrol;

                // Change state
                Set_PatrolState();
            }
        }
	}
}
