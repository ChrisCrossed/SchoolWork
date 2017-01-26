using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using XInputDotNetPure;
using UnityEngine.UI;

/*********************************
 * 
 * Copyright DigiPen Institute of Technology 2016
 * 
 * Streak 2 by Christopher Christensen
 * 
 * *******************************/

enum MenuButtonSelected
{
    Button_Zero,
    Button_One,
    Button_Two,
    Button_Three
}

public class Cs_MainMenuLogic : MonoBehaviour
{
    GameObject go_GameSettings;

    GameObject go_MenuButtons_1;
    GameObject go_MenuButtons_2;

    Vector3 v3_MenuPosition_OnScreen;
    Vector3 v3_MenuPosition_Left;
    Vector3 v3_MenuPosition_Below;

    PlayerIndex pad_PlayerOne = PlayerIndex.One;
    GamePadState state;
    GamePadState prevState;

    bool b_OnNewGameMenu;
    float f_MenuTransitionTimer;

    bool b_PlayerInputAllowed;

    [SerializeField] GameObject[] go_ButtonList = new GameObject[7];

    MenuButtonSelected enum_ButtonSelected;

    [SerializeField] AnimationCurve animCurve;

    float f_CreditsTimer;
    static float f_CreditsTimer_Max = 30f;

	// Use this for initialization
	void Start ()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (GameObject.Find("GameSettings"))
        {
            go_GameSettings = GameObject.Find("GameSettings").gameObject;
        }

        // Set Menu Button Positions
        v3_MenuPosition_OnScreen = GameObject.Find("MenuPosition_OnScreen").transform.position;
        v3_MenuPosition_Left = GameObject.Find("MenuPosition_Left").transform.position;
        v3_MenuPosition_Below = GameObject.Find("MenuPosition_Below").transform.position;

        // Deselect all buttons
        for (int i_ = 0; i_ < go_ButtonList.Length; ++i_)
        {
            go_ButtonList[i_].GetComponent<Cs_MenuButtonLogic>().Init_Button(false);
        }

        // Enable first button
        go_ButtonList[0].GetComponent<Cs_MenuButtonLogic>().IsSelected = true;

        // Set default menu values
        enum_ButtonSelected = MenuButtonSelected.Button_One;
        b_OnNewGameMenu = false;

        go_MenuButtons_1 = GameObject.Find("MenuButtons_1");
        go_MenuButtons_2 = GameObject.Find("MenuButtons_2");
        go_QuitMenuObjects = GameObject.Find("QuitMenuObjects");
        go_CreditsMenuObjects = GameObject.Find("Credits");
        v3_Position_OffScreen = GameObject.Find("QuitMenu_Pos_OffScreen").transform.position;
        v3_Position_OnScreen = GameObject.Find("QuitMenu_Pos_OnScreen").transform.position;

        // Set button initial positions
        v3_MainMenu_PrevLoc = go_MenuButtons_1.transform.position;
        v3_MainMenu_CurrLoc = v3_MenuPosition_OnScreen;
        v3_NewGame_PrevLoc = go_MenuButtons_2.transform.position;
        v3_NewGame_CurrLoc = v3_MenuPosition_Below;
        f_LerpTimer = 0.0f;

