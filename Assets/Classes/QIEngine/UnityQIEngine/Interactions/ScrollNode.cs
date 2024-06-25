using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Quantumation/ScrollNode")]
public class ScrollNode : BaseNodeInteraction
{
    public Transform Scrollbar;
    public Transform ScrollIndicator;
    public UnityQINode CalculationNode;   
    private bool isScrolling;
    private RectTransform scrollbarRect;
    private RectTransform indicatorRect;
    private float parentHeight;
    private float indicatorHeight;

    public override void Start()
    {
        base.Start();
        scrollbarRect = Scrollbar.GetComponent<RectTransform>();
        indicatorRect = ScrollIndicator.GetComponent<RectTransform>();

        if (indicatorRect.pivot.y != 1 || Mathf.Abs(indicatorRect.pivot.x - 0.5f) > 0.01f)
        {
            Debug.LogWarning("Indicator pivot should be set to (0.5, 1) for top center.");
        }

        parentHeight = scrollbarRect.rect.height;
        indicatorHeight = indicatorRect.rect.height;
    }

    public override void OnDeselected()
    {
        isScrolling = false;
    }

    public override void OnSelected()
    {
        isScrolling = true;
    }

    public override void OnQIEngineUpdate()
    {        
        if (isScrolling)
        {
            CalculateRValue(CalculationNode);
            float maxMovement = parentHeight - indicatorHeight; // The total vertical distance available for the indicator to move
            float invertedConfidence = 1 - RValue; // This inverts the confidence
            float newPosition = maxMovement * invertedConfidence; // This will be a value between 0 (top) and maxMovement (bottom)
            indicatorRect.anchoredPosition = new Vector2(0, -newPosition); // Negate the position
        }
    }
}
