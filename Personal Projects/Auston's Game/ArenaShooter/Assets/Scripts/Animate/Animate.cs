using UnityEngine;
using System.Collections;

[System.Serializable]
public class AnimateArray
{
    public Animate[] animations;
}

public abstract class Animate : MonoBehaviour
{
    public bool setStartValueOnStart = true;
    public bool playAtStart = true;
    public bool stopAtStart = false;
    public float delay = 0;
    public bool loop = true;
    public bool reverseOnLoop = false;

    public float speed = 1;
    public float duration = 1;
    public AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);

    private float timer = 0;
    public bool animating { get; private set; }
    public enum StopType { Stay, GoToStart, GoToEnd }
    public StopType onCompletion = StopType.GoToEnd;

    public bool useFixedUpdate = false;
    public bool usesUnscaledTime = false;

    // Use this for initialization
    protected virtual void Start()
    {
        if (setStartValueOnStart)
            SetStartValue();

        if (stopAtStart)
            Stop();

        if (playAtStart)
        {
            if (delay > 0)
                StartCoroutine(PlayAfterDelay());
            else
                Play();
        }
    }
    private IEnumerator PlayAfterDelay()
    {
        yield return new WaitForSeconds(delay);
        Play();
    }
    protected virtual void SetStartValue()
    {

    }
    public void Play(bool _reset = true, bool _reverse = false)
    {
        if (_reverse)
            speed *= -1;

        if (_reset)
            Stop();

        animating = true;
    }
    public void Pause()
    {
        Stop(StopType.Stay);
    }
    public void Stop(StopType _stopType = StopType.GoToStart)
    {
        animating = false;
        if (_stopType == StopType.Stay)
            return;

        if (_stopType == StopType.GoToStart)
            timer = speed < 0 ? duration : 0;
        else
            timer = speed > 0 ? duration : 0;

        SetValueFromRatio(timer / duration);
    }
    void UpdateAnimation(float _deltaTime)
    {
        if (animating)
        {
            timer += _deltaTime * speed;
            float _completionRatio = timer / duration;

            if (_completionRatio > 1 || _completionRatio < 0)
            {
                if (reverseOnLoop)
                    speed *= -1;

                Stop(onCompletion);

                if (loop)
                    Play();

                return;
            }
            
            SetValueFromRatio(_completionRatio);
        }
    }
    void FixedUpdate()
    {
        if (useFixedUpdate) UpdateAnimation(Time.fixedDeltaTime);
    }
    void Update()
    {
        if (!useFixedUpdate) UpdateAnimation(usesUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime);
    }
    protected virtual void SetValueFromRatio(float _ratio)
    {
        
    }
}
