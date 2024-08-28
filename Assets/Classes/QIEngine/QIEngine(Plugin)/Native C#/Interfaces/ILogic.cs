using QuantumInterface.QIEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILogic
{
    public float Weight { get; set; }
    public double Calculate(Node node);
}
