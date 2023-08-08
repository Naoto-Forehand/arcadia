
using UdonSharp;
using UnityEngine;
using VRC.Economy;
using VRC.SDKBase;
using VRC.Udon;

public class GroupDisplay : UdonSharpBehaviour
{
    public string GroupID;
    public string GroupName;
    
    void Start()
    {
        InteractionText = $"Support {GroupName}";
    }

    public override void Interact()
    {
        Store.OpenGroupPage(GroupID);
    }
}
