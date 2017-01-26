using UnityEngine;
using System.Collections;

public class Cs_IconLogic : MonoBehaviour
{
    bool b_IsActive;

    // Use this for initialization
	public void Remove ()
    {
        b_IsActive = true;
	}

    int i_NumTimes = 0;
    int i_NumTimes_Max = 2;
    bool b_DoneFlashing = false;
    Color clr_Green = new Color(0, 1, 0);
    Color clr_Black = new Color(0, 0, 0);
    void FlashGreen()
    {

    }
    
	// Update is called once per frame
	void Update ()
    {
	    
	}
}
