using UnityEngine;

[CreateAssetMenu(menuName = "Passives/FighterPassive")]
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
    public override void OnSkillUsed(BattleCharacter self, Skill skill)
    {
        if(skill.damageType != SkillDamageType.None && !self.passives.Contains(fighterDefenseBoost))
        {
            fighterDefenseBoost.setStatBoosts(defenseBoostAmount, resistanceBoostAmount);
            BattleTurnManager.Instance.passivesToAdd.Add(fighterDefenseBoost);
        }
    }
}
