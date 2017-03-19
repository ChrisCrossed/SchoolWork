using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cs_HighScore : MonoBehaviour
{
    bool b_IsActive;
    Color clr_Curr;

	// Use this for initialization
	void Start ()
    {
		
	}

    public void Activate()
    {
        if(!b_IsActive)
        {
            b_IsActive = true;
        }
    }

    // Update is called once per frame
    float f_Timer;
	void Update ()
    {
		if(b_IsActive)
        {
            f_Timer += Time.deltaTime;

            float f_Perc = Mathf.Cos(f_Timer) + 0.5f;

            Color clr_NewColor = Color.Lerp(Color.red, Color.blue, f_Perc);

            gameObject.GetComponent<Text>().color = clr_NewColor;
        }
	}
}
