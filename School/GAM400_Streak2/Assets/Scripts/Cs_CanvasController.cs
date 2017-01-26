using UnityEngine;
using System.Collections;

/*********************************
 * 
 * Copyright DigiPen Institute of Technology 2016
 * 
 * Streak 2 by Christopher Christensen
 * 
 * *******************************/

public class Cs_CanvasController : MonoBehaviour
{
    GameObject go_TopLeft;
    GameObject go_TopCenter;
    GameObject go_TopRight;
    GameObject go_MiddleLeft;
    GameObject go_MiddleCenter;
    GameObject go_MiddleRight;
    GameObject go_BottomLeft;
    GameObject go_BottomCenter;
    GameObject go_BottomRight;

    // Use this for initialization
    void Awake ()
    {
        #region Set GameObjects
        go_TopLeft = GameObject.Find("HUD_TopLeft").gameObject;
        go_TopCenter = GameObject.Find("HUD_TopMiddle").gameObject;
        go_TopRight = GameObject.Find("HUD_TopRight").gameObject;

        go_MiddleLeft = GameObject.Find("HUD_CenterLeft").gameObject;
        go_MiddleCenter = GameObject.Find("HUD_CenterMiddle").gameObject;
        go_MiddleRight = GameObject.Find("HUD_CenterRight").gameObject;

        go_BottomLeft = GameObject.Find("HUD_BottomLeft").gameObject;
        go_BottomCenter = GameObject.Find("HUD_BottomMiddle").gameObject;
        go_BottomRight = GameObject.Find("HUD_BottomRight").gameObject;
        #endregion
        
        Set_ScreenResolution( Screen.currentResolution );
    }

    void Set_ScreenResolution( Resolution currResolution_ )
    {
        print("Resolution: " + Screen.width + " / " + Screen.height);

        float f_CurrRes = Mathf.Round( (float)Screen.currentResolution.width / (float)Screen.currentResolution.height * 1000f );

        float f_5x4     = Mathf.Round( 5f / 4f * 1000f);
        float f_4x3     = Mathf.Round(4f / 3f * 1000f);
        float f_3x2     = Mathf.Round(3f / 2f * 1000f);
        float f_16x10   = Mathf.Round(16f / 10f * 1000f);
        float f_16x9    = Mathf.Round(16f / 9f * 1000f);

        print("Current: " + f_CurrRes + ", " + "Actual: " + f_16x10);

        if ( f_CurrRes == f_5x4 )
        {
            print("5:4 Resolution");
            Res_5x4();
        }
        else if( f_CurrRes == f_4x3 )
        {
            print("4:3 Resolution");
        }
        else if ( f_CurrRes == f_3x2 )
        {
            print("3:2 Resolution");
        }
        else if ( f_CurrRes == f_16x10 )
        {
            print("16:10 Resolution");
        }
        else if ( f_CurrRes == f_16x9 )
        {
            print("16:9 Resolution");
        }

        Res_16x9();
    }

    void Res_5x4()
    {
        // Set block scale

        // Set bottom left block position

        // Position each block & Scale them
    }

    void Res_16x9()
    {
        // Set block scale
        float f_Scale = 75f / 1920f; // Ensures a consistent block size of 75 units @ 1920 width
        float f_Buffer = 10f / 1920f * Screen.width;
        float f_BlockScale = f_Scale * Screen.width;

        print((float)Screen.currentResolution.width);

        // Position each block & Scale them
        Set_BlockScale( f_BlockScale );

        Set_BlockPosition( f_BlockScale, f_Buffer);
    }

    void Set_BlockScale( float f_BlockScale_ )
    {
        // Vector2 v2_Scale = new Vector2(f_BlockScale_, f_BlockScale_);

        // go_TopLeft.transform.localScale = v2_Scale;
        /*
        go_TopLeft.GetComponent<RectTransform>().sizeDelta = v2_Scale;
        go_TopCenter.GetComponent<RectTransform>().sizeDelta = v2_Scale;
        go_TopRight.GetComponent<RectTransform>().sizeDelta = v2_Scale;
        go_MiddleLeft.GetComponent<RectTransform>().sizeDelta = v2_Scale;
        go_MiddleCenter.GetComponent<RectTransform>().sizeDelta = v2_Scale;
        go_MiddleRight.GetComponent<RectTransform>().sizeDelta = v2_Scale;
        go_BottomLeft.GetComponent<RectTransform>().sizeDelta = v2_Scale;
        go_BottomCenter.GetComponent<RectTransform>().sizeDelta = v2_Scale;
        go_BottomRight.GetComponent<RectTransform>().sizeDelta = v2_Scale;
        */
    }
    
