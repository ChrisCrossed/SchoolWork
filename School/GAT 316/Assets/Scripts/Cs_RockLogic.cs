using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cs_RockLogic : MonoBehaviour
{
    float f_Magnitude;
    float f_Timer;
    float f_LiveTimer;
    bool b_IsDead;
    bool b_HasMadeSound;
    float f_HitTimer;

    // Visualizer
    int i_NumVisualizePoints = 20;
    List<GameObject> go_ArrayPoints = new List<GameObject>();
    float f_Radius = 0;

    // Audio
    AudioSource as_AudioSource;
    AudioClip ac_Thud;

    void Start()
    {
        as_AudioSource = gameObject.GetComponent<AudioSource>();
        ac_Thud = Resources.Load("SFX_Thud") as AudioClip;

        #region Visualizer Logic
        // Create a series of empty game objects, add to a list
        for (int i_ = 0; i_ < i_NumVisualizePoints; ++i_)
        {
            GameObject go_Point = new GameObject();

            go_Point.name = "go_Point_" + i_;

            go_Point.transform.SetParent(gameObject.transform);

            go_ArrayPoints.Add(go_Point);
        }

        Update_VisualizerRotation();
        #endregion

        SetLineRenderer();
    }

    void Update_VisualizerRotation()
    {
        float f_Angle = 360f / i_NumVisualizePoints;

        // Go through the series of empty game objects & set their rotation based on the number of Visual Points
        for (int j_ = 0; j_ < go_ArrayPoints.Count; ++j_)
        {
            go_ArrayPoints[j_].transform.rotation = Quaternion.Euler(0, f_Angle * j_, 0);

            go_ArrayPoints[j_].transform.position = gameObject.transform.position;
        }
    }

    // Update is called once per frame
    void Update ()
    {
        gameObject.transform.Find("Sound_Collider").transform.rotation = Quaternion.Euler(transform.up);

        Update_VisualizerRotation();

        // if(gameObject.GetComponent<MeshCollider>().isTrigger)
        if (!b_HasMadeSound)
        {
            UpdateRaycast();
        }

        if(!b_IsDead)
        {
            CheckMagnitude();
        }
        else
        {
            FadeToOblivion();
        }

        if(b_RunVisual)
        {
            UpdateVisualizer();
        }

        UpdateLiveTimer();

        if (f_HitTimer > 0) f_HitTimer += Time.deltaTime;
	}

    bool b_RunVisual = false;
    public void Run_Visualizer()
    {
        b_RunVisual = true;
    }

    float f_VisualizerTimer;
    void UpdateVisualizer()
    {
        f_VisualizerTimer += Time.deltaTime;

        if (f_VisualizerTimer > 0.25f)
        {
            f_VisualizerTimer = 0f;

            b_RunVisual = false;
        }

        for (int i_ = 0; i_ < go_ArrayPoints.Count; ++i_)
        {
            Vector3 v3_yPos = transform.Find("Sound_Collider").transform.position;

            v3_yPos = (go_ArrayPoints[i_].transform.forward * f_VisualizerTimer * 5);

            go_ArrayPoints[i_].transform.position = v3_yPos;
        }

        SetLineRenderer();
    }

    void UpdateRaycast()
    {
        RaycastHit hit;

        int layer_mask = LayerMask.GetMask("Ground", "Wall");

        Vector3 v3_VelocityNormalized = gameObject.GetComponent<Rigidbody>().velocity.normalized;

        Debug.DrawRay(gameObject.transform.position, v3_VelocityNormalized, Color.red, Time.deltaTime);

        if(Physics.Raycast(gameObject.transform.position, Vector3.down, out hit, 0.2f, layer_mask))
        {
            // Tell 'sound collider' to inform enemies (Occurs as a trigger BEFORE making the change to 'IsTrigger = true'
            if (!b_HasMadeSound)
            {
                float f_Pitch = Random.Range(0.8f, 1.21f);
                as_AudioSource.pitch = f_Pitch;
                as_AudioSource.PlayOneShot(ac_Thud);

                gameObject.transform.Find("Sound_Collider").GetComponent<Cs_RockSoundLogic>().MakeSound();

                f_HitTimer += Time.deltaTime;

                b_HasMadeSound = true;
            }

            // gameObject.GetComponent<Collider>().isTrigger = false;
            gameObject.GetComponent<Collider>().isTrigger = false;
        }
        else if (Physics.Raycast(gameObject.transform.position, v3_VelocityNormalized, out hit, 0.2f, layer_mask))
        {
            // Tell 'sound collider' to inform enemies (Occurs as a trigger BEFORE making the change to 'IsTrigger = true'
            if (!b_HasMadeSound)
            {
                float f_Pitch = Random.Range(0.8f, 1.21f);
                as_AudioSource.pitch = f_Pitch;
                as_AudioSource.PlayOneShot(ac_Thud);

                gameObject.transform.Find("Sound_Collider").GetComponent<Cs_RockSoundLogic>().MakeSound();

                f_HitTimer += Time.deltaTime;

                b_HasMadeSound = true;
            }

            // gameObject.GetComponent<Collider>().isTrigger = false;
            gameObject.GetComponent<Collider>().isTrigger = false;
        }
        else
        {
            // gameObject.GetComponent<Collider>().isTrigger = true;
            gameObject.GetComponent<Collider>().isTrigger = true;
        }
    }

    void CheckMagnitude()
    {
        f_Magnitude = gameObject.GetComponent<Rigidbody>().velocity.magnitude;

        if (f_Magnitude >= 0.5f)
        {
            f_Timer = 0f;
        }
        else
        {
            f_Timer += Time.deltaTime;

            if (f_Timer >= 3.0f)
            {
                b_IsDead = true;
            }
        }

        Vector3 v3_OldVelocity = gameObject.GetComponent<Rigidbody>().velocity;
        v3_OldVelocity.y -= (Time.deltaTime);
        gameObject.GetComponent<Rigidbody>().velocity = v3_OldVelocity;
    }

    void FadeToOblivion()
    {
        Color c_CurrColor = gameObject.GetComponent<MeshRenderer>().material.color;
        c_CurrColor.a -= Time.deltaTime;

        if(c_CurrColor.a <= 0.0f)
        {
            GameObject.Destroy(gameObject);
        }
        else
        {
            gameObject.GetComponent<MeshRenderer>().material.color = c_CurrColor;
        }
    }

    void UpdateLiveTimer()
    {
        f_LiveTimer += Time.deltaTime;

        if (f_LiveTimer >= 10.0f)
        {
            b_IsDead = true;
        }
    }

    void SetLineRenderer()
    {
        // Tell the line renderer how many positions will exist
        gameObject.GetComponent<LineRenderer>().SetVertexCount(go_ArrayPoints.Count + 1);

        for (int i_ = 0; i_ < go_ArrayPoints.Count + 1; ++i_)
        {
            if (i_ != go_ArrayPoints.Count)
            {
                // Apply lines to each point in the line renderer
                gameObject.GetComponent<LineRenderer>().SetPosition(i_, go_ArrayPoints[i_].transform.position + gameObject.transform.position);
            }
            else
            {
                gameObject.GetComponent<LineRenderer>().SetPosition(i_, go_ArrayPoints[0].transform.position + gameObject.transform.position);
            }
        }
    }

    void OnCollisionEnter( Collision collision_ )
    {
        if(f_HitTimer >= 1f)
        {
            gameObject.transform.Find("Sound_Collider").GetComponent<Cs_RockSoundLogic>().MakeSound();
        }
    }
}
