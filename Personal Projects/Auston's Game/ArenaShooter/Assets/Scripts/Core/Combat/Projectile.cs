using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    private AffiliatedObject affiliatedObject = null;

    public float lifetime = 3;
    public float timeSinceCreated;
    public GameObject createOnDestroy;

    public float seekingStrength = 1;

    public float explosionRadius { get { return transform.lossyScale.x * transform.lossyScale.x; } }
    public int damage = 1;
    public AnimationCurve damageFalloff = AnimationCurve.Linear(0, 1, 1, 0);

    [System.NonSerialized]
    public GameObject creator = null;

    [System.NonSerialized]
    public Targeting targeting = null;
    new private Rigidbody rigidbody = null;

    public bool useCreatorsTargeting = false;
    private bool isDead = false;

    public bool limitSpeed;
    private float maxSpeed = 0;
    public float maxDistance = 50;
    public AnimationCurve speedCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public AnimationCurve lookAngleCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

    public Vector3 createdAtPosition = Vector3.zero;

    public ParticleSystem trailParticles;

    private Vector3 currentOffset;
    private float currentOffsetMagnitude;
    public Vector2 offsetTimeRange = new Vector2(0.1f, 0.45f);
    public Vector2 offsetMagnitudeRange = new Vector2(1f, 5f);
    private float offsetTimer;
    private float offsetDuration;
    private float offsetMagnitude;

    public AnimationCurve kockbackCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);
    public float maxKnockbackRange = 30;
    public float maxKnockbackIntensity = 30;

    // Use this for initialization
    void Start()
    {
        targeting = GetComponent<Targeting>();
        rigidbody = GetComponent<Rigidbody>();
        affiliatedObject = GetComponent<AffiliatedObject>();

        if (useCreatorsTargeting)
        {
            Destroy(GetComponent<Detection>());
            Destroy(targeting);
            targeting = creator.GetComponent<Targeting>();
        }

        createdAtPosition = transform.position;
        maxSpeed = rigidbody.velocity.magnitude;
    }

    // Update is called once per frame
    void Update()
    {
        offsetTimer += Time.deltaTime;
        if (offsetTimer >= offsetDuration)
        {
            currentOffset = Vector3.ProjectOnPlane(Random.onUnitSphere, transform.forward).normalized;

            offsetTimer = 0;
            offsetDuration = Random.Range(offsetTimeRange.x, offsetTimeRange.y);
            offsetMagnitude = Random.Range(offsetMagnitudeRange.x, offsetMagnitudeRange.y);
            //currentOffsetMagnitude = offsetMagnitude;
        }

        if (offsetDuration != 0)
            currentOffsetMagnitude = offsetMagnitude * (offsetTimer / offsetDuration);
        if (targeting == null || targeting.currentTarget == null || timeSinceCreated < 0.05f)
            timeSinceCreated += Time.deltaTime;
        else
            timeSinceCreated += Time.deltaTime;

        if (timeSinceCreated > lifetime)
            DestroySelf(Vector3.up);
    }

    void FixedUpdate()
    {
        if (targeting == null || targeting.currentTarget == null || timeSinceCreated < 0.05f)
        {
            if (limitSpeed)
                LimitSpeed();
            return;
        }

        //if targeting is null, use creator's current target (set up in start currently)


        Transform _transform = transform;

        CapsuleCollider _targetCapsuleCollider = targeting.currentTarget.GetComponent<CapsuleCollider>();
        Vector3 _targetCenter = Creation.GetCapsuleColliderCenter(_targetCapsuleCollider);

        Vector3 _vecToTarget = _targetCenter - transform.position;

        //Don't home if the target it dashing
        Dashing _targetDashing = targeting.currentTarget.GetComponent<Dashing>();
        if (_targetDashing != null && _targetDashing.avoidSeekWhileDashing && _targetDashing.isDashing)
        {
            if (limitSpeed)
                LimitSpeed(_vecToTarget);
            return;
        }

        //HOMING

        Vector3 _steeringVector = Vector3.ProjectOnPlane(_vecToTarget, _transform.forward);
        float _steerVecMagnitude = _steeringVector.magnitude;

        if (Physics.Raycast(transform.position, _vecToTarget, _vecToTarget.magnitude, 1 << 11, QueryTriggerInteraction.Ignore))
        {
            if (limitSpeed) LimitSpeed(_vecToTarget);
            return;
        }

        if (_steerVecMagnitude > 1)
            _steeringVector /= _steerVecMagnitude;

        rigidbody.velocity += _steeringVector * GetEvaluationTotal(_vecToTarget) * seekingStrength * Time.fixedDeltaTime;
        _transform.LookAt(_transform.position + rigidbody.velocity, transform.up);

        if (limitSpeed)
            LimitSpeed(_vecToTarget);
    }

    void LimitSpeed(Vector3 _vecToTarget = default(Vector3))
    {
        //Debug.Log(GetEvaluationTotal(_vecToTarget));

        if (timeSinceCreated > 0.05f)
            rigidbody.velocity += currentOffset * GetEvaluationTotal(_vecToTarget) * currentOffsetMagnitude * Time.fixedDeltaTime; //velocity offset (more organic)

        if (_vecToTarget == Vector3.zero)
            rigidbody.velocity = rigidbody.velocity.normalized * maxSpeed;
        else
            rigidbody.velocity = rigidbody.velocity.normalized * GetEvaluationTotal(_vecToTarget) * maxSpeed;
    }

    float GetEvaluationTotal(Vector3 _vecToTarget)
    {
        if (targeting.currentTarget == null)
            return 1;

        float _targetLookAngle = Vector3.Angle(transform.position - targeting.currentTarget.position, targeting.currentTarget.forward);
        float _ownLookAngle = Vector3.Angle((targeting.currentTarget.position + targeting.currentTarget.up) - transform.position, transform.forward);
        
        float _targetLookRatio = lookAngleCurve.Evaluate(_targetLookAngle / 180);
        float _ownLookRatio = 1 - Mathf.Clamp01(lookAngleCurve.Evaluate(_ownLookAngle / 90));

        //return Mathf.Lerp(speedCurve.Evaluate(_vecToTarget.magnitude / maxDistance), 1, (_targetLookAngle + _ownLookAngle) / 2);
        //return Mathf.Lerp(speedCurve.Evaluate(_vecToTarget.magnitude / maxDistance), 1, _targetLookAngle);
        //return Mathf.Lerp(speedCurve.Evaluate(_vecToTarget.magnitude / maxDistance), 1, _ownLookAngle);
        //Debug.Log(_vecToTarget.magnitude);
        return speedCurve.Evaluate(_vecToTarget.magnitude / maxDistance);
    }

    void OnCollisionEnter(Collision _collision)//Collider _colliderHit)
    {
        Collider _colliderHit = _collision.collider;

        //if (CheckGetDeflected(_colliderHit)) return;

        bool _isRelatedToHitObject = false;
        if (creator != null)
            _isRelatedToHitObject = _colliderHit.transform.root == creator.transform.root;

        bool _isTrigger = _colliderHit.isTrigger;

        if (_colliderHit.GetComponent<Projectile>() != null)
            _isTrigger = false; //should always collide with other projectiles

        if (_isTrigger || _isRelatedToHitObject || isDead)
            return;

        isDead = true;

        Rigidbody _rigidbody = GetComponent<Rigidbody>();
        DestroySelf(_collision.contacts[0].normal);

        Vector3 _explostionPosition = transform.position;
        Collider[] _colliders = Physics.OverlapSphere(_explostionPosition, explosionRadius);

        //deal damage to everything in the explosion
        foreach (Collider _colliderInExplosion in _colliders)
        {
            if (_colliderInExplosion.transform.root == transform.root)
                continue;

            Rigidbody _otherRigidbody = _colliderInExplosion.GetComponent<Rigidbody>();
            if (_otherRigidbody == null)
                continue;

            Vector3 _explosionToOther = _otherRigidbody.worldCenterOfMass - _explostionPosition;
            float _distanceToOther = _explosionToOther.magnitude;
            _explosionToOther /= _distanceToOther;

            Debug.DrawLine(_explostionPosition, _explostionPosition + _explosionToOther * _distanceToOther, Color.cyan);
            Debug.DrawLine(_explostionPosition, _explostionPosition + Vector3.up, Color.red);
            if (Physics.Raycast(_otherRigidbody.worldCenterOfMass, -_explosionToOther, _distanceToOther, 1 << 11, QueryTriggerInteraction.Ignore))
                continue;

            if (!_otherRigidbody.GetComponent<Player>())
                _otherRigidbody.velocity = _explosionToOther * damage * 40;

            Health _health = _colliderInExplosion.GetComponent<Health>();
            if (_health == null)
                continue;

            float _distanceRatio = 1 - _distanceToOther;
            _distanceRatio = Mathf.Clamp01(_distanceRatio);
            int _damage = (int)(damage * damageFalloff.Evaluate(_distanceRatio));

            if (_health.GetComponent<AffiliatedObject>().team != affiliatedObject.team)
                _health.DealDamage(damage, creator, affiliatedObject.team, createdAtPosition);
        }

        //Rigidbody _otherRootRigidbody = _colliderHit.transform.root.GetComponent<Rigidbody>();

        //if (_otherRootRigidbody != null)
        //    _otherRootRigidbody.AddForce(_rigidbody.velocity.normalized * 100);
    }

    private bool CheckGetDeflected(Collider _colliderHit)
    {
        Punching _punching = _colliderHit.transform.root.GetComponent<Punching>();
        if (_punching != null && _punching.isLunging)
        {
            if (Vector3.Angle(_punching.head.forward, -rigidbody.velocity) < 45)
            {
                Deflect(_punching);
                return true;
            }
        }
        return false;
    }

    public void Deflect(Punching _punching)
    {
        _punching.target = transform;
        _punching.lungeDuration = _punching.timeSincePunch + 0.1f;

        CameraShake.Shake(_punching.player.index, 0.15f, 0.65f, 40, 3);
        CameraShake.VibrateGamepad(_punching.player.index, 0.45f, 0.15f);
        CameraShake.AberrateCamera(_punching.player.index, 0.4f, 4, 50);

        rigidbody.velocity = _punching.head.forward;
        transform.LookAt(transform.position + rigidbody.velocity, transform.up);

        createdAtPosition = transform.position;
        timeSinceCreated = 0;
        creator = _punching.transform.root.gameObject;
        affiliatedObject.team = creator.GetComponent<AffiliatedObject>().team;
        creator.GetComponent<ManualShooting>().SetUpProjectile(gameObject);

        if (useCreatorsTargeting)
        {
            Destroy(GetComponent<Detection>());
            Destroy(targeting);
            targeting = creator.GetComponent<Targeting>();
        }

        maxSpeed *= _punching.deflectSpeedMultiplier;
        seekingStrength *= _punching.deflectSeekMultiplier;
    }

    void DestroySelf(Vector3 _normal)
    {
        //Material _material = GetComponent<MeshRenderer>().material;
        //ParticleSystem[] _particleSystems = createOnDestroy.GetComponentsInChildren<ParticleSystem>();
        //foreach (ParticleSystem _particleSystem in _particleSystems)
        //    _particleSystem.startColor = _material.GetColor("_EmissionColor") - new Color(0.3f, 0.3f, 0.3f, 0);

        HandleParticles();

        GameObject _createdObject = (GameObject)Instantiate(createOnDestroy, transform.position + _normal * 0.75f, transform.rotation);
        _createdObject.transform.localScale = Vector3.one * explosionRadius * 0.75f;
        _createdObject.GetComponent<RecolorParticlesToTeamColor>().teamIndexOverride = affiliatedObject.team;
        _createdObject.GetComponent<RecolorLightToTeamColor>().teamIndexOverride = affiliatedObject.team;

        //_createdObject.transform.LookAt(transform.position + _forward);
        //_createdObject.transform.localScale = transform.localScale;

        for (int i = 0; i < 4; i++)
        {
            if (CameraShake.Get(i) == null) continue;

            float _distance = (CameraShake.Get(i).transform.position - transform.position).magnitude;

            float _intensity = Mathf.Clamp01(1 - (_distance / 80));

            CameraShake.ShakeWithVibration(i, 0.25f, _intensity / 2, 60, _intensity * 3);
            CameraShake.AberrateCamera(i, 0.45f, _intensity * 10, 30);
        }

        for (int i = 0; i < 4; i++)
        {
            if (PlayerManager.players[i] == null)
                continue;

            Rigidbody _rigidbody = PlayerManager.players[i].GetComponent<Rigidbody>();
            Vector3 _vecToPlayer = (_rigidbody.transform.position + Vector3.up) - (transform.position + _normal);
            float _distanceToPlayer = _vecToPlayer.magnitude;
            float _distanceRatio = Mathf.Clamp01(_distanceToPlayer / maxKnockbackRange);

            if (Physics.Raycast(transform.position + Vector3.up, _vecToPlayer, _distanceToPlayer, 1 << 11, QueryTriggerInteraction.Ignore))
                continue;

            _rigidbody.velocity += Vector3.up * kockbackCurve.Evaluate(_distanceRatio) * maxKnockbackIntensity;

            PlayerManager.players[i].StartCoroutine(KnockbackPlayerAfterTime(_rigidbody, _vecToPlayer, _distanceToPlayer, _distanceRatio, _normal));
        }

        //Destroy(gameObject);
        GetComponent<Health>().DealDamage(int.MaxValue, null, -1);
    }

    private IEnumerator KnockbackPlayerAfterTime(Rigidbody _rigidbody, Vector3 _vecToPlayer, float _distanceToPlayer, float _distanceRatio, Vector3 _normal)
    {
        yield return new WaitForSeconds(_distanceRatio / 1.75f);

        //_vecToPlayer = (_rigidbody.transform.position + Vector3.up) - (transform.position + _normal);
        //_distanceToPlayer = _vecToPlayer.magnitude;
        //_distanceRatio = Mathf.Clamp01(_distanceToPlayer / maxKnockbackRange);

        _rigidbody.velocity += (_vecToPlayer / _distanceToPlayer) * kockbackCurve.Evaluate(_distanceRatio) * maxKnockbackIntensity;
    }

    void HandleParticles()
    {
        if (trailParticles == null)
            trailParticles = GetComponentInChildren<ParticleSystem>();

        trailParticles.transform.parent = null;

        ParticleSystem.EmissionModule _emission = trailParticles.emission;
        ParticleSystem.MinMaxCurve _rate = _emission.rate;
        _rate.constant = 0;
        _emission.rate = _rate;
    }
}