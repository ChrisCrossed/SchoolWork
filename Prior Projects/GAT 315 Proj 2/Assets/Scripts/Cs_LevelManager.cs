using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using XInputDotNetPure; // Controller input

enum GameState
{
    Tutorial,
    PreGame,
    Playing
}

public class Cs_LevelManager : MonoBehaviour
{
    GameState enum_GameState = GameState.Tutorial;

    GamePadState state;
    GamePadState prevState;
    public PlayerIndex playerIndex = PlayerIndex.One;

    int i_PlayerScore;

    int i_TutorialCounter;
    float f_TutorialTimer;
    public Camera mainCamera;
    GameObject camRef_Tut_1;
    GameObject camRef_Tut_2;
    GameObject camRef_Tut_3;
    GameObject camRef_Tut_4;
    GameObject camRef_Tut_5;

    // Level Game Objects
    GameObject Wall_PosX;
    GameObject Wall_NegX;
    GameObject Wall_PosY;
    GameObject Wall_NegY;
    GameObject Floor;
    int i_Pos_Level1;
    int i_Pos_Level2;
    int i_Pos_Level3;
    int i_Pos_Level4;
    int i_Pos_Level5;
    int i_Width_Level1;
    int i_Width_Level2;
    int i_Width_Level3;
    int i_Width_Level4;
    int i_Width_Level5;
    GameObject hudCamera;

    // Timer Objects
    public float GameLengthMinutes;
    public float TimeIncreaseOnScore;
    float f_Timer;
    float f_Countdown;
    float f_TimerIncrement;
    int i_CurrLevel;

    // Score Game Objects
    GameObject Score_Primary;
    GameObject Score_Secondary;
    GameObject ui_Score;
    GameObject ui_LevelInfo;
    public GameObject Text_YouWin;

    // Sound Effects
    public AudioClip[] soundEffects;
    public AudioSource audioSource;

    // Use this for initialization
    void Start ()
    {
        i_CurrLevel = 1;

        i_PlayerScore = 0;
        
        camRef_Tut_1 = GameObject.Find("CamReference_Tut_1");
        camRef_Tut_2 = GameObject.Find("CamReference_Tut_2");
        camRef_Tut_3 = GameObject.Find("CamReference_Tut_3");
        camRef_Tut_4 = GameObject.Find("CamReference_Tut_4");
        camRef_Tut_5 = GameObject.Find("CamReference_Tut_5");

        // Level Game Objects
        Wall_PosX = GameObject.Find("Wall_PosX");
        Wall_NegX = GameObject.Find("Wall_NegX");
        Wall_PosY = GameObject.Find("Wall_PosY");
        Wall_NegY = GameObject.Find("Wall_NegY");
        Floor = GameObject.Find("Floor");

        /* Version 1.0
        i_Pos_Level1 = 325;
        i_Pos_Level2 = 275;
        i_Pos_Level3 = 225;
        i_Pos_Level4 = 175;
        i_Pos_Level5 = 125;

        i_Width_Level1 = 650;
        i_Width_Level2 = 550;
        i_Width_Level3 = 450;
        i_Width_Level4 = 350;
        i_Width_Level5 = 250;
        */

        // Version 2.0
        i_Width_Level1 = 350;
        i_Width_Level2 = 350;
        i_Width_Level3 = 350;
        i_Width_Level4 = 350;
        i_Width_Level5 = 350;

        i_Pos_Level1 = 175;
        i_Pos_Level2 = 175;
        i_Pos_Level3 = 175;
        i_Pos_Level4 = 175;
        i_Pos_Level5 = 175;

        hudCamera = GameObject.Find("HUD_Camera");

        // Timer Objects
        f_Timer = 0;
        f_Countdown = GameLengthMinutes * 60f;
        f_TimerIncrement = TimeIncreaseOnScore;

        // Score Game Objects;
        Score_Primary = GameObject.Find("Gold_Primary");
        Score_Secondary = GameObject.Find("Gold_Secondary");
        ui_Score = GameObject.Find("PlayerScore");
        ui_LevelInfo = GameObject.Find("LevelInfo");
    }

