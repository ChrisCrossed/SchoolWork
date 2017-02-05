using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//public class RatioFloat
//{
//    public float minimum = 0;
//    public float maximum = 1;
//    public float current = 1;
//    public float speed = 1;
//    public AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);

//    public float ratio { get { return (current - minimum) / (maximum - minimum); } }
//    public float evaluatedRatio { get { return curve.Evaluate(ratio); } }

//    public RatioFloat() { }
//    public RatioFloat(float _start, float _end, float _speed)
//    {
//        minimum = _start;
//        maximum = _end;
//        speed = _speed;
//        current = _start;
//    }

//    public float Update(float _deltaTime)
//    {
//        current += _deltaTime * speed;
//        return ratio;
//    }
//}

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Player))]
public class Movement : MonoBehaviour
{
    private new Rigidbody rigidbody = null;
    private Player player = null;
    private Energy energy = null;
    private LockOn lockOn = null;

    [Range(0, 20)]
    public float moveSpeed = 5;
    [System.NonSerialized]
    public Modifier speedModifier = new Modifier();
    [System.NonSerialized]
    public Modifier gravityModifier = new Modifier();
    [System.NonSerialized]
    public Modifier accelerationModifier = new Modifier();
    [System.NonSerialized]
    public Modifier deccelerationModifier = new Modifier();
    [System.NonSerialized]
    public Modifier totalMovementModifier = new Modifier();
    private float previousMovementModifier = 1;

    [Tooltip("Modifier for speed based on input direction where 0 is forward and 1 is backward.")]
    public AnimationCurve directionalModifier = AnimationCurve.Linear(0, 1, 1, 1);

    public float acceleration = 12;
    public float decceleration = 6;

    public float airAcceleration = 5;
    public float airDecceleration = 5;

    [System.NonSerialized]
    public Vector3 movementInput;

    [Range(0, 30)]
    public float jumpPower = 10;
    [Range(0, 1)]
    public float jumpCancelRatio = 0.2f;

    private float timeSinceLastJump;
    public int airJumpCost = 1;

    [Range(0, 50)]
    public float gravity = 10;
    [Range(0, 1)]
    public float groundGravityRatio = 0.1f;

    public float groundCastRadius;
    public float groundCheckDistance;

    private int groundContacts;
    public bool isGrounded { get { return groundContacts > 0; } }
    private Vector3 averageGroundNormal;

    public float maxSlopeAngle = 45;

    private float desiredHeight = 1;
    private float desiredHeightOffset = 0;
    private float heightOffset = 0;
    public float heightChangeSpeed = 1;

    [System.NonSerialized]
    public new Transform camera;
    public Transform boom;

    private GameObject groundObject;
    private Vector3 groundObjectPosLastFrame;
    private float stepTimer;
    private Vector3 velocityLastFrame;

    public SoundSource airMovementSound;
    public SoundSource groundMovementSound;
    public SoundSource jumpSound;
    public SoundSource landingSound;

    [System.NonSerialized]
    public bool movementDisabled = false;
    [System.NonSerialized]
    public bool gravityDisabled = false;

    private bool moveInputEnabled_ = false;
    private bool jumpInputEnabled_ = false;

    public bool moveInputEnabled
    {
        get { return moveInputEnabled_; }
        set
        {
            if (value == moveInputEnabled_)
                return;

            if (value)
                InputEvents.Movement.Subscribe(OnMove, player.index);
            else
                InputEvents.Movement.Unsubscribe(OnMove, player.index);

            moveInputEnabled_ = value;
        }
    }
    public bool jumpInputEnabled
    {
        get { return jumpInputEnabled_; }
        set
        {
            if (value == jumpInputEnabled_)
                return;

            if (value)
                InputEvents.Jump.Subscribe(OnJump, player.index);
            else
                InputEvents.Jump.Unsubscribe(OnJump, player.index);

            jumpInputEnabled_ = value;
        }
    }

    public AnimationCurve stepCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    public void DisableInput(bool _moveInput = true, bool _jumpInput = true)
    {
        if (_moveInput) moveInputEnabled = false;
        if (_jumpInput) jumpInputEnabled = false;
    }
    public void EnableInput(bool _moveInput = true, bool _jumpInput = true)
    {
        if (_moveInput) moveInputEnabled = true;
        if (_jumpInput) jumpInputEnabled = true;
    }

