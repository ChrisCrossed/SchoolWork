using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Cs_LoadingScreen : MonoBehaviour
{
    [SerializeField]
    int i_GoToSceneNumber;

    [SerializeField]
    bool b_WaitForInput;

    float f_Timer;
    
    // Update is called once per frame
	void Update ()
    {
        if(!b_WaitForInput)
        {
            f_Timer += Time.deltaTime;
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.KeypadEnter)) f_Timer = 5.0f;
        }

        if(f_Timer >= 5.0f)
        {
            SceneManager.LoadScene(i_GoToSceneNumber);
        }
	}
}
