using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HighlightManager : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    // Highlight modifier components.
    public GameObject ResizeHandle;
    public GameObject MoveHandle;
    public GameObject DeleteHighlightButton;
        
    private RectTransform rectTransform;
    private Image highlightImage;

    private bool isResizing = false;
    private bool isMoving = false;
    private bool isSelected = false;

    private Vector2 originalSize;
    private Vector2 originalPosition;
    private Vector2 originalMousePosition;

    // Colors for visual feedback on highlights.
    public Color SelectedColor = new Color(0f, 0f, 200f, 0.3f); // Semi-transparent blue.
    public Color DeselectedColor = new Color(255f, 255f, 0f ,0.3f); // Semi-transparent yellow.

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        highlightImage = GetComponent<Image>();        
        
        highlightImage.color = DeselectedColor; // Initialize the highlight with deselectedColor.        
    }

    void Update()
    {
        // Detect clicks outside of the highlight to deselect the highlight.
        if (isSelected && Input.GetMouseButtonDown(0))
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            List<RaycastResult> raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, raycastResults);

            bool clickedOutside = true;
            foreach (RaycastResult result in raycastResults)
            {
                if (result.gameObject == gameObject || result.gameObject == ResizeHandle.gameObject)
                {
                    clickedOutside = false;
                    break;
                }
            }

            if (clickedOutside)
            {
                Deselect();
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.pointerEnter == ResizeHandle.gameObject && isSelected)
        {
            isResizing = true;
            originalSize = rectTransform.sizeDelta;
            originalMousePosition = eventData.position;
        }
        else if(eventData.pointerEnter == MoveHandle.gameObject && isSelected)
        {
            isMoving = true;
            originalPosition = rectTransform.position;
            originalMousePosition = eventData.position;
        }
        else if (eventData.pointerEnter == gameObject)
        {
            Select();
        }
        else
        {
            Deselect();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isResizing = false;
        isMoving = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Drag to resize.
        if (isResizing)
        {
            Vector2 currentMousePosition = eventData.position;
            Vector2 sizeDelta = currentMousePosition - originalMousePosition;
            rectTransform.sizeDelta = originalSize + new Vector2(sizeDelta.x, -sizeDelta.y);
        }

        // Drag to move.
        if (isMoving)
        {
            Vector2 currentMousePosition = eventData.position;
            Vector2 distanceMoved = currentMousePosition - originalMousePosition;
            rectTransform.position = originalPosition + new Vector2(distanceMoved.x, distanceMoved.y);
        }
    }

    private void Select()
    {
        isSelected = true;
        ToggleModifiers(true);
        highlightImage.color = SelectedColor;        
    }

    private void Deselect()
    {
        isSelected = false;
        ToggleModifiers(false);
        highlightImage.color = DeselectedColor;       
    }

    private void ToggleModifiers(bool toggle)
    {
        DeleteHighlightButton.SetActive(toggle);
        ResizeHandle.SetActive(toggle);
        MoveHandle.SetActive(toggle);        
    }

    public void DeleteHighlight()
    {        
        Destroy(this.gameObject);
    }
}