    void UpdateLevelSpecs(int i_LevelWidth_)
    {
        // Update current floor width
        Vector3 floorWidth = Floor.transform.localScale;

        if (floorWidth.x > i_LevelWidth_ / 10)
        {
            floorWidth.x -= Time.deltaTime * 5;
            floorWidth.z -= Time.deltaTime * 5;
        }
        else
        {
            floorWidth.x = i_LevelWidth_ / 10;
            floorWidth.z = i_LevelWidth_ / 10;
        }

        //Floor.transform.localScale = floorWidth;

        // Update current wall positions
        float wallPos = Floor.transform.localScale.x * 10 / 2;

        //Wall_PosX.transform.position = new Vector3(wallPos, Wall_PosX.transform.position.y, Wall_PosX.transform.position.z);

        //Wall_NegX.transform.position = new Vector3(-wallPos, Wall_NegX.transform.position.y, Wall_NegX.transform.position.z);

        //Wall_PosY.transform.position = new Vector3(Wall_PosY.transform.position.x, Wall_PosY.transform.position.y, wallPos);

        //Wall_NegY.transform.position = new Vector3(Wall_NegY.transform.position.x, Wall_NegY.transform.position.y, -wallPos);

        // Update current wall lengths
        float wallLength = Floor.transform.localScale.x * 10;

        //Wall_PosX.transform.localScale = new Vector3(Wall_PosX.transform.localScale.x, Wall_PosX.transform.localScale.y, wallLength);

        //Wall_NegX.transform.localScale = new Vector3(Wall_NegX.transform.localScale.x, Wall_NegX.transform.localScale.y, wallLength);

        //Wall_PosY.transform.localScale = new Vector3(wallLength, Wall_PosY.transform.localScale.y, Wall_PosY.transform.localScale.z);

        //Wall_NegY.transform.localScale = new Vector3(wallLength, Wall_NegY.transform.localScale.y, Wall_NegY.transform.localScale.z);

        // Lerp camera's height
        Vector3 newCamPos = hudCamera.transform.position;
        newCamPos.y = Mathf.Lerp(newCamPos.y, i_LevelWidth_, 0.05f / i_CurrLevel);
        hudCamera.transform.position = newCamPos;
    }

    public int GetFieldSize()
    {
        if (i_CurrLevel == 1) return i_Pos_Level1;
        if (i_CurrLevel == 2) return i_Pos_Level2;
        if (i_CurrLevel == 3) return i_Pos_Level3;
        if (i_CurrLevel == 4) return i_Pos_Level4;
        if (i_CurrLevel == 5) return i_Pos_Level5;

        return i_Pos_Level1;
    }

    public void StartGame()
    {
        GameObject.Find("Timer").GetComponent<Text>().text = "";
        GameObject.Find("PlayerScore").GetComponent<Text>().text = "Score:\n";
        GameObject.Find("LevelInfo").GetComponent<Text>().text = "Level:\n";

        // Disable Tutorial Gold
        GameObject.Destroy(GameObject.Find("Gold_Primary_Tutorial"));
        GameObject.Destroy(GameObject.Find("Gold_Secondary_Tutorial"));
        GameObject.Destroy(GameObject.Find("UI_Primary_Tutorial"));
        GameObject.Destroy(GameObject.Find("UI_Secondary_Tutorial"));

        // Enable the gold
        Score_Primary.GetComponent<Cs_GoldLogic>().StartGame();
        Score_Secondary.GetComponent<Cs_GoldLogic>().StartGame();

        f_Countdown = GameLengthMinutes * 60;
        
        ui_Score.GetComponent<Text>().text = "Score:\n" + string.Format("{0}", i_PlayerScore);
        ui_LevelInfo.GetComponent<Text>().text = "Level:\n" + i_CurrLevel.ToString() + " of 5";

        audioSource.Play();

        enum_GameState = GameState.Playing;
    }

