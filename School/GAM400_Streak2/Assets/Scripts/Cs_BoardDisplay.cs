using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/*********************************
 * 
 * Copyright DigiPen Institute of Technology 2016
 * 
 * Streak 2 by Christopher Christensen
 * 
 * *******************************/

public class Cs_BoardDisplay : MonoBehaviour
{
    // The visual blocks we add to, change and move to show objects on the screen.
    Enum_BlockType[,] DisplayArray;
    GameObject[,] DisplayArray_Blocks;

    GameObject go_GridBlock;
    GameObject go_GridWall;
    GameObject go_GridWarning;

    GameObject go_Block_A;
    GameObject go_Block_B;
    GameObject go_Block_C;
    GameObject go_Empty;

    GameObject go_Camera;
    GameObject go_ThemeManager;

    int i_Height;
    int i_Width;

    bool b_IsDone;

    GameObject txt_Score;

    // Use this for initialization
    void Start()
    {
        txt_Score = GameObject.Find("Text_Score");
    }

    void LoadResources()
    {
        go_GridBlock = Resources.Load("GridBlock") as GameObject;
        go_GridWall = Resources.Load("GridBlock_Edge") as GameObject;
        go_GridWarning = Resources.Load("GridBlock_Warning") as GameObject;

        go_Block_A = Resources.Load("Block_X") as GameObject;
        go_Block_B = Resources.Load("Block_O") as GameObject;
        go_Block_C = Resources.Load("Block_Tri") as GameObject;

        go_Empty = Resources.Load("Block_Empty") as GameObject;

        go_Camera = GameObject.Find("Main Camera");
        go_ThemeManager = GameObject.Find("ThemeManager");
    }

    // Create a row of Grid objects
    List<List<GameObject>> Grid_Columns = new List<List<GameObject>>();
    List<GameObject> Grid_Row = new List<GameObject>();
    // When called from BoardLogic, create the grid background
    public void Init_Board(int i_Width_, int i_Height_, int i_MidWall_ = -1, Enum_BlockSize e_BlockSize_ = Enum_BlockSize.size_2w_2h)
    {
        i_Height = i_Height_;
        i_Width = i_Width_;

        LoadResources();

        #region Create Grid & GridWalls
        if (i_MidWall_ > 1 && i_MidWall_ < i_Width_ - 2)
        {

        }
        else // No midwall
        {
            for (int y_ = 0; y_ < i_Height; ++y_)
            {
                GameObject go_WallTemp;

                Grid_Row = new List<GameObject>();

                for (int x_ = 0; x_ < i_Width_; ++x_)
                {
                    // Instantiate the object
                    go_WallTemp = Instantiate(go_GridBlock);

                    // Add a GridWall
                    if (x_ == 0 || x_ == i_Width_ - 1)
                    {
                        // Instantiate the object
                        GameObject.Destroy(go_WallTemp); // Destroys the previous block that was created
                        go_WallTemp = Instantiate(go_GridWall);
                    }
                    else
                    {
                        #region 2 Wide
                        // Check for initial block positions
                        if(e_BlockSize_ == Enum_BlockSize.size_2w_2h || e_BlockSize_ == Enum_BlockSize.size_2w_3h)
                        {
                            // Left side
                            int i_xPos = (i_Width - 1) / 2;
                            if (x_ == i_xPos || x_ == i_xPos + 1)
                            {
                                if(e_BlockSize_ == Enum_BlockSize.size_2w_2h)
                                {
                                    if( y_ >= i_Height - 2)
                                    {
                                        go_WallTemp = Instantiate(go_GridWarning);
                                    }
                                }
                                else // 2x3
                                {
                                    if( y_ >= i_Height - 3)
                                    {
                                        go_WallTemp = Instantiate(go_GridWarning);
                                    }
                                }
                            }
                        }
                        #endregion
                        else if(e_BlockSize_ == Enum_BlockSize.size_3w_2h || e_BlockSize_ == Enum_BlockSize.size_3w_3h)
                        {
                            // Left side
                            int i_xPos = ((i_Width - 1) / 2) - 1;
                            if (x_ == i_xPos || x_ == i_xPos + 1 || x_ == i_xPos + 2 )
                            {
                                if (e_BlockSize_ == Enum_BlockSize.size_3w_2h)
                                {
                                    if (y_ >= i_Height - 2)
                                    {
                                        GameObject.Destroy(go_WallTemp); // Destroys the previous block that was created
                                        go_WallTemp = Instantiate(go_GridWarning);
                                    }
                                }
                                else // 2x3
                                {
                                    if (y_ >= i_Height - 3)
                                    {
                                        GameObject.Destroy(go_WallTemp); // Destroys the previous block that was created
                                        go_WallTemp = Instantiate(go_GridWarning);
                                    }
                                }
                            }
                        }
                    }
                    Grid_Row.Add(go_WallTemp);

                    // Set position
                    go_WallTemp.transform.position = new Vector3(x_ * go_GridBlock.transform.lossyScale.x, y_ * go_GridBlock.transform.lossyScale.y, 0);
                    go_WallTemp.transform.parent = GameObject.Find("GridBlockList").transform;
                }
                // Add the current Row to the Column List
                Grid_Columns.Add(Grid_Row);

            }

            // Reset the Row
            Grid_Row = new List<GameObject>();
            // Set the position
        }
        #endregion

        #region Populate 'Block Array' & 'Display Array' as empty
        
        DisplayArray = new Enum_BlockType[i_Height, i_Width];
        DisplayArray_Blocks = new GameObject[i_Height, i_Width];

        for (int y_ = 0; y_ < i_Height; ++y_)
        {
            for (int x_ = 0; x_ < i_Width; ++x_)
            {
                DisplayArray[y_, x_] = Enum_BlockType.Empty;

                DisplayArray_Blocks[y_, x_] = Instantiate(go_Empty);

                // Set parent (for Editor clutter)
                DisplayArray_Blocks[y_, x_].transform.SetParent(GameObject.Find("EmptyBlocks").transform);
            }
        }

        #endregion

        // Set camera position
        go_Camera.GetComponent<Cs_CameraController>().Init_CameraPosition(i_Width_, i_Height_, 3.0f);
    }

    // Update is called once per frame
    int i_X;
    int i_Y;
    int i_CurrMax;
    float f_InitBoardTimer;
    float f_InitBoardTimer_Max = 0.05f;
    void Update()
    {
        if (!b_IsDone)
        {
            CascadeTiles();
        }

        Set_ScoreTextIncrease();
    }

    void CascadeTiles()
    {
        f_InitBoardTimer += Time.deltaTime;

        if (f_InitBoardTimer >= f_InitBoardTimer_Max)
        {
            // Reset Timer
            f_InitBoardTimer = 0.0f;

            // Set new amounts
            // Cap X
            i_X = i_CurrMax;
            if (i_X > i_Width - 1) i_X = i_Width - 1;

            // Cap Y
            i_Y = i_CurrMax - i_X;
            if (i_Y > i_Height - 1) i_Y = i_Height - 1;

            Grid_Columns[i_Y][i_X].GetComponent<Cs_GridBlockLogic>().Set_FadeState(Enum_FadeState.FadeIn);

            // Run through incremental loop
            for (int i_ = 0; i_ < i_CurrMax; ++i_)
            {
                // As X decreases, Y increases.
                --i_X;
                i_Y = i_CurrMax - i_X;

                if (i_X < 0) i_X = 0;
                if (i_Y > i_Height - 1) i_Y = i_Height - 1;

                if (i_Y < i_Height && i_X < i_Width)
                {
                    Grid_Columns[i_Y][i_X].GetComponent<Cs_GridBlockLogic>().Set_FadeState(Enum_FadeState.FadeIn);
                }
            }

            if (i_CurrMax < i_Height + i_Width)
            {
                ++i_CurrMax;
            }
            else
            {
                b_IsDone = true;

                // i_X = 0;
                i_Y = 0;
                i_CurrMax = 0;
            }
        }
    }

