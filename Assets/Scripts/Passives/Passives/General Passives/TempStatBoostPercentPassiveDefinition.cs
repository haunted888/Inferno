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
        
        if(displayName == "Stat Modification") return; // Allow multiple instances of this passive
        foreach (var existingPassive in self.passives)
        {
            if (existingPassive != this && existingPassive.displayName == displayName)
            {
                self.RemovePassive(this);
                break;
            }
        }
    }

    public override void GetStatBoosts(BattleCharacter self)
    {
        if (self == null) return;
        
        self.bonusStats.physicalAttack += Mathf.CeilToInt(self.baseStats.physicalAttack * Mathf.Abs(attackBoostPercent)) * (int)Mathf.Sign(attackBoostPercent);
        self.bonusStats.elementalPower += Mathf.CeilToInt(self.baseStats.elementalPower * Mathf.Abs(elementalPowerBoostPercent)) * (int)Mathf.Sign(elementalPowerBoostPercent);
        self.bonusStats.defense += Mathf.CeilToInt(self.baseStats.defense * Mathf.Abs(defenseBoostPercent)) * (int)Mathf.Sign(defenseBoostPercent);
        self.bonusStats.elementalResistance += Mathf.CeilToInt(self.baseStats.elementalResistance * Mathf.Abs(elementalResistanceBoostPercent)) * (int)Mathf.Sign(elementalResistanceBoostPercent);

    }

    public override void OnResolvePhaseEnd(BattleCharacter self)
    {
        if (self == null) return;
        self.QueuePassiveToRemove(this);
    }
}