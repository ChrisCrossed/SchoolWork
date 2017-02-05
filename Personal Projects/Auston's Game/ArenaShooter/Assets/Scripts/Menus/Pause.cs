using UnityEngine;
using System.Collections;
using System;

public class Pause : MonoBehaviour
{
    private static bool isPaused_;
    public static bool isPaused
    {
        get { return isPaused_; }
        set { ChangePauseState(value); }
    }
    public Behaviour[] toggleOnPause;
    private static Pause instance;
    public Menu menuToOpen;

    // Use this for initialization
    void Start ()
    {
        instance = this;
        InputEvents.Pause.Subscribe(OnPause); //for detecting pause input
        SetBehaviours();
    }
    
    private static void SetBehaviours()
    {
        for (int i = 0; i < instance.toggleOnPause.Length; i++)
            instance.toggleOnPause[i].enabled = isPaused;
    }
    private void OnPause(InputEventInfo _inputInfo)
    {
        isPaused = !isPaused;
    }

    public static void TogglePause(bool _waitForEndOfFrame = true)
    {
        if (_waitForEndOfFrame)
            instance.StartCoroutine(TogglePauseAtEndOfFrame());
        else
            isPaused = !isPaused;
    }
    private static IEnumerator TogglePauseAtEndOfFrame()
    {
        yield return new WaitForEndOfFrame();
        isPaused = !isPaused;
    }
    private static void ChangePauseState(bool value)
    {
        if (isPaused == value) return;
        Events.TogglePause.Send();
        isPaused_ = value;

        SoundManager.PauseSound(isPaused);

        if (isPaused)
        {
            //Debug.Log("Game Paused");
            //Cursor.lockState = CursorLockMode.None;
            GameManager.instance.allowGameInput = false;

            Time.timeScale = 0f;
            Time.fixedDeltaTime = 0;

            Menu.GoTo(instance.menuToOpen);
        }
        else
        {
            //Debug.Log("Game Unpaused");
            //Cursor.lockState = CursorLockMode.Locked;
            GameManager.instance.allowGameInput = true;

            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f;

            Menu.Clear();
        }

        SetBehaviours();
    }

    void OnDestroy()
    {
        InputEvents.Pause.Unsubscribe(OnPause);
    }
}
