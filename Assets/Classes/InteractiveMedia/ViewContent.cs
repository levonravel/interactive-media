using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewContent : MonoBehaviour
{
    public GameObject FullscreenImage;
    private RawImage targetImage;
    private RawImage sourceImage;
    private bool fullscreenIsActive = false;

    private void Start()
    {
        // Retrieve components.
        targetImage = FullscreenImage.GetComponent<RawImage>();
        sourceImage = GetComponent<RawImage>();

        if(sourceImage == null )
        {
            Debug.LogError("Node does not have an image component!");
        }
    }

    public void MaxmimizeContent()
    {

        FullscreenImage.SetActive(true);
        fullscreenIsActive = true;
        if (sourceImage != null)
        {
            Texture sourceTexture = sourceImage.mainTexture;
            targetImage.texture = sourceTexture;
        }                          
    }

    public void CloseContent()
    {
        FullscreenImage.SetActive(false);
        fullscreenIsActive = false;        
    }
}
