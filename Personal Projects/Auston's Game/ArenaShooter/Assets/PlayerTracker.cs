using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(RectTransform))]
public class PlayerTracker : MonoBehaviour
{
    private Player player = null;

    //[Range(-1, 1)] public float screenRatioPositionX;
    //[Range(-1, 1)] public float screenRatioPositionY;

    //private Vector2 directionVector = Vector2.up;

    public float distanceFromScreenEdge = 0.5f;

    public Transform targetObject = null;
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
    void Start ()
    {
        rectTransform = GetComponent<RectTransform>();
        player = rectTransform.root.GetComponent<Player>();
        creation = rectTransform.root.GetComponent<Creation>();

        camera = rectTransform.root.GetComponentInChildren<Camera>();
        camTransform = camera.transform;

        images = GetComponentsInChildren<Image>();
        arrowRectTransform = arrow.rectTransform;
    }

    // Update is called once per frame
    void Update ()
    {
        List<Detectable> _detectedObjects = new List<Detectable>();

        targetObject = null;

        for (int i = 0; i < creation.createdObjects.Count; i++)
        {
            if (creation.createdObjects[i] == null)
                creation.createdObjects.RemoveAt(i);
            else
            {
                Transform _detectionTransform = creation.createdObjects[i].transform;
                List<Detectable> _detectables = _detectionTransform.GetComponent<Detection>().detectedObjects;

                if (_detectables == null)
                    break;

                for (int j = 0; j < _detectables.Count; j++)
                {
                    CapsuleCollider _detectionCollider = _detectionTransform.GetComponent<CapsuleCollider>();
                    Vector3 _detectionCenter = Creation.GetCapsuleColliderCenter(_detectionCollider);

                    CapsuleCollider _detectableCollider = _detectables[j].GetComponent<CapsuleCollider>();
                    Vector3 _detectableCenter = Creation.GetCapsuleColliderCenter(_detectableCollider);

                    Vector3 _vecToDetected = _detectableCenter - _detectionCenter;

                    if (Physics.Raycast(_detectionCenter, _vecToDetected, _vecToDetected.magnitude, 1 << 11, QueryTriggerInteraction.Ignore))
                        continue;

                    _detectedObjects.Add(_detectables[j]);
                }
            }
        }

        for (int i = 0; i < _detectedObjects.Count; i++)
        {
            if (_detectedObjects[i] == null ||
                _detectedObjects[i].affiliatedObject.team == player.team ||
                _detectedObjects[i].GetComponent<Player>() == null)
                continue;
            else
            {
                targetObject = _detectedObjects[i].transform;
                break;
            }
        }

        //string _toPrint = "";
        //for (int j = 0; j < _detectedObjects.Count; j++)
        //    _toPrint += _detectedObjects[j].name + ", ";
        //Debug.Log(_toPrint);

        if (targetObject == null)
        {
            imagesEnabled = false;
            return;
        }

        Vector3 _targetPosition = targetObject.transform.position + targetObject.transform.up;
        Vector3 _viewportPosition = camera.WorldToViewportPoint(_targetPosition);

        bool _onScreen = _viewportPosition.x < 1 && _viewportPosition.x > 0 && _viewportPosition.y < 1 && _viewportPosition.y > 0 && _viewportPosition.z > 0;

        //GetComponent<Image>().enabled = !_onScreen;

        //Debug.Log(_onScreen);

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
