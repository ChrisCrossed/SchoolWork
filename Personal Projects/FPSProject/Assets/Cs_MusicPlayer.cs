using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Enum_Song
{
    One,
    Two,
    Three,
    None
}

public class Cs_MusicPlayer : MonoBehaviour
{
    GameObject MusicSource_One;
    AudioSource this_MusicSource_One;

    GameObject MusicSource_Two;
    AudioSource this_MusicSource_Two;

    GameObject MusicSource_Three;
    AudioSource this_MusicSource_Three;

    Enum_Song e_Song;

    // Use this for initialization
    void Start ()
    {
        MusicSource_One = transform.Find("MusicSource_One").gameObject;
        this_MusicSource_One = MusicSource_One.GetComponent<AudioSource>();

        MusicSource_Two = transform.Find("MusicSource_Two").gameObject;
        this_MusicSource_Two = MusicSource_Two.GetComponent<AudioSource>();

        MusicSource_Three = transform.Find("MusicSource_Three").gameObject;
        this_MusicSource_Three = MusicSource_Three.GetComponent<AudioSource>();

        e_Song = Enum_Song.None;
    }

    public Enum_Song RunningSong
    {
        set
        {
            if(e_Song != value)
            {
                e_Song = value;

                if (e_Song == Enum_Song.One) this_MusicSource_One.Play();
                if (e_Song == Enum_Song.Two) this_MusicSource_Two.Play();
                if (e_Song == Enum_Song.Three) this_MusicSource_Three.Play();
            }

        }
        private get { return e_Song; }
    }

    void PlayMusic()
    {
        if(e_Song != Enum_Song.None)
        {
            if (RunningSong == Enum_Song.One) this_MusicSource_One.volume += Time.deltaTime;
            else this_MusicSource_One.volume -= Time.deltaTime;

            if (RunningSong == Enum_Song.Two) this_MusicSource_Two.volume += Time.deltaTime;
            else this_MusicSource_Two.volume -= Time.deltaTime;

            if (RunningSong == Enum_Song.Three) this_MusicSource_Three.volume += Time.deltaTime;
            else this_MusicSource_Three.volume -= Time.deltaTime;
        }
        else
        {
            this_MusicSource_One.volume -= Time.deltaTime / 5f;
            this_MusicSource_Two.volume -= Time.deltaTime / 5f;
            this_MusicSource_Three.volume -= Time.deltaTime / 5f;
        }

        if (this_MusicSource_One.volume > .75f) this_MusicSource_One.volume = .75f;
        else if (this_MusicSource_One.volume < 0f) this_MusicSource_One.volume = 0f;

        if (this_MusicSource_Two.volume > .75f) this_MusicSource_Two.volume = .75f;
        else if (this_MusicSource_Two.volume < 0f) this_MusicSource_Two.volume = 0f;

        if (this_MusicSource_Three.volume > .75f) this_MusicSource_Three.volume = .75f;
        else if (this_MusicSource_Three.volume < 0f) this_MusicSource_Three.volume = 0f;
    }

    // Update is called once per frame
    void Update ()
    {
        PlayMusic();
    }
}
