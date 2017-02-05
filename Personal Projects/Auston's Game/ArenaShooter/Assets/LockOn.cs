using UnityEngine;
using System.Collections;

public class LockOn : MonoBehaviour
{
    private Player player;
    private Movement movement;
    private new Rigidbody rigidbody;
    private AnimateFOV animateFOV;
    private ManualShooting manualShooting;

    [Range(0, 10)]
    public float drag = 2f;
    [Range(0, 1)]
    public float gravityMultiplier = 0.25f;
    [Range(0, 1)]
    public float accelerationMultiplier = 0.25f;
    [Range(0, 1)]
    public float decelerationMultiplier = 0.25f;

    public float targetLateralWhenInLockOn;
    public float lateralInterpolationSpeed;

    public float targetVerticalWhenInLockOn;
    public float verticalInterpolationSpeed;

    public bool inLockOn;
    private Vector3 velocityOnTriggered;
    private float speedOnTriggered;

    public float[] stepSpeeds = new float[7];

    [System.NonSerialized]
    public float lockOnRatio;
    [System.NonSerialized]
    public int lockOnStep = 0;

    private bool lockOnInputEnabled_ = false;
    public bool lockOnInputEnabled
    {
        get { return lockOnInputEnabled_; }
        set
        {
            if (value == lockOnInputEnabled_)
                return;

            if (value)
                InputEvents.LockOn.Subscribe(OnLockOn, player.index);
            else
                InputEvents.LockOn.Unsubscribe(OnLockOn, player.index);

            lockOnInputEnabled_ = value;
        }
    }

    private void OnLockOn(InputEventInfo _inputEventInfo)
    {
        if (_inputEventInfo.inputState == InputState.Triggered)
        {
            //rigidbody.drag = drag;
            //movement.gravityModifier.AddMultiplier(gravityMultiplier);
            //movement.accelerationModifier.AddMultiplier(accelerationMultiplier);
            //movement.deccelerationModifier.AddMultiplier(decelerationMultiplier);

            inLockOn = true;
            velocityOnTriggered = rigidbody.velocity;
            speedOnTriggered = velocityOnTriggered.magnitude;

            lockOnRatio = 0f;
            lockOnStep = 0;
            animateFOV.endValue = 55;
        }
        else
        {
            //rigidbody.drag = 0;
            //movement.gravityModifier.RemoveMultiplier(gravityMultiplier);
            //movement.accelerationModifier.RemoveMultiplier(accelerationMultiplier);
            //movement.deccelerationModifier.RemoveMultiplier(decelerationMultiplier);

            ExitLockOn();
        }
    }

    public void ExitLockOn(bool _fireRocket = true)
    {
        if (!inLockOn) return;

        animateFOV.endValue = Mathf.MoveTowards(65, 75, (lockOnStep / 7) * 10);
        inLockOn = false;

        if (_fireRocket)
            manualShooting.Shoot();
    }

    // Use this for initialization
    void Start()
    {
        player = GetComponent<Player>();
        rigidbody = GetComponent<Rigidbody>();
        movement = GetComponent<Movement>();
        manualShooting = GetComponent<ManualShooting>();

        animateFOV = player.camera.GetComponent<AnimateFOV>();

        lockOnInputEnabled = true;
    }

    //void OnEnable()
    //{
    //    player = GetComponent<Player>();
    //    rigidbody = GetComponent<Rigidbody>();
    //    movement = GetComponent<Movement>();

    //    lockOnInputEnabled = true;
    //}

    void OnDestroy()
    {
        lockOnInputEnabled = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        if (!inLockOn) return;

        Vector3 lateralVelocity = new Vector3(velocityOnTriggered.x, 0, velocityOnTriggered.z); ;
        Vector3 verticalVelocty = new Vector3(0, velocityOnTriggered.y, 0);

        rigidbody.velocity = Vector3.Lerp(lateralVelocity, lateralVelocity.normalized * targetLateralWhenInLockOn, lateralInterpolationSpeed * Time.fixedDeltaTime);
        rigidbody.velocity += Vector3.Lerp(verticalVelocty, verticalVelocty.normalized * targetVerticalWhenInLockOn, verticalInterpolationSpeed * Time.fixedDeltaTime);

        velocityOnTriggered = rigidbody.velocity;

        lockOnRatio = Mathf.MoveTowards(lockOnRatio, 1, Time.deltaTime * (1 / stepSpeeds[lockOnStep]));

        Debug.Log(lockOnRatio + " | " + lockOnStep);

        if (lockOnRatio == 1)
        {
            CameraShake.Shake(player.index, 0.2f, 0.1f, 80, 1);
            CameraShake.VibrateGamepad(player.index, 0.1f, 0.85f);
            CameraShake.AberrateCamera(player.index, 0.05f, 8, 30);
            animateFOV.Play();
            animateFOV.endValue += 1.33f;
            lockOnRatio = 0;
            ++lockOnStep;

            if (lockOnStep > 7)
            {
                ExitLockOn();
            }
        }
    }
}
