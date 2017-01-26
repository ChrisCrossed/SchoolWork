using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/*********************************
 * 
 * Copyright DigiPen Institute of Technology 2016
 * 
 * Streak 2 by Christopher Christensen
 * 
 * *******************************/

public enum Enum_BlockType
{
    Block_1_Static = 10,
    Block_1_Active = 15,
    Block_2_Static = 20,
    Block_2_Active = 25,
    Block_3_Static = 30,
    Block_3_Active = 35,
    Empty = 00
}
public enum Enum_BlockSize
{
    size_2w_2h,
    size_2w_3h,
    size_3w_2h,
    size_3w_3h
}
public enum Enum_PauseEffect
{
    StartGame,
    Unpause,
    ScoreLine,
    GameOver
}
enum Enum_WhiteBlockChance
{
    OneInThree,
    OneInFive,
    OneInSeven,
    OneInNine,
    OneInEleven,
}

#region Tools
public enum Enum_Direction
{
    Right,
    Down,
    Up,
    Left
}

public struct IntVector2
{
    private int _x;
    private int _y;

    public IntVector2(int x, int y) : this()
    {
        this._x = x;
        this._y = y;
    }

    public int x
    {
        set { _x = value; }
        get { return _x; }
    }

    public int y
    {
        set { _y = value; }
        get { return _y; }
    }

    public override string ToString()
    {
        return string.Format("[" + this._x + ", " + this._y + "]");
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }

    // Reference: http://stackoverflow.com/questions/15199026/comparing-two-structs-using
    public static bool operator ==(IntVector2 iv2_a_, IntVector2 iv2_b_)
    {
        return iv2_a_.Equals(iv2_b_);
    }

    public static bool operator !=(IntVector2 iv2_a_, IntVector2 iv2_b_)
    {
        return !iv2_a_.Equals(iv2_b_);
    }
}
#endregion

public class Cs_BoardLogic : MonoBehaviour
{
    GameObject go_GameSettings;

    Enum_PauseEffect e_PauseEffect = Enum_PauseEffect.StartGame;

    bool b_DemoGame_InputReceived = false;

    int i_Score;
    float f_TimeToDrop = -3f;

    Enum_BlockType[,] BlockArray;

    // Current Active Block Information
    Vector2 v2_ActiveBlockLocation;
    Enum_BlockSize e_BlockSize;
    [SerializeField] Enum_WhiteBlockChance e_WhiteBlockChance = Enum_WhiteBlockChance.OneInSeven;
    int i_WhiteBlockChance;
    Enum_BlockType[] e_NextBlockList = new Enum_BlockType[27];

    // Game Settings
    [SerializeField] bool b_2w_2h_Allowed = true;
    [SerializeField] bool b_2w_3h_Allowed = true;
    [SerializeField] bool b_3w_2h_Allowed = true;
    [SerializeField] bool b_3w_3h_Allowed = true;
    [SerializeField] bool b_ThreeBlockColors = false;
    [SerializeField] [Range(5, 20)] int i_ArrayWidth;
    [SerializeField] [Range(5, 20)] int i_ArrayHeight;
    [Range(-1, 5)] [SerializeField] int i_TimeToDrop_Max = 3;
    
    float f_StartGameTimer = 2.0f;

    float f_GameOver_FadeOut_Timer;
    GameObject go_FadeOut;

    GameObject go_SFX;
    AudioClip sfx_ScoreEffect;

    // Used to begin gameplay
    public void Init_Gameplay()
    {
        // print("************** DID THE THING **************");
    }

    void Awake()
    {
        // Find GameSettings Object and pull in information
        if (GameObject.Find("GameSettings"))
        {
            go_GameSettings = GameObject.Find("GameSettings");

            Cs_MainMenu_GameSettings gameSettings = go_GameSettings.GetComponent<Cs_MainMenu_GameSettings>();

            b_2w_2h_Allowed = gameSettings.b_2w_2h;
            b_2w_3h_Allowed = gameSettings.b_2w_3h;
            b_3w_2h_Allowed = gameSettings.b_3w_2h;
            b_3w_3h_Allowed = gameSettings.b_3w_3h;

            b_ThreeBlockColors = gameSettings.b_ThreeBlocks;

            i_ArrayWidth = gameSettings.i_BoardWidth;
            i_ArrayHeight = gameSettings.i_BoardHeight;

            i_TimeToDrop_Max = gameSettings.i_DropTimer;
        }
    }

	// Use this for initialization
	void Start ()
    {
        // Initialization
        go_FadeOut = GameObject.Find("FadeOut");
        go_SFX = GameObject.Find("AudioSource_SFX");
        sfx_ScoreEffect = Resources.Load("SFX_Score") as AudioClip;

        Enum_BlockSize e_MaxBlockSize = Enum_BlockSize.size_2w_2h;
        if(  b_3w_3h_Allowed ||
            (b_2w_3h_Allowed && b_3w_2h_Allowed) )
        {
            e_MaxBlockSize = Enum_BlockSize.size_3w_3h;
        }
        else
        {
            if(b_3w_2h_Allowed)
            {
                e_MaxBlockSize = Enum_BlockSize.size_3w_2h;
            }
            else if(b_2w_3h_Allowed)
            {
                e_MaxBlockSize = Enum_BlockSize.size_2w_3h;
            }
        }

        // Initialize Board
        GameObject.Find("BoardDisplay").GetComponent<Cs_BoardDisplay>().Init_Board( i_ArrayWidth, i_ArrayHeight, 0, e_MaxBlockSize );

        // Set i_TimeToDrop_Max to be -1 if it starts at 0, to make sure we aren't dropping infinitely
        if (i_TimeToDrop_Max == 0) i_TimeToDrop_Max = -1;

        #region Set First Block to Random Size
            // Determine the size of the next block to use
        bool b_FoundNextBlock = false;
        // While we haven't found the next block, loop
        while (!b_FoundNextBlock)
        {
            int i_RandBlock = Random.Range(0, 4);

            if (i_RandBlock == 0 && b_2w_2h_Allowed)
            {
                e_NextBlockSize = Enum_BlockSize.size_2w_2h;
                b_FoundNextBlock = true;
            }
            else if (i_RandBlock == 1 && b_2w_3h_Allowed)
            {
                e_NextBlockSize = Enum_BlockSize.size_2w_3h;
                b_FoundNextBlock = true;
            }
            else if (i_RandBlock == 2 && b_3w_2h_Allowed)
            {
                e_NextBlockSize = Enum_BlockSize.size_3w_2h;
                b_FoundNextBlock = true;
            }
            else if (i_RandBlock == 3 && b_3w_3h_Allowed)
            {
                e_NextBlockSize = Enum_BlockSize.size_3w_3h;
                b_FoundNextBlock = true;
            }
        }
        // Set random initial block
        e_BlockSize = e_NextBlockSize;
        #endregion

        // Initialize NextBlockList
        for(int i_ = 0; i_ < e_NextBlockList.Length; ++i_)
        {
            e_NextBlockList[i_] = Enum_BlockType.Empty;
        }
        PopulateNextBlockList();

        // Initialize block array
        BlockArray = new Enum_BlockType[i_ArrayHeight, i_ArrayWidth];
        Initialize_BlockArray();
        
        // PrintArrayToConsole();
    }

    #region Block Creation
    Enum_BlockSize e_NextBlockSize; // Choose the size of the next random block to put on the screen
    // Fills e_NextBlockList with random blocks to push into CreateNewBlock
    void PopulateNextBlockList()
    {
        // Find the first unpopulated position
        int i_FirstOpenPosition = 0;
        for(int j_ = 0; j_ < e_NextBlockList.Length; ++j_)
        {
            if (e_NextBlockList[j_] == Enum_BlockType.Empty) continue;
            else i_FirstOpenPosition = j_;
        }

        // Changing the system. 0/1/2 = Block 1, 3/4/5 = Block 2, 6 = Block 3.
        // If 'b_ThreeBlockColors', a 1-in-7 chance to have a black block
        int i_NumBlockTypes = 2;

        if(b_ThreeBlockColors)
        {
            if(e_WhiteBlockChance == Enum_WhiteBlockChance.OneInThree)      i_NumBlockTypes = 3;
            else if (e_WhiteBlockChance == Enum_WhiteBlockChance.OneInFive) i_NumBlockTypes = 5;
            else if (e_WhiteBlockChance == Enum_WhiteBlockChance.OneInSeven) i_NumBlockTypes = 7;
            else if (e_WhiteBlockChance == Enum_WhiteBlockChance.OneInNine) i_NumBlockTypes = 9;
            else if (e_WhiteBlockChance == Enum_WhiteBlockChance.OneInEleven) i_NumBlockTypes = 11;
        }

        // Start from the first open position & populate all remaining positions
        for(int i_ = i_FirstOpenPosition; i_ < e_NextBlockList.Length; ++i_)
        {
            // Find a random block
            int i_RandBlock = Random.Range(0, i_NumBlockTypes);

            // Block 'One'
            if( i_RandBlock < i_NumBlockTypes / 2 )
            {
                e_NextBlockList[i_] = Enum_BlockType.Block_1_Active;
            }
            // Block 'Two'
            else if( i_RandBlock >= i_NumBlockTypes / 2 && i_RandBlock != i_NumBlockTypes - 1 )
            {
                e_NextBlockList[i_] = Enum_BlockType.Block_2_Active;
            }
            // Block 'Three'
            else
            {
                if(b_ThreeBlockColors)
                {
                    e_NextBlockList[i_] = Enum_BlockType.Block_3_Active;
                }
                // Special probability case since we need to hit a 0 or 1 if we're not checking for three block types
                else
                {
                    e_NextBlockList[i_] = Enum_BlockType.Block_2_Active;
                }
            }
        }

        // Determine the size of the next block to use
        bool b_FoundNextBlock = false;
        // While we haven't found the next block, loop
        while(!b_FoundNextBlock)
        {
            int i_RandBlock = Random.Range(0, 4);

            if(i_RandBlock == 0 && b_2w_2h_Allowed)
            {
                e_NextBlockSize = Enum_BlockSize.size_2w_2h;
                b_FoundNextBlock = true;
            }
            else if (i_RandBlock == 1 && b_2w_3h_Allowed)
            {
                e_NextBlockSize = Enum_BlockSize.size_2w_3h;
                b_FoundNextBlock = true;
            }
            else if (i_RandBlock == 2 && b_3w_2h_Allowed)
            {
                e_NextBlockSize = Enum_BlockSize.size_3w_2h;
                b_FoundNextBlock = true;
            }
            else if (i_RandBlock == 3 && b_3w_3h_Allowed)
            {
                e_NextBlockSize = Enum_BlockSize.size_3w_3h;
                b_FoundNextBlock = true;
            }
        }
    }

