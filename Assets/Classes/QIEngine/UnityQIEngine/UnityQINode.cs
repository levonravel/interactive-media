using QuantumInterface.QIEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnityQINode : MonoBehaviour
{
    public bool ShowPositionDebug;
    public int Id = -1;
    public float Confidence;

    [Tooltip("The last logic will do the final calculation")]
    public List<LogicTypeContainer> LogicTypes = new List<LogicTypeContainer>() { new LogicTypeContainer() { CalculationType = CalcType.Distance, Weight = 1 } };
    public float SelectionThreshold = .9F;
    public float DeselectionThreshold = .89F;
    public float HoldOpenThreshold = .4F;
    public float ConfidenceCalculationRange;
    public Vector3 dimensions = Vector3.zero;
    public bool isConfirmationNode;
    public List<UnityQINode> Children = new List<UnityQINode>();
    public bool IsRadial = true;
    public bool SkipChildCheck;

    private Vector3 position = Vector3.zero;
    private Vector3 priorPosition;
    private Quaternion priorRotation;
    private Vector3 priorCameraPosition;
    private Quaternion priorCameraRotation;
    public float radius = 10f;
    private MeshRenderer meshRenderer;
    private RectTransform rectTransform;
    private bool isReady;
    private BoxCollider collider;
    private bool isVisible;

    public void Start()
    {
        if (Id == -1)
            Register(-1);
    }

    public void Update()
    {
        if (!isReady) return;

        if (!IsPositionVisible(QIEngineManager.Instance.Camera, transform.position))
        {
            if (isVisible)
            {
                QIEngineInterpreter.SetConfidenceUpdates(Id, false);
                isVisible = false;
            }
            return;
        }

        if (!isVisible)
        {
            QIEngineInterpreter.SetConfidenceUpdates(Id, true);
            isVisible = true;
        }

        //check if the object moved
        if (transform.position != priorPosition || transform.rotation != priorRotation || QIEngineManager.Instance.Camera.transform.position != priorCameraPosition || QIEngineManager.Instance.Camera.transform.rotation != priorCameraRotation)
        {
            priorPosition = transform.position;
            priorRotation = transform.rotation;
            priorCameraPosition = QIEngineManager.Instance.Camera.transform.position;
            priorCameraRotation = QIEngineManager.Instance.Camera.transform.rotation;

            //do not adjust will crash the api
            SetPixelSize();
            QIEngineInterpreter.UpdateNodeDimensions(Id, dimensions.x, dimensions.y, 0, radius);
            QIEngineInterpreter.UpdateNodeOrientation(Id, position.x, position.y, 0, transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);
        }
        SetPixelSize();
        QIEngineInterpreter.UpdateNodeDimensions(Id, dimensions.x, dimensions.y, 0, radius);
        QIEngineInterpreter.UpdateNodeOrientation(Id, position.x, position.y, 0, transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);
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

    public void Register(int parentId)
    {
        GetColliderType();
        SetPixelSize();

        for (int i = 0; i < transform.childCount; i++)
        {
            if (SkipChildCheck) continue;
            if (transform.GetChild(i).GetComponent<UnityQINode>() != null)
            {
                var childNode = transform.GetChild(i).GetComponent<UnityQINode>();
                Children.Add(childNode);
            }
        }

        //register this parent
        var rotation = transform.eulerAngles;
        Id = QIEngineInterpreter.CreateNode(SelectionThreshold, DeselectionThreshold, HoldOpenThreshold, ConfidenceCalculationRange, position.x, position.y, position.z, rotation.x, rotation.y, rotation.z, dimensions.x, dimensions.y, 0, IsRadial ? radius : 0);

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

    public void GetColliderType()
    {
        if (IsRadial)
        {
            position = GetPosition(gameObject.transform.position);
        }
        else if (collider = GetComponent<BoxCollider>())
        {
            position = GetPosition(collider.center);
        }
        else if (meshRenderer = GetComponent<MeshRenderer>())
        {
            position = GetPosition(meshRenderer.bounds.center);
        }
        else if (rectTransform = GetComponent<RectTransform>())
        {
            position = GetPosition(rectTransform.rect.center);
        }
        else
        {
            position = GetPosition(gameObject.transform.position);
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
        if (IsRadial)
        {       
            var sphereCollider = GetComponent<SphereCollider>();
            position = GetPosition(gameObject.transform.position);
            Vector3 center = sphereCollider.bounds.center;
            radius = sphereCollider.bounds.size.x / 2;
            Vector2 centerPixel = Camera.main.WorldToScreenPoint(center);
            Vector2 edgePixel = Camera.main.WorldToScreenPoint(center + Vector3.right * radius);
            radius = (edgePixel - centerPixel).magnitude;
        }
        else if (collider)
        {
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
        else if (meshRenderer)
        {
            position = GetPosition(meshRenderer.bounds.center);
        }
        else if (rectTransform)
        {
            position = GetPosition(rectTransform.rect.center);
            dimensions = new Vector3(rectTransform.rect.width, rectTransform.rect.height);
            radius = rectTransform.rect.width / 2;
        }
        else
        {
            position = GetPosition(gameObject.transform.position);
            dimensions = new Vector3(10, 10);
        }
    }

    private Vector3 GetPosition(Vector3 position)
    {
        return QIEngineManager.Instance.Camera.WorldToScreenPoint(position);
    }

    public void OnConfidenceChanged(float confidence)
    {
        Confidence = confidence;
        if (confidence > 1)
        {
            Confidence = 1;
        }
    }

    bool IsPositionVisible(Camera camera, Vector3 position)
    {
        Vector3 viewportPoint = camera.WorldToViewportPoint(position);
        return viewportPoint.x >= 0 && viewportPoint.x <= 1 &&
               viewportPoint.y >= 0 && viewportPoint.y <= 1 &&
               viewportPoint.z > 0;
    }

    public bool AnyChildrenSelected()
    {
        foreach (UnityQINode child in Children)
        {
            if (child.Confidence >= child.SelectionThreshold) return true;
        }

        return false;
    }

    public void OnDestroy()
    {
        QIEngineInterpreter.RemoveNode(Id);
    }
}