    // WIP
    void CollapseTiles()
    {
        f_InitBoardTimer += Time.deltaTime;

        if (f_InitBoardTimer >= f_InitBoardTimer_Max)
        {
            // Reset Timer
            f_InitBoardTimer = 0.0f;

            // Set new amounts
            // Cap X
            i_X = i_CurrMax;
            if (i_X > i_Width - 1) i_X = i_Width - 1;

            // Cap Y
            i_Y = i_CurrMax - i_X;
            if (i_Y > i_Height - 1) i_Y = i_Height - 1;

            Grid_Columns[i_Y][i_X].GetComponent<Cs_GridBlockLogic>().Set_FadeState(Enum_FadeState.FadeOut);

            // Run through incremental loop
            for (int i_ = 0; i_ < i_CurrMax; ++i_)
            {
                // As X decreases, Y increases.
                --i_X;
                i_Y = i_CurrMax - i_X;

                if (i_X < 0) i_X = 0;
                if (i_Y > i_Height - 1) i_Y = i_Height - 1;

                if (i_Y < i_Height && i_X < i_Width)
                {
                    Grid_Columns[i_Y][i_X].GetComponent<Cs_GridBlockLogic>().Set_FadeState(Enum_FadeState.FadeIn);
                }
            }

            if (i_CurrMax < i_Height + i_Width)
            {
                ++i_CurrMax;
                print(i_CurrMax);
            }
            else
            {
                b_IsDone = false;

                // i_X = 0;
                i_Y = 0;
                i_CurrMax = 0;
            }
        }
    }

    public void MoveBlock_Dir( Enum_Direction e_MoveDir_, IntVector2 iv2_Pos_ )
    {
        if( e_MoveDir_ == Enum_Direction.Left )
        {
            // Push indicated block model to the left
            if (DisplayArray_Blocks[iv2_Pos_.y, iv2_Pos_.x].GetComponent<Cs_BlockOnBoardLogic>())
            {
                DisplayArray_Blocks[iv2_Pos_.y, iv2_Pos_.x].GetComponent<Cs_BlockOnBoardLogic>().Set_MoveLeft();
            }

            // Store block to left of this block
            Enum_BlockType e_TempBlock = DisplayArray[iv2_Pos_.y + 0, iv2_Pos_.x - 1];

            // Swap Blocks in Array
            DisplayArray[iv2_Pos_.y + 0, iv2_Pos_.x - 1] = DisplayArray[iv2_Pos_.y + 0, iv2_Pos_.x + 0];
            DisplayArray[iv2_Pos_.y + 0, iv2_Pos_.x + 0] = e_TempBlock;

            // Swap DisplayArray_Blocks in same manner
            GameObject go_TempBlock = DisplayArray_Blocks[iv2_Pos_.y, iv2_Pos_.x - 1];

            DisplayArray_Blocks[iv2_Pos_.y + 0, iv2_Pos_.x - 1] = DisplayArray_Blocks[iv2_Pos_.y + 0, iv2_Pos_.x + 0];
            DisplayArray_Blocks[iv2_Pos_.y + 0, iv2_Pos_.x + 0] = go_TempBlock;

            // Set the Backdrop Color
            Set_BackdropColor(DisplayArray[iv2_Pos_.y + 0, iv2_Pos_.x - 1], new IntVector2( iv2_Pos_.x - 1, iv2_Pos_.y + 0 ));
            Set_BackdropColor(DisplayArray[iv2_Pos_.y + 0, iv2_Pos_.x + 0], new IntVector2(iv2_Pos_.x + 0, iv2_Pos_.y + 0));
            if(iv2_Pos_.x + 1 < i_Width)
            {
                Set_BackdropColor(Enum_BlockType.Empty, new IntVector2(iv2_Pos_.x + 1, iv2_Pos_.y + 0));
            }
        }
        else if( e_MoveDir_ == Enum_Direction.Right )
        {
            // Push indicated block to the right
            if (DisplayArray_Blocks[iv2_Pos_.y + 0, iv2_Pos_.x + 0].GetComponent<Cs_BlockOnBoardLogic>())
            {
                DisplayArray_Blocks[iv2_Pos_.y + 0, iv2_Pos_.x - 0].GetComponent<Cs_BlockOnBoardLogic>().Set_MoveRight();
            }

            // Store block to the right of this block
            Enum_BlockType e_TempBlock = DisplayArray[iv2_Pos_.y + 0, iv2_Pos_.x + 1];

            // Swap Blocks in Array
            DisplayArray[iv2_Pos_.y + 0, iv2_Pos_.x + 1] = DisplayArray[iv2_Pos_.y + 0, iv2_Pos_.x - 0];
            DisplayArray[iv2_Pos_.y + 0, iv2_Pos_.x - 0] = e_TempBlock;

            // Swap DisplayArray_Blocks in same manner
            GameObject go_TempBlock = DisplayArray_Blocks[iv2_Pos_.y + 0, iv2_Pos_.x + 1];

            DisplayArray_Blocks[iv2_Pos_.y + 0, iv2_Pos_.x + 1] = DisplayArray_Blocks[iv2_Pos_.y + 0, iv2_Pos_.x - 0];
            DisplayArray_Blocks[iv2_Pos_.y + 0, iv2_Pos_.x - 0] = go_TempBlock;

            // Set the Backdrop Color
            Set_BackdropColor(DisplayArray[iv2_Pos_.y + 0, iv2_Pos_.x + 1], new IntVector2(iv2_Pos_.x + 1, iv2_Pos_.y + 0));
            Set_BackdropColor(DisplayArray[iv2_Pos_.y + 0, iv2_Pos_.x + 0], new IntVector2(iv2_Pos_.x + 0, iv2_Pos_.y + 0));
            if (iv2_Pos_.x - 1 > 0)
            {
                Set_BackdropColor(Enum_BlockType.Empty, new IntVector2(iv2_Pos_.x - 1, iv2_Pos_.y + 0));
            }
        }
        else if( e_MoveDir_ == Enum_Direction.Down )
        {
            // Push indicated block to the left
            if(DisplayArray_Blocks[iv2_Pos_.y + 0, iv2_Pos_.x - 0].GetComponent<Cs_BlockOnBoardLogic>())
            {
                DisplayArray_Blocks[iv2_Pos_.y + 0, iv2_Pos_.x - 0].GetComponent<Cs_BlockOnBoardLogic>().Set_MoveDown();
            }
            
            // Store block to the right of this block
            Enum_BlockType e_TempBlock = DisplayArray[iv2_Pos_.y - 1, iv2_Pos_.x + 0];

            // Swap Blocks in Array
            DisplayArray[iv2_Pos_.y - 1, iv2_Pos_.x + 0] = DisplayArray[iv2_Pos_.y + 0, iv2_Pos_.x - 0];
            DisplayArray[iv2_Pos_.y + 0, iv2_Pos_.x - 0] = e_TempBlock;

            // Swap DisplayArray_Blocks in same manner
            GameObject go_TempBlock = DisplayArray_Blocks[iv2_Pos_.y - 1, iv2_Pos_.x + 0];

            DisplayArray_Blocks[iv2_Pos_.y - 1, iv2_Pos_.x + 0] = DisplayArray_Blocks[iv2_Pos_.y + 0, iv2_Pos_.x - 0];
            DisplayArray_Blocks[iv2_Pos_.y + 0, iv2_Pos_.x - 0] = go_TempBlock;

            // Set the Backdrop Color
            Set_BackdropColor(DisplayArray[iv2_Pos_.y - 1, iv2_Pos_.x + 0], new IntVector2(iv2_Pos_.x + 0, iv2_Pos_.y - 1));
            Set_BackdropColor(DisplayArray[iv2_Pos_.y + 0, iv2_Pos_.x + 0], new IntVector2(iv2_Pos_.x + 0, iv2_Pos_.y + 0));
            if(iv2_Pos_.y + 1 < i_Height)
            {
                Set_BackdropColor(Enum_BlockType.Empty, new IntVector2(iv2_Pos_.x + 0, iv2_Pos_.y + 1));
            }
        }
        else if(e_MoveDir_ == Enum_Direction.Up )
        {
            // Push indicated block up
            if (DisplayArray_Blocks[iv2_Pos_.y + 0, iv2_Pos_.x + 0].GetComponent<Cs_BlockOnBoardLogic>())
            {
                DisplayArray_Blocks[iv2_Pos_.y + 0, iv2_Pos_.x - 0].GetComponent<Cs_BlockOnBoardLogic>().Set_MoveUp();
            }

            // Store block above this block
            Enum_BlockType e_TempBlock = DisplayArray[iv2_Pos_.y + 1, iv2_Pos_.x + 0];

            // Swap Blocks in Array
            DisplayArray[iv2_Pos_.y + 1, iv2_Pos_.x + 0] = DisplayArray[iv2_Pos_.y + 0, iv2_Pos_.x - 0];
            DisplayArray[iv2_Pos_.y + 0, iv2_Pos_.x - 0] = e_TempBlock;

            // Swap DisplayArray_Blocks in same manner
            GameObject go_TempBlock = DisplayArray_Blocks[iv2_Pos_.y + 1, iv2_Pos_.x + 0];

            DisplayArray_Blocks[iv2_Pos_.y + 1, iv2_Pos_.x + 0] = DisplayArray_Blocks[iv2_Pos_.y + 0, iv2_Pos_.x - 0];
            DisplayArray_Blocks[iv2_Pos_.y + 0, iv2_Pos_.x - 0] = go_TempBlock;

            // Set the Backdrop Color
            Set_BackdropColor(DisplayArray[iv2_Pos_.y + 1, iv2_Pos_.x + 0], new IntVector2(iv2_Pos_.x + 0, iv2_Pos_.y + 1));
            Set_BackdropColor(DisplayArray[iv2_Pos_.y + 0, iv2_Pos_.x + 0], new IntVector2(iv2_Pos_.x + 0, iv2_Pos_.y + 0));
            Set_BackdropColor(Enum_BlockType.Empty, new IntVector2(iv2_Pos_.x + 0, iv2_Pos_.y - 1));
        }
    }

