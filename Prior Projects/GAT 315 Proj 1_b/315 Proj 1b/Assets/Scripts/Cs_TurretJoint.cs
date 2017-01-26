using UnityEngine;
using System.Collections;

public class Cs_TurretJoint : MonoBehaviour
{
    bool b_TurretEnabled = true;

    // Number of attacks
    uint ui_NumShotsFired = 0;
    public uint ui_ShotsBeforeReload = 3;

    // Timers
    float f_FireTimer;
    public float f_TimeToFire = 3;
    public float f_TimeBetweenShots = 0.2f;

    // Objects to aim at
    GameObject player;
    public GameObject NonPlayerAttackPoint;
    public bool b_AttackOnlyPlayer;

    // Gun objects
    GameObject gunObj;
    public GameObject bulletObj;

	// Use this for initialization
	void Start ()
    {
        player = GameObject.Find("Mech");
        gunObj = transform.FindChild("Turret_Model").FindChild("Turret_Gun").gameObject;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(b_TurretEnabled)
        {
            // Look at player
            Vector3 playerPos = player.transform.position;
            playerPos.y = 1;
            transform.LookAt(playerPos);

            gunObj.transform.LookAt(playerPos);
            Debug.DrawRay(gunObj.transform.position, gunObj.transform.forward);
        
            Ray ray = new Ray(gunObj.transform.position, gunObj.transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Debug.DrawLine(gunObj.transform.position, hit.point);

                // print("Hit: " + hit.collider.name);
                if (hit.collider.gameObject == player.gameObject)
                {
                    AttackPlayerTimer();
                }
                else
                {
                    f_FireTimer = 0;
                }
            }
        }
    }

    void AttackPlayerTimer()
    {
        f_FireTimer += Time.deltaTime;

        // If we reached the number of shots
        if(f_FireTimer >= f_TimeToFire)
        {
            // If we fired the max number of shots in one 'clip', then 'reload'
            ++ui_NumShotsFired;

            if(ui_NumShotsFired < ui_ShotsBeforeReload)
            {
                // Reset the fire timer between bullets
                f_FireTimer = f_TimeToFire - f_TimeBetweenShots;
            }
            else
            {
                // Reset the timer
                f_FireTimer = 0;
                ui_NumShotsFired = 0;
            }

            // Fire one bullet
            FireBullet();
        }
    }

    void FireBullet()
    {
        // Create a bullet at the gun's position & rotation
        GameObject.Instantiate(bulletObj, gunObj.transform.position, gunObj.transform.rotation);
    }

    public void SetTurretState(bool b_TurretState_)
    {
        // Reset the bullet timer if we're de-activating
        if (!b_TurretState_) f_FireTimer = 0;

        // Set the turret state
        b_TurretEnabled = b_TurretState_;
    }
}
