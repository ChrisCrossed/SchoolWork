using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    public new static Transform transform;

    public SoundSource oneShotSound;

    [System.NonSerialized] public static List<SoundListener> soundListeners = new List<SoundListener>();
    [System.NonSerialized] public static List<SoundSource> soundSources = new List<SoundSource>();

    public static float audioPitch = 1;
    public static float audioVolume = 1;

    void Awake()
    {
        instance = this;
        transform = base.transform;
    }

    public static void PauseSound(bool _soundPaused = true)
    {
        if(_soundPaused)
        {
            for (int i = 0; i < soundListeners.Count; i++)
                for (int j = 0; j < soundListeners[i].audioSources.Count; j++)
                    soundListeners[i].audioSources[j].Pause();
        }
        else
        {
            for (int i = 0; i < soundListeners.Count; i++)
                for (int j = 0; j < soundListeners[i].audioSources.Count; j++)
                    soundListeners[i].audioSources[j].UnPause();
        }
    }

    public static void AddSoundListener(SoundListener _soundListener)
    {
        soundListeners.Add(_soundListener);

        for (int i = 0; i < soundSources.Count; i++)
        {
            SoundSource _soundSource = soundSources[i];
            AudioSource _newAudioSource = CreateAudioSource(_soundSource);

            _soundSource.audioSources.Add(_newAudioSource);
            _soundListener.audioSources.Add(_newAudioSource);
        }
    }
    public static void RemoveSoundListener(SoundListener _soundListener)
    {
        int _listenerIndex = soundListeners.IndexOf(_soundListener);
        if (_listenerIndex == -1) return;
        soundListeners.RemoveAt(_listenerIndex);

        for (int i = 0; i < soundSources.Count; i++) //for each sound source
        {
            SoundSource _soundSource = soundSources[i];
            if(_soundSource.audioSources[_listenerIndex] != null)
                Destroy(_soundSource.audioSources[_listenerIndex].gameObject);
            _soundSource.audioSources.RemoveAt(_listenerIndex);
        }
    }
    public static void AddSoundSource(SoundSource _soundSource)
    {
        soundSources.Add(_soundSource);

        for (int i = 0; i < soundListeners.Count; i++)
        {
            SoundListener _soundListener = soundListeners[i];
            AudioSource _newAudioSource = CreateAudioSource(_soundSource);

            _soundListener.audioSources.Add(_newAudioSource);
            _soundSource.audioSources.Add(_newAudioSource);
        }
    }
    public static void RemoveSoundSource(SoundSource _soundSource)
    {
        int _sourceIndex = soundSources.IndexOf(_soundSource);
        if (_sourceIndex == -1) return;
        soundSources.RemoveAt(_sourceIndex);

        for (int i = 0; i < soundListeners.Count; i++) //for each sound source
        {
            SoundListener _soundListener = soundListeners[i];
            if(_soundListener.audioSources[_sourceIndex] != null)
                Destroy(_soundListener.audioSources[_sourceIndex].gameObject);
            _soundListener.audioSources.RemoveAt(_sourceIndex);
        }
    }

    static public void PlayOneShot(Vector3 _position, AudioClip _clip, AudioMixerGroup _mixerGroup, AnimationCurve _volumeOverTime = default(AnimationCurve))
    {
        GameObject _newObject = (GameObject)Instantiate(instance.oneShotSound.gameObject, _position, Quaternion.identity);
        SoundSource _newSoundSource = _newObject.GetComponent<SoundSource>();
        
        _newSoundSource.audioClip = _clip;
        _newSoundSource.mixerGroup = _mixerGroup;
        _newSoundSource.destroyOnFinish = true;

        if (_volumeOverTime != default(AnimationCurve))
            _newSoundSource.volumeOverTime = _volumeOverTime;
    }

    public static AudioSource CreateAudioSource(SoundSource _soundSource)
    {
        GameObject _newSoundObject = new GameObject();
        _newSoundObject.transform.parent = transform;
        AudioSource _newAudioSource = _newSoundObject.AddComponent<AudioSource>();

        _newAudioSource.Stop();
        _newAudioSource.clip = _soundSource.audioClip;
        _newAudioSource.outputAudioMixerGroup = _soundSource.mixerGroup;
        _newAudioSource.priority = _soundSource.priority;
        _newAudioSource.spatialBlend = 1;
        _newAudioSource.rolloffMode = AudioRolloffMode.Logarithmic;
        _newAudioSource.volume = _soundSource.volume;
        _newAudioSource.loop = _soundSource.loop;
        _newAudioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, _soundSource.volumeCurve);
        _newAudioSource.maxDistance = 0;

        if (_soundSource.isPlaying)
        {
            _newAudioSource.Play();
            _newAudioSource.time = _soundSource.time;
        }
        else if(_soundSource.playAtStart)
            _newAudioSource.Play();

        return _newAudioSource;
    }

    public static SoundSource PlayAtPosition(AudioClip _clip, Vector3 _position, AudioMixerGroup _mixerGroup = null,
        AnimationCurve _volumeCurve = default(AnimationCurve), bool _destroyAfterFinish = true)
        {
            GameObject _newSoundObject = new GameObject();
            _newSoundObject.transform.position = _position;
            SoundSource _newSoundSource = _newSoundObject.AddComponent<SoundSource>();
            _newSoundSource.audioClip = _clip;
            _newSoundSource.mixerGroup = _mixerGroup;
            _newSoundSource.volumeCurve = _volumeCurve;

            return _newSoundSource;
        }
}