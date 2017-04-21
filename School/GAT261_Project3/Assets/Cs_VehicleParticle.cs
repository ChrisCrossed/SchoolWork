using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cs_VehicleParticle : MonoBehaviour
{
    [SerializeField] GameObject go_Particle;

    // Use this for initialization
	void Start ()
    {
        Particles = new List<GameObject>();

        lineRenderer = gameObject.GetComponent<LineRenderer>();
    }

    List<GameObject> Particles;
    void CreateParticle()
    {
        if(Particles.Count > 15)
        {
            Particles.RemoveAt(0);
        }

        GameObject go_Temp = Instantiate(go_Particle);
        go_Temp.transform.position = gameObject.transform.position;
        go_Temp.transform.eulerAngles = new Vector3(0, 180, 0);

        Particles.Add(go_Temp);
    }
    
    LineRenderer lineRenderer;
    void SetLineRenderer()
    {
        lineRenderer.SetPosition(0, gameObject.transform.position);
        if(Particles.Count > 0)
        {
            lineRenderer.positionCount = Particles.Count;

            for(int i_ = Particles.Count - 1; i_ > 0; --i_)
            {
                lineRenderer.SetPosition(Particles.Count - i_, Particles[i_].transform.position);
            }
        }
    }

    // Update is called once per frame
    float f_ParticleTimer;
	void Update ()
    {
        f_ParticleTimer += Time.deltaTime;
        if(f_ParticleTimer > 0.05f)
        {
            f_ParticleTimer = 0f;
            CreateParticle();
        }

        SetLineRenderer();
    }
}
