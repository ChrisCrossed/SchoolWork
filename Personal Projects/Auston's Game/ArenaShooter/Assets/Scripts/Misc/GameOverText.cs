using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
[RequireComponent(typeof(RectTransform))]
public class GameOverText : MonoBehaviour
{
    private bool animating;
    public int team;

    private float animationTimer;
    public float animationDuration;
    public AnimationCurve animationCurve = AnimationCurve.EaseInOut(0,0,1,1);

    public Color victoryColor = Color.green;
    public Color defeatColor = Color.red;

    private Text text = null;
    private RectTransform rectTransform = null;

    // Use this for initialization
    void Start ()
    {
        team = transform.root.GetComponent<AffiliatedObject>().team;

        text = GetComponent<Text>();
        rectTransform = GetComponent<RectTransform>();
        Events.GameOver.Subscribe(OnGameOver);

        float _scale = animationCurve.Evaluate(0);
        rectTransform.localScale = new Vector3(_scale, _scale, _scale);
    }

    private void OnGameOver(int _winningTeam)
    {
        animationTimer = 0;
        animating = true;

        if(_winningTeam == team)
        {
            text.color = victoryColor;
            text.text = "Victory";
        }
        else
        {
            text.color = defeatColor;
            text.text = "Defeat";
        }

        //StartCoroutine(Animate());
    }

    //private IEnumerator Animate()
    //{
    //    animationTimer += Time.deltaTime;
    //    float _animationRatio = animationTimer / animationDuration;
    //    bool _finished = _animationRatio > 1;

    //    if (_finished) _animationRatio = 1;

    //    float _scale = animationCurve.Evaluate(_animationRatio);
    //    rectTransform.localScale = new Vector3(_scale, _scale, _scale);

    //    //if(_finished)
    //    //    yield break;
    //    //else
    //        //yield return null;
    //}

    private void OnDestroy()
    {
        Events.GameOver.Unsubscribe(OnGameOver);
    }

    private void Update()
    {
        if (!animating)
            return;

        animationTimer += Time.deltaTime;
        float _animationRatio = animationTimer / animationDuration;
        animating = _animationRatio < 1;

        float _scale = animationCurve.Evaluate(_animationRatio);
        rectTransform.localScale = new Vector3(_scale, _scale, _scale);
    }
}
