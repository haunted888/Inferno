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
    public bool startWithXp = false;  // false = use levels, true = use xp
    [Min(1)] public int startLevel = 1;
    [Min(0)] public int startXp = 0;

    [Header("Money")]
    public int moneyAmount;

    [Header("Item")]
    public ItemDefinition item;
    public int itemQuantity = 1;
}
