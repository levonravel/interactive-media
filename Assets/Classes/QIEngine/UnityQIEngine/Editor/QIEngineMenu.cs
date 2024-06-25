using UnityEditor;
using UnityEngine;

public class QIEngineMenu : EditorWindow
{
    private QIEngineSettings qiSettings;

    [MenuItem("QIEngine/Settings")]
    private static void ToggleOption()
    {
        GetWindow<QIEngineMenu>("QI Settings");
    }

    private void OnEnable()
    {
        qiSettings = Resources.Load<QIEngineSettings>("QIEngineSettings");

        // Set minimum window size to ensure all elements are visible
        minSize = new Vector2(400f, 400f);


    }

    private void OnGUI()
    {
        EditorGUILayout.Space(); // Add some space between titlebar and content

        // Gaze Providers Section
        DrawGazeProviderSection();

        EditorGUILayout.Space(20f); // Add more space between Gaze Providers and Additional Settings

        // Additional Settings Section
        DrawAdditionalSettingsSection();

        EditorGUILayout.Space(); // Add some space between sections

        // Apply button
        GUILayout.FlexibleSpace(); // Stick to the bottom
        EditorGUILayout.Space(); // Add some space before the button
        if (GUILayout.Button("Apply Settings", GUILayout.Height(30)))
        {
            ApplyDefineSymbols();
            EditorUtility.SetDirty(qiSettings);
        }
    }

    private void DrawGazeProviderSection()
    {
        float originalLabelWidth = EditorGUIUtility.labelWidth;

        GUIStyle sectionLabelStyle = new GUIStyle(EditorStyles.boldLabel);
        sectionLabelStyle.normal.textColor = new Color(0.11f, 0.11f, 0.11f); // Dark grey color

        EditorGUILayout.LabelField("Gaze Providers", sectionLabelStyle);

        EditorGUIUtility.labelWidth = originalLabelWidth;

        qiSettings.HololensProvider = EditorGUILayout.Toggle("Hololens", qiSettings.HololensProvider);
        qiSettings.ViveProvider = EditorGUILayout.Toggle("Vive", qiSettings.ViveProvider);
    }

    private void DrawAdditionalSettingsSection()
    {
        GUIStyle sectionLabelStyle = new GUIStyle(EditorStyles.boldLabel);
        sectionLabelStyle.normal.textColor = new Color(0.11f, 0.11f, 0.11f); // Dark grey color

        EditorGUILayout.LabelField("Additional Settings", sectionLabelStyle);

        float fieldWidth = 300f;

        qiSettings.MaxZetaDistance = EditorGUILayout.FloatField("Max Zeta Distance", qiSettings.MaxZetaDistance, GUILayout.Width(fieldWidth));
        qiSettings.DistanceScale = EditorGUILayout.FloatField("Distance Scale", qiSettings.DistanceScale, GUILayout.Width(fieldWidth));
        qiSettings.InputSensitivity = EditorGUILayout.FloatField("Input Sensitivity", qiSettings.InputSensitivity, GUILayout.Width(fieldWidth));
        qiSettings.InputType = (QIEngineSettings.GazeType)EditorGUILayout.EnumPopup("Gaze Type", qiSettings.InputType, GUILayout.Width(fieldWidth));
        qiSettings.WindingOrder = EditorGUILayout.TextField("Winding Order", qiSettings.WindingOrder, GUILayout.Width(fieldWidth));
        qiSettings.CustomPointer = (Transform)EditorGUILayout.ObjectField("Custom Pointer", qiSettings.CustomPointer, typeof(Transform), true, GUILayout.Width(fieldWidth));
        qiSettings.ShowEyeTracker = EditorGUILayout.Toggle("Show Eye Tracker", qiSettings.ShowEyeTracker, GUILayout.Width(fieldWidth));
    }

    private void ApplyDefineSymbols()
    {
        if (qiSettings.HololensProvider)
        {
            AddDefineSymbol("QIENGINE_HOLOLENS");
        }
        else
        {
            RemoveDefineSymbol("QIENGINE_HOLOLENS");
        }

        if (qiSettings.ViveProvider)
        {
            AddDefineSymbol("QIENGINE_VIVE");
        }
        else
        {
            RemoveDefineSymbol("QIENGINE_VIVE");
        }
    }

    private void AddDefineSymbol(string symbol)
    {
        string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
        if (!defines.Contains(symbol))
        {
            defines += ";" + symbol;
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, defines);
        }
    }

    private void RemoveDefineSymbol(string symbol)
    {
        string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
        defines = defines.Replace(symbol + ";", "").Replace(";" + symbol, "").Replace(symbol, "");
        PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, defines);
    }
}
