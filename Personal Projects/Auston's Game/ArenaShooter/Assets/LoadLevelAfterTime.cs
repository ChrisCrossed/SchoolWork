using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadLevelAfterTime : MonoBehaviour
{
    public float timeBeforeLoad = 1;

    public string sceneToLoad;
    public float delayBeforeLoad = 0;

    // Update is called once per frame
    void Update()
    {
        if (timeBeforeLoad <= 0) return;

        timeBeforeLoad -= Time.deltaTime;
        if (timeBeforeLoad <= 0)
            StartCoroutine(LoadAfterTime());
    }

    private IEnumerator LoadAfterTime()
    {
        yield return new WaitForSeconds(delayBeforeLoad);
        SceneManager.LoadScene(sceneToLoad);
    }
}
