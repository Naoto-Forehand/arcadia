
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class Token : UdonSharpBehaviour
{
    [SerializeField]
    private AudioSource _tokenSFX;
    

    private Rigidbody _rigidbody;
    [UdonSynced, FieldChangeCallback(nameof(IsEnabled))]
    private bool _isEnabled;
    private TokenSlot _tokenSlot;
    private ConsolePurchaser _tokenDispenser;

    public bool IsEnabled
    {
        get { return _isEnabled; }
        set
        {
            _isEnabled = value;
            Debug.Log("TOKEN SET TO ENABLED");
            if (Networking.LocalPlayer.IsOwner(gameObject))
            {
                RequestSerialization();
            }
        }
    }
    
    void Start()
    {
        InteractionText = "TOKEN";
        _rigidbody = gameObject.GetComponent<Rigidbody>();
    }

    public void Init(ConsolePurchaser tokenDispenser)
    {
        _tokenDispenser = tokenDispenser;
        _tokenDispenser.TokensGenerated += 1;
    }

    public override void Interact()
    {
        _rigidbody.isKinematic = true;
    }

    public override void OnPickup()
    {
        _rigidbody.isKinematic = true;
    }

    public override void OnDrop()
    {
        _rigidbody.isKinematic = false;
    }

    public override void OnPickupUseUp()
    {
        Debug.Log($"Use Token {IsEnabled}");
        
        if (IsEnabled && _tokenSlot != null)
        {
            _tokenSFX.PlayOneShot(_tokenSFX.clip);
            _tokenSlot.SendCustomNetworkEvent(NetworkEventTarget.Owner,"TriggerScreen");
            _tokenDispenser.TokensGenerated -= 1;
            Destroy(gameObject);
        }
    }

    public void SetTokenSlot(TokenSlot slot)
    {
        _tokenSlot = slot;
    }
}
