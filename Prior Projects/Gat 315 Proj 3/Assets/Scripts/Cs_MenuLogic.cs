using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Cs_MenuLogic : MonoBehaviour
{
    public string goToScene;
    float f_Timer;
    public float f_MaxTime = 5.0f;

	// Use this for initialization
	void Start ()
    {
        f_Timer = 0;
        Time.timeScale = 1.0f;
	}
	
	// Update is called once per frame
	void Update ()
    {
        f_Timer += Time.deltaTime;

        if(Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Escape))
        {
            f_Timer = f_MaxTime;
        }

        if (f_Timer >= f_MaxTime)
        {
            SceneManager.LoadScene(goToScene);
        }
	}
}
