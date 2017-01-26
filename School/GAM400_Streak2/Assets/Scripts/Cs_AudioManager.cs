using UnityEngine;
using System.Collections;

/*********************************
 * 
 * Copyright DigiPen Institute of Technology 2016
 * 
 * Streak 2 by Christopher Christensen
 * 
 * *******************************/

public enum Enum_SoundEffect
{
    RotateClockwise,
    RotateCounterclock,
    Move,
    DropBlock
}

public class Cs_AudioManager : MonoBehaviour
{
    // Audio Source
    [SerializeField] AudioSource as_MusicSource;
    [SerializeField] AudioSource as_SFXSource;

    // Music
    [SerializeField] AudioClip music_GeneralHandsome;

    // SFX
    [SerializeField] AudioClip sfx_RotateBlock;
    [SerializeField] AudioClip sfx_DropBlock;
    [SerializeField] AudioClip sfx_Whoosh;

    // Use this for initialization
    void Start ()
    {
        #region Load Audio Source
        // as_MusicSource = gameObject.GetComponent<AudioSource>();
        #endregion

        Set_FirstTrack();
    }

    public void Set_FirstTrack()
    {
        // TODO: Create overload to set a specific track, rather than a random first one
        as_MusicSource.clip = music_GeneralHandsome;

        as_MusicSource.Play();
    }

    public void Set_NextTrack()
    {
        // TODO: Find next song to play

        // TODO: Play next song & switch themes

        as_MusicSource.clip = music_GeneralHandsome;

        as_MusicSource.Play();
    }

    public void Play_SoundEffect( Enum_SoundEffect e_SFX_ )
    {
        switch (e_SFX_)
        {
            case Enum_SoundEffect.RotateClockwise:
                as_SFXSource.pitch = 0.95f;
                as_SFXSource.PlayOneShot(sfx_RotateBlock);
                break;

            case Enum_SoundEffect.RotateCounterclock:
                as_SFXSource.pitch = 1.05f;
                as_SFXSource.PlayOneShot(sfx_RotateBlock);
                break;

            case Enum_SoundEffect.DropBlock:
                as_SFXSource.pitch = 1.0f;
                as_SFXSource.PlayOneShot(sfx_DropBlock);
                break;

            case Enum_SoundEffect.Move:
                as_SFXSource.pitch = 1.0f;
                as_SFXSource.PlayOneShot(sfx_Whoosh);
                break;

            default:
                break;
        }
    }
}
