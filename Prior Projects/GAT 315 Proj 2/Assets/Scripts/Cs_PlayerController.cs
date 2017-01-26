using UnityEngine;
using System.Collections;
using XInputDotNetPure; // Controller input
using UnityEngine.UI;
using UnityEngine.SceneManagement;

enum CurrentGear
{
    Zero    = 1,
    One     = 2,
    Two     = 3,
    Three   = 4,
    Four    = 5,
    Off     = -1
}

public class Cs_PlayerController : MonoBehaviour
{
    GamePadState state;
    GamePadState prevState;
    public PlayerIndex playerIndex = PlayerIndex.One;

    bool b_DriveMode = true;

    float f_CurrSpeed;
    public float f_MaxSpeed_GearZero = 50f;
    public float f_Acceleration = 5f;
    float f_CurrSpeed_Min;
    float f_CurrSpeed_Max;
    float f_CurrTurn;
    public float f_TurnRate = 2;
    public float f_TurnRate_Max = 10;
    CurrentGear enum_CurrGear;

    public GameObject GO_LightWall;
    public GameObject lastWallReference;
    string lastWallMade;
    GameObject oldWallRef, newWallRef;
    GameObject playerLightWall;
    int lightWallName = 0;
    float f_NewWallTimer;

    float f_EndGameTimer;
    bool b_IsGameOver;
    public GameObject Text_YouLose;

    bool b_Beginning = true;
    public GameObject Text_Timer;
    float f_BeginningTimer = 4.5f;

    public AudioClip[] soundEffects;
    public AudioSource audioSource;

    bool b_IsTutorial;

    // Use this for initialization
    void Start ()
    {
        #if !UNITY_EDITOR
        UnityEngine.Cursor.visible = false;
        #endif

        b_IsTutorial = true;

        f_CurrSpeed = 0f;

        enum_CurrGear = CurrentGear.Off;

        // Pre-load the light wall
        oldWallRef = new GameObject();
        oldWallRef.transform.position = lastWallReference.transform.position;
        oldWallRef.name = NameLightWall();

        newWallRef = new GameObject();
        newWallRef.transform.position = lastWallReference.transform.position;
        newWallRef.name = NameLightWall();

        playerLightWall = Instantiate(GO_LightWall, oldWallRef.transform.position, newWallRef.transform.rotation) as GameObject;
        lastWallMade = playerLightWall.name;

        Text_YouLose.SetActive(false);
        Text_Timer.SetActive(true);
    }

	// Update is called once per frame
	void Update ()
    {
        if(GameObject.Find("LightWall_Player(Clone)"))
        {
            GameObject.Destroy(GameObject.Find("LightWall_Player(Clone)"));
        }

        if(!b_IsTutorial)
        {
#region Not Tutorial

            if (f_BeginningTimer > -0.1) f_BeginningTimer -= Time.deltaTime;

            if (f_BeginningTimer <= 0.0f)
            {
                Text_Timer.GetComponent<Text>().enabled = false;
            }

            if (b_Beginning)
            {
                Text_Timer.GetComponent<Text>().text = string.Format("{0:0}", f_BeginningTimer - 1);
                GameObject.Find("PlayerScore").GetComponent<Text>().text = "Score:\n";
                GameObject.Find("LevelInfo").GetComponent<Text>().text = "Level:\n";

                if (f_BeginningTimer <= 1.5f)
                {
                    audioSource.Play();
                    Text_Timer.GetComponent<Text>().text = "Go!";

                    enum_CurrGear = CurrentGear.Zero;
                    SetGearMinMax(0);
                    ToggleDriveMode();

                    f_CurrSpeed = f_MaxSpeed_GearZero;
                    b_Beginning = false;

                    GameObject.Find("LevelManager").GetComponent<Cs_LevelManager>().StartGame();

                }
            }

            if (!b_IsGameOver)
            {
                prevState = state;
                state = GamePad.GetState(playerIndex);

                if (state.Buttons.RightShoulder == ButtonState.Pressed && prevState.Buttons.RightShoulder == ButtonState.Released)
                {
                    GameObject.Find("LevelManager").GetComponent<Cs_LevelManager>().PlayerScoredPrimary();
                    gameObject.GetComponent<Rigidbody>().velocity = gameObject.transform.forward * 0;

                    /*
                    b_IsGameOver = true;

                    gameObject.GetComponent<Renderer>().enabled = false;
                    gameObject.GetComponent<BoxCollider>().enabled = false;

                    Text_YouLose.GetComponent<Text>().text = "Cheater!";
                    Text_YouLose.SetActive(true);
                    */
                }

                if (enum_CurrGear != CurrentGear.Off)
                {
                    Update_Speed();
                    TurnCycle();
                }

                /*
                if (state.Buttons.A == ButtonState.Pressed && prevState.Buttons.A == ButtonState.Released && enum_CurrGear == CurrentGear.Off)
                {
                    enum_CurrGear = CurrentGear.Zero;
                    SetGearMinMax(0);
                    ToggleDriveMode();

                    f_CurrSpeed = f_MaxSpeed_GearZero;
                }
                */

                if (!b_DriveMode)
                {
                    PlaceWall();
                }
            }
            else
            {
                f_EndGameTimer += Time.deltaTime;

                if (f_EndGameTimer >= 5.0f)
                {
                    SceneManager.LoadScene(0);
                }
            }

        #endregion
        }
        else
        {
        #region Tutorial



        #endregion
        }
    }

