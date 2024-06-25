using UnityEngine;

public abstract class IGestureLogic
{
    abstract public void Invoke(GestureManager.GestureData gestureData, Transform obj, float sensitivity);
}