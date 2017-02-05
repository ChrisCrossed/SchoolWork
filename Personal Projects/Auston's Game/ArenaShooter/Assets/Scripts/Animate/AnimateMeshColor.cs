using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class AnimateMeshColor : AnimateColor
{
    private MeshRenderer meshRenderer;

    // Use this for initialization
    protected override void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        base.Start();
    }
    protected override void SetStartValue()
    {
        startColor = meshRenderer.material.color;
    }
    protected override void SetColor(Color _newColor)
    {
        meshRenderer.material.color = _newColor;
    }
}
