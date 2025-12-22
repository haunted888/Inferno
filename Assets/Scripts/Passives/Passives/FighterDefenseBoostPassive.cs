using UnityEngine;

[CreateAssetMenu(menuName = "Passives/FighterDefenseBoost")]
public class FighterDefenseBoostPassive : PassivesDefinition
{
    float defenseBoost = 0f; 
    float resistanceBoost = 0f; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void setStatBoosts(float defenseBoost, float resistanceBoost)
    {
        this.defenseBoost = defenseBoost;
        this.resistanceBoost = resistanceBoost;
    }

    public override void getStatBoosts(BattleCharacter self)
    {
        self.bonusStats.defense += Mathf.CeilToInt(self.baseStats.defense * defenseBoost);
        self.bonusStats.elementalResistance += Mathf.CeilToInt(self.baseStats.elementalResistance * resistanceBoost);
    }

    public override void OnSkillUsed(BattleCharacter self, Skill skill)
    {
        if(skill.damageType == SkillDamageType.None)
        {
            self.RemovePassive(this);
        }
    }
}
