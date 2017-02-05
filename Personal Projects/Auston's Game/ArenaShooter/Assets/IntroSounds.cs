using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class IntroSounds : MonoBehaviour
{
    private AudioSource audioSource;

    public AudioMixerGroup readySetOutput;
    public AudioMixerGroup goOutput;

    public float delayBeforeReady;
    public float delayBeforeSet;
    public float delayBeforeGo;

    public AudioClip readySound;
    public AudioClip setSound;
    public AudioClip goSound;

    // Use this for initialization
    void Start ()
    {
        audioSource = GetComponent<AudioSource>();

        StartCoroutine(PlayAfterTime(delayBeforeReady, readySound, readySetOutput));
        StartCoroutine(PlayAfterTime(delayBeforeSet, setSound, readySetOutput));
        StartCoroutine(PlayAfterTime(delayBeforeGo, goSound, goOutput));
    }
	
	// Update is called once per frame
	private IEnumerator PlayAfterTime(float _time, AudioClip _clip, AudioMixerGroup _mixerGroup)
    {
        yield return new WaitForSeconds(_time);

        audioSource.outputAudioMixerGroup = _mixerGroup;
        audioSource.clip = _clip;
        audioSource.Play();
    }
}