    public void EndTutorial() { b_IsTutorial = false; }

    string NameLightWall()
    {
        return ("" + lightWallName++);
    }

    void PlaceWall()
    {
        // Increment timer for new wall
        f_NewWallTimer += Time.deltaTime;

        // If we reach a specific amount of time, create a new wall & update appropriately
        if(f_NewWallTimer >= 0.1f)
        {
            oldWallRef = newWallRef;

            newWallRef = new GameObject();
            newWallRef.transform.position = lastWallReference.transform.position;
            newWallRef.name = NameLightWall();

            playerLightWall = Instantiate(GO_LightWall, (newWallRef.transform.position + oldWallRef.transform.position) / 2, newWallRef.transform.rotation) as GameObject;
            playerLightWall.name = NameLightWall();
            lastWallMade = playerLightWall.name;

            // Reset timer
            f_NewWallTimer = 0f;
        }

        // Get the location between the last empty object created
        newWallRef.transform.position = lastWallReference.transform.position;

        // Move the wall between the two empty objects
        playerLightWall.transform.position = (oldWallRef.transform.position + newWallRef.transform.position) / 2;

        // Rotate the wall between the two empty objects
        // Mathf.Atan2(p2.y-p1.y, p2.x-p1.x)*180 / Mathf.PI;
        float newAngle = Mathf.Atan2(oldWallRef.transform.position.x - newWallRef.transform.position.x, oldWallRef.transform.position.z - newWallRef.transform.position.z) * 180 / Mathf.PI;
        playerLightWall.transform.eulerAngles = new Vector3(0, newAngle, 0);

        // Scale the wall between the two empty objects
        Vector3 newScale = playerLightWall.transform.localScale;
        float f_X = newWallRef.transform.position.x - oldWallRef.transform.position.x;
        float f_Y = newWallRef.transform.position.y - oldWallRef.transform.position.y;
        float f_Z = newWallRef.transform.position.z - oldWallRef.transform.position.z;
        float dist = Mathf.Sqrt( (f_X * f_X) + (f_Y * f_Y) + (f_Z * f_Z) );

        playerLightWall.transform.localScale = new Vector3(newScale.x, newScale.y, dist);
    }

    void Update_Speed()
    {
#region Cap player speed
        if (f_CurrSpeed < f_CurrSpeed_Min) f_CurrSpeed += f_Acceleration * Time.deltaTime;
        if (f_CurrSpeed > f_CurrSpeed_Max) f_CurrSpeed -= f_Acceleration * Time.deltaTime;

        // Cap the speed
        /*if(f_CurrSpeed > f_CurrSpeed_Min && f_CurrSpeed < f_CurrSpeed_Min + 0.1f)
        {
            f_CurrSpeed = f_CurrSpeed_Min;
        }
        if(f_CurrSpeed < f_CurrSpeed_Max && f_CurrSpeed > f_CurrSpeed_Max - 0.1f)
        {
            f_CurrSpeed = f_CurrSpeed_Max;
        }*/
#endregion

        bool b_RightTrigger = (state.Triggers.Right >= 0.5f);
        bool b_LeftTrigger = (state.Triggers.Left >= 0.5f) && !b_RightTrigger;

        // Thumbstick 
        if (b_RightTrigger)
        {
            // Increase speed based on Acceleration
            f_CurrSpeed += (f_Acceleration / GetIntFromEnum(enum_CurrGear)) * Time.deltaTime;
        }
        else if(b_LeftTrigger)
        {
            // Decrease speed based on Acceleration * 5 (Brakes)
        }
        else
        {
            // Decrease speed based on Acceleration (Natural slow)
        }

        gameObject.GetComponent<Rigidbody>().velocity = transform.forward * f_CurrSpeed;
    }

