using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Cs_MainMenu : MonoBehaviour
{
    GameObject go_One;
    GameObject go_Two;

    // Use this for initialization
    void Start()
    {
        go_One = GameObject.Find("DigiPen");
        go_Two = GameObject.Find("Main");

        // Lock mouse cursor
        Cursor.lockState = CursorLockMode.Locked;
    }

    float f_Timer = 0f;
    bool b_Begin;
    // Update is called once per frame
    void Update()
    {
        f_Timer += Time.deltaTime;

        if (f_Timer > 1.0f && f_Timer < 5.0f) go_One.GetComponent<Image>().enabled = true;
        else go_One.GetComponent<Image>().enabled = false;

        if (f_Timer > 7.0f) go_Two.GetComponent<Image>().enabled = true;

        if (f_Timer > 7f)
        {
            if(!b_Begin && Input.anyKey)
            {
                b_Begin = true;
            }

            if(b_Begin)
            {
                Color clr_ = go_Two.GetComponent<Image>().color;
                clr_.a -= Time.deltaTime / 3.0f;
                go_Two.GetComponent<Image>().color = clr_;

                if (clr_.a <= 0f) SceneManager.LoadScene(1);
            }
        }
    }
}
