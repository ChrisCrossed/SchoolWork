using UnityEngine;
using System.Collections;

public class Cs_ObjectiveLogic : MonoBehaviour
{
    GameObject mdl_Briefcase;

	// Use this for initialization
	void Start ()
    {
        gameObject.GetComponent<MeshRenderer>().enabled = false;

        mdl_Briefcase = transform.Find("mdl_Briefcase").gameObject;
	}

    bool b_MakeDoorInvisForever;
    void Update()
    {
        if(b_MakeDoorInvisForever)
        {
            go_Door.GetComponent<Cs_WallLogic>().SetVisibilityState(true);
        }
    }

    GameObject go_Door;
    void OnTriggerEnter( Collider collider_ )
    {
        if (collider_.transform.root.gameObject.name == "Player")
        {
            RaycastHit hit;
            int i_LayerMask = LayerMask.GetMask("Player", "Wall");
            Vector3 v3_VectorToPlayer = collider_.transform.root.gameObject.transform.position - gameObject.transform.position;

            if (Physics.Raycast(gameObject.transform.position, v3_VectorToPlayer, out hit, float.PositiveInfinity, i_LayerMask))
            {
                if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
                {
                    mdl_Briefcase.GetComponent<Cs_BriefcaseLogic>().Set_PickedUp();

                    // 'Destroy' the wall guarding the briefcase
                    go_Door = GameObject.Find("Gate_Exit");
                    go_Door.GetComponent<BoxCollider>().isTrigger = true;
                    go_Door.GetComponent<Cs_GateScript>().Set_DoorOpen(true);
                    go_Door.GetComponent<Cs_GateScript>().Set_ObjectiveActive(false);

                    // Because I am lazy, set the objectives that need to know that the player's grabbed the objective
                    GameObject.Find("RoadLogic").GetComponent<Cs_RoadLogic>().ObjectiveCollected = true;

                    // TODO: Set the HUD to show the briefcase picked up
                    GameObject.Find("Canvas").GetComponent<Cs_ObjectiveWindow>().Set_GrabBriefcase = true;

                    b_MakeDoorInvisForever = true;
                }
            }
        }
    }
}
