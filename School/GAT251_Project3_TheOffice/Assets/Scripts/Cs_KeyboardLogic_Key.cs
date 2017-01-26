using UnityEngine;
using System.Collections;

public class Cs_KeyboardLogic_Key : MonoBehaviour
{
    GameObject go_Keyboard;

    [SerializeField] Color clr_MonitorColor;

	// Use this for initialization
	void Start ()
    {
        go_Keyboard = transform.root.gameObject;

        // Set key to match the color of what we stated
        Material[] mat_CurrMat = gameObject.GetComponent<MeshRenderer>().materials;
        for(int i_ = 0; i_ < mat_CurrMat.Length; ++i_)
        {
            mat_CurrMat[i_].color = clr_MonitorColor;
        }
	}

    public void Use()
    {
        // Tell attached keyboard to set this screen's color
        go_Keyboard.GetComponent<Cs_KeyboardLogic>().Set_MonitorColor( clr_MonitorColor );
    }
	
	// Update is called once per frame
	void Update ()
    {
	    
	}
}
