using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cs_Grenade : MonoBehaviour
{
    float f_Timer = 5.0f;
    GameObject go_Player;
    [SerializeField] float f_GrenadeStrength = 50f;
    bool b_Active = true;
    bool b_Thrown = false;

    Rigidbody this_Rigidbody;

	// Use this for initialization
	void Start ()
    {
        go_Player = GameObject.Find("Player");
        this_Rigidbody = gameObject.GetComponent<Rigidbody>();

        // Vector3 v3_TestVector = gameObject.transform.forward + gameObject.transform.up;
        // v3_TestVector.Normalize();

        // Initialize( 5.0f, v3_TestVector, 10f );
	}

    public void Initialize( float f_RemainingTimer_ )
    {
        // Set timer
        f_Timer = f_RemainingTimer_;

        b_Active = true;
        b_Thrown = false;
    }

    public void ThrowGrenade( Vector3 v3_ThrownVector_, float f_Magnitude_ )
    {
        b_Active = true;
        b_Thrown = true;

        // Apply initial velocity
        this_Rigidbody.velocity = v3_ThrownVector_ * f_Magnitude_;
    }

    void Detonate()
    {
        // Gather distance between grenade and player
        float f_Distance = Vector3.Distance(go_Player.transform.position, gameObject.transform.position);

        // Determine grenade strength
        if (f_Distance < 10.0f)
        {
            // Determine vector between grenade and player
            Vector3 v3_Vector = go_Player.transform.position - gameObject.transform.position;
            v3_Vector.Normalize();

            // Piecewise function.
            // If the player is up to 5.0 units away, 1.0f
            // Else, Fall off between 1.0 and 0.0.
            float f_Percent = 1.0f;
            if (f_Distance <= 10f && f_Distance > 5.0f)
            {
                f_Distance -= 5f;
                f_Distance /= 5.0f;

                f_Percent = 1.0f - f_Distance;
            }

            // f_Percent *= f_GrenadeStrength;
            f_GrenadeStrength *= f_Percent;

            // Call to player
            go_Player.GetComponent<Cs_PlayerController>().ApplyGrenadeBounce(v3_Vector, f_GrenadeStrength);
        }

        // Disable
        b_Active = false;

        // GameObject.Destroy(gameObject);
    }

    // Update is called once per frame
    void Update ()
    {
        print(5.0f - f_Timer);
        f_Timer -= Time.deltaTime;

        if(!b_Thrown)
        {
            this_Rigidbody.MovePosition( go_Player.transform.position );
            
            if (f_Timer < 0.1f)
            {
                ThrowGrenade(Vector3.down, 1.0f);
            }
        }
        else if(b_Active)
        {
            if(f_Timer < 0.0f)
            {
                Detonate();
            }
        }
        else if(!b_Active && f_Timer < 0f)
        {
            if (f_Timer < -3.0f) GameObject.Destroy(this);
        }
	}
}