    public void ShiftBlocks( Enum_Direction e_MoveDir_, Enum_BlockSize e_BlockSize_, IntVector2 iv2_BottomLeft_)
    {
        if( e_MoveDir_ == Enum_Direction.Left )
        {
            #region Shift Left
            // Shift original 2x2 left
            MoveBlock_Dir(Enum_Direction.Left, iv2_BottomLeft_);
            MoveBlock_Dir(Enum_Direction.Left, new IntVector2(iv2_BottomLeft_.x + 1, iv2_BottomLeft_.y + 0));
            MoveBlock_Dir(Enum_Direction.Left, new IntVector2(iv2_BottomLeft_.x + 0, iv2_BottomLeft_.y + 1));
            MoveBlock_Dir(Enum_Direction.Left, new IntVector2(iv2_BottomLeft_.x + 1, iv2_BottomLeft_.y + 1));

            // 3 wide
            if(e_BlockSize_ == Enum_BlockSize.size_3w_2h || e_BlockSize_ == Enum_BlockSize.size_3w_3h)
            {
                MoveBlock_Dir(Enum_Direction.Left, new IntVector2(iv2_BottomLeft_.x + 2, iv2_BottomLeft_.y + 0));
                MoveBlock_Dir(Enum_Direction.Left, new IntVector2(iv2_BottomLeft_.x + 2, iv2_BottomLeft_.y + 1));
            }

            // 3 high
            if (e_BlockSize_ == Enum_BlockSize.size_2w_3h || e_BlockSize_ == Enum_BlockSize.size_3w_3h)
            {
                MoveBlock_Dir(Enum_Direction.Left, new IntVector2(iv2_BottomLeft_.x + 0, iv2_BottomLeft_.y + 2));
                MoveBlock_Dir(Enum_Direction.Left, new IntVector2(iv2_BottomLeft_.x + 1, iv2_BottomLeft_.y + 2));
            }

            // 3x3
            if(e_BlockSize_ == Enum_BlockSize.size_3w_3h)
            {
                MoveBlock_Dir(Enum_Direction.Left, new IntVector2(iv2_BottomLeft_.x + 2, iv2_BottomLeft_.y + 2));
            }
            #endregion
        }
        else if( e_MoveDir_ == Enum_Direction.Right )
        {
            #region Shift Right
            // Order is jumbled to allow blocks to not overwrite one-another

            // 3x3
            if (e_BlockSize_ == Enum_BlockSize.size_3w_3h)
            {
                MoveBlock_Dir(Enum_Direction.Right, new IntVector2(iv2_BottomLeft_.x + 2, iv2_BottomLeft_.y + 2));
            }

            // 3 wide
            if (e_BlockSize_ == Enum_BlockSize.size_3w_2h || e_BlockSize_ == Enum_BlockSize.size_3w_3h)
            {
                MoveBlock_Dir(Enum_Direction.Right, new IntVector2(iv2_BottomLeft_.x + 2, iv2_BottomLeft_.y + 0));
                MoveBlock_Dir(Enum_Direction.Right, new IntVector2(iv2_BottomLeft_.x + 2, iv2_BottomLeft_.y + 1));
            }

            // 3 high
            if (e_BlockSize_ == Enum_BlockSize.size_2w_3h || e_BlockSize_ == Enum_BlockSize.size_3w_3h)
            {
                MoveBlock_Dir(Enum_Direction.Right, new IntVector2(iv2_BottomLeft_.x + 1, iv2_BottomLeft_.y + 2));
                MoveBlock_Dir(Enum_Direction.Right, new IntVector2(iv2_BottomLeft_.x + 0, iv2_BottomLeft_.y + 2));
            }

            // Shift original 2x2 left
            MoveBlock_Dir(Enum_Direction.Right, new IntVector2(iv2_BottomLeft_.x + 1, iv2_BottomLeft_.y + 0));
            MoveBlock_Dir(Enum_Direction.Right, new IntVector2(iv2_BottomLeft_.x + 1, iv2_BottomLeft_.y + 1));
            MoveBlock_Dir(Enum_Direction.Right, iv2_BottomLeft_);
            MoveBlock_Dir(Enum_Direction.Right, new IntVector2(iv2_BottomLeft_.x + 0, iv2_BottomLeft_.y + 1));
            #endregion
        }
        else if( e_MoveDir_ == Enum_Direction.Down )
        {
            #region Shift Down
            // Shift original 2x2 left
            MoveBlock_Dir(Enum_Direction.Down, iv2_BottomLeft_);
            MoveBlock_Dir(Enum_Direction.Down, new IntVector2(iv2_BottomLeft_.x + 1, iv2_BottomLeft_.y + 0));
            MoveBlock_Dir(Enum_Direction.Down, new IntVector2(iv2_BottomLeft_.x + 0, iv2_BottomLeft_.y + 1));
            MoveBlock_Dir(Enum_Direction.Down, new IntVector2(iv2_BottomLeft_.x + 1, iv2_BottomLeft_.y + 1));

            // 3 wide
            if (e_BlockSize_ == Enum_BlockSize.size_3w_2h || e_BlockSize_ == Enum_BlockSize.size_3w_3h)
            {
                MoveBlock_Dir(Enum_Direction.Down, new IntVector2(iv2_BottomLeft_.x + 2, iv2_BottomLeft_.y + 0));
                MoveBlock_Dir(Enum_Direction.Down, new IntVector2(iv2_BottomLeft_.x + 2, iv2_BottomLeft_.y + 1));
            }

            // 3 high
            if (e_BlockSize_ == Enum_BlockSize.size_2w_3h || e_BlockSize_ == Enum_BlockSize.size_3w_3h)
            {
                MoveBlock_Dir(Enum_Direction.Down, new IntVector2(iv2_BottomLeft_.x + 0, iv2_BottomLeft_.y + 2));
                MoveBlock_Dir(Enum_Direction.Down, new IntVector2(iv2_BottomLeft_.x + 1, iv2_BottomLeft_.y + 2));
            }

            // 3x3
            if (e_BlockSize_ == Enum_BlockSize.size_3w_3h)
            {
                MoveBlock_Dir(Enum_Direction.Down, new IntVector2(iv2_BottomLeft_.x + 2, iv2_BottomLeft_.y + 2));
            }
            #endregion
        }

        // Play Sound Effect
        go_ThemeManager.GetComponent<Cs_AudioManager>().Play_SoundEffect(Enum_SoundEffect.Move);
    }
    public void ShiftBlocks( Enum_Direction e_MoveDir_, Enum_BlockSize e_BlockSize_, Vector2 v2_BottomLeft_ )
    {
        IntVector2 iv2_BottomLeft = new IntVector2( (int)v2_BottomLeft_.x, (int)v2_BottomLeft_.y );

        ShiftBlocks(e_MoveDir_, e_BlockSize_, iv2_BottomLeft);
    }

