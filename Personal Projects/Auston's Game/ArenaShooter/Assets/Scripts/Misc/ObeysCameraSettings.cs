using UnityEngine;
using UnityStandardAssets.ImageEffects;
using System.Collections;

public class ObeysCameraSettings : MonoBehaviour
{
    public BloomOptimized bloom = null;
    public Antialiasing antialiasing = null;
    public DepthOfField depthOfField = null;
    public TiltShift tiltShift = null;
    public ScreenSpaceAmbientOcclusion ambientOcclusion = null;
    public ContrastStretch contrast = null;
    public ColorCorrectionCurves colorCorrection = null;
    public EdgeDetection edgeDetection = null;

#if UNITY_EDITOR

    public void OnGUI()
    {
        if (UnityEditor.EditorApplication.isPlaying)
            return;

        GetImageEffects();
    }

#endif

    // Use this for initialization
    void Start ()
    {
        GetImageEffects();

        //bloom.enabled = CameraSettings.bloomEnabled;
        //antialiasing.enabled = CameraSettings.antialiasingEnabled;
        //depthOfField.enabled = CameraSettings.depthOfFieldEnabled;
        //tiltShift.enabled = CameraSettings.tiltShiftEnabled;
        //ambientOcclusion.enabled = CameraSettings.ambientOcclusionEnabled;
        //contrast.enabled = CameraSettings.contrastEnabled;
        //colorCorrection.enabled = CameraSettings.colorCorrectionEnabled;
        //edgeDetection.enabled = CameraSettings.edgeDetectionEnabled;
    }

    void GetImageEffects()
    {
        bloom = GetComponent<BloomOptimized>();
        antialiasing = GetComponent<Antialiasing>();
        depthOfField = GetComponent<DepthOfField>();
        tiltShift = GetComponent<TiltShift>();
        ambientOcclusion = GetComponent<ScreenSpaceAmbientOcclusion>();
        contrast = GetComponent<ContrastStretch>();
        colorCorrection = GetComponent<ColorCorrectionCurves>();
        edgeDetection = GetComponent<EdgeDetection>();
    }
	
	// Update is called once per frame
	void Update ()
    {
	
	}
}
