using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cs_VisualDisplay : MonoBehaviour
{
    // Reference: https://www.youtube.com/watch?v=rQG9aUWarwE
    // Part 2: https://www.youtube.com/watch?v=73Dc5JTCmKI

    [Range(0, 50)]
    public float f_ViewRadius;
    [Range(0, 361)]
    public float f_ViewAngle;
    [Range(0, 360)]
    public float f_StartAngle;

    [SerializeField] float f_StartRadius = 1.0f;

    public LayerMask i_LayerMask;
    public LayerMask i_ObstacleMask;

    [Range(0, 1)]
    public float f_MeshResolution;
    public int i_EdgeResolveIterations;
    public float f_EdgeDistanceThreshhold;

    public MeshFilter viewMeshFilter;
    Mesh viewMesh;

    public struct ViewCastInfo
    {
        public bool hit;
        public Vector3 point;
        public float dist;
        public float angle;

        public ViewCastInfo(bool _hit, Vector3 _point, float _dist, float _angle)
        {
            hit = _hit;
            point = _point;
            dist = _dist;
            angle = _angle;
        }
    }
    public struct EdgeInfo
    {
        public Vector3 v3_PointA;
        public Vector3 v3_PointB;

        public EdgeInfo( Vector3 _v3_PointA, Vector3 _v3_PointB )
        {
            v3_PointA = _v3_PointA;
            v3_PointB = _v3_PointB;
        }
    }

	// Use this for initialization
	void Start ()
    {
        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;

        i_ObstacleMask = LayerMask.GetMask("Wall", "Player", "Ground");
	}
	
	// Update is called once per frame
	void LateUpdate ()
    {
        DrawFieldOfView();
	}

    void DrawFieldOfView()
    {
        // Ray count
        int i_StepCount = Mathf.RoundToInt((f_ViewAngle + f_StartAngle) * f_MeshResolution);

        float f_StepAngleSize = f_ViewAngle / i_StepCount;

        List<Vector3> v3_ViewPoints = new List<Vector3>();

        ViewCastInfo oldViewCast = new ViewCastInfo();

        for(int i_ = 0; i_ < i_StepCount; ++i_)
        {
            float f_Angle = (transform.eulerAngles.y + f_StartAngle) - f_ViewAngle / 2 + f_StepAngleSize * i_;

            ViewCastInfo newViewCast = ViewCast(f_Angle);

            if (i_ > 0)
            {
                bool b_EdgeDistanceThreshholdExceeded = Mathf.Abs(oldViewCast.dist - newViewCast.dist) > f_EdgeDistanceThreshhold;

                if(oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && b_EdgeDistanceThreshholdExceeded) )
                {
                    EdgeInfo edge = FindEdge(oldViewCast, newViewCast);

                    if(edge.v3_PointA != Vector3.zero)
                    {
                        v3_ViewPoints.Add(edge.v3_PointA);
                    }

                    if (edge.v3_PointB != Vector3.zero)
                    {
                        v3_ViewPoints.Add(edge.v3_PointB);
                    }
                }
            }

            v3_ViewPoints.Add(newViewCast.point);

            oldViewCast = newViewCast;

            // Debug.DrawLine(transform.position, transform.position + Get_DirFromAngle(f_Angle, true) * f_ViewRadius, Color.red);
        }

        int i_VertexCount = v3_ViewPoints.Count + 1;
        Vector3[] vertices = new Vector3[i_VertexCount];
        int[] triangles = new int[(i_VertexCount - 2) * 3];

        vertices[0] = Vector3.zero;

        for (int i_ = 0; i_ < i_VertexCount - 1; ++i_)
        {
            vertices[i_ + 1] = transform.InverseTransformPoint(v3_ViewPoints[i_]);

            if(i_ < i_VertexCount - 2)
            {
                triangles[i_ * 3] = 0;
                triangles[(i_ * 3) + 1] = i_ + 1;
                triangles[(i_ * 3) + 2] = i_ + 2;
            }
        }

        viewMesh.Clear();

        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();
    }

    EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
    {
        float f_MinAngle = minViewCast.angle;
        float f_MaxAngle = maxViewCast.angle;

        Vector3 v3_MinPoint = Vector3.zero;
        Vector3 v3_MaxPoint = Vector3.zero;

        for(int i_ = 0; i_ < i_EdgeResolveIterations; ++i_)
        {
            float f_Angle = (f_MinAngle + f_MaxAngle) / 2;
            ViewCastInfo newViewCast = ViewCast(f_Angle);

            bool b_EdgeDistanceThreshholdExceeded = Mathf.Abs(minViewCast.dist - newViewCast.dist) > f_EdgeDistanceThreshhold;

            if (newViewCast.hit == minViewCast.hit && !b_EdgeDistanceThreshholdExceeded )
            {
                f_MinAngle = f_Angle;
                v3_MinPoint = newViewCast.point;
            }
            else
            {
                f_MaxAngle = f_Angle;
                v3_MaxPoint = newViewCast.point;
            }
        }

        return new EdgeInfo(v3_MinPoint, v3_MaxPoint);
    }

    ViewCastInfo ViewCast( float f_GlobalAngle_ )
    {
        Vector3 dir = Get_DirFromAngle(f_GlobalAngle_, true);
        RaycastHit hit;

        // if(Physics.Raycast( transform.position, dir, out hit, f_ViewRadius, i_ObstacleMask ) )
        if (Physics.Raycast(transform.position, dir, out hit, f_ViewRadius, i_ObstacleMask))
        {
            return new ViewCastInfo(true, hit.point, hit.distance, f_GlobalAngle_);
        }
        else
        {
            return new ViewCastInfo(false, transform.position + dir * f_ViewRadius, f_ViewRadius, f_GlobalAngle_);
        }
    }

    public Vector3 Get_DirFromAngle( float angleInDegrees_, bool b_AngleIsGlobal_ = false)
    {
        if(!b_AngleIsGlobal_)
        {
            angleInDegrees_ += transform.eulerAngles.y;
        }

        // This switch between Sin & Cos is discussed in the first tutorial, due to trig running Sin(90-x) which is the same as Cos(x).
        return new Vector3( Mathf.Sin( angleInDegrees_ * Mathf.Deg2Rad ), 0, Mathf.Cos( angleInDegrees_ * Mathf.Deg2Rad ));
    }

    // Detects objects within the proper radius and refers to them. You already have code that does this elsewhere.
    /*
    void FindVisableTargets()
    {
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, f_ViewRadius, i_LayerMask);

        for(int i_ = 0; i_ < targetsInViewRadius.Length; ++i_)
        {
            Transform target = targetsInViewRadius[i_].transform;

            Vector3 v3_DirToTarget = (target.position - transform.position).normalized;

            if(Vector3.Angle(transform.forward, v3_DirToTarget) < f_ViewAngle / 2)
            {
                float f_Distance = Vector3.Distance(transform.position, target.position);

                if(!Physics.Raycast(transform.position, v3_DirToTarget, i_ObstacleMask))
                {

                }
            }
        }
    }
    */
}
