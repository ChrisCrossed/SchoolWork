using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Menu : MonoBehaviour
{
    public Canvas menuCanvas;
    public MenuObject startObject;

    private MenuObject hoveredObject_;
    private MenuObject hoveredObject
    {
        get { return hoveredObject_; }
        set { SetHoverObject(value); }
    }

    public bool useHierarchyList = true;
    public bool isHorizontal = false;

    public float maxSpeed = 10;
    public float minSpeed = 1;

    [NonSerialized] public Menu previousMenu;

    public float timeToMaxSpeed = 3;
    public AnimationCurve speedCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private float menuTimer = 0; //should be distance or (speed / time)?
    private float timeHeld = 0;
    private Direction currentDirection = Direction.None;

    //private List<MenuObject> menuObjects = null;
    private MenuObject[] menuObjects = null;
    private int objectIndex = 0;

    private static Menu currentMenu;
    public Selectable selectOnPreviousIsNull;

    // Use this for initialization
    void Start()
    {
        if (menuCanvas == null) menuCanvas = GetComponent<Canvas>();
    }

    void OnEnable()
    {
        if(currentMenu != null) currentMenu.enabled = false; //disable the old menu
        currentMenu = this;
        if(menuObjects == null) menuObjects = GetComponentsInChildren<MenuObject>();
        if (hoveredObject == null && menuObjects.Length > 0) hoveredObject = menuObjects[0];

        menuCanvas.enabled = true; //when this component is enabled, the canvas will be too
        InputEvents.MenuDirectional.Subscribe(OnMenuInput);
        InputEvents.MenuSelect.Subscribe(OnMenuSelect);
        InputEvents.MenuPrevious.Subscribe(OnMenuPrevious);
    }
    void OnDisable()
    {
        hoveredObject = null;
        objectIndex = 0;
        menuCanvas.enabled = false; //when this component is disabled, the canvas will be too
        InputEvents.MenuDirectional.Unsubscribe(OnMenuInput);
        InputEvents.MenuSelect.Unsubscribe(OnMenuSelect);
        InputEvents.MenuPrevious.Unsubscribe(OnMenuPrevious);
    }

    void OnMenuInput(InputEventInfo _eventInfo)
    {   
        switch (_eventInfo.inputState)
        {
            case InputState.Triggered:
                OnMenuInputTriggered(_eventInfo.dualAxisValue);
                break;
            case InputState.Active:
                OnMenuInputActive(_eventInfo.dualAxisValue);
                break;
            case InputState.Released:
                OnMenuInputReleased();
                break;
        }
    }
    void OnMenuSelect(InputEventInfo _eventInfo)
    {
        hoveredObject.SelectObject();
        MenuAudio.PlayOnSelect();
    }
    void OnMenuPrevious(InputEventInfo _eventInfo)
    {
        GoToPrevious();
    }

    private void OnMenuInputTriggered(Vector2 _dualAxisValue)
    {
        float _angle = GetAngle(_dualAxisValue, true);
        Direction _direction = DirectionFromAngle(_angle);

        timeHeld = 0;
        menuTimer = 1 / minSpeed;

        MenuObject _nextHoverObject = GetNextHoverObject(_direction);
        if (_nextHoverObject != null)
            SetHoverObject(_nextHoverObject);
        else
            SendAltDirectionInput(_direction);
    }
    private void OnMenuInputActive(Vector2 _dualAxisValue)
    {
        float _angle = GetAngle(_dualAxisValue, true);
        Direction _direction = DirectionFromAngle(_angle);
        float _inputMagnitude = Mathf.Clamp01(_dualAxisValue.magnitude);

        timeHeld += Time.unscaledDeltaTime * _inputMagnitude;
        menuTimer -= Time.unscaledDeltaTime * _inputMagnitude;

        if (menuTimer <= 0)
        {
            menuTimer = 1 / GetCurrentSpeed(timeHeld);

            MenuObject _nextHoverObject = GetNextHoverObject(_direction);
            if (_nextHoverObject != null)
                SetHoverObject(_nextHoverObject);
            else
                SendAltDirectionInput(_direction);
        }
    }

    private void SendAltDirectionInput(Direction _direction)
    {
        if (isHorizontal)
        {
            if (_direction == Direction.Up) hoveredObject.GiveAltDirectional(1);
            else if (_direction == Direction.Down) hoveredObject.GiveAltDirectional(-1);
        }
        else
        {
            if (_direction == Direction.Right) hoveredObject.GiveAltDirectional(1);
            else if (_direction == Direction.Left) hoveredObject.GiveAltDirectional(-1);
        }
    }

    private MenuObject GetNextHoverObject(Direction _direction)
    {
        if (hoveredObject == null) return null;
        MenuObject _override = hoveredObject.GetAdjacentOverride(_direction);
        if (_override != null) return _override;

        if (menuObjects.Length < 2) return null;

        if(isHorizontal)
        {
            if (_direction == Direction.Right) --objectIndex;
            else if (_direction == Direction.Left) ++objectIndex;
            else return null;
        }
        else
        {
            if (_direction == Direction.Up) --objectIndex;
            else if (_direction == Direction.Down) ++objectIndex;
            else return null;
        }

        if (objectIndex > menuObjects.Length - 1) objectIndex -= menuObjects.Length;
        else if (objectIndex < 0) objectIndex += menuObjects.Length;

        return menuObjects[objectIndex];
    }
    private void SetHoverObject(MenuObject _newHoveredObject)
    {
        if(hoveredObject != null) hoveredObject.ExitHover();
        if (_newHoveredObject != null) _newHoveredObject.EnterHover();

        hoveredObject_ = _newHoveredObject;

        MenuAudio.PlayOnHover();
    }
    private float GetCurrentSpeed(float _timeHeld)
    {
        float _speedRange = maxSpeed - minSpeed;
        return (speedCurve.Evaluate(_timeHeld / timeToMaxSpeed) * _speedRange) + minSpeed;
    }
    private void OnMenuInputReleased()
    {
        //menuTimer = 0;
    }

    public static void Clear()
    {
        while (currentMenu != null && currentMenu.previousMenu != null)
            GoToPrevious();

        if(currentMenu != null)
            currentMenu.enabled = false;
        currentMenu = null;
    }
    
    private static Direction DirectionFromAngle(float _angle)
    {
        float _directionAngle = _angle + 45;
        if (_directionAngle > 360) _directionAngle -= 360;
        _directionAngle /= 90;
        _directionAngle += 1;
        _directionAngle = Mathf.Clamp(_directionAngle, 1, 4);

        Direction _direction = (Direction)_directionAngle;
        return _direction;
    }
    static private float GetAngle(Vector2 _inputVector, bool outOf360 = false)
    {
        if (outOf360)
        {
            float _angle = Vector3.Angle(Vector2.down, _inputVector);
            Vector3 _cross = Vector3.Cross(Vector2.down, _inputVector);
            if (_cross.z > 0) _angle = -_angle;

            return _angle + 180;
        }
        else
        {
            float _angle = Vector3.Angle(Vector2.up, _inputVector);
            Vector3 _cross = Vector3.Cross(Vector2.up, _inputVector);
            if (_cross.z > 0) _angle = -_angle;

            return _angle;
        }
    }

    internal static void GoTo(Menu menuToGoTo)
    {
        menuToGoTo.previousMenu = currentMenu;
        menuToGoTo.enabled = true; //sets currentMenu in OnEnabled
    }
    internal static void GoToPrevious()
    {
        MenuAudio.PlayOnPrevious();

        if (currentMenu.previousMenu)
        {
            Menu _nextMenu = currentMenu.previousMenu;
            currentMenu.previousMenu = null;
            _nextMenu.enabled = true; //sets currentMenu in OnEnabled (disables previous menu and everything)
        }
        else if (currentMenu.selectOnPreviousIsNull)
            currentMenu.selectOnPreviousIsNull.OnSelected();
    }
}