    public void RotateBlocks(Enum_Direction e_RotDir_, Enum_BlockSize e_BlockSize_, IntVector2 iv2_BottomLeft_)
    {
        // Store Bottom Left
        Enum_BlockType e_BotLeftBlock = DisplayArray[iv2_BottomLeft_.y, iv2_BottomLeft_.x];
        GameObject go_BotLeftBlock = DisplayArray_Blocks[iv2_BottomLeft_.y, iv2_BottomLeft_.x];

        // Only continue if a block exists
        if(DisplayArray[iv2_BottomLeft_.y, iv2_BottomLeft_.x] == Enum_BlockType.Empty) return;

        if (e_RotDir_ == Enum_Direction.Left)
        {
            #region Shift Left (Counter-clockwise)
            if( e_BlockSize_ == Enum_BlockSize.size_2w_2h)
            {
                // Move Blocks Visually
                DisplayArray_Blocks[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 0].GetComponent<Cs_BlockOnBoardLogic>().Set_MoveDown();
                DisplayArray_Blocks[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 1].GetComponent<Cs_BlockOnBoardLogic>().Set_MoveLeft();
                DisplayArray_Blocks[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 1].GetComponent<Cs_BlockOnBoardLogic>().Set_MoveUp();
                DisplayArray_Blocks[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 0].GetComponent<Cs_BlockOnBoardLogic>().Set_MoveRight();

                // Re-arrange the blocks in the DisplayArray_Blocks to match
                DisplayArray_Blocks[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 0] = DisplayArray_Blocks[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 0];
                DisplayArray_Blocks[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 0] = DisplayArray_Blocks[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 1];
                DisplayArray_Blocks[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 1] = DisplayArray_Blocks[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 1];
                DisplayArray_Blocks[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 1] = go_BotLeftBlock;

                // Re-arrange the blocks in the Display Array to match
                DisplayArray[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 0] = DisplayArray[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 0];
                DisplayArray[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 0] = DisplayArray[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 1];
                DisplayArray[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 1] = DisplayArray[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 1];
                DisplayArray[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 1] = e_BotLeftBlock;

                // Change the Grid_Backdrop
                Set_BackdropColor(DisplayArray[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 0], new IntVector2(iv2_BottomLeft_.x + 0, iv2_BottomLeft_.y + 0));
                Set_BackdropColor(DisplayArray[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 0], new IntVector2(iv2_BottomLeft_.x + 0, iv2_BottomLeft_.y + 1));
                Set_BackdropColor(DisplayArray[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 1], new IntVector2(iv2_BottomLeft_.x + 1, iv2_BottomLeft_.y + 1));
                Set_BackdropColor(DisplayArray[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 1], new IntVector2(iv2_BottomLeft_.x + 1, iv2_BottomLeft_.y + 0));
            }
            else if( e_BlockSize_ == Enum_BlockSize.size_3w_2h )
            {
                // Move Blocks Visually
                DisplayArray_Blocks[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 0].GetComponent<Cs_BlockOnBoardLogic>().Set_MoveRight();
                DisplayArray_Blocks[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 1].GetComponent<Cs_BlockOnBoardLogic>().Set_MoveRight();
                DisplayArray_Blocks[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 2].GetComponent<Cs_BlockOnBoardLogic>().Set_MoveUp();
                DisplayArray_Blocks[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 2].GetComponent<Cs_BlockOnBoardLogic>().Set_MoveLeft();
                DisplayArray_Blocks[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 1].GetComponent<Cs_BlockOnBoardLogic>().Set_MoveLeft();
                DisplayArray_Blocks[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 0].GetComponent<Cs_BlockOnBoardLogic>().Set_MoveDown();

                // Re-arrange the blocks in the DisplayArray_Blocks to match
                DisplayArray_Blocks[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 0] = DisplayArray_Blocks[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 0];
                DisplayArray_Blocks[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 0] = DisplayArray_Blocks[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 1];
                DisplayArray_Blocks[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 1] = DisplayArray_Blocks[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 2];
                DisplayArray_Blocks[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 2] = DisplayArray_Blocks[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 2];
                DisplayArray_Blocks[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 2] = DisplayArray_Blocks[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 1];
                DisplayArray_Blocks[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 1] = go_BotLeftBlock;

                // Re-arrange the blocks in the Display Array to match
                DisplayArray[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 0] = DisplayArray[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 0];
                DisplayArray[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 0] = DisplayArray[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 1];
                DisplayArray[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 1] = DisplayArray[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 2];
                DisplayArray[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 2] = DisplayArray[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 2];
                DisplayArray[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 2] = DisplayArray[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 1];
                DisplayArray[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 1] = e_BotLeftBlock;

                // Change the Grid_Backdrop
                Set_BackdropColor(DisplayArray[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 0], new IntVector2(iv2_BottomLeft_.x + 0, iv2_BottomLeft_.y + 0));
                Set_BackdropColor(DisplayArray[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 0], new IntVector2(iv2_BottomLeft_.x + 0, iv2_BottomLeft_.y + 1));
                Set_BackdropColor(DisplayArray[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 1], new IntVector2(iv2_BottomLeft_.x + 1, iv2_BottomLeft_.y + 1));
                Set_BackdropColor(DisplayArray[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 2], new IntVector2(iv2_BottomLeft_.x + 2, iv2_BottomLeft_.y + 1));
                Set_BackdropColor(DisplayArray[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 2], new IntVector2(iv2_BottomLeft_.x + 2, iv2_BottomLeft_.y + 0));
                Set_BackdropColor(DisplayArray[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 1], new IntVector2(iv2_BottomLeft_.x + 1, iv2_BottomLeft_.y + 0));
            }
            else if( e_BlockSize_ == Enum_BlockSize.size_2w_3h )
            {
                // Move Blocks Visually
                DisplayArray_Blocks[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 0].GetComponent<Cs_BlockOnBoardLogic>().Set_MoveRight();
                DisplayArray_Blocks[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 1].GetComponent<Cs_BlockOnBoardLogic>().Set_MoveUp();
                DisplayArray_Blocks[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 1].GetComponent<Cs_BlockOnBoardLogic>().Set_MoveUp();
                DisplayArray_Blocks[iv2_BottomLeft_.y + 2, iv2_BottomLeft_.x + 1].GetComponent<Cs_BlockOnBoardLogic>().Set_MoveLeft();
                DisplayArray_Blocks[iv2_BottomLeft_.y + 2, iv2_BottomLeft_.x + 0].GetComponent<Cs_BlockOnBoardLogic>().Set_MoveDown();
                DisplayArray_Blocks[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 0].GetComponent<Cs_BlockOnBoardLogic>().Set_MoveDown();

                // Re-arrange the blocks in the DisplayArray_Blocks to match
                DisplayArray_Blocks[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 0] = DisplayArray_Blocks[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 0];
                DisplayArray_Blocks[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 0] = DisplayArray_Blocks[iv2_BottomLeft_.y + 2, iv2_BottomLeft_.x + 0];
                DisplayArray_Blocks[iv2_BottomLeft_.y + 2, iv2_BottomLeft_.x + 0] = DisplayArray_Blocks[iv2_BottomLeft_.y + 2, iv2_BottomLeft_.x + 1];
                DisplayArray_Blocks[iv2_BottomLeft_.y + 2, iv2_BottomLeft_.x + 1] = DisplayArray_Blocks[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 1];
                DisplayArray_Blocks[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 1] = DisplayArray_Blocks[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 1];
                DisplayArray_Blocks[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 1] = go_BotLeftBlock;

                // Re-arrange the blocks in the Display Array to match
                DisplayArray[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 0] = DisplayArray[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 0];
                DisplayArray[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 0] = DisplayArray[iv2_BottomLeft_.y + 2, iv2_BottomLeft_.x + 0];
                DisplayArray[iv2_BottomLeft_.y + 2, iv2_BottomLeft_.x + 0] = DisplayArray[iv2_BottomLeft_.y + 2, iv2_BottomLeft_.x + 1];
                DisplayArray[iv2_BottomLeft_.y + 2, iv2_BottomLeft_.x + 1] = DisplayArray[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 1];
                DisplayArray[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 1] = DisplayArray[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 1];
                DisplayArray[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 1] = e_BotLeftBlock;

                // Change the Grid_Backdrop
                Set_BackdropColor(DisplayArray[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 0], new IntVector2(iv2_BottomLeft_.x + 0, iv2_BottomLeft_.y + 0));
                Set_BackdropColor(DisplayArray[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 0], new IntVector2(iv2_BottomLeft_.x + 0, iv2_BottomLeft_.y + 1));
                Set_BackdropColor(DisplayArray[iv2_BottomLeft_.y + 2, iv2_BottomLeft_.x + 0], new IntVector2(iv2_BottomLeft_.x + 0, iv2_BottomLeft_.y + 2));
                Set_BackdropColor(DisplayArray[iv2_BottomLeft_.y + 2, iv2_BottomLeft_.x + 1], new IntVector2(iv2_BottomLeft_.x + 1, iv2_BottomLeft_.y + 2));
                Set_BackdropColor(DisplayArray[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 1], new IntVector2(iv2_BottomLeft_.x + 1, iv2_BottomLeft_.y + 1));
                Set_BackdropColor(DisplayArray[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 1], new IntVector2(iv2_BottomLeft_.x + 1, iv2_BottomLeft_.y + 0));
            }
            else if( e_BlockSize_ == Enum_BlockSize.size_3w_3h )
            {
                // Move Blocks Visually
                DisplayArray_Blocks[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 0].GetComponent<Cs_BlockOnBoardLogic>().Set_MoveRight();
                DisplayArray_Blocks[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 1].GetComponent<Cs_BlockOnBoardLogic>().Set_MoveRight();
                DisplayArray_Blocks[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 2].GetComponent<Cs_BlockOnBoardLogic>().Set_MoveUp();
                DisplayArray_Blocks[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 2].GetComponent<Cs_BlockOnBoardLogic>().Set_MoveUp();
                DisplayArray_Blocks[iv2_BottomLeft_.y + 2, iv2_BottomLeft_.x + 2].GetComponent<Cs_BlockOnBoardLogic>().Set_MoveLeft();
                DisplayArray_Blocks[iv2_BottomLeft_.y + 2, iv2_BottomLeft_.x + 1].GetComponent<Cs_BlockOnBoardLogic>().Set_MoveLeft();
                DisplayArray_Blocks[iv2_BottomLeft_.y + 2, iv2_BottomLeft_.x + 0].GetComponent<Cs_BlockOnBoardLogic>().Set_MoveDown();
                DisplayArray_Blocks[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 0].GetComponent<Cs_BlockOnBoardLogic>().Set_MoveDown();

                // Re-arrange the blocks in the DisplayArray_Blocks to match
                DisplayArray_Blocks[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 0] = DisplayArray_Blocks[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 0];
                DisplayArray_Blocks[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 0] = DisplayArray_Blocks[iv2_BottomLeft_.y + 2, iv2_BottomLeft_.x + 0];
                DisplayArray_Blocks[iv2_BottomLeft_.y + 2, iv2_BottomLeft_.x + 0] = DisplayArray_Blocks[iv2_BottomLeft_.y + 2, iv2_BottomLeft_.x + 1];
                DisplayArray_Blocks[iv2_BottomLeft_.y + 2, iv2_BottomLeft_.x + 1] = DisplayArray_Blocks[iv2_BottomLeft_.y + 2, iv2_BottomLeft_.x + 2];
                DisplayArray_Blocks[iv2_BottomLeft_.y + 2, iv2_BottomLeft_.x + 2] = DisplayArray_Blocks[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 2];
                DisplayArray_Blocks[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 2] = DisplayArray_Blocks[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 2];
                DisplayArray_Blocks[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 2] = DisplayArray_Blocks[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 1];
                DisplayArray_Blocks[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 1] = go_BotLeftBlock;

                // Re-arrange the blocks in the Display Array to match
                DisplayArray[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 0] = DisplayArray[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 0];
                DisplayArray[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 0] = DisplayArray[iv2_BottomLeft_.y + 2, iv2_BottomLeft_.x + 0];
                DisplayArray[iv2_BottomLeft_.y + 2, iv2_BottomLeft_.x + 0] = DisplayArray[iv2_BottomLeft_.y + 2, iv2_BottomLeft_.x + 1];
                DisplayArray[iv2_BottomLeft_.y + 2, iv2_BottomLeft_.x + 1] = DisplayArray[iv2_BottomLeft_.y + 2, iv2_BottomLeft_.x + 2];
                DisplayArray[iv2_BottomLeft_.y + 2, iv2_BottomLeft_.x + 2] = DisplayArray[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 2];
                DisplayArray[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 2] = DisplayArray[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 2];
                DisplayArray[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 2] = DisplayArray[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 1];
                DisplayArray[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 1] = e_BotLeftBlock;

                // Change the Grid_Backdrop
                Set_BackdropColor(DisplayArray[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 0], new IntVector2(iv2_BottomLeft_.x + 0, iv2_BottomLeft_.y + 0));
                Set_BackdropColor(DisplayArray[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 0], new IntVector2(iv2_BottomLeft_.x + 0, iv2_BottomLeft_.y + 1));
                Set_BackdropColor(DisplayArray[iv2_BottomLeft_.y + 2, iv2_BottomLeft_.x + 0], new IntVector2(iv2_BottomLeft_.x + 0, iv2_BottomLeft_.y + 2));
                Set_BackdropColor(DisplayArray[iv2_BottomLeft_.y + 2, iv2_BottomLeft_.x + 1], new IntVector2(iv2_BottomLeft_.x + 1, iv2_BottomLeft_.y + 2));
                Set_BackdropColor(DisplayArray[iv2_BottomLeft_.y + 2, iv2_BottomLeft_.x + 2], new IntVector2(iv2_BottomLeft_.x + 2, iv2_BottomLeft_.y + 2));
                Set_BackdropColor(DisplayArray[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 2], new IntVector2(iv2_BottomLeft_.x + 2, iv2_BottomLeft_.y + 1));
                Set_BackdropColor(DisplayArray[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 2], new IntVector2(iv2_BottomLeft_.x + 2, iv2_BottomLeft_.y + 0));
                Set_BackdropColor(DisplayArray[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 1], new IntVector2(iv2_BottomLeft_.x + 1, iv2_BottomLeft_.y + 0));
                Set_BackdropColor(DisplayArray[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 1], new IntVector2(iv2_BottomLeft_.x + 1, iv2_BottomLeft_.y + 1));
            }

            // Play Sound Effect
            go_ThemeManager.GetComponent<Cs_AudioManager>().Play_SoundEffect(Enum_SoundEffect.RotateCounterclock);
            #endregion
        }
        else if( e_RotDir_ == Enum_Direction.Right )
        {
            #region Shift Right (Clockwise)
            if( e_BlockSize_ == Enum_BlockSize.size_2w_2h)
            { 
                // Move Blocks Visually
                DisplayArray_Blocks[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 0].GetComponent<Cs_BlockOnBoardLogic>().Set_MoveUp();
                DisplayArray_Blocks[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 0].GetComponent<Cs_BlockOnBoardLogic>().Set_MoveRight();
                DisplayArray_Blocks[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 1].GetComponent<Cs_BlockOnBoardLogic>().Set_MoveDown();
                DisplayArray_Blocks[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 1].GetComponent<Cs_BlockOnBoardLogic>().Set_MoveLeft();

                // Re-arrange the blocks in the DisplayArray_Blocks to match
                DisplayArray_Blocks[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 0] = DisplayArray_Blocks[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 1];
                DisplayArray_Blocks[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 1] = DisplayArray_Blocks[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 1];
                DisplayArray_Blocks[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 1] = DisplayArray_Blocks[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 0];
                DisplayArray_Blocks[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 0] = go_BotLeftBlock;

                // Re-arrange the blocks in the Display Array to match
                DisplayArray[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 0] = DisplayArray[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 1];
                DisplayArray[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 1] = DisplayArray[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 1];
                DisplayArray[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 1] = DisplayArray[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 0];
                DisplayArray[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 0] = e_BotLeftBlock;

                // Change the Grid_Backdrop
                Set_BackdropColor(DisplayArray[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 0], new IntVector2(iv2_BottomLeft_.x + 0, iv2_BottomLeft_.y + 0));
                Set_BackdropColor(DisplayArray[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 0], new IntVector2(iv2_BottomLeft_.x + 0, iv2_BottomLeft_.y + 1));
                Set_BackdropColor(DisplayArray[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 1], new IntVector2(iv2_BottomLeft_.x + 1, iv2_BottomLeft_.y + 1));
                Set_BackdropColor(DisplayArray[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 1], new IntVector2(iv2_BottomLeft_.x + 1, iv2_BottomLeft_.y + 0));
            }
            else if( e_BlockSize_ == Enum_BlockSize.size_3w_2h )
            {
                // Move Blocks Visually
                DisplayArray_Blocks[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 0].GetComponent<Cs_BlockOnBoardLogic>().Set_MoveUp();
                DisplayArray_Blocks[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 1].GetComponent<Cs_BlockOnBoardLogic>().Set_MoveLeft();
                DisplayArray_Blocks[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 2].GetComponent<Cs_BlockOnBoardLogic>().Set_MoveLeft();
                DisplayArray_Blocks[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 2].GetComponent<Cs_BlockOnBoardLogic>().Set_MoveDown();
                DisplayArray_Blocks[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 1].GetComponent<Cs_BlockOnBoardLogic>().Set_MoveRight();
                DisplayArray_Blocks[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 0].GetComponent<Cs_BlockOnBoardLogic>().Set_MoveRight();

                // Re-arrange the blocks in the DisplayArray_Blocks to match
                DisplayArray_Blocks[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 0] = DisplayArray_Blocks[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 1];
                DisplayArray_Blocks[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 1] = DisplayArray_Blocks[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 2];
                DisplayArray_Blocks[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 2] = DisplayArray_Blocks[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 2];
                DisplayArray_Blocks[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 2] = DisplayArray_Blocks[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 1];
                DisplayArray_Blocks[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 1] = DisplayArray_Blocks[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 0];
                DisplayArray_Blocks[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 0] = go_BotLeftBlock;

                // Re-arrange the blocks in the Display Array to match
                DisplayArray[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 0] = DisplayArray[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 1];
                DisplayArray[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 1] = DisplayArray[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 2];
                DisplayArray[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 2] = DisplayArray[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 2];
                DisplayArray[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 2] = DisplayArray[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 1];
                DisplayArray[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 1] = DisplayArray[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 0];
                DisplayArray[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 0] = e_BotLeftBlock;

                // Change the Grid_Backdrop
                Set_BackdropColor(DisplayArray[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 0], new IntVector2(iv2_BottomLeft_.x + 0, iv2_BottomLeft_.y + 0));
                Set_BackdropColor(DisplayArray[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 0], new IntVector2(iv2_BottomLeft_.x + 0, iv2_BottomLeft_.y + 1));
                Set_BackdropColor(DisplayArray[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 1], new IntVector2(iv2_BottomLeft_.x + 1, iv2_BottomLeft_.y + 1));
                Set_BackdropColor(DisplayArray[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 2], new IntVector2(iv2_BottomLeft_.x + 2, iv2_BottomLeft_.y + 1));
                Set_BackdropColor(DisplayArray[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 2], new IntVector2(iv2_BottomLeft_.x + 2, iv2_BottomLeft_.y + 0));
                Set_BackdropColor(DisplayArray[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 1], new IntVector2(iv2_BottomLeft_.x + 1, iv2_BottomLeft_.y + 0));
            }
            else if( e_BlockSize_ == Enum_BlockSize.size_2w_3h )
            {
                // Move Blocks Visually (Clockwise)
                DisplayArray_Blocks[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 0].GetComponent<Cs_BlockOnBoardLogic>().Set_MoveUp();
                DisplayArray_Blocks[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 0].GetComponent<Cs_BlockOnBoardLogic>().Set_MoveUp();
                DisplayArray_Blocks[iv2_BottomLeft_.y + 2, iv2_BottomLeft_.x + 0].GetComponent<Cs_BlockOnBoardLogic>().Set_MoveRight();
                DisplayArray_Blocks[iv2_BottomLeft_.y + 2, iv2_BottomLeft_.x + 1].GetComponent<Cs_BlockOnBoardLogic>().Set_MoveDown();
                DisplayArray_Blocks[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 1].GetComponent<Cs_BlockOnBoardLogic>().Set_MoveDown();
                DisplayArray_Blocks[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 1].GetComponent<Cs_BlockOnBoardLogic>().Set_MoveLeft();

                // Re-arrange the blocks in the DisplayArray_Blocks to match
                DisplayArray_Blocks[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 0] = DisplayArray_Blocks[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 1];
                DisplayArray_Blocks[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 1] = DisplayArray_Blocks[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 1];
                DisplayArray_Blocks[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 1] = DisplayArray_Blocks[iv2_BottomLeft_.y + 2, iv2_BottomLeft_.x + 1];
                DisplayArray_Blocks[iv2_BottomLeft_.y + 2, iv2_BottomLeft_.x + 1] = DisplayArray_Blocks[iv2_BottomLeft_.y + 2, iv2_BottomLeft_.x + 0];
                DisplayArray_Blocks[iv2_BottomLeft_.y + 2, iv2_BottomLeft_.x + 0] = DisplayArray_Blocks[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 0];
                DisplayArray_Blocks[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 0] = go_BotLeftBlock;

                // Re-arrange the blocks in the Display Array to match
                DisplayArray[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 0] = DisplayArray[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 1];
                DisplayArray[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 1] = DisplayArray[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 1];
                DisplayArray[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 1] = DisplayArray[iv2_BottomLeft_.y + 2, iv2_BottomLeft_.x + 1];
                DisplayArray[iv2_BottomLeft_.y + 2, iv2_BottomLeft_.x + 1] = DisplayArray[iv2_BottomLeft_.y + 2, iv2_BottomLeft_.x + 0];
                DisplayArray[iv2_BottomLeft_.y + 2, iv2_BottomLeft_.x + 0] = DisplayArray[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 0];
                DisplayArray[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 0] = e_BotLeftBlock;

                // Change the Grid_Backdrop
                Set_BackdropColor(DisplayArray[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 0], new IntVector2(iv2_BottomLeft_.x + 0, iv2_BottomLeft_.y + 0));
                Set_BackdropColor(DisplayArray[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 0], new IntVector2(iv2_BottomLeft_.x + 0, iv2_BottomLeft_.y + 1));
                Set_BackdropColor(DisplayArray[iv2_BottomLeft_.y + 2, iv2_BottomLeft_.x + 0], new IntVector2(iv2_BottomLeft_.x + 0, iv2_BottomLeft_.y + 2));
                Set_BackdropColor(DisplayArray[iv2_BottomLeft_.y + 2, iv2_BottomLeft_.x + 1], new IntVector2(iv2_BottomLeft_.x + 1, iv2_BottomLeft_.y + 2));
                Set_BackdropColor(DisplayArray[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 1], new IntVector2(iv2_BottomLeft_.x + 1, iv2_BottomLeft_.y + 1));
                Set_BackdropColor(DisplayArray[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 1], new IntVector2(iv2_BottomLeft_.x + 1, iv2_BottomLeft_.y + 0));
            }
            else if( e_BlockSize_ == Enum_BlockSize.size_3w_3h )
            {
                // Move Blocks Visually (Clockwise)
                DisplayArray_Blocks[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 0].GetComponent<Cs_BlockOnBoardLogic>().Set_MoveUp();
                DisplayArray_Blocks[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 0].GetComponent<Cs_BlockOnBoardLogic>().Set_MoveUp();
                DisplayArray_Blocks[iv2_BottomLeft_.y + 2, iv2_BottomLeft_.x + 0].GetComponent<Cs_BlockOnBoardLogic>().Set_MoveRight();
                DisplayArray_Blocks[iv2_BottomLeft_.y + 2, iv2_BottomLeft_.x + 1].GetComponent<Cs_BlockOnBoardLogic>().Set_MoveRight();
                DisplayArray_Blocks[iv2_BottomLeft_.y + 2, iv2_BottomLeft_.x + 2].GetComponent<Cs_BlockOnBoardLogic>().Set_MoveDown();
                DisplayArray_Blocks[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 2].GetComponent<Cs_BlockOnBoardLogic>().Set_MoveDown();
                DisplayArray_Blocks[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 2].GetComponent<Cs_BlockOnBoardLogic>().Set_MoveLeft();
                DisplayArray_Blocks[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 1].GetComponent<Cs_BlockOnBoardLogic>().Set_MoveLeft();

                // Re-arrange the blocks in the DisplayArray_Blocks to match
                DisplayArray_Blocks[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 0] = DisplayArray_Blocks[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 1];
                DisplayArray_Blocks[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 1] = DisplayArray_Blocks[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 2];
                DisplayArray_Blocks[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 2] = DisplayArray_Blocks[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 2];
                DisplayArray_Blocks[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 2] = DisplayArray_Blocks[iv2_BottomLeft_.y + 2, iv2_BottomLeft_.x + 2];
                DisplayArray_Blocks[iv2_BottomLeft_.y + 2, iv2_BottomLeft_.x + 2] = DisplayArray_Blocks[iv2_BottomLeft_.y + 2, iv2_BottomLeft_.x + 1];
                DisplayArray_Blocks[iv2_BottomLeft_.y + 2, iv2_BottomLeft_.x + 1] = DisplayArray_Blocks[iv2_BottomLeft_.y + 2, iv2_BottomLeft_.x + 0];
                DisplayArray_Blocks[iv2_BottomLeft_.y + 2, iv2_BottomLeft_.x + 0] = DisplayArray_Blocks[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 0];
                DisplayArray_Blocks[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 0] = go_BotLeftBlock;

                // Re-arrange the blocks in the Display Array to match
                DisplayArray[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 0] = DisplayArray[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 1];
                DisplayArray[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 1] = DisplayArray[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 2];
                DisplayArray[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 2] = DisplayArray[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 2];
                DisplayArray[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 2] = DisplayArray[iv2_BottomLeft_.y + 2, iv2_BottomLeft_.x + 2];
                DisplayArray[iv2_BottomLeft_.y + 2, iv2_BottomLeft_.x + 2] = DisplayArray[iv2_BottomLeft_.y + 2, iv2_BottomLeft_.x + 1];
                DisplayArray[iv2_BottomLeft_.y + 2, iv2_BottomLeft_.x + 1] = DisplayArray[iv2_BottomLeft_.y + 2, iv2_BottomLeft_.x + 0];
                DisplayArray[iv2_BottomLeft_.y + 2, iv2_BottomLeft_.x + 0] = DisplayArray[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 0];
                DisplayArray[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 0] = e_BotLeftBlock;

                // Change the Grid_Backdrop
                Set_BackdropColor(DisplayArray[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 0], new IntVector2(iv2_BottomLeft_.x + 0, iv2_BottomLeft_.y + 0));
                Set_BackdropColor(DisplayArray[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 0], new IntVector2(iv2_BottomLeft_.x + 0, iv2_BottomLeft_.y + 1));
                Set_BackdropColor(DisplayArray[iv2_BottomLeft_.y + 2, iv2_BottomLeft_.x + 0], new IntVector2(iv2_BottomLeft_.x + 0, iv2_BottomLeft_.y + 2));
                Set_BackdropColor(DisplayArray[iv2_BottomLeft_.y + 2, iv2_BottomLeft_.x + 1], new IntVector2(iv2_BottomLeft_.x + 1, iv2_BottomLeft_.y + 2));
                Set_BackdropColor(DisplayArray[iv2_BottomLeft_.y + 2, iv2_BottomLeft_.x + 2], new IntVector2(iv2_BottomLeft_.x + 2, iv2_BottomLeft_.y + 2));
                Set_BackdropColor(DisplayArray[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 2], new IntVector2(iv2_BottomLeft_.x + 2, iv2_BottomLeft_.y + 1));
                Set_BackdropColor(DisplayArray[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 2], new IntVector2(iv2_BottomLeft_.x + 2, iv2_BottomLeft_.y + 0));
                Set_BackdropColor(DisplayArray[iv2_BottomLeft_.y + 0, iv2_BottomLeft_.x + 1], new IntVector2(iv2_BottomLeft_.x + 1, iv2_BottomLeft_.y + 0));
                Set_BackdropColor(DisplayArray[iv2_BottomLeft_.y + 1, iv2_BottomLeft_.x + 1], new IntVector2(iv2_BottomLeft_.x + 1, iv2_BottomLeft_.y + 1));
            }
            #endregion
            
            // Play Sound Effect
            go_ThemeManager.GetComponent<Cs_AudioManager>().Play_SoundEffect(Enum_SoundEffect.RotateClockwise);
        }
    }
    public void RotateBlocks(Enum_Direction e_RotDir_, Enum_BlockSize e_BlockSize_, Vector2 v2_BottomLeft_)
    {
        IntVector2 iv2_Temp = new IntVector2((int)v2_BottomLeft_.x, (int)v2_BottomLeft_.y);
        RotateBlocks(e_RotDir_, e_BlockSize_, iv2_Temp);
    }