    void CreateNewBlock()
    {
        // Manually create a set of new blocks in the proper location
        int i_NumToShift = 0;

        // Find the 'X' location to set the block location (2 wide)
        if(e_BlockSize == Enum_BlockSize.size_2w_2h || e_BlockSize == Enum_BlockSize.size_2w_3h)
        {
            // Finds the center of the List width
            v2_ActiveBlockLocation.x = (int)((i_ArrayWidth - 1) / 2);
        }
        else // (3 high)
        {
            // Finds the center of the List width, and shifts to the left one space
            v2_ActiveBlockLocation.x = (int)((i_ArrayWidth - 1) / 2) - 1;
        }

        // Find the 'Y' location to set the block location (2 high)
        if (e_BlockSize == Enum_BlockSize.size_2w_2h || e_BlockSize == Enum_BlockSize.size_3w_2h)
        {
            v2_ActiveBlockLocation.y = i_ArrayHeight - 2;
        }
        // (3 high)
        else v2_ActiveBlockLocation.y = i_ArrayHeight - 3;

        // Check for GameOver based on new ActiveBlockLocation information. Returns true if the game's over.
        if (GameOverCheck())
        {
            // PauseState is set to Game Over
            e_PauseEffect = Enum_PauseEffect.GameOver;

            return;
        }

        // Set the number of blocks to shift afterward
        if (e_BlockSize == Enum_BlockSize.size_2w_2h)                                                   i_NumToShift = 4;
        else if (e_BlockSize == Enum_BlockSize.size_2w_3h || e_BlockSize == Enum_BlockSize.size_3w_2h)  i_NumToShift = 6;
        else if (e_BlockSize == Enum_BlockSize.size_3w_3h)                                              i_NumToShift = 9;

        // Create temp block list
        int i_NewBlocks_Width = 2;
        int i_NewBlocks_Height = 2;

        if (e_BlockSize == Enum_BlockSize.size_3w_2h || e_BlockSize == Enum_BlockSize.size_3w_3h) i_NewBlocks_Width = 3;
        if (e_BlockSize == Enum_BlockSize.size_2w_3h || e_BlockSize == Enum_BlockSize.size_3w_3h) i_NewBlocks_Height = 3;

        // print("BLOCK SIZE: " + e_BlockSize);
        Enum_BlockType[,] e_SmallList = new Enum_BlockType[i_NewBlocks_Height, i_NewBlocks_Width];

        // No matter what, set the initial 2x2
        SetBlock(v2_ActiveBlockLocation,                                            e_NextBlockList[0]);
        e_SmallList[0, 0] = e_NextBlockList[0];

        SetBlock(new Vector2(v2_ActiveBlockLocation.x + 1, v2_ActiveBlockLocation.y + 0),   e_NextBlockList[1]);
        e_SmallList[0, 1] = e_NextBlockList[1];

        SetBlock(new Vector2(v2_ActiveBlockLocation.x + 0, v2_ActiveBlockLocation.y + 1),   e_NextBlockList[2]);
        e_SmallList[1, 0] = e_NextBlockList[2];

        SetBlock(new Vector2(v2_ActiveBlockLocation.x + 1, v2_ActiveBlockLocation.y + 1),   e_NextBlockList[3]);
        e_SmallList[1, 1] = e_NextBlockList[3];

        // If we're specifically 3x2, set those positions
        if ( e_BlockSize == Enum_BlockSize.size_3w_2h )
        {
            SetBlock(new Vector2(v2_ActiveBlockLocation.x + 2, v2_ActiveBlockLocation.y + 1), e_NextBlockList[4]);
            e_SmallList[1, 2] = e_NextBlockList[4];

            SetBlock(new Vector2(v2_ActiveBlockLocation.x + 2, v2_ActiveBlockLocation.y + 0), e_NextBlockList[5]);
            e_SmallList[0, 2] = e_NextBlockList[5];
        }
        // If we're specifically 2x3, set those positions
        else if( e_BlockSize == Enum_BlockSize.size_2w_3h)
        {
            SetBlock(new Vector2(v2_ActiveBlockLocation.x + 0, v2_ActiveBlockLocation.y + 2), e_NextBlockList[4]);
            e_SmallList[2, 0] = e_NextBlockList[4];

            SetBlock(new Vector2(v2_ActiveBlockLocation.x + 1, v2_ActiveBlockLocation.y + 2), e_NextBlockList[5]);
            e_SmallList[2, 1] = e_NextBlockList[5];
        }
        else if( e_BlockSize == Enum_BlockSize.size_3w_3h )
        {
            SetBlock(new Vector2(v2_ActiveBlockLocation.x + 2, v2_ActiveBlockLocation.y + 1), e_NextBlockList[4]);
            e_SmallList[1, 2] = e_NextBlockList[4];

            SetBlock(new Vector2(v2_ActiveBlockLocation.x + 2, v2_ActiveBlockLocation.y + 0), e_NextBlockList[5]);
            e_SmallList[0, 2] = e_NextBlockList[5];

            SetBlock(new Vector2(v2_ActiveBlockLocation.x + 0, v2_ActiveBlockLocation.y + 2), e_NextBlockList[6]);
            e_SmallList[2, 0] = e_NextBlockList[6];

            SetBlock(new Vector2(v2_ActiveBlockLocation.x + 1, v2_ActiveBlockLocation.y + 2), e_NextBlockList[7]);
            e_SmallList[2, 1] = e_NextBlockList[7];

            SetBlock(new Vector2(v2_ActiveBlockLocation.x + 2, v2_ActiveBlockLocation.y + 2), e_NextBlockList[8]);
            e_SmallList[2, 2] = e_NextBlockList[8];
        }

        #region Send e_SmallList to Board Display
        GameObject.Find("BoardDisplay").GetComponent<Cs_BoardDisplay>().Set_NewBlocks(e_SmallList, e_BlockSize, new IntVector2((int)v2_ActiveBlockLocation.x, (int)v2_ActiveBlockLocation.y));
        #endregion

        #region Send Set_ShowPotentialBlockVisual Information
        PotentialBlockVisual();
        #endregion

        ShiftNewBlockList( i_NumToShift );
    }

    void PotentialBlockVisual()
    {
        // Clear Backdrops before positioning new stuff
        // GameObject.Find("BoardDisplay").GetComponent<Cs_BoardDisplay>().Set_ClearBackdrops();

        // Reset SmallList_Vert
        int i_BlockHeight = 2;
        if (e_BlockSize == Enum_BlockSize.size_2w_3h || e_BlockSize == Enum_BlockSize.size_3w_3h) i_BlockHeight = 3;

        int i_BlockWidth = 2;
        if (e_BlockSize == Enum_BlockSize.size_3w_2h || e_BlockSize == Enum_BlockSize.size_3w_3h) i_BlockWidth = 3;

        Enum_BlockType[] e_SmallList_Vert = new Enum_BlockType[i_BlockHeight];

        // Start with the ActiveBlockLocation bottom left and cycle upward
        for(int x_ = 0; x_ < i_BlockWidth; ++x_)
        {
            for(int y_ = 0; y_ < i_BlockHeight; ++y_)
            {
                // When the active blocks reach the bottom, the blocks used to apply the visual blocks above the active blocks. This prevents that.
                if((int)v2_ActiveBlockLocation.y > 0)
                {
                    e_SmallList_Vert[y_] = GetBlock((int)v2_ActiveBlockLocation.x + x_, (int)v2_ActiveBlockLocation.y + y_);
                }
            }
            
            // Send to Set_ShowPotentialBlockVisual
            GameObject.Find("BoardDisplay").GetComponent<Cs_BoardDisplay>().Set_ShowPotentialBlockVisual((int)v2_ActiveBlockLocation.x + x_, (int)v2_ActiveBlockLocation.y, e_SmallList_Vert);
        }
    }

    // Used within 'CreateNewBlock' to manipulate the Next Block List
    void ShiftNewBlockList( int i_NumToShift_ )
    {
        // Shift the blocks at the i_NumToShift_ positions down
        for(int i_ = 0; i_ < (e_NextBlockList.Length - i_NumToShift_); ++i_)
        {
            e_NextBlockList[i_] = e_NextBlockList[i_ + i_NumToShift_];
        }

        // Set the final positions to empty
        for(int j_ = (e_NextBlockList.Length - i_NumToShift_); j_ < e_NextBlockList.Length; ++ j_)
        {
            e_NextBlockList[j_] = Enum_BlockType.Empty;
        }

        // Re-populate the list
        PopulateNextBlockList();
    }

    void Initialize_BlockArray()
    {
        for(int y = 0; y < i_ArrayHeight; ++y)
        {
            for (int x = 0; x < i_ArrayWidth; ++x)
            {
                SetBlock(x, y, Enum_BlockType.Empty);
            }
        }
    }
    #endregion

