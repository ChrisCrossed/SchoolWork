using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(CameraSettings))]
public class CameraSettingsEditor : Editor
{

    public override void OnInspectorGUI()
    {
        CameraSettings targetComponent = (CameraSettings)target;

        GUIStyle disabledStyle = new GUIStyle(EditorStyles.miniButton);
        GUIStyle enabledStyle = new GUIStyle(EditorStyles.miniButton);
        enabledStyle.normal = enabledStyle.active;

        GUIStyle buttonStyle = null;
        
        if (CameraSettings.settingCameras == null)
            CameraSettings.settingCameras = FindObjectsOfType<ObeysCameraSettings>();

        GUILayout.Space(5);
        GUILayout.BeginHorizontal();

        buttonStyle = CameraSettings.allEnabled ? enabledStyle : disabledStyle;
        if (GUILayout.Button("All Enabled", buttonStyle, GUILayout.Height(23)))
            CameraSettings.allEnabled = true;

        buttonStyle = CameraSettings.allDisabled ? enabledStyle : disabledStyle;
        if (GUILayout.Button("All Disabled", buttonStyle, GUILayout.Height(23)))
            CameraSettings.allDisabled = true;

        GUILayout.EndHorizontal();
        GUILayout.Space(5);
        GUILayout.BeginHorizontal();

        buttonStyle = CameraSettings.bloomEnabled ? enabledStyle : disabledStyle;
        if (GUILayout.Button("Bloom", buttonStyle))
            CameraSettings.bloomEnabled = !CameraSettings.bloomEnabled;

        buttonStyle = CameraSettings.antialiasingEnabled ? enabledStyle : disabledStyle;
        if (GUILayout.Button("Antialiasing", buttonStyle))
            CameraSettings.antialiasingEnabled = !CameraSettings.antialiasingEnabled;

        buttonStyle = CameraSettings.depthOfFieldEnabled ? enabledStyle : disabledStyle;
        if (GUILayout.Button("Depth of Field", buttonStyle))
            CameraSettings.depthOfFieldEnabled = !CameraSettings.depthOfFieldEnabled;

        buttonStyle = CameraSettings.tiltShiftEnabled ? enabledStyle : disabledStyle;
        if (GUILayout.Button("Tilt Shift", buttonStyle))
            CameraSettings.tiltShiftEnabled = !CameraSettings.tiltShiftEnabled;

        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();

        buttonStyle = CameraSettings.ambientOcclusionEnabled ? enabledStyle : disabledStyle;
        if (GUILayout.Button("Ambient Occlusion", buttonStyle))
            CameraSettings.ambientOcclusionEnabled = !CameraSettings.ambientOcclusionEnabled;

        buttonStyle = CameraSettings.contrastEnabled ? enabledStyle : disabledStyle;
        if (GUILayout.Button("Contrast", buttonStyle))
            CameraSettings.contrastEnabled = !CameraSettings.contrastEnabled;

        buttonStyle = CameraSettings.colorCorrectionEnabled ? enabledStyle : disabledStyle;
        if (GUILayout.Button("Color Correction", buttonStyle))
            CameraSettings.colorCorrectionEnabled = !CameraSettings.colorCorrectionEnabled;

        buttonStyle = CameraSettings.edgeDetectionEnabled ? enabledStyle : disabledStyle;
        if (GUILayout.Button("Edge Detection", buttonStyle))
            CameraSettings.edgeDetectionEnabled = !CameraSettings.edgeDetectionEnabled;

        GUILayout.EndHorizontal();
    }
}