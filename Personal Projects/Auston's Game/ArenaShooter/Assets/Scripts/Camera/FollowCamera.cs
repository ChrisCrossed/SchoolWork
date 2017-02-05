using UnityEngine;
using System.Collections;

public class FollowCamera : MonoBehaviour
{
    public LayerMask layersToCastAgainst = ~0;
    public float castRadius = 0.5f;
    public float targetDistance = 5;
    public float heightOffset = 1;
    public float interpolationSpeed = 10;
    public Transform camTransform;

    private AutomaticAiming aiming;

    public AnimationCurve zoomCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public float maxTargetDistance = 50;
    public float minFOV = 20;
    private float timer;
    private float startingFOV;

    private new Camera camera;

    void OnEnable()
    {
        Destroy(GetComponentInChildren<Canvas>().transform.GetChild(0).gameObject);
        camera = camTransform.GetComponent<Camera>();
        startingFOV = camera.fieldOfView;
        aiming = GetComponent<AutomaticAiming>();
    }

    void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;

        RaycastHit _hitInfo;
        if (!Physics.SphereCast(transform.position, castRadius, -transform.forward, out _hitInfo, targetDistance, layersToCastAgainst.value, QueryTriggerInteraction.Ignore))
            _hitInfo.distance = targetDistance;

        if (aiming.targetOverride == null) return;

        float _distanceToTarget = (camTransform.position - aiming.targetOverride.position).magnitude;
        float _distanceRatio = 1 - Mathf.Clamp01(_distanceToTarget / maxTargetDistance);

        float _targetFOV = 10 + (startingFOV - 20) * _distanceRatio;
        camTransform.localPosition = Vector3.Lerp(camTransform.localPosition, new Vector3(0, heightOffset, -_hitInfo.distance), interpolationSpeed * Time.fixedDeltaTime);
        camera.fieldOfView = startingFOV + (_targetFOV - startingFOV) * zoomCurve.Evaluate(timer / 3);
    }
}