    #region Block Position Manipulation
    // Complete
    void MoveActiveBlocks_Down(Vector2 v2_BottomLeft, Enum_BlockSize BlockSize_)
    {
        #region Ensure appropriate spaces below are empty. Otherwise, convert all blocks to static.
        if (v2_BottomLeft.y - 1 < 0) { AllBlocksStatic(); return; }

        if (GetBlock((int)v2_BottomLeft.x, (int)v2_BottomLeft.y - 1) != Enum_BlockType.Empty) { AllBlocksStatic(); return; }
        if (GetBlock((int)v2_BottomLeft.x + 1, (int)v2_BottomLeft.y - 1) != Enum_BlockType.Empty) { AllBlocksStatic(); return; }

        if (BlockSize_ == Enum_BlockSize.size_3w_2h || BlockSize_ == Enum_BlockSize.size_3w_3h)
        {
            if (GetBlock((int)v2_BottomLeft.x + 2, (int)v2_BottomLeft.y - 1) != Enum_BlockType.Empty) { AllBlocksStatic(); return; }
        }
        #endregion

        GameObject.Find("BoardDisplay").GetComponent<Cs_BoardDisplay>().Set_ClearBackdrops();

        #region Default 2x2
        // (0,0) -> (0, -1)
        SetBlock((int)v2_BottomLeft.x, (int)v2_BottomLeft.y - 1, GetBlock((int)v2_BottomLeft.x, (int)v2_BottomLeft.y));

        // (1, 0) -> (0, -1)
        SetBlock((int)v2_BottomLeft.x + 1, (int)v2_BottomLeft.y - 1, GetBlock((int)v2_BottomLeft.x + 1, (int)v2_BottomLeft.y));

        // (0, 1) -> (0, 0)
        SetBlock((int)v2_BottomLeft.x, (int)v2_BottomLeft.y, GetBlock((int)v2_BottomLeft.x, (int)v2_BottomLeft.y + 1));

        // (1,1) -> (1, 0)
        SetBlock((int)v2_BottomLeft.x + 1, (int)v2_BottomLeft.y, GetBlock((int)v2_BottomLeft.x + 1, (int)v2_BottomLeft.y + 1));

        if(BlockSize_ == Enum_BlockSize.size_2w_2h)
        {
            // (0,1) -> CLEAR
            SetBlock((int)v2_BottomLeft.x, (int)v2_BottomLeft.y + 1, Enum_BlockType.Empty);

            // (1,1) -> CLEAR
            SetBlock((int)v2_BottomLeft.x + 1, (int)v2_BottomLeft.y + 1, Enum_BlockType.Empty);
        }
        #endregion

        #region 3 wide by 2 tall
        if(BlockSize_ == Enum_BlockSize.size_3w_2h || BlockSize_ == Enum_BlockSize.size_3w_3h)
        {
            // (2, 0) -> (2, -1)
            SetBlock((int)v2_BottomLeft.x + 2, (int)v2_BottomLeft.y - 1, GetBlock((int)v2_BottomLeft.x + 2, (int)v2_BottomLeft.y));

            // (2, 1) -> (2, 0)
            SetBlock((int)v2_BottomLeft.x + 2, (int)v2_BottomLeft.y    , GetBlock((int)v2_BottomLeft.x + 2, (int)v2_BottomLeft.y + 1));

            // If 3x2:
            if (BlockSize_ == Enum_BlockSize.size_3w_2h)
            {
                // (0,1) -> CLEAR
                SetBlock((int)v2_BottomLeft.x    , (int)v2_BottomLeft.y + 1, Enum_BlockType.Empty);

                // (1,1) -> CLEAR
                SetBlock((int)v2_BottomLeft.x + 1, (int)v2_BottomLeft.y + 1, Enum_BlockType.Empty);

                // (2,1) -> CLEAR
                SetBlock((int)v2_BottomLeft.x + 2, (int)v2_BottomLeft.y + 1, Enum_BlockType.Empty);
            }
        }
        #endregion

        #region 2 wide by 3 tall
        if (BlockSize_ == Enum_BlockSize.size_2w_3h || BlockSize_ == Enum_BlockSize.size_3w_3h)
        {
            // (0,2) -> (0, 1) 
            SetBlock((int)v2_BottomLeft.x, (int)v2_BottomLeft.y + 1, GetBlock((int)v2_BottomLeft.x, (int)v2_BottomLeft.y + 2));

            // (1,2) -> (1, 1)
            SetBlock((int)v2_BottomLeft.x + 1, (int)v2_BottomLeft.y + 1, GetBlock((int)v2_BottomLeft.x + 1, (int)v2_BottomLeft.y + 2));

            // (0,2) -> CLEAR
            SetBlock((int)v2_BottomLeft.x, (int)v2_BottomLeft.y + 2, Enum_BlockType.Empty);

            // (1,2) -> CLEAR
            SetBlock((int)v2_BottomLeft.x + 1, (int)v2_BottomLeft.y + 2, Enum_BlockType.Empty);
        }
        #endregion

        #region 3 wide by 3 tall
        if (BlockSize_ == Enum_BlockSize.size_3w_3h)
        {
            // (2,2) -> (2,1)
            SetBlock((int)v2_BottomLeft.x + 2, (int)v2_BottomLeft.y + 1, GetBlock((int)v2_BottomLeft.x + 2, (int)v2_BottomLeft.y + 2));

            // (2,2) -> CLEAR
            SetBlock((int)v2_BottomLeft.x + 2, (int)v2_BottomLeft.y + 2, Enum_BlockType.Empty);
        }
        #endregion

        #region Send movement to BoardDisplay
        GameObject.Find("BoardDisplay").GetComponent<Cs_BoardDisplay>().ShiftBlocks(Enum_Direction.Down, e_BlockSize, v2_BottomLeft);
        #endregion

        // Move the CurrentBlockLocation 'y'
        --v2_ActiveBlockLocation.y;

        // Show new block drop positions
        PotentialBlockVisual();
    }

    // Complete
    void MoveActiveBlocks_Left(Vector2 v2_BottomLeft, Enum_BlockSize e_BlockSize_)
    {
        #region Ensure appropriate spaces below are empty. Otherwise, do not do anything.
        if (v2_BottomLeft.x - 1 < 0) { return; }

        // As for block repositioning, I do not want blocks to be pushed around if another block exists.
        if (GetBlock((int)v2_BottomLeft.x - 1, (int)v2_BottomLeft.y + 0) != Enum_BlockType.Empty) { return; }
        if (GetBlock((int)v2_BottomLeft.x - 1, (int)v2_BottomLeft.y + 1) != Enum_BlockType.Empty) { return; }

        // Check in case the active blocks we have are 3 high
        if(e_BlockSize_ == Enum_BlockSize.size_2w_3h || e_BlockSize_ == Enum_BlockSize.size_3w_3h)
        {
            if (GetBlock((int)v2_BottomLeft.x - 1, (int)v2_BottomLeft.y + 2) != Enum_BlockType.Empty) { return; }
        }
        #endregion

        GameObject.Find("BoardDisplay").GetComponent<Cs_BoardDisplay>().Set_ClearBackdrops();

        #region Default 2x2
        // (0,0) -> (-1, 0)
        SetBlock((int)v2_BottomLeft.x - 1, (int)v2_BottomLeft.y + 0, GetBlock((int)v2_BottomLeft.x, (int)v2_BottomLeft.y));

        // (1, 0) -> (0, 0)
        SetBlock((int)v2_BottomLeft.x + 0, (int)v2_BottomLeft.y + 0, GetBlock((int)v2_BottomLeft.x + 1, (int)v2_BottomLeft.y));

        // (0, 1) -> (-1, 1)
        SetBlock((int)v2_BottomLeft.x - 1, (int)v2_BottomLeft.y + 1, GetBlock((int)v2_BottomLeft.x, (int)v2_BottomLeft.y + 1));

        // (1, 1) -> (0, 1)
        SetBlock((int)v2_BottomLeft.x, (int)v2_BottomLeft.y + 1, GetBlock((int)v2_BottomLeft.x + 1, (int)v2_BottomLeft.y + 1));

        if (e_BlockSize_ == Enum_BlockSize.size_2w_2h)
        {
            // (1,0) -> CLEAR
            SetBlock((int)v2_BottomLeft.x + 1, (int)v2_BottomLeft.y, Enum_BlockType.Empty);

            // (1,1) -> CLEAR
            SetBlock((int)v2_BottomLeft.x + 1, (int)v2_BottomLeft.y + 1, Enum_BlockType.Empty);
        }
        #endregion

        #region 3 wide by 2 tall
        if (e_BlockSize_ == Enum_BlockSize.size_3w_2h || e_BlockSize_ == Enum_BlockSize.size_3w_3h)
        {
            // (2, 0) -> (1, 0)
            SetBlock((int)v2_BottomLeft.x + 1, (int)v2_BottomLeft.y, GetBlock((int)v2_BottomLeft.x + 2, (int)v2_BottomLeft.y));

            // (2, 1) -> (1, 1)
            SetBlock((int)v2_BottomLeft.x + 1, (int)v2_BottomLeft.y + 1, GetBlock((int)v2_BottomLeft.x + 2, (int)v2_BottomLeft.y + 1));
            
            // (2,0) -> CLEAR
            SetBlock((int)v2_BottomLeft.x + 2, (int)v2_BottomLeft.y, Enum_BlockType.Empty);

            // (2,1) -> CLEAR
            SetBlock((int)v2_BottomLeft.x + 2, (int)v2_BottomLeft.y + 1, Enum_BlockType.Empty);
        }
        #endregion

        #region 2 wide by 3 tall
        if (e_BlockSize_ == Enum_BlockSize.size_2w_3h || e_BlockSize_ == Enum_BlockSize.size_3w_3h)
        {
            // (0,2) -> (-1, 2) 
            SetBlock((int)v2_BottomLeft.x - 1, (int)v2_BottomLeft.y + 2, GetBlock((int)v2_BottomLeft.x, (int)v2_BottomLeft.y + 2));

            // (1,2) -> (0, 2)
            SetBlock((int)v2_BottomLeft.x, (int)v2_BottomLeft.y + 2, GetBlock((int)v2_BottomLeft.x + 1, (int)v2_BottomLeft.y + 2));

            // If 2x3: 
            if(e_BlockSize_ == Enum_BlockSize.size_2w_3h)
            {
                // (1,0) -> CLEAR
                SetBlock((int)v2_BottomLeft.x + 1, (int)v2_BottomLeft.y, Enum_BlockType.Empty);

                // (1,1) -> CLEAR
                SetBlock((int)v2_BottomLeft.x + 1, (int)v2_BottomLeft.y + 1, Enum_BlockType.Empty);

                // (1,2) -> CLEAR
                SetBlock((int)v2_BottomLeft.x + 1, (int)v2_BottomLeft.y + 2, Enum_BlockType.Empty);
            }
        }
        #endregion

        #region 3 wide by 3 tall
        if (e_BlockSize_ == Enum_BlockSize.size_3w_3h)
        {
            // (2,2) -> (1,2)
            SetBlock((int)v2_BottomLeft.x + 1, (int)v2_BottomLeft.y + 2, GetBlock((int)v2_BottomLeft.x + 2, (int)v2_BottomLeft.y + 2));

            // (2,2) -> CLEAR
            SetBlock((int)v2_BottomLeft.x + 2, (int)v2_BottomLeft.y + 2, Enum_BlockType.Empty);
        }
        #endregion

        #region Send movement to BoardDisplay
        GameObject.Find("BoardDisplay").GetComponent<Cs_BoardDisplay>().ShiftBlocks(Enum_Direction.Left, e_BlockSize, v2_BottomLeft);
        #endregion

        --v2_ActiveBlockLocation.x;
        
        // Show new block drop positions
        PotentialBlockVisual();
    }

