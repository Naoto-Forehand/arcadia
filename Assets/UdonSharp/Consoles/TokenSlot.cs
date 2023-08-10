
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class TokenSlot : UdonSharpBehaviour
{
    [SerializeField]
    private GameObject[] _triggerObjects;

    [UdonSynced, FieldChangeCallback(nameof(IsOccupied))]
    private bool _isOccupied;

    private bool _objectsEnabled;
    
    public bool IsOccupied
    {
        get { return _isOccupied; }
        set
        {
            _isOccupied = value;
        }
    }

    public bool ObjectsEnabled
    {
        get { return _objectsEnabled; }
        set
        {
            _objectsEnabled = value;
            foreach (var triggerObject in _triggerObjects)
            {
                triggerObject.SetActive(_objectsEnabled);
            }
        }
    }
    
    void Start()
    {
        if (Networking.LocalPlayer.IsOwner(gameObject))
        {
            IsOccupied = false;
            RequestSerialization();
        }
    }

    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        if (Networking.LocalPlayer == player && !Networking.LocalPlayer.IsOwner(gameObject))
        {
            Networking.SetOwner(player,gameObject);
            IsOccupied = true;
            RequestSerialization();
        }
    }

    public override void OnPlayerTriggerStay(VRCPlayerApi player)
    {
        if (IsOccupied && Networking.IsOwner(player, gameObject))
        {
            var possibleToken = GetToken(player);
            if (possibleToken != null && !possibleToken.IsEnabled)
            {
                possibleToken.IsEnabled = true;
                possibleToken.SetTokenSlot(this);
            }
        }
    }

    public override void OnPlayerTriggerExit(VRCPlayerApi player)
    {
        if (IsOccupied && Networking.IsOwner(player, gameObject))
        {
            var possibleToken = GetToken(player);
            if (possibleToken != null && possibleToken.IsEnabled)
            {
                possibleToken.IsEnabled = false;
                possibleToken.SetTokenSlot(null);
            }

            IsOccupied = false;
            RequestSerialization();
        }
    }

    public void TriggerScreen()
    {
        if (Networking.LocalPlayer.IsOwner(gameObject))
        {
            if (!ObjectsEnabled)
            {
                ObjectsEnabled = true;
                RequestSerialization();
            }
        }
    }
    
    private Token GetToken(VRCPlayerApi player)
    {
        var leftPickup =  (player != null) ? player.GetPickupInHand(VRC_Pickup.PickupHand.Left) : null;
        var rightPickup = (player != null) ? player.GetPickupInHand(VRC_Pickup.PickupHand.Right) : null;

        if (leftPickup != null || rightPickup != null)
        {
            var pickup = (leftPickup != null) ? leftPickup : rightPickup;
            return (pickup.gameObject.GetComponent<Token>() != null) ? pickup.gameObject.GetComponent<Token>() : null;
        }
        else
        {
            return null;
        }
    }
}
