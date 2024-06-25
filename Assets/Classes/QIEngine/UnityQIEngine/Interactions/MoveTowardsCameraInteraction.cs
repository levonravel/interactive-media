using QuantumInterface.QIEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTowardsCameraInteraction : BaseNodeInteraction
{
    public float Speed;
    public float MaxDistance;
    public bool Is2D;
    public Vector3 WantedPosition;
    public float DeselectionConfidence;
    private bool isSelected;
    public Transform ObjectToMoveTo;
    public bool OnlyZeta;
    private Vector3 priorPosition;

    public void DidMove()
    {
    }

    public override void Start()
    {
        Node = GetComponent<UnityQINode>();
        WantedPosition = transform.position;
        base.Start();
        if (ObjectToMoveTo == null)
        {
            ObjectToMoveTo = QIEngineManager.Instance.Camera.transform;
        }
    }

    public override void OnSelected()
    {
        isSelected = true;
    }

    public override void OnDeselected()
    {
        isSelected = false;
    }

    public override void OnQIEngineUpdate()
    {
    }

    public void Update()
    {
        var confidence = Node.Confidence;

        if (confidence < DeselectionConfidence)
        {
            transform.position = Vector3.Lerp(transform.position, WantedPosition, Time.deltaTime * Speed);
            return;
        }
        Vector3 expectedPosition = OnlyZeta ? new Vector3(transform.position.x, transform.position.y, Camera.main.transform.position.z) : Camera.main.transform.position;
        var distance = Vector3.Distance(transform.position, ObjectToMoveTo.position);
        if (distance < MaxDistance) return;
        transform.position = Vector3.Lerp(transform.position, ObjectToMoveTo.position, Time.deltaTime * Speed);
    }
}
