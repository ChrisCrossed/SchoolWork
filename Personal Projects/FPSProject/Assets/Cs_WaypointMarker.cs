using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cs_WaypointMarker : MonoBehaviour
{
    // Use this for initialization
	void Start ()
    {
        if(!GameObject.Find("WaypointList"))
        {
            GameObject go_WaypointList = new GameObject();
            go_WaypointList.transform.position = new Vector3(0, 0, 0);
            go_WaypointList.name = "WaypointList";
            go_WaypointList = null;
        }

        gameObject.transform.SetParent(GameObject.Find("WaypointList").transform);

        // Disable mesh renderer
        gameObject.GetComponent<MeshRenderer>().enabled = false;

        // Disable collider
        gameObject.GetComponent<BoxCollider>().enabled = false;

        // Position object one unit above
        Vector3 v3_Pos_ = gameObject.transform.position;
        v3_Pos_.y += 1f;
        gameObject.transform.position = v3_Pos_;

        // Raycast downward
        int i_LayerMask_ = LayerMask.GetMask("Ground", "Default");
        RaycastHit hit_;
        if( Physics.Raycast( gameObject.transform.position, Vector3.down, out hit_, 10.0f, i_LayerMask_ ) )
        {
            Vector3 v3_NewPos_ = hit_.point;
            v3_NewPos_.y += 0.75f;
            gameObject.transform.position = v3_NewPos_;
        }
	}

    public Vector3 WaypointLocation()
    {
        return gameObject.transform.position;
    }
}
