using UnityEngine;
using System.Collections;

public class Cs_CameraLogic : MonoBehaviour
{
    [SerializeField]
    GameObject go_RaycastPoint;

    // Store the current wall we recently raycast to
    GameObject go_CurrentWall;

	// Use this for initialization
	void Start ()
    {
	    
	}
	
	// Update is called once per frame
	void Update ()
    {
        // Raycast toward the player's 'SlopeRaycast' object. If we hit a 'wall', call that walls WallLogic transparency
        float f_Distance = Vector3.Distance(gameObject.transform.position, go_RaycastPoint.transform.position);

        RaycastHit hit;
        int i_LayerMask = LayerMask.GetMask("Wall");

        // Get the vector between the two points
        Vector3 v3_Vector = new Vector3();
        v3_Vector.x = go_RaycastPoint.transform.position.x - gameObject.transform.position.x;
        v3_Vector.y = go_RaycastPoint.transform.position.y - gameObject.transform.position.y;
        v3_Vector.z = go_RaycastPoint.transform.position.z - gameObject.transform.position.z;
        v3_Vector.Normalize();

        Debug.DrawRay(gameObject.transform.position, v3_Vector, Color.red);

        if(Physics.Raycast(gameObject.transform.position, v3_Vector, out hit, f_Distance, i_LayerMask))
        {
            if(hit.collider.gameObject != go_CurrentWall)
            {
                // Turn off the previous wall's transparency
                if(go_CurrentWall != null)
                {
                    if(go_CurrentWall.GetComponent<Cs_WallLogic>())
                    {
                        go_CurrentWall.GetComponent<Cs_WallLogic>().SetVisibilityState(false);
                    }
                }

                // Store the current wall
                go_CurrentWall = hit.collider.gameObject;

                // Enable the current wall's transparency
                if(go_CurrentWall.GetComponent<Cs_WallLogic>())
                {
                    go_CurrentWall.GetComponent<Cs_WallLogic>().SetVisibilityState(true);
                }
            }
        }
        else
        {

            // Turn off the last wall's transparency
            if (go_CurrentWall != null)
            {
                if (go_CurrentWall.GetComponent<Cs_WallLogic>())
                {
                    go_CurrentWall.GetComponent<Cs_WallLogic>().SetVisibilityState(false);
                }
            }

            // Set the current wall stored to null
            go_CurrentWall = null;
        }

    }
}
