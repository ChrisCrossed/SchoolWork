using UnityEngine;
using System.Collections;

public class Cs_ArrowModelLogic : MonoBehaviour
{
    float f_ArrowLerp;
    GameObject go_Arrow;

    float f_ColorLerp;

	// Use this for initialization
	void Start ()
    {
        go_Arrow = transform.Find("V").gameObject;
	}
	
	// Update is called once per frame
	void Update ()
    {
        f_ArrowLerp += Time.deltaTime * 3f;

        float f_ArrowLerpPosition = (Mathf.Sin(f_ArrowLerp) / 2f) - 0.25f;

        Vector3 v3_NewPosition = go_Arrow.transform.localPosition;
        v3_NewPosition.z = f_ArrowLerpPosition;
        go_Arrow.transform.localPosition = v3_NewPosition;
	}
}
