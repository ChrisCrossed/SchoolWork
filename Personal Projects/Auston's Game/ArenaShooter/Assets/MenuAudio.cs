using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class MenuAudio : MonoBehaviour
{
    public static AudioSource audioSource;
    public static MenuAudio instance;
    public AudioMixerGroup hoverOutput;
    public AudioMixerGroup selectOutput;
    public AudioMixerGroup backOutput;

    public AudioClip hoverSound;
    public AudioClip selectSound;
    public AudioClip backSound;

    // Use this for initialization
    void Start()
    {
        instance = this;
        audioSource = GetComponent<AudioSource>();
    }

    public static void PlayOnHover()
    {
        audioSource.clip = instance.hoverSound;
        audioSource.outputAudioMixerGroup = instance.hoverOutput;
        audioSource.Play();
    }
    public static void PlayOnSelect()
    {
        audioSource.clip = instance.selectSound;
        audioSource.outputAudioMixerGroup = instance.selectOutput;
        audioSource.Play();
    }
    public static void PlayOnPrevious()
    {
        instance.StartCoroutine(PlayAfterEndOfFrame(instance.backSound, instance.backOutput));
    }

    private static IEnumerator PlayAfterEndOfFrame(AudioClip _clip, AudioMixerGroup _output)
    {
        yield return new WaitForEndOfFrame();
        
        audioSource.clip = _clip;
        audioSource.outputAudioMixerGroup = _output;
        audioSource.Play();
    }
}
