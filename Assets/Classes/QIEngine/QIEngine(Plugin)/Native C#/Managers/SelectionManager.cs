
using System.Diagnostics;

public static class SelectionManager
{
    public static Node CurrentSelection;

    public static bool TrySelect(Node node)
    {
        if (CurrentSelection == node) return true;

        //quick out
        if (!node.IsSelectable())
        {
            return false;
        }

        if (CurrentSelection == null)
        {
            //children cannot be selected as a root
            if (node.Parent != null) return false;
            CurrentSelection = node;
            return true;
        }

        if (!CurrentSelection.ContainsNode(node)) return false;

        CurrentSelection = node;

        return true;
    }

    /**
     * @brief Checks if a node meets the requirements to be deselected.
     *
     * The TryDeselect method follows these steps:
     * 1. If the node has selectable children, return false.
     * 2. If CurrentSelection is empty, return true.
     * 3. If CurrentSelection isn't the node, return whether or not the node doesn't have a selectable confidence.
     * 4. If CurrentSelection has a selectable confidence then return false.
     * 5. //TODO: talk to levon about the need for member IsSelectable
     * 6. return false.
     * @param node The node to check for deselect-ability.
     * @return Whether or not the node can be deselected.
     */
    public static bool TryDeselect(Node node)
    {
        if (node.State != State.Selected) return false;

        //quick out nullptr
        if (CurrentSelection == null) return true;

        foreach (var child in node.Children)
        {
            if (child.State == State.Selected) return false;
        }

        if (!node.IsDeselectable()) return false;

        CurrentSelection = node.Parent;
        node.State = State.Deselected;
        return true;
    }

    /**
     * @brief Deselects the current selection and sets CurrentSelection to `nullptr`.
     */
    public static void ForceDeselection()
    {
        if (CurrentSelection == null) return;
        CurrentSelection.State = State.Deselected;
        CurrentSelection.Confidence = 0;
        CurrentSelection.Notifications.OnDeselected?.Invoke();
        CurrentSelection.Notifications.OnConfidenceChanged?.Invoke(0);
        CurrentSelection.Configuration.ResetPositions();
        CurrentSelection = null;
    }

    /**
     * @brief Changes CurrentSelection the address of the node that was passed in.
     * @param node The node to be selected.
     */
    public static void ForceSelection(Node node)
    {
        /*
        Node* wantedNode = node.ParentNode ? node.ParentNode : &node;            
        if (CurrentSelection == wantedNode) return;

        //if we do not adjust the nodes expectedConfidence and leave it hardcoded at 1 the node will automatically deselect
        //I added a .1 confidence padding just in case force selected and then tiny confidence change might occur. -Levon
        wantedNode->ExpectedConfidenceReason = wantedNode-> Confidence - .1f;

        //check if the current node is not null if it isnt we need to also set its state to deselected
        if (CurrentSelection)
        {
            CurrentSelection->State = Deselected;
        }

        CurrentSelection = wantedNode;
        CurrentSelection->State = Selected;
        //fire the selection state notifications this wont work in state handler round robin as the Selection notification happens in Animation state.
        for (auto& select : wantedNode->Notifier.Selected)
        {
            select();
        }
        */
    }
}

