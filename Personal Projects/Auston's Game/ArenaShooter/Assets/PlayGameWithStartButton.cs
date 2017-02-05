using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using XInputDotNetPure;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif
using UnityEngine.SceneManagement;

[ExecuteInEditMode]
public class PlayGameWithStartButton : MonoBehaviour
{
#if UNITY_EDITOR
    GamePadState previousState;

    PlayGameWithStartButton()
    {
        EditorApplication.update += EditorUpdate;
    }

    void EditorUpdate()
    {
        GamePadState _state = GamePad.GetState(0);

        if (_state.Buttons.Start == ButtonState.Pressed && previousState.Buttons.Start == ButtonState.Released)
        {
            if (EditorApplication.isPlaying == true) return;
            if (!EditorApplication.isPlayingOrWillChangePlaymode)
                EditorSceneManager.SaveScene(SceneManager.GetActiveScene(), "", false);
            EditorApplication.ExecuteMenuItem("Edit/Play");
        }

        previousState = _state;
    }

    ~PlayGameWithStartButton()
    {
        EditorApplication.update -= EditorUpdate;
    }
#endif
}
