using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public class Health : MonoBehaviour
{

    private Player player = null;
    private SeparateOnKilled separateOnKilled = null;

    public float respawnTime = 1;
    public int maxHealth = 1;
    private int currentHealth_;
    public int currentHealth
    {
        get { return currentHealth_; }
        private set
        {
            if (value == currentHealth_) //nothing changed
                return;

            //if (value < currentHealth_) //if the new value is less than the current health
            //do damage animation
            //else //if the new value is greater than the current health
            //do heal animation

            currentHealth_ = value;
        }
    }

    public bool killIfBelowThreshold = true;

    public AudioClip audioClip;
    public AudioMixerGroup audioMixerGroup;
    public AnimationCurve audioVolumeOverTime = AnimationCurve.EaseInOut(0, 1, 1, 1);

    public void DealDamage(int _damage, GameObject _source, int _teamIndex, Vector3 _worldPosition = default(Vector3))
    {
        currentHealth -= _damage;

        if (currentHealth <= 0)
            Kill(_source, -1, _worldPosition);
    }

    // Use this for initialization
    void Start()
    {
        player = GetComponent<Player>();

        separateOnKilled = GetComponent<SeparateOnKilled>();

        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < GameManager.instance.killThreshold && killIfBelowThreshold)
            DealDamage(int.MaxValue, null, -1);
    }

    void Kill(GameObject _killedBy, int _teamKilledBy, Vector3 _damagePosition)
    {
        if (separateOnKilled != null)
            separateOnKilled.Separate(_killedBy);

        if (transform == null)
            return;

        Destroy(transform.root.gameObject);

        SoundManager.PlayOneShot(transform.position, audioClip, audioMixerGroup, audioVolumeOverTime);

        FollowCamera _followCam = GetComponentInChildren<FollowCamera>();
        if (player == null || _followCam == null)
            return;

        Events.PlayerDied.Send(GetComponent<Player>(), _killedBy);

        _followCam.transform.parent = null;
        _followCam.enabled = true;
        AutomaticAiming _autoAiming = _followCam.GetComponent<AutomaticAiming>();
        _autoAiming.enabled = true;

        if (_killedBy == null)
            _autoAiming.targetOverride = separateOnKilled.transformsToSeparate[0];
        else
            _autoAiming.targetOverride = _killedBy.transform;

        DestroyAfterTime _destroyAfterTime = _followCam.gameObject.AddComponent<DestroyAfterTime>();
        _destroyAfterTime.lifetime = respawnTime;

        PlayerManager.SpawnPlayer(player.index, respawnTime);
        --PlayerManager.playersInWorld;
    }
}
