using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof(Cs_VisualDisplay))]
public class Cs_FieldOfView_Editor : Editor
{

    void OnSceneGUI()
    {
        Cs_VisualDisplay fov = (Cs_VisualDisplay)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward, 360, fov.f_ViewRadius);
        Vector3 v3_ViewAngle_A = fov.Get_DirFromAngle(-fov.f_ViewAngle / 2, false);
        Vector3 v3_ViewAngle_B = fov.Get_DirFromAngle( fov.f_ViewAngle / 2, false);

        Handles.DrawLine(fov.transform.position, fov.transform.position + v3_ViewAngle_A * fov.f_ViewRadius);
        Handles.DrawLine(fov.transform.position, fov.transform.position + v3_ViewAngle_B * fov.f_ViewRadius);
    }

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}
}
