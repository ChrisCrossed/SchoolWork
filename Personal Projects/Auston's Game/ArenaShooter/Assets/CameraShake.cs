using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XInputDotNetPure;
using UnityStandardAssets.ImageEffects;

[RequireComponent(typeof(ShakeObject))]
public class CameraShake : MonoBehaviour
{
    private static CameraShake[] cameraShakeObjects = new CameraShake[4];
    private Player player;
    private ShakeObject shakeObject;
    private VignetteAndChromaticAberration aberration;

    private class VibrationInfo
    {
        public float duration;
        public float timeRemaining;
        public float leftMaxIntensity;
        public float rightMaxIntensity;
        public AnimationCurve intensityCurve;

        public VibrationInfo(float _duration, float _leftIntensity, float _rightIntensity, AnimationCurve _curve = null)
        {
            if (_curve == null)
                _curve = AnimationCurve.Linear(0, 1, 1, 1);

            duration = _duration;
            timeRemaining = _duration;
            leftMaxIntensity = _leftIntensity;
            rightMaxIntensity = _rightIntensity;
            intensityCurve = _curve;
        }
    }
    private class AberrationInfo
    {
        public float aberrationDuration;
        public float totalAberrationTime;
        public float aberrationTimer;
        public float timePerAberration;
        public float currentAberrationOffset;
        public float targetAberrationOffset;
        public float maxAberrationIntensity;
        public AnimationCurve aberrationCurve;

        public AberrationInfo(float _duration, float _intensity, float _speed, AnimationCurve _curve = null)
        {
            if (_curve == null)
                _curve = AnimationCurve.Linear(0, 1, 1, 1);

            aberrationDuration = _duration;
            totalAberrationTime = 0;
            aberrationTimer = 0;
            timePerAberration = 1 / _speed;
            maxAberrationIntensity = _intensity * 10;
            aberrationCurve = _curve;
        }
    }

    private List<VibrationInfo> vibrationInfos = new List<VibrationInfo>();
    private List<AberrationInfo> aberrationInfos = new List<AberrationInfo>();

    // Use this for initialization
    void Start()
    {
        player = transform.root.GetComponent<Player>();
        shakeObject = GetComponent<ShakeObject>();
        aberration = GetComponent<VignetteAndChromaticAberration>();
        cameraShakeObjects[player.index] = this;

        Events.TogglePause.Subscribe(OnTogglePause);
    }

    private void OnTogglePause()
    {
        GamePad.SetVibration((PlayerIndex)player.index, 0, 0);
    }

    void Update()
    {
        if (Pause.isPaused)
            return;

        UpdateChromaticAberration();

        float totalLeftIntensity = 0;
        float totalRightIntensity = 0;

        for (int i = 0; i < vibrationInfos.Count; i++)
        {
            VibrationInfo _vibrationInfo = vibrationInfos[i];
            _vibrationInfo.timeRemaining -= Time.deltaTime;

            float _completion = (_vibrationInfo.duration - _vibrationInfo.timeRemaining) / _vibrationInfo.duration;
            if(_completion > 1)
            {
                vibrationInfos.RemoveAt(i);
                --i;
                continue;
            }

            totalLeftIntensity += _vibrationInfo.leftMaxIntensity * _vibrationInfo.intensityCurve.Evaluate(_completion);
            totalRightIntensity += _vibrationInfo.rightMaxIntensity * _vibrationInfo.intensityCurve.Evaluate(_completion);
        }

        if (vibrationInfos.Count == 0)
        {
            GamePad.SetVibration((PlayerIndex)player.index, 0, 0);
            return;
        }

        //totalLeftIntensity /= vibrationInfos.Count;
        //totalRightIntensity /= vibrationInfos.Count;

        GamePad.SetVibration((PlayerIndex)player.index, totalLeftIntensity, totalRightIntensity);
    }

