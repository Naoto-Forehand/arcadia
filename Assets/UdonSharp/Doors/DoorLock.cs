
using System;
using UdonSharp;
using UnityEngine;
using VRC.Economy;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common;
using VRC.Udon.Common.Interfaces;

public class DoorLock : UdonSharpBehaviour
{
    private GameObject _door;
    private Animator _doorAnimator;
    private const string DOOR_TRIGGER = "character_nearby";
    private IProduct _fetchedProduct;
    
    [SerializeField]
    private SimpleProduct _product;
    // private EventReceiver _eventReceiver;
    
    [UdonSynced, FieldChangeCallback(nameof(IsLocked))]
    private bool _isLocked;
    [UdonSynced, FieldChangeCallback(nameof(IsOpen))]
    private bool _isOpen;
    
    public bool IsLocked
    {
        get { return _isLocked; }
        set { _isLocked = value; }
    }

    public bool IsOpen
    {
        get { return _isOpen; }
        set
        {
            _isOpen = IsLocked ? false : value;
            _doorAnimator.SetBool(DOOR_TRIGGER, _isOpen);
        }
    }

    public IProduct Product
    {
        get { return _product; }
        // set { _product = value as SimpleProduct; }
    }
    
    void Start()
    {
        _door = gameObject;
        if (_door != null)
        {
            _doorAnimator = _door.GetComponent<Animator>();
        }

        // if (gameObject.GetComponent<EventReceiver>() != null)
        // {
        //     _eventReceiver = gameObject.GetComponent<EventReceiver>();
        // }
        
        if (Networking.LocalPlayer.IsOwner(gameObject))
        {
            IsLocked = true;
            IsOpen = false;
            RequestSerialization();
        }
        else
        {
            Debug.Log($"PLAYER CANNOT UNLOCK THIS UNTIL THEY PURCHASE {_product.Name}");
        }
        
        var eventReceiver = (IUdonEventReceiver)this;
        Store.ListAvailableProducts(eventReceiver);
    }

    public override void Interact()
    {
        if (!Networking.LocalPlayer.IsOwner(gameObject))
        {
            Networking.SetOwner(Networking.LocalPlayer,gameObject);
        }

        if (Store.DoesPlayerOwnProduct(Networking.LocalPlayer, _product))
        {
            Debug.Log("PLAYER CAN ENTER HERE");
            IsLocked = false;
            IsOpen = true;
            
            RequestSerialization();
        }
    }

    public override void OnDeserialization()
    {
        if (Store.DoesPlayerOwnProduct(Networking.LocalPlayer, _product))
        {
            Debug.Log("CAN UNLOCK THIS DOOR");
        }
        else
        {
            _isLocked = true;
            _isOpen = false;
        }
    }

    public void OnPurchaseUse(IProduct product)
    {
        //
    }
    
    public void OnListAvailableProducts(IProduct[] products)
    {
        for (int index = 0; index < products.Length; ++index)
        {
            if (products[index] == Product)
            {
                Debug.Log($"STORE HAS {products[index]}");
                _fetchedProduct = products[index];
            }
        }
    }
}
