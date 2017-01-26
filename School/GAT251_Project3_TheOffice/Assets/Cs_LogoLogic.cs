using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Cs_LogoLogic : MonoBehaviour
{
    [SerializeField] int i_NextScene;
    [SerializeField] bool b_IsMainMenu = false;
    float f_Timer;
    static float f_Timer_Max = 5f;

    // Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();

        if(!b_IsMainMenu)
        {
            f_Timer += Time.deltaTime;
        }

        if(Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            f_Timer = f_Timer_Max;
        }

        if(f_Timer >= f_Timer_Max)
        {
            if (b_IsMainMenu) GameObject.Find("LoadingText").GetComponent<Text>().text = "LOADING";

            SceneManager.LoadScene(i_NextScene);
        }
	}
}
