using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

[RequireComponent(typeof(Player))]
public class ManualShooting : Shooting
{
    private Player player = null;
    private Targeting targeting = null;
    public new Transform camera = null;

    protected DepthOfField depthOfField = null;
    protected Transform focalTransform = null;

    [Range(0, 2)]
    public float cooldown;
    [Range(0, 3)]
    public int cost;

    public float maxTargetDistance = 40;
    public bool enableInputAtStart = true;

    private bool shootInputEnabled_ = false;
    public bool shootInputEnabled
    {
        get { return shootInputEnabled_; }
        set
        {
            if (value == shootInputEnabled_ || player == null)
                return;

            if (value)
                InputEvents.RightBasic.Subscribe(OnShoot, player.index);
            else
                InputEvents.RightBasic.Unsubscribe(OnShoot, player.index);

            shootInputEnabled_ = value;
        }
    }

    public float maxAngle = 180;
    public AnimationCurve angleSeekingFalloff = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public AnimationCurve otherSpeedSeekingFalloff = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public float maxOtherSpeedThreshold = 30;
    //own velocity seeking falloff?

    public Animate[] playAllOnShoot;

    private Vector3 debugLastEstimatedPosition;
    public float shootFromVerticalSpeed = 1;

    public bool shotCancelled = false;

    // Use this for initialization
    new void Start()
    {
        base.Start();
        player = GetComponent<Player>();
        targeting = GetComponent<Targeting>();

        if (camera != null)
        {
            depthOfField = camera.GetComponent<DepthOfField>();

            focalTransform = new GameObject().transform;
            focalTransform.name = "FocalTransform";
            focalTransform.parent = transform;
            depthOfField.focalTransform = focalTransform;
        }

        if (enableInputAtStart)
            EnableInput();
    }

    public void DisableInput()
    {
        shootInputEnabled = false;
    }
    public void EnableInput()
    {
        shootInputEnabled = true;
    }

    public void OnShoot(InputEventInfo _eventInfo)
    {

    }

    public void Shoot()
    {
        if (!GameManager.instance.allowGameInput) return;

        if (timeSinceShot < cooldown)
            return;

        if (shootFrom.Length < 1)
        {
            Debug.Log(gameObject.name + " has no transform to shoot from");
            return;
        }

        //if (!GetComponent<Energy>().SpendEnergy(cost))
        //    return;

        if (!GetComponent<RocketAmmo>().SpendRocket(cost))
            return;

        CameraShake.Shake(player.index, 0.2f, 0.35f, 30, 1);
        CameraShake.AberrateCamera(player.index, 0.35f, 4, 30);

        if (currentShootFrom == 1) CameraShake.VibrateGamepad(player.index, 0.35f, 0.25f, 0.45f);
        else CameraShake.VibrateGamepad(player.index, 0.35f, 0.45f, 0.25f);

        if (shootFrom.Length == 1) currentShootFrom = 0;
        else ++currentShootFrom;

        if (currentShootFrom >= shootFrom.Length)
            currentShootFrom -= shootFrom.Length;

        Transform _shootFrom = shootFrom[currentShootFrom];

        for (int i = 0; i < playAllOnShoot.Length; i++)
        {
            if (playAllOnShoot[i] != null)
                playAllOnShoot[i].Play();
        }

        GameObject _newProjectile = null;
        if (projectileObject == null)
            _newProjectile = CreateAndShootProjectile(_shootFrom);
        else
            _newProjectile = ShootProjectile(_shootFrom);

        SetUpProjectile(_newProjectile);
    }

    public void SetUpProjectile(GameObject _newProjectile)
    {
        //setting up the projectile
        if (targeting.currentTarget != null)
        {
            Projectile _projectile = _newProjectile.GetComponent<Projectile>();
            _projectile.GetComponent<Targeting>().targetOverride = targeting.currentTarget; //setting the target

            //Debug.Log("Target: " + AffiliatedObject.activeObjects[_targetObjectIndex].gameObject.name);

            //seek strength modifiers
            //Debug.Log("Angle: " + _smallestAngle + " | " + "Ratio: " + _smallestAngle / maxAngle);
            //_projectile.seekingStrength *= 1 - angleSeekingFalloff.Evaluate(_smallestAngle / maxAngle); //angle modifier
            //float _otherSpeed = AffiliatedObject.activeObjects[_targetObjectIndex].GetComponent<Rigidbody>().velocity.magnitude;
            //_projectile.seekingStrength *= otherSpeedSeekingFalloff.Evaluate(_otherSpeed / maxOtherSpeedThreshold); //target movement modifier
        }
        else
            _newProjectile.GetComponent<Projectile>().GetComponent<Targeting>().targetOverride = null;
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();

        RaycastHit _hitInfo;
        //LayerMask _targetables = (1 << 10);
        LayerMask _onlyGeometry = (1 << 11);
        Vector3 _currentCastPosition = camera.transform.position;
        Vector3 _targetPoint = _currentCastPosition + camera.forward * maxTargetDistance;

        //for (int i = 1; i <= castResolution; i++)
        //{
        //    float _castRadius = (maxCastRadius / castResolution) * i;
        //    Vector3 _castStart = _currentCastPosition - (boom.forward * _castRadius);
        //    float _castDistance = (maxTargetDistance / castResolution) - _castRadius;

        //    if (Physics.SphereCast(_castStart, _castRadius, boom.forward, out _hitInfo, _castDistance, _notBullets))
        //    {
        //        _targetPoint = _hitInfo.point;
        //        break;
        //    }

        //    _currentCastPosition = _castStart + boom.forward * _castDistance;
        //}

        if (Physics.Raycast(_currentCastPosition, camera.forward, out _hitInfo, maxTargetDistance, _onlyGeometry, QueryTriggerInteraction.Ignore))
            _targetPoint = _currentCastPosition + camera.forward * (_hitInfo.distance - 0.35f);

        //if (focalTransform != null)
        //    focalTransform.position = _targetPoint;

        foreach (Transform _shootFrom in shootFrom)
        {
            InterpolateToPosition _interpolator = _shootFrom.GetComponent<InterpolateToPosition>();
            Vector3 _castPosition = _shootFrom.parent.TransformPoint(new Vector3(_interpolator.startingLocal.x, -0.95f, 0));

            Debug.DrawLine(_castPosition, _castPosition + camera.forward * 0.95f);

            if (Physics.Raycast(_castPosition, _shootFrom.forward, 3, _onlyGeometry, QueryTriggerInteraction.Ignore))
                _shootFrom.GetComponent<InterpolateToPosition>().startingLocal.y = 0;
            else
                _shootFrom.GetComponent<InterpolateToPosition>().startingLocal.y = -0.45f;

            if ((_targetPoint - _shootFrom.position).sqrMagnitude > 1.15f)
                _shootFrom.LookAt(_targetPoint);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(debugLastEstimatedPosition, 1);
    }

    void OnDestroy()
    {
        DisableInput();
    }
}
