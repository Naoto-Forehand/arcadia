
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class FakeJoystick : UdonSharpBehaviour
{
    [SerializeField]
    private GameObject _camera;

    [UdonSynced, FieldChangeCallback(nameof(IsHeld))]
    private bool _isHeld;

    [UdonSynced, FieldChangeCallback(nameof(PickUpHand))]
    private int _pickUpHand;
    
    public bool IsHeld
    {
        get { return _isHeld; }
        set { _isHeld = value; }
    }

    public int PickUpHand
    {
        get { return _pickUpHand; }
        set { _pickUpHand = value; }
    }

    private const int UNASSIGNED = -1;
    private Vector3 _lastVelocity;
    
    void Start()
    {
        if (Networking.IsOwner(gameObject))
        {
            IsHeld = false;
            PickUpHand = UNASSIGNED;
            RequestSerialization();
        }
        
        _lastVelocity = Vector3.zero;
    }

    private void FixedUpdate()
    {
        if (IsHeld)
        {
            _lastVelocity = Networking.LocalPlayer.GetVelocity();
        }
    }

    private void LateUpdate()
    {
        if (IsHeld)
        {
            var currentVelocity = Networking.LocalPlayer.GetVelocity();
            var cameraPos = _camera.transform.position;
            var updated = currentVelocity - _lastVelocity / Time.deltaTime;
            Debug.Log($"{currentVelocity} {updated} {cameraPos} {currentVelocity.magnitude} {updated.magnitude}");
            cameraPos += updated;
            Debug.Log($"{currentVelocity} {cameraPos}");
            _camera.transform.position = cameraPos;
        }
    }

    public override void OnPickup()
    {
        if (!Networking.IsOwner(gameObject))
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
        }

        if (Networking.IsOwner(gameObject))
        {
            var leftHandPickup = Networking.LocalPlayer.GetPickupInHand(VRC_Pickup.PickupHand.Left);
            var rightHandPickup = Networking.LocalPlayer.GetPickupInHand(VRC_Pickup.PickupHand.Right);
            if (leftHandPickup != null && PickUpHand == UNASSIGNED && leftHandPickup == this)
            {
                PickUpHand = (int)VRC_Pickup.PickupHand.Left;
            }

            if (rightHandPickup != null && PickUpHand == UNASSIGNED && rightHandPickup == this)
            {
                PickUpHand = (int)VRC_Pickup.PickupHand.Right;
            }
            
            IsHeld = true;
            RequestSerialization();
        }
    }

    public override void OnDrop()
    {
        if (Networking.IsOwner(gameObject))
        {
            IsHeld = false;
            PickUpHand = UNASSIGNED;
            RequestSerialization();
        }
    }
}
