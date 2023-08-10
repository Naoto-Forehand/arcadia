
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Joystick : UdonSharpBehaviour
{
    public Rigidbody StickBody;


    [UdonSynced, FieldChangeCallback(nameof(IsActiveInUse))]
    private bool _isActive;

    public bool IsActiveInUse
    {
        get { return _isActive; }
        set
        {
            _isActive = value;
            StickBody.freezeRotation = _isActive;
        }
    }
    
    void Start()
    {
        if (StickBody != null)
        {
            IsActiveInUse = false;
        }
    }

    private void Update()
    {
        if (IsActiveInUse && Networking.LocalPlayer.IsOwner(gameObject))
        {
            
            // var holdingHand = Networking.LocalPlayer.GetPickupInHand(VRC_Pickup.PickupHand.Left)
            Debug.Log("TRACKING POSITION");
        }
    }

    public override void OnPickup()
    {
        IsActiveInUse = true;
        if (!Networking.LocalPlayer.IsOwner(gameObject))
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            
            RequestSerialization();
        }
    }

    public override void OnDrop()
    {
        IsActiveInUse = false;
    }

    Vector3 GetBonePosition(VRC_Pickup.PickupHand pickupHand)
    {
        var pickup = (int)pickupHand;
        Vector3 handPosition = Vector3.zero;
        switch (pickup)
        {
            case (int)VRC_Pickup.PickupHand.Right:
                handPosition = Networking.LocalPlayer.GetBonePosition(HumanBodyBones.RightHand);
                break;
            case (int)VRC_Pickup.PickupHand.Left:
                handPosition = Networking.LocalPlayer.GetBonePosition(HumanBodyBones.LeftHand);
                break;
            default:
                break;
        }

        return handPosition;
    }
    
    
    public void OnPositionUpdate()
    {
        
    }
}
