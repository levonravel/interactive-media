using QuantumInterface.QIEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalcContainer
{
    public float Weight;
    public CalcType Type;
    public bool IsLogicFinalizer;

    public CalcContainer(float weight, CalcType type, bool isLogicFinalizer)
    {
        Weight = weight;
        Type = type;
        IsLogicFinalizer = isLogicFinalizer;
    }
}
