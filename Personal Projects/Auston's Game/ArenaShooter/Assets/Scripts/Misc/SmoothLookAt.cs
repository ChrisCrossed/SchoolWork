/*/////////////////////////////////////////////////////////////////////////
//SCRIPT - SmoothLookAt.cs
//AUTHOR - Auston Lindsay
//COPYRIGHT - © 2016 DigiPen Institute of Technology
/////////////////////////////////////////////////////////////////////////*/

using UnityEngine;
using System.Collections;

public class SmoothLookAt : MonoBehaviour
{
    [SerializeField]
    private Transform Target;
    [SerializeField]
    private Vector3 TargetLocalOffset; //The target's local, not our own
    private Vector3 FocalPoint;

    [SerializeField]
    private float RotationSpeed = 10;
    [SerializeField]
    private AnimationCurve RotationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    // Use this for initialization
    void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        this.FocalPoint = this.Target.position + this.Target.InverseTransformDirection(this.TargetLocalOffset);

        //Update Rotation
        Quaternion desiredRotation = Quaternion.LookRotation(this.FocalPoint - this.transform.position, this.Target.up);
        float angleToTarg = Quaternion.Angle(this.transform.rotation, desiredRotation);
        float evaluation = this.RotationCurve.Evaluate(1 - (angleToTarg / 180));
        this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, desiredRotation, this.RotationSpeed * 180 * evaluation * Time.deltaTime);

        //this.CamRod.transform.rotation = desiredRotation; //Ignore interpolation
    }
}
