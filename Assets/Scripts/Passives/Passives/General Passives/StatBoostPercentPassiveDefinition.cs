using UnityEngine;

[CreateAssetMenu(menuName = "Passives/Stat Boost Percent")]
public class StatBoostPercentPassiveDefinition : PassivesDefinition
{
    public CombatStats statBoostsPercent;

    public int duration = int.MaxValue; // Duration in turns, default to max for "until end of battle" behavior
    public bool restoreResources = false; // Whether to restore HP/SP based on the new max values when this passive is applied


    public void SetStatBoosts(
        CombatStats statBoostsPercent
        )
    {
        this.statBoostsPercent = statBoostsPercent;
    }

    public override void OnCreated(BattleCharacter self)
    {
        if(statBoostsPercent.maxHealth != 0) self.SetMaxHealth(self.MaxHealth +Mathf.CeilToInt(self.baseStats.maxHealth * Mathf.Abs(statBoostsPercent.maxHealth) * 0.01f) * (int)Mathf.Sign(statBoostsPercent.maxHealth));
        if(statBoostsPercent.maxSp != 0) self.SetMaxSp(self.MaxSp + Mathf.CeilToInt(self.baseStats.maxSp * Mathf.Abs(statBoostsPercent.maxSp) * 0.01f) * (int)Mathf.Sign(statBoostsPercent.maxSp));

        if(restoreResources)
        {
            if(statBoostsPercent.maxHealth > 0) self.Heal(Mathf.CeilToInt(self.baseStats.maxHealth * Mathf.Abs(statBoostsPercent.maxHealth) * 0.01f) * (int)Mathf.Sign(statBoostsPercent.maxHealth)) ;
            if(statBoostsPercent.maxSp > 0) self.RecoverSp(Mathf.CeilToInt(self.baseStats.maxSp * Mathf.Abs(statBoostsPercent.maxSp) * 0.01f) * (int)Mathf.Sign(statBoostsPercent.maxSp));
        }
        
        if(displayName == "Stat Modification") return; // Allow multiple instances of this passive
        foreach (var existingPassive in self.passives)
        {
            if (existingPassive != this && existingPassive.displayName == displayName)
            {
                Debug.Log($"Removing duplicate passive: {displayName}");
                self.RemovePassive(this);
                break;
            }
        }
    }

