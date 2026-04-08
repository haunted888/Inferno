using UnityEngine;

[CreateAssetMenu(menuName = "Passives/Stat Boost Flat")]
public class StatBoostFlatPassiveDefinition : PassivesDefinition
{
    public CombatStats statBoosts;

    public int duration = int.MaxValue; // Duration in turns, default to max for "until end of battle" behavior


    public void SetStatBoosts(
        CombatStats statBoostFlat)
    {
        statBoosts = statBoostFlat;
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
        
        
        self.bonusStats.maxHealth += statBoosts.maxHealth;
        self.bonusStats.maxSp += statBoosts.maxSp;
        self.bonusStats.physicalAttack += statBoosts.physicalAttack;
        self.bonusStats.elementalPower += statBoosts.elementalPower;
        self.bonusStats.defense += statBoosts.defense;
        self.bonusStats.elementalResistance += statBoosts.elementalResistance;
        self.bonusStats.speed += statBoosts.speed;
        self.bonusStats.critChance += statBoosts.critChance;
        self.bonusStats.critDamage += statBoosts.critDamage;

        self.bonusStats.piercingAttack += statBoosts.piercingAttack;
        self.bonusStats.bludgeoningAttack += statBoosts.bludgeoningAttack;
        self.bonusStats.slashingAttack += statBoosts.slashingAttack;

        self.bonusStats.fireAttack += statBoosts.fireAttack;
        self.bonusStats.iceAttack += statBoosts.iceAttack;
        self.bonusStats.stormAttack += statBoosts.stormAttack;
        self.bonusStats.bloodAttack += statBoosts.bloodAttack;
        self.bonusStats.acidAttack += statBoosts.acidAttack;
        self.bonusStats.psychicAttack += statBoosts.psychicAttack;

        self.bonusStats.piercingDefense += statBoosts.piercingDefense;
        self.bonusStats.bludgeoningDefense += statBoosts.bludgeoningDefense;
        self.bonusStats.slashingDefense += statBoosts.slashingDefense;

        self.bonusStats.fireDefense += statBoosts.fireDefense;
        self.bonusStats.iceDefense += statBoosts.iceDefense;
        self.bonusStats.stormDefense += statBoosts.stormDefense;
        self.bonusStats.bloodDefense += statBoosts.bloodDefense;
        self.bonusStats.acidDefense += statBoosts.acidDefense;
        self.bonusStats.psychicDefense += statBoosts.psychicDefense;

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
        if (statBoosts.maxHealth != 0) descriptionReturn += $"Max Health {(statBoosts.maxHealth > 0 ? "+" : "-")}{statBoosts.maxHealth}\n";
        if (statBoosts.maxSp != 0) descriptionReturn += $"Max SP {(statBoosts.maxSp > 0 ? "+" : "-")}{statBoosts.maxSp}\n";
        if (statBoosts.physicalAttack != 0) descriptionReturn += $"Physical Attack {(statBoosts.physicalAttack > 0 ? "+" : "-")}{statBoosts.physicalAttack}\n";
        if (statBoosts.elementalPower != 0) descriptionReturn += $"Elemental Power {(statBoosts.elementalPower > 0 ? "+" : "-")}{statBoosts.elementalPower}\n";
        if (statBoosts.defense != 0) descriptionReturn += $"Defense {(statBoosts.defense > 0 ? "+" : "-")}{statBoosts.defense}\n";
        if (statBoosts.elementalResistance != 0) descriptionReturn += $"Elemental Resistance {(statBoosts.elementalResistance > 0 ? "+" : "-")}{statBoosts.elementalResistance}\n";
        if (statBoosts.speed != 0) descriptionReturn += $"Speed {(statBoosts.speed > 0 ? "+" : "-")}{statBoosts.speed}\n";
        if (statBoosts.critChance != 0) descriptionReturn += $"Crit Chance {(statBoosts.critChance > 0 ? "+" : "-")}{statBoosts.critChance}%\n";
        if (statBoosts.critDamage != 0) descriptionReturn += $"Crit Damage {(statBoosts.critDamage > 0 ? "+" : "-")}{statBoosts.critDamage}%\n";

        if (statBoosts.piercingAttack != 0) descriptionReturn += $"Piercing Attack {(statBoosts.piercingAttack > 0 ? "+" : "-")}{statBoosts.piercingAttack}\n";
        if (statBoosts.bludgeoningAttack != 0) descriptionReturn += $"Bludgeoning Attack {(statBoosts.bludgeoningAttack > 0 ? "+" : "-")}{statBoosts.bludgeoningAttack}\n";
        if (statBoosts.slashingAttack != 0) descriptionReturn += $"Slashing Attack {(statBoosts.slashingAttack > 0 ? "+" : "-")}{statBoosts.slashingAttack}\n";
        
        if (statBoosts.fireAttack != 0) descriptionReturn += $"Fire Attack {(statBoosts.fireAttack > 0 ? "+" : "-")}{statBoosts.fireAttack}\n";
        if (statBoosts.iceAttack != 0) descriptionReturn += $"Ice Attack {(statBoosts.iceAttack > 0 ? "+" : "-")}{statBoosts.iceAttack}\n";
        if (statBoosts.stormAttack != 0) descriptionReturn += $"Storm Attack {(statBoosts.stormAttack > 0 ? "+" : "-")}{statBoosts.stormAttack}\n";
        if (statBoosts.bloodAttack != 0) descriptionReturn += $"Blood Attack {(statBoosts.bloodAttack > 0 ? "+" : "-")}{statBoosts.bloodAttack}\n";
        if (statBoosts.acidAttack != 0) descriptionReturn += $"Acid Attack {(statBoosts.acidAttack > 0 ? "+" : "-")}{statBoosts.acidAttack}\n";
        if (statBoosts.psychicAttack != 0) descriptionReturn += $"Psychic Attack {(statBoosts.psychicAttack > 0 ? "+" : "-")}{statBoosts.psychicAttack}\n";

        if (statBoosts.piercingDefense != 0) descriptionReturn += $"Piercing Defense {(statBoosts.piercingDefense > 0 ? "+" : "-")}{statBoosts.piercingDefense}\n";
        if (statBoosts.bludgeoningDefense != 0) descriptionReturn += $"Bludgeoning Defense {(statBoosts.bludgeoningDefense > 0 ? "+" : "-")}{statBoosts.bludgeoningDefense}\n";
        if (statBoosts.slashingDefense != 0) descriptionReturn += $"Slashing Defense {(statBoosts.slashingDefense > 0 ? "+" : "-")}{statBoosts.slashingDefense}\n";

        if (statBoosts.fireDefense != 0) descriptionReturn += $"Fire Defense {(statBoosts.fireDefense > 0 ? "+" : "-")}{statBoosts.fireDefense}\n";
        if (statBoosts.iceDefense != 0) descriptionReturn += $"Ice Defense {( statBoosts.iceDefense > 0 ? "+" : "-")}{statBoosts.iceDefense}\n";
        if (statBoosts.stormDefense != 0) descriptionReturn += $"Storm Defense {(statBoosts.stormDefense > 0 ? "+" : "-")}{statBoosts.stormDefense}\n";
        if (statBoosts.bloodDefense != 0) descriptionReturn += $"Blood Defense {(statBoosts.bloodDefense > 0 ? "+" : "-")}{statBoosts.bloodDefense}\n";
        if (statBoosts.acidDefense != 0) descriptionReturn += $"Acid Defense {(statBoosts.acidDefense > 0 ? "+" : "-")}{statBoosts.acidDefense}\n";
        if (statBoosts.psychicDefense != 0) descriptionReturn += $"Psychic Defense {(statBoosts.psychicDefense > 0 ? "+" : "-")}{statBoosts.psychicDefense}\n";

        // Add other stats as needed

        return descriptionReturn.TrimEnd(); // Remove trailing newline
    }
}
