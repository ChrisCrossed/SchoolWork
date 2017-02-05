using UnityEngine;
using System.Collections;

public class CameraSettings : MonoBehaviour
{
    [SerializeField] private bool bloomEnabled_ = true;
    [SerializeField] private bool antialiasingEnabled_ = true;
    [SerializeField] private bool depthOfFieldEnabled_ = true;
    [SerializeField] private bool tiltShiftEnabled_ = true;
    [SerializeField] private bool ambientOcclusionEnabled_ = true;
    [SerializeField] private bool contrastEnabled_ = true;
    [SerializeField] private bool colorCorrectionEnabled_ = true;
    [SerializeField] private bool edgeDetectionEnabled_ = true;

    public static CameraSettings instance = null;
    public static ObeysCameraSettings[] settingCameras = null;
    
    public static bool allEnabled
    {
        get { return bloomEnabled && antialiasingEnabled && depthOfFieldEnabled && tiltShiftEnabled
            && ambientOcclusionEnabled && contrastEnabled && colorCorrectionEnabled && edgeDetectionEnabled; }
        set
        {
            if (value)
            {
                bloomEnabled = true; antialiasingEnabled = true; depthOfFieldEnabled = true; tiltShiftEnabled = true;
                ambientOcclusionEnabled = true; contrastEnabled = true; colorCorrectionEnabled = true; edgeDetectionEnabled = true;
            }
        }
    }
    public static bool allDisabled
    {
        get
        {
            return !(bloomEnabled || antialiasingEnabled || depthOfFieldEnabled || tiltShiftEnabled
          || ambientOcclusionEnabled || contrastEnabled || colorCorrectionEnabled || edgeDetectionEnabled);
        }
        set
        {
            if (value)
            {
                bloomEnabled = false; antialiasingEnabled = false; depthOfFieldEnabled = false; tiltShiftEnabled = false;
                ambientOcclusionEnabled = false; contrastEnabled = false; colorCorrectionEnabled = false; edgeDetectionEnabled = false;
            }
        }
    }

    public static bool bloomEnabled
    {
        get { return instance.bloomEnabled_; }
        set
        {
            foreach (ObeysCameraSettings settingCam in settingCameras)
                settingCam.bloom.enabled = value;

            instance.bloomEnabled_ = value;
        }
    }
    public static bool antialiasingEnabled
    {
        get { return instance.antialiasingEnabled_; }
        set
        {
            foreach (ObeysCameraSettings settingCam in settingCameras)
                settingCam.antialiasing.enabled = value;

            instance.antialiasingEnabled_ = value;
        }
    }
    public static bool depthOfFieldEnabled
    {
        get { return instance.depthOfFieldEnabled_; }
        set
        {
            foreach (ObeysCameraSettings settingCam in settingCameras)
                settingCam.depthOfField.enabled = value;

            instance.depthOfFieldEnabled_ = value;
        }
    }
    public static bool tiltShiftEnabled
    {
        get { return instance.tiltShiftEnabled_; }
        set
        {
            foreach (ObeysCameraSettings settingCam in settingCameras)
                settingCam.tiltShift.enabled = value;

            instance.tiltShiftEnabled_ = value;
        }
    }
    public static bool ambientOcclusionEnabled
    {
        get { return instance.ambientOcclusionEnabled_; }
        set
        {
            foreach (ObeysCameraSettings settingCam in settingCameras)
                settingCam.ambientOcclusion.enabled = value;

            instance.ambientOcclusionEnabled_ = value;
        }
    }
    public static bool contrastEnabled
    {
        get { return instance.contrastEnabled_; }
        set
        {
            foreach (ObeysCameraSettings settingCam in settingCameras)
                settingCam.contrast.enabled = value;

            instance.contrastEnabled_ = value;
        }
    }
    public static bool colorCorrectionEnabled
    {
        get { return instance.colorCorrectionEnabled_; }
        set
        {
            foreach (ObeysCameraSettings settingCam in settingCameras)
                settingCam.colorCorrection.enabled = value;

            instance.colorCorrectionEnabled_ = value;
        }
    }
    public static bool edgeDetectionEnabled
    {
        get { return instance.edgeDetectionEnabled_; }
        set
        {
            foreach (ObeysCameraSettings settingCam in settingCameras)
                settingCam.edgeDetection.enabled = value;

            instance.edgeDetectionEnabled_ = value;
        }
    }

    CameraSettings()
    {
        instance = this;
    }

    // Use this for initialization
    void Start ()
    {
        //bloomEnabled = bloom;
        //antialiasingEnabled = antialiasing;
        //depthOfFieldEnabled = depthOfField;
        //tiltShiftEnabled = tiltShift;
        //ambientOcclusionEnabled = ambientOcclusion;
        //contrastEnabled = contrast;
        //colorCorrectionEnabled = colorCorrection;
        //edgeDetectionEnabled = edgeDetection;
    }
	
	// Update is called once per frame
	void Update ()
    {
	
	}
}
