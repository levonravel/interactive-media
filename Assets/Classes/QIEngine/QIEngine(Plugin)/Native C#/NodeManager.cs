using QuantumInterface.QIEngine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Reflection;

public class NodeManager
{
    Dictionary<CalcType, Action<Node, float, bool>> logicMapper = new Dictionary<CalcType, Action<Node, float, bool>>();
    List<Node> Collection = new List<Node>();
    Queue<int> ReusableIds = new Queue<int>();
    Action<Node, System.Numerics.Vector2> OnCalculateMetricData;
    bool shouldRunEngineCalculations = true;

    public void ResetEngine()
    {
        Collection.Clear();
        ReusableIds.Clear();
        ForceDeselection();
    }

    public NodeManager()
    {
        Type iMetric = typeof(IMetric);

        Type[] metricClasses = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => iMetric.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
            .ToArray();

        for (int i = 0; i < metricClasses.Length; i++)
        {
            OnCalculateMetricData += (Activator.CreateInstance(metricClasses[i]) as IMetric).Calculate;
        }

        //Populate the logicMapper
        Type iLogic = typeof(ILogic);

        Type[] logicClasses = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => iLogic.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
            .ToArray();

        for (int i = 0; i < logicClasses.Length; i++)
        {
            var logicClass = Activator.CreateInstance(logicClasses[i]) as ILogic;
            logicMapper.Add(logicClass.CalcType, logicClass.Calculate);
        }
    }

    /**
     * @brief Creates a new node and assigns it an id.
     * @param dimensions The size of a cubical node.
     * @param position The node's position.
     * @param rotation
     * @param radius The radius of a spherical node.
     * @return The id of the newly created node.
     */
    public int CreateNode(float selectionThreshold, float deselectionThreshold, float holdOpenThreshold, float startConfidenceDistance, Vector3 dimensions, Vector3 position, Quaternion rotation, float radius)
    {
        var newNode = new Node();
        newNode.Configuration.SelectionThreshold = selectionThreshold;
        newNode.Configuration.DeselectionThreshold = deselectionThreshold;
        newNode.Configuration.HoldOpenThreshold = holdOpenThreshold;
        newNode.Configuration.StartConfidenceDistance = startConfidenceDistance;
        newNode.Configuration.Dimensions = new System.Numerics.Vector3(dimensions.X, dimensions.Y, dimensions.Z);
        newNode.Configuration.Position = new System.Numerics.Vector2(position.X, position.Y);
        newNode.Configuration.Radius = radius;

        AssignId(newNode);
        return newNode.Id;
    }

    /**
     * @brief Assigns an id to a given node and adds that node to the collection.
     *
     * The AssignId method takes in a node and assigns its id, making sure to reuse any ids from deleted nodes if they are
     * available. It then places the node in the node collection according to its id.
     * @param node The node to assign an id to and add.
     * @return The assigned id.
     */
    void AssignId(Node node)
    {
        if (ReusableIds.Count > 0)
        {
            node.Id = ReusableIds.Dequeue();
            Collection[node.Id] = node;
        }
        else
        {
            node.Id = (int)Collection.Count;
            Collection.Add(node);
        }
    }

    /**
     * @brief Places a confidence logic into a node's `LogicCalculations`.
     * @param id The id of the node to add the calculation type to.
     * @param type The type of calculation to add.
     * @param weight The calculation weight.
     * @param isLogicFinalizer Whether or not this confidence logic is the final decider in the confidence
     * calculation
     */
    public void AssignConfidenceLogic(int id, CalcType type, float weight, bool isLogicFinalizer)
    {
        Collection[id].LogicCalculations.Add(new CalcContainer(weight, type, isLogicFinalizer));
    }

    /**
     * @brief Get the node at a certain id in the collection.
     * @param id The id of the node to get.
     * @return A reference to the node at the specified id.
     */
    Node GetNodeById(int id)
    {
        return Collection[id];
    }

    /**
     * @brief Add a child to a node.
     * @param rootId The parent node's id.
     * @param childId The new child node's id.
     */
    public void AddChild(int rootId, int childId)
    {
        Node child = Collection[childId];
        child.Parent = Collection[rootId];
        //assign the parent node to the child
        Collection[rootId].Children.Add(Collection[childId]);
    }

