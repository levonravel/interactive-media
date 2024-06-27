using QuantumInterface.QIEngine;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;

public interface IMetric
{
    public void Calculate(Node node, Vector2 inputPosition);
}
