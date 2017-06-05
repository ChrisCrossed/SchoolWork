using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cs_ENEMY : MonoBehaviour
{
    Color this_Color;
    Material this_Material;

	// Use this for initialization
	protected void Initialize()
    {
        this_Color = gameObject.GetComponent<MeshRenderer>().material.color;
        this_Material = gameObject.GetComponent<MeshRenderer>().material;
    }

    public void Hit()
    {
        this_Material.color = Color.red;
        f_HitTimer = 0.0f;
    }

    float f_HitTimer = 0.25f;
    float f_HitTimer_Max = 0.25f;
	// Update is called once per frame
	protected void SkinUpdate ()
    {
        if(f_HitTimer < f_HitTimer_Max)
        {
            f_HitTimer += Time.deltaTime;

            if(f_HitTimer > f_HitTimer_Max)
            {
                this_Material.color = Color.white;

                f_HitTimer = f_HitTimer_Max;
            }
        }
	}
}
