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
    public float RValueModier = 0.1f;

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
            RValue = Mathf.Lerp(RValue, Node.Confidence, Time.deltaTime * RValueModier);
        }
        else 
        {
            RValue = Mathf.Lerp(RValue, node.Confidence, Time.deltaTime * RValueModier);
        }
    } 
    
    public abstract void OnSelected();
    public abstract void OnDeselected();
    public abstract void OnQIEngineUpdate();
}