    public override void GetStatBoosts(BattleCharacter self)
    {

        if (self == null) return;
             
        self.bonusStats.physicalAttack += Mathf.CeilToInt(self.baseStats.physicalAttack * Mathf.Abs(statBoostsPercent.physicalAttack) * 0.01f) * (int)Mathf.Sign(statBoostsPercent.physicalAttack);
        self.bonusStats.elementalPower += Mathf.CeilToInt(self.baseStats.elementalPower * Mathf.Abs(statBoostsPercent.elementalPower) * 0.01f) * (int)Mathf.Sign(statBoostsPercent.elementalPower);
        self.bonusStats.defense += Mathf.CeilToInt(self.baseStats.defense * Mathf.Abs(statBoostsPercent.defense) * 0.01f) * (int)Mathf.Sign(statBoostsPercent.defense);
        self.bonusStats.elementalResistance += Mathf.CeilToInt(self.baseStats.elementalResistance * Mathf.Abs(statBoostsPercent.elementalResistance) * 0.01f) * (int)Mathf.Sign(statBoostsPercent.elementalResistance);
        self.bonusStats.speed += Mathf.CeilToInt(self.baseStats.speed * Mathf.Abs(statBoostsPercent.speed) * 0.01f) * (int)Mathf.Sign(statBoostsPercent.speed);
    
        self.bonusStats.piercingAttack += Mathf.CeilToInt(self.baseStats.piercingAttack * Mathf.Abs(statBoostsPercent.piercingAttack) * 0.01f) * (int)Mathf.Sign(statBoostsPercent.piercingAttack);
        self.bonusStats.bludgeoningAttack += Mathf.CeilToInt(self.baseStats.bludgeoningAttack * Mathf.Abs(statBoostsPercent.bludgeoningAttack) * 0.01f) * (int)Mathf.Sign(statBoostsPercent.bludgeoningAttack);
        self.bonusStats.slashingAttack += Mathf.CeilToInt(self.baseStats.slashingAttack * Mathf.Abs(statBoostsPercent.slashingAttack) * 0.01f) * (int)Mathf.Sign(statBoostsPercent.slashingAttack);

        self.bonusStats.fireAttack += Mathf.CeilToInt(self.baseStats.fireAttack * Mathf.Abs(statBoostsPercent.fireAttack) * 0.01f) * (int)Mathf.Sign(statBoostsPercent.fireAttack);
        self.bonusStats.iceAttack += Mathf.CeilToInt(self.baseStats.iceAttack * Mathf.Abs(statBoostsPercent.iceAttack) * 0.01f) * (int)Mathf.Sign(statBoostsPercent.iceAttack);
        self.bonusStats.stormAttack += Mathf.CeilToInt(self.baseStats.stormAttack * Mathf.Abs(statBoostsPercent.stormAttack) * 0.01f) * (int)Mathf.Sign(statBoostsPercent.stormAttack);
        self.bonusStats.bloodAttack += Mathf.CeilToInt(self.baseStats.bloodAttack * Mathf.Abs(statBoostsPercent.bloodAttack) * 0.01f) * (int)Mathf.Sign(statBoostsPercent.bloodAttack);
        self.bonusStats.acidAttack += Mathf.CeilToInt(self.baseStats.acidAttack * Mathf.Abs(statBoostsPercent.acidAttack) * 0.01f) * (int)Mathf.Sign(statBoostsPercent.acidAttack);
        self.bonusStats.psychicAttack += Mathf.CeilToInt(self.baseStats.psychicAttack * Mathf.Abs(statBoostsPercent.psychicAttack) * 0.01f) * (int)Mathf.Sign(statBoostsPercent.psychicAttack);

        self.bonusStats.piercingDefense += Mathf.CeilToInt(self.baseStats.piercingDefense * Mathf.Abs(statBoostsPercent.piercingDefense) * 0.01f) * (int)Mathf.Sign(statBoostsPercent.piercingDefense);
        self.bonusStats.bludgeoningDefense += Mathf.CeilToInt(self.baseStats.bludgeoningDefense * Mathf.Abs(statBoostsPercent.bludgeoningDefense) * 0.01f) * (int)Mathf.Sign(statBoostsPercent.bludgeoningDefense);
        self.bonusStats.slashingDefense += Mathf.CeilToInt(self.baseStats.slashingDefense * Mathf.Abs(statBoostsPercent.slashingDefense) * 0.01f) * (int)Mathf.Sign(statBoostsPercent.slashingDefense);

        self.bonusStats.fireDefense += Mathf.CeilToInt(self.baseStats.fireDefense * Mathf.Abs(statBoostsPercent.fireDefense) * 0.01f) * (int)Mathf.Sign(statBoostsPercent.fireDefense);
        self.bonusStats.iceDefense += Mathf.CeilToInt(self.baseStats.iceDefense * Mathf.Abs(statBoostsPercent.iceDefense) * 0.01f) * (int)Mathf.Sign(statBoostsPercent.iceDefense);
        self.bonusStats.stormDefense += Mathf.CeilToInt(self.baseStats.stormDefense * Mathf.Abs(statBoostsPercent.stormDefense) * 0.01f) * (int)Mathf.Sign(statBoostsPercent.stormDefense);
        self.bonusStats.bloodDefense += Mathf.CeilToInt(self.baseStats.bloodDefense * Mathf.Abs(statBoostsPercent.bloodDefense) * 0.01f) * (int)Mathf.Sign(statBoostsPercent.bloodDefense);
        self.bonusStats.acidDefense += Mathf.CeilToInt(self.baseStats.acidDefense * Mathf.Abs(statBoostsPercent.acidDefense) * 0.01f) * (int)Mathf.Sign(statBoostsPercent.acidDefense);
        self.bonusStats.psychicDefense += Mathf.CeilToInt(self.baseStats.psychicDefense * Mathf.Abs(statBoostsPercent.psychicDefense) * 0.01f) * (int)Mathf.Sign(statBoostsPercent.psychicDefense);

        self.bonusStats.critChance += Mathf.CeilToInt(self.baseStats.critChance * Mathf.Abs(statBoostsPercent.critChance) * 0.01f) * (int)Mathf.Sign(statBoostsPercent.critChance);
        self.bonusStats.critDamage += Mathf.CeilToInt(self.baseStats.critDamage * Mathf.Abs(statBoostsPercent.critDamage) * 0.01f) * (int)Mathf.Sign(statBoostsPercent.critDamage);

        self.bonusStats.accuracy += Mathf.CeilToInt(self.baseStats.accuracy * Mathf.Abs(statBoostsPercent.accuracy) * 0.01f) * (int)Mathf.Sign(statBoostsPercent.accuracy);
        self.bonusStats.evasion += Mathf.CeilToInt(self.baseStats.evasion * Mathf.Abs(statBoostsPercent.evasion) * 0.01f) * (int)Mathf.Sign(statBoostsPercent.evasion);
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

    public override void OnDestroyed(BattleCharacter self)
    {
        if(statBoostsPercent.maxHealth != 0) self.SetMaxHealth(self.MaxHealth - Mathf.CeilToInt(self.baseStats.maxHealth * Mathf.Abs(statBoostsPercent.maxHealth) * 0.01f) * (int)Mathf.Sign(statBoostsPercent.maxHealth));
        if(statBoostsPercent.maxSp != 0) self.SetMaxSp(self.MaxSp - Mathf.CeilToInt(self.baseStats.maxSp * Mathf.Abs(statBoostsPercent.maxSp) * 0.01f) * (int)Mathf.Sign(statBoostsPercent.maxSp));
    }

    public override string GetDescription(BattleCharacter character)
    {
        if (character == null) return description;
        string descriptionReturn = "";
        if (statBoostsPercent.maxHealth != 0) descriptionReturn += $"Max Health: {(statBoostsPercent.maxHealth > 0 ? "+" : "-")}{Mathf.CeilToInt(Mathf.Abs(statBoostsPercent.maxHealth) * 0.01f * character.baseStats.maxHealth)}\n";
        if (statBoostsPercent.maxSp != 0) descriptionReturn += $"Max SP: {(statBoostsPercent.maxSp > 0 ? "+" : "-")}{Mathf.CeilToInt(Mathf.Abs(statBoostsPercent.maxSp) * 0.01f * character.baseStats.maxSp)}\n";

        if (statBoostsPercent.physicalAttack != 0) descriptionReturn += $"Attack: {(statBoostsPercent.physicalAttack > 0 ? "+" : "-")}{Mathf.CeilToInt(Mathf.Abs(statBoostsPercent.physicalAttack) * 0.01f * character.baseStats.physicalAttack)}\n";
        if (statBoostsPercent.elementalPower != 0) descriptionReturn += $"Elemental Power: {(statBoostsPercent.elementalPower > 0 ? "+" : "-")}{Mathf.CeilToInt(Mathf.Abs(statBoostsPercent.elementalPower) * 0.01f * character.baseStats.elementalPower)}\n";
        if (statBoostsPercent.defense != 0) descriptionReturn += $"Defense: {(statBoostsPercent.defense > 0 ? "+" : "-")}{Mathf.CeilToInt(Mathf.Abs(statBoostsPercent.defense) * 0.01f * character.baseStats.defense)}\n";
        if (statBoostsPercent.elementalResistance != 0) descriptionReturn += $"Elemental Resistance: {(statBoostsPercent.elementalResistance > 0 ? "+" : "-")}{Mathf.CeilToInt(Mathf.Abs(statBoostsPercent.elementalResistance) * 0.01f * character.baseStats.elementalResistance)}\n";
        if (statBoostsPercent.speed != 0) descriptionReturn += $"Speed: {(statBoostsPercent.speed > 0 ? "+" : "-")}{Mathf.CeilToInt(Mathf.Abs(statBoostsPercent.speed) * 0.01f * character.baseStats.speed)}\n";

        if (statBoostsPercent.piercingAttack != 0) descriptionReturn += $"Piercing Attack: {(statBoostsPercent.piercingAttack > 0 ? "+" : "-")}{Mathf.CeilToInt(Mathf.Abs(statBoostsPercent.piercingAttack) * 0.01f * character.baseStats.piercingAttack)}\n";
        if (statBoostsPercent.bludgeoningAttack != 0) descriptionReturn += $"Bludgeoning Attack: {(statBoostsPercent.bludgeoningAttack > 0 ? "+" : "-")}{Mathf.CeilToInt(Mathf.Abs(statBoostsPercent.bludgeoningAttack) * 0.01f * character.baseStats.bludgeoningAttack)}\n";
        if (statBoostsPercent.slashingAttack != 0) descriptionReturn += $"Slashing Attack: {(statBoostsPercent.slashingAttack > 0 ? "+" : "-")}{Mathf.CeilToInt(Mathf.Abs(statBoostsPercent.slashingAttack) * 0.01f * character.baseStats.slashingAttack)}\n";

        if (statBoostsPercent.fireAttack != 0) descriptionReturn += $"Fire Attack: {(statBoostsPercent.fireAttack > 0 ? "+" : "-")}{Mathf.CeilToInt(Mathf.Abs(statBoostsPercent.fireAttack) * 0.01f * character.baseStats.fireAttack)}\n";
        if (statBoostsPercent.iceAttack != 0) descriptionReturn += $"Ice Attack: {(statBoostsPercent.iceAttack > 0 ? "+" : "-")}{Mathf.CeilToInt(Mathf.Abs(statBoostsPercent.iceAttack) * 0.01f * character.baseStats.iceAttack)}\n";
        if (statBoostsPercent.stormAttack != 0) descriptionReturn += $"Storm Attack: {(statBoostsPercent.stormAttack > 0 ? "+" : "-")}{Mathf.CeilToInt(Mathf.Abs(statBoostsPercent.stormAttack) * 0.01f * character.baseStats.stormAttack)}\n";
        if (statBoostsPercent.bloodAttack != 0) descriptionReturn += $"Blood Attack: {(statBoostsPercent.bloodAttack > 0 ? "+" : "-")}{Mathf.CeilToInt(Mathf.Abs(statBoostsPercent.bloodAttack) * 0.01f * character.baseStats.bloodAttack)}\n";
        if (statBoostsPercent.acidAttack != 0) descriptionReturn += $"Acid Attack: {(statBoostsPercent.acidAttack > 0 ? "+" : "-")}{Mathf.CeilToInt(Mathf.Abs(statBoostsPercent.acidAttack) * 0.01f * character.baseStats.acidAttack)}\n";
        if (statBoostsPercent.psychicAttack != 0) descriptionReturn += $"Psychic Attack: {(statBoostsPercent.psychicAttack > 0 ? "+" : "-")}{Mathf.CeilToInt(Mathf.Abs(statBoostsPercent.psychicAttack) * 0.01f * character.baseStats.psychicAttack)}\n";

        if (statBoostsPercent.piercingDefense != 0) descriptionReturn += $"Piercing Defense: {(statBoostsPercent.piercingDefense > 0 ? "+" : "-")}{Mathf.CeilToInt(Mathf.Abs(statBoostsPercent.piercingDefense) * 0.01f * character.baseStats.piercingDefense)}\n";
        if (statBoostsPercent.bludgeoningDefense != 0) descriptionReturn += $"Bludgeoning Defense: {(statBoostsPercent.bludgeoningDefense > 0 ? "+" : "-")}{Mathf.CeilToInt(Mathf.Abs(statBoostsPercent.bludgeoningDefense) * 0.01f * character.baseStats.bludgeoningDefense)}\n";
        if (statBoostsPercent.slashingDefense != 0) descriptionReturn += $"Slashing Defense: {(statBoostsPercent.slashingDefense > 0 ? "+" : "-")}{Mathf.CeilToInt(Mathf.Abs(statBoostsPercent.slashingDefense) * 0.01f * character.baseStats.slashingDefense)}\n";

        if (statBoostsPercent.fireDefense != 0) descriptionReturn += $"Fire Defense: {(statBoostsPercent.fireDefense > 0 ? "+" : "-")}{Mathf.CeilToInt(Mathf.Abs(statBoostsPercent.fireDefense) * 0.01f * character.baseStats.fireDefense)}\n";
        if (statBoostsPercent.iceDefense != 0) descriptionReturn += $"Ice Defense: {(statBoostsPercent.iceDefense > 0 ? "+" : "-")}{Mathf.CeilToInt(Mathf.Abs(statBoostsPercent.iceDefense) * 0.01f * character.baseStats.iceDefense)}\n";
        if (statBoostsPercent.stormDefense != 0) descriptionReturn += $"Storm Defense: {(statBoostsPercent.stormDefense > 0 ? "+" : "-")}{Mathf.CeilToInt(Mathf.Abs(statBoostsPercent.stormDefense) * 0.01f * character.baseStats.stormDefense)}\n";
        if (statBoostsPercent.bloodDefense != 0) descriptionReturn += $"Blood Defense: {(statBoostsPercent.bloodDefense > 0 ? "+" : "-")}{Mathf.CeilToInt(Mathf.Abs(statBoostsPercent.bloodDefense) * 0.01f * character.baseStats.bloodDefense)}\n";
        if (statBoostsPercent.acidDefense != 0) descriptionReturn += $"Acid Defense: {(statBoostsPercent.acidDefense > 0 ? "+" : "-")}{Mathf.CeilToInt(Mathf.Abs(statBoostsPercent.acidDefense) * 0.01f * character.baseStats.acidDefense)}\n";
        if (statBoostsPercent.psychicDefense != 0) descriptionReturn += $"Psychic Defense: {(statBoostsPercent.psychicDefense > 0 ? "+" : "-")}{Mathf.CeilToInt(Mathf.Abs(statBoostsPercent.psychicDefense) * 0.01f * character.baseStats.psychicDefense)}\n"; 


        if (statBoostsPercent.critChance != 0) descriptionReturn += $"Crit Chance: {(statBoostsPercent.critChance > 0 ? "+" : "-")}{Mathf.CeilToInt(Mathf.Abs(statBoostsPercent.critChance) * 0.01f * character.baseStats.critChance)}\n";
        if (statBoostsPercent.critDamage != 0) descriptionReturn += $"Crit Damage: {(statBoostsPercent.critDamage > 0 ? "+" : "-")}{Mathf.CeilToInt(Mathf.Abs(statBoostsPercent.critDamage) * 0.01f * character.baseStats.critDamage)}\n";
        
        if (statBoostsPercent.accuracy != 0) descriptionReturn += $"Accuracy: {(statBoostsPercent.accuracy > 0 ? "+" : "-")}{Mathf.CeilToInt(Mathf.Abs(statBoostsPercent.accuracy) * 0.01f * character.baseStats.accuracy)}\n";
        if (statBoostsPercent.evasion != 0) descriptionReturn += $"Evasion: {(statBoostsPercent.evasion > 0 ? "+" : "-")}{Mathf.CeilToInt(Mathf.Abs(statBoostsPercent.evasion) * 0.01f * character.baseStats.evasion)}\n";

        return descriptionReturn.TrimEnd('\n');
    }
}
