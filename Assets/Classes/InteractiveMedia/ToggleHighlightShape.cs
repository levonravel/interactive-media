using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleHighlightShape : MonoBehaviour
{
    public GameObject CircleButton;
    public GameObject SquareButton;

    public Image TargetImage;
    public Sprite SquareHighlight;
    public Sprite CircleHighlight;

    public void ChangeToCircle()
    {        
        CircleButton.SetActive(false);
        SquareButton.SetActive(true);
        TargetImage.sprite = CircleHighlight;
    }

    public void ChangeToSquare()
    {
        CircleButton.SetActive(true);
        SquareButton.SetActive(false);
        TargetImage.sprite = SquareHighlight;
    }  
}
