using System;

public class Notification
{
    public Action<double> OnConfidenceChanged;
    public Action OnQIEngineUpdate;
    public Action OnSelected;
    public Action OnDeselected;

    //Internal notification to calculate node confidence
    public Action<Node, double, bool> GetConfidence;
}
