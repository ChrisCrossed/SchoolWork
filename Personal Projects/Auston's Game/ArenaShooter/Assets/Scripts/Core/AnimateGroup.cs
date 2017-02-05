using UnityEngine;
using System.Collections;
using System;

public class AnimateGroup : MonoBehaviour
{
    public Animate[] animations;

    // Use this for initialization
    public void Play()
    {
        for (int i = 0; i < animations.Length; i++)
            animations[i].Play();
    }
    public void Stop()
    {
        for (int i = 0; i < animations.Length; i++)
            animations[i].Stop();
    }
    public void SetDuration(float _newDuration)
    {
        for (int i = 0; i < animations.Length; i++)
            animations[i].duration = _newDuration;
    }
}
