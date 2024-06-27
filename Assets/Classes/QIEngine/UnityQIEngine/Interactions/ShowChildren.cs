using UnityEngine;

[AddComponentMenu("Quantumation/ShowSelection")]
public class ShowSelection : BaseNodeInteraction
{
    public GameObject Selection;
    public bool UseAsToggle;
    public GameObject[] DeSelect;   // RPM added to act like a ToggleGroup

    public override void Start()
    {
        base.Start();

        if (Selection.gameObject.activeSelf) Selection.gameObject.SetActive(false);
    }

    public override void OnQIEngineUpdate()
    {
        // TODO: figure out why this isn't firing... or should quantimation be on the front end only?
    }

    public override void OnDeselected()
    {
        if (Selection == null) return;
        if (UseAsToggle) return;
        if (Selection.gameObject.activeSelf) Selection.gameObject.SetActive(false);
    }

    public override void OnSelected()
    {
        if (Selection == null) return;

        if (UseAsToggle)
        {
            Selection.gameObject.SetActive(!Selection.gameObject.activeSelf);
            // RPM Turn off and DeSelect objects
            foreach (GameObject go in DeSelect)
            {
                go.SetActive(false);
            }
        }
        else
        {
            if (!Selection.gameObject.activeSelf) Selection.gameObject.SetActive(true);
        }
    }
}