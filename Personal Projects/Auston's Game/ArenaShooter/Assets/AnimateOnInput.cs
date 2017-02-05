using UnityEngine;
using System.Collections;
using System;

public class AnimateOnInput : MonoBehaviour
{
    public KeyCode key = KeyCode.None;
    public GamePadInput.Button gamepadButton = GamePadInput.Button.None;
    public bool mouseClickActivates = false;
    public bool anyInputActivates = true;

    public Animate[] toAnimate;
    public float newSpeed;

    public bool preventReset = true;
    private bool activated = false;

	// Use this for initialization
	void Update ()
    {
        if (anyInputActivates && Input.anyKeyDown)
            OnInputActivated();
        else if (key != KeyCode.None && Input.GetKeyDown(key))
            OnInputActivated();
        else if (gamepadButton != GamePadInput.Button.None && GamePadInput.GetInputTriggered(0, gamepadButton))
            OnInputActivated();
        else if (mouseClickActivates && Input.GetMouseButtonDown(0))
            OnInputActivated();
    }

    private void OnInputActivated()
    {
        if (preventReset && activated) return;
        activated = true;

        for (int i = 0; i < toAnimate.Length; i++)
        {
            if (newSpeed != 0)
                toAnimate[i].speed = newSpeed;
            toAnimate[i].Play();
        }
    }
}
