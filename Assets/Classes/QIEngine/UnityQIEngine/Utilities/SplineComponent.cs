using UnityEngine;

[System.Serializable]
public class Spline
{
    public Vector3[] Points;

    public Vector3 GetPoint(float t)
    {
        int pointCount = Points.Length;
        int index = Mathf.FloorToInt(t * (pointCount - 1));
        int nextIndex = Mathf.Min(index + 1, pointCount - 1);
        float localT = t * (pointCount - 1) - index;

        return Vector3.Lerp(Points[index], Points[nextIndex], localT);
    }
}

public class SplineComponent : MonoBehaviour
{
    public Spline Spline = new Spline();
    public int CircleSegments = 16;
    public Vector3[] RadiusControlPoints;
    public float TunnelRadius = 1f;
    public int Seed = 42;  // Seed for consistent random generation

    private LineRenderer splineLineRenderer;
    private GameObject[] circleLineRenderers;
    private GameObject[] controlPointMarkers;

    private void Start()
    {
        GenerateSplinePoints();
        InitializeRadiusControlPoints();

        splineLineRenderer = gameObject.AddComponent<LineRenderer>();
        splineLineRenderer.startWidth = 0.1f;
        splineLineRenderer.endWidth = 0.1f;
        splineLineRenderer.material = new Material(Shader.Find("Standard"));
        splineLineRenderer.positionCount = Spline.Points.Length;
        splineLineRenderer.SetPositions(Spline.Points);

        DrawCircles();
        DrawControlPoints();
    }

    private void GenerateSplinePoints()
    {
        Random.InitState(Seed);
        int pointCount = 10;  // Number of spline points to generate
        Spline.Points = new Vector3[pointCount];

        for (int i = 0; i < pointCount; i++)
        {
            Spline.Points[i] = new Vector3(i * 2.0f, Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
        }
    }

    private void InitializeRadiusControlPoints()
    {
        RadiusControlPoints = new Vector3[Spline.Points.Length - 1];
        for (int i = 0; i < RadiusControlPoints.Length; i++)
        {
            RadiusControlPoints[i] = (Spline.Points[i] + Spline.Points[i + 1]) / 2 + Vector3.up * 0.5f;
        }
    }

    private void DrawCircles()
    {
        if (circleLineRenderers != null)
        {
            foreach (var obj in circleLineRenderers)
            {
                Destroy(obj);
            }
        }

        circleLineRenderers = new GameObject[RadiusControlPoints.Length];
        for (int i = 0; i < RadiusControlPoints.Length; i++)
        {
            circleLineRenderers[i] = new GameObject($"CircleRenderer_{i}");
            circleLineRenderers[i].transform.parent = transform;  // Attach to the spline object
            LineRenderer circleRenderer = circleLineRenderers[i].AddComponent<LineRenderer>();
            circleRenderer.startWidth = 0.05f;
            circleRenderer.endWidth = 0.05f;
            circleRenderer.material = new Material(Shader.Find("Standard"));
            circleRenderer.positionCount = CircleSegments + 1;

            Vector3 start = Spline.Points[i];
            Vector3 end = Spline.Points[i + 1];
            Vector3 controlPoint = RadiusControlPoints[i];
            Vector3 midpoint = (start + end) / 2f;

            Vector3 direction = (end - start).normalized;
            Vector3 up = Vector3.up;
            if (Vector3.Dot(up, direction) > 0.999f)
            {
                up = Vector3.right;
            }

            Vector3 right = Vector3.Cross(direction, up).normalized;
            up = Vector3.Cross(right, direction).normalized;

            for (int j = 0; j <= CircleSegments; j++)
            {
                float angle = (j / (float)CircleSegments) * Mathf.PI * 2;
                Vector3 point = midpoint + right * Mathf.Cos(angle) * TunnelRadius + up * Mathf.Sin(angle) * TunnelRadius;
                circleRenderer.SetPosition(j, point);
            }
        }
    }

    private void DrawControlPoints()
    {
        if (controlPointMarkers != null)
        {
            foreach (var obj in controlPointMarkers)
            {
                Destroy(obj);
            }
        }

        controlPointMarkers = new GameObject[Spline.Points.Length];
        for (int i = 0; i < Spline.Points.Length; i++)
        {
            controlPointMarkers[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            controlPointMarkers[i].transform.position = Spline.Points[i];
            controlPointMarkers[i].transform.localScale = Vector3.one * 0.1f;
            controlPointMarkers[i].GetComponent<Renderer>().material.color = Color.black;
            controlPointMarkers[i].transform.parent = transform;  // Attach to the spline object
        }
    }
}
