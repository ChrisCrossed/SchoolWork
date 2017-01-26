using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Cs_MenuSystem : MonoBehaviour
{
    [SerializeField] int i_GoToScene;
    float f_Timer;
    [SerializeField] bool b_ButtonContinues = false;

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        { 
            f_Timer = 7.0f;
        }

        if(!b_ButtonContinues)
        {
            f_Timer += Time.deltaTime;
        }

        if(f_Timer >= 7.0f)
        {
            SceneManager.LoadScene(i_GoToScene);
        }
	}
}