    public int GetCountdownTimer() { return (int)f_Countdown; }

    public void SetPlayerScore(int i_PlayerScore_)
    {
        i_PlayerScore += i_PlayerScore_;
        ui_Score.GetComponent<Text>().text = "Score:\n" + string.Format("{0}", i_PlayerScore);
    }

    public void PlayerScoredPrimary()
    {
        if(i_CurrLevel < 5)
        {
            ++i_CurrLevel;

            GameObject.Find("Player").GetComponent<Cs_PlayerController>().PlaySFX(3);

            Score_Primary.GetComponent<Cs_GoldLogic>().RespawnGold();
            Score_Secondary.GetComponent<Cs_GoldLogic>().RespawnGold();
        }
        else
        {
            // End game - Player Wins
            Text_YouWin.GetComponent<Text>().text = "You Win!";

            GameObject.Find("Player").GetComponent<Cs_PlayerController>().PlaySFX(2);

            GameObject.Find("Player").GetComponent<Cs_PlayerController>().Crash();
        }

        ui_LevelInfo.GetComponent<Text>().text = "Level:\n" + i_CurrLevel.ToString() + " of 5";
    }
	
	// Update is called once per frame
	void Update ()
    {
        if(enum_GameState == GameState.Playing)
        {
            #region Playing

            // Cheat Codes
            if (Input.GetKeyDown(KeyCode.P)) { PlayerScoredPrimary(); }

            f_Timer += Time.deltaTime;
            if(f_Timer >= 1.0f)
            {
                f_Timer = 0;
                --f_Countdown;

                // if (f_Countdown <= 0) GameObject.Find("Player").GetComponent<Cs_PlayerController>().Crash();
            }

	        if (i_CurrLevel == 1)
            {
                UpdateLevelSpecs(i_Width_Level1);
            }
            else if (i_CurrLevel == 2)
            {
                UpdateLevelSpecs(i_Width_Level2);
            }
            else if (i_CurrLevel == 3)
            {
                UpdateLevelSpecs(i_Width_Level3);
            }
            else if (i_CurrLevel == 4)
            {
                UpdateLevelSpecs(i_Width_Level4);
            }
            else // i_CurrLevel == 5
            {
                UpdateLevelSpecs(i_Width_Level5);
            }

            #endregion
        }
        else if(enum_GameState == GameState.Tutorial)
        {
            #region Tutorial

            f_TutorialTimer += Time.deltaTime;

            prevState = state;
            state = GamePad.GetState(playerIndex);

            if (state.Buttons.Start == ButtonState.Pressed && prevState.Buttons.Start == ButtonState.Released)
            {
                GameObject.Find("Player").GetComponent<Cs_PlayerController>().EndTutorial();
                StartGame();
                audioSource.Stop();
                mainCamera.GetComponent<Cs_CameraController>().SetCameraLock(true);

                GameObject.Find("Timer").GetComponent<Text>().enabled = true;
                GameObject.Find("PlayerScore").GetComponent<Text>().enabled = true;
                GameObject.Find("LevelInfo").GetComponent<Text>().enabled = true;
                GameObject.Destroy(GameObject.Find("InfoSkip"));
            }

            float f_CamMoveTime = 0.05f;

            if (i_TutorialCounter == 0) // Objective of Game
            {
                GameObject.Find("Timer").GetComponent<Text>().text = "";
                GameObject.Find("PlayerScore").GetComponent<Text>().text = "";
                GameObject.Find("LevelInfo").GetComponent<Text>().text = "";

                GameObject.Find("Gold_Primary_Tutorial").GetComponent<MeshRenderer>().enabled = true;
                GameObject.Find("Gold_Secondary_Tutorial").GetComponent<MeshRenderer>().enabled = true;

                mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, camRef_Tut_1.transform.position, f_CamMoveTime);
                mainCamera.transform.rotation = Quaternion.Lerp(mainCamera.transform.rotation, camRef_Tut_1.transform.rotation, f_CamMoveTime);

                if (f_TutorialTimer > 1.0f && f_TutorialTimer < 1.1f)
                {
                    audioSource.PlayOneShot(soundEffects[1], 1.0f);
                    f_TutorialTimer = 1.1f;
                }

                if (f_TutorialTimer > 7f)
                {
                    ++i_TutorialCounter;
                    f_TutorialTimer = 0f;
                }
            }
            else if (i_TutorialCounter == 1) // Show Objectives
            {
                mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, camRef_Tut_2.transform.position, f_CamMoveTime);
                mainCamera.transform.rotation = Quaternion.Lerp(mainCamera.transform.rotation, camRef_Tut_2.transform.rotation, f_CamMoveTime);

                if (f_TutorialTimer > 1.0f && f_TutorialTimer < 1.1f)
                {
                    audioSource.PlayOneShot(soundEffects[2], 1.0f);
                    f_TutorialTimer = 1.1f;
                }

                if (f_TutorialTimer > 8f)
                {
                    ++i_TutorialCounter;
                    f_TutorialTimer = 0f;
                }
            }
            else if (i_TutorialCounter == 2) // Walls Shrink
            {
                mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, camRef_Tut_3.transform.position, f_CamMoveTime);
                mainCamera.transform.rotation = Quaternion.Lerp(mainCamera.transform.rotation, camRef_Tut_3.transform.rotation, f_CamMoveTime);

