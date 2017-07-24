using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cs_EndgameTrigger : MonoBehaviour
{
    // Player Connection
    GameObject go_Player;
    GameObject go_Backdrop;

    // Timer
    bool b_Begin;
    float f_Timer;
    int i_Counter;

    // Lights Sequence
    [SerializeField] GameObject[] go_Lights;
    [SerializeField] GameObject[] go_SFX;

    // Use this for initialization
    void Start ()
    {
        go_Player = GameObject.Find("Player");
        go_Backdrop = GameObject.Find("Backdrop");
    }
	
	// Update is called once per frame
	void Update ()
    {
		if(b_Begin)
        {
            f_Timer += Time.deltaTime;

            if(f_Timer > 1f)
            {
                // Reset timer
                f_Timer = 0f;

                // Disable light (if exists)
                if(i_Counter < go_Lights.Length)
                {
                    go_Lights[i_Counter].SetActive(false);
                }
                else // If the final light is done, End the game
                {
                    Endgame();

                    b_Begin = false;
                }

                // Play sound effects at location
                go_SFX[i_Counter].GetComponent<AudioSource>().Play();

                // Increment counter
                ++i_Counter;
            }
        }
	}

    void Endgame()
    {
        // Enable the backdrop
        go_Backdrop.GetComponent<Image>().enabled = true;

        // Disable the player's controller
        go_Player.GetComponent<Cs_PlayerController>().enabled = false;
    }

    private void OnTriggerEnter(Collider collider_)
    {
        if(collider_.gameObject == go_Player.gameObject)
        {
            if(!b_Begin) b_Begin = true;

            f_Timer = 1.0f;
        }
    }
}
