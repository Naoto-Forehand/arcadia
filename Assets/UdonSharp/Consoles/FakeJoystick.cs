
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class FakeJoystick : UdonSharpBehaviour
{
    [SerializeField]
    private UnityEngine.UI.Text[] _texts;
    
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
    private Rigidbody _rigidbody;
    
    void Start()
    {
        if (Networking.IsOwner(gameObject))
        {
            IsHeld = false;
            PickUpHand = UNASSIGNED;
            RequestSerialization();
        }
        
        _lastVelocity = Vector3.zero;
        _rigidbody = gameObject.GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (IsHeld && Networking.LocalPlayer != null)
        {
            _lastVelocity = Networking.LocalPlayer.GetVelocity();
        }
    }

    private void LateUpdate()
    {
        if (IsHeld)
        {
            Debug.Log($"{_rigidbody.velocity} {Vector3.right} {Vector3.left} {Vector3.forward} {Vector3.back}");
            var rbVelocity = _rigidbody.velocity;

            var currentVelocity = Networking.LocalPlayer.GetVelocity();
            var cameraPos = _camera.transform.position;
            // var updated = currentVelocity - _lastVelocity / Time.deltaTime;
            // Debug.Log($"{currentVelocity} {updated} {cameraPos} {currentVelocity.magnitude} {updated.magnitude}");
            // cameraPos += updated;
            var updatedPos = new Vector3((cameraPos.x + (rbVelocity.x * currentVelocity.magnitude)),(cameraPos.y + (rbVelocity.y * currentVelocity.magnitude)),(cameraPos.z + (rbVelocity.z * currentVelocity.magnitude)));
            // cameraPos.x += rbVelocity.x;
            // cameraPos.y += rbVelocity.y;
            // cameraPos.z += rbVelocity.z;
            // cameraPos += updatedPos;
            Debug.Log($"{currentVelocity} {cameraPos} {updatedPos}");
            _camera.transform.position = updatedPos;
            UpdateTextFields();
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

    private void UpdateTextFields()
    {
        for (int index = 0; index < _texts.Length; ++index)
        {
            _texts[index].text = GetTextToDisplay(index);
        }
    }

    private string GetTextToDisplay(int index)
    {
        switch (index)
        {
            case 0:
                return $"{_camera.transform.position}";
            case 1:
                return $"{_rigidbody.transform.forward}";
            case 2:
                return $"{_rigidbody.transform.rotation}";
            default:
                return string.Empty;
        }
    }
}
