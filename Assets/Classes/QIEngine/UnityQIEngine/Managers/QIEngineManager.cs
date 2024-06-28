using System.Collections.Generic;
using QuantumInterface.QIEngine;
using UnityEngine;

public class QIEngineManager : MonoBehaviour
{
    public enum InputType
    {
        Mouse,
        CameraGaze,
#if QIENGINE_HOLOLENS
        Hololens,
#endif
#if QIENGINE_VIVE
        Vive,
#endif
    }
    #region SingletonInstance
    private static QIEngineManager instance;
    public static QIEngineManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<QIEngineManager>();
            }
            return instance;
        }
    }
    #endregion

    public Camera Camera
    {
        get
        {
            if (camera == null)
            {
                UnityEngine.Debug.LogWarning("No camera was assigned, assigning the main for you.");
                camera = Camera.main;
            }
            return camera;
        }

        set
        {
            camera = value;
        }
    }


    [HideInInspector]public QIEngineSettings EngineSettings; 
    [HideInInspector]public bool PauseConfidence;
    public UnityQINode[] SceneNodes;
    public IGazeInput GazeInputModule;
    private Camera camera;
    public bool IsRunning = true;



    public void Awake()
    {
        EngineSettings = Resources.Load<QIEngineSettings>("QIEngineSettings");

        if(Camera == null)
        {
            UnityEngine.Debug.LogWarning("No camera was assigned, assigning the main for you.");
            Camera = Camera.main;
        }

        //Reset the QIEngine just incase of scene changes
        QIEngineInterpreter.ResetEngine();

        SceneNodes = FindObjectsOfType<UnityQINode>(true);

        foreach(var node in SceneNodes)
        {
            if (node.transform.parent != null && node.transform.parent.GetComponent<UnityQINode>()) continue;
            node.Register(-1);
        }

        QIEngineInterpreter.SetEnvironmentVaribles(EngineSettings.DistanceScale, Screen.width, Screen.height, Camera.aspect, Camera.fieldOfView, EngineSettings.WindingOrder, EngineSettings.MaxZetaDistance);
        QualitySettings.vSyncCount = 0;  // VSync must be disabled
        Application.targetFrameRate = 72;

        switch (EngineSettings.InputType)
        {
            case QIEngineSettings.GazeType.Mouse:
                GazeInputModule = new MouseGazeModule();
                break;
            case QIEngineSettings.GazeType.CameraGaze:
                GazeInputModule = new CameraGazeModule();
                break;
#if QIENGINE_HOLOLENS
            case QIEngineSettings.GazeType.Hololens:
                GazeInputModule = new HololensGazeModule();
                GazeInputModule.CustomPointer = EngineSettings.ShowEyeTracker ? EngineSettings.CustomPointer : null;
                break;
#endif
            default:
                break;
        }

        GazeInputModule.InputSensitivity = EngineSettings.InputSensitivity;
    }


    public void CustomerPointerCheck()
    {
        EngineSettings.CustomPointer.gameObject.SetActive(EngineSettings.ShowEyeTracker);
        GazeInputModule.CustomPointer = EngineSettings.ShowEyeTracker ? EngineSettings.CustomPointer : null;
    }
    
    
    void Update()
    {
        if (PauseConfidence) return;
        
        GazeInputModule.Invoke();
    }

    
    public void ForceDeselection()
    {
        QIEngineInterpreter.ForceDeselection();
    }
    
    
    private void OnApplicationQuit()
    {
        IsRunning = false;
        GazeInputModule.Quit();

        //find all nodes and disable them if we dont it will destroy causing an issue with the library
        var nodes = FindObjectsOfType<UnityQINode>(true);
        foreach (var node in nodes)
        {
            node.enabled = false;
        }
    }
}
