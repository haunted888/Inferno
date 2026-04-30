using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Traits/Academic Trait")]
public class AcademicTraitDefinition : TraitDefinition
{
    public StatBoostPercentPassiveDefinition statBoostPassive;
    public float boostPercentPerSkill;
    public float scrollSaveChance = 0.5f;
    
    void Awake()
    {
        traitType = CharacterTrait.Academic;
    }

    public override void OnBattleStart(BattleCharacter user)
    {
        int skillMultiplier = user.Skills.Count;

        CombatStats statBoosts = new CombatStats()
        {
            maxHealth = Mathf.CeilToInt(100 * boostPercentPerSkill * skillMultiplier),
            maxSp = Mathf.CeilToInt(100 * boostPercentPerSkill * skillMultiplier),

            physicalAttack = Mathf.CeilToInt(100 * boostPercentPerSkill * skillMultiplier),
            elementalPower = Mathf.CeilToInt(100 * boostPercentPerSkill * skillMultiplier),
            defense = Mathf.CeilToInt(100 * boostPercentPerSkill * skillMultiplier),
            elementalResistance = Mathf.CeilToInt(100 * boostPercentPerSkill * skillMultiplier),

            speed = Mathf.CeilToInt(100 * boostPercentPerSkill * skillMultiplier),
        };
        statBoostPassive.SetStatBoosts(statBoosts);

        user.AddPassive(statBoostPassive);
    }

    public override void OnMapItemUsed(MapPartyMemberDefinition user, ItemDefinition item)
    {
        if (item == null || item.mapConsumable == null || !item.itemTypes.Contains(ItemType.Scroll)) return;

        if(Random.value < scrollSaveChance)
        {
            MapCombatTransfer.Instance.AddItem(item, 1);
            Debug.Log($"Academic trait: Returning {item.displayName} to inventory after use.");
        }

        
    }
}
