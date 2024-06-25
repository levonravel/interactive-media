using UnityEditor;
using UnityEngine;

public class CreateScriptableObject : MonoBehaviour
{
    [MenuItem("Assets/Make Scriptable Object", false, 10)]
    private static void CreateScriptable()
    {
        // Get the selected object in the Project view
        Object selectedObject = Selection.activeObject;

        if (selectedObject != null)
        {
            // Ensure it's a script and inherits from ScriptableObject
            MonoScript script = selectedObject as MonoScript;

            if (script != null)
            {
                System.Type classType = script.GetClass();

                // Ensure it's a serializable class and inherits from ScriptableObject
                if (classType != null && classType.IsSubclassOf(typeof(ScriptableObject)))
                {
                    // Create a ScriptableObject asset based on the class type
                    ScriptableObject asset = ScriptableObject.CreateInstance(classType);
                    string assetPath = AssetDatabase.GetAssetPath(script);
                    assetPath = assetPath.Substring(0, assetPath.LastIndexOf('/')) + "/" + classType.Name + ".asset";
                    AssetDatabase.CreateAsset(asset, assetPath);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    Selection.activeObject = asset;
                    Debug.Log($"ScriptableObject created from class: {classType.Name}");
                }
                else
                {
                    Debug.LogWarning("Selected class is not serializable or does not inherit from ScriptableObject.");
                }
            }
            else
            {
                Debug.LogWarning("Selected object is not a script. Please select a C# script.");
            }
        }
        else
        {
            Debug.LogWarning("No object selected. Please select a C# script.");
        }
    }
}
