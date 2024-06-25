using QuantumInterface.QIEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("Quantumation/SelectionColorChange")]
public class SelectionColorChange : BaseNodeInteraction
{
    public Color SelectionColor;
    private Color OriginalColor;
    private MeshRenderer meshRenderer;
    private Image image;

    public override void Start()
    {
        base.Start();
        
        if(GetComponent<MeshRenderer>())
        {
            meshRenderer = GetComponent<MeshRenderer>();
            OriginalColor = meshRenderer.material.color;
        }
        else if(GetComponent<Image>())
        {
            image = GetComponent<Image>();
            OriginalColor = image.color;
        }
    }
    public override void OnQIEngineUpdate()
    {
    }

    public override void OnDeselected()
    {
        if (meshRenderer != null)
            meshRenderer.material.color = OriginalColor;

        if (image != null)
            image.color = OriginalColor;
    }

    public override void OnSelected()
    {
        if(meshRenderer != null)
            meshRenderer.material.color = SelectionColor;

        if(image != null)
            image.color = SelectionColor;


    }
}
