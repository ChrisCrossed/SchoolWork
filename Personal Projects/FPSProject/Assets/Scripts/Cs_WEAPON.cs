using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cs_WEAPON : MonoBehaviour
{

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (f_FireTimer > 0f)
        {
            f_FireTimer -= Time.deltaTime;

            if (f_FireTimer < 0f)
            {
                f_FireTimer = 0f;

                b_CanShoot = true;
            }
        }

        if(Input.GetMouseButton(0))
        {
            if(b_CanShoot)
            {
                ShootGun();

                b_CanShoot = false;
                f_FireTimer = f_FireTimer_Max;
            }
        }
    }

    float f_FireTimer = 0.01f;
    float f_FireTimer_Max = 0.5f;
    bool b_CanShoot;
    public virtual void ShootGun()
    {
        print("BANG");
    }
}
