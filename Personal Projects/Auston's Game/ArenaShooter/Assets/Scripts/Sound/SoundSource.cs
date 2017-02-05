    using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Audio;

public class SoundSource : MonoBehaviour
{
    public AudioClip audioClip;
    public AudioMixerGroup mixerGroup;
    
    public bool playAtStart = false;
    public bool loop;

    public float range = 1;
    [Range(0,1)]
    public float volume = 1;
    [System.NonSerialized] public float volumeOffsetRatio = 1;
    [System.NonSerialized] public float pitchOffsetRatio = 1;
    public AnimationCurve volumeCurve = AnimationCurve.EaseInOut(0,1,1,0);
    [Range(0, 256)]
    public int priority = 128;

    public float time { get { if (audioSources.Count == 0) return 0; else return audioSources[0].time; } }
    public bool isPlaying { get { if (audioSources.Count == 0) return false; else return audioSources[0].isPlaying; } }
    public float completionRatio { get { if (audioSources.Count == 0) return 0; else return audioSources[0].time / audioSources[0].clip.length; } }

    public AnimationCurve volumeOverTime = AnimationCurve.Linear(0, 1, 1, 1);

    [System.NonSerialized]
    public bool destroyOnFinish = false;

    [System.NonSerialized] public List<AudioSource> audioSources = new List<AudioSource>();

    // Use this for initialization
    void Start()
    {
        SoundManager.AddSoundSource(this);
    }

    public void Play()
    {
        for (int i = 0; i < audioSources.Count; i++)
        {
            audioSources[i].Stop();
            audioSources[i].Play();
        }
    }
    public void Resume()
    {
        for (int i = 0; i < audioSources.Count; i++)
            audioSources[i].UnPause();
    }
    public void Pause()
    {
        for (int i = 0; i < audioSources.Count; i++)
            audioSources[i].Pause();
    }
    public void Stop()
    {
        for (int i = 0; i < audioSources.Count; i++)
            audioSources[i].Stop();
    }

    public void Update()
    {
        //Debug.Log(gameObject.name + " | " + audioSources.Count);

        if (isPlaying)
        {
            float _newVolume = volume * volumeOverTime.Evaluate(completionRatio) * volumeOffsetRatio;

            for (int i = 0; i < audioSources.Count; i++)
            {
                audioSources[i].volume = _newVolume * SoundManager.audioVolume * ((float)Options.volume / 10f);
                audioSources[i].pitch = pitchOffsetRatio * SoundManager.audioPitch;
            }
        }
        else if(destroyOnFinish)
            Destroy(gameObject);
    }

    void OnDestroy()
    {
        SoundManager.RemoveSoundSource(this);
    }
}