    int GetIntFromEnum(CurrentGear currGear_)
    {
        if (currGear_ == CurrentGear.Zero)  return 1;
        if (currGear_ == CurrentGear.One)   return 2;
        if (currGear_ == CurrentGear.Two)   return 3;
        if (currGear_ == CurrentGear.Three) return 4;
        if (currGear_ == CurrentGear.Four)  return 5;

        return 1;
    }

    void SetGearMinMax(int newGear_)
    {
        f_CurrSpeed_Min = newGear_ * f_MaxSpeed_GearZero;
        print("Min: " + f_CurrSpeed_Min);
        f_CurrSpeed_Max = (newGear_ + 1) * f_MaxSpeed_GearZero;
        print("Max: " + f_CurrSpeed_Max);
    }

    void ToggleDriveMode()
    {
        b_DriveMode = !b_DriveMode;

        if(!b_DriveMode)
        {
            // Pre-load the light wall
            oldWallRef = new GameObject();
            oldWallRef.transform.position = lastWallReference.transform.position;

            newWallRef = new GameObject();
            newWallRef.transform.position = lastWallReference.transform.position;

            playerLightWall = Instantiate(GO_LightWall, (newWallRef.transform.position + oldWallRef.transform.position) / 2, newWallRef.transform.rotation) as GameObject;
            playerLightWall.name = NameLightWall();
            lastWallMade = playerLightWall.name;
        }
    }

    void TurnCycle()
    {
        float newRot = state.ThumbSticks.Left.X * f_TurnRate;
        f_CurrTurn = Mathf.Lerp(f_CurrTurn, newRot, 0.05f);

        var currRot = gameObject.transform.eulerAngles;
        currRot.y = f_CurrTurn + gameObject.transform.eulerAngles.y;
        gameObject.transform.eulerAngles = currRot;

        /*
        if(state.ThumbSticks.Left.X <= -0.1f || state.ThumbSticks.Left.X >= 0.1f)
        {
            // The new turn rate increases/decreases based on the X pos of the stick multiplied by the turn rate, multiplied by DT
            // f_CurrTurn = 

            var currRot = gameObject.transform.eulerAngles;
            currRot.y += state.ThumbSticks.Left.X;
            gameObject.transform.eulerAngles = currRot;
        }
        */
    }

    void ToggleGear(bool b_IncreaseGear_)
    {

    }

    void Penalize()
    {

    }

    public void Crash()
    {
        // Destroy(gameObject);
        gameObject.GetComponent<Renderer>().enabled = false;
        gameObject.GetComponent<BoxCollider>().enabled = false;
        gameObject.GetComponent<Rigidbody>().velocity = gameObject.transform.forward * 0;

        Text_YouLose.SetActive(true);

        b_IsGameOver = true;
    }

	void OnCollisionEnter(Collision collision_)
    {
        if(collision_.gameObject.tag == "Wall")
        {
            print("Hit: " + collision_.gameObject.name);

            audioSource.Stop();

            PlaySFX(1);

            Crash();
        }
    }

    public void PlaySFX(int i_)
    {
        audioSource.PlayOneShot(soundEffects[i_], 0.6f);
    }

    void OnTriggerEnter(Collider collider_)
    {
        if (collider_.gameObject == newWallRef) print("New");
        if (collider_.gameObject == oldWallRef) print("Old");

        if(collider_.tag == "Wall_Player" || collider_.tag == "Wall_Enemy")
        {
            if(collider_.name != lastWallMade && collider_.name != "LightWall_Player(Clone)")
            {
                print("Hit: " + collider_.gameObject.name);

                audioSource.Stop();

                PlaySFX(1);

                Crash();
            }
        }
    }
}
