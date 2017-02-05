using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class AnimateVolume : AnimateFloat
{
    private AudioSource audioSource;

    protected override void Start()
    {
        audioSource = GetComponent<AudioSource>();

        base.Start();
    }

    protected override void SetNewValue(float _newValue)
    {
        audioSource.volume = _newValue;
    }
}
