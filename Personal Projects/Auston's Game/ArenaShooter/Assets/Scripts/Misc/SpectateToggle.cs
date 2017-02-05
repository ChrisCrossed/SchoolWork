using UnityEngine;
using System.Collections;

public class SpectateToggle : MonoBehaviour
{
    private new Camera camera = null;
    private AudioListener audioListener = null;
    private Player player = null;
    public Camera spectatorCamera = null;
    public AudioListener spectatorAudioListener = null;
    private Movement movement = null;
    private ManualAiming aiming = null;
    private bool playerInputDisabled = false;

    public Vector3 spectateCamStartOffset = new Vector3(0, 2, -2);

    private bool spectateInputEnabled_ = false;
    private bool spectateInputEnabled
    {
        get { return spectateInputEnabled_; }
        set
        {
            if (value == spectateInputEnabled_)
                return;

            if(value)
                InputEvents.Spectate.Subscribe(OnSpectate, player.index);
            else
                InputEvents.Spectate.Unsubscribe(OnSpectate, player.index);

            spectateInputEnabled_ = value;
        }
    }

    // Use this for initialization
    void Start ()
    {
        camera = GetComponent<Camera>();
        player = transform.root.GetComponent<Player>();
        audioListener = GetComponent<AudioListener>();
        movement = transform.root.GetComponent<Movement>();
        aiming = transform.root.GetComponent<ManualAiming>();

        spectateInputEnabled = true;
    }

    void OnSpectate(InputEventInfo _eventInfo)
    {
        if (spectatorCamera == null) //first press
        {
            spectatorCamera = Instantiate(gameObject).GetComponent<Camera>();
            spectatorCamera.gameObject.AddComponent<SpectatorCamera>();
            spectatorCamera.transform.parent = null;

            Player _player = spectatorCamera.gameObject.AddComponent<Player>();
            _player.index = transform.root.GetComponent<Player>().index;

            spectatorCamera.gameObject.AddComponent<ManualAiming>();

            Destroy(spectatorCamera.gameObject.GetComponent<SpectateToggle>());

            Rigidbody _rigidbody = spectatorCamera.gameObject.AddComponent<Rigidbody>();
            _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            SphereCollider _collider = spectatorCamera.gameObject.AddComponent<SphereCollider>();
            _collider.material.bounciness = 0;
            _collider.material.dynamicFriction = 0;
            _collider.material.staticFriction = 0;
            _collider.material.bounceCombine = PhysicMaterialCombine.Minimum;
            _collider.material.frictionCombine = PhysicMaterialCombine.Minimum;

            spectatorCamera.transform.position = transform.position + transform.TransformVector(spectateCamStartOffset);
            spectatorCamera.transform.rotation = transform.rotation;
            spectatorCamera.cullingMask = ~0;

            camera.enabled = false;

            movement.DisableInput();
            aiming.DisableInput();

            playerInputDisabled = true;
        }
        else if(playerInputDisabled) //second press
        {
            movement.EnableInput();
            aiming.EnableInput();

            spectatorCamera.GetComponent<SpectatorCamera>().DisableInput();
            spectatorCamera.GetComponent<ManualAiming>().DisableInput();
            
            playerInputDisabled = false;
        }
        else //third press
        {
            camera.enabled = true;
            //audioListener.enabled = true;

            Destroy(spectatorCamera.gameObject);
        }
    }
	
	void OnDestroy ()
    {
        spectateInputEnabled = false;

        if (spectatorCamera != null)
            Destroy(spectatorCamera.gameObject);
    }
}
