using QuantumInterface.QIEngine;
using System.Runtime.CompilerServices;
using UnityEngine;

/*
 * Be kind if you inherit this class please put this label above your class name
 * [AddComponentMenu("Quantumation/YourClassName")]
*/
public abstract class BaseNodeInteraction : MonoBehaviour
{
    [HideInInspector]
    public UnityQINode Node;
    [HideInInspector]public float RValue;
    public double RValueModier = 0.1f;

    public virtual void Start()
    {
        if (Node == null)
        {
            try
            {
                Node = GetComponent<UnityQINode>();
            }
            catch
            { 
                Debug.LogError($"Your gameobject {gameObject.name} does not have a UnityQINode but you have an interaction on it.");
            }
        }

        QIEngineInterpreter.SubscribeToCallbacks(Node.Id, null, OnQIEngineUpdate, OnSelected, OnDeselected);
    }

    public virtual void CalculateRValue(UnityQINode node = null)
    {
        if (node == null)
        {
            RValue = Mathf.Lerp(RValue, (float)Node.Confidence, Time.deltaTime * (float)RValueModier);
        }
        else 
        {
            RValue = Mathf.Lerp(RValue, (float)node.Confidence, Time.deltaTime *(float)RValueModier);
        }
    } 
    
    public abstract void OnSelected();
    public abstract void OnDeselected();
    public abstract void OnQIEngineUpdate();
}
