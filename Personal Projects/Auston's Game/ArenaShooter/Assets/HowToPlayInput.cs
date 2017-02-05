using UnityEngine;
using System.Collections;

public class HowToPlayInput : MonoBehaviour
{
    public Menu howToPlayMenu;

    // Use this for initialization
    void Start ()
    {
        InputEvents.HowToPlay.Subscribe(OnHowToPlay);
    }

    void OnHowToPlay(InputEventInfo _inputInfo)
    {
        Pause.TogglePause(false);

        if (_inputInfo.inputState == InputState.Triggered)
        {
            Menu.GoTo(howToPlayMenu);
        }
        else
        {
            Menu.Clear();
        }
    }

    void OnDestroy()
    {
        InputEvents.HowToPlay.Unsubscribe(OnHowToPlay);
    }
}