    // Use this for initialization
    void Start()
    {
        player = GetComponent<Player>();
        rigidbody = GetComponent<Rigidbody>();
        energy = GetComponent<Energy>();
        lockOn = GetComponent<LockOn>();

    camera = boom.GetComponentInChildren<Camera>().transform;

        if (camera == null)
            Debug.LogError("Camera is null. The boom transform must have a child camera.");

        if (groundMovementSound != null)
            groundMovementSound.volumeOffsetRatio = 0;
        if (airMovementSound)
            airMovementSound.volumeOffsetRatio = 0;

        EnableInput();
    }

    void OnMove(InputEventInfo _inputEventInfo)
    {
        if (!GameManager.instance.allowGameInput)
        {
            movementInput = Vector3.zero;
            return;
        }
        movementInput = new Vector3(_inputEventInfo.dualAxisValue.x, 0, _inputEventInfo.dualAxisValue.y);
    }
    void OnJump(InputEventInfo _inputEventInfo)
    {
        if (!GameManager.instance.allowGameInput /*|| lockOn.inLockOn*/) return;

        if (_inputEventInfo.inputState == InputState.Released)
        {
            if (rigidbody.velocity.y > 0)
                rigidbody.velocity = new Vector3(rigidbody.velocity.x, rigidbody.velocity.y * (1 - jumpCancelRatio), rigidbody.velocity.z); //Cancel Jump
            return;
        }

        lockOn.ExitLockOn(false);

        if (timeSinceLastJump > 0.15f && (isGrounded || energy.SpendEnergy(airJumpCost)))
        {
            if (jumpSound != null)
                jumpSound.Play();

            CameraShake.Shake(player.index, 0.1f, 0.55f, 40, 1);
            CameraShake.VibrateGamepad(player.index, 0.2f, 0.25f);
            CameraShake.AberrateCamera(player.index, 0.3f, 5, 30);

            timeSinceLastJump = 0;
            rigidbody.velocity = new Vector3(rigidbody.velocity.x, jumpPower, rigidbody.velocity.z); //Jump
        }
    }

    public void DisableForTime(float _seconds, bool _disableGravity = false)
    {
        movementDisabled = true;
        if (_disableGravity) gravityDisabled = true;
        StartCoroutine(EnableAfterTime(_seconds, _disableGravity));
    }
    private IEnumerator EnableAfterTime(float _seconds, bool _enableGravity = false)
    {
        yield return new WaitForSeconds(_seconds);
        movementDisabled = false;
        if (_enableGravity) gravityDisabled = false;
    }

