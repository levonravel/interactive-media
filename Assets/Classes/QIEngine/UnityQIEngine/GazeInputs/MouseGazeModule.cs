using UnityEngine;
using QuantumInterface.QIEngine;
using UnityEngine.UIElements;

public class MouseGazeModule : IGazeInput
{
    private RollingQueue<Vector3> gazePositions = new RollingQueue<Vector3>(4);
    public override Vector3 InputPosition { get; set; }
    public override float InputSensitivity { get; set; }
    public override Transform CustomPointer { get; set; }

    public override void Invoke()
    {
        gazePositions.Enqueue(Input.mousePosition);
        InputPosition = AverageVectors();
        var headPosition = QIEngineManager.Instance.Camera.WorldToScreenPoint(QIEngineManager.Instance.Camera.transform.position); //2d space
        headPosition.z = QIEngineManager.Instance.Camera.transform.position.z;
        QIEngineInterpreter.UpdateConfidence(InputPosition.x, InputPosition.y, headPosition.x, headPosition.y, headPosition.z);
    }

    public override void Quit()
    {
    }

    public Vector3 AverageVectors()
    {
        Vector3 average = Vector3.zero;
        Vector3[] positions = gazePositions.GetArray();

        for (int i = 0; i < positions.Length; i++)
        {
            average += positions[i];
        }

        return average / positions.Length;
    }
}
