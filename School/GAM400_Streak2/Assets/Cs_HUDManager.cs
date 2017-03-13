using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cs_HUDManager : MonoBehaviour
{
    GameObject go_HUD_Camera;

    GameObject go_HUDdisplay_Left;
    GameObject go_HUDdisplay_Right;

    GameObject[] go_HUDblocks;

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

        if( b_IsNormalDifficulty_ )
        {
            // Enable the 'Normal' difficulty blocks UI
            go_HUD_Camera.transform.FindChild("Hud_Normal").gameObject.SetActive(true);

            // State which blocks are to be used
            go_HUDdisplay_Left = go_HUD_Camera.transform.Find("Hud_Normal").transform.Find("Block_Left").gameObject;
            go_HUDdisplay_Right = go_HUD_Camera.transform.Find("Hud_Normal").transform.Find("Block_Right").gameObject;

            go_HUDblocks = new GameObject[8];

            for(int i_ = 0; i_ < 8;  ++i_)
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

            print("ADD INITIALIZE INFO IN Cs_HUDMANAGER");
            /*for (int i_ = 0; i_ < 12; ++i_)
            {
                go_HUDblocks[i_] = go_HUDdisplay_Right.transform.FindChild("HUD_Block_" + i_.ToString()).gameObject;
            }*/
        }
    }

    public void Set_NextBlockList( Enum_BlockType[] e_BlockList_, Enum_BlockSize[] e_BlockSize_ )
    {
        if (b_IsTutorial) return;

        for(int i_ = 0; i_ < 12; ++i_)
        {
            print("CHRIS RIGHT HERE: " + e_BlockList_[i_]);
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
