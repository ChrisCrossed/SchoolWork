using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cs_HUDManager : MonoBehaviour
{
    GameObject go_HUD_Camera;

    GameObject go_HUDdisplay_Left;
    GameObject go_HUDdisplay_Right;

    GameObject[] go_HUDblocks;

    bool b_IsNormalDifficulty;

    bool b_IsTutorial = true;

    private void Start()
    {
        go_HUD_Camera = GameObject.Find("HUD_Camera");
    }

    // Use this for initialization
    public void Initialize( bool b_IsNormalDifficulty_ )
    {
        // Not a tutorial, therefore we can manipulate HUD elements
        b_IsTutorial = false;

        b_IsNormalDifficulty = b_IsNormalDifficulty_;

        go_HUDblocks = new GameObject[27];

        if( b_IsNormalDifficulty )
        {
            // Enable the 'Normal' difficulty blocks UI
            go_HUD_Camera.transform.FindChild("Hud_Normal").gameObject.SetActive(true);

            // State which blocks are to be used
            go_HUDdisplay_Left = go_HUD_Camera.transform.Find("Hud_Normal").transform.Find("Block_Left").gameObject;
            go_HUDdisplay_Right = go_HUD_Camera.transform.Find("Hud_Normal").transform.Find("Block_Right").gameObject;

            for(int i_ = 0; i_ < go_HUDblocks.Length; ++i_)
            {
                go_HUDblocks[i_] = go_HUDdisplay_Left.transform.FindChild("HUD_Block_" + i_.ToString()).gameObject;
            }
        }
        else
        {
            // Enable the 'Difficult' blocks UI
            go_HUD_Camera.transform.FindChild("Hud_Hard").gameObject.SetActive(true);

            // State which blocks are to be used
            go_HUDdisplay_Left = go_HUD_Camera.transform.Find("Hud_Hard").transform.Find("Block_Left").gameObject;
            go_HUDdisplay_Right = go_HUD_Camera.transform.Find("Hud_Hard").transform.Find("Block_Right").gameObject;

            for (int i_ = 0; i_ < go_HUDblocks.Length; ++i_)
            {
                go_HUDblocks[i_] = go_HUDdisplay_Left.transform.FindChild("HUD_Block_" + i_.ToString()).gameObject;
            }
        }
    }

    public void Set_NextBlockList( Enum_BlockType[] e_BlockList_, Enum_BlockSize[] e_BlockSize_ )
    {
        if (b_IsTutorial) return;

        #region Normal difficulty
        if(b_IsNormalDifficulty)
        {
            int i_ = 0;

            go_HUDblocks[i_ + 0].GetComponent<Cs_HUDBlockLogic>().Set_NewBlock(e_BlockList_[0]);
            go_HUDblocks[i_ + 1].GetComponent<Cs_HUDBlockLogic>().Set_NewBlock(e_BlockList_[1]);
            go_HUDblocks[i_ + 3].GetComponent<Cs_HUDBlockLogic>().Set_NewBlock(e_BlockList_[2]);
            go_HUDblocks[i_ + 4].GetComponent<Cs_HUDBlockLogic>().Set_NewBlock(e_BlockList_[3]);

            go_HUDblocks[i_ + 2].GetComponent<Cs_HUDBlockLogic>().Set_NewBlock(Enum_BlockType.Empty);
            go_HUDblocks[i_ + 5].GetComponent<Cs_HUDBlockLogic>().Set_NewBlock(Enum_BlockType.Empty);
            go_HUDblocks[i_ + 6].GetComponent<Cs_HUDBlockLogic>().Set_NewBlock(Enum_BlockType.Empty);
            go_HUDblocks[i_ + 7].GetComponent<Cs_HUDBlockLogic>().Set_NewBlock(Enum_BlockType.Empty);
            go_HUDblocks[i_ + 8].GetComponent<Cs_HUDBlockLogic>().Set_NewBlock(Enum_BlockType.Empty);

            i_ = 9;

            go_HUDblocks[i_ + 0].GetComponent<Cs_HUDBlockLogic>().Set_NewBlock(e_BlockList_[4]);
            go_HUDblocks[i_ + 1].GetComponent<Cs_HUDBlockLogic>().Set_NewBlock(e_BlockList_[5]);
            go_HUDblocks[i_ + 3].GetComponent<Cs_HUDBlockLogic>().Set_NewBlock(e_BlockList_[6]);
            go_HUDblocks[i_ + 4].GetComponent<Cs_HUDBlockLogic>().Set_NewBlock(e_BlockList_[7]);

            go_HUDblocks[i_ + 2].GetComponent<Cs_HUDBlockLogic>().Set_NewBlock(Enum_BlockType.Empty);
            go_HUDblocks[i_ + 5].GetComponent<Cs_HUDBlockLogic>().Set_NewBlock(Enum_BlockType.Empty);
            go_HUDblocks[i_ + 6].GetComponent<Cs_HUDBlockLogic>().Set_NewBlock(Enum_BlockType.Empty);
            go_HUDblocks[i_ + 7].GetComponent<Cs_HUDBlockLogic>().Set_NewBlock(Enum_BlockType.Empty);
            go_HUDblocks[i_ + 8].GetComponent<Cs_HUDBlockLogic>().Set_NewBlock(Enum_BlockType.Empty);

            i_ = 18;

            go_HUDblocks[i_ + 0].GetComponent<Cs_HUDBlockLogic>().Set_NewBlock(e_BlockList_[8]);
            go_HUDblocks[i_ + 1].GetComponent<Cs_HUDBlockLogic>().Set_NewBlock(e_BlockList_[9]);
            go_HUDblocks[i_ + 3].GetComponent<Cs_HUDBlockLogic>().Set_NewBlock(e_BlockList_[10]);
            go_HUDblocks[i_ + 4].GetComponent<Cs_HUDBlockLogic>().Set_NewBlock(e_BlockList_[11]);

            go_HUDblocks[i_ + 2].GetComponent<Cs_HUDBlockLogic>().Set_NewBlock(Enum_BlockType.Empty);
            go_HUDblocks[i_ + 5].GetComponent<Cs_HUDBlockLogic>().Set_NewBlock(Enum_BlockType.Empty);
            go_HUDblocks[i_ + 6].GetComponent<Cs_HUDBlockLogic>().Set_NewBlock(Enum_BlockType.Empty);
            go_HUDblocks[i_ + 7].GetComponent<Cs_HUDBlockLogic>().Set_NewBlock(Enum_BlockType.Empty);
            go_HUDblocks[i_ + 8].GetComponent<Cs_HUDBlockLogic>().Set_NewBlock(Enum_BlockType.Empty);
        }
        #endregion
        #region Harder difficulties
        else
        {
            int i_NextBlockStart = 0;
            for(int i_ = 0; i_ < 3; ++i_)
            {
                if (e_BlockSize_[i_] == Enum_BlockSize.size_2w_2h)
                {
                    go_HUDblocks[i_ * 9 + 0].GetComponent<Cs_HUDBlockLogic>().Set_NewBlock(e_BlockList_[0 + i_NextBlockStart]);
                    go_HUDblocks[i_ * 9 + 1].GetComponent<Cs_HUDBlockLogic>().Set_NewBlock(e_BlockList_[1 + i_NextBlockStart]);
                    go_HUDblocks[i_ * 9 + 3].GetComponent<Cs_HUDBlockLogic>().Set_NewBlock(e_BlockList_[2 + i_NextBlockStart]);
                    go_HUDblocks[i_ * 9 + 4].GetComponent<Cs_HUDBlockLogic>().Set_NewBlock(e_BlockList_[3 + i_NextBlockStart]);

                    go_HUDblocks[i_ * 9 + 2].GetComponent<Cs_HUDBlockLogic>().Set_NewBlock(Enum_BlockType.Empty);
                    go_HUDblocks[i_ * 9 + 5].GetComponent<Cs_HUDBlockLogic>().Set_NewBlock(Enum_BlockType.Empty);
                    go_HUDblocks[i_ * 9 + 6].GetComponent<Cs_HUDBlockLogic>().Set_NewBlock(Enum_BlockType.Empty);
                    go_HUDblocks[i_ * 9 + 7].GetComponent<Cs_HUDBlockLogic>().Set_NewBlock(Enum_BlockType.Empty);
                    go_HUDblocks[i_ * 9 + 8].GetComponent<Cs_HUDBlockLogic>().Set_NewBlock(Enum_BlockType.Empty);

                    i_NextBlockStart += 4;
                }
                else if( e_BlockSize_[i_] == Enum_BlockSize.size_3w_2h || e_BlockSize_[i_] == Enum_BlockSize.size_2w_3h )
                {
                    if (e_BlockSize_[i_] == Enum_BlockSize.size_3w_2h)
                    {
                        go_HUDblocks[i_ * 9 + 0].GetComponent<Cs_HUDBlockLogic>().Set_NewBlock(e_BlockList_[0 + i_NextBlockStart]);
                        go_HUDblocks[i_ * 9 + 1].GetComponent<Cs_HUDBlockLogic>().Set_NewBlock(e_BlockList_[1 + i_NextBlockStart]);
                        go_HUDblocks[i_ * 9 + 2].GetComponent<Cs_HUDBlockLogic>().Set_NewBlock(e_BlockList_[2 + i_NextBlockStart]);
                        go_HUDblocks[i_ * 9 + 3].GetComponent<Cs_HUDBlockLogic>().Set_NewBlock(e_BlockList_[3 + i_NextBlockStart]);
                        go_HUDblocks[i_ * 9 + 4].GetComponent<Cs_HUDBlockLogic>().Set_NewBlock(e_BlockList_[4 + i_NextBlockStart]);
                        go_HUDblocks[i_ * 9 + 5].GetComponent<Cs_HUDBlockLogic>().Set_NewBlock(e_BlockList_[5 + i_NextBlockStart]);

                        go_HUDblocks[i_ * 9 + 6].GetComponent<Cs_HUDBlockLogic>().Set_NewBlock(Enum_BlockType.Empty);
                        go_HUDblocks[i_ * 9 + 7].GetComponent<Cs_HUDBlockLogic>().Set_NewBlock(Enum_BlockType.Empty);
                        go_HUDblocks[i_ * 9 + 8].GetComponent<Cs_HUDBlockLogic>().Set_NewBlock(Enum_BlockType.Empty);
                    }
                    else // e_BlockSize_[i_] == Enum_BlockSize.size_2w_3h
                    {
                        go_HUDblocks[i_ * 9 + 0].GetComponent<Cs_HUDBlockLogic>().Set_NewBlock(e_BlockList_[0 + i_NextBlockStart]);
                        go_HUDblocks[i_ * 9 + 1].GetComponent<Cs_HUDBlockLogic>().Set_NewBlock(e_BlockList_[1 + i_NextBlockStart]);
                        go_HUDblocks[i_ * 9 + 3].GetComponent<Cs_HUDBlockLogic>().Set_NewBlock(e_BlockList_[2 + i_NextBlockStart]);
                        go_HUDblocks[i_ * 9 + 4].GetComponent<Cs_HUDBlockLogic>().Set_NewBlock(e_BlockList_[3 + i_NextBlockStart]);
                        go_HUDblocks[i_ * 9 + 6].GetComponent<Cs_HUDBlockLogic>().Set_NewBlock(e_BlockList_[4 + i_NextBlockStart]);
                        go_HUDblocks[i_ * 9 + 7].GetComponent<Cs_HUDBlockLogic>().Set_NewBlock(e_BlockList_[5 + i_NextBlockStart]);

                        go_HUDblocks[i_ * 9 + 2].GetComponent<Cs_HUDBlockLogic>().Set_NewBlock(Enum_BlockType.Empty);
                        go_HUDblocks[i_ * 9 + 5].GetComponent<Cs_HUDBlockLogic>().Set_NewBlock(Enum_BlockType.Empty);
                        go_HUDblocks[i_ * 9 + 8].GetComponent<Cs_HUDBlockLogic>().Set_NewBlock(Enum_BlockType.Empty);
                    }
                    
                    i_NextBlockStart += 6;
                }
                else // 3x3 block
                {
                    for(int j_ = 0; j_ < 9; ++j_)
                    {
                        go_HUDblocks[i_ + j_].GetComponent<Cs_HUDBlockLogic>().Set_NewBlock(e_BlockList_[j_ + i_NextBlockStart]);
                    }

                    i_NextBlockStart += 9;
                }
            }
        }
        #endregion
    }

    // Update is called once per frame
    void Update ()
    {
		
	}
}
