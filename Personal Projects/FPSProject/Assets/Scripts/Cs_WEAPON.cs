using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum Enum_WeaponState
{
    Active, // Weapon is ready to fire
    Reloading, // Weapon is adding a bullet to the clip
    Disabled, // Weapon is disabled and not ready
    Loading, // Weapon is mounting
    Unloading // Weapon is unmounting
}

public class Cs_WEAPON : MonoBehaviour
{

    Enum_WeaponState e_WeaponState = Enum_WeaponState.Disabled;
    GameObject go_WeaponModel;
    GameObject go_Camera;

	// Use this for initialization
	protected void Start ()
    {
        go_Camera = GameObject.Find("Main Camera");
	}

    protected GameObject SetWeaponModel
    {
        set { go_WeaponModel = value; }
    }
	
	// Update is called once per frame
	protected void Update ()
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
            if(e_WeaponState == Enum_WeaponState.Active)
            {
                if(b_CanShoot)
                {
                    ShootGun();

                    b_CanShoot = false;
                    f_FireTimer = f_FireTimer_Max;
                }
            }
        }

        Update_WeaponState();
    }
    
    bool b_IsActive;
    public bool WeaponState
    {
        get { return b_IsActive; }
        set
        {
            b_IsActive = value;
            Set_WeaponState(b_IsActive);
        }
    }

    void Set_WeaponState(bool b_IsActive_)
    {
        if(b_IsActive)
        {
            if (e_WeaponState == Enum_WeaponState.Disabled) e_WeaponState = Enum_WeaponState.Loading;
        }
        else
        {
            e_WeaponState = Enum_WeaponState.Unloading;
        }
    }

    float f_LoadingTimer;
    static float f_LoadingTimer_MAX = 0.5f;
    Vector3 v3_DisabledRot = new Vector3(0, 90, 30);
    void Update_WeaponState()
    {
        if(e_WeaponState == Enum_WeaponState.Loading)
        {
            f_LoadingTimer += Time.deltaTime;
            if (f_LoadingTimer > f_LoadingTimer_MAX)
            {
                f_LoadingTimer = f_LoadingTimer_MAX;

                e_WeaponState = Enum_WeaponState.Active;
            }
        }
        else if(e_WeaponState == Enum_WeaponState.Unloading)
        {
            f_LoadingTimer -= Time.deltaTime;
            if (f_LoadingTimer < 0f)
            {
                f_LoadingTimer = 0f;

                e_WeaponState = Enum_WeaponState.Disabled;
            }
        }

        float f_Perc = f_LoadingTimer / f_LoadingTimer_MAX;
        go_WeaponModel.transform.eulerAngles = Vector3.Lerp(v3_DisabledRot + gameObject.transform.eulerAngles, new Vector3(0, 90, 0) + go_Camera.transform.eulerAngles, f_Perc);
    }

    float f_FireTimer = 0.01f;
    float f_FireTimer_Max = 0.5f;
    bool b_CanShoot;
    public virtual void ShootGun()
    {
        print("BANG");
    }
}
