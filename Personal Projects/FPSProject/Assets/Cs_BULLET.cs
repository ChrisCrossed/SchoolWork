using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cs_BULLET : MonoBehaviour
{
    Rigidbody this_Rigidbody;
    Vector3 v3_StartRotation;
    LineRenderer this_LineRenderer;

    float f_Speed = 10f;
    float f_Rotation = 0f;
    float f_Slope = 0f;

    float f_Timer;

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

    void Update()
    {
        // Add point to LineRenderer
        if (this_LineRenderer.numPositions < i_NumPoints)
        {
            this_LineRenderer.numPositions += 1;
            this_LineRenderer.SetPosition(this_LineRenderer.numPositions - 1, gameObject.transform.position);

            for(int i_ = 0; i_ < this_LineRenderer.numPositions; ++i_)
            {
                if( this_LineRenderer.GetPosition(i_) == Vector3.zero ) this_LineRenderer.SetPosition( i_, gameObject.transform.position );
            }
        }
        else // Create a brand new line comprised of all previous points except the first
        {
            Vector3[] v3_Positions = new Vector3[i_NumPoints];
            for (int i_ = 1; i_ < i_NumPoints; ++i_)
            {
                v3_Positions[i_ - 1] = this_LineRenderer.GetPosition(i_);

                // Fixes visual bug that sets a point at 0,0,0
                if (v3_Positions[i_ - 1] == Vector3.zero) v3_Positions[i_ - 1] = gameObject.transform.position;
            }

            this_LineRenderer.SetPositions(v3_Positions);
        }
    }

    // Update is called once per frame
    bool b_IsStationary;
    int i_NumPoints = 7;
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
