using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QuantumInterface.QIEngine;

public class GrowInteraction : BaseNodeInteraction
{
    public GameObject WantedGrowObject;
    private Vector3 currentSize;
    private float multiplier;
    public bool IsInverted;
    public float DampenSpeed;
    public Vector3 Size;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        currentSize = transform.localScale;
    }
    
    public override void OnSelected()
    {
    }

    public override void OnDeselected()
    {
    }

    public void Update()
    {
        var scale = IsInverted == true ? Node.Confidence : 1 - Node.Confidence;
        WantedGrowObject.transform.localScale = Vector3.Lerp(WantedGrowObject.transform.localScale, Size * (float)scale, Time.deltaTime * DampenSpeed);
    }

    public override void OnQIEngineUpdate()
    {
    }
}
