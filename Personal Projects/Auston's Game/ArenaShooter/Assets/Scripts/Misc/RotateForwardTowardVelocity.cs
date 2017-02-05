/*/////////////////////////////////////////////////////////////////////////
//SCRIPT - RotateForwardTowardVelocity.cs
//AUTHOR - Auston Lindsay
//COPYRIGHT - © 2016 DigiPen Institute of Technology
/////////////////////////////////////////////////////////////////////////*/

using UnityEngine;
using System.Collections;

public class RotateForwardTowardVelocity : MonoBehaviour
{
    [SerializeField]
    private float MaxRotation = 20;
    [SerializeField]
    private float Speed = 10;
    [SerializeField]
    private float MaxVelocityMagnitude = 3;

    // Use this for initialization
    void Start ()
    {
	    
	}
	
	// Update is called once per frame
	void Update ()
    {
        Rigidbody rootRigidbody = this.transform.root.GetComponent<Rigidbody>();
        if (rootRigidbody == null)
            return;
        
        float rootSpeed = rootRigidbody.velocity.magnitude;

        //Vector3 desiredUp = Vector3.RotateTowards(this.transform.root.up, rootRigidbody.velocity / rootSpeed, Mathf.Clamp01(rootSpeed / this.MaxVelocityMagnitude) * Mathf.Deg2Rad * this.MaxRotation, 0);
        //Quaternion desiredRotation = Quaternion.LookRotation(Vector3.Cross(this.transform.root.right, desiredUp), desiredUp);
        //this.transform.rotation = Quaternion.Lerp(this.transform.rotation, desiredRotation, Time.deltaTime * this.Speed);

        float maxRadiansDelta = Mathf.Clamp01(rootSpeed / this.MaxVelocityMagnitude) * Mathf.Deg2Rad * this.MaxRotation;
        Vector3 desiredForward = Vector3.RotateTowards(this.transform.root.forward, rootRigidbody.velocity / rootSpeed, maxRadiansDelta, 0);
        Quaternion desiredRotation = Quaternion.LookRotation(desiredForward, Vector3.Cross(this.transform.root.right, desiredForward));
        this.transform.rotation = Quaternion.Lerp(this.transform.rotation, desiredRotation, Time.deltaTime * this.Speed);
	}
}
