using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum Enum_GrenadeThrowState
{
    Default,
    CookGrenade,
    ThrowGrenade
}
public class Cs_ThrowGrenade : MonoBehaviour
{
    float f_ThrowMagnitude = 10f;
    Enum_GrenadeThrowState e_ThrowState = Enum_GrenadeThrowState.Default;
    float f_CookTimer;
    static float f_CookTimer_Max = 5.0f;
    [SerializeField] GameObject go_Grenade;

	// Use this for initialization
	void Start ()
    {
        f_CookTimer = f_CookTimer_Max;
	}

    GameObject go_Grenade_;
    public void GrenadeButtonPressed()
    {
        if( e_ThrowState == Enum_GrenadeThrowState.Default )
        {
            // Create grenade
            go_Grenade_ = Instantiate( go_Grenade );

            // Begin cooking grenade
            go_Grenade_.GetComponent<Cs_Grenade>().Initialize( f_CookTimer );

            // Change state
            e_ThrowState = Enum_GrenadeThrowState.CookGrenade;
        }
        else if( e_ThrowState == Enum_GrenadeThrowState.CookGrenade )
        {
            // Enable the mesh renderer
            go_Grenade_.GetComponent<MeshRenderer>().enabled = true;

            // Throw grenade
            Vector3 v3_ThrownVector = Vector3.forward + Vector3.up;
            v3_ThrownVector.Normalize();
            v3_ThrownVector = gameObject.transform.rotation * v3_ThrownVector;
            go_Grenade_.GetComponent<Cs_Grenade>().ThrowGrenade( v3_ThrownVector, 10f );

            // Empty the grenade object
            go_Grenade_ = null;

            // Change state
            e_ThrowState = Enum_GrenadeThrowState.Default;
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
	    
	}
}
