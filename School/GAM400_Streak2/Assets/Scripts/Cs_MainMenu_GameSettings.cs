using UnityEngine;
using System.Collections;

/*********************************
 * 
 * Copyright DigiPen Institute of Technology 2016
 * 
 * Streak 2 by Christopher Christensen
 * 
 * *******************************/

public struct GameSettings
{
    // Game Settings
    private bool _b_2w_2h;
    private bool _b_2w_3h;
    private bool _b_3w_2h;
    private bool _b_3w_3h;
    private bool _b_ThreeBlocks;
    private int _i_BoardWidth;
    private int _i_BoardHeight;
    private int _i_DropTimer;
    private bool _b_IsTutorial;

    public GameSettings(bool b_2w_2h_, bool b_2w_3h_, bool b_3w_2h_, bool b_3w_3h_, bool b_ThreeBlocks_, int i_BoardWidth_, int i_BoardHeight_, int i_DropTimer_, bool b_IsTutorial_ = false ) : this()
    {
        this._b_2w_2h = b_2w_2h_;
        this._b_2w_3h = b_2w_3h_;
        this._b_3w_2h = b_3w_2h_;
        this._b_3w_3h = b_3w_3h_;

        this._b_ThreeBlocks = b_ThreeBlocks_;

        this._i_BoardWidth = i_BoardWidth_;
        this._i_BoardHeight = i_BoardHeight_;

        this._i_DropTimer = i_DropTimer_;

        this._b_IsTutorial = b_IsTutorial_;
    }

    #region Public Functions
    public bool b_2w_2h
    {
        set { _b_2w_2h = value; }
        get { return _b_2w_2h; }
    }

    public bool b_2w_3h
    {
        set { _b_2w_3h = value; }
        get { return _b_2w_3h; }
    }

    public bool b_3w_2h
    {
        set { _b_3w_2h = value; }
        get { return _b_3w_2h; }
    }

    public bool b_3w_3h
    {
        set { _b_3w_3h = value; }
        get { return _b_3w_3h; }
    }

    public bool b_ThreeBlocks
    {
        set { _b_ThreeBlocks = value; }
        get { return _b_ThreeBlocks; }
    }

    public int i_BoardWidth
    {
        set { _i_BoardWidth = value; }
        get { return _i_BoardWidth; }
    }

    public int i_BoardHeight
    {
        set { _i_BoardHeight = value; }
        get { return _i_BoardHeight; }
    }

    public int i_DropTimer
    {
        set { _i_DropTimer = value; }
        get { return _i_DropTimer; }
    }

    public bool b_IsTutorial
    {
        set { _b_IsTutorial = value; }
        get { return _b_IsTutorial; }
    }

    public override string ToString()
    {
        string s_Text;

        s_Text = "[";
        if (_b_2w_2h) s_Text += "2x2, ";
        if (_b_2w_3h) s_Text += "2x3, ";
        if (_b_3w_2h) s_Text += "3x2, ";
        if (_b_3w_2h) s_Text += "3x3, ";
        if (_b_ThreeBlocks) s_Text += "3 Blocks, "; else s_Text += "2 Blocks, ";
        s_Text += "Width: " + this._i_BoardWidth + ", ";
        s_Text += "Height: " + this._i_BoardHeight + ", ";
        s_Text += "Drop Time: " + this._i_DropTimer;
        s_Text += "]";

        return s_Text;
    }
    #endregion
}

public class Cs_MainMenu_GameSettings : MonoBehaviour
{
    GameSettings gameSettings;

    void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public void Print_GameSettings()
    {
        print(gameSettings.ToString());
    }

    public void Set_GameSettings(bool b_2w_2h_, bool b_2w_3h_, bool b_3w_2h_, bool b_3w_3h_, bool b_ThreeBlocks_, int i_BoardWidth_, int i_BoardHeight_, int i_DropTimer_, bool b_IsTutorial_ = false )
    {
        gameSettings.b_2w_2h = b_2w_2h_;
        gameSettings.b_2w_3h = b_2w_3h_;
        gameSettings.b_3w_2h = b_3w_2h_;
        gameSettings.b_3w_3h = b_3w_3h_;

        gameSettings.b_ThreeBlocks = b_ThreeBlocks_;

        gameSettings.i_BoardWidth = i_BoardWidth_;
        gameSettings.i_BoardHeight = i_BoardHeight_;

        gameSettings.i_DropTimer = i_DropTimer_;

        gameSettings.b_IsTutorial = b_IsTutorial_;
    }

    #region Public functions
    public bool b_2w_2h
    {
        set { gameSettings.b_2w_2h = value; }
        get { return gameSettings.b_2w_2h; }
    }

    public bool b_2w_3h
    {
        set { gameSettings.b_2w_3h = value; }
        get { return gameSettings.b_2w_3h; }
    }

    public bool b_3w_2h
    {
        set { gameSettings.b_3w_2h = value; }
        get { return gameSettings.b_3w_2h; }
    }

    public bool b_3w_3h
    {
        set { gameSettings.b_3w_3h = value; }
        get { return gameSettings.b_3w_3h; }
    }

    public bool b_ThreeBlocks
    {
        set { gameSettings.b_ThreeBlocks = value; }
        get { return gameSettings.b_ThreeBlocks; }
    }

    public int i_BoardWidth
    {
        set { gameSettings.i_BoardWidth = value; }
        get { return gameSettings.i_BoardWidth; }
    }

    public int i_BoardHeight
    {
        set { gameSettings.i_BoardHeight = value; }
        get { return gameSettings.i_BoardHeight; }
    }

    public int i_DropTimer
    {
        set { gameSettings.i_DropTimer = value; }
        get { return gameSettings.i_DropTimer; }
    }

    public bool b_IsTutorial
    {
        set { gameSettings.b_IsTutorial = value; }
        get { return gameSettings.b_IsTutorial; }
    }
    #endregion
}
