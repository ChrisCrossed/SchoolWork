using UnityEngine;
using System.Collections;

/*********************************
 * 
 * Copyright DigiPen Institute of Technology 2016
 * 
 * Streak 2 by Christopher Christensen
 * 
 * *******************************/

public class Cs_HUDBlockLogic : MonoBehaviour
{
    GameObject go_Block_A;
    GameObject go_Block_B;
    GameObject go_Block_C;
    GameObject go_Empty;

    GameObject go_CurrBlock;

    // Use this for initialization
    void Start ()
    {
        #region Load Resources
        go_Block_A = Resources.Load("Block_X_mdl") as GameObject;
        go_Block_B = Resources.Load("Block_O_mdl") as GameObject;
        go_Block_C = Resources.Load("Block_Tri_mdl") as GameObject;

        go_Empty = Resources.Load("Block_Empty") as GameObject;
        #endregion
    }

    void Destroy_OldBlock()
    {
        if(go_CurrBlock)
        {
            GameObject.Destroy(go_CurrBlock);
        }
    }

    public void Set_NewBlock( Enum_BlockType e_BlockType_ )
    {
        // Destroy the old block before creating a new one in it's place
        Destroy_OldBlock();

        if( e_BlockType_ == Enum_BlockType.Block_1_Active || e_BlockType_ == Enum_BlockType.Block_1_Static )
        {
            go_CurrBlock = Instantiate(go_Block_A);
        }
        else if (e_BlockType_ == Enum_BlockType.Block_2_Active || e_BlockType_ == Enum_BlockType.Block_2_Static)
        {
            go_CurrBlock = Instantiate(go_Block_B);
        }
        else if (e_BlockType_ == Enum_BlockType.Block_3_Active || e_BlockType_ == Enum_BlockType.Block_3_Static)
        {
            go_CurrBlock = Instantiate(go_Block_C);
        }
        else if (e_BlockType_ == Enum_BlockType.Empty)
        {
            go_CurrBlock = Instantiate(go_Empty);
        }

        Vector2 pos = gameObject.transform.position;
        Vector2 viewportPos = Camera.main.ScreenToWorldPoint(pos);

        print(viewportPos);
        
        // Vector2 viewportPos = Camera.main.ScreenToWorldPoint(pos);

        // Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, gameObject.transform.position);
        
        

        go_CurrBlock.transform.position = pos;
        // go_CurrBlock.transform.SetParent( gameObject.transform );

        // go_CurrBlock.transform.SetParent( GameObject.Find("CanvasTest").transform );

        // go_CurrBlock.GetComponent<Cs_BlockOnBoardLogic>().Init_BlockModel((int)viewportPos.x - 5, (int)viewportPos.y - 5, 1.0f, 1.0f, 0);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.M))
        {
            Set_NewBlock(Enum_BlockType.Block_1_Active);
        }
    }
}