    // TODO: Fix & Complete
    void MoveActiveBlocks_Right(Vector2 v2_BottomLeft, Enum_BlockSize BlockSize_)
    {
        // Check two spaces over from the bottom left block
        // Return check. Performs no operation if we're outside the size of the grid
        if (BlockSize_ == Enum_BlockSize.size_2w_2h || BlockSize_ == Enum_BlockSize.size_2w_3h)
        {
            // If the position to the right of the block doesn't exist, quit out.
            if (v2_BottomLeft.x + 2 >= i_ArrayWidth) { return; }
            // if (v2_BottomLeft.x + 2 > ) { return; }


            // If the position to the right of the block isn't empty, quit out
            if (GetBlock( (int)v2_BottomLeft.x + 2, (int)v2_BottomLeft.y    ) != Enum_BlockType.Empty) { return; }
            if (GetBlock( (int)v2_BottomLeft.x + 2, (int)v2_BottomLeft.y + 1) != Enum_BlockType.Empty) { return; }
        }
        else // Block is 3 wide, not 2
        {
            // If the position to the right of the block doesn't exist, quit out.
            if (v2_BottomLeft.x + 3 >= i_ArrayWidth) { return; }

            // If the position to the right of the block isn't empty, quit out
            if (GetBlock((int)v2_BottomLeft.x + 3, (int)v2_BottomLeft.y) != Enum_BlockType.Empty) { return; }
            if (GetBlock((int)v2_BottomLeft.x + 3, (int)v2_BottomLeft.y + 1) != Enum_BlockType.Empty) { return; }
        }

        GameObject.Find("BoardDisplay").GetComponent<Cs_BoardDisplay>().Set_ClearBackdrops();

        #region Run the check from the top right, backward
        if (BlockSize_ == Enum_BlockSize.size_3w_3h)
        {
            // 2, 2 -> 3, 2
            SetBlock((int)v2_BottomLeft.x + 3, (int)v2_BottomLeft.y + 2, GetBlock((int)v2_BottomLeft.x + 2, (int)v2_BottomLeft.y + 2));
        }

        if(BlockSize_ == Enum_BlockSize.size_2w_3h || BlockSize_ == Enum_BlockSize.size_3w_3h)
        {
            // 1, 2 -> 2, 2
            SetBlock((int)v2_BottomLeft.x + 2, (int)v2_BottomLeft.y + 2, GetBlock((int)v2_BottomLeft.x + 1, (int)v2_BottomLeft.y + 2));

            // 0, 2 -> 1, 2
            SetBlock((int)v2_BottomLeft.x + 1, (int)v2_BottomLeft.y + 2, GetBlock((int)v2_BottomLeft.x + 0, (int)v2_BottomLeft.y + 2));

            // 0, 2 -> CLEAR
            SetBlock((int)v2_BottomLeft.x + 0, (int)v2_BottomLeft.y + 2, Enum_BlockType.Empty);
        }

        if (BlockSize_ == Enum_BlockSize.size_3w_2h || BlockSize_ == Enum_BlockSize.size_3w_3h)
        {
            // 2, 1 -> 3, 1
            SetBlock((int)v2_BottomLeft.x + 3, (int)v2_BottomLeft.y + 1, GetBlock((int)v2_BottomLeft.x + 2, (int)v2_BottomLeft.y + 1));

            // 2, 0 -> 3, 0
            SetBlock((int)v2_BottomLeft.x + 3, (int)v2_BottomLeft.y + 0, GetBlock((int)v2_BottomLeft.x + 2, (int)v2_BottomLeft.y + 0));
        }

        // Default (2x2 Block) - Remember, reverse order. Right to Left.
        // 1, 1 -> 2, 1
        SetBlock((int)v2_BottomLeft.x + 2, (int)v2_BottomLeft.y + 1, GetBlock((int)v2_BottomLeft.x + 1, (int)v2_BottomLeft.y + 1));

        // 0, 1 -> 1, 1
        SetBlock((int)v2_BottomLeft.x + 1, (int)v2_BottomLeft.y + 1, GetBlock((int)v2_BottomLeft.x + 0, (int)v2_BottomLeft.y + 1));

        // 0, 1 -> CLEAR
        SetBlock((int)v2_BottomLeft.x + 0, (int)v2_BottomLeft.y + 1, Enum_BlockType.Empty);

        // 1, 0 -> 2, 0
        SetBlock((int)v2_BottomLeft.x + 2, (int)v2_BottomLeft.y + 0, GetBlock((int)v2_BottomLeft.x + 1, (int)v2_BottomLeft.y + 0));

        // 0, 0 -> 1, 0
        SetBlock((int)v2_BottomLeft.x + 1, (int)v2_BottomLeft.y + 0, GetBlock((int)v2_BottomLeft.x + 0, (int)v2_BottomLeft.y + 0));

        // 0, 0 -> CLEAR
        SetBlock((int)v2_BottomLeft.x + 0, (int)v2_BottomLeft.y + 0, Enum_BlockType.Empty);

        #endregion

        #region Send movement to BoardDisplay
        GameObject.Find("BoardDisplay").GetComponent<Cs_BoardDisplay>().ShiftBlocks(Enum_Direction.Right, e_BlockSize, v2_BottomLeft);
        #endregion

        ++v2_ActiveBlockLocation.x;

        // Show new block drop positions
        PotentialBlockVisual();
    }

    // Complete
    void RotateBlocks_Clockwise(Vector2 v2_BottomLeft, Enum_BlockSize BlockSize_)
    {
        GameObject.Find("BoardDisplay").GetComponent<Cs_BoardDisplay>().Set_ClearBackdrops();

        // Store Bottom Left (Temporary)
        Enum_BlockType e_TempBlockType_ = GetBlock((int)v2_BottomLeft.x, (int)v2_BottomLeft.y);

        // Bottom Left -> Bottom Right
        SetBlock((int)v2_BottomLeft.x + 0, (int)v2_BottomLeft.y + 0, GetBlock((int)v2_BottomLeft.x + 1, (int)v2_BottomLeft.y + 0));

        if (BlockSize_ == Enum_BlockSize.size_2w_2h)
        {
            // Bottom Right -> Top Right
            SetBlock((int)v2_BottomLeft.x + 1, (int)v2_BottomLeft.y + 0, GetBlock((int)v2_BottomLeft.x + 1, (int)v2_BottomLeft.y + 1));

            // Top Right -> Top Left
            SetBlock((int)v2_BottomLeft.x + 1, (int)v2_BottomLeft.y + 1, GetBlock((int)v2_BottomLeft.x + 0, (int)v2_BottomLeft.y + 1));
        }
        else if(BlockSize_ == Enum_BlockSize.size_3w_2h)
        {
            // (1,0) becomes (2,0)
            SetBlock((int)v2_BottomLeft.x + 1, (int)v2_BottomLeft.y + 0, GetBlock((int)v2_BottomLeft.x + 2, (int)v2_BottomLeft.y + 0));

            // (2,0) becomes (2,1)
            SetBlock((int)v2_BottomLeft.x + 2, (int)v2_BottomLeft.y + 0, GetBlock((int)v2_BottomLeft.x + 2, (int)v2_BottomLeft.y + 1));

            // (2,1) becomes (1,1)
            SetBlock((int)v2_BottomLeft.x + 2, (int)v2_BottomLeft.y + 1, GetBlock((int)v2_BottomLeft.x + 1, (int)v2_BottomLeft.y + 1));

            // (1,1) becomes (0,1)
            SetBlock((int)v2_BottomLeft.x + 1, (int)v2_BottomLeft.y + 1, GetBlock((int)v2_BottomLeft.x + 0, (int)v2_BottomLeft.y + 1));
        }
        else if(BlockSize_ == Enum_BlockSize.size_2w_3h)
        {
            // (1,0) becomes (1,1)
            SetBlock((int)v2_BottomLeft.x + 1, (int)v2_BottomLeft.y + 0, GetBlock((int)v2_BottomLeft.x + 1, (int)v2_BottomLeft.y + 1));

            // (1,1) becomes (1,2)
            SetBlock((int)v2_BottomLeft.x + 1, (int)v2_BottomLeft.y + 1, GetBlock((int)v2_BottomLeft.x + 1, (int)v2_BottomLeft.y + 2));

            // (1,2) becomes (0,2)
            SetBlock((int)v2_BottomLeft.x + 1, (int)v2_BottomLeft.y + 2, GetBlock((int)v2_BottomLeft.x + 0, (int)v2_BottomLeft.y + 2));

            // (0,2) becomes (0,1)
            SetBlock((int)v2_BottomLeft.x + 0, (int)v2_BottomLeft.y + 2, GetBlock((int)v2_BottomLeft.x + 0, (int)v2_BottomLeft.y + 1));
        }
        else if(BlockSize_ == Enum_BlockSize.size_3w_3h)
        {
            // (1,0) becomes (2,0)
            SetBlock((int)v2_BottomLeft.x + 1, (int)v2_BottomLeft.y + 0, GetBlock((int)v2_BottomLeft.x + 2, (int)v2_BottomLeft.y + 0));

            // (2,0) becomes (2,1)
            SetBlock((int)v2_BottomLeft.x + 2, (int)v2_BottomLeft.y + 0, GetBlock((int)v2_BottomLeft.x + 2, (int)v2_BottomLeft.y + 1));

            // (2,1) becomes (2,2)
            SetBlock((int)v2_BottomLeft.x + 2, (int)v2_BottomLeft.y + 1, GetBlock((int)v2_BottomLeft.x + 2, (int)v2_BottomLeft.y + 2));

            // (2,2) becomes (1,2)
            SetBlock((int)v2_BottomLeft.x + 2, (int)v2_BottomLeft.y + 2, GetBlock((int)v2_BottomLeft.x + 1, (int)v2_BottomLeft.y + 2));

            // (1,2) becomes (0,2)
            SetBlock((int)v2_BottomLeft.x + 1, (int)v2_BottomLeft.y + 2, GetBlock((int)v2_BottomLeft.x + 0, (int)v2_BottomLeft.y + 2));

            // (0,2) becomes (0,1)
            SetBlock((int)v2_BottomLeft.x + 0, (int)v2_BottomLeft.y + 2, GetBlock((int)v2_BottomLeft.x + 0, (int)v2_BottomLeft.y + 1));
        }

        // (0,1) becomes Temporary
        SetBlock((int)v2_BottomLeft.x + 0, (int)v2_BottomLeft.y + 1, e_TempBlockType_);

        GameObject.Find("BoardDisplay").GetComponent<Cs_BoardDisplay>().RotateBlocks(Enum_Direction.Right, e_BlockSize, v2_BottomLeft);

        // Show new block drop positions
        PotentialBlockVisual();
    }

