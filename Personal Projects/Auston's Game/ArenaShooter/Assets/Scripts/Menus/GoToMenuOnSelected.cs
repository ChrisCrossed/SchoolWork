using UnityEngine;

public class GoToMenuOnSelected : Selectable
{
    public Menu menuToGoTo;
    public bool goToPrevious;

    public override void OnSelected()
    {
        if (menuToGoTo)
            Menu.GoTo(menuToGoTo);
        else if (goToPrevious)
            Menu.GoToPrevious();
    }
}
