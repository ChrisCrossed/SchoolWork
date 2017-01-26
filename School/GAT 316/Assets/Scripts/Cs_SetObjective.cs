using UnityEngine;
using System.Collections;

public class Cs_SetObjective : MonoBehaviour
{
	void OnTriggerEnter( Collider collider_ )
    {
        if(collider_.transform.root.gameObject.name == "Player")
        {
            RaycastHit hit;
            int i_LayerMask = LayerMask.GetMask("Player", "Wall", "Ground");
            Vector3 v3_VectorToPlayer = collider_.transform.root.gameObject.transform.position - gameObject.transform.position;

            if (Physics.Raycast(gameObject.transform.position, v3_VectorToPlayer, out hit, float.PositiveInfinity, i_LayerMask))
            {
                if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
                {
                    GameObject.Find("Canvas").GetComponent<Cs_ObjectiveWindow>().Set_EnterCompound = true;
                }
            }
        }
    }
}
