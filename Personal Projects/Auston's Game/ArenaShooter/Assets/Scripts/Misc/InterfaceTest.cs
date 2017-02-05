using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof(RectTransform))]
public class InterfaceTest : MonoBehaviour
{
    private RectTransform rootCanvasTransform = null;
    private RectTransform parentRectTransform = null;
    private RectTransform rectTransform = null;

    [HideInInspector] public Vector2 previousParentAnchorMax;
    [HideInInspector] public Vector2 previousParentAnchorMin;

    //[HideInInspector] public Vector2 previousMin;
    //[HideInInspector] public Vector2 previousMax;

    [HideInInspector] public Vector2 previousParentSize;
    public float heightWidthPriority = 0.5f;

    void Reset()
    {
        rectTransform = (RectTransform)transform;
        parentRectTransform = (RectTransform)rectTransform.parent;
        rootCanvasTransform = (RectTransform)rectTransform.root.GetComponentInChildren<Canvas>().transform;

        //previousMin = rectTransform.offsetMin;
        //previousMax = rectTransform.offsetMax;

        previousParentAnchorMax = parentRectTransform.anchorMax;
        previousParentAnchorMin = parentRectTransform.anchorMin;

        previousParentSize = parentRectTransform.sizeDelta;
    }

	void OnGUI()
    {
        if (rectTransform == null)
        {
            Reset();

            if (rectTransform == null)
                return;
        }

        if (gameObject.name == "Image (1)")
            Debug.Log(rectTransform.position);

        if (previousParentAnchorMax != parentRectTransform.anchorMax || previousParentAnchorMin != parentRectTransform.anchorMin)
            Reset();

        //Debug.Log("Offset Min: " + rectTransform.offsetMin + " | Offset Max: " + rectTransform.offsetMax + " | Size Delta: " + rectTransform.sizeDelta);

        //Debug.Log("Parent Offset Min: " + parentRectTransform.offsetMin + " | Parent Offset Max: "
        //    + parentRectTransform.offsetMax + " | Parent Size Delta: " + parentRectTransform.sizeDelta);

        //Debug.Log("Canvas Offset Min: " + rootCanvasTransform.offsetMin + " | Canvas Offset Max: "
        //    + rootCanvasTransform.offsetMax + " | Canvas Size Delta: " + rootCanvasTransform.sizeDelta);

        Vector2 _parentSizeDelta = previousParentSize - parentRectTransform.sizeDelta;

        Vector2 _deltaRatio = Vector2.zero;
        _deltaRatio.x = 1 - (_parentSizeDelta.x / previousParentSize.x);
        _deltaRatio.y = 1 - (_parentSizeDelta.y / previousParentSize.y);

        float _averageDeltaRatio = (heightWidthPriority * _deltaRatio.x) + ((1 - heightWidthPriority) * _deltaRatio.y);

        //Debug.Log(_averageDeltaRatio);

        Vector2 _newSizeDelta = rectTransform.sizeDelta * _averageDeltaRatio;
        
        _newSizeDelta.x = Mathf.Clamp(_newSizeDelta.x, 10f, float.PositiveInfinity);
        _newSizeDelta.y = Mathf.Clamp(_newSizeDelta.y, 10f, float.PositiveInfinity);
        rectTransform.sizeDelta = _newSizeDelta;

        //rectTransform.offsetMin *= _averageDeltaRatio;
        //rectTransform.offsetMax *= _averageDeltaRatio;

        //Vector2 _newOffsetMin = default(Vector2);
        //_newOffsetMin.x = Mathf.Clamp(rectTransform.offsetMin.x, 1f, float.PositiveInfinity);
        //_newOffsetMin.y = Mathf.Clamp(rectTransform.offsetMin.y, 1f, float.PositiveInfinity);

        //Vector2 _newOffsetMax = default(Vector2);
        //_newOffsetMax.x = Mathf.Clamp(rectTransform.offsetMax.x, 1f, float.PositiveInfinity);
        //_newOffsetMax.y = Mathf.Clamp(rectTransform.offsetMax.y, 1f, float.PositiveInfinity);

        //rectTransform.offsetMin = _newOffsetMin;
        //rectTransform.offsetMax = _newOffsetMax;

        //previousMin = rectTransform.offsetMin;
        //previousMax = rectTransform.offsetMax;

        previousParentSize = parentRectTransform.sizeDelta;
    }
	
	// Update is called once per frame
	void Update ()
    {
	
	}
}