    // Complete
    void RotateBlocks_CounterClock(Vector2 v2_BottomLeft, Enum_BlockSize BlockSize_)
    {
        GameObject.Find("BoardDisplay").GetComponent<Cs_BoardDisplay>().Set_ClearBackdrops();

        // Store Bottom Left (Temporary)
        Enum_BlockType e_TempBlockType_ = GetBlock((int)v2_BottomLeft.x, (int)v2_BottomLeft.y);

        // Bottom Left -> Top Left
        SetBlock((int)v2_BottomLeft.x + 0, (int)v2_BottomLeft.y + 0, GetBlock((int)v2_BottomLeft.x + 0, (int)v2_BottomLeft.y + 1));

        if (BlockSize_ == Enum_BlockSize.size_2w_2h)
        {
            // Top Left -> Top Right
            SetBlock((int)v2_BottomLeft.x + 0, (int)v2_BottomLeft.y + 1, GetBlock((int)v2_BottomLeft.x + 1, (int)v2_BottomLeft.y + 1));

            // Top Right -> Bottom Right
            SetBlock((int)v2_BottomLeft.x + 1, (int)v2_BottomLeft.y + 1, GetBlock((int)v2_BottomLeft.x + 1, (int)v2_BottomLeft.y + 0));
        }
        else if (BlockSize_ == Enum_BlockSize.size_3w_2h)
        {
            // (0,1) becomes (1,1)
            SetBlock((int)v2_BottomLeft.x + 0, (int)v2_BottomLeft.y + 1, GetBlock((int)v2_BottomLeft.x + 1, (int)v2_BottomLeft.y + 1));

            // (1,1) becomes (2,1)
            SetBlock((int)v2_BottomLeft.x + 1, (int)v2_BottomLeft.y + 1, GetBlock((int)v2_BottomLeft.x + 2, (int)v2_BottomLeft.y + 1));

            // (2,1) becomes (2,0)
            SetBlock((int)v2_BottomLeft.x + 2, (int)v2_BottomLeft.y + 1, GetBlock((int)v2_BottomLeft.x + 2, (int)v2_BottomLeft.y + 0));

            // (2,0) becomes (1,0)
            SetBlock((int)v2_BottomLeft.x + 2, (int)v2_BottomLeft.y + 0, GetBlock((int)v2_BottomLeft.x + 1, (int)v2_BottomLeft.y + 0));
        }
        else if (BlockSize_ == Enum_BlockSize.size_2w_3h)
        {
            // (0,1) becomes (0,2)
            SetBlock((int)v2_BottomLeft.x + 0, (int)v2_BottomLeft.y + 1, GetBlock((int)v2_BottomLeft.x + 0, (int)v2_BottomLeft.y + 2));

            // (0,2) becomes (1,2)
            SetBlock((int)v2_BottomLeft.x + 0, (int)v2_BottomLeft.y + 2, GetBlock((int)v2_BottomLeft.x + 1, (int)v2_BottomLeft.y + 2));

            // (1,2) becomes (1,1)
            SetBlock((int)v2_BottomLeft.x + 1, (int)v2_BottomLeft.y + 2, GetBlock((int)v2_BottomLeft.x + 1, (int)v2_BottomLeft.y + 1));

            // (1,1) becomes (1,0)
            SetBlock((int)v2_BottomLeft.x + 1, (int)v2_BottomLeft.y + 1, GetBlock((int)v2_BottomLeft.x + 1, (int)v2_BottomLeft.y + 0));
        }
        else if (BlockSize_ == Enum_BlockSize.size_3w_3h)
        {
            // (0,1) becomes (0,2)
            SetBlock((int)v2_BottomLeft.x + 0, (int)v2_BottomLeft.y + 1, GetBlock((int)v2_BottomLeft.x + 0, (int)v2_BottomLeft.y + 2));

            // (0,2) becomes (1,2)
            SetBlock((int)v2_BottomLeft.x + 0, (int)v2_BottomLeft.y + 2, GetBlock((int)v2_BottomLeft.x + 1, (int)v2_BottomLeft.y + 2));

            // (1,2) becomes (2,2)
            SetBlock((int)v2_BottomLeft.x + 1, (int)v2_BottomLeft.y + 2, GetBlock((int)v2_BottomLeft.x + 2, (int)v2_BottomLeft.y + 2));

            // (2,2) becomes (2,1)
            SetBlock((int)v2_BottomLeft.x + 2, (int)v2_BottomLeft.y + 2, GetBlock((int)v2_BottomLeft.x + 2, (int)v2_BottomLeft.y + 1));

            // (2,1) becomes (2,0)
            SetBlock((int)v2_BottomLeft.x + 2, (int)v2_BottomLeft.y + 1, GetBlock((int)v2_BottomLeft.x + 2, (int)v2_BottomLeft.y + 0));

            // (2,0) becomes (1,0)
            SetBlock((int)v2_BottomLeft.x + 2, (int)v2_BottomLeft.y + 0, GetBlock((int)v2_BottomLeft.x + 1, (int)v2_BottomLeft.y + 0));
        }

        // (1,0) becomes Temporary
        SetBlock((int)v2_BottomLeft.x + 1, (int)v2_BottomLeft.y + 0, e_TempBlockType_);

        GameObject.Find("BoardDisplay").GetComponent<Cs_BoardDisplay>().RotateBlocks( Enum_Direction.Left, e_BlockSize, v2_BottomLeft );

        // Show new block drop positions
        PotentialBlockVisual();
    }

    void SetBlock(int x_Pos_, int y_Pos_, Enum_BlockType blockType_)
    {
        if (x_Pos_ < 0 || x_Pos_ >= i_ArrayWidth)  return;
        if (y_Pos_ < 0 || y_Pos_ >= i_ArrayHeight) return;

        BlockArray[y_Pos_, x_Pos_] = blockType_;
    }
    
    void SetBlock(Vector2 blockPos_, Enum_BlockType blockType_)
    {
        SetBlock((int)blockPos_.x, (int)blockPos_.y, blockType_);
    }
    
    void PullBlocksDown()
    {
        // Clear backdrops
        GameObject.Find("BoardDisplay").GetComponent<Cs_BoardDisplay>().Set_ClearBackdrops();

        // Run through the array and pull blocks down to their lowest point
        for (int x_ = 0; x_ < i_ArrayWidth; ++x_)
        {
            // The y begins at 1 since we can't move a block down at y = 0
            for (int y_ = 1; y_ < i_ArrayHeight; ++y_)
            {
                // Get the current block type
                Enum_BlockType thisBlock = GetBlock(x_, y_);

                // Get the current block type beneath us
                Enum_BlockType lowerBlock = GetBlock(x_, y_ - 1);

                if (thisBlock != Enum_BlockType.Empty && lowerBlock == Enum_BlockType.Empty)
                {
                    // If a static block is found with an open spot beneath it, shift it down one spot
                    SetBlock(x_, y_ - 1, thisBlock);

                    // Set the previous block position to empty
                    SetBlock(x_, y_, Enum_BlockType.Empty);

                    #region Send movement to BoardDisplay
                    GameObject.Find("BoardDisplay").GetComponent<Cs_BoardDisplay>().MoveBlock_Dir(Enum_Direction.Down, new IntVector2(x_, y_));
                    #endregion

                    // Reset and re-loop
                    y_ = 0;
                    
                    continue;
                }
            }
        }
    }

    void DestroyBlackBlocks()
    {
        for(int y_ = 0; y_ < i_ArrayHeight; ++y_)
        {
            for(int x_ = 0; x_ < i_ArrayWidth; ++x_)
            {
                if(BlockArray[y_, x_] == Enum_BlockType.Block_3_Active || BlockArray[y_, x_] == Enum_BlockType.Block_3_Static)
                {
                    // TODO: Destroy Block at Location
                    // Tell the board model to destroy the block
                    GameObject.Find("BoardDisplay").GetComponent<Cs_BoardDisplay>().DestroyBlockAt(new IntVector2(x_, y_));

                    // Set Position to be 'Empty'
                    SetBlock(x_, y_, Enum_BlockType.Empty);
                }
            }
        }
    }

    void CheckScoreLineLeftWall()
    {
        // Run through all of the ScoreLine Blocks and remove duplicate 'y = LeftWall' Blocks
        for (int i_ = 0; i_ < iv2_ScoreLine.Count; ++i_)
        {
            // If the 'second' block in the list is also along the left wall...
            if (iv2_ScoreLine[i_ + 1].x == 1)
            {
                // Remove the 'first' block
                iv2_ScoreLine.RemoveAt(0);

                // Reset the i_ position to loop again
                i_ = 0; // Since the next loop increments i_
            }
            else
            {
                // Break out
                break;
            }
        }
    }

    int i_ScoreLine_Counter;
    bool b_RunAgain = false;
    void AllBlocksStatic()
    {
        b_RunAgain = false;

        #region Set Left & Right Walls to 'Empty' (Done BEFORE scoring)
        for (int y_ = 0; y_ < i_ArrayHeight; ++y_)
        {
            // Tell the board model to destroy the block
            if (GetBlock(0, y_) != Enum_BlockType.Empty)
            {
                GameObject.Find("BoardDisplay").GetComponent<Cs_BoardDisplay>().DestroyBlockAt(new IntVector2(0, y_));
            }

            // Set Left Wall to be 'Empty'
            SetBlock(0, y_, Enum_BlockType.Empty);

            // Tell the board model to destroy the block
            if (GetBlock(i_ArrayWidth - 1, y_) != Enum_BlockType.Empty)
            {
                GameObject.Find("BoardDisplay").GetComponent<Cs_BoardDisplay>().DestroyBlockAt(new IntVector2(i_ArrayWidth - 1, y_));
            }

            // Set Right Wall to be 'Empty'
            SetBlock(i_ArrayWidth - 1, y_, Enum_BlockType.Empty);
        }
        #endregion

        #region Convert All to Static
        // Run through the array and convert all blocks into their static counterpart. Run bottom to top, left to right.
        for (int x_ = 0; x_ < i_ArrayWidth; ++x_)
        {
            for (int y_ = 0; y_ < i_ArrayHeight; ++y_)
            {
                Enum_BlockType tempBlock = GetBlock(x_, y_);

                if (tempBlock == Enum_BlockType.Block_1_Active)
                {
                    SetBlock(x_, y_, Enum_BlockType.Block_1_Static);
                }
                else if (tempBlock == Enum_BlockType.Block_2_Active)
                {
                    SetBlock(x_, y_, Enum_BlockType.Block_2_Static);
                }
                else if (tempBlock == Enum_BlockType.Block_3_Active)
                {
                    SetBlock(x_, y_, Enum_BlockType.Block_3_Static);
                }
            }
        }
        #endregion

        #region Pull Blocks Down
        PullBlocksDown();
        #endregion

        // TODO: RUN SCORE CODE HERE FIRST
        if (Load_ScoreLine())
        {
            #region Print Scoreline to console
            string s_ScoreLine = "LINE REACHED: (" + iv2_ScoreLine.Count + "):";
            for (int i_ = 0; i_ < iv2_ScoreLine.Count; ++i_)
            {
                if (i_ != iv2_ScoreLine.Count - 1)
                {
                    s_ScoreLine += iv2_ScoreLine[i_] + ", ";
                }
                else
                {
                    s_ScoreLine += iv2_ScoreLine[i_] + "";
                }
            }
            print(s_ScoreLine);
            #endregion

            CheckScoreLineLeftWall();

            // Destroy all Black Blocks
            if (b_ThreeBlockColors)
            {
                DestroyBlackBlocks();
            }

            b_RunAgain = true;

            e_PauseEffect = Enum_PauseEffect.ScoreLine;

            GameObject.Find("BoardDisplay").GetComponent<Cs_BoardDisplay>().Set_ScoreTextIncrease(iv2_ScoreLine.Count);

            i_ScoreLine_Counter = 0;

            return;
        }

        #region Set 'Mid Empty' row to empty (Done AFTER scoring)
        //for (int y_ = 0; y_ < i_ArrayHeight; ++y_)
        //{
        //    if (b_MidRowBlank)
        //    {
        //        if (i_ExtraBlankRow > 0 &&
        //            i_ExtraBlankRow < i_ArrayWidth)
        //        {
        //            // Destroy the block model on screen
        //            if (GetBlock(0, y_) != Enum_BlockType.Empty)
        //            {
        //                GameObject.Find("BoardDisplay").GetComponent<Cs_BoardDisplay>().DestroyBlockAt(new IntVector2(i_ExtraBlankRow, y_));
        //            }

        //            SetBlock(i_ExtraBlankRow, y_, Enum_BlockType.Empty);
        //        }
        //    }
        //}
        #endregion

        // Set the next block size to be whatever we found
        e_BlockSize = e_NextBlockSize;

        // Clear all backdrops now. Creating new blocks will reset the new backdrop colors
        // GameObject.Find("BoardDisplay").GetComponent<Cs_BoardDisplay>().Set_ClearBackdrops();

        if(b_RunAgain)
        {
            AllBlocksStatic();

            // GameObject.Find("BoardDisplay").GetComponent<Cs_BoardDisplay>().Set_ClearBackdrops();
        }
        else
        {
            // GameObject.Find("BoardDisplay").GetComponent<Cs_BoardDisplay>().Set_ClearBackdrops();

            Load_ScoreLine();

            CheckScoreLineLeftWall();

            CreateNewBlock();

            // Reset time for new block to be created
            f_TimeToDrop = -1f;
        }
    }

