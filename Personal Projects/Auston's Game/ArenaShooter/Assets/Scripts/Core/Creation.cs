using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Player))]
public class Creation : MonoBehaviour
{
    public GameObject objectToCreate;

    private Player player = null;
    private bool createInputEnabled_ = false;
    public bool createInputEnabled
    {
        get { return createInputEnabled_; }
        set
        {
            if (value == createInputEnabled_ || player == null)
                return;

            if (value)
                InputEvents.LeftBasic.Subscribe(OnBuild, player.index);
            else
                InputEvents.LeftBasic.Unsubscribe(OnBuild, player.index);

            createInputEnabled_ = value;
        }
    }

    public bool enableInputAtStart = true;

    public void EnableInput()
    {
        createInputEnabled = true;
    }
    public void DisableInput()
    {
        createInputEnabled = false;
    }

    private static Vector3 debugCapsuleBottom;
    private static Vector3 debugCapsuleTop;
    private static float debugCapsuleRadius;

    public float cooldown = 0.15f;
    public int energyCost = 0;
    public int ammoCost = 1;

    public int maximumCreatedObjects = 3;

    private float timeSinceCreate;

    public bool debugDraw = false;

    [System.NonSerialized]
    public List<GameObject> createdObjects = new List<GameObject>();

    void OnBuild(InputEventInfo _inputEventInfo)
    {
        if (!GameManager.instance.allowGameInput) return;

        Vector3 _camPosition = player.camera.position;

        if (createdObjects.Count >= maximumCreatedObjects)
        {
            Health _health = createdObjects[0].GetComponent<Health>();

            if (_health != null)
                _health.DealDamage(int.MaxValue, null, -1);
            else
                Destroy(createdObjects[0]);

            createdObjects.RemoveAt(0);
        }

        RaycastHit _hitInfo;
        if (Physics.Raycast(_camPosition, player.camera.forward, out _hitInfo, 40, ~0, QueryTriggerInteraction.Ignore))
        {
            if (_hitInfo.collider.gameObject.layer != 11)
                return;
        }
        else
        {
            //Debug.Log("No surface to build on...");
            return;
        }

        //if (Physics.CheckCapsule(_hitInfo.point + _hitInfo.normal * 0.3f, _hitInfo.point + _hitInfo.normal * 1f, 0.25f, ~0, QueryTriggerInteraction.Ignore))
        if (CheckCapsuleCollider(objectToCreate.GetComponent<CapsuleCollider>(), _hitInfo.point, _hitInfo.normal, 0.95f))
        {
            //Debug.Log("Not enough room...");
            return;
        }

        if (!GetComponent<RocketAmmo>().SpendRocket(ammoCost))
            return;

        if (!GetComponent<Energy>().SpendEnergy(energyCost))
            return;

        Vector3 _pointToSelfFlat = Vector3.ProjectOnPlane(_camPosition - _hitInfo.point, _hitInfo.normal);
        Quaternion _botRotation = Quaternion.LookRotation(_pointToSelfFlat, _hitInfo.normal);

        GameObject _newBot = (GameObject)Instantiate(objectToCreate, _hitInfo.point, _botRotation);
        Bot _bot = _newBot.GetComponent<Bot>();
        _bot.team = player.team;
        _bot.createdByIndex = player.index;

        createdObjects.Add(_newBot);

        //_newBot.GetComponent<AffiliatedObject>().team = Random.Range(player.team, player.team + 2);
    }

    // Use this for initialization
    void Start ()
    {
        player = GetComponent<Player>();

        if (enableInputAtStart)
            EnableInput();
	}

    //uses the collider's object's rotation and position
    public static bool CheckCapsuleCollider(CapsuleCollider _collider, float _radialGapRatio = 0.9f, bool _onlyStretchWidth = false)
    {
        Transform _transform = _collider.transform;
        Vector3 _position = _transform.position;
        Vector3 _upVector = _transform.up;
        return CheckCapsuleCollider(_collider, _position, _upVector, _radialGapRatio);
    }
    //uses the supplied rotation and position
    public static bool CheckCapsuleCollider(CapsuleCollider _collider, Vector3 _position, Quaternion rotation, float _radialGapRatio = 0.9f, bool _onlyStretchWidth = false)
    {
        Vector3 _upVector = rotation * Vector3.up;
        return CheckCapsuleCollider(_collider, _position, _upVector, _radialGapRatio);
    }
    //uses the supplied up vector and position
    public static bool CheckCapsuleCollider(CapsuleCollider _collider, Vector3 _position, Vector3 _upVector, float _radialGapRatio = 0.9f, bool _onlyStretchWidth = false)
    {
        _radialGapRatio = Mathf.Clamp01(_radialGapRatio);

        Transform _transform = _collider.transform;
        Vector3 _truePosition = _position + (_collider.center.y * _upVector);

        float _adjustedRadius = _collider.radius * _radialGapRatio;
        float _adjustedHeight = _collider.height - _collider.radius * 2;

        //if (_onlyStretchWidth && _adjustedHeight - _adjustedRadius * 2 > 0)
        //    _adjustedHeight -= _adjustedRadius * 2;

        Vector3 _bottom = _truePosition - (_upVector * _adjustedHeight * 0.5f);
        Vector3 _top = _truePosition + (_upVector * _adjustedHeight * 0.5f);

        debugCapsuleBottom = _bottom;
        debugCapsuleTop = _top;
        debugCapsuleRadius = _adjustedRadius;

        return Physics.CheckCapsule(_bottom, _top, _adjustedRadius, ~0, QueryTriggerInteraction.Ignore);
    }
    public static Vector3 GetCapsuleColliderCenter(CapsuleCollider _collider)
    {
        Transform _transform = _collider.transform;
        Vector3 _position = _transform.position;
        Vector3 _upVector = _transform.up;
        return _position + (_collider.center.y * _upVector);
    }
    public static Vector3 GetCapsuleColliderCenter(CapsuleCollider _collider, Vector3 _position, Vector3 _upVector)
    {
        return _position + (_collider.center.y * _upVector);
    }

    void OnDrawGizmos()
    {
        if (!debugDraw)
            return;

        Gizmos.DrawWireSphere(debugCapsuleBottom, debugCapsuleRadius);
        Gizmos.DrawWireSphere(debugCapsuleTop, debugCapsuleRadius);
    }

    // Update is called once per frame
    void Update ()
    {
        //timeSinceCreate += Time.deltaTime;
	}
    
    void OnDestroy()
    {
        DisableInput();
    }
}
