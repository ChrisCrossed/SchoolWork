using UnityEngine;
using System.Collections;

public class Cs_TurretBullet : MonoBehaviour
{
    public float f_Speed = 5;
    public int i_Damage = 5;

    // SFX
    public AudioClip sfx_BulletFired;
    AudioSource audioSource;

    // Use this for initialization
    void Start ()
    {
        audioSource = GetComponent<AudioSource>();

        audioSource.PlayOneShot(sfx_BulletFired, 0.5f);
	}
	
	// Update is called once per frame
	void Update ()
    {
        // if (audioSource == null) audioSource = GameObject.Find("Mech_Turret").GetComponent<Cs_MechTurretController>().audioSource;

        // Stupid update
        gameObject.GetComponent<Rigidbody>().velocity = transform.forward * f_Speed;
	}

    void OnTriggerEnter(Collider collider_)
    {
        if (collider_.gameObject.GetComponent<HealthSystem>())
        {
            collider_.gameObject.GetComponent<HealthSystem>().ApplyDamage(i_Damage);
        }

        GameObject.Destroy(gameObject);
    }
}
