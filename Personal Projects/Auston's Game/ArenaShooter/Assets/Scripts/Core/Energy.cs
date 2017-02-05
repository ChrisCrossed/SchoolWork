using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Energy : MonoBehaviour
{
    public int maxEnergy = 3;
    public float[] regenerationRates = new float[0];

    public PowerMode powerMode;
    public LockOn lockOn;
    public Grapple grapple;

    private float exactEnergy_;
    public float exactEnergy
    {
        get { return exactEnergy_; }
        set
        {
            value = Mathf.Clamp(value, 0, maxEnergy);

            if (value == exactEnergy_) //nothing changed
                return;

            //if (value < currentEnergy_) //if the new value is less than the current energy
            //do lose animation (dispatch lose event?)
            //else //if the new value is greater than the current energy
            //do gain animation (dispatch gain event?)

            //dispatch value change event?

            exactEnergy_ = value;
        }
    }
    public float currentEnergyRatio { get { return exactEnergy_ - Mathf.Floor(exactEnergy_); } }
    public int currentEnergy { get { return Mathf.FloorToInt(exactEnergy_); } set { exactEnergy_ = value; } }

    private int energyLastFrame;

    public List<Image> energyImages = new List<Image>();
    public Color activeEnergyColor = new Color(0, 0, 0, 1);
    public Color inactiveEnergyColor = new Color(0, 0, 0, 1);
    public List<Image> backGroundEnergyImages = new List<Image>();
    public Color backgroundEnergyColor = new Color(0, 0, 0, 1);

    public RectTransform imageGroup;

    private bool animating = false;
    public Color activeWarningColor = Color.red;
    public Color inactiveWarningColor = Color.red;
    public Color backgroundWarningColor = Color.red;
    public float warningDuration = 1;
    private float warningTimer = 0;
    public AnimationCurve warningCurve = AnimationCurve.EaseInOut(0, 0, 0, 1);
    public float warningSize = 1.5f;

    public Text warningText;

    // Use this for initialization
    void Start()
    {
        exactEnergy = maxEnergy;
        powerMode = GetComponent<PowerMode>();
        lockOn = GetComponent<LockOn>();
        grapple = GetComponent<Grapple>();

        UpdateImageColors(activeEnergyColor, inactiveEnergyColor, backgroundEnergyColor);
    }

    public bool SpendEnergy(int _amount) //returns whether the energy was able to be spent (if true, was spent)
    {
        if (_amount == 0) return true;

        if (exactEnergy - _amount < 0)
        {
            AnimateWarning();
            return false;
        }

        exactEnergy -= _amount;
        return true;
    }

    public void AnimateWarning(float _duration = -1, float _size = -1)
    {
        if (_duration > 0)
            warningDuration = _duration;

        if (_size > 0)
            warningSize = _size;

        warningTimer = 0;
        animating = true;
    }

    void UpdateImageColors(Color _activeColor, Color _inactiveColor, Color _backgroundColor = default(Color))
    {
        for (int i = 0; i < energyImages.Count; i++)
        {
            if (currentEnergy > i)
            {
                energyImages[i].color = _activeColor;
                energyImages[i].fillAmount = 1;
            }
            else if (currentEnergy == i)
            {
                energyImages[i].color = _inactiveColor;
            }
            else
            {
                energyImages[i].fillAmount = 0;
            }
        }

        if (_backgroundColor != default(Color))
            for (int i = 0; i < backGroundEnergyImages.Count; i++)
                backGroundEnergyImages[i].color = _backgroundColor;
    }

    // Update is called once per frame
    void Update()
    {
        int _regenIndex = Mathf.Clamp(currentEnergy, 0, regenerationRates.Length - 1);

        if (!lockOn.inLockOn)
        {
            if (powerMode != null)// || powerMode.powerMode == PowerMode.PowerModes.NormalPower)
                exactEnergy += regenerationRates[_regenIndex] * Time.deltaTime;
            else if (powerMode.inLowPower)
                exactEnergy += regenerationRates[_regenIndex] * 2 * Time.deltaTime;
        }
        //else if(powerMode.inHighPower)
        //    exactEnergy -= regenerationRate * 0.5f * Time.deltaTime;
        //Debug.Log(exactEnergy);

        if (animating)
        {
            warningTimer += Time.deltaTime;

            float _warningRatio = warningTimer / warningDuration;

            if (_warningRatio > 1)
            {
                animating = false;
                _warningRatio = 1;
            }

            _warningRatio = warningCurve.Evaluate(_warningRatio);

            Color newActiveColor = activeEnergyColor + (activeWarningColor - activeEnergyColor) * _warningRatio;
            Color newInctiveColor = inactiveEnergyColor + (inactiveWarningColor - inactiveEnergyColor) * _warningRatio;
            Color newBackgroundColor = backgroundEnergyColor + (backgroundWarningColor - backgroundEnergyColor) * _warningRatio;

            float _size = 1 + (warningSize - 1) * _warningRatio;
            imageGroup.localScale = new Vector3(_size, _size, _size);

            if (warningText != null)
                warningText.color = new Color(warningText.color.r, warningText.color.g, warningText.color.b, _warningRatio);

            UpdateImageColors(newActiveColor, newInctiveColor, newBackgroundColor);
        }
        else if (currentEnergy != energyLastFrame) //if the state changed, we need to update the energy colors
            UpdateImageColors(activeEnergyColor, inactiveEnergyColor);

        if (currentEnergy < energyImages.Count)
            energyImages[currentEnergy].fillAmount = currentEnergyRatio;

        energyLastFrame = currentEnergy;
    }
}