    void Set_BlockPosition( float f_BlockScale_ , float f_Buffer_ )
    {
        /*
        float f_LeftColumn = f_BlockScale_ + f_Buffer_;
        float f_CenterColumn = f_LeftColumn + f_BlockScale_ + f_Buffer_;
        float f_RightColumn = f_CenterColumn + f_BlockScale_ + f_Buffer_;

        float f_MiddleRow = Screen.height / 2f;
        float f_TopRow = f_MiddleRow + f_BlockScale_ + f_Buffer_;
        float f_BottomRow = f_MiddleRow - f_BlockScale_ - f_Buffer_;

        go_MiddleLeft.transform.position = new Vector3(f_LeftColumn, f_MiddleRow, 0);
        // GameObject go_Test = Instantiate(Resources.Load("Block_X_mdl") as GameObject);
        // go_Test.transform.position = go_MiddleLeft.transform.position;

        go_MiddleCenter.transform.position = new Vector3(f_CenterColumn, f_MiddleRow, 0);
        go_MiddleRight.transform.position = new Vector3(f_RightColumn, f_MiddleRow, 0);

        go_TopLeft.transform.position = new Vector3(f_LeftColumn, f_TopRow, 0);
        go_TopCenter.transform.position = new Vector3(f_CenterColumn, f_TopRow, 0);
        go_TopRight.transform.position = new Vector3(f_RightColumn, f_TopRow, 0);

        go_BottomLeft.transform.position = new Vector3(f_LeftColumn, f_BottomRow, 0);
        go_BottomCenter.transform.position = new Vector3(f_CenterColumn, f_BottomRow, 0);
        go_BottomRight.transform.position = new Vector3(f_RightColumn, f_BottomRow, 0);
        */
    }

    public void Set_NextBlockArray( Enum_BlockType[,] e_BlockType_, Enum_BlockSize e_BlockSize_, IntVector2 iv2_BottomLeft_ )
    {
        if(e_BlockSize_ == Enum_BlockSize.size_2w_2h)
        {

        }

        //float f_LeftColumn = f_BlockScale_ + f_Buffer_;
        //float f_CenterColumn = f_LeftColumn + f_BlockScale_ + f_Buffer_;
        //float f_RightColumn = f_CenterColumn + f_BlockScale_ + f_Buffer_;

        //float f_MiddleRow = Screen.height / 2f;
        //float f_TopRow = f_MiddleRow + f_BlockScale_ + f_Buffer_;
        //float f_BottomRow = f_MiddleRow - f_BlockScale_ - f_Buffer_;

        /*
        // Find Height/Width of block array
        int i_BlockWidth = 2;
        int i_BlockHeight = 2;

        // 3 Wide
        if (e_BlockSize_ == Enum_BlockSize.size_3w_2h || e_BlockSize_ == Enum_BlockSize.size_3w_3h) i_BlockWidth = 3;
        // 3 High
        if (e_BlockSize_ == Enum_BlockSize.size_2w_3h || e_BlockSize_ == Enum_BlockSize.size_3w_3h) i_BlockHeight = 3;

        // Run through the list of NewBlockTypeArray
        for (int y_ = 0; y_ < i_BlockHeight; ++y_)
        {
            for (int x_ = 0; x_ < i_BlockWidth; ++x_)
            {
                GameObject go_BlockTemp;

                // Create a new block based on the position within NewBlockTypeArray
                if (e_BlockType_[y_, x_] == Enum_BlockType.Block_1_Active)
                {
                    // go_BlockTemp is Instantiated as above by default
                    go_BlockTemp = Instantiate(go_Block_A);
                    go_BlockTemp.transform.SetParent(GameObject.Find("DisplayBlockList").transform);
                }
                else if (e_BlockType_[y_, x_] == Enum_BlockType.Block_2_Active)
                {
                    go_BlockTemp = Instantiate(go_Block_B);
                    go_BlockTemp.transform.SetParent(GameObject.Find("DisplayBlockList").transform);
                }
                else if (e_BlockType_[y_, x_] == Enum_BlockType.Block_3_Active)
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
                if (go_BlockTemp.GetComponent<Cs_BlockOnBoardLogic>())
                {
                    go_BlockTemp.GetComponent<Cs_BlockOnBoardLogic>().Init_BlockModel(i_NewX, i_NewY, 3.0f, 0.75f, i_Width);
                }

                // Set the Backdrop Color
                // Set_BackdropColor(e_NewBlockTypeArray_[y_, x_], new IntVector2(i_NewX, i_NewY));

                // Add this block type to the proper position within Display Array
                DisplayArray[i_NewY, i_NewX] = e_NewBlockTypeArray_[y_, x_];

                DisplayArray_Blocks[i_NewY, i_NewX] = go_BlockTemp;
            }
        }
        */
    }

    // Update is called once per frame
    void Update ()
    {
        
    }
}
