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

    float f_TrailDistance = 1.0f;
    void Update()
    {
        /*
        // Add point to LineRenderer
        this_LineRenderer.numPositions++;
        this_LineRenderer.SetPosition(this_LineRenderer.numPositions - 1, gameObject.transform.position);

        Vector3[] v3_Positions = new Vector3[this_LineRenderer.numPositions - 1];
        for (int i_ = 0; i_ < this_LineRenderer.numPositions - 1; ++i_)
        {
            v3_Positions[i_] = this_LineRenderer.GetPosition(i_);
        }

        if( v3_Positions.Length > 3 )
        {
            print( Vector3.Distance( v3_Positions[0], v3_Positions[v3_Positions.Length - 1] ) + ", Length: " + v3_Positions.Length);

            while ( Vector3.Distance (v3_Positions[0], v3_Positions[v3_Positions.Length - 1] ) > f_TrailDistance )
            {
                // Create new array that's one smaller than before
                Vector3[] v3_NewArray = new Vector3[ v3_Positions.Length - 2 ];

                // Populate
                for( int i_ = 0; i_ < v3_Positions.Length - 2; ++i_ )
                {
                    v3_NewArray[i_] = v3_Positions[i_ + 1];
                }

                v3_Positions = new Vector3[v3_NewArray.Length - 1];
                for(int j_ = 0; j_ < v3_Positions.Length; ++j_)
                {
                    v3_Positions[j_] = v3_NewArray[j_];
                }

            }
        }

        this_LineRenderer.SetPositions(v3_Positions);
        */
        /*

        if( v3_Positions.Length > 2)
        {
            print("Length: " + v3_Positions.Length);
            while ( Vector3.Distance( v3_Positions[0], v3_Positions[v3_Positions.Length - 1] ) > f_TrailDistance )
            {
                // Create new Vector3[] with one fewer positions in the array
                Vector3[] v3_NewArray = new Vector3[v3_Positions.Length - 2];

                // Populate
                for(int i_ = 1; i_ < v3_NewArray.Length; ++i_)
                {
                    v3_NewArray[i_ - 1] = v3_Positions[i_];
                }

                v3_Positions = v3_NewArray;
            }
        }

        */
        

        /*
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
        */
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
