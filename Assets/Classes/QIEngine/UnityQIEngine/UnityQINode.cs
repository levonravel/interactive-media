using QuantumInterface.QIEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnityQINode : MonoBehaviour
{
    [HideInInspector]public int Id = -1;

    [Tooltip("The last logic will do the final calculation")]
    public List<LogicTypeContainer> LogicTypes = new List<LogicTypeContainer>() { new LogicTypeContainer() { CalculationType = CalcType.Distance, Weight = 1 } };
    public float SelectionThreshold = .9F, DeselectionThreshold = .89F, HoldOpenThreshold = .4F, ConfidenceCalculationRange;
    public double Confidence;
    public List<UnityQINode> Children = new List<UnityQINode>();
    private Vector3 position, priorPosition, priorCameraPosition, size, dimensions;
    private Quaternion priorRotation, priorCameraRotation;
    private float radius = 10f;
    private bool isReady, isRadial;
    private Collider collider;
    private Collider2D collider2D;

    public void Start()
    {
        if (Id == -1)
            Register(-1);
    }

    void SetUpCollider()
    {
        size = transform.localScale;
        collider = GetComponent<Collider>();
        collider2D = GetComponent<Collider2D>();

        if (collider != null)
        {
            isRadial = collider is SphereCollider;
        }
        if (collider2D != null)
        {
            isRadial = collider2D is CircleCollider2D;
        }
    }

    void Update()
    {
        if (!isReady || !HasTransformChanged() || IsPositionVisible()) return;

        priorPosition = transform.position;
        priorRotation = transform.rotation;
        priorCameraPosition = QIEngineManager.Instance.Camera.transform.position;
        priorCameraRotation = QIEngineManager.Instance.Camera.transform.rotation;

        SetPixelSize();
        QIEngineInterpreter.UpdateNodeDimensions(Id, dimensions.x, dimensions.y, 0, radius);
        QIEngineInterpreter.UpdateNodeOrientation(Id, position.x, position.y, 0, transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);
        QIEngineInterpreter.SetConfidenceUpdates(Id, true);
    }

    bool HasTransformChanged()
    {
        return transform.position != priorPosition || transform.rotation != priorRotation ||
               QIEngineManager.Instance.Camera.transform.position != priorCameraPosition ||
               QIEngineManager.Instance.Camera.transform.rotation != priorCameraRotation ||
               size != transform.localScale;
    }

    public void Register(int parentId)
    {
        SetUpCollider();        
        SetPixelSize();

        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).GetComponent<UnityQINode>() != null)
            {
                var childNode = transform.GetChild(i).GetComponent<UnityQINode>();
                Children.Add(childNode);
            }
        }

        //register this parent
        var rotation = transform.eulerAngles;
        Id = QIEngineInterpreter.CreateNode(SelectionThreshold, DeselectionThreshold, HoldOpenThreshold, ConfidenceCalculationRange, position.x, position.y, position.z, rotation.x, rotation.y, rotation.z, dimensions.x, dimensions.y, 0, isRadial ? radius : 0);

        if (parentId != -1)
        {
            QIEngineInterpreter.AddChild(parentId, Id);
        }

        FinalizeRegistration();

        if (Children.Count != 0)
        {
            foreach (var child in Children)
            {
                child.Register(Id);
            }
        }
    }

    private void RegisterLogics(int id)
    {
        for (int i = 0; i < LogicTypes.Count; i++)
        {
            if (i == LogicTypes.Count - 1)
            {
                QIEngineInterpreter.AssignConfidenceLogic(id, (int)LogicTypes[i].CalculationType, LogicTypes[i].Weight, true);
            }
            else
            {
                QIEngineInterpreter.AssignConfidenceLogic(id, (int)LogicTypes[i].CalculationType, LogicTypes[i].Weight, false);
            }
        }
    }

    private void FinalizeRegistration()
    {
        isReady = true;
        RegisterLogics(Id);
        QIEngineInterpreter.SubscribeToCallbacks(Id, OnConfidenceChanged, null, null, null);
        QIEngineInterpreter.UpdateNodeDimensions(Id, dimensions.x, dimensions.y, 0, radius);
    }

    public void SetPixelSize()
    {
        if (isRadial)
        {
            if (collider2D == null)
            {
                Vector3 center = collider.bounds.center;
                radius = collider.bounds.size.x / 2;
                Vector2 centerPixel = Camera.main.WorldToScreenPoint(center);
                Vector2 edgePixel = Camera.main.WorldToScreenPoint(center + Vector3.right * radius);
                radius = (edgePixel - centerPixel).magnitude;
                position = QIEngineManager.Instance.Camera.WorldToScreenPoint(collider.bounds.center);
            }
            else
            {
                position = collider2D.bounds.center;
                radius = ((CircleCollider2D)collider2D).radius;
            }
        }
        else
        {
            if(collider2D != null)
            {
                dimensions = new Vector3(((BoxCollider2D) collider2D).size.x, ((BoxCollider2D)collider2D).size.y);
                position = collider2D.bounds.center;
                radius = 0;
                return;
            }

            Bounds bounds = collider.bounds;

            Vector3[] vertices = new Vector3[8];
            Vector2[] screenPoints = new Vector2[8];

            vertices[0] = new Vector3(bounds.min.x, bounds.min.y, bounds.min.z);
            vertices[1] = new Vector3(bounds.min.x, bounds.min.y, bounds.max.z);
            vertices[2] = new Vector3(bounds.min.x, bounds.max.y, bounds.min.z);
            vertices[3] = new Vector3(bounds.min.x, bounds.max.y, bounds.max.z);
            vertices[4] = new Vector3(bounds.max.x, bounds.min.y, bounds.min.z);
            vertices[5] = new Vector3(bounds.max.x, bounds.min.y, bounds.max.z);
            vertices[6] = new Vector3(bounds.max.x, bounds.max.y, bounds.min.z);
            vertices[7] = new Vector3(bounds.max.x, bounds.max.y, bounds.max.z);

            for (int i = 0; i < 8; i++)
            {
                screenPoints[i] = QIEngineManager.Instance.Camera.WorldToScreenPoint(vertices[i]);
            }

            float minX = screenPoints.Min(point => point.x);
            float maxX = screenPoints.Max(point => point.x);
            float minY = screenPoints.Min(point => point.y);
            float maxY = screenPoints.Max(point => point.y);

            float width = maxX - minX;
            float height = maxY - minY;

            dimensions = new Vector3(width, height, 0);
            position = new Vector3((minX + maxX) / 2, (minY + maxY) / 2, 0);
            radius = 0;
        }
    }

    public void OnConfidenceChanged(double confidence)
    {
        Confidence = confidence;
        if (confidence > 1)
        {
            Confidence = 1;
        }
    }

    bool IsPositionVisible()
    {
        Vector3 viewportPoint = QIEngineManager.Instance.Camera.WorldToScreenPoint(transform.position);
        return viewportPoint.x >= 0 && viewportPoint.x <= 1 && viewportPoint.y >= 0 && viewportPoint.y <= 1 && viewportPoint.z > 0;
    }

    public void OnEnable()
    {
        if (Id == -1) return;
        QIEngineInterpreter.SetConfidenceUpdates(Id, true);
    }

    public void OnDisable()
    {
        if (Id == -1) return;
        QIEngineInterpreter.SetConfidenceUpdates(Id, false);
    }

    public void OnDestroy()
    {
        QIEngineInterpreter.RemoveNode(Id);
    }
}
