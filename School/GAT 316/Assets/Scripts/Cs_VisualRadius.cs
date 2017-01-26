using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cs_VisualRadius : MonoBehaviour
{
    [SerializeField] int i_NumVisualizePoints;

    List<GameObject> go_ArrayPoints = new List<GameObject>();

    float f_Radius = 0;

	// Use this for initialization
	void Start ()
    {
        // Create a series of empty game objects, add to a list
        for (int i_ = 0; i_ < i_NumVisualizePoints; ++i_)
        {
            GameObject go_Point = new GameObject();

            go_Point.name = "go_Point_" + i_;

            go_Point.transform.SetParent(transform.Find("PointsList"));

            go_ArrayPoints.Add(go_Point);
        }

        float f_Angle = 360f / i_NumVisualizePoints;

        // Go through the series of empty game objects & set their rotation based on the number of Visual Points
        for(int j_ = 0; j_ < go_ArrayPoints.Count; ++j_)
        {
            go_ArrayPoints[j_].transform.rotation = Quaternion.Euler(0, f_Angle * j_, 0);

            go_ArrayPoints[j_].transform.position = gameObject.transform.forward * f_Radius;
        }

        SetRadius();
	}
    float f_StepTimer = 0.5f;
    float f_RadiusTimer;
    void SetRadius()
    {
        float f_Magnitude = gameObject.GetComponent<Rigidbody>().velocity.magnitude;
        if (f_Magnitude >= 0.2f)
        {
            f_Radius = gameObject.GetComponent<Rigidbody>().velocity.magnitude * .5f;
        }
        else
        {
            f_Radius = 0;
        }

        // Adjust collider to match radius of movement
        float f_ColliderRadius = (transform.forward.z * f_Radius) / 2;
        if (f_ColliderRadius < 0.5f) f_ColliderRadius = 0.5f;
        gameObject.GetComponent<CapsuleCollider>().radius = f_ColliderRadius;

        f_RadiusTimer += Time.deltaTime * (f_Magnitude / 1f);

        if (f_RadiusTimer >= f_StepTimer && f_Magnitude != 0f)
        {
            // Cycle through all objects in list, moving their position based on the center of the game object * radius
            for(int i_ = 0; i_ < go_ArrayPoints.Count; ++i_)
            {
                Vector3 v3_yPos = go_ArrayPoints[i_].transform.position;
                v3_yPos = (go_ArrayPoints[i_].transform.forward * f_Radius) / 2;

                v3_yPos.y = -(gameObject.transform.lossyScale.y * 3 / 4);

                // v3_yPos.y += Random.Range(-.1f * f_Magnitude, .1f * f_Magnitude);
                v3_yPos.y += Random.Range(-(f_Magnitude / 1f), (f_Magnitude / 1f)) * 0.1f;
                go_ArrayPoints[i_].transform.position = v3_yPos;
            }

            f_RadiusTimer = 0f;
        }
        else
        {
            // Lerp the current yPosition of each point back to 0
            for (int i_ = 0; i_ < go_ArrayPoints.Count; ++i_)
            {
                Vector3 v3_yPos = go_ArrayPoints[i_].transform.position;
                v3_yPos.x = (go_ArrayPoints[i_].transform.forward.x * f_Radius) / 2;
                v3_yPos.z = (go_ArrayPoints[i_].transform.forward.z * f_Radius) / 2;

                v3_yPos.y = Mathf.Lerp(v3_yPos.y, -(gameObject.transform.lossyScale.y * 3 / 4), f_RadiusTimer / f_StepTimer);
                
                go_ArrayPoints[i_].transform.position = v3_yPos;
            }
        }

        SetLineRenderer();
    }

    void SetLineRenderer()
    {
        // Tell the line renderer how many positions will exist
        gameObject.GetComponent<LineRenderer>().SetVertexCount(go_ArrayPoints.Count + 1);

        for (int i_ = 0; i_ < go_ArrayPoints.Count + 1; ++i_)
        {
            //// Set the next point to connect to
            //int j_;

            //if (i_ != go_ArrayPoints.Count - 1) j_ = i_ + 1;
            //else j_ = 0;
            if(i_ != go_ArrayPoints.Count)
            {
                // Apply lines to each point in the line renderer
                gameObject.GetComponent<LineRenderer>().SetPosition(i_, go_ArrayPoints[i_].transform.position + gameObject.transform.position);
            }
            else
            {
                gameObject.GetComponent<LineRenderer>().SetPosition(i_, go_ArrayPoints[0].transform.position + gameObject.transform.position);
            }
        }
    }

    // Update is called once per frame
    Vector3 v3_PreviousLocation_Speed;
    Vector3 v3_CurrentLocation_Speed;
	void Update ()
    {
        v3_CurrentLocation_Speed = gameObject.transform.position;

        // Evaluates the distance between the player's current position and where they were last frame. Doesn't allow the visual to show otherwise we've moved.
        if(Vector3.Distance(v3_PreviousLocation_Speed, v3_CurrentLocation_Speed) > 0.01f)
        {
            SetRadius();
        }

        v3_PreviousLocation_Speed = v3_CurrentLocation_Speed;
	}
}
