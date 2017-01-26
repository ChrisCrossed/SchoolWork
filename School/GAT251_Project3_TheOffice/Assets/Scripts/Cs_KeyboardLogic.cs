using UnityEngine;
using System.Collections;

public class Cs_KeyboardLogic : MonoBehaviour
{
    GameObject go_Button_1;
    GameObject go_Button_2;
    GameObject go_Button_3;
    GameObject go_Button_4;

    [SerializeField] GameObject go_Monitor;

    // Use this for initialization
    void Start ()
    {
        // Initialize
        go_Button_1 = transform.Find("Key_1").gameObject;
        go_Button_2 = transform.Find("Key_2").gameObject;
        go_Button_3 = transform.Find("Key_3").gameObject;
        go_Button_4 = transform.Find("Key_4").gameObject;
    }

    public void Set_MonitorColor( Color clr_NewColor_ )
    {
        // Find the 'mat_Monitor' material of this Monitor
        Material[] mat_CurrMat = go_Monitor.GetComponent<MeshRenderer>().materials;
        for(int i_ = 0; i_ < mat_CurrMat.Length; ++i_)
        {
            if (mat_CurrMat[i_].name == "mat_Monitor (Instance)")
            {
                Color clr_CurrColor = mat_CurrMat[i_].color;
                clr_CurrColor = clr_NewColor_;
                mat_CurrMat[i_].color = clr_CurrColor;
            }
        }

        // Change it's color to match that of what was passed in
        // go_Monitor.
    }
	
	// Update is called once per frame
	void Update ()
    {
	
	}
}
