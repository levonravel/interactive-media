using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("Quantumation/GradientChange")]
public class GradientChange : BaseNodeInteraction
{
    public Color HighConfidence;
    public Color OriginalColor;
    public Color CurrentColor;
    public Image sprite;
    public MeshRenderer meshRenderer;

    Gradient gradient = new Gradient();
    GradientColorKey[] colors = new GradientColorKey[2];
    GradientAlphaKey[] alphas = new GradientAlphaKey[2];

    public override void Start()
    {
        base.Start();

        if (GetComponent<Image>())
        {
            sprite = GetComponent<Image>();
            OriginalColor = sprite.color;
        }
        else if (GetComponent<MeshRenderer>())
        {
            meshRenderer = GetComponent<MeshRenderer>();
            OriginalColor = meshRenderer.material.color;
        }

        colors[0] = new GradientColorKey(OriginalColor, 0.0f);
        colors[1] = new GradientColorKey(HighConfidence, 1.0f);
        alphas[0] = new GradientAlphaKey(1f, 0f);  // Full opacity at the start
        alphas[1] = new GradientAlphaKey(1f, 1f);  // Full opacity at the end
    }

    public override void OnQIEngineUpdate()
    {
        CalculateRValue();
        gradient.SetKeys(colors, alphas);
        try
        {
            CurrentColor = gradient.Evaluate((float)Node.Confidence);

            if (sprite != null)
            {
                sprite.color = CurrentColor;
            }

            if (meshRenderer != null)
            {
                meshRenderer.material.color = CurrentColor;
            }
        }
        catch { }
    }

    public override void OnDeselected()
    {
    }

    public override void OnSelected()
    {
    }
}
