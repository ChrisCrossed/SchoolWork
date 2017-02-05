using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShakeObject : MonoBehaviour
{
    [System.Serializable]
    private class ShakeInfo
    {
        public float shakeDuration; //the total shake duration
        public float totalShakeTime; //the total time that we have shaken for so far
        public float shakeTimer; //the time until the next shake
        public float timePerShake; //the time each shake lasts

        public float rollIntensity;
        public float maxShakeIntensity; //the maximum intensity as represented by Y=1 on the shakeCurve
        public AnimationCurve shakeCurve; //the shake cure that defines the intensity over time

        public Vector3 currentShakeOffset;
        public Vector3 targetShakeOffset;

        public float currentRollOffset;
        public float targetRollOffset;

        //public Vector2 recoilIntensity;
        //public float recoilTimer;
        //public float totalRecoilTime;
        //public AnimationCurve recoilCurve;
    }

    private List<ShakeInfo> shakeInfos = new List<ShakeInfo>();

    void Start()
    {
        //Eve
    }

    public void DefaultShake(float _duration)
    {
        Shake(_duration, 0.15f, 30, 5, null);
    }

    public void SimpleShake(float _duration, float _intensity, bool _roll = true, AnimationCurve _shakeCurve = null)
    {
        _intensity *= 100;
        if (_roll)
            Shake(_duration, _intensity / 30000, _intensity * 4, _intensity / 15, _shakeCurve);
        else
            Shake(_duration, _intensity / 30000, _intensity * 4, 0, _shakeCurve);
    }

    public void Shake(float _duration, float _intensity, float _shakeSpeed = 30, float _rollIntensity = 0, AnimationCurve _shakeCurve = null)
    {
        _intensity /= 3;

        if (_shakeCurve == null)
        {
            Keyframe[] _keys = new Keyframe[] { new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0) };
            AnimationCurve _curve = new AnimationCurve(_keys);
            _shakeCurve = _curve;
        }

        ShakeInfo _newShakeInfo = new ShakeInfo();

        _newShakeInfo.shakeDuration = _duration;
        _newShakeInfo.totalShakeTime = 0;
        _newShakeInfo.shakeTimer = 0;

        _newShakeInfo.timePerShake = 1 / _shakeSpeed;
        _newShakeInfo.maxShakeIntensity = _intensity;
        _newShakeInfo.rollIntensity = _rollIntensity;

        _newShakeInfo.shakeCurve = _shakeCurve;

        //string _debugString = _newShakeInfo.shakeCurve.keys[0].time + ", " + _newShakeInfo.shakeCurve.keys[0].value;
        //for (int i = 1; i < _newShakeInfo.shakeCurve.keys.Length; i++)
        //{
        //    _debugString += " | " + _newShakeInfo.shakeCurve.keys[i].time + ", " + _newShakeInfo.shakeCurve.keys[i].value;
        //}
        //Debug.Log(_debugString);

        shakeInfos.Add(_newShakeInfo);
    }

    // Update is called once per frame
    void Update()
    {
        //if (GamePadInput.GetInputTriggered(0, GamePadInput.Button.Y))
        //    SimpleShake(0.5f, 0.5f);

        Vector3 _totalShakeOffset = Vector3.zero;
        float _totalRollOffset = 0;

        for (int i = 0; i < shakeInfos.Count; i++)
        {
            ShakeInfo _shakeInfo = shakeInfos[i];
            _shakeInfo.shakeTimer += Time.deltaTime;
            _shakeInfo.totalShakeTime += Time.deltaTime;

            //Debug.Log(_shakeInfo.totalShakeTime);

            if (_shakeInfo.totalShakeTime > _shakeInfo.shakeDuration)
            {
                _shakeInfo.currentShakeOffset = Vector3.zero;
                shakeInfos.RemoveAt(i);
                --i;
                continue;
            }

            float _shakeRatio = _shakeInfo.totalShakeTime / _shakeInfo.shakeDuration;
            float _currentIntensity = _shakeInfo.maxShakeIntensity * _shakeInfo.shakeCurve.Evaluate(_shakeRatio);
            
            if (_shakeInfo.shakeTimer > _shakeInfo.timePerShake)
            {
                _shakeInfo.shakeTimer -= _shakeInfo.timePerShake;
                if (_shakeInfo.totalShakeTime + _shakeInfo.timePerShake > _shakeInfo.shakeDuration)
                {
                    _shakeInfo.targetShakeOffset = Vector3.zero;
                    _shakeInfo.targetRollOffset = 0;
                }
                else
                {
                    _shakeInfo.targetShakeOffset = Random.onUnitSphere * _currentIntensity;
                    float _currentRollIntensity = _shakeInfo.rollIntensity * _shakeInfo.shakeCurve.Evaluate(_shakeRatio);
                    _shakeInfo.targetRollOffset = Random.Range(-_currentRollIntensity, _currentRollIntensity);
                }
            }

            _shakeInfo.currentShakeOffset = Vector3.Lerp(_shakeInfo.currentShakeOffset, _shakeInfo.targetShakeOffset, _shakeInfo.shakeTimer / _shakeInfo.timePerShake);
            _shakeInfo.currentRollOffset = Mathf.Lerp(_shakeInfo.currentRollOffset, _shakeInfo.targetRollOffset, _shakeInfo.shakeTimer / _shakeInfo.timePerShake);

            _totalShakeOffset += _shakeInfo.currentShakeOffset;
            _totalRollOffset += _shakeInfo.currentRollOffset;
        }

        if (shakeInfos.Count == 0)
            return;

        _totalShakeOffset /= shakeInfos.Count;
        transform.localPosition = _totalShakeOffset;
        transform.localRotation = Quaternion.Euler(0, 0, _totalRollOffset);
    }

    //public void CameraShake(float _duration, float _intensity, float _numberOfShakes = 1, AnimationCurve _curve = null)
    //{
    //    if (_curve == null)
    //        shakeCurve = AnimationCurve.Linear(0, 1, 1, 1);
    //    else
    //        shakeCurve = _curve;

    //    timePerShake = _duration / (_numberOfShakes + 1);
    //    shakeDuration = _duration;
    //    maxShakeIntensity = _intensity;

    //    totalShakeTime = 0;
    //    shakeTimer = 0;
    //}

    //void UpdateCameraShake()
    //{
    //    totalShakeTime += Time.fixedDeltaTime;
    //    shakeTimer += Time.fixedDeltaTime;

    //    if (totalShakeTime > shakeDuration)
    //    {
    //        currentCamShakeOffset = Vector3.zero;
    //        return;
    //    }

    //    float _shakeRatio = totalShakeTime / shakeDuration;
    //    float _currentIntensity = maxShakeIntensity * shakeCurve.Evaluate(_shakeRatio);

    //    if (shakeTimer > timePerShake)
    //    {
    //        shakeTimer -= timePerShake;
    //        if (totalShakeTime + timePerShake > shakeDuration)
    //            targetCamShakeOffset = Vector3.zero;
    //        else
    //            targetCamShakeOffset = Random.onUnitSphere * _currentIntensity;
    //    }

    //    currentCamShakeOffset = Vector3.Lerp(currentCamShakeOffset, targetCamShakeOffset, shakeTimer / timePerShake);
    //}
}
