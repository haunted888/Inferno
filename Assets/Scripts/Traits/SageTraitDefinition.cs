using UnityEngine;

[CreateAssetMenu(menuName = "Traits/Sage Trait")]
public class SageTraitDefinition : TraitDefinition
{
    public ItemDefinition scrollToGive;
    
    private bool hasGivenScroll = false;


    void Awake()
    {
        traitType = CharacterTrait.Sage;
    }

    public override void OnInitialize(MapPartyMemberDefinition member)
    {
        hasGivenScroll = false; 
    }


    public override void OnLevelUp(MapPartyMemberDefinition member)
    {
        Debug.Log("Hase given scroll: " + hasGivenScroll);
        if (member.level >= 6 && !hasGivenScroll) 
        {
            MapCombatTransfer.Instance.AddItem(scrollToGive, 1);
            hasGivenScroll = true;
        }
    }
}
