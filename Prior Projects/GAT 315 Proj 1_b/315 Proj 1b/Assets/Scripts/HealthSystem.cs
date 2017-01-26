using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum CharacterTypes
{
    Player, Turret, Objective, Boss
}

public class HealthSystem : MonoBehaviour
{
    // Health Values
    public int i_MaxHealth = 10;
    int i_CurrHealth;

    // Store what type of object this is
    public CharacterTypes charType = CharacterTypes.Turret;

    // Flash timers
    float f_FlashModelTimer;
    float f_Timer;
    float f_Percent;
    Color startColor;
    float f_CountdownClock = 120;
    float f_DamageClock;
    float f_DeathTimer;
    float f_HealTimer = 10.0f;

    bool b_IsActive = true;
    bool b_IsAlive = true;

    // SFX
    public AudioClip sfx_LaserHit;
    AudioSource audioSource;

    // Use this for initialization
    void Start ()
    {
        if(gameObject.GetComponent<MeshRenderer>())
        {
            startColor = gameObject.GetComponent<MeshRenderer>().material.color;
        }

        i_CurrHealth = i_MaxHealth;
        f_FlashModelTimer = 1;

        GameObject.Find("Text_Countdown").GetComponent<Text>().text = "";

        // Start the 'bleed' system at 0
        GameObject.Find("UI_Damage").GetComponent<Image>().color = new Color(1, 0, 0, 0);

        audioSource = GameObject.Find("Mech_Turret").GetComponent<Cs_MechTurretController>().audioSource;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if(charType == CharacterTypes.Boss) FlashModel();

        if (Input.GetKeyDown(KeyCode.P))
        {
            if(charType == CharacterTypes.Boss)
            {
                ApplyDamage(10);
            }
        }

        if(charType == CharacterTypes.Boss)
        {
            if(i_CurrHealth <= 0)
            {
                if(GameObject.Find("EscapeHatch"))
                {
                    GameObject.Find("EscapeHatch").GetComponent<Cs_DoorLogic>().OpenDoor();
                }

                // Run the Countdown Clock
                f_CountdownClock -= Time.deltaTime;

                if(f_CountdownClock <= 0f)
                {
                    f_CountdownClock = 0;

                    f_DamageClock += Time.deltaTime;
                    if(f_DamageClock >= 0.1f)
                    {
                        f_DamageClock = 0;

                        // Damage player by 5 points per second until they die
                        GameObject.Find("Mech").GetComponent<HealthSystem>().ApplyDamage(5);
                    }
                }

                GameObject.Find("Text_Countdown").GetComponent<Text>().text = "Time To Escape: " + f_CountdownClock.ToString("0.0");
            }
        }

        if (charType == CharacterTypes.Player)
        {
            // Heal player if enough time has passed
            if(f_HealTimer > 0) f_HealTimer -= Time.deltaTime;

            if (f_HealTimer <= 0 && i_CurrHealth < 100)
            {
                // Only applies healing once per second (60 seconds to full heal)
                f_HealTimer = 3;

                print("Heal 5 to: " + i_CurrHealth);
                ApplyDamage(-5);
            }

            // Lerp the UI_Damage object's visability toward current health amount
            // Find current alpha
            float f_CurrVisability = GameObject.Find("UI_Damage").GetComponent<Image>().color.a;

            // Lerp toward the percent of currHealth/maxHealth
            float newVisability = Mathf.Lerp(f_CurrVisability, 1 - ((float)i_CurrHealth / (float)i_MaxHealth), 0.1f);
            
            GameObject.Find("UI_Damage").GetComponent<Image>().color = new Color(1, 0, 0, newVisability);


            if (i_CurrHealth <= 0) b_IsAlive = false;

            if(!b_IsAlive)
            {
                // GameObject.Find("Canvas").SetActive(false);
                GameObject.Find("Mech").GetComponent<Cs_MechBaseController>().EndGame();

                // Begin dimming the lights
                f_DeathTimer += Time.deltaTime;
                var currColor = GameObject.Find("EndGame").GetComponent<MeshRenderer>().material.color;
                currColor.a = f_DeathTimer;
                GameObject.Find("EndGame").GetComponent<MeshRenderer>().material.color = currColor;

                if (f_DeathTimer >= 5.0f)
                {
                    SceneManager.LoadScene("Level_MainMenu");
                }
            }
        }
	}