    /**
     * @brief Subscribe the node to a new function for selection.
     *
     * The SubscribeSelected method adds a function pointer to the node's Notifier.Selected vector. Whenever the node
     * is selected, this subscribed function will be called.
     * @param id The id of the node to subscribe to the function.
     * @param onSelected The address of the function to subscribe to.
     */
    public void SubscribeSelected(int id, Action onSelected)
    {
        Collection[id].Notifications.OnSelected += onSelected;
    }

    /**
     * @brief Subscribe the node to a new function for deselection.
     *
     * The SubscribeDeselected method adds a function pointer to the node's Notifier.Deselected vector. Whenever the node
     * is deselected, this subscribed function will be called.
     * @param id The id of the node to subscribe to the function.
     * @param onDeselected The address of the function to subscribe to.
     */
    public void SubscribeDeselected(int id, Action onDeselected)
    {
        Collection[id].Notifications.OnDeselected += onDeselected;
    }

    /**
     * @brief Subscribe the node to a new function for animation.
     *
     * The SubscribeAnimating method adds a function pointer to the node's Notifier.Animating vector. Whenever the node
     * is animating, this subscribed function will be called.
     * @param id The id of the node to subscribe to the function.
     * @param onAnimate The address of the function to subscribe to.
     */
    public void SubscribeOnUpdate(int id, Action onUpdate)
    {
        Collection[id].Notifications.OnQIEngineUpdate += onUpdate;
    }

    /**
     * @brief Subscribe the node to a new function for confidence changes.
     *
     * The SubscribeSelected method adds a function pointer to the node's Notifier.ConfidenceChanged vector. Whenever the node's
     * confidence changes, this subscribed function will be called.
     * @param id The id of the node to subscribe to the function.
     * @param onConfidenceChanged The address of the function to subscribe to.
     */
    public void SubscribeConfidenceChange(int id, Action<double> onConfidenceChanged)
    {
        Collection[id].Notifications.OnConfidenceChanged += onConfidenceChanged;
    }

    /**
     * @brief Move the node.
     * @param id The id of the node to move.
     * @param position The new location to position the node at.
     */
    public void UpdateNodePosition(int id, Vector3 position)
    {
        Collection[id].Configuration.Position = new System.Numerics.Vector2(position.X, position.Y);
        UnityEngine.Debug.Log($"Node Position: {position}");
    }

    public void UpdateNodeRotation(int id, Quaternion rotation)
    {
        Collection[id].Configuration.Rotation = new System.Numerics.Quaternion(rotation.W, rotation.X, rotation.Y, rotation.Z);
    }
    /**
     * @brief Grow or shrink the node.
     * @param id The id of the node to dilate.
     * @param radius The new radius of the node.
     */
    public void UpdateNodeDimensions(int id, float width, float height, float length, float radius)
    {
        Node node = Collection[id];
        node.Configuration.Radius = radius;
        node.Configuration.Dimensions = new System.Numerics.Vector3(width, height, length);
    }

    /**
     * @brief Set the screen size.
     * @param screenSize The new screen dimensions.
     */
    void SetScreenSize(Vector3 screenSize)
    {
        QIGlobalData.ScreenSize = new System.Numerics.Vector2(screenSize.X, screenSize.Y);
        QIGlobalData.HeadGazePosition = new System.Numerics.Vector2(screenSize.X / 2, screenSize.Y / 2);
    }

    /**
     * @brief Remove a node from the collection.
     *
     * The RemoveNode method sets the specified node's id to -1 so that any functions that iterate over collection
     * will ignore it, and adds the nodes position to the reusable ids list, so that it can be recycled.
     * @param id The id of the node to remove.
     */
    public void RemoveNode(int id)
    {
        ForceDeselection();
        Node node = Collection[id];
        ReusableIds.Enqueue(node.Id);
        node.Id = -1;
        Collection[id] = null;
    }

