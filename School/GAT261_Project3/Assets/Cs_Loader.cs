using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cs_Loader : MonoBehaviour
{
    [SerializeField] GameObject go_RoadStripe;
    Transform RoadStrips;

    [SerializeField] GameObject go_Obstacle;
    Transform Obstacles;

	// Use this for initialization
	void Awake ()
    {
        RoadStrips = GameObject.Find("RoadStrips").transform;
        Obstacles = GameObject.Find("Obstacles").transform;

        Init_RoadStripes();
        Init_Obstacles();
	}
	
	// Create 'RoadStripes'
    void Init_RoadStripes()
    {
        for(float f_StartZ = -10f; f_StartZ < 575f; f_StartZ += 5f)
        {
            GameObject go_NewBlock = Instantiate(go_RoadStripe);

            Vector3 v3_BlockPos = go_NewBlock.transform.position;
            v3_BlockPos.y = -0.225f;
            v3_BlockPos.z = f_StartZ;

            go_NewBlock.transform.position = v3_BlockPos;
            go_NewBlock.transform.SetParent(RoadStrips);
        }
    }

    void Init_Obstacles()
    {
        bool b_LeftSide = false;

        for (float f_StartZ = 50f; f_StartZ < 635; )
        {
            GameObject go_NewObstacle = Instantiate(go_Obstacle);

            Vector3 v3_BlockPos = go_NewObstacle.transform.position;
            if (b_LeftSide) v3_BlockPos.x = -1f; else v3_BlockPos.x = 1f;
            v3_BlockPos.y = 0.5f;
            v3_BlockPos.z = Random.Range(3, 25) + f_StartZ;

            b_LeftSide = !b_LeftSide;
            f_StartZ = v3_BlockPos.z;

            go_NewObstacle.transform.position = v3_BlockPos;
            go_NewObstacle.transform.SetParent(Obstacles);
        }
    }
}
