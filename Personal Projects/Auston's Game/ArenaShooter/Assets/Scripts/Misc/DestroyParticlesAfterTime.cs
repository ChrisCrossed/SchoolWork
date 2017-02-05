using UnityEngine;
using System.Collections;

public class DestroyParticlesAfterTime : MonoBehaviour
{
    private new ParticleSystem particleSystem;
    private float startingEmissionRate;
    public float lifetime = 5;
    private float timeAlive;
    public AnimationCurve emissionCurve = AnimationCurve.Linear(0, 1, 1, 0);

	// Use this for initialization
	void Start ()
    {   
        particleSystem = GetComponent<ParticleSystem>();
        startingEmissionRate = particleSystem.emission.rate.constant;
    }
	
	// Update is called once per frame
	void Update ()
    {
        timeAlive += Time.deltaTime;
        if (timeAlive >= lifetime + particleSystem.startLifetime)
            Destroy(gameObject);
        
        ParticleSystem.EmissionModule _emission = particleSystem.emission;
        ParticleSystem.MinMaxCurve _rate = _emission.rate;
        _rate.constant = startingEmissionRate * emissionCurve.Evaluate(timeAlive / lifetime);
        _emission.rate = _rate;
    }
}
