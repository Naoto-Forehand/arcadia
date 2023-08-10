
using UdonSharp;
using VRC.Economy;

// This class copies example described by Momo
public class GroupDisplay : UdonSharpBehaviour
{
    // Short ID for Group, not the full url, just the short id
    public string GroupID;
    // Displayable name, for conventional reading
    public string GroupName;
    
    // Sets the Interaction text on the Behavior Object at Start
    void Start()
    {
        InteractionText = $"Support {GroupName}";
    }

    // Opens the Group Page in the Menu Directly
    public override void Interact()
    {
        Store.OpenGroupStorePage(GroupID);
    }
}
