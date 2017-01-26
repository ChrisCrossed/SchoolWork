using UnityEngine;
using System.Collections;
using XInputDotNetPure; // Controller input
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Cs_InfoScreenLogic : MonoBehaviour
{
    // Player One
    GamePadState state_P1;
    GamePadState prevState_P1;
    PlayerIndex playerIndex_P1 = PlayerIndex.One;

    // Player Two
    GamePadState state_P2;
    GamePadState prevState_P2;
    PlayerIndex playerIndex_P2 = PlayerIndex.Two;

    public string LevelToLoad;

    float f_Timer = 0;
    public float f_MaxTimer = 10;

    public bool b_ShowButton = false;

    GameObject ui_ButtonA;

    // Use this for initialization
    void Start()
    {
        ui_ButtonA = GameObject.Find("Btn_A");

        ui_ButtonA.SetActive(false);

        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        prevState_P1 = state_P1;
        state_P1 = GamePad.GetState(playerIndex_P1);

        prevState_P2 = state_P2;
        state_P2 = GamePad.GetState(playerIndex_P2);

        f_Timer += Time.deltaTime;

        if(b_ShowButton && f_Timer >= f_MaxTimer / 3)
        {
            ui_ButtonA.SetActive(true);
        }

        if (state_P1.Buttons.A == ButtonState.Pressed && prevState_P1.Buttons.A == ButtonState.Released) f_Timer = f_MaxTimer;
        if (state_P2.Buttons.A == ButtonState.Pressed && prevState_P2.Buttons.A == ButtonState.Released) f_Timer = f_MaxTimer;

        if (f_Timer >= f_MaxTimer) SceneManager.LoadScene(LevelToLoad);
    }
}