    // Disable the object if it is an Objective
    public void SetObjectiveStatus(bool b_Status_)
    {
        if(charType == CharacterTypes.Objective)
        {
            b_IsActive = b_Status_;

            if (gameObject.GetComponent<Cs_DoorLogic>()) gameObject.GetComponent<Cs_DoorLogic>().OpenDoor();
        }

        if(charType == CharacterTypes.Turret)
        {
            gameObject.GetComponentInChildren<Cs_TurretAxel>().SetState(b_Status_);
            gameObject.GetComponentInChildren<Cs_TurretJoint>().SetTurretState(b_Status_);
        }
    }

    // Apply damage to this unit
    public void ApplyDamage(int i_DamageReceived_)
    {
        i_CurrHealth -= i_DamageReceived_;

        print(charType.ToString() + " has " + i_CurrHealth + " remaining");

        if(charType == CharacterTypes.Player && i_DamageReceived_ > 0)
        {
            f_HealTimer = 10f;
        }

        if(i_CurrHealth > 100)
        {
            i_CurrHealth = 100;
        }
    }

    void FlashModel()
    {
        if (b_IsAlive)
        {
            if (i_CurrHealth > 0)
            {
                // Keep counting upward to compare against
                if (f_FlashModelTimer <= 1) f_FlashModelTimer += Time.deltaTime;

                if (f_FlashModelTimer < 0.2f)
                {
                    Color currColor = gameObject.GetComponent<MeshRenderer>().material.color;
                    currColor.g = 1;
                    currColor.b = 1;
                    currColor.r = 0;
                    currColor.a = 1;
                    gameObject.GetComponent<MeshRenderer>().material.color = currColor;
                }
                else
                {
                    if(gameObject.GetComponent<MeshRenderer>())
                    {
                        gameObject.GetComponent<MeshRenderer>().material.color = startColor;

                        f_Timer += Time.deltaTime * 2;

                        // Sin waves between 0 & 1
                        f_Percent = (Mathf.Sin(f_Timer) / 2f) + 0.5f;

                        var currPos = gameObject.GetComponent<MeshRenderer>().material.color;
                        currPos.a = f_Percent;
                        gameObject.GetComponent<MeshRenderer>().material.color = currPos;
                    }

                }
            }
        }
    }

    void HealthCheckpoints()
    {
        if (i_CurrHealth > 30) print("Stage One");
        if(i_CurrHealth == 30)
        {
            print("Stage Two");
            GameObject.Find("Boss_Wall_1").GetComponent<Cs_BossWallTrigger>().SetState(true);
            GameObject.Find("Boss_Wall_2").GetComponent<Cs_BossWallTrigger>().SetState(true);
        }
        if(i_CurrHealth == 15 || i_CurrHealth == 5)
        {
            print("Stage Three");

            GameObject.Find("EnergyBox_Boss_2").GetComponent<Cs_EnergyBoxLogic>().TurnBoxOn();
            GameObject.Find("EnergyBox_Boss_1").GetComponent<Cs_EnergyBoxLogic>().TurnBoxOn();
        }
        if(i_CurrHealth <= 0)
        {
            print("Stage Four");
            GameObject.Find("EscapeHatch").GetComponent<Cs_DoorLogic>().OpenDoor();

            GameObject.Find("Boss_Wall_1").GetComponent<Cs_BossWallTrigger>().EndGame();
            GameObject.Find("Boss_Wall_2").GetComponent<Cs_BossWallTrigger>().EndGame();

            GameObject.Find("Text_Countdown").SetActive(true);

            // Enable Turrets
            GameObject.Find("EnergyBox_Test_0").GetComponent<Cs_EnergyBoxLogic>().TurnBoxOn();
            GameObject.Find("EnergyBox_Test_1").GetComponent<Cs_EnergyBoxLogic>().TurnBoxOn();
            GameObject.Find("EnergyBox_Test_2").GetComponent<Cs_EnergyBoxLogic>().TurnBoxOn();
            GameObject.Find("EnergyBox_Test_3").GetComponent<Cs_EnergyBoxLogic>().TurnBoxOn();
            GameObject.Find("EnergyBox_Turret_1").GetComponent<Cs_EnergyBoxLogic>().TurnBoxOn();
            GameObject.Find("EnergyBox_Inside_1").GetComponent<Cs_EnergyBoxLogic>().TurnBoxOn();
            GameObject.Find("EnergyBox_Inside_2").GetComponent<Cs_EnergyBoxLogic>().TurnBoxOn();
        }
    }

    void OnTriggerEnter(Collider collider_)
    {
        if(charType == CharacterTypes.Boss)
        {
            if (collider_.tag == "Laser")
            {
                audioSource.PlayOneShot(sfx_LaserHit);

                ApplyDamage(1);

                HealthCheckpoints();

                f_FlashModelTimer = 0;
            }
        }
    }
}
