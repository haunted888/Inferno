using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapEnemyDefinition
{
    public GameObject enemyPrefab;

    [Header("Display Name")]
    public string displayName = "Unnamed";

    
    [Header("Progression")]
    public const int MaxLevel = 20;
    [Range(1, MaxLevel)] public int level = 1;

    [Header("Optional Character Asset")]
    public CharacterTemplate characterAsset;

    [Header("Inspector Overrides")]
    public bool overrideStats  = false;
    public bool overrideSkills = false;

    [Header("Stats (max values, editable)")]
    public CombatStats stats = new CombatStats
    {
        maxHealth          = 50,
        maxSp              = 10,
        spGeneration       = 20,
        speed              = 10,
        physicalAttack     = 100,
        elementalPower     = 100,
        defense            = 0,
        elementalResistance= 0,
        critChance         = 5,
        critDamage         = 150
    };

    [Header("Skills (used if overrideSkills = true or no asset)")]
    public List<Skill> skills;

    [HideInInspector] public bool initializedFromAssetStats  = false;
    [HideInInspector] public bool initializedFromAssetSkills = false;

    
    private CombatStats baseStats = new CombatStats();
    private CombatStats levelUpStats = new CombatStats();

    public PassivesDefinition[] passives; // optional; can be empty

    public void EnsureInitializedFromAsset()
    {
        if (characterAsset == null)
            return;

        if (!initializedFromAssetStats && !overrideStats)
        {
            stats = characterAsset.baseStats;
            for(int i = 1; i < level; i++) // apply level-up effects for each level up to current level
                ApplyLevelUpEffects(i); 
            stats.maxHealth = Mathf.Max(1, stats.maxHealth);

            initializedFromAssetStats = true;
        }
        baseStats = stats;

        if (!initializedFromAssetSkills && !overrideSkills &&
            characterAsset.skills != null && characterAsset.skills.Count > 0)
        {
            skills = new List<Skill>(characterAsset.skills);
            initializedFromAssetSkills = true;
        }
    }

    public int GetMaxHealth()
    {
        return Mathf.Max(1, stats.maxHealth);
    }

    public CombatStats GetEffectiveStats()
    {
        var result = stats;
        result.maxHealth = Mathf.Max(1, result.maxHealth);
        return result;
    }

    public List<Skill> GetEffectiveSkills()
    {
        return skills ?? new List<Skill>();
    }

    public int GetMaxSp()
    {
        return Mathf.Max(0, stats.maxSp);
    }
    public string GetDisplayName()
    {
        if (!string.IsNullOrEmpty(displayName))
            return displayName;

        if (characterAsset != null && !string.IsNullOrEmpty(characterAsset.displayName))
            return characterAsset.displayName;

        return "Unnamed";
    }

    private readonly float levelUpBaseMultiplier = .06f;
    private readonly float levelUpLevelMultiplier = 0.02f;
    public void ApplyLevelUpEffects()
    {

        // apply level-up stats
        CombatStats delta = new CombatStats
        {
            maxHealth          = Mathf.RoundToInt(baseStats.maxHealth * (levelUpBaseMultiplier + level * levelUpLevelMultiplier)),
            maxSp              = Mathf.RoundToInt(baseStats.maxSp * (levelUpBaseMultiplier + level * levelUpLevelMultiplier)),
            speed              = Mathf.RoundToInt(baseStats.speed * (levelUpBaseMultiplier + level * levelUpLevelMultiplier)),
            physicalAttack     = Mathf.RoundToInt(baseStats.physicalAttack * (levelUpBaseMultiplier + level * levelUpLevelMultiplier)),
            elementalPower     = Mathf.RoundToInt(baseStats.elementalPower * (levelUpBaseMultiplier + level * levelUpLevelMultiplier)),
            defense            = Mathf.RoundToInt(baseStats.defense * (levelUpBaseMultiplier + level * levelUpLevelMultiplier)),
            elementalResistance= Mathf.RoundToInt(baseStats.elementalResistance * (levelUpBaseMultiplier + level * levelUpLevelMultiplier)),

            fireAttack           = Mathf.RoundToInt(baseStats.fireAttack * (levelUpBaseMultiplier + level * levelUpLevelMultiplier)),
            iceAttack            = Mathf.RoundToInt(baseStats.iceAttack * (levelUpBaseMultiplier + level * levelUpLevelMultiplier)),
            stormAttack          = Mathf.RoundToInt(baseStats.stormAttack * (levelUpBaseMultiplier + level * levelUpLevelMultiplier)),
            acidAttack           = Mathf.RoundToInt(baseStats.acidAttack * (levelUpBaseMultiplier + level * levelUpLevelMultiplier)),
            psychicAttack        = Mathf.RoundToInt(baseStats.psychicAttack * (levelUpBaseMultiplier + level * levelUpLevelMultiplier)),
            bloodAttack          = Mathf.RoundToInt(baseStats.bloodAttack * (levelUpBaseMultiplier + level * levelUpLevelMultiplier)),

            fireDefense          = Mathf.RoundToInt(baseStats.fireDefense * (levelUpBaseMultiplier + level * levelUpLevelMultiplier)),
            iceDefense           = Mathf.RoundToInt(baseStats.iceDefense * (levelUpBaseMultiplier + level * levelUpLevelMultiplier)),
            stormDefense         = Mathf.RoundToInt(baseStats.stormDefense * (levelUpBaseMultiplier + level * levelUpLevelMultiplier)),
            acidDefense          = Mathf.RoundToInt(baseStats.acidDefense * (levelUpBaseMultiplier + level * levelUpLevelMultiplier)),
            psychicDefense       = Mathf.RoundToInt(baseStats.psychicDefense * (levelUpBaseMultiplier + level * levelUpLevelMultiplier)),
            bloodDefense         = Mathf.RoundToInt(baseStats.bloodDefense * (levelUpBaseMultiplier + level * levelUpLevelMultiplier))
        };

        ApplyStatsDelta(delta, +1);

        levelUpStats.maxHealth += delta.maxHealth;
        levelUpStats.maxSp     += delta.maxSp;
        levelUpStats.speed     += delta.speed;
        levelUpStats.physicalAttack     += delta.physicalAttack;
        levelUpStats.elementalPower     += delta.elementalPower;
        levelUpStats.defense              += delta.defense;
        levelUpStats.elementalResistance  += delta.elementalResistance;
        levelUpStats.critChance           += delta.critChance;
        levelUpStats.critDamage           += delta.critDamage;

        
    }

    public void ApplyLevelUpEffects(int level)
    {

        // apply level-up stats
        CombatStats delta = new CombatStats
        {
            maxHealth          = Mathf.RoundToInt(baseStats.maxHealth * (levelUpBaseMultiplier + level * levelUpLevelMultiplier)),
            maxSp              = Mathf.RoundToInt(baseStats.maxSp * (levelUpBaseMultiplier + level * levelUpLevelMultiplier)),
            speed              = Mathf.RoundToInt(baseStats.speed * (levelUpBaseMultiplier + level * levelUpLevelMultiplier)),
            physicalAttack     = Mathf.RoundToInt(baseStats.physicalAttack * (levelUpBaseMultiplier + level * levelUpLevelMultiplier)),
            elementalPower     = Mathf.RoundToInt(baseStats.elementalPower * (levelUpBaseMultiplier + level * levelUpLevelMultiplier)),
            defense            = Mathf.RoundToInt(baseStats.defense * (levelUpBaseMultiplier + level * levelUpLevelMultiplier)),
            elementalResistance= Mathf.RoundToInt(baseStats.elementalResistance * (levelUpBaseMultiplier + level * levelUpLevelMultiplier)),

            fireAttack           = Mathf.RoundToInt(baseStats.fireAttack * (levelUpBaseMultiplier + level * levelUpLevelMultiplier)),
            iceAttack            = Mathf.RoundToInt(baseStats.iceAttack * (levelUpBaseMultiplier + level * levelUpLevelMultiplier)),
            stormAttack          = Mathf.RoundToInt(baseStats.stormAttack * (levelUpBaseMultiplier + level * levelUpLevelMultiplier)),
            acidAttack           = Mathf.RoundToInt(baseStats.acidAttack * (levelUpBaseMultiplier + level * levelUpLevelMultiplier)),
            psychicAttack        = Mathf.RoundToInt(baseStats.psychicAttack * (levelUpBaseMultiplier + level * levelUpLevelMultiplier)),
            bloodAttack          = Mathf.RoundToInt(baseStats.bloodAttack * (levelUpBaseMultiplier + level * levelUpLevelMultiplier)),

            fireDefense          = Mathf.RoundToInt(baseStats.fireDefense * (levelUpBaseMultiplier + level * levelUpLevelMultiplier)),
            iceDefense           = Mathf.RoundToInt(baseStats.iceDefense * (levelUpBaseMultiplier + level * levelUpLevelMultiplier)),
            stormDefense         = Mathf.RoundToInt(baseStats.stormDefense * (levelUpBaseMultiplier + level * levelUpLevelMultiplier)),
            acidDefense          = Mathf.RoundToInt(baseStats.acidDefense * (levelUpBaseMultiplier + level * levelUpLevelMultiplier)),
            psychicDefense       = Mathf.RoundToInt(baseStats.psychicDefense * (levelUpBaseMultiplier + level * levelUpLevelMultiplier)),
            bloodDefense         = Mathf.RoundToInt(baseStats.bloodDefense * (levelUpBaseMultiplier + level * levelUpLevelMultiplier))
        };

        ApplyStatsDelta(delta, +1);

        levelUpStats.maxHealth += delta.maxHealth;
        levelUpStats.maxSp     += delta.maxSp;
        levelUpStats.speed     += delta.speed;
        levelUpStats.physicalAttack     += delta.physicalAttack;
        levelUpStats.elementalPower     += delta.elementalPower;
        levelUpStats.defense              += delta.defense;
        levelUpStats.elementalResistance  += delta.elementalResistance;
        levelUpStats.critChance           += delta.critChance;
        levelUpStats.critDamage           += delta.critDamage;

        
    }

    void ApplyStatsDelta(CombatStats d, int sign = 1)
    {
        if (d.Equals(null)) return;

        stats.maxHealth            += sign * d.maxHealth;
        stats.maxSp                += sign * d.maxSp;
        stats.speed                += sign * d.speed;
        stats.physicalAttack       += sign * d.physicalAttack;
        stats.elementalPower       += sign * d.elementalPower;
        stats.defense              += sign * d.defense;
        stats.elementalResistance  += sign * d.elementalResistance;
        stats.critChance           += sign * d.critChance;
        stats.critDamage           += sign * d.critDamage;

        stats.bludgeoningAttack    += sign * d.bludgeoningAttack;
        stats.slashingAttack       += sign * d.slashingAttack;
        stats.piercingAttack       += sign * d.piercingAttack;

        stats.bludgeoningDefense   += sign * d.bludgeoningDefense;
        stats.slashingDefense      += sign * d.slashingDefense;
        stats.piercingDefense      += sign * d.piercingDefense;

        stats.fireAttack           += sign * d.fireAttack;
        stats.iceAttack            += sign * d.iceAttack;
        stats.stormAttack          += sign * d.stormAttack;
        stats.acidAttack           += sign * d.acidAttack;
        stats.psychicAttack        += sign * d.psychicAttack;
        stats.bloodAttack          += sign * d.bloodAttack;

        stats.fireDefense          += sign * d.fireDefense;
        stats.iceDefense           += sign * d.iceDefense;
        stats.stormDefense         += sign * d.stormDefense;
        stats.acidDefense          += sign * d.acidDefense;
        stats.psychicDefense       += sign * d.psychicDefense;
        stats.bloodDefense         += sign * d.bloodDefense;
    }

    public void ResetProgression()
    {
        level = 1;

        ApplyStatsDelta(levelUpStats, -1);
        levelUpStats = new CombatStats();

    }

    
}
