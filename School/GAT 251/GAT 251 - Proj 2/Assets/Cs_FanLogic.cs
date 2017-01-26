using UnityEngine;
using System.Collections;

public class Cs_FanLogic : MonoBehaviour
{
    [SerializeField] float f_RotateSpeed = 5.0f;
    [SerializeField] bool b_RotateClockwise = true;
	
	// Update is called once per frame
	void Update ()
    {
        Vector3 v3_Rotation = gameObject.transform.eulerAngles;

        if( b_RotateClockwise )
        {
            v3_Rotation.z = Mathf.LerpAngle(v3_Rotation.z, v3_Rotation.z - f_RotateSpeed, Time.deltaTime);
        }
        else
        {
            v3_Rotation.z = Mathf.LerpAngle(v3_Rotation.z, v3_Rotation.z + f_RotateSpeed, Time.deltaTime);
        }

        gameObject.transform.eulerAngles = v3_Rotation;
	}
}
