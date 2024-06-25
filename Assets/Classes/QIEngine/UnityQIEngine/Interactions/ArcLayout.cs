using UnityEngine;

[AddComponentMenu("Quantumation/ArcLayout")]
public class ArcLayout : BaseNodeInteraction
{
    public float Radius = 3f;
    public float TransitionSpeed = 1f;
    public float StartAngle;
    [Tooltip("Angle between items.  0 - evenly space through 360")]
    public float AngleIncrement = 0f;
    public float ConfidenceThreshold = .5f;
    public Transform ChildStartOffset;
    private Vector3 priorPosition;
    private float modifiedRadius;
    private bool isSelected;

    
    public override void Start()
    {
        base.Start();

        if(ChildStartOffset == null) 
        {
            ChildStartOffset = transform;
        }

        priorPosition = transform.position;
        for(int i = 0; i < Node.Children.Count; i++)
        {
            Node.Children[i].gameObject.SetActive(false);
        }
    }

    public override void OnQIEngineUpdate()
    {
        if (priorPosition != transform.position)
        {
            priorPosition = transform.position;
        }

        modifiedRadius = Radius * .1F;

        CalculateRValue();

        //float angleIncrement = 360f / Node.Children.Count;
        //float angleIncrement = 90f;

        if (Node.Children.Count == 1)
            StartAngle = 0;

        for (int i = 0; i < Node.Children.Count; i++)
        {
            float angle = (i * AngleIncrement + StartAngle) * Mathf.Deg2Rad;
            // Calculate the direction vector without rotation
            Vector3 direction = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * modifiedRadius;
            // Apply the Node's rotation to the direction vector
            direction = Node.transform.rotation * direction; // Assuming 'Node' is a Transform, apply its rotation to the direction
            Vector3 targetPosition = ChildStartOffset.position + direction;

            if (isSelected)
            {
                Node.Children[i].transform.position = Vector3.Lerp(Node.Children[i].transform.position, targetPosition, Time.deltaTime * TransitionSpeed);
            }
            else
            {
                Node.Children[i].transform.position = Vector3.Lerp(Node.Children[i].transform.position, ChildStartOffset.position, Time.deltaTime * TransitionSpeed);
            }
        }
    }


    public override void OnDeselected()
    {
        Debug.Log("Arc Layout OnDeselected");
        for (int i = 0; i < Node.Children.Count; i++)
        {
            Node.Children[i].gameObject.SetActive(false);
        }
        
        isSelected = false;
    }

    
    public override void OnSelected()
    {
        Debug.Log("Arc Layout OnSelected");
        for (int i = 0; i < Node.Children.Count; i++)
        {
            Node.Children[i].gameObject.SetActive(true);
        }
        
        isSelected = true;
    }
}