    Enum_BlockType GetBlock(float x_Pos_, float y_Pos_)
    {
        return GetBlock((int)x_Pos_, (int)y_Pos_);
    }
    Enum_BlockType GetBlock(int x_Pos_, int y_Pos_)
    {
        return BlockArray[y_Pos_, x_Pos_];
    }
    #endregion

    #region Scoring System
    
    List<IntVector2> iv2_ScoreLine    = new List<IntVector2>();
    List<IntVector2> iv2_PathfindLine = new List<IntVector2>();
    Enum_BlockType e_CurrBlockType;
    bool Load_ScoreLine()
    {
        // Reset v2_ScoreLine
        iv2_ScoreLine = new List<IntVector2>();
        
        // Reset storage of current block color
        e_CurrBlockType = Enum_BlockType.Empty;

        int i_LeftBound = 1;
        int i_RightBound = i_ArrayWidth - 1;

        // Begin checking for a scoreline only if the left-most column (X = 1) has a block in it
        for(int y_ = 0; y_ < i_ArrayHeight; ++y_)
        {
            //print("Checking: " + i_LeftBound + ", " + y_);
            //print("Curr Block: " + e_CurrBlockType);
            //print("New Block: " + GetBlock(i_LeftBound, y_));

            // If the previous checked block differs than the current block && isn't empty && isn't Block 3
            if( e_CurrBlockType != GetBlock(i_LeftBound, y_) &&
                GetBlock(i_LeftBound, y_) != Enum_BlockType.Empty &&
                GetBlock(i_LeftBound, y_) != Enum_BlockType.Block_3_Static)
            {
                
                // Store the new block
                e_CurrBlockType = GetBlock(i_LeftBound, y_);

                #region Check each column for this blocktype
                // Initiate a check to ensure every column has at least one block in it of the current block type
                for(int x = i_LeftBound; x < i_RightBound; ++x)
                {
                    // Reset the bool at the beginning of each column loop
                    bool b_RowHasBlockType = false;

                    // Check each column from the bottom up
                    for(int y = 0; y < i_ArrayHeight; ++y)
                    {
                        // If this column has the specific block type, move to the next column
                        if (GetBlock(x, y) == e_CurrBlockType)
                        {
                            b_RowHasBlockType = true;

                            // Move to the next column
                            continue;
                        }
                    }

                    // If we didn't find the specific block in this column, we can't have a completed line. Break out.
                    if (!b_RowHasBlockType)
                    {
                        // print("FAILED");

                        // Continue since we need to check the other blocks in the column, regardless of the outcome of this one
                        continue;
                    }
                }
                #endregion 

                // Clear the list since this is a new attempt to find a line
                iv2_PathfindLine = new List<IntVector2>();

                if( GetBlock(1, y_) == Enum_BlockType.Block_1_Static ||
                    GetBlock(1, y_) == Enum_BlockType.Block_2_Static ||
                    GetBlock(1, y_) == Enum_BlockType.Block_3_Static  )
                {
                    // Push findings into the i_PathfindLine
                    iv2_PathfindLine.Add(new IntVector2(1, y_));

                    // Begins the iterative check
                    if(ScoreLine(Enum_Direction.Right))
                    {
                        GameObject.Find("BoardDisplay").GetComponent<Cs_BoardDisplay>().Set_ClearBackdrops();

                        return true;
                    }
                }
            }
        }

        return false;
    }

    bool ScoreLine( Enum_Direction e_Dir_ )
    {
        // Continue from the last-populated position in the list
        IntVector2 iv2_CurrPos = iv2_PathfindLine[iv2_PathfindLine.Count - 1];

        #region If the current X position is the edge of the board, we have a line. Give the player the score.
        // Check the X coordinate of the current position to determine if we are done
        // 'i_ArrayWidth - 2' is the far right edge of the board, since we do not include the 'wall mechanic'
        if(iv2_CurrPos.x == i_ArrayWidth - 2)
        {
            // Since we are at the far right edge, we push the list into the 'i_ScoreLine'
            for (int i_ = 0; i_ < iv2_PathfindLine.Count; ++i_)
            {
                iv2_ScoreLine.Add(iv2_PathfindLine[i_]);
            }

            // TODO: Apply a score for the user

            // TODO: Pause gameplay while we run a 'scoreline' visual

            // (Unknown Remnant) b_AllowedToCheck = true

            // Return victorious
            return true;
        }
        #endregion

        #region Else - Continue searching through the board
        else // We have determined that we need to continue through the board
        {
            #region Check Adjacent Blocks
            // Begin checking to determine the next locations to check
            bool b_RightFilled = false;
            bool b_BelowFilled = false;
            bool b_AboveFilled = false;
            bool b_LeftFilled  = false;

            // If the space to the right exists, AND
            // If the space to the right has the same block type AND
            // We are searching anywhere but where we came from...
            if( iv2_CurrPos.x + 1 < i_ArrayWidth &&
                GetBlock(iv2_CurrPos.x + 1, iv2_CurrPos.y) == e_CurrBlockType &&
                e_Dir_ != Enum_Direction.Left)
            {
                b_RightFilled = true;
            }

            // If the space below exists AND
            // If the space below has the same block type AND
            // We are searching anywhere but where we came from...
            if( iv2_CurrPos.y - 1 >= 0 &&
                GetBlock( iv2_CurrPos.x, iv2_CurrPos.y - 1 ) == e_CurrBlockType &&
                e_Dir_ != Enum_Direction.Up )
            {
                b_BelowFilled = true;
            }

            // If the space above exists AND
            // If the space above has the same block type AND
            // We are searching anywhere but where we came from...
            if(iv2_CurrPos.y + 1 < i_ArrayHeight &&
               GetBlock( iv2_CurrPos.x, iv2_CurrPos.y + 1 ) == e_CurrBlockType &&
               e_Dir_ != Enum_Direction.Down )
            {
                b_AboveFilled = true;
            }

            // If the space to the left exists AND
            // If the space to the left has the same block type AND
            // We are searching anywhere but where we came from...
            if( iv2_CurrPos.x - 1 >= 0 && 
                GetBlock( iv2_CurrPos.x - 1, iv2_CurrPos.y ) == e_CurrBlockType &&
                e_Dir_ != Enum_Direction.Right )
            {
                b_LeftFilled = true;
            }
            #endregion

            // Reset b_ShouldContinue;
            bool b_ShouldContinue = true;

            #region 'Right Filled' check
            if(b_RightFilled)
            {
                // Set positions to check
                IntVector2 iv2_RightAbove = new IntVector2(iv2_CurrPos.x + 1, iv2_CurrPos.y + 1);
                IntVector2 iv2_RightBelow = new IntVector2(iv2_CurrPos.x + 1, iv2_CurrPos.y - 1);

                // Check to see if the adjacent positions to the above spot are within the array EXCEPT the last position
                for(int i_ = 0; i_ < iv2_PathfindLine.Count; ++i_)
                {
                    // If we ever find a position already stored within, do not continue
                    if( iv2_PathfindLine[i_] == iv2_RightAbove ||
                        iv2_PathfindLine[i_] == iv2_RightBelow)
                    {
                        b_ShouldContinue = false;

                        continue;
                    }
                }

                // Otherwise, we're good to keep checking in this direction
                if(b_ShouldContinue)
                {
                    // Store the new direction (To the right)
                    iv2_PathfindLine.Add(new IntVector2( iv2_CurrPos.x + 1, iv2_CurrPos.y ));

                    // Run another iterative cycle to the right
                    if( ScoreLine( Enum_Direction.Right ))
                    {
                        return true;
                    }
                }
            }
            #endregion

            // Reset b_ShouldContinue
            b_ShouldContinue = true;

            #region 'Below Filled' check
            if(b_BelowFilled)
            {
                // Set positions to check
                IntVector2 iv2_BelowLeft = new IntVector2(iv2_CurrPos.x - 1, iv2_CurrPos.y - 1);
                IntVector2 iv2_BelowRight = new IntVector2(iv2_CurrPos.x + 1, iv2_CurrPos.y - 1);

                // Check to see if the adjacent positions to the above spot are within the array EXCEPT the last position
                for(int i_ = 0; i_ < iv2_PathfindLine.Count; ++i_)
                {
                    // If we ever find a position already stored within, do not continue
                    if ( iv2_PathfindLine[i_] == iv2_BelowLeft ||
                        iv2_PathfindLine[i_] == iv2_BelowRight )
                    {
                        b_ShouldContinue = false;
                    }
                }

                // Otherwise, we're good to keep checking in this direction
                if(b_ShouldContinue)
                {
                    // Store the new direction (Down)
                    iv2_PathfindLine.Add( new IntVector2( iv2_CurrPos.x, iv2_CurrPos.y - 1 ));

                    // Run another iterative cycle downwards
                    if( ScoreLine( Enum_Direction.Down ))
                    {
                        return true;
                    }
                }
            }
            #endregion

            // Reset b_ShouldContinue
            b_ShouldContinue = true;

            #region 'Above Filled' check
            if (b_AboveFilled)
            {
                // Set positions to check
                IntVector2 iv2_AboveLeft = new IntVector2(iv2_CurrPos.x - 1, iv2_CurrPos.y + 1);
                IntVector2 iv2_AboveRight = new IntVector2(iv2_CurrPos.x + 1, iv2_CurrPos.y + 1);

                // Check to see if the adjacent positions to the above spot are within the array EXCEPT the last position
                for (int i_ = 0; i_ < iv2_PathfindLine.Count; ++i_)
                {
                    // If we ever find a position already stored within, do not continue
                    if (iv2_PathfindLine[i_] == iv2_AboveLeft ||
                        iv2_PathfindLine[i_] == iv2_AboveRight)
                    {
                        b_ShouldContinue = false;

                        continue;
                    }
                }

                // Otherwise, we're good to keep checking in this direction
                if (b_ShouldContinue)
                {
                    // Store the new direction (Above)
                    iv2_PathfindLine.Add(new IntVector2(iv2_CurrPos.x, iv2_CurrPos.y + 1));

                    // Run another iterative cycle above
                    if (ScoreLine(Enum_Direction.Up))
                    {
                        return true;
                    }
                }
            }
            #endregion

            // Reset b_ShouldContinue
            b_ShouldContinue = true;

            #region 'Left Filled' check
            if (b_LeftFilled)
            {
                // Set positions to check
                IntVector2 iv2_LeftAbove = new IntVector2(iv2_CurrPos.x - 1, iv2_CurrPos.y + 1);
                IntVector2 iv2_LeftBelow = new IntVector2(iv2_CurrPos.x - 1, iv2_CurrPos.y - 1);

                // Check to see if the adjacent positions to the above spot are within the array EXCEPT the last position
                for (int i_ = 0; i_ < iv2_PathfindLine.Count; ++i_)
                {
                    // If we ever find a position already stored within, do not continue
                    if (iv2_PathfindLine[i_] == iv2_LeftAbove ||
                        iv2_PathfindLine[i_] == iv2_LeftBelow)
                    {
                        b_ShouldContinue = false;

                        continue;
                    }
                }

                // Otherwise, we're good to keep checking in this direction
                if (b_ShouldContinue)
                {
                    // Store the new direction (Left)
                    iv2_PathfindLine.Add( new IntVector2( iv2_CurrPos.x - 1, iv2_CurrPos.y ));

                    // Run another iterative cycle above
                    if (ScoreLine(Enum_Direction.Left))
                    {
                        return true;
                    }
                }
            }
            #endregion
        }
        #endregion

        // If we got this far, none of the directions worked. Pop off this location and return false.
        iv2_PathfindLine.RemoveAt(iv2_PathfindLine.Count - 1);

        return false;
    }

