using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cs_BULLET : MonoBehaviour
{
    GameObject go_Player;
    Rigidbody this_Rigidbody;
    Vector3 v3_StartRotation;
    LineRenderer this_LineRenderer;

    float f_Speed = 10f;
    float f_Rotation = 0f;
    float f_Slope = 0f;

    float f_Timer;

    private void Awake()
    {
        go_BulletCreationPoint = GameObject.Find("Bullet_CreatePoint");
        go_Player = GameObject.Find("Player");
    }

    // Use this for initialization
    protected void Initialize ()
    {
        this_Rigidbody = gameObject.GetComponent<Rigidbody>();
        v3_StartRotation = gameObject.transform.eulerAngles;
        this_LineRenderer = gameObject.GetComponent<LineRenderer>();

        this_LineRenderer.SetPosition(0, gameObject.transform.position);
    }
    
    public float BulletSpeed
    {
        set { f_Speed = value; }
        get { return f_Speed; }
    }

    float f_TrailDistance = 1.0f;
    void Update()
    {

    }

    protected void RaycastBullet( Vector3 v3_Pos_, Vector3 v3_Dir_ )
    {
        RaycastHit hit_;
        int i_LayerMask_ = LayerMask.GetMask("Enemy", "Default", "Ground", "Use", "Wall");
        
        if( Physics.Raycast( BulletCreationPoint(), v3_Dir_, out hit_, Mathf.Infinity, i_LayerMask_) )
        {
            // Ensure the bullet hit an enemy
            if (hit_.collider.gameObject.layer != LayerMask.NameToLayer("Enemy")) return;

            // Enemy type '0'
            if (hit_.collider.gameObject.GetComponent<Cs_Enemy_Test>())
            {
                // Connect with the enemy hit
                hit_.collider.gameObject.GetComponent<Cs_Enemy_Test>().Hit();
            }
            // Basic enemy
            else if (hit_.collider.gameObject.GetComponent<Cs_Enemy_Basic>())
            {
                // Connect with the enemy hit
                hit_.collider.gameObject.GetComponent<Cs_Enemy_Basic>().Hit();
            }

            // Connect with the player to pass information
            go_Player.GetComponent<Cs_PlayerController>().Set_BulletHit = hit_.collider.gameObject;
        }
    }

    GameObject go_BulletCreationPoint;
    protected Vector3 BulletCreationPoint()
    {
        return go_BulletCreationPoint.transform.position;
    }

    // Update is called once per frame
    bool b_IsStationary;
	void FixedUpdate ()
    {
        f_Timer += Time.fixedDeltaTime;

        if (f_Timer > 1.0f)
        {
            DestroyBullet();
        }

        if(!b_IsStationary)
        {
            this_Rigidbody.velocity = gameObject.transform.forward * f_Speed;
        }
        else
        {
            this_Rigidbody.MovePosition(v3_FinalPosition);
        }
    }

    void DestroyBullet()
    {
        if(f_Timer < 1.0f)
        {
            this_Rigidbody.MovePosition(v3_FinalPosition);
        }
        else
        {
            GameObject.Destroy(gameObject);
        }
    }

    Vector3 v3_FinalPosition;
    private void OnCollisionEnter(Collision collision_)
    {
        int i_LayerMask = LayerMask.GetMask("Player_Bullet", "Player_Gun");

        if(collision_.gameObject.layer != i_LayerMask)
        {
            v3_FinalPosition = gameObject.transform.position;
            DestroyBullet();
            b_IsStationary = true;
        }
    }
}
