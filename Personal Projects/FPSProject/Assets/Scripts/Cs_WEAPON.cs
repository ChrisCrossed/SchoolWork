using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Enum_WeaponState
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
    GameObject this_PlayerObject;
    Cs_PlayerController this_PlayerController;

	// Use this for initialization
	protected void Start ()
    {
        go_Camera = GameObject.Find("Main Camera");
        this_PlayerObject = GameObject.Find("Player");
        this_PlayerController = this_PlayerObject.GetComponent<Cs_PlayerController>();

        // Math - Weapon rotation (Points directly in-front of player)
        q_ForwardRot = Quaternion.Euler(new Vector3(90, 0, 0));
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

        if (Input.GetMouseButton(0) && b_IsAutomatic)
        {
            FireBullet();
        }
        else if (Input.GetMouseButtonDown(0)) FireBullet();

        Update_WeaponState();
    }

    void FireBullet()
    {
        if (e_WeaponState == Enum_WeaponState.Active)
        {
            if (b_CanShoot)
            {
                ShootGun();

                b_CanShoot = false;
                f_FireTimer = f_FireTimer_Max;
            }
        }
    }
    
    bool b_IsActive;
    public bool WeaponState
    {
        get { return b_IsActive; }
        set
        {
            // If the gun was off and now activated, enable the mesh renderer
            if (value && b_IsActive != value) go_WeaponModel.GetComponent<MeshRenderer>().enabled = true;

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

    public Enum_WeaponState GetWeaponState
    {
        get { return e_WeaponState; }
    }

    float f_LoadingTimer;
    static float f_LoadingTimer_MAX = 0.5f;
    Quaternion q_DisabledRot;
    Quaternion q_ForwardRot;
    void Update_WeaponState()
    {
        if(e_WeaponState == Enum_WeaponState.Loading)
        {
            f_LoadingTimer += Time.deltaTime;
            if (f_LoadingTimer >= f_LoadingTimer_MAX)
            {
                f_LoadingTimer = f_LoadingTimer_MAX;

                e_WeaponState = Enum_WeaponState.Active;
            }
        }
        else if(e_WeaponState == Enum_WeaponState.Unloading)
        {
            f_LoadingTimer -= Time.deltaTime;
            if (f_LoadingTimer <= 0f)
            {
                f_LoadingTimer = 0f;

                go_WeaponModel.GetComponent<MeshRenderer>().enabled = false;

                e_WeaponState = Enum_WeaponState.Disabled;
            }
        }

        // Determine rotation to look in-front of player at a slight downward angle in relation to the player's rotation
        // q_DisabledRot = Quaternion.Euler(new Vector3(0, 90, 30) + gameObject.transform.eulerAngles);
        q_DisabledRot = Quaternion.Euler(new Vector3(135, 0, 0) + gameObject.transform.eulerAngles);

        // Lerp to proper position
        float f_Perc = f_LoadingTimer / f_LoadingTimer_MAX;
        go_WeaponModel.transform.rotation = Quaternion.Lerp(q_DisabledRot, this_PlayerController.GetCameraRotation * q_ForwardRot, f_Perc);
    }

    protected float AttackSpeed
    {
        set { f_FireTimer_Max = value; }
    }

    bool b_IsAutomatic;
    protected bool GunIsAutomatic
    {
        set { b_IsAutomatic = value; }
    }

    float f_FireTimer = 0.01f;
    float f_FireTimer_Max = 0.5f;
    bool b_CanShoot;
    public virtual void ShootGun()
    {
        print("BANG");
    }
}
