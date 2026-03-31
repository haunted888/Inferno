using UnityEngine;

[CreateAssetMenu(menuName = "Passives/Temp Stat Boost Percent")]
public class TempStatBoostPercent : PassivesDefinition
{
    [Range(0f, 1f)] public float attackBoostPercent = 0.00f;
    [Range(0f, 1f)] public float elementalPowerBoostPercent = 0.00f;
    [Range(0f, 1f)] public float defenseBoostPercent = 0.00f;
    [Range(0f, 1f)] public float elementalResistanceBoostPercent = 0.00f;


    public void setStatBoosts(
        float attackBoostPercent, 
        float elementalPowerBoostPercent, 
        float defenseBoostPercent, 
        float elementalResistanceBoostPercent)
    {
        this.attackBoostPercent = attackBoostPercent;
        this.elementalPowerBoostPercent = elementalPowerBoostPercent;
        this.defenseBoostPercent = defenseBoostPercent;
        this.elementalResistanceBoostPercent = elementalResistanceBoostPercent;
    }

    public override void OnCreated(BattleCharacter self)
    {
        
        if(displayName == "Boosted Stats") return; // Allow multiple instances of this passive
        foreach (var existingPassive in self.passives)
        {
            if (existingPassive != this && existingPassive.displayName == displayName)
            {
                self.RemovePassive(this);
            }
        }
    }

    public override void GetStatBoosts(BattleCharacter self)
    {
        if (self == null) return;

        self.bonusStats.physicalAttack += Mathf.CeilToInt(self.baseStats.physicalAttack * attackBoostPercent);
        self.bonusStats.elementalPower += Mathf.CeilToInt(self.baseStats.elementalPower * elementalPowerBoostPercent);
        self.bonusStats.defense += Mathf.CeilToInt(self.baseStats.defense * defenseBoostPercent);
        self.bonusStats.elementalResistance += Mathf.CeilToInt(self.baseStats.elementalResistance * elementalResistanceBoostPercent);

        Debug.Log($"TempStatBoostPercent applied to {self.name}: " +
                  $"ATK +{Mathf.CeilToInt(self.baseStats.physicalAttack * attackBoostPercent)}, " +
                  $"ELM POW +{Mathf.CeilToInt(self.baseStats.elementalPower * elementalPowerBoostPercent)}, " +
                  $"DEF +{Mathf.CeilToInt(self.baseStats.defense * defenseBoostPercent)}, " +
                  $"ELM RES +{Mathf.CeilToInt(self.baseStats.elementalResistance * elementalResistanceBoostPercent)}");
    }

    public override void OnResolvePhaseEnd(BattleCharacter self)
    {
        if (self == null) return;
        self.QueuePassiveToRemove(this);
    }
}