    void CheckGroundContact()
    {
        bool _wasOnGround = isGrounded;

        groundContacts = 0;
        averageGroundNormal = Vector3.zero;

        CastRayWithOffset(0, 0.5f, 1, groundCheckDistance); //forward
        CastRayWithOffset(0, 0.5f, -1, groundCheckDistance); //back
        CastRayWithOffset(-1, 0.5f, 0, groundCheckDistance); //left
        CastRayWithOffset(1, 0.5f, 0, groundCheckDistance); //right

        //print("Total Ground Normal: " + AverageGroundNormal);
        //print("Ground Contacts: " + GroundContacts);

        averageGroundNormal /= groundContacts;
        //print("Average Ground Normal: " + AverageGroundNormal);

        if (groundContacts == 0)
        {
            averageGroundNormal = Vector3.up;
            groundObject = null;

            //if(_wasOnGround)
            //{
            //    //now NOT on ground
            //}
        }
    }
    //Takes -1, 0, or 1 for each axis value (multiplier)
    void CastRayWithOffset(float xAxisValue, float yAxisValue, float zAxisValue, float distance)
    {
        Vector3 castStart = Vector3.zero;
        castStart.x = groundCastRadius * xAxisValue;
        castStart.y = groundCastRadius * yAxisValue;
        castStart.z = groundCastRadius * zAxisValue;
        castStart = transform.TransformPoint(castStart);
        Debug.DrawLine(castStart, castStart - (Vector3.up * distance), Color.white, 0, false);

        foreach (RaycastHit hitInfo in Physics.RaycastAll(castStart, -Vector3.up, distance))
        {
            if (hitInfo.collider.transform.root == transform)
                continue;

            if (hitInfo.collider != null)
                groundObject = hitInfo.collider.transform.root.gameObject;

            //print(Vector3.Angle(hitInfo.normal, Vector3.up));
            if (Vector3.Angle(hitInfo.normal, Vector3.up) < maxSlopeAngle)
            {
                ++groundContacts;
                averageGroundNormal += hitInfo.normal;
                //print("Ground Normal: " + hitInfo.normal);
                break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        CalculateSpeedChange();

        if (timeSinceLastJump < float.MaxValue - 100)
            timeSinceLastJump += Time.deltaTime;

        //airMovementSound.volumeOffsetRatio += Time.deltaTime;

        float horizontalSpeed = Vector3.ProjectOnPlane(rigidbody.velocity, Vector3.up).magnitude;
        float _movementSoundRatio = Mathf.Clamp01((horizontalSpeed / 25) * 1.5f);

        if (groundMovementSound != null)
        {
            float _lerpSpeed = 12;
            float _targetPitchOffset = 0.25f;
            if (isGrounded)
                _targetPitchOffset += _movementSoundRatio * 2.15f;
            else
                _lerpSpeed = 3;

            groundMovementSound.pitchOffsetRatio = Mathf.Lerp(groundMovementSound.pitchOffsetRatio, _targetPitchOffset, Time.deltaTime * _lerpSpeed);
            groundMovementSound.volumeOffsetRatio = Mathf.Lerp(groundMovementSound.volumeOffsetRatio, _movementSoundRatio / 3, Time.deltaTime * _lerpSpeed);
        }

        _movementSoundRatio = Mathf.Clamp01((rigidbody.velocity.magnitude / 25) * 1.5f);

        if (airMovementSound != null)
        {
            float _lerpSpeed = 12;
            float _targetPitchOffset = 0.25f;
            float _targetVolumeOffset = 0;

            if (!isGrounded)
            {
                _targetPitchOffset += _movementSoundRatio;
                _targetVolumeOffset += _movementSoundRatio / 1.5f;
            }
            else
                _lerpSpeed = 3;

            airMovementSound.pitchOffsetRatio = Mathf.Lerp(airMovementSound.pitchOffsetRatio, _targetPitchOffset, Time.deltaTime * _lerpSpeed);
            airMovementSound.volumeOffsetRatio = Mathf.Lerp(airMovementSound.volumeOffsetRatio, _targetVolumeOffset, Time.deltaTime * _lerpSpeed);
        }

        bool _wasOnGround = isGrounded;
        CheckGroundContact();
        if (!_wasOnGround && isGrounded)
            MadeGroundContact();

        UpdateHeight();
    }

    private void CalculateSpeedChange()
    {
        //float _deltaSpeed = Mathf.Abs(rigidbody.velocity.magnitude - velocityLastFrame.magnitude);
        //Debug.Log(_deltaSpeed);
        //float _deltaSpeedRatio = Mathf.Clamp01(_deltaSpeed / 20);

        //if (_deltaSpeedRatio < 0.25f)
        //    return;

        //CameraShake.ShakeWithVibration(player.index, 0.2f, _deltaSpeedRatio / 2.5f, 40);
        //CameraShake.AberrateCamera(player.index, 0.4f, _deltaSpeedRatio * 3, 50);
    }

    void OnCollisionEnter(Collision _collision)
    {
        //Debug.Log(_collision.impulse.magnitude);
        float _impuseIntensity = Mathf.Clamp01((_collision.impulse.magnitude - 3) / 50);

        //if (_impuseIntensity < 0.1f)
        //    return;

        //CameraShake.ShakeWithVibration(player.index, 0.2f, _impuseIntensity / 2.5f, 40);
        //CameraShake.AberrateCamera(player.index, 0.4f, _impuseIntensity * 3, 50);

        CameraShake.Shake(player.index, _impuseIntensity * 0.25f, _impuseIntensity, 40);
        CameraShake.VibrateGamepad(player.index, 0.2f + _impuseIntensity * 0.15f, _impuseIntensity / 3.5f);
        CameraShake.AberrateCamera(player.index, 0.25f + _impuseIntensity * 0.25f, _impuseIntensity * 5, 50);
    }

    private void MadeGroundContact()
    {
        float _landingIntensity = Mathf.Clamp01((velocityLastFrame.y / -20) - 0.5f);
        if (_landingIntensity <= 0) return;

        landingSound.volume = _landingIntensity;
        landingSound.Play();

        //CameraShake.ShakeWithVibration(player.index, 0.2f, 0.15f + (_landingIntensity / 3.5f), 40);
        //CameraShake.AberrateCamera(player.index, 0.4f, _landingIntensity * 3, 50);
    }

    void UpdateHeight()
    {
        float _stepSpeed = 0.375f;
        float _stepHeight = 0.25f;

        float _verticalBoomOffset = stepCurve.Evaluate(stepTimer) * _stepHeight;
        float newBoomHeight = Mathf.Lerp(boom.localPosition.y, _verticalBoomOffset, 10 * Time.deltaTime);
        boom.localPosition = new Vector3(boom.localPosition.x, newBoomHeight, boom.localPosition.z);

        float _newHeight = Mathf.Lerp(transform.localScale.y, desiredHeight, Time.deltaTime * heightChangeSpeed);
        transform.localScale = new Vector3(1, _newHeight, 1);

        float movementSpeed = rigidbody.velocity.magnitude;
        if (groundContacts > 0 && movementSpeed > 0.2f)
        {
            stepTimer += _stepSpeed * Time.deltaTime * movementSpeed;
            if (stepTimer > 1)
                stepTimer -= 1;
        }
        else
            stepTimer = 0;
    }

    void FixedUpdate()
    {
        if (!movementDisabled)
            UpdateMovement();

        if (!gravityDisabled)
            rigidbody.AddForce(-averageGroundNormal * gravity * gravityModifier.value);
    }

    private void UpdateMovement()
    {
        Vector3 _movement = movementInput;
        float _inputMagnitude = _movement.magnitude;
        float _inputAngle = Vector3.Angle(Vector3.forward, _movement);

        _movement = transform.TransformDirection(_movement);
        _movement = Vector3.ProjectOnPlane(_movement, averageGroundNormal);
        _movement.Normalize();
        _movement *= moveSpeed * speedModifier.value * _inputMagnitude * directionalModifier.Evaluate(_inputAngle / 180);

        Vector2 _oldNonVertical = new Vector2(rigidbody.velocity.x, rigidbody.velocity.z) / previousMovementModifier;
        Vector2 _desiredNonVertical = new Vector2(_movement.x, _movement.z);
        Vector2 _newNonVertical = Vector2.zero;

        float _acceleration = groundContacts > 0 ? acceleration : airAcceleration;
        float _decceleration = groundContacts > 0 ? decceleration : airDecceleration;

        _acceleration *= accelerationModifier.value;
        _decceleration *= deccelerationModifier.value;

        if (_desiredNonVertical.sqrMagnitude > _oldNonVertical.sqrMagnitude)
            _newNonVertical = Vector2.Lerp(_oldNonVertical, _desiredNonVertical, _acceleration * Time.fixedDeltaTime);
        else
            _newNonVertical = Vector2.Lerp(_oldNonVertical, _desiredNonVertical, _decceleration * Time.fixedDeltaTime);

        //if (_desiredNonVertical.sqrMagnitude > _oldNonVertical.sqrMagnitude)
        //    _newNonVertical = FixedLerp(_oldNonVertical, _desiredNonVertical, _acceleration * 4 * Time.fixedDeltaTime);
        //else
        //    _newNonVertical = FixedLerp(_oldNonVertical, _desiredNonVertical, _decceleration * 4 * Time.fixedDeltaTime);

        velocityLastFrame = rigidbody.velocity;
        rigidbody.velocity = new Vector3(_newNonVertical.x, rigidbody.velocity.y, _newNonVertical.y) * totalMovementModifier.value;
        previousMovementModifier = totalMovementModifier.value;
    }

    //sparate speed interpolation from direction interpolation?

    private static float FixedLerp(float _oldValue, float _newValue, float _maxDelta)
    {
        float _delta = Mathf.Clamp(_newValue - _oldValue, -_maxDelta, _maxDelta);
        return _delta + _oldValue;
    }
    private static Vector2 FixedLerp(Vector2 _oldValue, Vector2 _newValue, float _maxMagnitudeDelta)
    {
        Vector2 _delta = Vector2.ClampMagnitude(_newValue - _oldValue, _maxMagnitudeDelta);
        return _delta + _oldValue;
    }
    //private static Vector3 FixedLerp(Vector3 _oldValue, Vector3 _newValue, float _deltaMax)
    //{
    //    float _returnX = FixedLerp(_oldValue.x, _newValue.x, _deltaMax);
    //    float _returnY = FixedLerp(_oldValue.y, _newValue.y, _deltaMax);
    //    float _returnZ = FixedLerp(_oldValue.z, _newValue.z, _deltaMax);

    //    return new Vector3(_returnX, _returnY, _returnZ);
    //}

    private void OnDestroy()
    {
        DisableInput();
    }
}