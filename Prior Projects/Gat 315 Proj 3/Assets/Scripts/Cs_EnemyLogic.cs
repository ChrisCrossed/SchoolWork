using UnityEngine;
using System.Collections;

public class Cs_EnemyLogic : MonoBehaviour
{
    int i_Health = 3;
    float f_Speed = 3.0f;
    float f_DisabledTimer = 0f;
    int i_ColliderTimer;
    float f_LifeTimer;

	// Use this for initialization
	void Start ()
    {
        i_Health = 3;
        f_Speed = 3.0f;
        f_DisabledTimer = 0.0f;
	}

    void KillEnemy()
    {
        // Give money
        GameObject.Find("LevelController").GetComponent<Cs_LevelController>().ReceiveCurrency(150);

        // Destroy Object
        GameObject.Destroy(gameObject);
    }
	
	// Update is called once per frame
	void Update ()
    {
        if(f_DisabledTimer == 0)
        {
            // Rotate toward center of map
            GameObject go_Center = GameObject.Find("CenterObjective");
            Vector3 newPos = go_Center.transform.position;
            newPos.y = gameObject.transform.position.y;
            gameObject.transform.LookAt(newPos);

            // Move forward at set pace
            gameObject.GetComponent<Rigidbody>().velocity = transform.forward * f_Speed;
        }
        else
        {
            f_DisabledTimer -= Time.deltaTime;

            if (f_DisabledTimer <= 0)
            {
                f_DisabledTimer = 0f;
            }
        }

        f_LifeTimer += Time.deltaTime;
        if(f_LifeTimer >= 20f)
        {
            KillEnemy();
        }
    }

    void OnTriggerEnter(Collider collider_)
    {
        // If collide with tower, apply damage
        if(collider_.gameObject.tag == "Tower")
        {
            --i_Health;

            f_DisabledTimer = 1.0f;

            gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * -75f);

            collider_.gameObject.GetComponent<Cs_BaseColliderLogic>().ApplyDamage(1);

            if(i_Health <= 0)
            {
                KillEnemy();
            }
        }
        // If collide with bullet, take damage
    }

    void OnTriggerStay(Collider collider_)
    {
        if(collider_.gameObject.tag == "Tower")
        {
            ++i_ColliderTimer;

            if(i_ColliderTimer >= 10)
            {
                Vector3 newPos = gameObject.transform.position + (transform.forward * -5);
                gameObject.transform.position = newPos;
            }
        }
    }
}
