using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class QuitGameOnSelected : Selectable
{
    public override void OnSelected()
    {
        Application.Quit();
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
    }
}
