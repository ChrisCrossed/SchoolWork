using UnityEngine;
using System.Collections;

public class Grapple : MonoBehaviour
{
    private Player player;
    private new Rigidbody rigidbody;
    private Movement movement;
    private LineRenderer lineRenderer;

    private bool grappleInputEnabled_ = false;
    public bool grappleInputEnabled
    {
        get { return grappleInputEnabled_; }
        set
        {
            if (value == grappleInputEnabled_ || player == null)
                return;

            if (value)
                InputEvents.Grapple.Subscribe(OnGrapple, player.index);
            else
                InputEvents.Grapple.Unsubscribe(OnGrapple, player.index);

            grappleInputEnabled_ = value;
        }
    }

    public Transform raycastFrom;
    public Transform lineStartTransform;
    public float range = 100;
    public float pullSpeed = 10;

    public AnimationCurve pullCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    private float maxDistance = 30;
    private float pullTimer;

    public float minGrappleDistance = 3;

    private float oldDeceleration;
    private float oldAcceleration;

    [System.NonSerialized]
    public Vector3 grapplePosition;
    [System.NonSerialized]
    public float grappleDistance;
    [System.NonSerialized]
    public bool isGrappling = false;

    private float grappleInputRatio = 0;
    public AnimationCurve pullInputCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public AnimationCurve verticalLiftCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    public float rechargeRate = 1;
    public float depletionRate = 1;
    private float charge = 1;

    public Animate animateOnCanGrapple;
    public Animate animateOnCannotGrapple;

    public bool enableInputAtStart = true;

    private bool couldGrappleLastFrame = false;

    private Vector3 lastValidGrapple;

    public void EnableInput()
    {
        grappleInputEnabled = true;
    }
    public void DisableInput()
    {
        grappleInputEnabled = false;
    }

    public void Start()
    {
        player = GetComponent<Player>();
        rigidbody = GetComponent<Rigidbody>();
        movement = GetComponent<Movement>();
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
        SetLineColor();

        oldDeceleration = movement.airDecceleration;

        if (enableInputAtStart)
            EnableInput();
    }

    private void SetLineColor()
    {
        Color _newColor = player.color;

        if (PlayerManager.instance.secondaryOverride != new Color(1, 0, 1, 1))
            _newColor = PlayerManager.instance.secondaryOverride;

        Vector3 _colorOffsetVec = PlayerManager.instance.secondaryColorOffset;
        Color _colorOffset = new Color(_colorOffsetVec.x, _colorOffsetVec.y, _colorOffsetVec.z, 0);

        if (PlayerManager.instance.secondaryOffsetUsesHSV)
        {
            float hue; float saturation; float value;
            Color.RGBToHSV(_newColor, out hue, out saturation, out value);
            _newColor = new Color(hue, saturation, value, 1);
            _newColor += _colorOffset;
            _newColor = Color.HSVToRGB(_newColor.r, _newColor.g, _newColor.b);
        }
        else
            _newColor += _colorOffset;

        lineRenderer.material.SetColor("_EmissionColor", _newColor);
    }

    public SoundSource grappleSound;
    public SoundSource grappleLatchSound;

    public void OnGrapple(InputEventInfo _inputEvent)
    {
        if (!GameManager.instance.allowGameInput) return;

        if (_inputEvent.inputType == InputMethod.InputType.GamepadTrigger)
            grappleInputRatio = GamePadInput.GetInputValue(player.index, GamePadInput.Axis.LeftTrigger);
        else
            grappleInputRatio = 1;

        if (_inputEvent.inputState == InputState.Triggered)
        {
            if (charge <= 0.1) return;

            RaycastHit _hitInfo = new RaycastHit();
            float _distanceToGrapple = float.NaN;
            if (!CanGrapple(out _hitInfo, out _distanceToGrapple))
            {
                float _distanceToValidGrapple = (transform.position - lastValidGrapple).magnitude;
                float _grappleHeight = lastValidGrapple.y - GameManager.instance.killThreshold;

                if (_distanceToValidGrapple <= _grappleHeight)
                    EnterGrapple(lastValidGrapple, _distanceToValidGrapple);
            }
            else
                EnterGrapple(_hitInfo.point, _distanceToGrapple);
        }
        else if (_inputEvent.inputState == InputState.Released)
        {
            ReleaseGrapple();
        }
    }

    private void EnterGrapple(Vector3 _grapplePoint, float _distanceToGrapple)
    {
        grappleSound.Play();
        grappleLatchSound.Play();

        grapplePosition = _grapplePoint;
        grappleDistance = _distanceToGrapple;
        maxDistance = _distanceToGrapple;
        isGrappling = true;

        lineRenderer.SetPosition(0, lineStartTransform.position);
        lineRenderer.SetPosition(1, grapplePosition);
        lineRenderer.enabled = true;

        movement.airDecceleration = 0;

        //oldAcceleration = movement.airDecceleration;
        //movement.airAcceleration = 12;
    }

    void ReleaseGrapple()
    {
        grappleSound.Stop();

        grapplePosition = new Vector3(float.NaN, float.NaN, float.NaN);
        grappleDistance = float.NaN;
        movement.airDecceleration = oldDeceleration;
        //movement.airAcceleration = oldAcceleration;
        isGrappling = false;
        pullTimer = 0;

        lineRenderer.enabled = false;
    }