    #endregion

    #region Print Array To Console
    void PrintArrayToConsole()
    {
        // The 'y' is reversed (top to bottom) to compensate for printing
        for(int j = i_ArrayHeight - 1; j >= 0 ; j--)
        {
            string tempString = "";

            for(int i = 0; i < i_ArrayWidth; ++i)
            {
                // Left & Right 'Walls'
                if((i == 0 || i == i_ArrayWidth - 1) && (BlockArray[j, i] == Enum_BlockType.Empty))
                {
                    tempString += "{!!} ";
                }
                //else if( b_MidRowBlank && // The option is set for a mid row empty region
                //         i_ExtraBlankRow < i_ArrayWidth && // Extra Blank Row is Set
                //         i == i_ExtraBlankRow && // This row is the one that's been set
                //        (BlockArray[j, i] == Enum_BlockType.Empty)) // The block position is empty
                //{
                //    tempString += "{!!} ";
                //}
                // Normal empty block position
                else if (BlockArray[j, i] == Enum_BlockType.Empty) tempString += "[__] ";
                // Print a populated block
                else tempString += "[" + (int)BlockArray[j, i] + "] ";
            }
            print(tempString);
        }
        print("Active Block: " + v2_ActiveBlockLocation + "\n-----------------------------------------------------------------\n");
    }
    #endregion

