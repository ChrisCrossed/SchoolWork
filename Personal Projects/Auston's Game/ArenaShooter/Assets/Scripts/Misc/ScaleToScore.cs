using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ScaleToScore : MonoBehaviour
{
    public int team;
    private Image imageToScale = null;

	// Use this for initialization
	void Start ()
    {
        Events.ScoreChanged.Subscribe(OnScoreChanged);
        imageToScale = GetComponent<Image>();

        OnScoreChanged(team);
    }

    void OnScoreChanged(int _team)
    {
        if (_team != team) return;

        imageToScale.fillAmount = (float)Player.scores[team] / GameManager.instance.winningScore;
    }
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    void OnDestroy()
    {
        Events.ScoreChanged.Unsubscribe(OnScoreChanged);
    }
}
