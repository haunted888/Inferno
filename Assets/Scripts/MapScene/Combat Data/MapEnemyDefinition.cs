using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapEnemyDefinition
{
    public GameObject enemyPrefab;

    [Header("Display Name")]
    public string displayName = "Unnamed";

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

    public PassivesDefinition[] passives; // optional; can be empty

    public void EnsureInitializedFromAsset()
    {
        if (characterAsset == null)
            return;

        if (!initializedFromAssetStats && !overrideStats)
        {
            stats = characterAsset.baseStats;
            stats.maxHealth = Mathf.Max(1, stats.maxHealth);

            initializedFromAssetStats = true;
        }

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
}
