﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cs_ShotgunPellet : Cs_BULLET
{
    int i_LayerMask_Enemy;
    int i_LayerMask_Wall;
    int i_LayerMask_Use;

    Collider this_Collider;

    GameObject go_Player;

    // Use this for initialization
    void Start ()
    {
        Initialize();

        i_LayerMask_Enemy = LayerMask.NameToLayer("Enemy");
        i_LayerMask_Wall = LayerMask.NameToLayer("Wall");
        i_LayerMask_Use = LayerMask.NameToLayer("Use");

        this_Collider = gameObject.GetComponent<Collider>();

        // Set player
        go_Player = GameObject.Find("Player");
    }

    public void SetDirection(Vector3 _forward, float _intensity = 5f )
    {
        Vector3 _direction = _forward + Random.onUnitSphere * (_intensity / 90);
        transform.LookAt(transform.position + _direction);
    }

    
    private void OnCollisionEnter(Collision collision_)
    {
        if(collision_.gameObject.layer == i_LayerMask_Enemy)
        {
            if(collision_.gameObject.GetComponent<Cs_Enemy_Test>())
            {
                // Connect with the enemy hit
                collision_.gameObject.GetComponent<Cs_Enemy_Test>().Hit();

                // Connect with the player to pass information
                go_Player.GetComponent<Cs_PlayerController>().Set_BulletHit = collision_.gameObject;
            }
        }

        // Disable collider after first collision
        this_Collider.enabled = false;
        GameObject.Destroy(gameObject);
    }
}
