using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AffiliatedObject))]
public class Shooting : MonoBehaviour
{
    protected AffiliatedObject affiliatedObject = null;

    public GameObject projectileObject;
    public Vector3 shootOffset = new Vector3(0, 0, 0.5f); //the offset from the creation object for the projetile to be spawned at
    public float projectileSize = 0; //if 0 or negative the default values will be used

    [Range(0, 1)] public float inheritedVelocityRatio = 0.65f;
    [Range(30, 100)] public float projectileSpeed = 20;

    //public bool autoTargetCast = false;
    //public float maxCastRadius = 1;
    //public float castResolution = 1;
    
    public Transform[] shootFrom = new Transform[0];
    public AnimateGroup[] playOnShoot = new AnimateGroup[0];
    protected int currentShootFrom = 0;

    protected float timeSinceShot = 0;
    //public bool debugDraw = false;

    public SoundSource soundSource;

    // Use this for initialization
    protected void Start ()
    {
        if (soundSource == null)
            soundSource = GetComponent<SoundSource>();

        affiliatedObject = GetComponent<AffiliatedObject>();
    }

    protected GameObject ShootProjectile(Transform _shootFromTransform)
    {
        if (soundSource != null)
            soundSource.Play();

        Vector3 _position = _shootFromTransform.position + _shootFromTransform.TransformVector(shootOffset);
        Quaternion _rotation = _shootFromTransform.rotation;
        GameObject _projectileObject = (GameObject)Instantiate(projectileObject, _position, _rotation);
        _projectileObject.layer = 9;

        if(projectileSize > 0)
            _projectileObject.transform.localScale = new Vector3(projectileSize, projectileSize, projectileSize);

        Projectile _projectile = _projectileObject.GetComponent<Projectile>();
        _projectile.creator = transform.root.gameObject;

        _projectileObject.GetComponent<AffiliatedObject>().team = affiliatedObject.team;

        //projectile should just be given cooresponding "projectileInfo" from the "abilityInfo" that is shooting this

        if (currentShootFrom < playOnShoot.Length && playOnShoot[currentShootFrom] != null)
            playOnShoot[currentShootFrom].Play();

        Rigidbody _creatorRigidbody = GetComponent<Rigidbody>();
        Vector3 _inheritedVelocity = Vector3.zero;
        if (_creatorRigidbody != null)
            _inheritedVelocity = _creatorRigidbody.velocity * inheritedVelocityRatio;

        Rigidbody _rigidbody = _projectileObject.GetComponent<Rigidbody>();
        _rigidbody.velocity = _inheritedVelocity + _shootFromTransform.forward * projectileSpeed;

        timeSinceShot = 0;

        return _projectileObject;
    }
    protected GameObject CreateAndShootProjectile(Transform _shootFromTransform)
    {
        if (soundSource != null)
            soundSource.Play();

        GameObject _projectileObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        _projectileObject.transform.position = _shootFromTransform.position + _shootFromTransform.forward * 2f;
        _projectileObject.transform.rotation = _shootFromTransform.rotation;
        _projectileObject.transform.localScale = new Vector3(0.1f, 0.1f, 4f);
        _projectileObject.layer = 9;

        MeshRenderer _meshRenderer = _projectileObject.GetComponent<MeshRenderer>();
        //_meshRenderer.material = projectileMaterial;
        _meshRenderer.material.color = affiliatedObject.color;
        _meshRenderer.material.SetColor("_EmissionColor", affiliatedObject.color);

        Projectile _projectile = _projectileObject.AddComponent<Projectile>();
        //_projectile.createOnDestroy = createOnDestroy;
        _projectile.creator = transform.root.gameObject;

        Rigidbody _creatorRigidbody = GetComponent<Rigidbody>();
        Vector3 _inheritedVelocity = Vector3.zero;
        if (_creatorRigidbody != null)
            _inheritedVelocity = _creatorRigidbody.velocity;

        Rigidbody _rigidbody = _projectileObject.AddComponent<Rigidbody>();
        _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        _rigidbody.useGravity = false;
        _rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        
        _rigidbody.velocity = _inheritedVelocity + _shootFromTransform.forward * projectileSpeed;

        Destroy(_projectileObject.GetComponent<SphereCollider>());
        BoxCollider _collider = _projectileObject.AddComponent<BoxCollider>();
        _collider.isTrigger = true;

        timeSinceShot = 0;

        return projectileObject;
    }

    // Update is called once per frame
    protected void Update ()
    {
        timeSinceShot += Time.deltaTime;
    }

    void OnDrawGizmos()
    {
        //if (boom == null || debugDraw == false) return;

        ////RaycastHit _hitInfo;
        ////LayerMask _targetables = (1 << 10);
        //Vector3 _currentCastPosition = boom.transform.position;
        //Vector3 _targetPoint = _currentCastPosition + boom.forward * maxTargetDistance;
        ////bool _hit = false;

        //for (int i = 1; i <= castResolution; i++)
        //{
        //    float _castRadius = (maxCastRadius / castResolution) * i;
        //    Vector3 _castStart = _currentCastPosition - (boom.forward * _castRadius);
        //    float _castDistance = (maxTargetDistance / castResolution) - _castRadius;
        //    Vector3 _castEnd = _castStart + boom.forward * _castDistance;

        //    Gizmos.DrawWireSphere(_castStart, _castRadius);
        //    Gizmos.DrawLine(_castStart + Vector3.up * _castRadius, _castEnd + Vector3.up * _castRadius);
        //    Gizmos.DrawLine(_castStart + Vector3.down * _castRadius, _castEnd + Vector3.down * _castRadius);
        //    Gizmos.DrawLine(_castStart + Vector3.left * _castRadius, _castEnd + Vector3.left * _castRadius);
        //    Gizmos.DrawLine(_castStart + Vector3.right * _castRadius, _castEnd + Vector3.right * _castRadius);

        //    _currentCastPosition = _castEnd;
        //}
    }
}