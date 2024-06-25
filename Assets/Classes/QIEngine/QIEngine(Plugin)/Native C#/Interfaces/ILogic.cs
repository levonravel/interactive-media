using QuantumInterface.QIEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILogic
{
    public CalcType CalcType { get; }
    public void Calculate(Node node, float weight, bool isStateRunner) { }
}
