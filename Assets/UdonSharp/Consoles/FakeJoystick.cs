
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

    public bool IsHeld
    {
        get { return _isHeld; }
        set { _isHeld = value; }
    }
    
    void Start()
    {
        if (Networking.IsOwner(gameObject))
        {
            IsHeld = false;
            
            RequestSerialization();
        }
    }

    private void LateUpdate()
    {
        if (IsHeld)
        {
            var currentVelocity = Networking.LocalPlayer.GetVelocity();
            var cameraPos = _camera.transform.position;
            Debug.Log($"{currentVelocity} {cameraPos}");
            cameraPos += currentVelocity;
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
            IsHeld = true;
            RequestSerialization();
        }
    }

    public override void OnDrop()
    {
        if (Networking.IsOwner(gameObject))
        {
            IsHeld = false;
            RequestSerialization();
        }
    }
}
