using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Player : AffiliatedObject
{
    public int index = -1;
    private static int[] scores_ = new int[2];
    public static int[] scores
    {
        get { return scores_; }
        set
        {
            if (value == scores) return; //nothing changed

            scores_ = value;
        }
    }

    public int score { get { return scores[team]; } set { scores[team] = value; } }

    public string nearWinText = "Nearing victory";
    public string nearLossText = "Enemy nearing victory";

    [Header("Score Animations")]
    public static MethodDelegate scoreSetDelegate = delegate { };
    public Text[] scoreTexts = new Text[2];
    private AnimateScale[] scoreScaleAnimations = new AnimateScale[2];
    public AnimateRotation nextScoreLosesAnimation = null;
    public AnimateScale nextScoreWinsAnimation = null;
    public Text endGameWarningText;

    [Header("On Kill Animations")]
    public Animate[] animateOnKill;
    public GameObject createOnKilled;

    new public Transform camera;

    new void Start()
    {
        base.Start();
        name = "Player " + (index + 1);
        PlayerManager.players[index] = this;

        if (camera == null)
            camera = transform;

        int mask = 0;
        for (int i = 0; i < 4; i++)
        {
            if (index == i) continue;
            mask |= 1 << (i + 12);
        }
        camera.GetComponent<Camera>().cullingMask = ~mask;

        if (scoreTexts[0] == null || scoreTexts[1] == null)
            return;

        scoreScaleAnimations[0] = scoreTexts[0].GetComponent<AnimateScale>();
        scoreScaleAnimations[1] = scoreTexts[1].GetComponent<AnimateScale>();

        Events.ScoreChanged.Subscribe(UpdateAndAnimateScoreText);
        Events.GameOver.Subscribe(OnGameOver);
        Events.PlayerDied.Subscribe(OnPlayerDied);

        UpdateScoreText(0); //Making sure our score texts are set correctly (don't animate)
        UpdateScoreText(1);
    }

    private void UpdateAndAnimateScoreText(int _team)
    {
        UpdateScoreText(_team);

        scoreScaleAnimations[_team].Stop();
        scoreScaleAnimations[_team].Play();
    }
    private void UpdateScoreText(int _team)
    {
        scoreTexts[_team].text = scores[_team].ToString(); //setting the numeric score on the score display
        //Debug.Log("Player " + team + " now knows that player " + _team + " has " + scores[_team] + " points.");

        int _otherTeam = team == 0 ? 1 : 0;

        bool teamAboutToWin = scores[team] == GameManager.instance.winningScore - 1;
        bool otherTeamAboutToWin = scores[_otherTeam] == GameManager.instance.winningScore - 1;

        bool teamHasWon = scores[team] >= GameManager.instance.winningScore;
        bool otherTeamHasWon = scores[_otherTeam] >= GameManager.instance.winningScore;

        if (teamHasWon || otherTeamHasWon)
        {
            ResetScoreText();
        }
        else if (teamAboutToWin)
        {
            ResetScoreText();
            if (otherTeamAboutToWin)
                GoToSuddenDeath();
            else
                GoToAboutToWin();
        }
        else if (otherTeamAboutToWin)
        {
            ResetScoreText();
            GoToAboutToLose();
        }
        else
            ResetScoreText();
    }
    private void ResetScoreText()
    {
        endGameWarningText.text = "";
        nextScoreWinsAnimation.Stop();
        nextScoreLosesAnimation.Stop();
    }
    private void GoToSuddenDeath()
    {
        nextScoreLosesAnimation.Play();
        nextScoreWinsAnimation.Play();
        endGameWarningText.text = "Sudden Death!";
        endGameWarningText.color = Color.white;
        endGameWarningText.GetComponent<Outline>().effectColor = Color.black;
    }
    private void GoToAboutToLose()
    {
        nextScoreLosesAnimation.Play();
        endGameWarningText.text = nearLossText;
        endGameWarningText.color = GameManager.instance.negativeColor;
        endGameWarningText.GetComponent<Outline>().effectColor = Color.black;
        endGameWarningText.GetComponent<Outline>().effectDistance = Vector2.one * 0.5f;
    }
    private void GoToAboutToWin()
    {
        nextScoreWinsAnimation.Play();
        endGameWarningText.text = nearWinText;
        endGameWarningText.color = GameManager.instance.positiveColor;
        endGameWarningText.GetComponent<Outline>().effectColor = Color.black;
        endGameWarningText.GetComponent<Outline>().effectDistance = Vector2.one * 0.5f;
    }

    private void OnGameOver(int _winningTeam)
    {
        nextScoreLosesAnimation.Stop();
        nextScoreWinsAnimation.Stop();
        endGameWarningText.text = "";
    }
    private void OnPlayerDied(Player _player, GameObject _killedBy)
    {
        if (_killedBy == gameObject)
            OnKilledPlayer();
        else if (_player == this)
            OnKilled(_killedBy);
    }
    private void OnKilledPlayer()
    {
        for (int i = 0; i < animateOnKill.Length; i++)
            animateOnKill[i].Play();

        if (GameManager.instance.gameOver)
            return;

        ++score;
        Events.ScoreChanged.Send(team);
    }
    private void OnKilled(GameObject _killedBy)
    {
        CameraShake.Shake(index, 0.4f, 0.95f, 40, 5);
        CameraShake.VibrateGamepad(index, 0.55f, 0.85f);
        CameraShake.AberrateCamera(index, 0.5f, 15, 30);

        GameObject _createdObject = (GameObject)Instantiate(createOnKilled, transform.position + transform.up * 0.75f, transform.rotation);
        _createdObject.transform.localScale = Vector3.one * 1.5f;
        _createdObject.GetComponent<RecolorParticlesToTeamColor>().teamIndexOverride = team;
        _createdObject.GetComponent<RecolorLightToTeamColor>().teamIndexOverride = team;

        if (_killedBy != null)
            return;

        --score;
        Events.ScoreChanged.Send(team);
    }

    void OnDestroy()
    {
        //if(!GameManager.instance.gameOver)
        //{
        //    int otherTeam = team == 0 ? 1 : 0;
        //    scores[otherTeam]++;

        //    Events.ScoreChanged.Send(otherTeam);
        //}

        Events.ScoreChanged.Unsubscribe(UpdateAndAnimateScoreText);
        Events.GameOver.Unsubscribe(OnGameOver);
        Events.PlayerDied.Unsubscribe(OnPlayerDied);
    }
}