using UnityEngine;
using System.Collections.Generic;

public class SoundListener : MonoBehaviour
{
    [System.NonSerialized]
    new public Transform transform;

    public float listenRange = 1;
    public AnimationCurve listenCurve = AnimationCurve.Linear(0,1,1,1); //by default, falloff is not affected

    [System.NonSerialized]
    public List<AudioSource> audioSources = new List<AudioSource>();

    public float volumeOnSpeed = 3;
    private float volume = 0;

    // Use this for initialization
    void Start ()
    {
        transform = base.transform;
        SoundManager.AddSoundListener(this);

        for (int i = 0; i < audioSources.Count; i++)
        {
            audioSources[i].volume = 0;
        }
    }

    public void Update()
    {
        //volume = Mathf.Lerp(volume, 1, Time.deltaTime * volumeOnSpeed);

        for (int j = 0; j < SoundManager.soundSources.Count; j++)
        {
            SoundSource _soundSource = SoundManager.soundSources[j];

            Transform _sourceTransform = _soundSource.transform;
            Vector3 _vecToSource = _sourceTransform.position - transform.position;

            _vecToSource = transform.InverseTransformVector(_vecToSource);

            audioSources[j].transform.position = SoundManager.transform.position + _vecToSource;
            audioSources[j].volume = volume;
        }
    }
	
	// Update is called once per frame
	void OnDestroy ()
    {
        SoundManager.RemoveSoundListener(this);
    }
}
