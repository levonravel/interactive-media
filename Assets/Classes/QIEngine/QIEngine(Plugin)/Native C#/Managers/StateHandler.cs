using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum State
{
    Selected,
    Deselected,
}

public static class StateHandler
{
    public static void SwitchState(Node node)
    {
        //add confidence metric
        node.Metrics.Confidence.Enqueue(node.Confidence);

        switch (node.State)
        {
            case State.Selected:
                if (!CanDeselect(node)) return;
                node.State = State.Deselected;
                node.Notifications.OnDeselected?.Invoke();
                break;

            case State.Deselected:
                if (!CanSelect(node)) return;
                node.State = State.Selected;
                node.Notifications.OnSelected?.Invoke();
                break;
        }
    }

    /**
	 * @brief Checks if a node is selectable.
	 * @param node The node to check.
	 * @return Whether or not the node is selectable.
	 */
    static bool CanSelect(Node node)
    {
        return SelectionManager.TrySelect(node);
    }

    /**
	 * @brief Checks if a node is de-selectable.
	 * @param node The node to check.
	 * @return Whether or not the node is de-selectable.
	 */
    static bool CanDeselect(Node node)
    {
        return SelectionManager.TryDeselect(node);
    }
}
