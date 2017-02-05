using UnityEngine;
using System.Collections;

public class SpectatorCamera : MonoBehaviour
{
    public Player player;
    private new Rigidbody rigidbody = null;
    private Vector3 movementInput;
    public float moveSpeed = 12;
    public float acceleration = 10;
    public float deceleration = 3;
    
    private bool moveInputDisabled = false;
    private bool verticalMoveInputDisabled = false;

    // Use this for initialization
    void Start ()
    {
        Player player = GetComponent<Player>();

        InputEvents.Movement.Subscribe(OnMove, player.index);
        InputEvents.VerticalMovement.Subscribe(OnVerticalMove, player.index);

        rigidbody = GetComponent<Rigidbody>();
        if (rigidbody == null)
            rigidbody = gameObject.AddComponent<Rigidbody>();

        rigidbody.useGravity = false;
    }

    public void DisableInput(bool _moveInput = true, bool _verticalMoveInput = true)
    {
        if (_moveInput) moveInputDisabled = true;
        if (_verticalMoveInput) verticalMoveInputDisabled = true;
    }
    public void EnableInput(bool _lookInput = true, bool _shootInput = true)
    {
        if (_lookInput) moveInputDisabled = false;
        if (_shootInput) verticalMoveInputDisabled = false;
    }

    void OnMove(InputEventInfo _eventInfo)
    {
        if (moveInputDisabled) return;

        movementInput.x = _eventInfo.dualAxisValue.x;
        movementInput.z = _eventInfo.dualAxisValue.y;
    }
    void OnVerticalMove(InputEventInfo _eventInfo)
    {
        if (verticalMoveInputDisabled) return;

        movementInput.y = _eventInfo.singleAxisValue;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateMovement();
    }

    void UpdateMovement()
    {
        Vector3 _desiredVelocity = movementInput * moveSpeed;
        _desiredVelocity = transform.TransformDirection(_desiredVelocity);

        Vector3 _oldVelocity = rigidbody.velocity;
        Vector3 _newVelocity = Vector3.zero;

        if (_desiredVelocity.sqrMagnitude > _oldVelocity.sqrMagnitude)
            _newVelocity = Vector3.Lerp(_oldVelocity, _desiredVelocity, acceleration * Time.fixedDeltaTime);
        else
            _newVelocity = Vector3.Lerp(_oldVelocity, _desiredVelocity, deceleration * Time.fixedDeltaTime);

        rigidbody.velocity = _newVelocity;
    }
}