    void Update()
    {
        bool _canGrapple = CanGrapple();
        if (_canGrapple && !couldGrappleLastFrame)
            OnCanGrapple();
        else if (!_canGrapple && couldGrappleLastFrame)
            OnCannotGrapple();

        couldGrappleLastFrame = _canGrapple;
    }

    private void OnCannotGrapple()
    {
        animateOnCanGrapple.Stop();
    }

    private void OnCanGrapple()
    {
        animateOnCanGrapple.Play();
    }

    void UpdateCharge()
    {
        if (isGrappling)
            charge -= Time.deltaTime * depletionRate;
        else
            charge = Mathf.Clamp01(charge + Time.deltaTime * rechargeRate);

        if (charge <= 0) ReleaseGrapple();

        Debug.Log(charge);
    }

    bool CanGrapple()
    {
        RaycastHit _hitInfo;
        if (!Physics.Raycast(raycastFrom.position, raycastFrom.forward, out _hitInfo, range, 1 << 11, QueryTriggerInteraction.Ignore))
            return false;

        //limit distance based on grapple height attempt
        float _distanceToGrapple = (transform.position - _hitInfo.point).magnitude;
        float _grappleHeight = _hitInfo.point.y - GameManager.instance.killThreshold;

        if (_distanceToGrapple > _grappleHeight)
            return false;

        lastValidGrapple = _hitInfo.point;
        return true;
    }
    bool CanGrapple(out RaycastHit _hitInfo, out float _distanceToGrapple)
    {
        _distanceToGrapple = float.NaN;
        if (!Physics.Raycast(raycastFrom.position, raycastFrom.forward, out _hitInfo, range, 1 << 11, QueryTriggerInteraction.Ignore))
            return false;

        //limit distance based on grapple height attempt
        _distanceToGrapple = (transform.position - _hitInfo.point).magnitude;
        float _grappleHeight = _hitInfo.point.y - GameManager.instance.killThreshold;

        if (_distanceToGrapple > _grappleHeight)
            return false;

        return true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isGrappling) return;

        //if (grapplePosition.y > transform.position.y)
        //{
        //    Debug.Log("got here");
        //    rigidbody.velocity += Vector3.up * 100 * Time.fixedDeltaTime;
        //}

        //RaycastHit _castInfo = new RaycastHit();
        //if (Physics.SphereCast(transform.position + transform.up, 0.75f, rigidbody.velocity, out _castInfo, rigidbody.velocity.magnitude, 1 << 11, QueryTriggerInteraction.Ignore))
        //{
        //    if (Vector3.Angle(Vector3.up, _castInfo.normal) > 45 && Vector3.Angle(-Vector3.up, _castInfo.normal) > 45)
        //    {
        //        Vector3 _cross = Vector3.Cross(_castInfo.normal, rigidbody.velocity);
        //        Vector3 _newVelocity = Quaternion.AngleAxis(180, _cross) * rigidbody.velocity;
        //        //rigidbody.velocity = transform.up * rigidbody.velocity.magnitude;//_castInfo.normal * rigidbody.velocity.magnitude;

        //    }

        //}

        //if(grappleDistance < minGrappleDistance + 3)
        //{
        //    rigidbody.velocity += transform.up * Time.fixedDeltaTime * 100 * (1 - verticalLiftCurve.Evaluate(maxDistance / grappleDistance));
        //}

        lineRenderer.SetPosition(0, lineStartTransform.position);

        Vector3 _vecFromGrapple = transform.position - grapplePosition;

        pullTimer += Time.fixedDeltaTime;

        if (grappleDistance <= minGrappleDistance)
        {
            grappleDistance = (transform.position - grapplePosition).magnitude;
            return;
        }

        float _totalPullStrength = pullCurve.Evaluate(maxDistance / grappleDistance) * pullSpeed * pullInputCurve.Evaluate(grappleInputRatio); //evaluating the rate of update on the pull
        float _testGrappleDistance = Mathf.MoveTowards(grappleDistance, minGrappleDistance, _totalPullStrength * Time.fixedDeltaTime);

        Vector3 _newPosition = grapplePosition + (transform.position - grapplePosition).normalized * _testGrappleDistance;

        if (_testGrappleDistance > minGrappleDistance && !Physics.CheckSphere(_newPosition + transform.up, 0.9f, 1 << 11, QueryTriggerInteraction.Ignore))
            grappleDistance = _testGrappleDistance;

        float _distanceToGrapplePoint = (transform.position - grapplePosition).magnitude; //the distance that the player actually is from the grapple point

        if (_distanceToGrapplePoint < grappleDistance)
        {
            grappleDistance = _distanceToGrapplePoint;
            return;
        }

        if (Vector3.Angle(Vector3.Project(rigidbody.velocity, _vecFromGrapple), _vecFromGrapple) < 90)
        {
            float _speed = rigidbody.velocity.magnitude;
            Vector3 _newVelocity = Vector3.ProjectOnPlane(rigidbody.velocity, _vecFromGrapple);
            rigidbody.velocity = _newVelocity.normalized * _speed;
        }

        _newPosition = grapplePosition + (transform.position - grapplePosition).normalized * grappleDistance;
        rigidbody.MovePosition(_newPosition);

        //rigidbody.velocity -= _fromGrapple.normalized * 2;
    }

    void OnDestroy()
    {
        DisableInput();
    }
}
