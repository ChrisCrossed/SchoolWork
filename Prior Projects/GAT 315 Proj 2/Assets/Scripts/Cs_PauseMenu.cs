using UnityEngine;
using System.Collections;

public class Cs_PauseMenu : MonoBehaviour
{
    // Current states
    bool b_IsPaused = false;
    uint ui_CurrChoice = 1;
    bool b_HowToPlayShown = false;
    bool b_QuitConfirmShown = false;

    // UI Objects
    public GameObject pauseMenu;
    public GameObject selector;
    public GameObject resume;
    public GameObject quit;
    public GameObject howtoplay;
    public GameObject Img_HowToPlay;
    public GameObject Img_QuitConfirm;

    // Gameplay Objects
    public GameObject turretBase;
    public GameObject mechBase;

    // SFX
    public AudioClip sfx_MenuSelect;
    public AudioClip sfx_MenuBack;
    AudioSource audioSource;

    // Use this for initialization
    void Start ()
    {
        pauseMenu = GameObject.Find("PauseMenu");
        pauseMenu.SetActive(b_IsPaused);

        // SFX
        audioSource = GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if(b_IsPaused)
        {
	        if(ui_CurrChoice == 1)
            {
                // Lerp the Selector object alongside the text
                float newY = selector.transform.position.y;
                newY = Mathf.Lerp(newY, resume.transform.position.y, 0.1f);
                selector.transform.position = new Vector3(selector.transform.position.x, newY, selector.transform.position.z);
            }
            else if(ui_CurrChoice == 2)
            {
                // Lerp the Selector object alongside the text
                float newY = selector.transform.position.y;
                newY = Mathf.Lerp(newY, howtoplay.transform.position.y, 0.1f);
                selector.transform.position = new Vector3(selector.transform.position.x, newY, selector.transform.position.z);
            }
            else
            {
                // Lerp the Selector object alongside the text
                float newY = selector.transform.position.y;
                newY = Mathf.Lerp(newY, quit.transform.position.y, 0.1f);
                selector.transform.position = new Vector3(selector.transform.position.x, newY, selector.transform.position.z);
            }

            Img_HowToPlay.SetActive(b_HowToPlayShown);
            Img_QuitConfirm.SetActive(b_QuitConfirmShown);
        }
	}

    public void TogglePause()
    {
        // Toggle the pause state.
        b_IsPaused = !b_IsPaused;

        // Enable/Disable the UI element on-screen
        pauseMenu.SetActive(b_IsPaused);

        // Time.timeScale pauses the game, or changes the standard pace of the game.
        if (b_IsPaused) Time.timeScale = 0f; else Time.timeScale = 1.0f;

        // If we just paused the game, reset the menu choice
        if (b_IsPaused) ui_CurrChoice = 1;

        // Tell each player that they are now paused/unpaused
        // turretBase.GetComponent<Cs_MechTurretLogic>().SetPausedState(b_IsPaused);
        // mechBase.GetComponent<Cs_MechBaseLogic>().SetPausedState(b_IsPaused);
    }

    public void SetPauseMenuChoice(bool b_UpPressed_)
    {
        // Stops input from occurring while the HowToPlay screen is up
        if(!b_HowToPlayShown && !b_QuitConfirmShown)
        {
            if (b_UpPressed_) ui_CurrChoice -= 1;
            else ui_CurrChoice += 1;

            if (b_UpPressed_ && ui_CurrChoice >= 1) audioSource.PlayOneShot(sfx_MenuSelect, 0.5f);
            if (!b_UpPressed_ && ui_CurrChoice <= 3) audioSource.PlayOneShot(sfx_MenuSelect, 0.5f);


            if (ui_CurrChoice <= 1) ui_CurrChoice = 1;
            if (ui_CurrChoice >= 3) ui_CurrChoice = 3;
        }
    }

    public void ButtonBPressed()
    {
        audioSource.PlayOneShot(sfx_MenuBack, 0.5f);

        if (b_HowToPlayShown || b_QuitConfirmShown)
        {
            b_HowToPlayShown = false;
            b_QuitConfirmShown = false;
        }
        else TogglePause();
    }

    public void SelectPauseMenuChoice()
    {
        // Stops input from occurring while the HowToPlay screen is up
        if (!b_HowToPlayShown && !b_QuitConfirmShown)
        {
            audioSource.PlayOneShot(sfx_MenuSelect, 0.5f);

            if (ui_CurrChoice == 1)
            {
                // Disable the pause. Return to game.
                TogglePause();
            }
            if(ui_CurrChoice == 2)
            {
                // Show HowToPlay screen
                b_HowToPlayShown = true;
            }
            if(ui_CurrChoice == 3)
            {
                // Quit
                b_QuitConfirmShown = true;
            }
        }
        else // How To Play is shown
        {
            audioSource.PlayOneShot(sfx_MenuBack, 0.5f);

            if (b_HowToPlayShown) b_HowToPlayShown = false;

            if (b_QuitConfirmShown) Application.Quit();
        }
    }
}
