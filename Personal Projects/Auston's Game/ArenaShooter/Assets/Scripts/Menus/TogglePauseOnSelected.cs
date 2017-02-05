public class TogglePauseOnSelected : Selectable
{
    public override void OnSelected()
    {
        Pause.TogglePause();
    }
}
