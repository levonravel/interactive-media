using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CustomEditor(typeof(GestureTable))]
public class GestureEnumEditor : Editor
{
    private GestureTable gestureTable;

    private void OnEnable()
    {
        gestureTable = Resources.Load<GestureTable>("GestureTable");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        // Display a save button
        //if (GUILayout.Button("Save"))
        //{
        //    UpdateEnumValues();
        //}
    }

    private void RestoreDictionary()
    {
        UpdateEnumValues();

        //just in case the dictionary lost its values or something went wrong repopulate
        //gestureTable.GestureLookup.Clear();

        // Get all the existing enum values
        var enumValues = System.Enum.GetValues(typeof(GestureTable.GestureType));

        foreach (var item in gestureTable.Poses)
        {
            foreach (var enumItem in enumValues)
            {
                if (enumItem.ToString() == item.PoseName)
                {
                    //gestureTable.GestureLookup.Add((GestureTable.GestureType)enumItem, new List<GestureTable.DistanceIdentifier>());

                    foreach (var acceptor in item.Gestures)
                    {
                        //gestureTable.GestureLookup[(GestureTable.GestureType)enumItem].Add(acceptor);
                    }
                }
            }
        }
    }

    private void UpdateEnumValues()
    {
        if (gestureTable.Poses != null)
        {
            // Get all the existing enum values
            var enumValues = System.Enum.GetValues(typeof(GestureTable.GestureType));
            List<string> existingEnumValues = new List<string>();

            foreach (var value in enumValues)
            {
                existingEnumValues.Add(value.ToString());
            }

            // Check each PoseName, if it's not in the enum, add it
            foreach (var pose in gestureTable.Poses)
            {
                if (!existingEnumValues.Contains(pose.PoseName))
                {
                    AddEnumValue(pose.PoseName);
                }
            }
        }
    }

    private void AddEnumValue(string enumValue)
    {
        string enumName = "GestureTable.GestureType";

        // Split the enum name to get the class name and the enum name separately
        string[] enumNameParts = enumName.Split('.');
        string className = enumNameParts[0];
        string enumSimpleName = enumNameParts[1];

        // Find all scripts in the project
        string[] scriptPaths = AssetDatabase.FindAssets("t:Script")
            .Select(AssetDatabase.GUIDToAssetPath)
            .Where(path => path.EndsWith(".cs"))
            .ToArray();

        // Iterate through all scripts to find the one containing the enum
        foreach (string path in scriptPaths)
        {
            string content = System.IO.File.ReadAllText(path);

            // Check if the script contains the class name
            if (content.Contains(className))
            {
                // Find the line where the enum is defined within the class
                int classIndex = content.IndexOf("class " + className);
                if (classIndex != -1)
                {
                    int enumIndex = content.IndexOf("public enum " + enumSimpleName, classIndex);
                    if (enumIndex != -1)
                    {
                        // Find the closing bracket of the enum
                        int closingBracketIndex = content.IndexOf("}", enumIndex);
                        if (closingBracketIndex != -1)
                        {
                            content = content.Insert(closingBracketIndex, "\n    " + enumValue + ",");
                            System.IO.File.WriteAllText(path, content);

                            // Refresh the asset database to reflect changes
                            AssetDatabase.Refresh();
                            return;
                        }
                    }
                }
            }
        }

        Debug.LogError($"Failed to find the enum {enumName} in any scripts.");
    }
}