    public void DestroyBlockAt( IntVector2 iv2_DestroyLoc_ )
    {
        if(DisplayArray[iv2_DestroyLoc_.y, iv2_DestroyLoc_.x] != Enum_BlockType.Empty)
        {
            // Set the DisplayArray position to be empty
            DisplayArray[iv2_DestroyLoc_.y, iv2_DestroyLoc_.x] = Enum_BlockType.Empty;

            // Set the DisplayArray_Blocks block to destroy itself visually
            DisplayArray_Blocks[iv2_DestroyLoc_.y, iv2_DestroyLoc_.x].GetComponent<Cs_BlockOnBoardLogic>().Set_DeleteBlock();

            // Separate the DisplayArray_Blocks block by making it into an empty object
            DisplayArray_Blocks[iv2_DestroyLoc_.y, iv2_DestroyLoc_.x] = Instantiate(go_Empty);
            DisplayArray_Blocks[iv2_DestroyLoc_.y, iv2_DestroyLoc_.x].transform.SetParent(GameObject.Find("EmptyBlocks").transform);

            // Reset Backdrop Color
            Set_BackdropColor( Enum_BlockType.Empty, new IntVector2(iv2_DestroyLoc_.x, iv2_DestroyLoc_.y) );
        }
    }

    public void ScoreBlockAt( IntVector2  iv2_ScoreLoc_ )
    {
        if(DisplayArray[iv2_ScoreLoc_.y, iv2_ScoreLoc_.x] != Enum_BlockType.Empty)
        {
            // Set the DisplayArray position to be empty
            DisplayArray[iv2_ScoreLoc_.y, iv2_ScoreLoc_.x] = Enum_BlockType.Empty;

            // Set the DisplayArray_Blocks block to destroy itself visually
            DisplayArray_Blocks[iv2_ScoreLoc_.y, iv2_ScoreLoc_.x].GetComponent<Cs_BlockOnBoardLogic>().Set_ScoreBlock();

            // Separate the DisplayArray_Blocks block by making it into an empty object
            DisplayArray_Blocks[iv2_ScoreLoc_.y, iv2_ScoreLoc_.x] = Instantiate(go_Empty);
            DisplayArray_Blocks[iv2_ScoreLoc_.y, iv2_ScoreLoc_.x].transform.SetParent(GameObject.Find("EmptyBlocks").transform);
        }
    }
    
