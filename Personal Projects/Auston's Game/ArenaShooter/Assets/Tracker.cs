using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(RectTransform))]
public class Tracker : MonoBehaviour
{
    private Player player = null;

    //[Range(-1, 1)] public float screenRatioPositionX;
    //[Range(-1, 1)] public float screenRatioPositionY;

    //private Vector2 directionVector = Vector2.up;

    public float distanceFromScreenEdge = 0.5f;

    public Targeting targeting;
    public Transform targetObject { get { if (targetOverride != null || targeting == null) return targetOverride; return targeting.currentTarget; } }
    public Transform targetOverride = null;

    private new Camera camera = null;
    private Transform camTransform = null;
    private RectTransform rectTransform;

    private Image[] images = null;
    private bool imagesEnabled
    {
        get { return images[0].enabled; }
        set
        {
            for (int i = 0; i < images.Length; i++)
                images[i].enabled = value;
        }
    }

    public Image arrow = null;
    private RectTransform arrowRectTransform = null;

    private Creation creation;

    // Use this for initialization
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        player = rectTransform.root.GetComponent<Player>();

        camera = rectTransform.root.GetComponentInChildren<Camera>();
        camTransform = camera.transform;

        images = GetComponentsInChildren<Image>();
        arrowRectTransform = arrow.rectTransform;
    }

    // Update is called once per frame
    void Update()
    {
        if (targetObject == null)
        {
            imagesEnabled = false;
            return;
        }

        Vector3 _targetPosition = targetObject.transform.position + targetObject.transform.up;
        Vector3 _viewportPosition = camera.WorldToViewportPoint(_targetPosition);

        bool _onScreen = _viewportPosition.x < 1 && _viewportPosition.x > 0 && _viewportPosition.y < 1 && _viewportPosition.y > 0 && _viewportPosition.z > 0;

        Vector3 _vecToTarget = _targetPosition - camTransform.position;
        float _distanceToTarget = _vecToTarget.magnitude;
        if (_onScreen && !Physics.Raycast(camTransform.position, _vecToTarget, _distanceToTarget, 1 << 11, QueryTriggerInteraction.Ignore))
        {
            imagesEnabled = false;
            return;
        }

        imagesEnabled = true;

        Vector2 _directionVector = (Vector2)_viewportPosition - new Vector2(0.5f, 0.5f);
        _directionVector *= 2;

        if (_viewportPosition.z < 0)
            _directionVector *= -1;

        if (!_onScreen)
        {
            _directionVector = _directionVector.normalized;

            float _angleToTarget = Vector3.Angle(Vector3.up, _directionVector);
            Vector3 _cross = Vector3.Cross(Vector3.up, _directionVector);
            if (_cross.z < 0) _angleToTarget *= -1;

            Vector3 _eulers = arrowRectTransform.localEulerAngles;
            arrowRectTransform.localRotation = Quaternion.Euler(_eulers.x, _eulers.y, _angleToTarget);

            arrow.enabled = true;
        }
        else
            arrow.enabled = false;

        float _parentWidth = rectTransform.parent.GetComponent<RectTransform>().rect.width / 2;
        float _parentHeight = rectTransform.parent.GetComponent<RectTransform>().rect.height / 2;

        float _maxRatioPositionX = (_parentWidth - (distanceFromScreenEdge + rectTransform.rect.width / 2)) / _parentWidth;
        float _maxRatioPositionY = (_parentHeight - (distanceFromScreenEdge + rectTransform.rect.height / 2)) / _parentHeight;

        float _screenRatioPositionX = Mathf.Clamp(_onScreen ? _directionVector.x : _directionVector.x * 1.41f, -_maxRatioPositionX, _maxRatioPositionX);
        float _screenRatioPositionY = Mathf.Clamp(_onScreen ? _directionVector.y : _directionVector.y * 1.41f, -_maxRatioPositionY, _maxRatioPositionY);

        Vector2 _newPosition = new Vector2(_parentWidth * _screenRatioPositionX, _parentHeight * _screenRatioPositionY);
        rectTransform.localPosition = _newPosition;
    }
}
