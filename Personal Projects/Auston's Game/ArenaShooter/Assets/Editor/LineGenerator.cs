using UnityEngine;
using UnityEditor;
using System.Collections;

[ExecuteInEditMode]
public class LineGenerator : MonoBehaviour
{
    private LineRenderer lineRenderer;

    public Transform startTransform;
    public Transform endTransform;

    public Color startColor = Color.white;
    public Color endColor = Color.white;

    LineGenerator()
    {
        //EditorApplication.update += LineUpdate;
    }

    // Use this for initialization
    void Start ()
    {
	    
	}

    void OnGUI()
    {
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
            startTransform = transform;

            if(lineRenderer == null)
                return;
        }

        lineRenderer.SetColors(startColor, endColor);

        if (startTransform == null || endTransform == null)
            return;

        Vector3[] positions = new Vector3[2];
        positions[0] = startTransform.position;
        positions[1] = endTransform.position;
        lineRenderer.SetPositions(positions);
    }

    void LineUpdate()
    {
        Vector3[] positions = new Vector3[2];
        positions[0] = startTransform.position;
        positions[1] = endTransform.position;
        lineRenderer.SetPositions(positions);
        lineRenderer.SetColors(startColor, endColor);
    }
	
	// Update is called once per frame
	void Update ()
    {
	    
	}
}
