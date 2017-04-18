using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cs_Loader : MonoBehaviour
{
    [SerializeField] GameObject go_RoadStripe;
    Transform RoadStrips;

	// Use this for initialization
	void Awake ()
    {
        RoadStrips = GameObject.Find("RoadStrips").transform;

        Init_RoadStripes();
	}
	
	// Create 'RoadStripes'
    void Init_RoadStripes()
    {
        for(float f_StartZ = -10f; f_StartZ < 575; f_StartZ += 5f)
        {
            GameObject go_NewBlock = Instantiate(go_RoadStripe);

            Vector3 v3_BlockPos = go_NewBlock.transform.position;
            v3_BlockPos.y = -0.225f;
            v3_BlockPos.z = f_StartZ;

            go_NewBlock.transform.position = v3_BlockPos;
            go_NewBlock.transform.SetParent(RoadStrips);
        }
    }
}
