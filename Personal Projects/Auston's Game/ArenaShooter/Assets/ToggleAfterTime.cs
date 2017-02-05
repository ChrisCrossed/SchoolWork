using UnityEngine;
using System.Collections;
using System;

public class ToggleAfterTime : MonoBehaviour
{
    public float timeBeforeEnable = 1;

    public Behaviour[] toEnable;
    public float newSpeed;

    // Use this for initialization
    void Update()
    {
        if (timeBeforeEnable <= 0) return;
        timeBeforeEnable -= Time.deltaTime;
        if (timeBeforeEnable <= 0)
            OnTimePassed();
    }

    private void OnTimePassed()
    {
        for (int i = 0; i < toEnable.Length; i++)
        {
            toEnable[i].enabled = !toEnable[i].enabled;
        }
    }
}
