using UnityEngine;
using System.Collections;
using XInputDotNetPure; // Controller input
using UnityEngine.SceneManagement;

public class Cs_TitleScreenLogic : MonoBehaviour
{
    float f_Timer;

    public GameObject DigiPenLogo;
    public GameObject DigiPenText;
    public GameObject ControllerLogo;
    public GameObject RabbleRotsLogo;

    GamePadState state_one;
    GamePadState prevState_one;
    GamePadState state_two;
    GamePadState prevState_two;
    public PlayerIndex playerIndex_One = PlayerIndex.One;
    public PlayerIndex playerIndex_Two = PlayerIndex.Two;

    bool b_LogoActive;

    // Use this for initialization
    void Start ()
    {
        // UnityEngine.Cursor.visible = false;

        // Enable DigiPen Logos
        DigiPenLogo.SetActive(true);
        DigiPenText.SetActive(true);

        // Disable Controller Logos
        ControllerLogo.SetActive(false);

        RabbleRotsLogo.SetActive(false);
    }
	
	// Update is called once per frame
	void Update ()
    {
        if(!b_LogoActive)
        {
            f_Timer += Time.deltaTime;

            if (f_Timer >= 3.0f)
            {
                // Disable DigiPen Logos
                DigiPenLogo.SetActive(false);
                DigiPenText.SetActive(false);

                // Enable Controller Logos
                ControllerLogo.SetActive(true);

                // RabbleRots Logo
                RabbleRotsLogo.SetActive(false);
            }
            if(f_Timer >= 6.0f)
            {
                // Disable DigiPen Logos
                DigiPenLogo.SetActive(false);
            
                // Disable Controller Logos
                ControllerLogo.SetActive(false);

                // Set Game Info
                DigiPenText.SetActive(true);
                RabbleRotsLogo.SetActive(true);

                b_LogoActive = true;
            }
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            print("Escape Pressed");
            Application.Quit();
        }

        if(b_LogoActive)
        {
            prevState_one = state_one;
            state_one = GamePad.GetState(playerIndex_One);

            prevState_two = state_two;
            state_two = GamePad.GetState(playerIndex_Two);
            // If either player presses 'A', start game

            if(state_one.Buttons.Start == ButtonState.Pressed && prevState_one.Buttons.Start == ButtonState.Released)
            {
                Application.Quit();
            }

            if(state_one.Buttons.A == ButtonState.Pressed && prevState_one.Buttons.A == ButtonState.Released)
            {
                SceneManager.LoadScene(1);
            }
            if (state_two.Buttons.A == ButtonState.Pressed && prevState_two.Buttons.A == ButtonState.Released)
            {
                SceneManager.LoadScene(1);
            }

            if (state_one.Buttons.B == ButtonState.Pressed && prevState_one.Buttons.B == ButtonState.Released)
            {
                Application.Quit();
            }
            if (state_two.Buttons.B == ButtonState.Pressed && prevState_two.Buttons.B == ButtonState.Released)
            {
                Application.Quit();
            }
        }
	}
}
