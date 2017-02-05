//using UnityEngine;
//using System.Collections;

//public class Punching : MonoBehaviour
//{
//    private Energy energy;

//    public int punchStrength = 1;
//    public float lungeDistance = 3;
//    public bool limitVertical;
//    public float lungeSpeed = 1;
//    public AnimationCurve lungeSpeedCurve = AnimationCurve.EaseInOut(0, 1, 1, 1);
    
//    public float deflectSeekMultiplier = 2;
//    public float deflectSpeedMultiplier = 2;

//    [Range(0,2)]
//    public float cooldown = 0.35f;
//    public int cost = 1;

//    public Transform collisionCheckObject;
//    public Vector3 collisionCheckOffset;
//    public float collisionRadius = 0.5f;
//    public LayerMask collisionCheckLayers;

//    public AnimateGroup[] playOnPunch = new AnimateGroup[0];

//    private Movement movement;
//    private new Rigidbody rigidbody;
//    private Player player;
//    public Transform head = null;

//    private float lungeDuration;
//    public bool isLunging { get { return timeSincePunch <= lungeDuration; } }
//    private float lungeCompletionRatio { get { return Mathf.Clamp01(timeSincePunch / lungeDuration); } }

//    private float timeSincePunch = 0;
//    private int currentIndex;
    
//    public Vector2 completionRatioDamageRange;

//    // Use this for initialization
//    void Start ()
//    {
//        rigidbody = GetComponent<Rigidbody>();
//        movement = GetComponent<Movement>();
//        player = GetComponent<Player>();
//        energy = GetComponent<Energy>();

//        InputEvents.RightSpecial.Subscribe(OnPunch, player.index);
//        InputEvents.Restart.Subscribe(OnRestart);
//    }

//    void OnRestart(InputEventInfo _inputInfo)
//    {
//        StartCoroutine(GameManager.RestartAfterTime(0.3f));
//    }

//    void OnPunch(InputEventInfo _inputInfo)
//    {
//        if (timeSincePunch < cooldown) return; //cant punch again yet

//        if (!energy.SpendEnergy(cost))
//            return;

//        lungeDuration = lungeDistance / lungeSpeed;
//        movement.movementDisabled = true;
//        movement.gravityDisabled = true;
//        timeSincePunch = 0;

//        AnimateGroup _animateGroup = playOnPunch[currentIndex];
//        ++currentIndex;
//        if (currentIndex >= playOnPunch.Length)
//            currentIndex = 0;

//        float _newDuration = lungeDuration + (lungeDuration * (1 - completionRatioDamageRange.y));
//        _animateGroup.SetDuration(_newDuration);
//        _animateGroup.Play();
//    }
	
//	// Update is called once per frame
//	void FixedUpdate ()
//    {
//        bool _wasLunging = isLunging;
//        timeSincePunch += Time.fixedDeltaTime;

//        if(_wasLunging && !isLunging)
//        {
//            movement.movementDisabled = false;
//            movement.gravityDisabled = false;
//        }

//        if (!isLunging) return;

//        float _newSpeed = lungeSpeedCurve.Evaluate(lungeCompletionRatio) * lungeSpeed; //evaluating what the speed should be at this point in the lunge
//        //Vector3 _forwardNulledVelocity = Vector3.ProjectOnPlane(rigidbody.velocity, head.forward); //zeroing forward velocity (maintains left-right and up-down)
//        rigidbody.velocity = /*_forwardNulledVelocity + */(head.forward * _newSpeed); //setting new forward velocity

//        if (limitVertical && transform.InverseTransformDirection(rigidbody.velocity).y > 0)
//            rigidbody.velocity = Vector3.ProjectOnPlane(rigidbody.velocity, transform.up);

//        if (lungeCompletionRatio < completionRatioDamageRange.x || lungeCompletionRatio > completionRatioDamageRange.y)
//            return;

//        Vector3 _collisionPosition = collisionCheckObject.position + collisionCheckObject.TransformDirection(collisionCheckOffset);
//        Collider[] _hitObjects = Physics.OverlapSphere(_collisionPosition, collisionRadius, collisionCheckLayers.value, QueryTriggerInteraction.Ignore);
//        for (int i = 0; i < _hitObjects.Length; i++)
//        {
//            if (_hitObjects[i].transform.root == transform.root)
//                continue; //skip over child objects

//            timeSincePunch = lungeDuration;
//            //rigidbody.velocity = Vector3.zero;

//            _hitObjects[i].transform.root.GetComponent<Rigidbody>().velocity = transform.forward * punchStrength * 10;
//            Health _hitHealth = _hitObjects[i].GetComponent<Health>();
//            if (_hitHealth != null) _hitHealth.DealDamage(punchStrength, gameObject);

//            break;
//        }
//    }

//    void OnDrawGizmos()
//    {
//        Vector3 _collisionPosition = collisionCheckObject.position + collisionCheckObject.TransformDirection(collisionCheckOffset);
//        Gizmos.color = Color.green;
//        Gizmos.DrawWireSphere(_collisionPosition, collisionRadius);
//    }

//    void OnDestroy()
//    {
//        InputEvents.RightSpecial.Unsubscribe(OnPunch, player.index);
//        InputEvents.Restart.Unsubscribe(OnRestart);
//    }
//}
