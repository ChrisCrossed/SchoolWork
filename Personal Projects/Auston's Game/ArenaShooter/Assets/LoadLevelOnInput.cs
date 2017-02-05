using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadLevelOnInput : MonoBehaviour
{
    public KeyCode key = KeyCode.None;
    public GamePadInput.Button gamepadButton = GamePadInput.Button.None;
    public bool mouseClickActivates = false;
    public bool anyInputActivates = true;

    public string sceneToLoad;

    public float delayBeforeLoad = 0;
	
	// Update is called once per frame
	void Update ()
    {
        if (anyInputActivates && Input.anyKeyDown)
            StartCoroutine(LoadAfterTime());
        else if (key != KeyCode.None && Input.GetKeyDown(key))
            StartCoroutine(LoadAfterTime());
        else if (gamepadButton != GamePadInput.Button.None && GamePadInput.GetInputTriggered(0, gamepadButton))
            StartCoroutine(LoadAfterTime());
        else if (mouseClickActivates && Input.GetMouseButtonDown(0))
            StartCoroutine(LoadAfterTime());
    }

    private IEnumerator LoadAfterTime()
    {
        yield return new WaitForSeconds(delayBeforeLoad);
        SceneManager.LoadScene(sceneToLoad);
    }
}