        // Set Quit Menu Objects Off Screen
        QuitMenuActive = false;
        f_QuitMenuLerpTimer = 1.0f;
        CreditsActive = false;
        f_CreditsLerpTimer = 0.0f;
    }

    void Set_ButtonHighlighed( bool b_OnNewGameMenu_, MenuButtonSelected e_ButtonSelected_ )
    {
        // Deselect all buttons
        for(int i_ = 0; i_ < go_ButtonList.Length; ++i_)
        {
            go_ButtonList[i_].GetComponent<Cs_MenuButtonLogic>().IsSelected = false;
        }

        // On main menu
        if( !b_OnNewGameMenu_ )
        {
            if(e_ButtonSelected_ == MenuButtonSelected.Button_One)
            {
                // Button One
                go_ButtonList[0].GetComponent<Cs_MenuButtonLogic>().IsSelected = true;
            }
            else if (e_ButtonSelected_ == MenuButtonSelected.Button_Two)
            {
                // Button Two
                go_ButtonList[1].GetComponent<Cs_MenuButtonLogic>().IsSelected = true;
            }
            else
            {
                // Button Three
                go_ButtonList[2].GetComponent<Cs_MenuButtonLogic>().IsSelected = true;
            }
        }
        else
        {
            if( e_ButtonSelected_ == MenuButtonSelected.Button_Zero )
            {
                // Button Zero (Tutorial)
                go_ButtonList[3].GetComponent<Cs_MenuButtonLogic>().IsSelected = true;
            }
            else if (e_ButtonSelected_ == MenuButtonSelected.Button_One)
            {
                // Button One
                go_ButtonList[4].GetComponent<Cs_MenuButtonLogic>().IsSelected = true;
            }
            else if (e_ButtonSelected_ == MenuButtonSelected.Button_Two)
            {
                // Button Two
                go_ButtonList[5].GetComponent<Cs_MenuButtonLogic>().IsSelected = true;
            }
            else
            {
                // Button Three
                go_ButtonList[6].GetComponent<Cs_MenuButtonLogic>().IsSelected = true;
            }
        }
    }

    void Menu_Up()
    {
        if( enum_ButtonSelected == MenuButtonSelected.Button_Two )
        {
            enum_ButtonSelected = MenuButtonSelected.Button_One;

            Set_ButtonHighlighed( b_OnNewGameMenu, enum_ButtonSelected );
        }
        else if( enum_ButtonSelected == MenuButtonSelected.Button_Three )
        {
            enum_ButtonSelected = MenuButtonSelected.Button_Two;

            Set_ButtonHighlighed(b_OnNewGameMenu, enum_ButtonSelected);
        }
        // Special Case: Go to button Zero only if in game mode selection
        else if( b_OnNewGameMenu && enum_ButtonSelected == MenuButtonSelected.Button_One )
        {
            enum_ButtonSelected = MenuButtonSelected.Button_Zero;

            Set_ButtonHighlighed(b_OnNewGameMenu, enum_ButtonSelected);
        }
    }

    void Menu_Down()
    {
        if (enum_ButtonSelected == MenuButtonSelected.Button_One)
        {
            enum_ButtonSelected = MenuButtonSelected.Button_Two;

            Set_ButtonHighlighed(b_OnNewGameMenu, enum_ButtonSelected);
        }
        else if (enum_ButtonSelected == MenuButtonSelected.Button_Two)
        {
            enum_ButtonSelected = MenuButtonSelected.Button_Three;

            Set_ButtonHighlighed(b_OnNewGameMenu, enum_ButtonSelected);
        }
        else if( b_OnNewGameMenu && enum_ButtonSelected == MenuButtonSelected.Button_Zero )
        {
            enum_ButtonSelected = MenuButtonSelected.Button_One;

            Set_ButtonHighlighed(b_OnNewGameMenu, enum_ButtonSelected);
        }
    }

    void Menu_Select()
    {
        // Main Menu List
        if(!b_OnNewGameMenu)
        {
            #region New Game -> Reposition buttons, enable b_OnNewGameMenu
            if( enum_ButtonSelected == MenuButtonSelected.Button_One )
            {
                // The normal menu screen is sent off screen
                SetButtonPosition( false, false );

                // The New Game menu screen is sent on screen
                SetButtonPosition( true, true );

                b_OnNewGameMenu = true;
                enum_ButtonSelected = MenuButtonSelected.Button_One;

                Set_ButtonHighlighed( b_OnNewGameMenu, enum_ButtonSelected );
            }
            #endregion

            #region Credits -> Reposition buttons
            else if ( enum_ButtonSelected == MenuButtonSelected.Button_Two )
            {
                if(!CreditsActive)
                {
                    // The normal menu screen is sent off screen
                    SetButtonPosition(false, false);
                    SetButtonPosition(true, false);

                    CreditsActive = true;

                    f_QuitMenuLerpTimer = 1.0f;
                }
                // We disable the credits screen and return to the previous menu
                else
                {
                    CreditsActive = false;

                    // Return the main menu to the screen
                    SetButtonPosition(false, true);
                }
            }
            #endregion

            #region Quit -> Confirm Quit
            else if (enum_ButtonSelected == MenuButtonSelected.Button_Three)
            {
                if(!QuitMenuActive)
                {
                    // The normal menu screen is sent off screen
                    SetButtonPosition(false, false);
                    SetButtonPosition(true, false);

                    // TODO: Quit Confirm
                    QuitMenuActive = true;
                }
                else
                {
                    Application.Quit();
                }
            }
            #endregion
        }
        else
        {
            #region New Game Options
            bool b_2w_2h = false;
            bool b_3w_2h = false;
            bool b_2w_3h = false;
            bool b_3w_3h = false;

            bool b_ThreeBlocks = false;

            int i_BoardWidth = 15;
            int i_BoardHeight = 15;

            int i_DropTimer = 1;

            // Tutoria
            int i_Scene = 3;
            bool b_IsTutorial = false;

            #region Tutorial -> Set game settings, begin game
            if (enum_ButtonSelected == MenuButtonSelected.Button_Zero)
            {
                if (go_GameSettings)
                {
                    // 2x2 only, No 3rd block, 15x15 grid size, 2 second drop delay
                    b_2w_2h = true;
                    i_BoardWidth = 8;
                    i_BoardHeight = 10;
                    i_DropTimer = 3;

                    // Loads the tutorial instead
                    i_Scene = 4;
                    b_IsTutorial = true;
                }
            }
            #endregion

            #region Normal Difficulty -> Set game settings, begin game
            if (enum_ButtonSelected == MenuButtonSelected.Button_One)
            {
                if (go_GameSettings)
                {
                    // 2x2 only, No 3rd block, 15x15 grid size, 2 second drop delay
                    b_2w_2h = true;
                    i_BoardWidth = 12;
                    i_BoardHeight = 10;
                    i_DropTimer = 1;
                }
            }
            #endregion


            #region Hard Difficulty -> Set game settings, begin game
            else if (enum_ButtonSelected == MenuButtonSelected.Button_Two)
            {
                if (go_GameSettings)
                {
                    // Exclude 3w_3h block, No 3rd block, 20x15 grid size, 1 second drop delay
                    b_2w_2h = true;
                    b_2w_3h = true;
                    b_3w_2h = true;

                    i_BoardWidth = 15;
                    i_BoardHeight = 15;

                    i_DropTimer = 1;
                }
            }
            #endregion


            #region Master Difficulty -> Set game settings, begin game
            else if (enum_ButtonSelected == MenuButtonSelected.Button_Three)
            {
                if (go_GameSettings)
                {
                    // Exclude 3w_3h block, Include 3rd block, 20x20 grid size, 1 second drop delay
                    b_2w_2h = true;
                    b_2w_3h = true;
                    b_3w_2h = true;

                    b_ThreeBlocks = true;

                    i_BoardWidth = 15;
                    i_BoardHeight = 15;

                    i_DropTimer = 1;
                }
            }
            #endregion

            go_GameSettings.GetComponent<Cs_MainMenu_GameSettings>().Set_GameSettings( b_2w_2h, b_2w_3h, b_3w_2h, b_3w_3h, b_ThreeBlocks, i_BoardWidth, i_BoardHeight, i_DropTimer, b_IsTutorial );
            SceneManager.LoadScene(i_Scene);
            #endregion
        }

        b_PlayerInputAllowed = false;
    }

    void Menu_Deselect()
    {
        if(b_OnNewGameMenu)
        {
            // Move the Main Menu onto the screen
            SetButtonPosition(false, true);

            // Move the New Game choices off screen
            SetButtonPosition(true, false);

            b_OnNewGameMenu = false;
            enum_ButtonSelected = MenuButtonSelected.Button_One;

            Set_ButtonHighlighed(b_OnNewGameMenu, enum_ButtonSelected);
        }
        else
        {
            if( CreditsActive ) // Or Credits Menu Active
            {
                // We disable the credits screen and return to the previous menu
                CreditsActive = false;

                // Return the main menu to the screen
                SetButtonPosition(false, true);
                // SetButtonPosition(true, false);

                b_OnNewGameMenu = false;
                enum_ButtonSelected = MenuButtonSelected.Button_One;

                Set_ButtonHighlighed(b_OnNewGameMenu, enum_ButtonSelected);
            }
            else if( QuitMenuActive )
            {
                QuitMenuActive = false;

                // Move the Main Menu onto the screen
                SetButtonPosition(false, true);
                SetButtonPosition(true, false);

                b_OnNewGameMenu = false;
                enum_ButtonSelected = MenuButtonSelected.Button_One;

                Set_ButtonHighlighed(b_OnNewGameMenu, enum_ButtonSelected);
            }
        }

        b_PlayerInputAllowed = false;
    }

    bool b_QuitMenuActive;
    float f_QuitMenuLerpTimer;
    GameObject go_QuitMenuObjects;
    Vector3 v3_Position_OffScreen;
    Vector3 v3_Position_OnScreen;
    bool QuitMenuActive
    {
        set
        {
            f_QuitMenuLerpTimer = 0f;

            b_QuitMenuActive = value;
        }
        get { return b_QuitMenuActive; }
    }

    bool b_CreditsActive;
    float f_CreditsLerpTimer;
    GameObject go_CreditsMenuObjects;
    bool CreditsActive
    {
        set
        {
            f_CreditsTimer = 0f;

            b_CreditsActive = value;
        }
        get { return b_CreditsActive; }
    }

    Vector3 v3_MainMenu_PrevLoc;
    Vector3 v3_MainMenu_CurrLoc;
    Vector3 v3_NewGame_PrevLoc;
    Vector3 v3_NewGame_CurrLoc;
    float f_LerpTimer;
    static float f_LerpTimer_Max = 2.0f;
    void SetButtonPosition( bool b_NewGameMenu_, bool b_OnScreen_ )
    {
        // Reset f_LerpTimer
        f_LerpTimer = 0.0f;

        // If this is the New Game Menu
        if(b_NewGameMenu_)
        {
            // Store current location
            v3_NewGame_PrevLoc = go_MenuButtons_2.transform.position;

            // If positioning on screen, set 'On Screen' location
            if( b_OnScreen_ )
            {
                v3_NewGame_CurrLoc = v3_MenuPosition_OnScreen;
            }
            // Otherwise, set the 'Off Screen' location
            else
            {
                v3_NewGame_CurrLoc = v3_MenuPosition_Below;
            }
        }
        // Otherwise, this is the Main Menu button set
        else
        {
            // Store current location
            v3_MainMenu_PrevLoc = go_MenuButtons_1.transform.position;

            // If positioning on screen, set 'On Screen' location
            if( b_OnScreen_ )
            {
                v3_MainMenu_CurrLoc = v3_MenuPosition_OnScreen;
            }
            // Otherwise, set the 'Off Screen' location
            else
            {
                v3_MainMenu_CurrLoc = v3_MenuPosition_Left;
            }
        }
    }

    // Update is called once per frame
    void Update ()
    {
        prevState = state;
        state = GamePad.GetState(pad_PlayerOne);

        #region Player Input
        if(b_PlayerInputAllowed)
        {
            if( (state.ThumbSticks.Left.Y < -0.5f && prevState.ThumbSticks.Left.Y > -0.5f) ||
                (state.DPad.Down == ButtonState.Pressed && prevState.DPad.Down == ButtonState.Released) ||
                Input.GetKeyDown(KeyCode.DownArrow) ||
                Input.GetKeyDown(KeyCode.S))
            {
                Menu_Down();
            }

            if( (state.ThumbSticks.Left.Y > 0.5f && prevState.ThumbSticks.Left.Y < 0.5f) || 
                (state.DPad.Up == ButtonState.Pressed && prevState.DPad.Up == ButtonState.Released) ||
                Input.GetKeyDown(KeyCode.UpArrow) || 
                Input.GetKeyDown(KeyCode.W) )
            {
                Menu_Up();
            }

            if( (state.Buttons.A == ButtonState.Pressed) && (prevState.Buttons.A == ButtonState.Released) ||
                Input.GetKeyDown(KeyCode.Return) ||
                Input.GetKeyDown(KeyCode.Space) )
            {
                Menu_Select();
            }

            if ((state.Buttons.B == ButtonState.Pressed) && (prevState.Buttons.B == ButtonState.Released) ||
                Input.GetKeyDown(KeyCode.Backspace) ||
                Input.GetKeyDown(KeyCode.Escape))
            {
                Menu_Deselect();
            }
        }
        #endregion

        #region Lerp Button Position
        // Lerp button positions
        if (f_LerpTimer < f_LerpTimer_Max)
        {
            // Increment & Clamp
            f_LerpTimer += Time.deltaTime;
            if (f_LerpTimer > f_LerpTimer_Max) f_LerpTimer = f_LerpTimer_Max;

            float f_Perc = animCurve.Evaluate(f_LerpTimer / f_LerpTimer_Max);

            go_MenuButtons_1.transform.position = Vector3.LerpUnclamped(v3_MainMenu_PrevLoc, v3_MainMenu_CurrLoc, f_Perc);
            go_MenuButtons_2.transform.position = Vector3.LerpUnclamped(v3_NewGame_PrevLoc, v3_NewGame_CurrLoc, f_Perc);
        }

        if(f_LerpTimer == f_LerpTimer_Max)
        {
            b_PlayerInputAllowed = true;
        }
        #endregion

        #region Lerp Quit Menu
        if( QuitMenuActive )
        {
            if(f_QuitMenuLerpTimer < 1.0f)
            {
                b_PlayerInputAllowed = false;

                f_QuitMenuLerpTimer += Time.deltaTime;
                if (f_QuitMenuLerpTimer > 1.0f)
                {
                    f_QuitMenuLerpTimer = 1.0f;

                    b_PlayerInputAllowed = true;
                }
            }

            float f_Perc = animCurve.Evaluate(f_QuitMenuLerpTimer);

            Vector3 v3_CurrPos = Vector3.LerpUnclamped(v3_Position_OffScreen, v3_Position_OnScreen, f_Perc);

            go_QuitMenuObjects.transform.position = v3_CurrPos;
        }
        else
        {
            if (f_QuitMenuLerpTimer < 1.0f)
            {
                b_PlayerInputAllowed = false;

                f_QuitMenuLerpTimer += Time.deltaTime;
                if (f_QuitMenuLerpTimer > 1.0f)
                {
                    f_QuitMenuLerpTimer = 1.0f;

                    b_PlayerInputAllowed = true;
                }
            }

            float f_Perc = animCurve.Evaluate(f_QuitMenuLerpTimer);

            Vector3 v3_CurrPos = Vector3.LerpUnclamped(v3_Position_OnScreen, v3_Position_OffScreen, f_Perc);

            go_QuitMenuObjects.transform.position = v3_CurrPos;
        }
        #endregion

        #region Lerp Credits
        if( CreditsActive )
        {
            f_CreditsTimer += Time.deltaTime;

            if (f_CreditsTimer > f_CreditsTimer_Max)
            {
                f_CreditsTimer = 0f;

                Menu_Deselect();
            }

            if (f_CreditsLerpTimer < 1.0f)
            {
                b_PlayerInputAllowed = false;

                f_CreditsLerpTimer += Time.deltaTime;
                if (f_CreditsLerpTimer > 1.0f)
                {
                    f_CreditsLerpTimer = 1.0f;

                    b_PlayerInputAllowed = true;
                }
            }
        }
        else
        {
            if (f_CreditsLerpTimer > 0.0f)
            {
                b_PlayerInputAllowed = false;

                f_CreditsLerpTimer -= Time.deltaTime;
                if (f_CreditsLerpTimer < 0.0f)
                {
                    f_CreditsLerpTimer = 0.0f;

                    b_PlayerInputAllowed = true;
                }
            }
        }

        Color clr_CurrAlpha = go_CreditsMenuObjects.GetComponent<Image>().color;
        clr_CurrAlpha.a = f_CreditsLerpTimer;
        go_CreditsMenuObjects.GetComponent<Image>().color = clr_CurrAlpha;
        #endregion
    }
}
