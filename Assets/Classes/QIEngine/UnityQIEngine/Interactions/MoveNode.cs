using UnityEngine;

public class MoveNode : MonoBehaviour
{
    // public void Update()
    // {
    //     // hack for demo
    //     Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0)); // Center of the screen.
    //     
    //     if (Physics.Raycast(ray, out RaycastHit hit))
    //     {
    //         GameObject viewedObject = hit.transform.gameObject;
    //
    //         Debug.Log("Raycast Hit: " + viewedObject.name);
    //         
    //         if (viewedObject.GetComponent<MoveNode>() == null) return;
    //         //if (viewedObject != gameObject) return;
    //         
    //         Debug.Log("has MoveNode: " + viewedObject.name);
    //         
    //         TOManager.Instance.TryChangeSelection(transform.parent);
    //     }
    // }
}
