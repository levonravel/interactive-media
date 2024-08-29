using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;

namespace QuantumInterface.QIEngine
{
    public class QIEngineInterpreter
    {

        //TODO TEMP FIXES REMOVE THIS
        public static NodeManager NodeManager = new NodeManager();
        //TODO REVERT THIS CODE ONCE DLL WORKING IN UWP


        public static void RemoveNode(int id)
        {
            NodeManager.RemoveNode(id);
        }

        public static void AssignConfidenceLogic(int id, int calcType, float weight)
        {
            NodeManager.AssignConfidenceLogic(id, calcType, weight);
        }

        public static void AdjustNodeConfig(int id, int calcType, float weight)
        {
            //not implemented
        }

        public static void ResetEngine()
        {
            NodeManager.ResetEngine();
        }

        public static void AddChild(int parentId, int childId)
        {
            NodeManager.AddChild(parentId, childId);
        }

        public static void UpdateNodeDimensions(int id, float width, float height, float length, float radius)
        {
            NodeManager.UpdateNodeDimensions(id, width, height, length, radius);
        }

        public static void UpdateNodeOrientation(int id, float positionX, float positionY, float positionZ, float rotationX, float rotationY, float rotationZ)
        {
            NodeManager.UpdateNodePosition(id, new Vector3(positionX, positionY, 0));
            NodeManager.UpdateNodeRotation(id, new Quaternion(rotationX, rotationY, rotationZ, 0));
        }

        public static void SetConfidenceUpdates(int id, bool shouldCalculate)
        {
            NodeManager.SetConfidenceUpdates(id, shouldCalculate);
        }

        public static void ShouldRunCalculations(bool running)
        {
            NodeManager.ShouldRunCalculations(running);
        }

        public static void SetEnvironmentVaribles(float distanceScale, float screenWidth, float screenHeight, float aspectRation, float fieldOfView, string windingOrder, float maxZetaDistance)
        {
            NodeManager.SetEnvironmentVarible(distanceScale, screenWidth, screenHeight, aspectRation, fieldOfView, windingOrder, maxZetaDistance);
        }

        public static void ForceManagedNode(int id)
        {
            NodeManager.ForceSelection(id);
        }

        public static int GetCurrentSelection()
        {           
            return NodeManager.GetCurrentSelection();
        }

        public static void ForceDeselection()
        {
            NodeManager.ForceDeselection();
        }
        
        private static void SubscribeCallbacks(int id, Action<double> confidenceCallback, Action qiEngineUpdateCallback, Action selectionCallback, Action deselectionCallback)
        {
            NodeManager.SubscribeDeselected(id, deselectionCallback);
            NodeManager.SubscribeConfidenceChange(id, confidenceCallback);
            NodeManager.SubscribeSelected(id, selectionCallback);
            NodeManager.SubscribeOnUpdate(id, qiEngineUpdateCallback);
        }

        public static void UpdateConfidence(float screenX, float screenY, float worldX, float worldY, float worldZ)
        {
            NodeManager.UpdateConfidence(screenX, screenY, worldX, worldY);
        }

        public static int CreateNode(
            float selectionThreshold,
            float deselectionThreshold,
            float holdOpenThreshold,
            float startConfidenceDistance,
            float posX,
            float posY,
            float posZ,
            float rotX,
            float rotY,
            float rotZ,
            float dimX,
            float dimY,
            float dimZ,
            float radius)
        {
            return NodeManager.CreateNode(
                selectionThreshold, 
                deselectionThreshold, 
                holdOpenThreshold, 
                startConfidenceDistance,
                new Vector3(dimX, dimY, dimZ),
                new Vector3(posX, posY, posZ), 
                new Quaternion(rotX, rotY, rotZ,0),
                radius);
        }

        /*
        public static string GetPlotPoints()
        {
            IntPtr ptr = ObjectTrackingDebug();
            string str = Marshal.PtrToStringAnsi(ptr);
            if (str == null) return "";
            return str;
        }
        */

        public static void SubscribeToCallbacks(int id, Action<double> confidenceCallback, Action qiEngineUpdateCallback, Action selectionCallback, Action deselectionCallback)
        {
            SubscribeCallbacks(id, confidenceCallback, qiEngineUpdateCallback, selectionCallback, deselectionCallback);
        }
    }


    public enum CalcType
    {
        Distance,
        Direction,
        Velocity,
        Dampening,
    };
}