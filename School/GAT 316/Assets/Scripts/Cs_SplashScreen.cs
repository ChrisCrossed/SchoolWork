using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Cs_SplashScreen : MonoBehaviour
{
    float f_Timer = 5.0f;
    [SerializeField] int i_ToScene;

    void Start()
    {
        Cursor.visible = false;
    }
  
	// Update is called once per frame
	void Update ()
    {
        f_Timer -= Time.deltaTime;

        if(f_Timer <= 0f)
        {
            SceneManager.LoadScene(i_ToScene);
        }
	}
}
