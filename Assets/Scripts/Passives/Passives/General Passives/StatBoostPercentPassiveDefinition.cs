using UnityEngine;

[CreateAssetMenu(menuName = "Passives/Stat Boost Percent")]
public class StatBoostPercentPassiveDefinition : PassivesDefinition
{
    public float attackBoostPercent = 0.00f;
    public float elementalPowerBoostPercent = 0.00f;
    public float defenseBoostPercent = 0.00f;
    public float elementalResistanceBoostPercent = 0.00f;

    public int duration = int.MaxValue; // Duration in turns, default to max for "until end of battle" behavior


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

        Debug.Log($"Applying stat boosts: {attackBoostPercent*100}% attack, {elementalPowerBoostPercent*100}% elemental power, {defenseBoostPercent*100}% defense, {elementalResistanceBoostPercent*100}% elemental resistance");
        if (self == null) return;
        
        self.bonusStats.physicalAttack += Mathf.CeilToInt(self.baseStats.physicalAttack * Mathf.Abs(attackBoostPercent)) * (int)Mathf.Sign(attackBoostPercent);
        self.bonusStats.elementalPower += Mathf.CeilToInt(self.baseStats.elementalPower * Mathf.Abs(elementalPowerBoostPercent)) * (int)Mathf.Sign(elementalPowerBoostPercent);
        self.bonusStats.defense += Mathf.CeilToInt(self.baseStats.defense * Mathf.Abs(defenseBoostPercent)) * (int)Mathf.Sign(defenseBoostPercent);
        self.bonusStats.elementalResistance += Mathf.CeilToInt(self.baseStats.elementalResistance * Mathf.Abs(elementalResistanceBoostPercent)) * (int)Mathf.Sign(elementalResistanceBoostPercent);


        
    }

    public override void OnResolvePhaseEnd(BattleCharacter self)
    {
        if (self == null) return;
        if(duration > 100000) return;
        duration--;
        if (duration <= 0){
            self.QueuePassiveToRemove(this, PassivesDefinition.PassiveHook.OnResolvePhaseEnd);
        }
    }

    public override string GetDescription(BattleCharacter character)
    {
        if (character == null) return description;
        string descriptionReturn = "";
        if (attackBoostPercent != 0) descriptionReturn += $"Attack: {(attackBoostPercent > 0 ? "+" : "-")}{Mathf.CeilToInt(Mathf.Abs(attackBoostPercent) * character.baseStats.physicalAttack)}\n";
        if (elementalPowerBoostPercent != 0) descriptionReturn += $"Elemental Power: {(elementalPowerBoostPercent > 0 ? "+" : "-")}{Mathf.CeilToInt(Mathf.Abs(elementalPowerBoostPercent) * character.baseStats.elementalPower)}\n";
        if (defenseBoostPercent != 0) descriptionReturn += $"Defense: {(defenseBoostPercent > 0 ? "+" : "-")}{Mathf.CeilToInt(Mathf.Abs(defenseBoostPercent) * character.baseStats.defense)}\n";
        if (elementalResistanceBoostPercent != 0) descriptionReturn += $"Elemental Resistance: {(elementalResistanceBoostPercent > 0 ? "+" : "-")}{Mathf.CeilToInt(Mathf.Abs(elementalResistanceBoostPercent) * character.baseStats.elementalResistance)}\n";
        return descriptionReturn.TrimEnd('\n');
    }
}
