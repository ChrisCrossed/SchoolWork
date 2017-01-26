using UnityEngine;
using System.Collections;
using XInputDotNetPure; // Controller input
using UnityEngine.SceneManagement;

public class Cs_MainMenuLogic : MonoBehaviour
{
    // Player One
    GamePadState state_P1;
    GamePadState prevState_P1;
    PlayerIndex playerIndex_P1 = PlayerIndex.One;

    // Player Two
    GamePadState state_P2;
    GamePadState prevState_P2;
    PlayerIndex playerIndex_P2 = PlayerIndex.Two;

    // Menu UI
    public GameObject ui_Selector;
    public GameObject ui_StartGame;
    public GameObject ui_HowToPlay;
    public GameObject ui_Controls;
    public GameObject ui_Quit;
    public GameObject ui_QuitConfirm;

    // UI State
    uint i_CurrChoice = 1;
    bool b_QuitConfirm = false;
    bool b_HowToPlay = false;
    bool b_Controls = false;

    // Use this for initialization
    void Start ()
    {
	
	}

    void ControllerInput()
    {
        print(state_P1.ThumbSticks.Left.Y);

        if (state_P1.ThumbSticks.Left.Y >= 0.4f && prevState_P1.ThumbSticks.Left.Y < 0.4f) ChangeMenuOption(true);
        if (state_P1.ThumbSticks.Left.Y <= -0.4f && prevState_P1.ThumbSticks.Left.Y > -0.4f) ChangeMenuOption(false);

        if (state_P2.ThumbSticks.Left.Y >= 0.4f && prevState_P2.ThumbSticks.Left.Y < 0.4f) ChangeMenuOption(true);
        if (state_P2.ThumbSticks.Left.Y <= -0.4f && prevState_P2.ThumbSticks.Left.Y > -0.4f) ChangeMenuOption(false);

        if (state_P1.Buttons.A == ButtonState.Pressed && prevState_P1.Buttons.A == ButtonState.Released) SelectMenuOption();
        if (state_P2.Buttons.A == ButtonState.Pressed && prevState_P2.Buttons.A == ButtonState.Released) SelectMenuOption();
    }
	
	// Update is called once per frame
	void Update ()
    {
        prevState_P1 = state_P1;
        state_P1 = GamePad.GetState(PlayerIndex.One);

        prevState_P2 = state_P2;
        state_P2 = GamePad.GetState(PlayerIndex.Two);

        if (!b_QuitConfirm && !b_Controls && !b_HowToPlay)
        {
            ControllerInput();

            #region Lerp Position
            Vector3 newPos = ui_Selector.GetComponent<RectTransform>().transform.position;

            if (i_CurrChoice == 1)
            {
                newPos.y = ui_StartGame.GetComponent<RectTransform>().transform.position.y;
            }
            else if (i_CurrChoice == 2)
            {
                newPos.y = ui_HowToPlay.GetComponent<RectTransform>().transform.position.y;
            }
            else if (i_CurrChoice == 3)
            {
                newPos.y = ui_Controls.GetComponent<RectTransform>().transform.position.y;
            }
            else
            {
                newPos.y = ui_Quit.GetComponent<RectTransform>().transform.position.y;
            }

            ui_Selector.GetComponent<RectTransform>().transform.position = Vector3.Lerp(ui_Selector.GetComponent<RectTransform>().transform.position, newPos, 0.1f);
            #endregion
        }
        else
        {
            if(b_QuitConfirm)
            {
                #region Quit Confirm Overlay
                ui_QuitConfirm.SetActive(true);

                // Quit if either player presses A
                if (state_P1.Buttons.A == ButtonState.Pressed && prevState_P1.Buttons.A == ButtonState.Released) Application.Quit();
                if (state_P2.Buttons.A == ButtonState.Pressed && prevState_P2.Buttons.A == ButtonState.Released) Application.Quit();

                if ((state_P1.Buttons.X == ButtonState.Pressed && prevState_P1.Buttons.X == ButtonState.Released) ||
                     state_P2.Buttons.X == ButtonState.Pressed && prevState_P2.Buttons.X == ButtonState.Released)
                {
                    // Disable the visual
                    ui_QuitConfirm.SetActive(false);

                    // Deactivate the bool state
                    b_QuitConfirm = false;
                }
                #endregion
            }
            if (b_HowToPlay)
            {
                #region How To Play Overlay
                /*
                ui_HowToPlay.SetActive(true);

                // Quit if either player presses A
                if (state_P1.Buttons.A == ButtonState.Pressed && prevState_P1.Buttons.A == ButtonState.Released) Application.Quit();
                if (state_P2.Buttons.A == ButtonState.Pressed && prevState_P2.Buttons.A == ButtonState.Released) Application.Quit();

                if ((state_P1.Buttons.X == ButtonState.Pressed && prevState_P1.Buttons.X == ButtonState.Released) ||
                     state_P2.Buttons.X == ButtonState.Pressed && prevState_P2.Buttons.X == ButtonState.Released)
                {
                    // Disable the visual
                    ui_HowToPlay.SetActive(false);

                    // Deactivate the bool state
                    b_QuitConfirm = false;
                }
                */
                #endregion
            }
            if (b_Controls)
            {

            }
        }
    }

    void ChangeMenuOption(bool b_GoUp_)
    {
        if (b_GoUp_) i_CurrChoice -= 1;
        if (!b_GoUp_) i_CurrChoice += 1;
        if (i_CurrChoice < 1) i_CurrChoice = 1;
        if (i_CurrChoice > 4) i_CurrChoice = 4;
    }

    void SelectMenuOption()
    {
        if (i_CurrChoice == 1) SceneManager.LoadScene("Level_1"); // Play Game
        if (i_CurrChoice == 2) SceneManager.LoadScene("Info_HowToPlay"); ; // How To Play
        if (i_CurrChoice == 3) SceneManager.LoadScene("Info_Controls_Driver"); ; // Controls
        if (i_CurrChoice == 4) b_QuitConfirm = true; // Quit Game
    }
}
