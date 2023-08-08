using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRC.Udon.Common;
using VRC.Udon.Common.Enums;
using VRC.Udon.Common.Interfaces;

public class EventReceiver : MonoBehaviour, IUdonEventReceiver
{
    #region IUdonEventReceiver

    public bool DisableInteractive { get; set; }

    public bool DisableEventProcessing { get; set; }

    public string InteractionText { get; set; }

    public void RequestSerialization()
    {
        Debug.Log("REQUEST SERIALIZATION");
    }

    public void SendCustomEvent(string eventName)
    {
        Debug.Log($"SEND CUSTOM EVENT {eventName}");
    }

    public void SendCustomEventDelayedFrames(string eventName, int delayFrames, EventTiming eventTiming = EventTiming.Update)
    {
        Debug.Log($"SEND CUSTOM EVENT {eventName}");
    }

    public void SendCustomEventDelayedSeconds(string eventName, float delaySeconds, EventTiming eventTiming = EventTiming.Update)
    {
        Debug.Log($"SEND CUSTOM EVENT {eventName}");
    }

    public void SendCustomNetworkEvent(NetworkEventTarget target, string eventName)
    {
        Debug.Log($"SEND CUSTOM EVENT {eventName}");
    }

    public bool RunEvent(string eventName, params (string symbolName, object value)[] eventParameters)
    {
        Debug.Log($"RunEvent: {eventName}");
        return true;
    }

    public void RunInputEvent(string eventName, UdonInputEventArgs args)
    {
        Debug.Log($"RunInputEvent: {eventName}");
    }

    public T GetProgramVariable<T>(string symbolName)
    {
        return default(T);
    }

    public Type GetProgramVariableType(string symbolName)
    {
        return default(Type);
    }

    public object GetProgramVariable(string symbolName)
    {
        return default(object);
    }

    public void SetProgramVariable<T>(string symbolName, T value)
    {
        Debug.Log($"Attempt to Set {symbolName} with {value}");
    }

    public void SetProgramVariable(string symbolName, object value)
    {
        Debug.Log($"Attempt to Set {symbolName} with {value}");
    }

    public bool TryGetProgramVariable(string symbolName, out object value)
    {
        value = default(object);
        return true;
    }

    public bool TryGetProgramVariable<T>(string symbolName, out T value)
    {
        value = default(T);
        return true;
    }
    
    #endregion IUdonEventReceiver
}
