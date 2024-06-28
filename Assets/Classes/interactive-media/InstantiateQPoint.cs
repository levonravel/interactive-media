using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateQPoint : MonoBehaviour
{
    public GameObject QPointRefrence;
    public GameObject SquareQPoint;
    public GameObject CircleQPoint;
    public GameObject Canvas;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            var position = Input.mousePosition;
            QPointRefrence.transform.position = position;
            QPointRefrence.SetActive(true);
        }

        if(Input.GetMouseButtonUp(0))
        {
            QPointRefrence.SetActive(false);
        }
    }

    public void InstantiateIQPoint(bool isSquare)
    {
        if(isSquare)
        {
            Instantiate(SquareQPoint, Input.mousePosition, Quaternion.identity, Canvas.transform);
        }
        else
        {
            Instantiate(CircleQPoint, Input.mousePosition, Quaternion.identity, Canvas.transform);
        }
    }
}
