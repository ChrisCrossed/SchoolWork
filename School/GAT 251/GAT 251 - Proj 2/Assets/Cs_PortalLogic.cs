using UnityEngine;
using System.Collections;

public class Cs_PortalLogic : MonoBehaviour
{
    [SerializeField] GameObject go_PortalLocation;
    GameObject go_Player;

    void Start()
    {
        go_Player = GameObject.Find("Player");
    }

	void OnTriggerEnter( Collider collider_ )
    {
        if(collider_.transform.root.name == "Player")
        {
            collider_.gameObject.transform.position = go_PortalLocation.transform.position;
        }
    }
}
