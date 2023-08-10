
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class Token : UdonSharpBehaviour
{
    private bool _isEnabled;
    private TokenSlot _tokenSlot;
    
    public bool IsEnabled
    {
        get { return _isEnabled; }
        set
        {
            _isEnabled = value;
            if (Networking.LocalPlayer.IsOwner(gameObject))
            {
                RequestSerialization();
            }
        }
    }
    
    void Start()
    {
        InteractionText = "TOKEN";
    }

    public override void OnPickupUseUp()
    {
        Debug.Log("Use Token");
        if (IsEnabled && _tokenSlot != null)
        {
            _tokenSlot.SendCustomNetworkEvent(NetworkEventTarget.Owner,"TriggerScreen");
            Destroy(gameObject);
        }
    }

    public void SetTokenSlot(TokenSlot slot)
    {
        _tokenSlot = slot;
    }
}
