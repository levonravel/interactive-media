using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseNode : BaseNodeInteraction
{
    public GameObject NodeToClose;
    public override void OnDeselected()
    {
        NodeToClose.SetActive(false);
    }

    public override void OnQIEngineUpdate()
    {
    }

    public override void OnSelected()
    {
        NodeToClose.SetActive(true);
    }
}
