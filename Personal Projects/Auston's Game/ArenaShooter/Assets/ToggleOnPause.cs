using UnityEngine;
using System.Collections;

public class ToggleOnPause : MonoBehaviour
{
    public Behaviour[] behaviorsToToggle;

	// Use this for initialization
	void Start ()
    {
        Events.TogglePause.Subscribe(OnTogglePause);
	}

    void OnTogglePause()
    {
        for (int i = 0; i < behaviorsToToggle.Length; i++)
            behaviorsToToggle[i].enabled = !behaviorsToToggle[i].enabled;
    }

    void OnDestroy()
    {
        Events.TogglePause.Unsubscribe(OnTogglePause);
    }
}