    public void Set_NewBlocks( Enum_BlockType[,] e_NewBlockTypeArray_, Enum_BlockSize e_BlockSize_, IntVector2 iv2_BottomLeft_ )
    {
        // Find Height/Width of block array
        int i_BlockWidth = 2;
        int i_BlockHeight = 2;

        // 3 Wide
        if (e_BlockSize_ == Enum_BlockSize.size_3w_2h || e_BlockSize_ == Enum_BlockSize.size_3w_3h) i_BlockWidth = 3;
        // 3 High
        if (e_BlockSize_ == Enum_BlockSize.size_2w_3h || e_BlockSize_ == Enum_BlockSize.size_3w_3h) i_BlockHeight = 3;
        
        // Run through the list of NewBlockTypeArray
        for(int y_ = 0; y_ < i_BlockHeight; ++y_)
        {
            for (int x_ = 0; x_ < i_BlockWidth; ++x_)
            {
                GameObject go_BlockTemp;

                // Create a new block based on the position within NewBlockTypeArray
                if (e_NewBlockTypeArray_[y_, x_] == Enum_BlockType.Block_1_Active)
                {
                    // go_BlockTemp is Instantiated as above by default
                    go_BlockTemp = Instantiate(go_Block_A);
                    go_BlockTemp.transform.SetParent(GameObject.Find("DisplayBlockList").transform);
                }
                else if (e_NewBlockTypeArray_[y_, x_] == Enum_BlockType.Block_2_Active)
                {
                    go_BlockTemp = Instantiate(go_Block_B);
                    go_BlockTemp.transform.SetParent(GameObject.Find("DisplayBlockList").transform);
                }
                else if (e_NewBlockTypeArray_[y_, x_] == Enum_BlockType.Block_3_Active)
                {
                    go_BlockTemp = Instantiate(go_Block_C);
                    go_BlockTemp.transform.SetParent(GameObject.Find("DisplayBlockList").transform);
                }
                else
                {
                    print("INVALID BLOCK ADDED TO CS_BOARDDISPLAY");
                    go_BlockTemp = Instantiate(go_Empty);
                    go_BlockTemp.transform.SetParent(GameObject.Find("EmptyBlocks").transform);
                }

                // Initialize this block
                int i_NewX = x_ + iv2_BottomLeft_.x;
                int i_NewY = y_ + iv2_BottomLeft_.y;
                if(go_BlockTemp.GetComponent<Cs_BlockOnBoardLogic>())
                {
                    go_BlockTemp.GetComponent<Cs_BlockOnBoardLogic>().Init_BlockModel(i_NewX, i_NewY, 3.0f, 0.75f, i_Width);
                }

                // Set the Backdrop Color
                Set_BackdropColor(e_NewBlockTypeArray_[y_, x_], new IntVector2( i_NewX, i_NewY ));

                // Add this block type to the proper position within Display Array
                DisplayArray[i_NewY, i_NewX] = e_NewBlockTypeArray_[y_, x_];

                DisplayArray_Blocks[i_NewY, i_NewX] = go_BlockTemp;
            }
        }
    }

