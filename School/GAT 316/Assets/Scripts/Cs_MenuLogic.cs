using UnityEngine;
using System.Collections;
using XInputDotNetPure;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Cs_MenuLogic : MonoBehaviour
{
    bool b_RunGame;
    float f_Alpha;

    // Controller Input
    GamePadState state;
    GamePadState prevState;
    public PlayerIndex playerOne = PlayerIndex.One;

    GameObject go_Title;
    GameObject go_PressStart;
    GameObject go_DigiPen;

    // Use this for initialization
    void Start ()
    {
        state = GamePad.GetState(playerOne);

        go_Title = transform.Find("GameName").gameObject;
        go_PressStart = transform.Find("Press Start").gameObject;
        go_DigiPen = transform.Find("DigiPen").gameObject;
    }
	
	// Update is called once per frame
	void Update ()
    {
        prevState = state;
        state = GamePad.GetState(playerOne);


        // Quit with appropriate input
        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
        if (state.Buttons.Back == ButtonState.Pressed && prevState.Buttons.Back == ButtonState.Released) Application.Quit();
        
        // Run level
        if (state.Buttons.Start == ButtonState.Pressed) b_RunGame = true;

        // Cap alpha
	    if(!b_RunGame)
        {
            f_Alpha += Time.deltaTime;
            if ( f_Alpha > 1.0f ) f_Alpha = 1.0f;
        }
        else
        {
            f_Alpha -= Time.deltaTime;
            if (f_Alpha < 0.0f) f_Alpha = 0.0f;
        }

        Color clr_CurrAlpha = go_PressStart.GetComponent<Text>().color;
        clr_CurrAlpha.a = f_Alpha;
        go_PressStart.GetComponent<Text>().color = clr_CurrAlpha;

        clr_CurrAlpha = go_Title.GetComponent<Text>().color;
        clr_CurrAlpha.a = f_Alpha;
        go_Title.GetComponent<Text>().color = clr_CurrAlpha;

        clr_CurrAlpha = go_DigiPen.GetComponent<Text>().color;
        clr_CurrAlpha.a = f_Alpha;
        go_DigiPen.GetComponent<Text>().color = clr_CurrAlpha;

        if (b_RunGame && f_Alpha == 0f)
        {
            // Load game
            SceneManager.LoadScene(3);
        }

        // Run through text and set alpha

	}
}