    private void UpdateChromaticAberration()
    {
        if (GamePadInput.GetInputTriggered(player.index, GamePadInput.Button.Y))
            AberrateCamera(player.index, 0.45f, 10, 30);

        float _totalAberrationOffset = 0;

        for (int i = 0; i < aberrationInfos.Count; i++)
        {
            AberrationInfo _aberrationInfo = aberrationInfos[i];
            _aberrationInfo.aberrationTimer += Time.deltaTime;
            _aberrationInfo.totalAberrationTime += Time.deltaTime;

            //Debug.Log(_aberrationInfo.totalAberrationTime);

            if (_aberrationInfo.totalAberrationTime > _aberrationInfo.aberrationDuration)
            {
                aberrationInfos.RemoveAt(i);
                --i;
                continue;
            }

            float _aberrationRatio = _aberrationInfo.totalAberrationTime / _aberrationInfo.aberrationDuration;
            float _currentIntensity = _aberrationInfo.maxAberrationIntensity * _aberrationInfo.aberrationCurve.Evaluate(_aberrationRatio);

            if (_aberrationInfo.aberrationTimer > _aberrationInfo.timePerAberration)
            {
                _aberrationInfo.aberrationTimer -= _aberrationInfo.timePerAberration;
                if (_aberrationInfo.totalAberrationTime + _aberrationInfo.timePerAberration > _aberrationInfo.aberrationDuration)
                {
                    _aberrationInfo.targetAberrationOffset = 0;
                }
                else
                {
                    float _currentRollIntensity = _aberrationInfo.maxAberrationIntensity * _aberrationInfo.aberrationCurve.Evaluate(_aberrationRatio);
                    _aberrationInfo.targetAberrationOffset = Random.Range(-_currentRollIntensity, _currentRollIntensity);
                }
            }
            
            _aberrationInfo.currentAberrationOffset = Mathf.Lerp(_aberrationInfo.currentAberrationOffset, _aberrationInfo.targetAberrationOffset, _aberrationInfo.aberrationTimer / _aberrationInfo.timePerAberration);

            _totalAberrationOffset += _aberrationInfo.currentAberrationOffset;
        }

        if (aberrationInfos.Count == 0 || aberration == null)
        {
            aberration.chromaticAberration = 0;
            return;
        }

        _totalAberrationOffset /= aberrationInfos.Count;
        aberration.chromaticAberration = _totalAberrationOffset;
    }

    public static void SimpleShake(int _index, float _duration, float _intensity, bool _roll = true, AnimationCurve _shakeCurve = null)
    {
        if (Get(_index) == null) return;
        Get(_index).shakeObject.SimpleShake(_duration, _intensity, _roll, _shakeCurve);
    }

    public static void Shake(int _index, float _duration, float _intensity, float _shakeSpeed = 30, float _rollIntensity = 0, AnimationCurve _shakeCurve = null)
    {
        if (Get(_index) == null) return;
        Get(_index).shakeObject.Shake(_duration, _intensity, _shakeSpeed, _rollIntensity, _shakeCurve);
    }
    public static void ShakeWithVibration(int _index, float _duration, float _intensity, float _shakeSpeed = 30, float _rollIntensity = 0, AnimationCurve _shakeCurve = null)
    {
        if (Get(_index) == null) return;
        Get(_index).shakeObject.Shake(_duration, _intensity, _shakeSpeed, _rollIntensity, _shakeCurve);
        VibrateGamepad(_index, _duration, _intensity, _shakeCurve);
    }

    public static CameraShake Get(int _index)
    {
        return cameraShakeObjects[_index];
    }

    public static void VibrateGamepad(int _index, float _duration, float _leftIntensity, float _rightIntensity, AnimationCurve _curve = null)
    {
        if (Get(_index) == null) return;
        Get(_index).vibrationInfos.Add(new VibrationInfo(_duration, _leftIntensity, _rightIntensity, _curve));
    }
    public static void VibrateGamepad(int _index, float _duration, float _intensity, AnimationCurve _curve = null)
    {
        if (Get(_index) == null) return;
        Get(_index).vibrationInfos.Add(new VibrationInfo(_duration, _intensity, _intensity, _curve));
    }

    public static void AberrateCamera(int _index, float _duration, float _intensity, float _speed, AnimationCurve _curve = null)
    {
        if (Get(_index) == null) return;
        Get(_index).aberrationInfos.Add(new AberrationInfo(_duration, _intensity, _speed, _curve));
    }

    void OnDestroy()
    {
        if (player != null)
            GamePad.SetVibration((PlayerIndex)player.index, 0, 0);

        if (player != null)
            Events.TogglePause.Unsubscribe(OnTogglePause);
    }
}