    void Set_BackdropColor( Enum_BlockType e_BlockType_, IntVector2 iv2_Pos_, bool b_IsInstant_ = false )
    {
        // Set Grid_Backdrop Color
        if (e_BlockType_ == Enum_BlockType.Block_1_Active)
        {
            Grid_Columns[iv2_Pos_.y][iv2_Pos_.x].GetComponent<Cs_GridBlockLogic>().Set_ColorState(Enum_ColorState.Red, b_IsInstant_);
        }
        else if (e_BlockType_ == Enum_BlockType.Block_2_Active)
        {
            Grid_Columns[iv2_Pos_.y][iv2_Pos_.x].GetComponent<Cs_GridBlockLogic>().Set_ColorState(Enum_ColorState.Blue, b_IsInstant_);
        }
        else if (e_BlockType_ == Enum_BlockType.Block_3_Active)
        {
            // TODO: Block 3
            Grid_Columns[iv2_Pos_.y][iv2_Pos_.x].GetComponent<Cs_GridBlockLogic>().Set_ColorState(Enum_ColorState.White, b_IsInstant_);
        }
        else if( e_BlockType_ == Enum_BlockType.Empty )
        {
            Grid_Columns[iv2_Pos_.y][iv2_Pos_.x].GetComponent<Cs_GridBlockLogic>().Set_ColorState(Enum_ColorState.Original, b_IsInstant_);
        }
    }

