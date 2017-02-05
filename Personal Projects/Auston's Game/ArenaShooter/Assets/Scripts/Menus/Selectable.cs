using UnityEngine;
using System.Collections;

public abstract class Selectable : MonoBehaviour
{
    public abstract void OnSelected();
    public virtual void OnAltDirectionalInput(int _positivity) { }
    public virtual void OnExitHover() { }
    public virtual void OnEnterHover() { }
}