    /**
     * @brief Signs up for or removes a node from confidence updates.
     *
     * If shouldCalculate is false, the method will also try to deselect the node.
     * @param id The id of the node to sign up for or remove from confidence updates.
     * @param shouldCalculate Whether or not the node should have its confidence updated.
     */
    public void SetConfidenceUpdates(int id, bool shouldCalculate)
    {
        try
        {
            Node node = Collection[id];
            node.ShouldCalculateConfidence = shouldCalculate;
            if (shouldCalculate) return;
            SelectionManager.TryDeselect(node);
            node.Confidence = 0;
            node.Notifications.OnConfidenceChanged?.Invoke(node.Confidence);
        }catch (Exception ex)
        {

        }
    }

    /**
     * @brief Gets the confidence of a node.
     * @param id The id of the node to get the confidence from.
     * @return The node's confidence.
     */
    public double GetConfidence(int id)
    {
        return Collection[id].Confidence;
    }

    /**
     * @brief Forces the current selection to be deselected, regardless of any condition.
     */
    public void ForceDeselection()
    {
        SelectionManager.ForceDeselection();
    }

    public int GetCurrentSelection()
    {
        if (SelectionManager.CurrentSelection == null) return -1;
        return SelectionManager.CurrentSelection.Id;
    }
    /**
     * @brief Forces the selection of a node, regardless of any condition.
     * @param id The id of the node to select.
     */
    public void ForceSelection(int id)
    {
        SelectionManager.ForceSelection(Collection[id]);
    }

    public void SetEnvironmentVarible(float distanceScale, float screenWidth, float screenHeight, float aspectRation, float fieldOfView, string windingOrder, float maxZetaDistance)
    {
        QIGlobalData.DistanceScale = distanceScale;
        QIGlobalData.ScreenSize.X = screenWidth;
        QIGlobalData.ScreenSize.Y = screenHeight;
        QIGlobalData.AspectRatio = aspectRation;
        QIGlobalData.FieldOfView = fieldOfView;
        QIGlobalData.WindingOrder = windingOrder;
        QIGlobalData.MaxZetaDistance = maxZetaDistance;
    }

    public void UpdateConfidence(float screenPosX, float screenPosY, float worldPosX, float worldPosY)//we do not have head rotation world rotation
    {
        if (!shouldRunEngineCalculations) return;
        
        System.Numerics.Vector2 inputPosition = new System.Numerics.Vector2(screenPosX, screenPosY);

        if (!QIGlobalData.DuplicationFreeGazePositionSamples.GetNewest().Equals(new Vector2(screenPosX, screenPosY)))
        {
            QIGlobalData.DuplicationFreeGazePositionSamples.Enqueue(new System.Numerics.Vector2(screenPosX, screenPosY));
        }

        QIGlobalData.GazePositionSamples.Enqueue(new System.Numerics.Vector2(screenPosX, screenPosY));

        for (int i = Collection.Count - 1; i >= 0; i--)
        {
            var node = Collection[i];
            if (node == null) continue;
            if (node.Id == -1 || !node.ShouldCalculateConfidence) continue;

            OnCalculateMetricData?.Invoke(node, inputPosition);

            //check if we can process any quantumations
            if (!node.ShouldCalculateConfidence) continue;

            //check if this is a child to a root if so dont allow processing if the root isnt selected
            if (node.Parent != null && node.Parent.State != State.Selected) continue;

            foreach (var calc in node.LogicCalculations)
            {
                logicMapper[calc.Type](node, calc.Weight, calc.IsLogicFinalizer);
            }

            //notify update
            node.Notifications.OnQIEngineUpdate?.Invoke();

            if (double.IsNaN(node.Confidence))
            {
                node.Confidence = 0;
                continue;
            }

            //this might be greater than 1 sometimes. 
            node.Confidence = Math.Clamp(node.Confidence, 0, 1);

            //notify the confidence change
            node.Notifications.OnConfidenceChanged?.Invoke(node.Confidence);
            node.Confidence = 0;
        }
    }

    public void ShouldRunCalculations(bool running)
    {
        foreach (var node in Collection)
        {
            if (node == null) continue;
            node.ShouldCalculateConfidence = running;
            node.Confidence = 0;
        }
        shouldRunEngineCalculations = running;
    }
}
