using UnityEngine;

public class SpatialExplorer : BaseNodeInteraction
{
    public Vector3 parentOffset = new Vector3(2f, 2f, 2f);      // Absolute values for offsets
    public Direction xDirection = Direction.Right;              // Direction for X axis
    public Direction yDirection = Direction.Up;                 // Direction for Y axis
    public Direction zDirection = Direction.Backward;           // Direction for Z axis
    public float radius = 2f;                                   // Radius for spreading children radially
    public SpreadAxis spreadAxis = SpreadAxis.Y;                // Axis to spread children
    public float Speed;

    public enum Direction
    {
        Left,
        Right,
        Up,
        Down,
        Forward,
        Backward
    }

    public enum SpreadAxis
    {
        X,
        Y,
        Z
    }

    private Vector3 originalPosition;

    public override void OnSelected()
    {
        originalPosition = transform.position;
        OffsetAndSpread(transform, 0);
    }

    public override void OnDeselected()
    {
        ResetChildren(transform);
    }

    public override void OnQIEngineUpdate()
    {
    }

    public void Update()
    {
        if(Node.Confidence > .5f)
        {
            transform.position = Vector3.Lerp(transform.position, transform.forward * 5, Time.deltaTime * Speed);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, originalPosition, Time.deltaTime * Speed);
        }
    }
    void OffsetAndSpread(Transform current, int depth)
    {
        if (depth > 0)
        {
            Vector3 offset = CalculateOffset(depth);
            current.position += offset;
        }

        int childCount = current.childCount;
        if (childCount > 0)
        {
            float angleStep = 360f / childCount;
            float angle = 0f;

            for (int i = 0; i < childCount; i++)
            {
                Transform child = current.GetChild(i);

                if (child.childCount > 0)
                {
                    // If the child is also a parent, apply offset
                    OffsetAndSpread(child, depth + 1);
                }
                else
                {
                    // Spread child radially based on selected axis
                    Vector3 childOffset = Vector3.zero;
                    switch (spreadAxis)
                    {
                        case SpreadAxis.X:
                            childOffset = new Vector3(0, Mathf.Sin(angle * Mathf.Deg2Rad) * radius, Mathf.Cos(angle * Mathf.Deg2Rad) * radius);
                            break;
                        case SpreadAxis.Y:
                            childOffset = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad) * radius, 0, Mathf.Sin(angle * Mathf.Deg2Rad) * radius);
                            break;
                        case SpreadAxis.Z:
                            childOffset = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad) * radius, Mathf.Sin(angle * Mathf.Deg2Rad) * radius, 0);
                            break;
                    }

                    child.position = current.position + childOffset;
                    angle += angleStep;
                }
            }
        }
    }

    void ResetChildren(Transform current)
    {
        foreach (Transform child in current)
        {
            child.position = originalPosition;
            if (child.childCount > 0)
            {
                ResetChildren(child);
            }
        }
    }

    Vector3 CalculateOffset(int depth)
    {
        Vector3 offset = Vector3.zero;

        switch (xDirection)
        {
            case Direction.Left:
                offset += Vector3.left * parentOffset.x * depth;
                break;
            case Direction.Right:
                offset += Vector3.right * parentOffset.x * depth;
                break;
            case Direction.Up:
                offset += Vector3.up * parentOffset.x * depth;
                break;
            case Direction.Down:
                offset += Vector3.down * parentOffset.x * depth;
                break;
            case Direction.Forward:
                offset += Vector3.forward * parentOffset.x * depth;
                break;
            case Direction.Backward:
                offset += Vector3.back * parentOffset.x * depth;
                break;
        }

        switch (yDirection)
        {
            case Direction.Left:
                offset += Vector3.left * parentOffset.y * depth;
                break;
            case Direction.Right:
                offset += Vector3.right * parentOffset.y * depth;
                break;
            case Direction.Up:
                offset += Vector3.up * parentOffset.y * depth;
                break;
            case Direction.Down:
                offset += Vector3.down * parentOffset.y * depth;
                break;
            case Direction.Forward:
                offset += Vector3.forward * parentOffset.y * depth;
                break;
            case Direction.Backward:
                offset += Vector3.back * parentOffset.y * depth;
                break;
        }

        switch (zDirection)
        {
            case Direction.Left:
                offset += Vector3.left * parentOffset.z * depth;
                break;
            case Direction.Right:
                offset += Vector3.right * parentOffset.z * depth;
                break;
            case Direction.Up:
                offset += Vector3.up * parentOffset.z * depth;
                break;
            case Direction.Down:
                offset += Vector3.down * parentOffset.z * depth;
                break;
            case Direction.Forward:
                offset += Vector3.forward * parentOffset.z * depth;
                break;
            case Direction.Backward:
                offset += Vector3.back * parentOffset.z * depth;
                break;
        }

        return offset;
    }
}
