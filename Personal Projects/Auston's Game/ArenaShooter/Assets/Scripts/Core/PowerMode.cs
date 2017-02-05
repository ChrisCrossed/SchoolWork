using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Player))]
[RequireComponent(typeof(Movement))]
public class PowerMode : MonoBehaviour
{
    public enum PowerModes { LowPower, NormalPower, HighPower }
    private Player player;
    private Movement movement;

    private bool lowPowerInputEnabled_ = false;
    public bool lowPowerInputEnabled
    {
        get { return lowPowerInputEnabled_; }
        set
        {
            if (value == lowPowerInputEnabled_)
                return;

            if (value)
                InputEvents.LowPower.Subscribe(OnLowPower, player.index);
            else
                InputEvents.LowPower.Unsubscribe(OnLowPower, player.index);

            lowPowerInputEnabled_ = value;
        }
    }

    private bool highPowerInputEnabled_ = false;
    public bool highPowerInputEnabled
    {
        get { return highPowerInputEnabled_; }
        set
        {
            if (value == highPowerInputEnabled_)
                return;

            if (value)
                InputEvents.HighPower.Subscribe(OnHighPower, player.index);
            else
                InputEvents.HighPower.Unsubscribe(OnHighPower, player.index);

            highPowerInputEnabled_ = value;
        }
    }
    
    private uint powerState = 1;
    public PowerModes powerMode
    {
        get { return (PowerModes)powerState; }
        /*set { powerState = (uint)value; }*/
    }

    public bool inLowPower
    {
        get { return powerState == 0; }
        set
        {
            if (value)
            {
                inHighPower = false;

                powerState = 0;
                movement.speedModifier.AddMultiplier(lowPowerSpeedMultiplier);
                if (powerModeText == null) return;
                powerModeText.text = "Low Power";
                powerModeText.color = GameManager.instance.positiveColor;
                //Debug.Log("Entered LowPower");
            }
            else if (!value && powerState == 0)
            {
                powerState = 1;
                movement.speedModifier.RemoveMultiplier(lowPowerSpeedMultiplier);
                powerModeText.text = "";
                //Debug.Log("Entered NormalPower");
            }
        }
    }
    public bool inHighPower
    {
        get { return powerState == 2; }
        set
        {
            if (value)
            {
                inLowPower = false;

                powerState = 2;
                movement.speedModifier.AddMultiplier(highPowerSpeedMultiplier);
                if (powerModeText == null) return;
                powerModeText.text = "High Power";
                powerModeText.color = GameManager.instance.negativeColor;
                //Debug.Log("Entered HighPower");
            }
            else if (!value && powerState == 2)
            {
                powerState = 1;
                movement.speedModifier.RemoveMultiplier(highPowerSpeedMultiplier);
                if (powerModeText == null) return;
                powerModeText.text = "";
                //Debug.Log("Entered NormalPower");
            }
        }
    }

    [Range(0, 3)] public float lowPowerSpeedMultiplier = 0.5f;
    [Range(0, 3)] public float highPowerSpeedMultiplier = 1.5f;

    public bool toggleLowPower = true;
    public bool toggleHighPower = true;
    public bool autoExitHighPower = false; //auto exits high power if the player comes to a stop

    public bool enableInputAtStart = true;
    public Text powerModeText;
    
    [Range(0,1)]
    public float highExitMagnitude = 0.65f;
    public float highExitTime = 0.25f;
    private float highExitTimer = 0;

    public void EnableInput(bool _lowPowerInput = true, bool _highPowerInput = true)
    {
        if (_lowPowerInput) lowPowerInputEnabled = true;
        if (_highPowerInput) highPowerInputEnabled = true;
    }
    public void DisableInput(bool _lowPowerInput = true, bool _highPowerInput = true)
    {
        if (_lowPowerInput) lowPowerInputEnabled = false;
        if (_highPowerInput) highPowerInputEnabled = false;
    }

    void OnHighPower(InputEventInfo _inputEventInfo)
    {
        if (_inputEventInfo.inputState == InputState.Triggered)
        {
            if (toggleHighPower) inHighPower = !inHighPower;
            else inHighPower = true;
        }
        else
            if (!toggleHighPower) inHighPower = false;
    }
    void OnLowPower(InputEventInfo _inputEventInfo)
    {
        if (_inputEventInfo.inputState == InputState.Triggered)
        {
            if (toggleLowPower) inLowPower = !inLowPower;
            else inLowPower = true;
        }
        else
            if (!toggleLowPower) inLowPower = false;
    }

    void OnJump(InputEventInfo _inputEventInfo)
    {
        if (inLowPower)
            inLowPower = false;
    }

    // Use this for initialization
    void Start ()
    {
        player = GetComponent<Player>();
        movement = GetComponent<Movement>();

        if(enableInputAtStart)
            EnableInput(); ;

        InputEvents.Jump.Subscribe(OnJump, player.index);
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (inHighPower && movement != null && movement.movementInput.magnitude < 0.65f)
        {
            highExitTimer += Time.deltaTime;

            if (highExitTimer > highExitTime)
                inHighPower = false;
        }
        else
            highExitTimer = 0;
    }

    private void OnDestroy()
    {
        DisableInput();
        if (player != null)
            InputEvents.Jump.Unsubscribe(OnJump, player.index);
    }
}
