using UnityEngine;
using System.Collections;
using System;

public class AnimateAfterTime : MonoBehaviour
{
    public float timeBeforeAnimate = 1;

    public Animate[] toAnimate;
    public float newSpeed;

    public bool preventReset = true;
    private bool activated = false;

    // Use this for initialization
    void Update()
    {
        if (timeBeforeAnimate <= 0) return;
        timeBeforeAnimate -= Time.deltaTime;
        if (timeBeforeAnimate <= 0)
            OnTimePassed();
    }

    private void OnTimePassed()
    {
        activated = true;

        for (int i = 0; i < toAnimate.Length; i++)
        {
            if (toAnimate[i].animating)
            {
                Debug.Log("Got here");
                continue;
            }

            if (newSpeed != 0)
                toAnimate[i].speed = newSpeed;

            toAnimate[i].Play();
        }
    }
}
