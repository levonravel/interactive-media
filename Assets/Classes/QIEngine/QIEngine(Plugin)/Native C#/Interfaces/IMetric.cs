using QuantumInterface.QIEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMetric
{
    public void Calculate(Node node, Vector3 inputPosition);
}
