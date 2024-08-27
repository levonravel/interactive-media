using System.Collections.Generic;

public class Node
{
    public int Id;
    public State State = State.Deselected;
    public Metrics Metrics = new Metrics();
    public Configuration Configuration = new Configuration();
    public Notification Notifications = new Notification();
    public Node Parent;
    public List<CalcContainer> LogicCalculations = new List<CalcContainer>();
    public List<Node> Children = new List<Node>();
    public double Confidence;
    public bool ShouldCalculateConfidence;

    /**
    * @brief Checks if the node has a high enough confidence to be selected.
    * @return Whether or not the node has a selectable confidence.
    */
    public bool IsSelectable()
    {
        return Confidence >= Configuration.SelectionThreshold;
    }

    public bool IsDeselectable()
    {
        //check if any children are selectable if so return false
        if (IsChildInteractable()) return false;
        return Confidence <= Configuration.DeselectionThreshold;
    }

    /**
     * @brief Checks if the node contains a specified node as a child.
     *
     * The ContainsNode method loops through every child of the node and checks it against the node that was passed in.
     * It also recursively checks children. Ergo, if the passed in node is a grandchild, great-grandchild, etc, the function
     * will still return true.
     * @param node The potential child to check.
     * @return Whether or not the node contains the other as a child.
     */
    public bool ContainsNode(Node node)
    {
        foreach (var child in Children)
        {
            if (node == child) return true;
        }
        return false;
    }

    /**
     * @brief Checks whether or not one of the node's children has a high enough confidence to be selected.
     *
     * The IsChildSelectable method loops through every child of the node and returns true if that child's prior
     * confidence is greater than a certain value (currently 0.00065). The prior confidence is used because the child node's
     * current confidence might not have been calculated yet.
     * @return Whether the node has a selectable child or not.s            <--- distance .1 |.5                    x 1
     */
    public bool IsChildInteractable()
    {
        foreach (var child in Children)
        {
            if (child.Metrics.Confidence.GetNewest() >= Configuration.HoldOpenThreshold)
            {
                Confidence = 1;
                return true;
            }
        }
        return false;
    }

    //since we do not want to allow processing if its under a parent node then we have to notify the child to deselect as well
    public void DeselectChildren()
    {
        foreach (var child in Children)
        {
            child.State = State.Deselected;
            Notifications.OnDeselected();
        }
    }
}
