using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Player))]
public class ManualAiming : Aiming
{
    private Player player = null;
    private bool aimInputEnabled_ = false;
    public bool aimInputEnabled
    {
        get { return aimInputEnabled_; }
        set
        {
            if (value == aimInputEnabled_ || player == null)
                return;

            aimInput = Vector3.zero;

            if (value)
                InputEvents.Aim.Subscribe(OnAim, player.index);
            else
                InputEvents.Aim.Unsubscribe(OnAim, player.index);

            aimInputEnabled_ = value;
        }
    }

    public bool enableInputAtStart = true;
    public bool lockCursor = false;

    // Use this for initialization
    new void Start ()
    {
        base.Start();
        player = GetComponent<Player>();
        
        if (enableInputAtStart)
            EnableInput();

        if(lockCursor)
            Cursor.lockState = CursorLockMode.Locked;
    }

    public void EnableInput()
    {
        aimInputEnabled = true;
    }
    public void DisableInput()
    {
        aimInputEnabled = false;
    }

    void OnAim(InputEventInfo _inputEventInfo)
    {
        if (!GameManager.instance.allowGameInput || _inputEventInfo.inputState == InputState.Released)
        {
            aimInput = Vector3.zero;
            return;
        }

        aimInput = _inputEventInfo.dualAxisValue;

        float _minimumSensitivity = 1.25f;
        float _aimSpeed = (Options.playerControls[player.index].sensitivity + _minimumSensitivity) * 2;
        float _maxAimSpeed = (Options.playerControls[player.index].sensitivity + _minimumSensitivity) * 4;
        float _aimAcceleration = Options.playerControls[player.index].sensitivity + _minimumSensitivity;

        if (_inputEventInfo.inputType == InputMethod.InputType.GamepadStick)
        {
            float _newInputMagnitude = sensitivityCurve.Evaluate(aimInput.magnitude);
            aimInput = aimInput.normalized * _newInputMagnitude;

            //Debug.Log(aimInput);

            if (aimInput.magnitude < 0.9f)
            {
                //Debug.Log("Not Pegged");
                currentAimSpeed = _aimSpeed;
            }
            else
            {
                //Debug.Log("Pegged");
                currentAimSpeed = Mathf.Lerp(currentAimSpeed, _maxAimSpeed, _aimAcceleration * Time.fixedDeltaTime);
            }

            aimInput *= 20;
            aimInput *= currentAimSpeed;
            aimInput.y *= verticalRatio;
            if (Options.playerControls[player.index].aimInversion) aimInput.y *= -1;
        }
        else
        {
            aimInput *= 70;
            aimInput *= _aimSpeed;
        }
    }
    void Update()
    {
        
    }

    void OnDestroy()
    {
        DisableInput();
    }
}
