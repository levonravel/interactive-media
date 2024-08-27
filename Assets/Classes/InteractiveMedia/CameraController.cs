using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    public GameObject ContentNodeReference;
    public GameObject HighlightReference;
    public GameObject HighlightContent;
    public Canvas Canvas;
    public GameObject FullscreenImage;

    private void Start()
    {
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OpenCamera);
        }
    }

    private void OpenCamera()
    {
        TakePicture(512);
    }

    private async void RequestPermissionAsynchronously(bool isPicturePermission)
    {
        NativeCamera.Permission permission = await NativeCamera.RequestPermissionAsync(isPicturePermission);
        Debug.Log("Permission result: " + permission);
    }

    private void TakePicture(int maxSize)
    {
        NativeCamera.Permission permission = NativeCamera.TakePicture((path) =>
        {
            Debug.Log("Image path: " + path);
            if (path != null)
            {
                // Create a Texture2D from the captured image
                Texture2D texture = NativeCamera.LoadImageAtPath(path, maxSize);
                if (texture == null)
                {
                    Debug.Log("Couldn't load texture from " + path);
                    return;
                }

                // Assign texture to a temporary quad and destroy it after 5 seconds
                GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
                quad.transform.position = Canvas.transform.position + Canvas.transform.forward * 2.5f;
                quad.transform.forward = Canvas.transform.forward;
                quad.transform.localScale = new Vector3(1f, texture.height / (float)texture.width, 1f);

                quad.transform.SetParent(Canvas.transform);

                Material material = quad.GetComponent<Renderer>().material;
                if (!material.shader.isSupported) // happens when Standard shader is not included in the build
                    material.shader = Shader.Find("Legacy Shaders/Diffuse");

                material.mainTexture = texture;               
                
                CreateContentNode(texture);
            }
        }, maxSize);

        Debug.Log("Permission result: " + permission);
    }

    private void CreateContentNode(Texture _texture)
    {
        GameObject contentNode = Instantiate(ContentNodeReference, HighlightReference.transform.position, Quaternion.identity, HighlightContent.transform);
        RawImage img = (RawImage)contentNode.GetComponent<RawImage>();
        img.texture = _texture;
        contentNode.SetActive(true);
    }   
}
