
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class TokenSlot : UdonSharpBehaviour
{
    [SerializeField]
    private GameObject[] _triggerObjects;

    [SerializeField]
    private Camera _target;
    
    [UdonSynced, FieldChangeCallback(nameof(IsOccupied))]
    private bool _isOccupied;

    private bool _objectsEnabled;
    private Vector3 _defaultPosition;
    private Quaternion _defaultRotation;
    
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
        _defaultPosition = _target.transform.position;
        _defaultRotation = _target.transform.rotation;
        
        if (Networking.LocalPlayer.IsOwner(gameObject))
        {
            IsOccupied = false;
            RequestSerialization();
        }
    }

    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        Debug.Log($"{player.playerId} entered");
        if (Networking.LocalPlayer == player && !Networking.LocalPlayer.IsOwner(gameObject))
        {
            Debug.Log("SETTING OWNER");
        }

        if (Networking.IsOwner(player, gameObject))
        {
            Networking.SetOwner(player,gameObject);
            IsOccupied = true;
            var possibleToken = GetToken(player);
            if (possibleToken != null && !possibleToken.IsEnabled)
            {
                Debug.Log("ENABLING TOKEN");
                possibleToken.IsEnabled = true;
                possibleToken.SetTokenSlot(this);
            }
            RequestSerialization();
        }
    }

    public override void OnPlayerTriggerStay(VRCPlayerApi player)
    {
        if (IsOccupied && Networking.IsOwner(player, gameObject))
        {
            //
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
        Debug.Log("TURN THINGS ON NOW PLEASE");
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

    public void TriggerReset()
    {
        Debug.Log($"SHOULD RESET CAM POSITION AND ROTATION TO {_defaultPosition} {_defaultRotation}");
        _target.transform.SetPositionAndRotation(_defaultPosition,_defaultRotation);
    }
}
