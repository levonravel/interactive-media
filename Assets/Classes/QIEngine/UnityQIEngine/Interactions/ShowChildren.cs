using UnityEngine;

[AddComponentMenu("Quantumation/ShowChildren")]
public class ShowChildren : BaseNodeInteraction
{
    public override void Start()
    {
        base.Start();
        ActivateChildren(false);
    }


    public override void OnDeselected()
    {
        ActivateChildren(false);
    }


    public override void OnSelected()
    {
        ActivateChildren(true);
    }


    private void ActivateChildren(bool a)
    {
        if (Node.Children.Count == 0)
            return;

        foreach (var child in Node.Children)
        {
            child.gameObject.SetActive(a);
        }
    }


    public override void OnQIEngineUpdate()
    {
    }
}