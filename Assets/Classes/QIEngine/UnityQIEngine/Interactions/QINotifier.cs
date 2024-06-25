using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class QINotifier : BaseNodeInteraction
{
    public UnityEvent Notification;

    public override void OnDeselected()
    {
    }

    public override void OnQIEngineUpdate()
    {
    }

    public override void OnSelected()
    {
        Debug.Log($"GameObject {gameObject.name} Confidence {Node.Confidence}");
        Notification?.Invoke();
    }
}