    // Returns true if the game is over
    bool GameOverCheck()
    {
        int i_Width = 2;
        int i_Height = 2;

        if (e_BlockSize == Enum_BlockSize.size_3w_2h || e_BlockSize == Enum_BlockSize.size_3w_3h) i_Width = 3;
        if (e_BlockSize == Enum_BlockSize.size_2w_3h || e_BlockSize == Enum_BlockSize.size_3w_3h) i_Height = 3;

        for (int x_ = 0; x_ < i_Width; ++x_ )
        {
            for( int y_ = 0; y_ < i_Height; ++y_ )
            {
                // print("Checking: " + (int)(v2_ActiveBlockLocation.x + x_) + ", " + (int)(v2_ActiveBlockLocation.y + y_));

                if(x_ < i_ArrayWidth && y_ < i_ArrayHeight)
                {
                    if(GetBlock((v2_ActiveBlockLocation.x + x_), (v2_ActiveBlockLocation.y + y_)) != Enum_BlockType.Empty)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public void Input_MoveLeft()
    {
        if (e_PauseEffect == Enum_PauseEffect.GameOver) return;

        MoveActiveBlocks_Left(v2_ActiveBlockLocation, e_BlockSize);

        b_DemoGame_InputReceived = true;
    }

    public void Input_MoveRight()
    {
        if (e_PauseEffect == Enum_PauseEffect.GameOver) return;

        MoveActiveBlocks_Right(v2_ActiveBlockLocation, e_BlockSize);

        b_DemoGame_InputReceived = true;
    }

    public void Input_MoveDown()
    {
        if (e_PauseEffect == Enum_PauseEffect.GameOver) return;

        MoveActiveBlocks_Down(v2_ActiveBlockLocation, e_BlockSize);

        // Reset timer to drop blocks
        f_TimeToDrop = 0;

        b_DemoGame_InputReceived = true;
    }

    public void Input_Drop()
    {
        if (e_PauseEffect == Enum_PauseEffect.GameOver) return;

        AllBlocksStatic();

        // Reset timer to drop blocks
        // f_TimeToDrop = -1f;
        f_TimeToDrop = 0f;

        GameObject.Find("ThemeManager").GetComponent<Cs_AudioManager>().Play_SoundEffect(Enum_SoundEffect.DropBlock);

        b_DemoGame_InputReceived = true;
    }

    public void Input_RotateCounterclock()
    {
        if (e_PauseEffect == Enum_PauseEffect.GameOver) return;

        RotateBlocks_CounterClock(v2_ActiveBlockLocation, e_BlockSize);

        b_DemoGame_InputReceived = true;
    }

    public void Input_RotateClockwise()
    {
        if (e_PauseEffect == Enum_PauseEffect.GameOver) return;

        RotateBlocks_Clockwise(v2_ActiveBlockLocation, e_BlockSize);

        b_DemoGame_InputReceived = true;
    }

    void Cheat_SetDoubleLine()
    {
        // 1,0
        SetBlock(1, 0, Enum_BlockType.Block_1_Static);
        GameObject.Find("BoardDisplay").GetComponent<Cs_BoardDisplay>().Set_OneBlock(1, 0, Enum_BlockType.Block_1_Static);

        // 1,1
        SetBlock(1, 1, Enum_BlockType.Block_2_Static);
        GameObject.Find("BoardDisplay").GetComponent<Cs_BoardDisplay>().Set_OneBlock(1, 1, Enum_BlockType.Block_2_Static);

        // 2,0
        SetBlock(2, 0, Enum_BlockType.Block_1_Static);
        GameObject.Find("BoardDisplay").GetComponent<Cs_BoardDisplay>().Set_OneBlock(2, 0, Enum_BlockType.Block_1_Static);

        // 2,1
        SetBlock(2, 1, Enum_BlockType.Block_2_Static);
        GameObject.Find("BoardDisplay").GetComponent<Cs_BoardDisplay>().Set_OneBlock(2, 1, Enum_BlockType.Block_2_Static);

        // 3,0
        SetBlock(3, 0, Enum_BlockType.Block_1_Static);
        GameObject.Find("BoardDisplay").GetComponent<Cs_BoardDisplay>().Set_OneBlock(3, 0, Enum_BlockType.Block_1_Static);

        // 3,1
        SetBlock(3, 1, Enum_BlockType.Block_2_Static);
        GameObject.Find("BoardDisplay").GetComponent<Cs_BoardDisplay>().Set_OneBlock(3, 1, Enum_BlockType.Block_2_Static);

        // 4,0
        SetBlock(4, 0, Enum_BlockType.Block_1_Static);
        GameObject.Find("BoardDisplay").GetComponent<Cs_BoardDisplay>().Set_OneBlock(4, 0, Enum_BlockType.Block_1_Static);

        // 4,1
        SetBlock(4, 1, Enum_BlockType.Block_2_Static);
        GameObject.Find("BoardDisplay").GetComponent<Cs_BoardDisplay>().Set_OneBlock(4, 1, Enum_BlockType.Block_2_Static);

        // 5,0
        SetBlock(5, 0, Enum_BlockType.Block_1_Static);
        GameObject.Find("BoardDisplay").GetComponent<Cs_BoardDisplay>().Set_OneBlock(5, 0, Enum_BlockType.Block_1_Static);

        // 5,1
        SetBlock(5, 1, Enum_BlockType.Block_2_Static);
        GameObject.Find("BoardDisplay").GetComponent<Cs_BoardDisplay>().Set_OneBlock(5, 1, Enum_BlockType.Block_2_Static);

        // 6,0
        SetBlock(6, 0, Enum_BlockType.Block_1_Static);
        GameObject.Find("BoardDisplay").GetComponent<Cs_BoardDisplay>().Set_OneBlock(6, 0, Enum_BlockType.Block_1_Static);

        // 6,1
        SetBlock(6, 1, Enum_BlockType.Block_2_Static);
        GameObject.Find("BoardDisplay").GetComponent<Cs_BoardDisplay>().Set_OneBlock(6, 1, Enum_BlockType.Block_2_Static);

        // 7,0
        SetBlock(7, 0, Enum_BlockType.Block_1_Static);
        GameObject.Find("BoardDisplay").GetComponent<Cs_BoardDisplay>().Set_OneBlock(7, 0, Enum_BlockType.Block_1_Static);
        
        // 8,0
        SetBlock(8, 0, Enum_BlockType.Block_2_Static);
        GameObject.Find("BoardDisplay").GetComponent<Cs_BoardDisplay>().Set_OneBlock(8, 0, Enum_BlockType.Block_2_Static);

        // 7,1
        SetBlock(7, 1, Enum_BlockType.Block_1_Static);
        GameObject.Find("BoardDisplay").GetComponent<Cs_BoardDisplay>().Set_OneBlock(7, 1, Enum_BlockType.Block_1_Static);

        // 7,2
        SetBlock(7, 2, Enum_BlockType.Block_2_Static);
        GameObject.Find("BoardDisplay").GetComponent<Cs_BoardDisplay>().Set_OneBlock(7, 2, Enum_BlockType.Block_2_Static);

        // 8,1
        SetBlock(8, 1, Enum_BlockType.Block_1_Static);
        GameObject.Find("BoardDisplay").GetComponent<Cs_BoardDisplay>().Set_OneBlock(8, 1, Enum_BlockType.Block_1_Static);

        // 8,2
        SetBlock(8, 2, Enum_BlockType.Block_1_Static);
        GameObject.Find("BoardDisplay").GetComponent<Cs_BoardDisplay>().Set_OneBlock(8, 2, Enum_BlockType.Block_1_Static);

        // ---------------

        // 1,2
        SetBlock(1, 2, Enum_BlockType.Block_1_Static);
        GameObject.Find("BoardDisplay").GetComponent<Cs_BoardDisplay>().Set_OneBlock(1, 2, Enum_BlockType.Block_1_Static);
        
        // 2,2
        SetBlock(2, 2, Enum_BlockType.Block_1_Static);
        GameObject.Find("BoardDisplay").GetComponent<Cs_BoardDisplay>().Set_OneBlock(2, 2, Enum_BlockType.Block_1_Static);

        // 3,2
        SetBlock(3, 2, Enum_BlockType.Block_1_Static);
        GameObject.Find("BoardDisplay").GetComponent<Cs_BoardDisplay>().Set_OneBlock(3, 2, Enum_BlockType.Block_1_Static);

        // 4,2
        SetBlock(4, 2, Enum_BlockType.Block_1_Static);
        GameObject.Find("BoardDisplay").GetComponent<Cs_BoardDisplay>().Set_OneBlock(4, 2, Enum_BlockType.Block_1_Static);

        // 5,2
        SetBlock(5, 2, Enum_BlockType.Block_1_Static);
        GameObject.Find("BoardDisplay").GetComponent<Cs_BoardDisplay>().Set_OneBlock(5, 2, Enum_BlockType.Block_1_Static);

        // 6,2
        SetBlock(6, 2, Enum_BlockType.Block_1_Static);
        GameObject.Find("BoardDisplay").GetComponent<Cs_BoardDisplay>().Set_OneBlock(6, 2, Enum_BlockType.Block_1_Static);

        // 6,3
        SetBlock(6, 3, Enum_BlockType.Block_1_Static);
        GameObject.Find("BoardDisplay").GetComponent<Cs_BoardDisplay>().Set_OneBlock(6, 3, Enum_BlockType.Block_1_Static);

        // 7,3
        SetBlock(7, 3, Enum_BlockType.Block_1_Static);
        GameObject.Find("BoardDisplay").GetComponent<Cs_BoardDisplay>().Set_OneBlock(7, 3, Enum_BlockType.Block_1_Static);

        // 8,3
        SetBlock(8, 3, Enum_BlockType.Block_1_Static);
        GameObject.Find("BoardDisplay").GetComponent<Cs_BoardDisplay>().Set_OneBlock(8, 3, Enum_BlockType.Block_1_Static);
    }

    // Update is called once per frame
    float f_ScoreLine_Timer;
    // static float f_ScoreLine_Timer_Max = 0.075f;
    static float f_ScoreLine_Timer_Max = 0.1f;
    float f_ScoreLine_Timer_Conclusion; // A timer that continues to run after the last block's visual begins
    float f_ScoreLine_Timer_Conclusion_Max = 1.0f; // After the above timer completes, we set the pause state to continue
    int i_GameOver_X = 0;
    int i_GameOver_Y = 0;
    float f_GameOver_Timer;
    float f_GameOver_Timer_Max = 0.025f;
    Enum_BlockType e_GameOverBlock = Enum_BlockType.Block_1_Static;
    void Update ()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            Cheat_SetDoubleLine();
        }

        if(e_PauseEffect == Enum_PauseEffect.Unpause)
        {
            if(b_DemoGame_InputReceived)
            {
                #region Drop Block timer
            // Decrement timer for next time to move blocks down
            if (i_TimeToDrop_Max > 0)
            {
                f_TimeToDrop += Time.deltaTime;

                if(f_TimeToDrop > i_TimeToDrop_Max)
                {
                    // Reset timer
                    f_TimeToDrop = 0;

                    // Drop blocks down manually
                    MoveActiveBlocks_Down(v2_ActiveBlockLocation, e_BlockSize);
                }
            }
            #endregion
            }
        }
        else if(e_PauseEffect == Enum_PauseEffect.ScoreLine)
        {
            #region Scoreline Pause
            // Run through the Scoreline array and have them destroy themselves
            f_ScoreLine_Timer += Time.deltaTime;

            if(f_ScoreLine_Timer > f_ScoreLine_Timer_Max)
            {
                if(i_ScoreLine_Counter < iv2_ScoreLine.Count)
                {
                    // Set the block in BlockArray to empty
                    SetBlock(iv2_ScoreLine[i_ScoreLine_Counter].x, iv2_ScoreLine[i_ScoreLine_Counter].y, Enum_BlockType.Empty);

                    // Score the block model on the screen
                    GameObject.Find("BoardDisplay").GetComponent<Cs_BoardDisplay>().ScoreBlockAt(iv2_ScoreLine[i_ScoreLine_Counter]);

                    #region Play sound effect
                    // Store previous pitch
                    float f_Pitch_Prev = go_SFX.GetComponent<AudioSource>().pitch;

                    // Find pitch
                    float f_Pitch = 0.4f + ((iv2_ScoreLine[i_ScoreLine_Counter].x / 3f) * 0.2f) + ((iv2_ScoreLine[i_ScoreLine_Counter].y / 3f) * 0.05f);
                    print("Setting pitch: " + f_Pitch);
                    go_SFX.GetComponent<AudioSource>().clip = sfx_ScoreEffect;
                    go_SFX.GetComponent<AudioSource>().pitch = f_Pitch;
                    go_SFX.GetComponent<AudioSource>().time = 0.09f;
                    go_SFX.GetComponent<AudioSource>().Play();

                    // Set previous pitch
                    // go_SFX.GetComponent<AudioSource>().pitch = f_Pitch_Prev;
                    #endregion


                    // Increment i_ScoreLine_Counter
                    ++i_ScoreLine_Counter;
                }
                // Scoreline is finished, reset and move on
                else
                {
                    // f_ScoreLine_Timer_Max gets added here cause I'm lazy. This line doesn't hit until the above timer breaches f_ScoreLine_Timer_Max.
                    f_ScoreLine_Timer_Conclusion += Time.deltaTime + f_ScoreLine_Timer_Max;

                    if(f_ScoreLine_Timer_Conclusion > f_ScoreLine_Timer_Conclusion_Max)
                    {
                        if (Load_ScoreLine())
                        {
                            CheckScoreLineLeftWall();

                            b_RunAgain = true;

                            e_PauseEffect = Enum_PauseEffect.ScoreLine;

                            GameObject.Find("BoardDisplay").GetComponent<Cs_BoardDisplay>().Set_ScoreTextIncrease(iv2_ScoreLine.Count);

                            i_ScoreLine_Counter = 0;

                            f_ScoreLine_Timer_Conclusion = 0.0f;

                            return;
                        }
                        else
                        {
                            f_ScoreLine_Timer_Conclusion = 0.0f;

                            // Reset the counter for the next cycle
                            i_ScoreLine_Counter = 0;

                            b_RunAgain = true;

                            // Clear the ScoreLine list
                            iv2_ScoreLine = new List<IntVector2>();

                            // 'PullBlocksDown' to update the board
                            PullBlocksDown();

                            if(Load_ScoreLine())
                            {
                                CheckScoreLineLeftWall();

                                b_RunAgain = true;

                                e_PauseEffect = Enum_PauseEffect.ScoreLine;

                                GameObject.Find("BoardDisplay").GetComponent<Cs_BoardDisplay>().Set_ScoreTextIncrease(iv2_ScoreLine.Count);

                                i_ScoreLine_Counter = 0;

                                f_ScoreLine_Timer_Conclusion = 0.0f;

                                return;
                            }

                            // Make a new block
                            CreateNewBlock();

                            // Gameplay resumes
                            e_PauseEffect = Enum_PauseEffect.Unpause;
                        }

                        // Reset the ScoreLine timer
                    }
                }

                // Reset the float timer
                f_ScoreLine_Timer = 0.0f;
            }
            #endregion
        }
        else if(e_PauseEffect == Enum_PauseEffect.StartGame)
        {
            #region Start Game
            // Creates a new block after a couple seconds at the beginning of the game
            if (f_StartGameTimer > 0)
            {
                f_StartGameTimer -= Time.deltaTime;

                if (f_StartGameTimer < 0)
                {
                    // GameObject.Find("BoardDisplay").GetComponent<Cs_BoardDisplay>().Set_ClearBackdrops();
                    CreateNewBlock();
                    e_PauseEffect = Enum_PauseEffect.Unpause;
                }
            }
            #endregion
        }
        else if(e_PauseEffect == Enum_PauseEffect.GameOver)
        {
            #region End Game
            f_GameOver_Timer += Time.deltaTime;

            if(f_GameOver_Timer > f_GameOver_Timer_Max)
            {
                // Set block position
                if( i_GameOver_X < i_ArrayWidth - 1 &&
                    i_GameOver_Y < i_ArrayHeight )
                {
                    // Destroy the old. Set the new. All visual, no board array manipulation.
                    GameObject.Find("BoardDisplay").GetComponent<Cs_BoardDisplay>().DestroyBlockAt(new IntVector2(i_GameOver_X, i_GameOver_Y));
                    GameObject.Find("BoardDisplay").GetComponent<Cs_BoardDisplay>().Set_GameOverBlock( i_GameOver_X, i_GameOver_Y, e_GameOverBlock );
                }

                // Set next block type
                #region Next Block Type
                // Ensure that we're an odd number. Forces a checkerboard pattern.
                if(i_GameOver_X < i_ArrayWidth - 1)
                {
                    if (e_GameOverBlock == Enum_BlockType.Block_1_Active)
                    {
                        e_GameOverBlock = Enum_BlockType.Block_2_Active;
                    }
                    else if (e_GameOverBlock == Enum_BlockType.Block_2_Active)
                    {
                        if (b_ThreeBlockColors)
                        {
                            e_GameOverBlock = Enum_BlockType.Block_3_Active;
                        }
                        else
                        {
                            e_GameOverBlock = Enum_BlockType.Block_1_Active;
                        }
                    }
                    else // Block 3
                    {
                        e_GameOverBlock = Enum_BlockType.Block_1_Active;
                    }
                }
                else
                {
                    if (i_ArrayWidth % 2 == 0)
                    {
                        if (e_GameOverBlock == Enum_BlockType.Block_1_Active)
                        {
                            e_GameOverBlock = Enum_BlockType.Block_2_Active;
                        }
                        else if (e_GameOverBlock == Enum_BlockType.Block_2_Active)
                        {
                            if (b_ThreeBlockColors)
                            {
                                e_GameOverBlock = Enum_BlockType.Block_3_Active;
                            }
                            else
                            {
                                e_GameOverBlock = Enum_BlockType.Block_1_Active;
                            }
                        }
                        else
                        {
                            e_GameOverBlock = Enum_BlockType.Block_1_Active;
                        }
                    }
                }

                #endregion

                // Reset timer
                f_GameOver_Timer = 0f;

                // Increment X/Y
                ++i_GameOver_X;

                // When the GameOver_X position is greater than the inner walls...
                if(i_GameOver_X > i_ArrayWidth - 1)
                {
                    // Reset the X position
                    i_GameOver_X = 1;

                    // Increments the Y.
                    ++i_GameOver_Y;
                }
            }
            else
            {
                if (i_GameOver_X >= i_ArrayWidth - 1 && i_GameOver_Y >= i_ArrayHeight - 1)
                {
                    f_GameOver_FadeOut_Timer += Time.deltaTime;
                }
            }
            #endregion
            
            if(f_GameOver_FadeOut_Timer > 0f)
            {
                // Fade to black & return to main menu
                f_GameOver_FadeOut_Timer += Time.deltaTime;

                print(f_GameOver_FadeOut_Timer);

                if (f_GameOver_FadeOut_Timer > 5.0f)
                {
                    // Fade out the screen slowly
                    Color clr_CurrAlpha = go_FadeOut.GetComponent<Image>().color;

                    clr_CurrAlpha.a += Time.deltaTime / 3f;
                    float f_Perc = 1f - clr_CurrAlpha.a;
                    go_FadeOut.GetComponent<Image>().color = clr_CurrAlpha;

                    // Reduce volume to match
                    GameObject.Find("AudioSource_Music").GetComponent<AudioSource>().volume = f_Perc / 2.0f;
                }

                if (f_GameOver_FadeOut_Timer > 10f)
                {
                    SceneManager.LoadScene(2);
                }
            }
        }
    }
}
