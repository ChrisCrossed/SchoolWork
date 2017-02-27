﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cs_Shotgun : Cs_WEAPON
{
    [SerializeField] int i_NumBulletsToFire = 4;
    [Range(0, 90)] [SerializeField] int i_PelletIntensity = 2;
    [Range(1, 1000)] [SerializeField] int i_PelletSpeed = 100;
    GameObject go_Spawn;

    void Start()
    {
        go_Spawn = transform.Find("Main Camera").Find("mdl_Shotgun").Find("Bullet_CreatePoint").gameObject;
    }

    public override void ShootGun()
    {
        for(int i_ = 0; i_ < i_NumBulletsToFire; ++i_)
        {
            GameObject go_Bullet = (GameObject) Instantiate( Resources.Load("Bullet_Shotgun") );
            go_Bullet.GetComponent<Cs_ShotgunPellet>().SetDirection( go_Spawn.transform.forward, i_PelletIntensity );
            go_Bullet.GetComponent<Cs_ShotgunPellet>().BulletSpeed = (float)i_PelletSpeed;
            go_Bullet.transform.position = go_Spawn.transform.position;
        }
    }
}