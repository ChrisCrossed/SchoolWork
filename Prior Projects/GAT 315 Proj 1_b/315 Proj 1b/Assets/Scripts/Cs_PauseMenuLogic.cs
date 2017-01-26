using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Cs_PauseMenuLogic : MonoBehaviour
{
    // Current States
    bool b_IsPaused = false;
    bool b_HowToPlay = false;
    uint ui_CurrChoice = 1;
    bool b_QuitConfirm = false;

    // UI Objects
    public GameObject go_Backdrop;
    public GameObject go_Selector;
    public GameObject go_ResumeGame;
    public GameObject go_HowToPlay;
    public GameObject go_Quit;
    public GameObject go_QuitConfirm;
    public GameObject ui_HowToPlay1;
    public GameObject ui_HowToPlay2;
    public GameObject ui_HowToPlay3;
    int i_HowToPlay;

    // Player Objects
    GameObject mechBase;
    GameObject turretBase;

    // SFX
    public AudioClip sfx_MenuChoice;
    public AudioClip sfx_MenuSelect;
    AudioSource audioSource;

    // Use this for initialization
    void Start ()
    {
        mechBase = GameObject.Find("Mech");
        turretBase = GameObject.Find("Mech_Turret");

        audioSource = turretBase.GetComponent<Cs_MechTurretController>().audioSource;

        TogglePause();
        TogglePause();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (audioSource == null) audioSource = turretBase.GetComponent<Cs_MechTurretController>().audioSource;

        if (!b_QuitConfirm && !b_HowToPlay)
        {
            #region Lerp Position
            Vector3 newPos = go_Selector.GetComponent<RectTransform>().transform.position;

            if (ui_CurrChoice == 1)
            {
                newPos.y = go_ResumeGame.GetComponent<RectTransform>().transform.position.y;
            }
            else if (ui_CurrChoice == 2)
            {
                newPos.y = go_HowToPlay.GetComponent<RectTransform>().transform.position.y;
            }
            else
            {
                newPos.y = go_Quit.GetComponent<RectTransform>().transform.position.y;
            }

            go_Selector.GetComponent<RectTransform>().transform.position = Vector3.Lerp(go_Selector.GetComponent<RectTransform>().transform.position, newPos, 0.1f);
            #endregion
        }
        else
        {
            if(b_QuitConfirm) go_QuitConfirm.SetActive(true);
            else // How To Play
            {
                ui_HowToPlay1.SetActive(false);
                ui_HowToPlay2.SetActive(false);
                ui_HowToPlay3.SetActive(false);

                if (i_HowToPlay == 0) ui_HowToPlay1.SetActive(true);
                if (i_HowToPlay == 1) ui_HowToPlay2.SetActive(true);
                if (i_HowToPlay == 2) ui_HowToPlay3.SetActive(true);
                if (i_HowToPlay > 2) b_HowToPlay = false;
            }
        }
    }

    public void TogglePause()
    {
        if(audioSource != null) audioSource.PlayOneShot(sfx_MenuChoice);

        // Toggle the pause state.
        b_IsPaused = !b_IsPaused;

        // Enable/Disable the UI element on-screen
        go_Backdrop.SetActive(b_IsPaused);

        // Time.timeScale pauses the game, or changes the standard pace of the game.
        if (b_IsPaused) Time.timeScale = 0f; else Time.timeScale = 1.0f;

        // If we just paused the game, reset the menu choice
        if (b_IsPaused) ui_CurrChoice = 1;

        // Tell each player that they are now paused/unpaused
        turretBase.GetComponent<Cs_MechTurretController>().SetPausedState(b_IsPaused);
        mechBase.GetComponent<Cs_MechBaseController>().SetPausedState(b_IsPaused);
    }

    public void ChangePauseOption(bool b_GoUp_)
    {
        audioSource.PlayOneShot(sfx_MenuChoice);

        if (b_GoUp_) ui_CurrChoice -= 1;
        if (!b_GoUp_) ui_CurrChoice += 1;
        if (ui_CurrChoice < 1) ui_CurrChoice = 1;
        if (ui_CurrChoice > 3) ui_CurrChoice = 3;
    }

    public void ConfirmPauseOption(bool b_IsConfirmQuit_)
    {
        audioSource.PlayOneShot(sfx_MenuSelect);

        if (!b_QuitConfirm && b_IsConfirmQuit_ && !b_HowToPlay)
        {
            if (ui_CurrChoice == 1) TogglePause(); // Play Game
            if (ui_CurrChoice == 2) { b_HowToPlay = true; i_HowToPlay = 0; } // How To Play
            if (ui_CurrChoice == 3) b_QuitConfirm = true; // Quit Confirm
        }
        else if(b_HowToPlay)
        {
            ++i_HowToPlay;
        }
        else // Quit Confirm up
        {
            if (b_IsConfirmQuit_) SceneManager.LoadScene("Level_MainMenu");
            else
            {
                go_QuitConfirm.SetActive(false);
                b_QuitConfirm = false;
            }
        }
    }
}
