
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class TeleporterConsole : UdonSharpBehaviour
{
    [SerializeField]
    private AudioSource _audio;

    [SerializeField]
    private Animator _alertAnimator;
    
    [SerializeField]
    private TeleportVolume _teleporterVolume;

    [SerializeField]
    private TeleporterConsole _partnerPad;
    
    [SerializeField]
    [UdonSynced, FieldChangeCallback(nameof(IsReceiver))]
    private bool _isReceiver;
    
    // private Vector3 _targetPosition;
    // private Quaternion _targetRotation;
    
    public VRCPlayerApi[] InCollider
    {
        get { return _teleporterVolume.InCollider; }
    }

    public TeleporterConsole PartnerPad
    {
        get { return _partnerPad; }
    }

    public TeleportVolume TeleportVolume
    {
        get { return _teleporterVolume; }
    }
    
    public bool IsReceiver
    {
        get { return _isReceiver; }
        set { _isReceiver = value; }
    }
    
    void Start()
    {
        if (_teleporterVolume != null)
        {
            // _targetPosition = _teleporterVolume.Position;
            // _targetRotation = _teleporterVolume.Rotation;
            _teleporterVolume.Init(this);
        }
    }

    // public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    // {
    //     if (!_isReceiver)
    //     {
    //         if (!Networking.IsOwner(gameObject))
    //         {
    //             Networking.SetOwner(player, gameObject);
    //             Networking.SetOwner(player, _partnerPad.gameObject);
    //         }
    //         
    //         int availableIndex = -1;
    //         for (int index = 0; index < _inCollider.Length; ++index)
    //         {
    //             Debug.Log($"{(_inCollider[index] != null)}");
    //             if (_inCollider[index] == null)
    //             {
    //                 availableIndex = index;
    //                 break;
    //             }
    //         }
    //
    //         if (availableIndex > -1 && availableIndex < _inCollider.Length)
    //         {
    //             Debug.Log($"ADDING {player.playerId}");
    //             _inCollider[availableIndex] = player;
    //         }
    //         
    //         RequestSerialization();
    //     }
    // }

    // public override void OnPlayerTriggerExit(VRCPlayerApi player)
    // {
    //     if (!_isReceiver)
    //     {
    //         if (!Networking.IsOwner(gameObject))
    //         {
    //             Networking.SetOwner(player, gameObject);
    //             Networking.SetOwner(player, _partnerPad.gameObject);
    //         }
    //         
    //         int indexToRemove = -1;
    //         for (int index = 0; index < _inCollider.Length; ++index)
    //         {
    //             if (_inCollider[index] == player)
    //             {
    //                 indexToRemove = index;
    //                 break;
    //             }
    //         }
    //
    //         if (indexToRemove > -1 && indexToRemove < _inCollider.Length)
    //         {
    //             _inCollider[indexToRemove] = null;
    //         }
    //         
    //         RequestSerialization();
    //     }
    // }

    public override void Interact()
    {
        if (!Networking.IsOwner(gameObject))
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            Networking.SetOwner(Networking.LocalPlayer, _partnerPad.gameObject);
            Networking.SetOwner(Networking.LocalPlayer, _partnerPad.TeleportVolume.gameObject);
        }
        
        if (_isReceiver)
        {
            Debug.Log($"TELEPORT ANYONE IN THE WAITING PAD {_partnerPad.InCollider.Length}");
            if (_alertAnimator != null)
            {
                _alertAnimator.SetBool("incoming_message",false);
            }
            _partnerPad.TeleportVolume.SendCustomNetworkEvent(NetworkEventTarget.All,"TeleportPlayers");
            // for (int index = 0; index < _partnerPad.InCollider.Length; ++index)
            // {
            //     Debug.Log($"{(_partnerPad.InCollider[index] != null)}");
            //     if (_partnerPad.InCollider[index] != null)
            //     {
            //         _partnerPad.InCollider[index].TeleportTo(_targetPosition,_targetRotation);
            //     }
            // }
            // _partnerPad.ClearColliders();
        }
        else
        {
            _partnerPad.Knock();
        }

        if (Networking.IsOwner(gameObject))
        {
            RequestSerialization();
        }
    }

    public void Knock()
    {
        if (_audio != null)
        {
            _audio.PlayOneShot(_audio.clip);
        }

        if (_alertAnimator != null)
        {
            _alertAnimator.SetBool("incoming_message",true);
        }
    }
    
    public void ClearColliders()
    {
        _teleporterVolume.ClearColliders();
    }
    
    // public void TeleportVolume(BoxCollider collider)
    // {
    //     // var players = _teleporterVolume.GetComponentsInChildren<VRCPlayerApi>();
    //     for (int index = 0; index < _inCollider.Length; ++index)
    //     {
    //         if (_inCollider[index] != null && _inCollider[index].GetComponent<VRCPlayerApi>() != null)
    //         {
    //             _inCollider[index].GetComponent<VRCPlayerApi>().TeleportTo(collider.transform.position,collider.transform.rotation);
    //         }
    //     }
    // }
}
