using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AffiliatedObject : MonoBehaviour
{
    private int team_ = -1;
    public int team { get { return team_; } set { team_ = value; if (team < 0) return; color = PlayerManager.instance.playerColors[team]; } }

    private Color color_ = Color.white;
    public Color color
    {
        get { return color_; }
        set { color_ = value; UpdateColors(); }
    }

    private void UpdateColors()
    {
        //print(color);

        MeshRenderer[] _meshRenderers = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer _meshRenderer in _meshRenderers)
        {
            if (_meshRenderer.sharedMaterial == PlayerManager.instance.primaryMaterial)
            {
                Color _newColor = color;

                if (PlayerManager.instance.primaryColorOverride != new Color(1, 0, 1, 1))
                    _newColor = PlayerManager.instance.primaryColorOverride;

                Vector3 _colorOffsetVec = PlayerManager.instance.primaryColorOffset;
                Color _colorOffset = new Color(_colorOffsetVec.x, _colorOffsetVec.y, _colorOffsetVec.z, 0);

                if (PlayerManager.instance.primaryOffsetUsesHSV)
                {
                    float hue; float saturation; float value;
                    Color.RGBToHSV(_newColor, out hue, out saturation, out value);
                    _newColor = new Color(hue, saturation, value, 1);
                    _newColor += _colorOffset;
                    _newColor = Color.HSVToRGB(_newColor.r, _newColor.g, _newColor.b);
                }
                else
                    _newColor += _colorOffset;

                _meshRenderer.material.SetColor("_EmissionColor", _newColor);
            }
            if (_meshRenderer.sharedMaterial == PlayerManager.instance.secondaryMaterial)
            {
                Color _newColor = color;

                if (PlayerManager.instance.secondaryOverride != new Color(1, 0, 1, 1))
                    _newColor = PlayerManager.instance.secondaryOverride;

                Vector3 _colorOffsetVec = PlayerManager.instance.secondaryColorOffset;
                Color _colorOffset = new Color(_colorOffsetVec.x, _colorOffsetVec.y, _colorOffsetVec.z, 0);

                if (PlayerManager.instance.secondaryOffsetUsesHSV)
                {
                    float hue; float saturation; float value;
                    Color.RGBToHSV(_newColor, out hue, out saturation, out value);
                    _newColor = new Color(hue, saturation, value, 1);
                    _newColor += _colorOffset;
                    _newColor = Color.HSVToRGB(_newColor.r, _newColor.g, _newColor.b);
                }
                else
                    _newColor += _colorOffset;

                _meshRenderer.material.SetColor("_EmissionColor", _newColor);
            }
        }
    }

    public static List<AffiliatedObject> activeObjects = new List<AffiliatedObject>();

    // Use this for initialization
    public void Start ()
    {
        team = team;
        color = color;
	}

    public void OnEnable() { activeObjects.Add(this); }
    public void OnDisable() { activeObjects.Remove(this); }
}
