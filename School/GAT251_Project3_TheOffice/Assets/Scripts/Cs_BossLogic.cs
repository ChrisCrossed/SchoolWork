using UnityEngine;
using System.Collections;

public class Cs_BossLogic : MonoBehaviour
{
    GameObject go_BossSpot_1;
    GameObject go_BossSpot_2;
    GameObject go_BossSpot_3;
    GameObject go_BossSpot_4;

    GameObject go_CurrBossSpot;
    NavMeshAgent nav_Destination;

    // Use this for initialization
    void Start ()
    {
        go_BossSpot_1 = GameObject.Find("BossSpot_1");
        go_BossSpot_2 = GameObject.Find("BossSpot_2");
        go_BossSpot_3 = GameObject.Find("BossSpot_3");
        go_BossSpot_4 = GameObject.Find("BossSpot_4");

        // print("Set: 1");
        go_CurrBossSpot = go_BossSpot_1;
        nav_Destination = gameObject.GetComponent<NavMeshAgent>();
    }

    void Set_NewPosition( int i_Pos_ )
    {
        if ( i_Pos_ < 0 ) i_Pos_ = 0;
        if ( i_Pos_ > 3 ) i_Pos_ = 3;

        if(i_Pos_ == 0)
        {
            go_CurrBossSpot = go_BossSpot_1;
        }
        else if( i_Pos_ == 1)
        {
            go_CurrBossSpot = go_BossSpot_2;
        }
        else if (i_Pos_ == 2)
        {
            go_CurrBossSpot = go_BossSpot_3;
        }
        else if (i_Pos_ == 3)
        {
            go_CurrBossSpot = go_BossSpot_4;
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        if(nav_Destination.remainingDistance < 0.15f)
        {
            if(go_CurrBossSpot == go_BossSpot_1)
            {
                Set_NewPosition( 1 );
            }
            else if (go_CurrBossSpot == go_BossSpot_2)
            {
                Set_NewPosition( 2 );
            }
            else if (go_CurrBossSpot == go_BossSpot_3)
            {
                Set_NewPosition( 3 );
            }
            else if (go_CurrBossSpot == go_BossSpot_4)
            {
                Set_NewPosition( 0 );
            }
        }

        nav_Destination.SetDestination(go_CurrBossSpot.transform.position);
	}
}
