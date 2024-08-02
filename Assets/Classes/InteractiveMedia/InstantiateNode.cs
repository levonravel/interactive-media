using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InstantiateNode : MonoBehaviour
{
    public GameObject BaseNode;   
    public GameObject CircleNodeReference;
    public GameObject Canvas;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {

            var result = Physics2D.Raycast((Vector2)Input.mousePosition, Vector2.down);            
            if (result.collider != null && result.collider.tag.ToString() == ("ContentHighlight")) return;

            var position = Input.mousePosition;
            BaseNode.transform.position = position;
            BaseNode.SetActive(true);
        } 

        if (Input.GetMouseButtonUp(0))
        {
            BaseNode.SetActive(false);
        }
    }

    public void InstantiateNewHighlight(bool isSquare)
    {            
            GameObject instantiatedObject = Instantiate(CircleNodeReference, BaseNode.transform.position, Quaternion.identity, Canvas.transform);
            instantiatedObject.GetComponent<UnityQINode>().Register(-1);
            instantiatedObject.SetActive(true);        
    }
}
