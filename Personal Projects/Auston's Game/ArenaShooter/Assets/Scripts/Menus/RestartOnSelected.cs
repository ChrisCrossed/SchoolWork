using UnityEngine;

public class RestartOnSelected : Selectable
{
    private float timeBeforeRestart = float.PositiveInfinity;

    public override void OnSelected()
    {
        Pause.TogglePause();
        GameManager.Restart();
    }
}