    public void Set_ClearBackdrops()
    {
        for(int y_ = 0; y_ < i_Height; ++y_)
        {
            for(int x_ = 0; x_ < i_Width; ++x_)
            {
                if(x_ < i_Width && y_ < i_Height)
                {
                    Set_BackdropColor( Enum_BlockType.Empty, new IntVector2(x_, y_), true);
                }
            }
        }
    }
    
    // Set the x Position of the array to run through, and find the first open position. Stack blocks vertically from there.
    public void Set_ShowPotentialBlockVisual(int i_xPos_, int i_CurrY_, Enum_BlockType[] e_VertBlockArray_ )
    {
        // Initialize the yPosition to start from
        int i_yPos = 0;

        // Search for the first open position.
        for( int y_ = 0; y_ < i_Height; ++y_ )
        {
            // When we find the first open block position, step out.
            if( DisplayArray[y_, i_xPos_] == Enum_BlockType.Empty &&
                y_ <= i_CurrY_ )
            {
                i_yPos = y_;

                break;
            }
        }

        // Begin setting the new positions vertically
        for( int i_ = 0; i_ < e_VertBlockArray_.Length; ++i_ )
        {
            Set_BackdropColor( e_VertBlockArray_[i_], new IntVector2(i_xPos_, i_yPos + i_), true );
        }
    }

    public void Set_GameOverBlock( int i_xPos_, int i_yPos_, Enum_BlockType e_BlockType_ )
    {
        GameObject go_BlockTemp;

        // Create a new block based on the position within NewBlockTypeArray
        if (e_BlockType_ == Enum_BlockType.Block_1_Active)
        {
            // go_BlockTemp is Instantiated as above by default
            go_BlockTemp = Instantiate(go_Block_A);
            go_BlockTemp.transform.SetParent(GameObject.Find("DisplayBlockList").transform);
        }
        else if (e_BlockType_ == Enum_BlockType.Block_2_Active)
        {
            go_BlockTemp = Instantiate(go_Block_B);
            go_BlockTemp.transform.SetParent(GameObject.Find("DisplayBlockList").transform);
        }
        else if (e_BlockType_ == Enum_BlockType.Block_3_Active)
        {
            go_BlockTemp = Instantiate(go_Block_C);
            go_BlockTemp.transform.SetParent(GameObject.Find("DisplayBlockList").transform);
        }
        else
        {
            go_BlockTemp = Instantiate(go_Empty);
            go_BlockTemp.transform.SetParent(GameObject.Find("EmptyBlocks").transform);
        }

        // Initialize this block
        if (go_BlockTemp.GetComponent<Cs_BlockOnBoardLogic>())
        {
            go_BlockTemp.GetComponent<Cs_BlockOnBoardLogic>().Init_BlockModel(i_xPos_, i_yPos_, 3.0f, 0.001f, i_Width);
            go_BlockTemp.GetComponent<Cs_BlockOnBoardLogic>().Set_GameOverBlock();
        }

        // Set the Backdrop Color
        Set_BackdropColor(e_BlockType_, new IntVector2(i_xPos_, i_yPos_));
    }

    int i_Score;
    int i_Score_Remaining;
    float f_ScoreTimer;
    float f_ScoreTimer_Max = 0.25f;
    public void Set_ScoreTextIncrease(int i_AmountToIncreaseScore_ = 0)
    {
        if(i_AmountToIncreaseScore_ > 0)
        {
            i_Score_Remaining += i_AmountToIncreaseScore_;
        }
        else
        {
            if(f_ScoreTimer > 0f)
            {
                f_ScoreTimer -= Time.deltaTime;
                if (f_ScoreTimer <= 0f) f_ScoreTimer = 0f;
            }

            if(f_ScoreTimer == 0f)
            {
                // Check if there's a point to add to the text on screen
                if(i_Score_Remaining > 0)
                {
                    // Reduce amount remaining to score by 1
                    --i_Score_Remaining;

                    // Increment player's score by 1
                    ++i_Score;

                    // Reset score timer
                    f_ScoreTimer = f_ScoreTimer_Max;

                    // Set text on screen
                    txt_Score.GetComponent<Text>().text = "Score: " + i_Score.ToString();
                }
            }
        }
    }

    // Used for cheating.
    public void Set_OneBlock( int i_xPos_, int i_yPos_, Enum_BlockType e_BlockType_ )
    {
        // Clear prior block first
        ScoreBlockAt(new IntVector2(i_xPos_, i_yPos_));

        // Set new Temp Block
        GameObject go_BlockTemp;

        // Create a new block based on the position within NewBlockTypeArray
        if (e_BlockType_ == Enum_BlockType.Block_1_Static)
        {
            // go_BlockTemp is Instantiated as above by default
            go_BlockTemp = Instantiate(go_Block_A);
            go_BlockTemp.transform.SetParent(GameObject.Find("DisplayBlockList").transform);
        }
        else if (e_BlockType_ == Enum_BlockType.Block_2_Static)
        {
            go_BlockTemp = Instantiate(go_Block_B);
            go_BlockTemp.transform.SetParent(GameObject.Find("DisplayBlockList").transform);
        }
        else if (e_BlockType_ == Enum_BlockType.Block_3_Static)
        {
            go_BlockTemp = Instantiate(go_Block_C);
            go_BlockTemp.transform.SetParent(GameObject.Find("DisplayBlockList").transform);
        }
        else
        {
            print("INVALID BLOCK ADDED TO CS_BOARDDISPLAY");
            go_BlockTemp = Instantiate(go_Empty);
            go_BlockTemp.transform.SetParent(GameObject.Find("EmptyBlocks").transform);
        }

        // Initialize this block
        if (go_BlockTemp.GetComponent<Cs_BlockOnBoardLogic>())
        {
            go_BlockTemp.GetComponent<Cs_BlockOnBoardLogic>().Init_BlockModel(i_xPos_, i_yPos_, 3.0f, 0.75f, i_Width);
        }

        // Set the Backdrop Color
        Set_BackdropColor(e_BlockType_, new IntVector2(i_xPos_, i_yPos_));

        // Add this block type to the proper position within Display Array
        DisplayArray[i_yPos_, i_xPos_] = e_BlockType_;

        DisplayArray_Blocks[i_yPos_, i_xPos_] = go_BlockTemp;
    }

    void PrintArrayToConsole()
    {
        // The 'y' is reversed (top to bottom) to compensate for printing
        for(int j = i_Height - 1; j >= 0 ; j--)
        {
            string tempString = "";

            for(int i = 0; i < i_Width; ++i)
            {
                // Left & Right 'Walls'
                if((i == 0 || i == i_Width - 1) && (DisplayArray[j, i] == Enum_BlockType.Empty))
                {
                    tempString += "{!!} ";
                }
                // Normal empty block position
                else if (DisplayArray[j, i] == Enum_BlockType.Empty) tempString += "[__] ";
                // Print a populated block
                else tempString += "[" + (int)DisplayArray[j, i] + "] ";
            }
            print(tempString);
        }
        // print("Active Block: " + v2_ActiveBlockLocation + "\n-----------------------------------------------------------------\n");
    }
}
