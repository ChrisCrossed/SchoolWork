using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameManager : MonoBehaviour
{
    public int startingScore = 0;
    public int winningScore;

    public float rocketRespawnTime = 10;
    public float killThreshold = -4;
    public int sensitivityDefault;
    public int startingRocketCountOverride = -1;

    public static GameManager instance;
    [System.NonSerialized]
    public bool gameOver = false;

    public Color positiveColor = Color.green;
    public Color negativeColor = Color.red;

    public float delayBeforeExitMatch = 5;
    public string sceneToExitTo = "Game";

    public bool allowGameInput { get { return disallowInputs == 0; } set { if (value) --disallowInputs; else ++disallowInputs; } }
    private int disallowInputs = 1; //start with one disallow (to be removed after intro)

    public float introDuration = 3;

    // Use this for initialization
    void Awake()
    {
        instance = this;
        Events.ScoreChanged.Subscribe(OnScoreChanged);
        for (int i = 0; i < Player.scores.Length; i++)
            Player.scores[i] = startingScore;
    }

    void Start()
    {
        InputEvents.Restart.Subscribe(OnRestart);

        StartCoroutine(AllowInputAfterTime(introDuration));
    }

    private IEnumerator AllowInputAfterTime(float _introDuration)
    {
        yield return new WaitForSeconds(_introDuration);
        allowGameInput = true;
    }

    void OnRestart(InputEventInfo _inputInfo)
    {
        StartCoroutine(RestartAfterTime(0));
    }

    private void OnScoreChanged(int team)
    {
        if (gameOver || winningScore <= 0) return;

        if (Player.scores[team] >= winningScore)
        {
            Events.GameOver.Send(team);
            gameOver = true;
            StartCoroutine(RestartAfterTime(delayBeforeExitMatch));
        }
    }

    public static IEnumerator RestartAfterTime(float _time)
    {
        yield return new WaitForSeconds(_time);
        Restart();
    }

    public static void Restart()
    {

#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
        return;
#endif
        SceneManager.LoadScene(instance.sceneToExitTo);

    }

    void OnDestroy()
    {
        Events.ScoreChanged.Unsubscribe(OnScoreChanged);
        InputEvents.Restart.Unsubscribe(OnRestart);
    }
}