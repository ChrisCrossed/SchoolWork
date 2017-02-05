using UnityEngine;
using System.Collections;
using UnityEngine.Audio;
using System;

public class Punching : MonoBehaviour
{
    private Energy energy;

    public int punchStrength = 1;
    public AnimationCurve lungeSpeedCurve = AnimationCurve.EaseInOut(0, 1, 1, 1);
    
    public float deflectSeekMultiplier = 2;
    public float deflectSpeedMultiplier = 2;

    [Range(0,2)]
    public float cooldown = 0.35f;
    public int cost = 1;

    public Transform collisionCheckObject;
    public Vector3 playerCheckOffset = new Vector3(0,0,2.5f);
    public float playerCheckRadius = 2f;
    public Vector3 projectileCheckOffset = new Vector3(0,0,1.5f);
    public float projectileCheckRadius = 0.75f;
    public LayerMask collisionCheckLayers;

    public AnimateGroup[] playOnPunch = new AnimateGroup[0];

    private Movement movement;
    private new Rigidbody rigidbody;
    public Player player;
    public Transform head = null;

    public float lungeDuration = 0.5f;
    public bool isLunging { get { return timeSincePunch <= lungeDuration; } }
    private float lungeCompletionRatio { get { return Mathf.Clamp01(timeSincePunch / lungeDuration); } }

    public float timeSincePunch = 0;
    private int currentIndex;
    
    public Vector2 completionRatioDamageRange;
    public Transform target;

    public AudioClip audioClip;
    public AudioMixerGroup audioMixerGroup;
    public AnimationCurve audioVolumeOverTime = AnimationCurve.EaseInOut(0, 1, 1, 1);

    // Use this for initialization
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        movement = GetComponent<Movement>();
        player = GetComponent<Player>();
        energy = GetComponent<Energy>();

