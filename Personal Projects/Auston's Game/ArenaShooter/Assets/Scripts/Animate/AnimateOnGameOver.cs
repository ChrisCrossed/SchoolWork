using UnityEngine;
using System.Collections;

public class AnimateOnGameOver : MonoBehaviour
{
    public Animate animateComponent;
    public float newSpeed = float.NaN;
    public bool delay = false;

	// Use this for initialization
	void Start ()
    {
        Events.PlayerDied.Subscribe(OnPlayerDied);
        if (animateComponent == null)
            animateComponent = GetComponent<Animate>();
	}

    void OnPlayerDied(Player _player, GameObject _killedBy)
    {
        if (_player != transform.root.GetComponent<Player>())
            return;

        if (!float.IsNaN(newSpeed))
            animateComponent.speed = newSpeed;

        float _animationDuration = animateComponent.duration / Mathf.Abs(animateComponent.speed);
        float _animationDelay = GameManager.instance.delayBeforeExitMatch - _animationDuration;

        if(delay)
            StartCoroutine(PlayAfterTime(_animationDelay));
        else if(animateComponent != null)
            animateComponent.Play();
    }

    IEnumerator PlayAfterTime(float _time)
    {
        yield return new WaitForSeconds(_time);

        if (animateComponent != null)
            animateComponent.Play();
    }
	
	// Update is called once per frame
	void OnDestroy ()
    {
        Events.PlayerDied.Unsubscribe(OnPlayerDied);
    }
}
