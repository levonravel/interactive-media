using QuantumInterface.QIEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraGazeModule : IGazeInput
{
    public override Vector3 InputPosition { get; set; }

    public override float InputSensitivity { get; set; }
    public override Transform CustomPointer { get; set; }

    public override void Invoke()
    {
        Vector3 worldPosition = QIEngineManager.Instance.Camera.transform.position + QIEngineManager.Instance.Camera.transform.forward;
        InputPosition = QIEngineManager.Instance.Camera.WorldToScreenPoint(worldPosition);
        QIEngineInterpreter.UpdateConfidence(InputPosition.x, InputPosition.y, 0, 0, 0);
    }
}
