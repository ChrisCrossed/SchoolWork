using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cs_RaycastCollider : MonoBehaviour
{
    List<GameObject> go_ColliderList;
    int i_LayerMask;

	// Use this for initialization
	void Start ()
    {
        go_Player = GameObject.Find("RaycastPoint_Center");

        go_ColliderList = new List<GameObject>();

        i_LayerMask = LayerMask.GetMask("Ground");

        gameObject.GetComponent<MeshRenderer>().enabled = false;
    }

    public List<GameObject> GetColliderList
    {
        get { return go_ColliderList; }
    }

    void OnTriggerEnter(Collider col_)
    {
        // Add the GameObject collided with to the list.
        go_ColliderList.Add(col_.gameObject);
    }

    void OnTriggerExit(Collider col_)
    {
        if(go_ColliderList.Contains(col_.gameObject))
        {
            // Remove the GameObject collided with from the list.
            go_ColliderList.Remove(col_.gameObject);
        }
    }

    GameObject go_Player;
    private void Update()
    {
        // Find player position
        Vector3 v3_PlayerPos_ = go_Player.transform.position;

        // Raycast to find Y position
        RaycastHit hit_;
        if(Physics.Raycast(v3_PlayerPos_, Vector3.down, out hit_, 1.0f, i_LayerMask))
        {
            // Set new position
            v3_PlayerPos_ = hit_.point;

            // Move position up by half the collider's height
            // v3_PlayerPos_.y -= gameObject.transform.lossyScale.y;

            // Set new position
            gameObject.transform.position = v3_PlayerPos_;
        }
        else
        {
            // Set default position
            gameObject.transform.position = go_Player.transform.position;
        }
    }
}
