using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Cs_EndGameTrigger : MonoBehaviour
{
    bool b_Enabled;

    float f_Timer;
    float f_Visibility;

    public GameObject ui_GameOver;

	// Use this for initialization
    void Start()
    {
        gameObject.GetComponent<MeshRenderer>().enabled = false;

        ui_GameOver = GameObject.Find("EndGame");

        // Set object to black, alpha to 0
        var currColor = ui_GameOver.GetComponent<MeshRenderer>().material.color;
        currColor.a = 0.0f;
        ui_GameOver.GetComponent<MeshRenderer>().material.color = currColor;
    }
	
	// Update is called once per frame
	void Update ()
    {
	    if(b_Enabled)
        {
            // Begin dimming the lights

            f_Visibility += Time.deltaTime;
            var currColor = ui_GameOver.GetComponent<MeshRenderer>().material.color;
            currColor.a = f_Visibility;
            ui_GameOver.GetComponent<MeshRenderer>().material.color = currColor;

            // After X time, kick to the end-screen
            f_Timer += Time.deltaTime;

            if(f_Timer >= 3.0f)
            {
                SceneManager.LoadScene("Level_MainMenu");
            }
        }
	}

    void OnTriggerEnter(Collider collider_)
    {
        if(collider_.gameObject.name == "Mech")
        {
            b_Enabled = true;

            GameObject.Find("Canvas").SetActive(false);
            GameObject.Find("Mech").GetComponent<Cs_MechBaseController>().EndGame();
        }
    }
}
