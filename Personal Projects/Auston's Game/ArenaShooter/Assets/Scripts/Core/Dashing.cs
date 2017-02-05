using UnityEngine;
using System.Collections;

public class Dashing : MonoBehaviour
{
    private Movement movement;
    private new Rigidbody rigidbody;
    private Player player;
    private Energy energy;
    private Grapple grapple;
    public LockOn lockOn;

    public float dashDistance = 3;
    public float dashSpeed = 1;
    public AnimationCurve dashSpeedCurve = AnimationCurve.EaseInOut(0, 1, 1, 1);
    [Range(0, 2)] public float cooldown = 0.35f;
    public int cost = 1;

    public GameObject[] objectsToAnimate = new GameObject[0];
    public Transform head = null;

    private float dashDuration;
    public bool isDashing { get { return timeSinceDash <= dashDuration; } }
    private float dashCompletionRatio { get { return Mathf.Clamp01(timeSinceDash / dashDuration); } }

    private float timeSinceDash = 0;
    private Vector3 dashDirection;

    public SoundSource dashSound;

    [Range(0,1)]
    public float limitVertical = 1;

    private bool dashApplied = false;
    public bool avoidSeekWhileDashing = true;

    // Use this for initialization
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        movement = GetComponent<Movement>();
        player = GetComponent<Player>();
        energy = GetComponent<Energy>();
        grapple = GetComponent<Grapple>();
        lockOn = GetComponent<LockOn>();

        InputEvents.Dash.Subscribe(OnDash, player.index);
    }

    void OnDash(InputEventInfo _inputInfo)
    {
        if (!GameManager.instance.allowGameInput /*|| lockOn.inLockOn*/) return;

        lockOn.ExitLockOn(false);

        if (timeSinceDash < cooldown) return; //cant dash again yet

        if (!energy.SpendEnergy(cost))
            return;

        dashSound.Play();

        dashDuration = dashDistance / dashSpeed;
        movement.movementDisabled = true;
        movement.gravityDisabled = true;
        timeSinceDash = 0;

        dashApplied = false;

        CameraShake.Shake(player.index, 0.1f, 0.55f, 40, 1);
        CameraShake.VibrateGamepad(player.index, 0.2f, 0.25f);
        CameraShake.AberrateCamera(player.index, 0.3f, 5, 30);

        dashDirection = movement.movementInput.normalized;
        if (dashDirection == Vector3.zero)
            dashDirection = new Vector3(0, 0, 1);

        for (int j = 0; j < objectsToAnimate.Length; j++)
        {
            Animate[] _animations = objectsToAnimate[j].GetComponents<Animate>();

            for (int i = 0; i < _animations.Length; i++)
            {
                _animations[i].duration = dashDuration;
                _animations[i].Play();
            }
        }
    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        bool _wasDashing = isDashing;
        timeSinceDash += Time.fixedDeltaTime;

        if (_wasDashing && !isDashing)
        {
            movement.movementDisabled = false;
            movement.gravityDisabled = false;
        }

        if (!isDashing) return;

        float _newSpeed = dashSpeedCurve.Evaluate(dashCompletionRatio) * dashSpeed; //evaluating what the speed should be at this point in the dash
        //Vector3 _forwardNulledVelocity = Vector3.ProjectOnPlane(rigidbody.velocity, head.forward); //zeroing forward velocity (maintains left-right and up-down)
        //rigidbody.velocity = /*_forwardNulledVelocity + */(head.forward * _newSpeed); //setting new forward velocity

        if (grapple.isGrappling)
        {
            if (!dashApplied)
            {
                float _originalSpeed = rigidbody.velocity.magnitude;
                rigidbody.velocity += head.TransformDirection(dashDirection) * dashSpeed;
                float _finalSpeed = rigidbody.velocity.magnitude;
                if (_finalSpeed > dashSpeed)
                {
                    rigidbody.velocity /= _finalSpeed;
                    rigidbody.velocity *= Mathf.Max(dashSpeed, _originalSpeed);
                }

                dashApplied = true;
            }
        }
        else
            rigidbody.velocity = head.TransformDirection(dashDirection) * _newSpeed;

        //float _verticalVelocity = transform.InverseTransformDirection(rigidbody.velocity).y;
        //if (_verticalVelocity > 0)
        //{
        //    rigidbody.velocity = Vector3.ProjectOnPlane(rigidbody.velocity, transform.up);
        //    rigidbody.velocity += transform.up * _verticalVelocity * limitVertical;
        //}
    }

    void OnDestroy()
    {
        if (player != null)
            InputEvents.Dash.Unsubscribe(OnDash, player.index);
    }
}
