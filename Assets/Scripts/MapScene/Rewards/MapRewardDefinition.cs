using UnityEngine;

public enum RewardCategory
{
    PartyMember,
    Money,
    Item
}

[System.Serializable]
public class MapRewardDefinition
{
    public RewardCategory type = RewardCategory.PartyMember;

    [Header("Party Member (used when type = PartyMember)")]
    public MapPartyMemberDefinition partyMember;

    [Header("Money (not implemented yet)")]
    public int moneyAmount;

    [Header("Item")]
    public ItemDefinition item;
    public int itemQuantity = 1;
}