                if (f_TutorialTimer > 1.0f && f_TutorialTimer < 1.1f)
                {
                    audioSource.PlayOneShot(soundEffects[3], 1.0f);
                    f_TutorialTimer = 1.1f;
                }

                if (f_TutorialTimer > 8f)
                {
                    ++i_TutorialCounter;
                    f_TutorialTimer = 0f;
                }
            }
            else if (i_TutorialCounter == 3) // Discuss Failure
            {
                mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, camRef_Tut_4.transform.position, f_CamMoveTime);
                mainCamera.transform.rotation = Quaternion.Lerp(mainCamera.transform.rotation, camRef_Tut_4.transform.rotation, f_CamMoveTime);

                if (f_TutorialTimer > 1.0f && f_TutorialTimer < 1.1f)
                {
                    audioSource.PlayOneShot(soundEffects[4], 1.0f);
                    f_TutorialTimer = 1.1f;
                }

                if (f_TutorialTimer > 17f)
                {
                    ++i_TutorialCounter;
                    f_TutorialTimer = 0f;
                }
            }
            else if (i_TutorialCounter == 4) // Controls & Mini-Map
            {
                mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, camRef_Tut_5.transform.position, f_CamMoveTime);
                mainCamera.transform.rotation = Quaternion.Lerp(mainCamera.transform.rotation, camRef_Tut_5.transform.rotation, f_CamMoveTime);

                if (f_TutorialTimer > 1.0f && f_TutorialTimer < 1.1f)
                {
                    audioSource.PlayOneShot(soundEffects[5], 1.0f);
                    f_TutorialTimer = 1.1f;
                }

                if (f_TutorialTimer > 11f)
                {
                    GameObject.Find("Player").GetComponent<Cs_PlayerController>().EndTutorial();
                    StartGame();
                    audioSource.Stop();
                    mainCamera.GetComponent<Cs_CameraController>().SetCameraLock(true);
                    
                    GameObject.Find("PlayerScore").GetComponent<Text>().enabled = true;
                    GameObject.Find("LevelInfo").GetComponent<Text>().enabled = true;
                    GameObject.Destroy(GameObject.Find("InfoSkip"));
                }
            }
            #endregion
        }
    }
}
