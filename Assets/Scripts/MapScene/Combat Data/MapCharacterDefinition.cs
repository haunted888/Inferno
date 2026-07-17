using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapCharacterDefinition
{
    public string displayName = "Unnamed";

    [Header("Character Asset")]
    public CharacterTemplate characterAsset;

    [Header("Inspector Overrides")]
    public bool overrideStats = false;
    public bool overrideSkills = false;

    [Header("Stats (max values, editable)")]
    public CombatStats stats = new CombatStats
    {
        maxHealth = 100,
        maxSp = 10,
        spGeneration = 20,
        speed = 10,
        physicalAttack = 100,
        elementalPower = 100,
        defense = 0,
        elementalResistance = 0,
        critChance = 5,
        critDamage = 150
    };

    [Header("Skills (used if overrideSkills = true or no asset)")]
    public List<Skill> skills = new List<Skill>();

    [Header("Progression")]
    [Range(1, 20)] public int level = 1;

    [NonSerialized] public bool initializedFromAssetStats = false;
    [NonSerialized] public bool initializedFromAssetSkills = false;

    public PassivesDefinition[] passives = Array.Empty<PassivesDefinition>();

    protected CombatStats baseStats = new CombatStats();
    protected CombatStats levelUpStats = new CombatStats();

    protected virtual float LevelUpBaseMultiplier => 0.06f;
    protected virtual float LevelUpLevelMultiplier => 0.02f;
    protected virtual bool IncludeDamageTypeStatsInLevelUps => false;

    public virtual void EnsureInitializedFromAsset()
    {
        if (characterAsset == null)
            return;

        InitializeStatsFromAsset();
        InitializeSkillsFromAsset();
    }

    protected virtual void InitializeStatsFromAsset()
    {
        if (initializedFromAssetStats)
            return;

        if (!overrideStats)
        {
            stats = characterAsset.baseStats;
            ClampStats();
        }

        baseStats = stats;
        levelUpStats = new CombatStats();
        initializedFromAssetStats = true;
    }

    protected void InitializeSkillsFromAsset()
    {
        if (!initializedFromAssetSkills &&
            !overrideSkills &&
            characterAsset != null &&
            characterAsset.skills != null &&
            characterAsset.skills.Count > 0)
        {
            skills = new List<Skill>(characterAsset.skills);
            initializedFromAssetSkills = true;
        }
    }

    protected void ClampStats()
    {
        stats.maxHealth = Mathf.Max(1, stats.maxHealth);
        stats.maxSp = Mathf.Max(0, stats.maxSp);
    }

    public string GetDisplayName()
    {
        if (!string.IsNullOrEmpty(displayName))
            return displayName;

        if (characterAsset != null && !string.IsNullOrEmpty(characterAsset.displayName))
            return characterAsset.displayName;

        return "Unnamed";
    }

    public int GetMaxHealth()
    {
        return Mathf.Max(1, stats.maxHealth);
    }

    public int GetMaxSp()
    {
        return Mathf.Max(0, stats.maxSp);
    }

    public CombatStats GetEffectiveStats()
    {
        var result = stats;
        result.maxHealth = Mathf.Max(1, result.maxHealth);
        return result;
    }

    public virtual List<Skill> GetEffectiveSkills()
    {
        return skills ?? new List<Skill>();
    }

    public virtual void ApplyLevelUpEffects()
    {
        ApplyLevelUpEffects(level);
    }

    public virtual void ApplyLevelUpEffects(int level)
    {
        CombatStats delta = BuildLevelUpDelta(level);

        ApplyStatsDelta(delta, +1);
        AddToLevelUpStats(delta);
        OnLevelUpDeltaApplied(delta);
    }

    protected virtual CombatStats BuildLevelUpDelta(int level)
    {
        float multiplier = LevelUpBaseMultiplier + level * LevelUpLevelMultiplier;

        var delta = new CombatStats
        {
            maxHealth = Mathf.RoundToInt(baseStats.maxHealth * multiplier),
            maxSp = Mathf.RoundToInt(baseStats.maxSp * multiplier),
            speed = Mathf.RoundToInt(baseStats.speed * multiplier),
            physicalAttack = Mathf.RoundToInt(baseStats.physicalAttack * multiplier),
            elementalPower = Mathf.RoundToInt(baseStats.elementalPower * multiplier),
            defense = Mathf.RoundToInt(baseStats.defense * multiplier),
            elementalResistance = Mathf.RoundToInt(baseStats.elementalResistance * multiplier)
        };

        if (IncludeDamageTypeStatsInLevelUps)
        {
            delta.fireAttack = Mathf.RoundToInt(baseStats.fireAttack * multiplier);
            delta.iceAttack = Mathf.RoundToInt(baseStats.iceAttack * multiplier);
            delta.stormAttack = Mathf.RoundToInt(baseStats.stormAttack * multiplier);
            delta.acidAttack = Mathf.RoundToInt(baseStats.acidAttack * multiplier);
            delta.psychicAttack = Mathf.RoundToInt(baseStats.psychicAttack * multiplier);
            delta.bloodAttack = Mathf.RoundToInt(baseStats.bloodAttack * multiplier);

            delta.fireDefense = Mathf.RoundToInt(baseStats.fireDefense * multiplier);
            delta.iceDefense = Mathf.RoundToInt(baseStats.iceDefense * multiplier);
            delta.stormDefense = Mathf.RoundToInt(baseStats.stormDefense * multiplier);
            delta.acidDefense = Mathf.RoundToInt(baseStats.acidDefense * multiplier);
            delta.psychicDefense = Mathf.RoundToInt(baseStats.psychicDefense * multiplier);
            delta.bloodDefense = Mathf.RoundToInt(baseStats.bloodDefense * multiplier);
        }

        return delta;
    }

    protected virtual void OnLevelUpDeltaApplied(CombatStats delta) { }

    protected void AddToLevelUpStats(CombatStats delta)
    {
        levelUpStats.maxHealth += delta.maxHealth;
        levelUpStats.maxSp += delta.maxSp;
        levelUpStats.speed += delta.speed;
        levelUpStats.physicalAttack += delta.physicalAttack;
        levelUpStats.elementalPower += delta.elementalPower;
        levelUpStats.defense += delta.defense;
        levelUpStats.elementalResistance += delta.elementalResistance;
        levelUpStats.critChance += delta.critChance;
        levelUpStats.critDamage += delta.critDamage;

        levelUpStats.bludgeoningAttack += delta.bludgeoningAttack;
        levelUpStats.slashingAttack += delta.slashingAttack;
        levelUpStats.piercingAttack += delta.piercingAttack;
        levelUpStats.bludgeoningDefense += delta.bludgeoningDefense;
        levelUpStats.slashingDefense += delta.slashingDefense;
        levelUpStats.piercingDefense += delta.piercingDefense;
        levelUpStats.fireAttack += delta.fireAttack;
        levelUpStats.iceAttack += delta.iceAttack;
        levelUpStats.stormAttack += delta.stormAttack;
        levelUpStats.acidAttack += delta.acidAttack;
        levelUpStats.psychicAttack += delta.psychicAttack;
        levelUpStats.bloodAttack += delta.bloodAttack;
        levelUpStats.fireDefense += delta.fireDefense;
        levelUpStats.iceDefense += delta.iceDefense;
        levelUpStats.stormDefense += delta.stormDefense;
        levelUpStats.acidDefense += delta.acidDefense;
        levelUpStats.psychicDefense += delta.psychicDefense;
        levelUpStats.bloodDefense += delta.bloodDefense;
    }

    protected void ApplyStatsDelta(CombatStats d, int sign = 1)
    {
        if (d.Equals(null)) return;

        stats.maxHealth += sign * d.maxHealth;
        stats.maxSp += sign * d.maxSp;
        stats.speed += sign * d.speed;
        stats.physicalAttack += sign * d.physicalAttack;
        stats.elementalPower += sign * d.elementalPower;
        stats.defense += sign * d.defense;
        stats.elementalResistance += sign * d.elementalResistance;
        stats.critChance += sign * d.critChance;
        stats.critDamage += sign * d.critDamage;

        stats.bludgeoningAttack += sign * d.bludgeoningAttack;
        stats.slashingAttack += sign * d.slashingAttack;
        stats.piercingAttack += sign * d.piercingAttack;

        stats.bludgeoningDefense += sign * d.bludgeoningDefense;
        stats.slashingDefense += sign * d.slashingDefense;
        stats.piercingDefense += sign * d.piercingDefense;

        stats.fireAttack += sign * d.fireAttack;
        stats.iceAttack += sign * d.iceAttack;
        stats.stormAttack += sign * d.stormAttack;
        stats.acidAttack += sign * d.acidAttack;
        stats.psychicAttack += sign * d.psychicAttack;
        stats.bloodAttack += sign * d.bloodAttack;

        stats.fireDefense += sign * d.fireDefense;
        stats.iceDefense += sign * d.iceDefense;
        stats.stormDefense += sign * d.stormDefense;
        stats.acidDefense += sign * d.acidDefense;
        stats.psychicDefense += sign * d.psychicDefense;
        stats.bloodDefense += sign * d.bloodDefense;
    }

    public virtual void ResetProgression()
    {
        level = 1;
        ApplyStatsDelta(levelUpStats, -1);
        levelUpStats = new CombatStats();
    }

    public void AddPassive(PassivesDefinition passive)
    {
        if (passive == null) return;

        var newPassives = new List<PassivesDefinition>(passives)
        {
            passive
        };
        passives = newPassives.ToArray();
    }

    public void RemovePassive(PassivesDefinition passive)
    {
        if (passive == null) return;

        var newPassives = new List<PassivesDefinition>(passives);
        newPassives.Remove(passive);
        passives = newPassives.ToArray();
    }
}
