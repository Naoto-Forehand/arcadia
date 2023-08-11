
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class TeleportVolume : UdonSharpBehaviour
{
    public Vector3 Position
    {
        get { return transform.position; }
    }

    public Quaternion Rotation
    {
        get { return transform.rotation; }
    }

    public VRCPlayerApi[] InCollider
    {
        get { return _inCollider; }
    }
    
    private VRCPlayerApi[] _inCollider = new VRCPlayerApi[16];
    private TeleporterConsole _connectedConsole;

    public void Init(TeleporterConsole console)
    {
        _connectedConsole = console;
    }
    
    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        if (!_connectedConsole.IsReceiver)
        {
            if (!Networking.IsOwner(gameObject))
            {
                Networking.SetOwner(player, gameObject);
                Networking.SetOwner(player, _connectedConsole.PartnerPad.gameObject);
                Networking.SetOwner(player, _connectedConsole.TeleportVolume.gameObject);
            }
            
            int availableIndex = -1;
            for (int index = 0; index < _inCollider.Length; ++index)
            {
                Debug.Log($"{(_inCollider[index] != null)}");
                if (_inCollider[index] == null)
                {
                    availableIndex = index;
                    break;
                }
            }

            if (availableIndex > -1 && availableIndex < _inCollider.Length)
            {
                Debug.Log($"ADDING {player.playerId}");
                _inCollider[availableIndex] = player;
            }
            
            Debug.Log($"{gameObject.name} {_inCollider[availableIndex].displayName}");
            RequestSerialization();
        }
    }
    
    public override void OnPlayerTriggerExit(VRCPlayerApi player)
    {
        if (!_connectedConsole.IsReceiver)
        {
            if (!Networking.IsOwner(gameObject))
            {
                Networking.SetOwner(player, gameObject);
                Networking.SetOwner(player, _connectedConsole.PartnerPad.gameObject);
                Networking.SetOwner(player, _connectedConsole.TeleportVolume.gameObject);
            }
            
            int indexToRemove = -1;
            for (int index = 0; index < _inCollider.Length; ++index)
            {
                if (_inCollider[index] == player)
                {
                    indexToRemove = index;
                    break;
                }
            }

            if (indexToRemove > -1 && indexToRemove < _inCollider.Length)
            {
                _inCollider[indexToRemove] = null;
            }
            
            RequestSerialization();
        }
    }

    public void TeleportPlayers()
    {
        Debug.Log($"{gameObject.name}  {_inCollider.Length}");
        for (int index = 0; index < _inCollider.Length; ++index)
        {
            Debug.Log($"{(_inCollider[index] != null)}");
            if (_inCollider[index] != null)
            {
                var targetVolume = _connectedConsole.PartnerPad.TeleportVolume;
                _inCollider[index].TeleportTo(targetVolume.Position,targetVolume.Rotation);
            }
        }
        
        ClearColliders();
    }
    
    public void ClearColliders()
    {
        _inCollider = new VRCPlayerApi[16];
    }
}
