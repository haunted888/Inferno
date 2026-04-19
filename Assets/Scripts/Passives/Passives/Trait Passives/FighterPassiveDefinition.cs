using UnityEngine;

[CreateAssetMenu(menuName = "Passives/Trait Passives/Fighter Passive")]
public class FighterPassiveDefinition : PassivesDefinition
{
    public FighterDefenseBoostPassive fighterDefenseBoost;
    public float defenseBoostAmount;
    public float resistanceBoostAmount;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void setDefenseBoostAmount(float amount)
    {
        defenseBoostAmount = amount;
    }
    public void setResistanceBoostAmount(float amount)
    {
        resistanceBoostAmount = amount;
    }
    public override void OnSkillUsed(BattleCharacter self, BattleCharacter target, Skill skill)
    {
        if (skill == null || fighterDefenseBoost == null) return;

        if (skill.damageType != SkillDamageType.None && !self.passives.Contains(fighterDefenseBoost))
        {
            fighterDefenseBoost.setStatBoosts(defenseBoostAmount, resistanceBoostAmount);
            self.QueuePassiveToAdd(fighterDefenseBoost, PassivesDefinition.PassiveHook.OnSkillUsed, self);
        }
    }
}
