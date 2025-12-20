using System;
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class MapPartyMemberDefinition
{
    public GameObject characterPrefab;

    public string displayName = "Unnamed";

    [Header("Character Asset")]
    public CharacterTemplate characterAsset;

    [Header("Traits")]

    [Header("Inspector Overrides")]
    public bool overrideTraits = false;
    public bool overrideStats  = false;
    public bool overrideSkills = false;

    [Header("Traits (used if overrideTraits = true or no asset)")]
    public List<TraitDefinition> traits = new List<TraitDefinition>();
    [NonSerialized] public List<CharacterTrait> traitTypes = new List<CharacterTrait>();

    [Header("Stats (max values, editable)")]
    public CombatStats stats = new CombatStats
    {
        maxHealth          = 100,
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

    [NonSerialized] public int health = -1;
    [NonSerialized] public int sp     = -1; 

    
    [NonSerialized] public bool initializedFromAssetTraits  = false;
    [NonSerialized] public bool initializedFromAssetStats  = false;
    [NonSerialized] public bool initializedFromAssetSkills = false;

    public PassivesDefinition[] passives; // optional; can be empty


    // --- Talents ---
    [Header("Talent Tree")]
    public TalentTreeUIController talentTreePrefab;   // assign a prefab that has TalentTreeUIController on root

    [Header("Talents")]
    public int talentPoints = 5;                             // default pool per character
    public List<string> learnedTalentIds
        = new List<string>();

    public bool HasTalent(string talentId) =>
        !string.IsNullOrEmpty(talentId) && learnedTalentIds.Contains(talentId);

    public bool CanLearn(TalentDefinition t) =>
        t != null && !HasTalent(t.id) && talentPoints >= t.cost;

    public void EnsureInitializedFromAsset()
    {
        if (characterAsset == null)
            return;

        if (!initializedFromAssetTraits && !overrideTraits)
        {
            traits = new List<TraitDefinition>(characterAsset.traits);
            traitTypes = new List<CharacterTrait>();
            foreach (var t in traits)
                traitTypes.Add(t.traitType);
            initializedFromAssetTraits  = true;
        }

        if (!initializedFromAssetStats && !overrideStats)
        {
            stats = characterAsset.baseStats;
            stats.maxHealth = Mathf.Max(1, stats.maxHealth);
            stats.maxSp     = Mathf.Max(0, stats.maxSp);

            initializedFromAssetStats = true;
        }

        if (!initializedFromAssetSkills && !overrideSkills &&
            characterAsset.skills != null && characterAsset.skills.Count > 0)
        {
            skills = new List<Skill>(characterAsset.skills);
            initializedFromAssetSkills = true;
        }
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
    
    public bool HasTrait(CharacterTrait trait)
    {
        return traitTypes != null && traitTypes.Contains(trait);
    }
    public List<CharacterTrait> GetEffectiveTraits()
    {
        return traitTypes ?? new List<CharacterTrait>();
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


    public void LearnTalent(TalentDefinition t)
    {
        if (!CanLearn(t)) return;

        // stats
        ApplyStatsDelta(t.statBonus, +1);

        // skills
        if (t.grantSkills != null && t.grantSkills.Length > 0)
        {
            var list = skills != null ? new List<Skill>(skills)
                                    : new List<Skill>();
            foreach (var s in t.grantSkills)
                if (s != null && !list.Contains(s)) list.Add(s);
            skills = list;
        }

        // passives (array may not exist yet)
        if (t.grantPassives != null && t.grantPassives.Length > 0)
        {
            var list = passives != null ? new List<PassivesDefinition>(passives)
                                        : new List<PassivesDefinition>();
            foreach (var p in t.grantPassives)
                if (p != null && !list.Contains(p)) list.Add(p);
            passives = list.ToArray();
        }

        learnedTalentIds.Add(t.id);
        talentPoints -= t.cost;
    }

    public void UnlearnTalent(TalentDefinition t)
    {
        if (t == null || !HasTalent(t.id)) return;

        // reverse stats
        ApplyStatsDelta(t.statBonus, -1);

        // remove granted skills/passives if present
        if (t.grantSkills != null && t.grantSkills.Length > 0 && skills != null)
        {
            var list = new List<Skill>(skills);
            foreach (var s in t.grantSkills) if (s != null) list.Remove(s);
            skills = list;
        }
        if (t.grantPassives != null && t.grantPassives.Length > 0 && passives != null)
        {
            var list = new List<PassivesDefinition>(passives);
            foreach (var p in t.grantPassives) if (p != null) list.Remove(p);
            passives = list.ToArray();
        }

        learnedTalentIds.Remove(t.id);
        talentPoints += t.cost;
    }

    // helper: adds (sign=+1) or subtracts (sign=-1) all fields of CombatStats
    void ApplyStatsDelta(CombatStats d, int sign)
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
    public void InitializeTalentPointsIfFresh(int defaultPoints = 5)
    {
        // only set if they're “fresh” (no prior spends) and points are non-positive
        if ((learnedTalentIds == null || learnedTalentIds.Count == 0) && talentPoints <= 0)
            talentPoints = defaultPoints;
    }

}
