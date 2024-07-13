using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialArcPosition : MonoBehaviour
{
    [Range(50f, 300f)]
    public float Radius = 100.0f; // Radius of the arc around the parent in pixels.
    [Range(0f, 360f)]
    public float StartAngle = 45.0f;
    [Range(0f, 360f)]
    public float EndAngle = 135.0f;
    public Canvas OverlayCanvas; 

    public BoxCollider2D Collider;    

    void Start()
    {
        PositionChildrenInArc();
    }

    void Update()
    {       
        PositionChildrenInArc(); // Reposition children in update to handle parent movement.
    }

    void PositionChildrenInArc()
    {
        int childCount = transform.childCount;        
        if (childCount == 0) return;
       
        Vector2 parentScreenPosition = RectTransformUtility.WorldToScreenPoint(Camera.main, transform.position);
        float angleStep = (EndAngle - StartAngle) / childCount;

        // Adjust the radius when resizing or repositioning.
        Vector3 highlightSize = Collider.size;
        Radius = Mathf.Max(100f,(highlightSize.magnitude) / 2);

        for(int i = 0;i<childCount; i++)
        {
            Transform child = transform.GetChild(i);
                     
            float angle = (StartAngle + i * angleStep) * Mathf.Deg2Rad;

            Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * Radius;

            // Convert the offset to screen space relative to parent's current position.
            Vector2 childScreenPosition = parentScreenPosition + new Vector2(offset.x, offset.y);            
          
            RectTransform childRectTransform = child.GetComponent<RectTransform>();
                        
            childRectTransform.localPosition = childScreenPosition;                       
        }
    }        
}
