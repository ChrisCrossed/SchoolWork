using UnityEngine;
using System.Collections;

public class Cs_RotArrow : MonoBehaviour
{
    void Start()
    {
        IsEnabled = false;
    }

    bool b_IsEnabled;
    public bool IsEnabled
    {
        set
        {
            b_IsEnabled = value;

            gameObject.GetComponent<MeshRenderer>().enabled = b_IsEnabled;
        }
        get { return b_IsEnabled; }
    }
    
    // Update is called once per frame
	void Update ()
    {
        Vector3 v3_Rot = gameObject.transform.eulerAngles;
        v3_Rot.z += Time.deltaTime * 135;
        gameObject.transform.eulerAngles = v3_Rot;
    }
}
