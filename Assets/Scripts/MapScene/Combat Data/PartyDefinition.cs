using UnityEngine;

public class PartyDefinition : MonoBehaviour
{
    public MapPartyMemberDefinition[] members;

    void Awake()
    {
        var transfer = MapCombatTransfer.Instance;

        // If MapCombatTransfer already has a party (from previous battles/camp edits),
        // use that as the source of truth.
        if (transfer != null && transfer.HasPartyData && transfer.party != null)
        {
            members = transfer.party.ToArray();
        }
        else
        {
            // First time / no existing party in transfer: seed it from this definition
            transfer?.SetupParty(members);
        }
    }

}
