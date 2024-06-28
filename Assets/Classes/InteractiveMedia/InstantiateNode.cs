using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InstantiateNode : MonoBehaviour
{
    public GameObject BaseNode;
    public GameObject SquareNodeReference;
    public GameObject CircleNodeReference;
    public GameObject Canvas;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var position = Input.mousePosition;
            BaseNode.transform.position = position;
            BaseNode.SetActive(true);
        }

        if (Input.GetMouseButtonUp(0))
        {
            BaseNode.SetActive(false);
        }
    }

    public void InstantiateNewNode(bool isSquare)
    {
        if (isSquare)
        {
            GameObject instantiatedObject = Instantiate(SquareNodeReference, BaseNode.transform.position, Quaternion.identity, Canvas.transform);
            instantiatedObject.SetActive(true);
        }
        else
        {
            GameObject instantiatedObject = Instantiate(CircleNodeReference, BaseNode.transform.position, Quaternion.identity, Canvas.transform);
            instantiatedObject.SetActive(true);
        }
    }
}
