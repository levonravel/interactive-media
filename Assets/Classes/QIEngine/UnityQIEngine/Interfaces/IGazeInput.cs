using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class IGazeInput
{
    public abstract Vector3 InputPosition { get; set; }
    public abstract float InputSensitivity { get; set; }

    public abstract Transform CustomPointer { get; set; }

    public virtual void Invoke() { }
    public virtual void Quit() { }
}