        InputEvents.RightSpecial.Subscribe(OnPunch, player.index);
    }

    void OnPunch(InputEventInfo _inputInfo)
    {
        if (!GameManager.instance.allowGameInput) return;

        if (timeSincePunch < cooldown) return; //cant punch again yet

        if (!energy.SpendEnergy(cost))
            return;

        SoundManager.PlayOneShot(transform.position, audioClip, audioMixerGroup, audioVolumeOverTime);
        
        StartCoroutine(ShakeFeedbackAfterTime(0.065f));
        timeSincePunch = 0;

        Collider[] _colliders = Physics.OverlapSphere(collisionCheckObject.TransformPoint(playerCheckOffset), playerCheckRadius, collisionCheckLayers, QueryTriggerInteraction.Collide);

        target = null;

        for (int i = 0; i < _colliders.Length; i++)
        {
            if (_colliders[i].transform.root == transform.root)
                continue;

            Rigidbody _hitRigidbody = _colliders[i].transform.root.GetComponent<Rigidbody>();
            if (_hitRigidbody != null)
            {
                StartCoroutine(KnockbackAfterTime(0.135f, _hitRigidbody));
            }

            AffiliatedObject _affiliatedObject = _colliders[i].transform.root.GetComponent<AffiliatedObject>();
            if (_affiliatedObject == null || _affiliatedObject.team == player.team)
                continue;

            Health _hitHealth = _colliders[i].GetComponent<Health>();
            if (_hitHealth == null)
                continue;

            target = _colliders[i].transform.root;

            StartCoroutine(DamageTargetAfterTime(0.135f, _hitHealth));
        }

        if(target != null)
        {

            if (transform.InverseTransformPoint(target.position).x > 0)
            {
                currentIndex = 1;
            }
            else
                currentIndex = 0;
        }

        PlayAnimation();

        if (target == null) return;
        
        movement.movementDisabled = true;
        movement.movementInput = Vector2.zero;
        rigidbody.velocity = Vector3.zero;
    }

    private IEnumerator KnockbackAfterTime(float _time, Rigidbody _hitRigidbody)
    {
        yield return new WaitForSeconds(_time);

        int _currentIndex = currentIndex + 1;
        if (_currentIndex > 1) _currentIndex -= 2;

        Vector3 _vecFromHand = (_hitRigidbody.transform.position + _hitRigidbody.centerOfMass) - playOnPunch[_currentIndex].transform.position;
        float _distanceFromHand = _vecFromHand.magnitude;
        _vecFromHand /= _distanceFromHand;

        if (_distanceFromHand < 2f)
            _hitRigidbody.velocity += playOnPunch[_currentIndex].transform.forward * 30;
    }

    public void PlayAnimation()
    {
        AnimateGroup _animateGroup = playOnPunch[currentIndex];
        ++currentIndex;
        if (currentIndex >= playOnPunch.Length)
            currentIndex = 0;

        _animateGroup.SetDuration(0.5f);
        _animateGroup.Play();
    }

    public IEnumerator ShakeFeedbackAfterTime(float _time)
    {
        yield return new WaitForSeconds(_time);

        CameraShake.Shake(player.index, 0.1f, 0.65f, 25, 2);
        CameraShake.AberrateCamera(player.index, 0.2f, 4, 30);

        if (currentIndex == 1) CameraShake.VibrateGamepad(player.index, 0.15f, 0, 0.65f);
        else CameraShake.VibrateGamepad(player.index, 0.15f, 0.65f, 0);
    }

    public IEnumerator DamageTargetAfterTime(float _time, Health _targetHealth)
    {
        yield return new WaitForSeconds(_time);

        Vector3 _newVelocity = (transform.position - target.position).normalized * 70;

        _targetHealth.GetComponent<Rigidbody>().velocity += -_newVelocity * 700;
        _targetHealth.DealDamage(punchStrength, gameObject, player.team, transform.position);

        CameraShake.Shake(player.index, 0.2f, 0.55f, 40, 4);
        CameraShake.VibrateGamepad(player.index, 0.25f, 0.45f);
        CameraShake.AberrateCamera(player.index, 0.35f, 3, 30);

        yield return new WaitForSeconds(0.085f);

        rigidbody.velocity = _newVelocity;
        rigidbody.drag = 18;

        yield return new WaitForSeconds(0.085f);
        rigidbody.drag = 0;
    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        if (lungeCompletionRatio > completionRatioDamageRange.x && lungeCompletionRatio < completionRatioDamageRange.y)
        {
            Collider[] _colliders = Physics.OverlapSphere(collisionCheckObject.TransformPoint(projectileCheckOffset), projectileCheckRadius, 1 << 9, QueryTriggerInteraction.Collide);

            for (int i = 0; i < _colliders.Length; i++)
            {
                if (_colliders[i].transform.root == transform.root)
                    continue;

                Projectile _projectile = _colliders[i].GetComponent<Projectile>();
                if (_projectile != null)
                    _projectile.Deflect(this);

                //target = _projectile.transform;
            }
        }
        //else
        //{
        //    Collider[] _colliders = Physics.OverlapSphere(collisionCheckObject.TransformPoint(projectileCheckOffset), projectileCheckRadius, 1 << 9, QueryTriggerInteraction.Collide);

        //    for (int i = 0; i < _colliders.Length; i++)
        //    {
        //        if (_colliders[i].transform.root == transform.root)
        //            continue;

        //        Projectile _projectile = _colliders[i].GetComponent<Projectile>();
        //        if (_projectile == null)
        //            continue;
                
        //        _projectile.

        //        //target = _projectile.transform;
        //    }
        //}

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        bool _wasLunging = isLunging;
        timeSincePunch += Time.fixedDeltaTime;

        if(_wasLunging && !isLunging) //on finish lunge
        {
            movement.movementDisabled = false;
            movement.gravityDisabled = false;
        }

        if (!isLunging || target == null) return;

        //if (timeSincePunch > 0.65f) return;

        if (target.gameObject.layer != 9)
            rigidbody.MovePosition(Vector3.Lerp(transform.position, target.position + (transform.position - target.position).normalized * 1.9f, Time.fixedDeltaTime * 25));
        //if ((transform.position - target.position).magnitude < )

        Vector3 _targetWorldCenter = target.GetComponent<Rigidbody>().worldCenterOfMass + target.up * 0.5f;

        //if (Vector3.Angle(head.forward, _targetWorldCenter - transform.position) < 37)
        //    return;
        Vector3 _desiredForward = _targetWorldCenter - head.position;

        float _angle = Vector3.Angle(head.forward, _desiredForward);
        float _maxRadians = 2.5f * Mathf.PI * Time.fixedDeltaTime;
        if(target.gameObject.layer == 9)
            _maxRadians = 0.5f * Mathf.PI * Time.fixedDeltaTime;
        if (_maxRadians * Mathf.Rad2Deg > _angle) _maxRadians = _angle;

        Vector3 _newForward = Vector3.RotateTowards(head.forward, _desiredForward, _maxRadians, 0);

        transform.LookAt(transform.position + Vector3.ProjectOnPlane(_newForward, transform.up), transform.up);
        head.LookAt(head.position + _newForward, transform.up);

        //float _newSpeed = lungeSpeedCurve.Evaluate(lungeCompletionRatio) * lungeSpeed; //evaluating what the speed should be at this point in the lunge
        ////Vector3 _forwardNulledVelocity = Vector3.ProjectOnPlane(rigidbody.velocity, head.forward); //zeroing forward velocity (maintains left-right and up-down)
        //rigidbody.velocity = /*_forwardNulledVelocity + */(head.forward * _newSpeed); //setting new forward velocity

        //if (limitVertical && transform.InverseTransformDirection(rigidbody.velocity).y > 0)
        //    rigidbody.velocity = Vector3.ProjectOnPlane(rigidbody.velocity, transform.up);

        //if (lungeCompletionRatio < completionRatioDamageRange.x || lungeCompletionRatio > completionRatioDamageRange.y)
        //    return;

        //Vector3 _collisionPosition = collisionCheckObject.position + collisionCheckObject.TransformDirection(collisionCheckOffset);
        //Collider[] _hitObjects = Physics.OverlapSphere(_collisionPosition, collisionRadius, collisionCheckLayers.value, QueryTriggerInteraction.Ignore);
        //for (int i = 0; i < _hitObjects.Length; i++)
        //{
        //    if (_hitObjects[i].transform.root == transform.root)
        //        continue; //skip over child objects

        //    timeSincePunch = lungeDuration;
        //    //rigidbody.velocity = Vector3.zero;

        //    _hitObjects[i].transform.root.GetComponent<Rigidbody>().velocity = transform.forward * punchStrength * 10;
        //    Health _hitHealth = _hitObjects[i].GetComponent<Health>();
        //    if (_hitHealth != null) _hitHealth.DealDamage(punchStrength, gameObject);

        //    break;
        //}
    }

    void OnDrawGizmos()
    {
        Vector3 _collisionPosition = collisionCheckObject.TransformPoint(playerCheckOffset);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(_collisionPosition, playerCheckRadius);

        _collisionPosition = collisionCheckObject.TransformPoint(projectileCheckOffset);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(_collisionPosition, projectileCheckRadius);
    }

    void OnDestroy()
    {
        if(player != null)
            InputEvents.RightSpecial.Unsubscribe(OnPunch, player.index);
    }
}
