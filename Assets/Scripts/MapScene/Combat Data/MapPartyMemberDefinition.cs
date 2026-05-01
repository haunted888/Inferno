using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using NUnit.Framework.Internal;

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

    [NonSerialized] public int[] levelXpRequired =
    {
        0, // level 0 not used
        0, // level 1
        100, // level 2
        150, // level 3
        200, // level 4
        300, // level 5
        400, // level 6
        550, // level 7
        700, // level 8
        850, // level 9
        1000, // level 10
        1200, // level 11
        1400, // level 12
        1600, // level 13
    };

    [Header("Stats (max values, editable)")]
    public CombatStats stats = new CombatStats
    {
        maxHealth          = 100,
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

    private CombatStats baseStats = new CombatStats();
    private CombatStats levelUpStats = new CombatStats();

    [Header("Skills (used if overrideSkills = true or no asset)")]
    public List<Skill> skills;

    
    [Header("Progression")]
    public const int MaxLevel = 13;
    [Range(1, MaxLevel)] public int level = 1;
    [Min(0)] public int currentXp = 0;

    [Header("Bonus stat")]
    private int bonusStatsValue = 20;
    private int bonusStatsMin = 2;
    private int bonusStatsMax = 10;

    [NonSerialized] public int health = -1;
    [NonSerialized] public int sp     = -1; 

    
    [NonSerialized] public bool initializedFromAssetTraits  = false;
    [NonSerialized] public bool initializedFromAssetStats  = false;
    [NonSerialized] public bool initializedFromAssetSkills = false;
    [NonSerialized] public bool initializedTalents = false;

    public PassivesDefinition[] passives; // optional; can be empty


    // --- Talents ---
    [Header("Talent Tree")]
    public TalentTreeUIController talentTreePrefab;   // assign a prefab that has TalentTreeUIController on root

    [Header("Talents")]
    public int talentPoints = 1;                             // default pool per character
    public List<string> learnedTalentIds
        = new List<string>();

    [Header("Substats (initialized once)")]
    public List<CombatSubStat> mainSubStats = new List<CombatSubStat>(3);
    public List<CombatSubStat> subSubStats  = new List<CombatSubStat>(3);

    [NonSerialized] public bool initializedSubStats = false;


    public bool HasTalent(string talentId) =>
        !string.IsNullOrEmpty(talentId) && learnedTalentIds.Contains(talentId);

    public bool CanLearn(TalentDefinition t) =>
        t != null && !HasTalent(t.id) && talentPoints >= t.cost;

    void Awake()
    {
        SetupLevelMultiplierHashtable();
    }

    public void EnsureInitializedFromAsset()
    {
        if (characterAsset == null)
            return;


        if (!initializedFromAssetStats)
        {
            
            if(!overrideStats){
                stats = characterAsset.baseStats;
                Debug.Log(displayName + " max sp is " + stats.maxSp);
                stats.maxHealth = Mathf.Max(1, stats.maxHealth);
                stats.maxSp     = Mathf.Max(0, stats.maxSp);
            }

            baseStats = stats;
            levelUpStats = new CombatStats();

            initializedFromAssetStats = true;
        }
            

        if (!initializedFromAssetSkills && !overrideSkills &&
            characterAsset.skills != null && characterAsset.skills.Count > 0)
        {
            skills = new List<Skill>(characterAsset.skills);
            initializedFromAssetSkills = true;
        }
        
        if (!initializedSubStats)
        {
            InitializeSubStatsFromTemplate();
            initializedSubStats = true;
        }

        if (!initializedTalents)
        {
            InitializeTalents();
            initializedTalents = true;
        }

        
        if (!initializedFromAssetTraits) //Necessary to run after every other initialization since traits modify stats/skills
        {
            if(!overrideTraits){
                traits = new List<TraitDefinition>(characterAsset.traits);
                traitTypes = new List<CharacterTrait>();
                
            }
            foreach (var t in traits)
            {
                traitTypes.Add(t.traitType);
                t.OnInitialize(this);
            }
            
            initializedFromAssetTraits  = true;
            
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
    public void InitializeTalentPointsIfFresh(int defaultPoints = 1)
    {
        // only set if they're “fresh” (no prior spends) and points are non-positive
        if ((learnedTalentIds == null || learnedTalentIds.Count == 0) && talentPoints <= 0)
            talentPoints = defaultPoints;
    }

    public void ResetProgression()
    {
        level = 1;
        currentXp = 0;

        ApplyStatsDelta(levelUpStats, -1);
        levelUpStats = new CombatStats();

    }

    public void SetLevelFromReward(int currentLevel)
    {
        ResetProgression();
        currentXp += GetXpRequiredForXLevel(currentLevel);
    }

    public void SetXpFromReward(int totalXp)
    {
        ResetProgression();
        if (totalXp > 0)
            AddXp(totalXp);
    }

    public void AddXp(int amount)
    {
        if (amount <= 0) return;
        if (level >= MaxLevel) return;

        currentXp += amount;

    }

    public bool TryToLevelUp()
    {
        if (level >= MaxLevel) return false; // cannot level past max
       
        int required = GetXpRequiredForNextLevel(level);
        if (currentXp < required)
            return false;

        currentXp -= required;
        level++;

        // apply level-up effects
        ApplyLevelUpEffects();

        ApplyTraitLevelUpEffects();

        return true;
    }

    // Simple, centralized XP curve – tweak numbers as desired.
    public int GetXpRequiredForNextLevel(int fromLevel)
    {
        if (fromLevel >= MaxLevel) return int.MaxValue;
        // Example: linear curve, 20 * current level
        return levelXpRequired[fromLevel + 1];
    }

    public int GetXpRequiredForXLevel(int toLevel, int fromLevel = 1)
    {
        if (fromLevel >= toLevel) return 0;

        int totalXp = 0;
        for (int i = fromLevel; i < toLevel; i++)
            totalXp += GetXpRequiredForNextLevel(i);
        return totalXp;
    }

    private readonly float levelUpBaseMultiplier = .06f;
    private readonly float levelUpLevelMultiplier = 0.02f;
    public void ApplyLevelUpEffects()
    {
        talentPoints += 1; // add 1 talent point per level-up

        // apply level-up stats
        CombatStats delta = new CombatStats
        {
            maxHealth          = Mathf.RoundToInt(baseStats.maxHealth * (levelUpBaseMultiplier + level * levelUpLevelMultiplier)),
            maxSp              = Mathf.RoundToInt(baseStats.maxSp * (levelUpBaseMultiplier + level * levelUpLevelMultiplier)),
            speed              = Mathf.RoundToInt(baseStats.speed * (levelUpBaseMultiplier + level * levelUpLevelMultiplier)),
            physicalAttack     = Mathf.RoundToInt(baseStats.physicalAttack * (levelUpBaseMultiplier + level * levelUpLevelMultiplier)),
            elementalPower     = Mathf.RoundToInt(baseStats.elementalPower * (levelUpBaseMultiplier + level * levelUpLevelMultiplier)),
            defense            = Mathf.RoundToInt(baseStats.defense * (levelUpBaseMultiplier + level * levelUpLevelMultiplier)),
            elementalResistance= Mathf.RoundToInt(baseStats.elementalResistance * (levelUpBaseMultiplier + level * levelUpLevelMultiplier))
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

        health = stats.maxHealth;
        sp = stats.maxSp;
        
    }

    public void ApplyLevelUpEffects(int level)
    {
        talentPoints += 1; // add 1 talent point per level-up

        // apply level-up stats
        CombatStats delta = new CombatStats
        {
            maxHealth          = Mathf.RoundToInt(baseStats.maxHealth * (levelUpBaseMultiplier + level * levelUpLevelMultiplier)),
            maxSp              = Mathf.RoundToInt(baseStats.maxSp * (levelUpBaseMultiplier + level * levelUpLevelMultiplier)),
            speed              = Mathf.RoundToInt(baseStats.speed * (levelUpBaseMultiplier + level * levelUpLevelMultiplier)),
            physicalAttack     = Mathf.RoundToInt(baseStats.physicalAttack * (levelUpBaseMultiplier + level * levelUpLevelMultiplier)),
            elementalPower     = Mathf.RoundToInt(baseStats.elementalPower * (levelUpBaseMultiplier + level * levelUpLevelMultiplier)),
            defense            = Mathf.RoundToInt(baseStats.defense * (levelUpBaseMultiplier + level * levelUpLevelMultiplier)),
            elementalResistance= Mathf.RoundToInt(baseStats.elementalResistance * (levelUpBaseMultiplier + level * levelUpLevelMultiplier))
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

        health = stats.maxHealth;
        sp = stats.maxSp;
        
    }

    public void ApplyLevelUpBonusStats(CombatStats delta)
    {
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

        levelUpStats.bludgeoningAttack    += delta.bludgeoningAttack;
        levelUpStats.slashingAttack       += delta.slashingAttack;
        levelUpStats.piercingAttack       += delta.piercingAttack;
        levelUpStats.bludgeoningDefense   += delta.bludgeoningDefense;
        levelUpStats.slashingDefense      += delta.slashingDefense;
        levelUpStats.piercingDefense      += delta.piercingDefense;
        levelUpStats.fireAttack           += delta.fireAttack;
        levelUpStats.iceAttack            += delta.iceAttack;
        levelUpStats.stormAttack          += delta.stormAttack;
        levelUpStats.acidAttack           += delta.acidAttack;
        levelUpStats.psychicAttack        += delta.psychicAttack;
        levelUpStats.bloodAttack          += delta.bloodAttack;
        levelUpStats.fireDefense          += delta.fireDefense;
        levelUpStats.iceDefense           += delta.iceDefense;
        levelUpStats.stormDefense         += delta.stormDefense;
        levelUpStats.acidDefense          += delta.acidDefense;
        levelUpStats.psychicDefense       += delta.psychicDefense;
        levelUpStats.bloodDefense         += delta.bloodDefense;



        health += delta.maxHealth;
        sp += delta.maxSp;
    }

    private void InitializeSubStatsFromTemplate()
    {
        if (characterAsset == null) return;
        var seed = characterAsset.predeterminedSubStats;
        if(seed == null || seed.Count == 0) return;
        if (seed.Count != 6)
        {
            
            var substats = Enum.GetValues(typeof(CombatSubStat)).Cast<CombatSubStat>().ToList();
            
            while(seed.Count < 6){
                var substatRandom = UnityEngine.Random.Range(0, substats.Count);
                if(seed.Contains(substats[substatRandom])) {
                    substats.RemoveAt(substatRandom);
                    continue;
                }
                seed.Add(substats[substatRandom]);
            }
        }

        // Shuffle copy
        var list = new List<CombatSubStat>(seed);
        ShuffleExtension.Shuffle(list, 20);

        mainSubStats.Clear();
        subSubStats.Clear();

        // First 3 = main, last 3 = sub-sub
        for (int i = 0; i < 6; i++)
        {
            if (i < 3) mainSubStats.Add(list[i]);
            else       subSubStats.Add(list[i]);
        }

        // Main substats +10 each (this is separate from the 100 pool)
        for (int i = 0; i < mainSubStats.Count; i++)
            AddToSubStat(mainSubStats[i], 10);

        // Pool allocation
        int remaining = 100;
        int index = 0;

        // Track only pool points for the 40-cap
        var poolAlloc = new Dictionary<CombatSubStat, int>(6);
        for (int i = 0; i < 6; i++)
            poolAlloc[list[i]] = 0;

        while (remaining > 0)
        {
            var key = list[index];

            // If this stat hit 40 from pool, move on (no point spent)
            if (poolAlloc[key] >= 40)
            {
                index = (index + 1) % 6;
                continue;
            }

            // Allocate 1 point
            AddToSubStat(key, 1);
            poolAlloc[key] += 1;
            remaining -= 1;

            if (remaining <= 0) break;

            // Chance to move: 1 / remaining
            float moveChance = 1f / remaining;
            if (UnityEngine.Random.value < moveChance)
                index = (index + 1) % 6;
        }
    }

    private void AddToSubStat(CombatSubStat subStat, int delta)
    {
        switch (subStat)
        {
            case CombatSubStat.BludgeoningAttack:  stats.bludgeoningAttack += delta; break;
            case CombatSubStat.SlashingAttack:     stats.slashingAttack += delta; break;
            case CombatSubStat.PiercingAttack:     stats.piercingAttack += delta; break;

            case CombatSubStat.BludgeoningDefense: stats.bludgeoningDefense += delta; break;
            case CombatSubStat.SlashingDefense:    stats.slashingDefense += delta; break;
            case CombatSubStat.PiercingDefense:    stats.piercingDefense += delta; break;

            case CombatSubStat.FireAttack:         stats.fireAttack += delta; break;
            case CombatSubStat.IceAttack:          stats.iceAttack += delta; break;
            case CombatSubStat.StormAttack:        stats.stormAttack += delta; break;
            case CombatSubStat.AcidAttack:         stats.acidAttack += delta; break;
            case CombatSubStat.PsychicAttack:      stats.psychicAttack += delta; break;
            case CombatSubStat.BloodAttack:        stats.bloodAttack += delta; break;

            case CombatSubStat.FireDefense:        stats.fireDefense += delta; break;
            case CombatSubStat.IceDefense:         stats.iceDefense += delta; break;
            case CombatSubStat.StormDefense:       stats.stormDefense += delta; break;
            case CombatSubStat.AcidDefense:        stats.acidDefense += delta; break;
            case CombatSubStat.PsychicDefense:     stats.psychicDefense += delta; break;
            case CombatSubStat.BloodDefense:       stats.bloodDefense += delta; break;
        }
    }

    private Hashtable statLevelUpMultipliers = new Hashtable();

    private void SetupLevelMultiplierHashtable()
    {
        statLevelUpMultipliers.Add(CombatMainStat.MaxHealth,            10f); //Max Health
        statLevelUpMultipliers.Add(CombatMainStat.MaxSp,                5f); //Max SP
        statLevelUpMultipliers.Add(CombatMainStat.PhysicalAttack,       1f); //Physical Attack
        statLevelUpMultipliers.Add(CombatMainStat.ElementalPower,       1f); //Elemental Power
        statLevelUpMultipliers.Add(CombatMainStat.Defense,              1f); //Defense
        statLevelUpMultipliers.Add(CombatMainStat.ElementalResistance,  1f); //Elemental Resistance
        statLevelUpMultipliers.Add(CombatMainStat.Speed,                1f);  //Speed
        statLevelUpMultipliers.Add(CombatMainStat.CritChance,           .1f);  //Crit Chance
        statLevelUpMultipliers.Add(CombatMainStat.CritDamage,           .2f);  //Crit Damage

        statLevelUpMultipliers.Add(CombatSubStat.BludgeoningAttack,     2f); //Bludgeoning Attack
        statLevelUpMultipliers.Add(CombatSubStat.SlashingAttack,        2f); //Slashing Attack
        statLevelUpMultipliers.Add(CombatSubStat.PiercingAttack,        2f); //Piercing Attack

        statLevelUpMultipliers.Add(CombatSubStat.BludgeoningDefense,    2f); //Bludgeoning Defense
        statLevelUpMultipliers.Add(CombatSubStat.SlashingDefense,       2f); //Slashing Defense
        statLevelUpMultipliers.Add(CombatSubStat.PiercingDefense,       2f); //Piercing Defense

        statLevelUpMultipliers.Add(CombatSubStat.FireAttack,            2f); //Fire Attack
        statLevelUpMultipliers.Add(CombatSubStat.IceAttack,             2f); //Ice Attack
        statLevelUpMultipliers.Add(CombatSubStat.StormAttack,           2f); //Storm Attack
        statLevelUpMultipliers.Add(CombatSubStat.AcidAttack,            2f); //Acid Attack
        statLevelUpMultipliers.Add(CombatSubStat.PsychicAttack,         2f); //Psychic Attack
        statLevelUpMultipliers.Add(CombatSubStat.BloodAttack,           2f); //Blood Attack

        statLevelUpMultipliers.Add(CombatSubStat.FireDefense,           2f); //Fire Defense
        statLevelUpMultipliers.Add(CombatSubStat.IceDefense,            2f); //Ice Defense
        statLevelUpMultipliers.Add(CombatSubStat.StormDefense,          2f); //Storm Defense
        statLevelUpMultipliers.Add(CombatSubStat.AcidDefense,           2f); //Acid Defense
        statLevelUpMultipliers.Add(CombatSubStat.PsychicDefense,        2f); //Psychic Defense
        statLevelUpMultipliers.Add(CombatSubStat.BloodDefense,          2f); //Blood Defense
    }

    public void AddMainStatToCombatStats(CombatMainStat stat, ref CombatStats statObject, int value)
    {
        switch (stat)
        {
            case CombatMainStat.MaxHealth:            statObject.maxHealth += value; break;
            case CombatMainStat.MaxSp:                statObject.maxSp += value; break;
            case CombatMainStat.PhysicalAttack:       statObject.physicalAttack += value; break;
            case CombatMainStat.ElementalPower:       statObject.elementalPower += value; break;
            case CombatMainStat.Defense:              statObject.defense += value; break;
            case CombatMainStat.ElementalResistance:  statObject.elementalResistance += value; break;
            case CombatMainStat.Speed:                statObject.speed += value; break;
            case CombatMainStat.CritChance:           statObject.critChance += value; break;
            case CombatMainStat.CritDamage:           statObject.critDamage += value; break;
        }
    }

    public void AddSubStatToCombatStats(CombatSubStat stat, ref CombatStats statObject, int value)
    {
        switch (stat)
        {
            case CombatSubStat.BludgeoningAttack:  statObject.bludgeoningAttack += value; break;
            case CombatSubStat.SlashingAttack:     statObject.slashingAttack += value; break;
            case CombatSubStat.PiercingAttack:     statObject.piercingAttack += value; break;

            case CombatSubStat.BludgeoningDefense: statObject.bludgeoningDefense += value; break;
            case CombatSubStat.SlashingDefense:    statObject.slashingDefense += value; break;
            case CombatSubStat.PiercingDefense:    statObject.piercingDefense += value; break;

            case CombatSubStat.FireAttack:         statObject.fireAttack += value; break;
            case CombatSubStat.IceAttack:          statObject.iceAttack += value; break;
            case CombatSubStat.StormAttack:        statObject.stormAttack += value; break;
            case CombatSubStat.AcidAttack:         statObject.acidAttack += value; break;
            case CombatSubStat.PsychicAttack:      statObject.psychicAttack += value; break;
            case CombatSubStat.BloodAttack:        statObject.bloodAttack += value; break;

            case CombatSubStat.FireDefense:        statObject.fireDefense += value; break;
            case CombatSubStat.IceDefense:         statObject.iceDefense += value; break;
            case CombatSubStat.StormDefense:       statObject.stormDefense += value; break;
            case CombatSubStat.AcidDefense:        statObject.acidDefense += value; break;
            case CombatSubStat.PsychicDefense:     statObject.psychicDefense += value; break;
            case CombatSubStat.BloodDefense:       statObject.bloodDefense += value; break;
        }
    }

    public List<CombatStats> GetLevelUpBonusStats(int number = 3)
    {
        if(statLevelUpMultipliers == null || statLevelUpMultipliers.Count == 0)
        {
            SetupLevelMultiplierHashtable();
        }
        
        List<CombatMainStat> mainStatGroupA = new List<CombatMainStat>{
            CombatMainStat.PhysicalAttack,
            CombatMainStat.ElementalPower,
            CombatMainStat.MaxHealth
        };
        
        List<CombatMainStat> mainStatGroupB = new List<CombatMainStat>
        {
            CombatMainStat.Defense,
            CombatMainStat.ElementalResistance,
            CombatMainStat.MaxSp,
            CombatMainStat.Speed  
        };

        ShuffleExtension.Shuffle(mainStatGroupA, mainStatGroupA.Count*2);
        ShuffleExtension.Shuffle(mainStatGroupB, mainStatGroupB.Count*2);
        ShuffleExtension.Shuffle(mainSubStats, mainSubStats.Count*2);
        ShuffleExtension.Shuffle(subSubStats, subSubStats.Count*2);

        List<CombatStats> bonusStats = new List<CombatStats>();

        for(int i = 0; i < number; i++)
        {
            
            List<object> keys = new List<object>
            {
                mainStatGroupA[i % mainStatGroupA.Count],
                mainStatGroupB[i % mainStatGroupB.Count],
                mainSubStats.Count > 0 ? mainSubStats[i % mainSubStats.Count] : CombatSubStat.PiercingAttack,
                subSubStats.Count > 0 ? subSubStats[i % subSubStats.Count] : CombatSubStat.PiercingDefense
            };

            List<int> keyRemovealIndices = new List<int>();
            foreach(var key in keys)
                if(!(key is CombatMainStat || key is CombatSubStat)) keyRemovealIndices.Add(keys.IndexOf(key));
            foreach(var index in keyRemovealIndices)
                keys.RemoveAt(index);
            
            int[] splitValues = GeneralUtility.splitInt(bonusStatsValue, keys.Count, bonusStatsMin, bonusStatsMax);
            
            CombatStats bonusStatSet = new CombatStats();
            for(int j = 0; j < keys.Count; j++)
            {
                if(statLevelUpMultipliers[keys[j]] == null) continue;
                int multiplier = Mathf.RoundToInt((float)statLevelUpMultipliers[keys[j]]);
                Debug.Log($"Adding {splitValues[j]} * {multiplier} to {keys[j]}");

                if (keys[j] is CombatMainStat stat)
                {
                    AddMainStatToCombatStats(stat, ref bonusStatSet, splitValues[j] * multiplier);
                }
                else if (keys[j] is CombatSubStat stat1)
                {
                    AddSubStatToCombatStats(stat1, ref bonusStatSet, splitValues[j] * multiplier);
                }
            }
            bonusStats.Add(bonusStatSet);
        }
        
        return bonusStats;
    }
    
    public void InitializeTalents()
    {
        if (talentTreePrefab == null) return;

        // Automatically learn the first talent(s) in the tree that cost 0 points, if any
        foreach (var t in talentTreePrefab.nodes)
        {
            if (t.talent.cost == 0)
            {
                LearnTalent(t.talent);
                break;
            }
        }

        
    }

    public void ApplyTraitLevelUpEffects()
    {
        if (traits == null || traits.Count == 0) return;

        foreach (var trait in traits)
        {
            trait.OnLevelUp(this);
        }
    }

    public void ApplyMapItemUseTraitEffects(ItemDefinition item)
    {
        if (item == null) return;
        if (traits == null || traits.Count == 0) return;

        foreach (var trait in traits)
        {
            trait.OnMapItemUsed(this, item);
        }
    }
}
