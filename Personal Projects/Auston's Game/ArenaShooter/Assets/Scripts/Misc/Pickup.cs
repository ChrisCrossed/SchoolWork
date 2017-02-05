using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public class Pickup : MonoBehaviour
{
    public float respawnTime = 10;
    public bool useGlobalRespawn = true;
    public int addCount = 1;

    private float timeSinceGot;
    private bool available { get { return meshRenderer.enabled; } set { meshRenderer.enabled = value; light.enabled = value; } }

    private MeshRenderer meshRenderer;
    private new Light light;

    public AudioClip audioClip;
    public AudioClip noGoAudio;
    public AudioMixerGroup audioMixerGroup;
    public AnimationCurve audioVolumeOverTime = AnimationCurve.EaseInOut(0, 1, 1, 1);

    // Use this for initialization
    void Start()
    {
        timeSinceGot = float.PositiveInfinity;
        meshRenderer = GetComponent<MeshRenderer>();
        light = GetComponentInChildren<Light>();

        if (useGlobalRespawn)
            respawnTime = GameManager.instance.rocketRespawnTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (available) return;

        if (timeSinceGot < respawnTime)
            timeSinceGot += Time.deltaTime;
        else
            available = true;
    }

    void OnTriggerEnter(Collider _collider)
    {
        if (!available || _collider.isTrigger) return;

        RocketAmmo _rocketAmmo = _collider.transform.root.GetComponent<RocketAmmo>();
        if (_rocketAmmo == null) return;

        int _iteration = 0;
        while (true)
        {
            if (_iteration == addCount)
            {
                _rocketAmmo.animateOnFail.Play();
                SoundManager.PlayOneShot(transform.position, noGoAudio, audioMixerGroup, audioVolumeOverTime);
                return;
            }
            if (_rocketAmmo.currentRocketCount + (addCount - _iteration) <= _rocketAmmo.maxRocketCount)
            {
                _rocketAmmo.AddRocket(addCount - _iteration);
                break;
            }
            ++_iteration;
        }

        timeSinceGot = 0;
        available = false;

        SoundManager.PlayOneShot(transform.position, audioClip, audioMixerGroup, audioVolumeOverTime);
    }
}
