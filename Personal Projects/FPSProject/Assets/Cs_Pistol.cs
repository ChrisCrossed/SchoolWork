using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cs_Pistol : Cs_WEAPON
{
    [SerializeField] int i_NumBulletsToFire = 1;
    [Range(0, 90)] [SerializeField] int i_PelletIntensity = 2;
    [Range(1, 1000)] [SerializeField] int i_PelletSpeed = 100;
    GameObject go_Spawn;

    [SerializeField] float f_AttackSpeed;

    // Use this for initialization
    void Start ()
    {
        base.Start();

        go_Spawn = transform.Find("Main Camera").Find("mdl_Pistol").Find("Bullet_CreatePoint").gameObject;

        SetWeaponModel = GameObject.Find("mdl_Pistol");

        AttackSpeed = f_AttackSpeed;

        GunIsAutomatic = false;

        // Determine player's maximum 'walk' movespeed
        f_NormalMoveSpeed = gameObject.GetComponent<Cs_PlayerController>().GetMaxWalkspeed;
    }

    public override void ShootGun()
    {
        for (int i_ = 0; i_ < i_NumBulletsToFire; ++i_)
        {
            GameObject go_Bullet = (GameObject)Instantiate(Resources.Load("Bullet_Shotgun"));
            go_Bullet.GetComponent<Cs_ShotgunPellet>().SetDirection(go_Spawn.transform.forward, i_PelletIntensity);
            go_Bullet.GetComponent<Cs_ShotgunPellet>().BulletSpeed = (float)i_PelletSpeed;
            go_Bullet.transform.position = go_Spawn.transform.position;
        }

        // Gun was fired. Slowly increase pellet intensity
        i_PelletIntensity += 2;
        if (i_PelletIntensity > 7) i_PelletIntensity = 7;
    }

    void DecreasePelletIntensity()
    {
        // Decrease pellet intensity based on timer while making sure the player isn't running
        if (i_PelletIntensity > 0 && gameObject.GetComponent<Rigidbody>().velocity.magnitude < f_NormalMoveSpeed)
        {
            f_PelletIntensityTimer += Time.deltaTime;

            if (f_PelletIntensityTimer > f_PelletIntensityTimer_MAX)
            {
                f_PelletIntensityTimer = 0f;

                --i_PelletIntensity;

                if (i_PelletIntensity < 0) i_PelletIntensity = 0;
            }
        }
    }

    float f_NormalMoveSpeed;
    float f_MoveSpeedTimer;
    void IncreasePelletIntensityBasedOnMovespeed()
    {
        if(gameObject.GetComponent<Rigidbody>().velocity.magnitude > f_NormalMoveSpeed)
        {
            f_MoveSpeedTimer += Time.deltaTime;
            if (f_MoveSpeedTimer > 1.0f)
            {
                ++i_PelletIntensity;
                if (i_PelletIntensity > 7) i_PelletIntensity = 7;

                f_MoveSpeedTimer = 0f;
            }
        }
    }

    float f_PelletIntensityTimer;
    float f_PelletIntensityTimer_MAX = 0.3f;
    void Update()
    {
        base.Update();

        IncreasePelletIntensityBasedOnMovespeed();
        DecreasePelletIntensity();
    }
}
