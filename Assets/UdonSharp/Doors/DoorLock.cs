
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
    private bool _overrideLock;
    
    [SerializeField]
    private UdonProduct _product;

    [SerializeField]
    private string _productID;

    public string ProductID
    {
        get { return _productID; }
    }
    
    // [UdonSynced, FieldChangeCallback(nameof(IsLocked))]
    private bool _isLocked;
    [UdonSynced, FieldChangeCallback(nameof(IsOpen))]
    private bool _isOpen;
    
    public bool IsLocked
    {
        get
        {
            return (_overrideLock) ? false : _isLocked;
        }
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
        get { return _fetchedProduct; }
    }
    
    void Start()
    {
        _door = gameObject;
        if (_door != null)
        {
            _doorAnimator = _door.GetComponent<Animator>();
        }

        if (Networking.LocalPlayer.IsOwner(gameObject))
        {
            IsLocked = true;
            IsOpen = false;
            RequestSerialization();
        }
        else
        {
            _isLocked = true;
            _isOpen = false;
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

        if ((Store.DoesPlayerOwnProduct(Networking.LocalPlayer, Product)) || (_overrideLock))
        {
            Debug.Log("PLAYER CAN ENTER HERE");
            IsLocked = false;
            IsOpen = true;
            
            RequestSerialization();
        }
    }

    public override void OnDeserialization()
    {
        if (Store.DoesPlayerOwnProduct(Networking.LocalPlayer, Product))
        {
            Debug.Log("CAN UNLOCK THIS DOOR");
        }
        else
        {
            _isLocked = true;
            _isOpen = false;
        }
    }

    public override void OnListPurchases(IProduct[] products, VRCPlayerApi player)
    {
        
    }

    public override void OnListAvailableProducts(IProduct[] products)
    {
        foreach (var product in products)
        {
            if (product.ID.Equals(ProductID, StringComparison.InvariantCultureIgnoreCase))
            {
                _fetchedProduct = product;
            }
        }

        if (Product != null)
        {
            Store.UsePurchase((IUdonEventReceiver)this,Product);
        }
    }

    public override void OnPurchaseUse(IProduct product, VRCPlayerApi player)
    {
        
    }

    public override void OnPurchaseConfirmed(IProduct product, VRCPlayerApi player)
    {
        
    }

    public override void OnPurchaseExpired(IProduct product, VRCPlayerApi player)
    {
        if ((IsOpen || _isOpen) && (player == Networking.LocalPlayer))
        {
            IsOpen = false;
            IsLocked = true;
        }
    }

    public override void OnPurchasesLoaded(IProduct[] products, VRCPlayerApi player)
    {
        
    }
}
