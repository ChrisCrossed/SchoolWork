using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cs_HUDManager : MonoBehaviour
{
    GameObject go_HUD_Camera;

    GameObject go_HUDBlocks_Left;
    GameObject go_HUDBlocks_Right;

    private void Start()
    {
        go_HUD_Camera = GameObject.Find("HUD_Camera");
    }

    // Use this for initialization
    public void Initialize( bool b_IsNormalDifficulty_ )
    {
        if( b_IsNormalDifficulty_ )
        {
            // Enable the 'Normal' difficulty blocks UI
            go_HUD_Camera.transform.FindChild("Hud_Normal").gameObject.SetActive(true);

            // State which blocks are to be used
            go_HUDBlocks_Left = go_HUD
        }
        else
        {
            // Enable the 'Difficult' blocks UI
            go_HUD_Camera.transform.FindChild("Hud_Hard").gameObject.SetActive(true);
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
