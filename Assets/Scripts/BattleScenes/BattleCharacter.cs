using UnityEngine;
using System.Collections.Generic;
using System;
using System.Xml.XPath;

public class BattleCharacter : MonoBehaviour
{
    [Min(1)]
    public int slotSize = 1;   // how many slots this character “occupies”


    //NOTE: Check later to see if these need to be serialized
    [Header("Health")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;

    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;
    public bool IsDead => currentHealth <= 0;

    [Header("SP")]
    [SerializeField] private int maxSp = 10;
    [SerializeField] private int currentSp;

    public int MaxSp => maxSp;
    public int CurrentSp => currentSp;
    

    public List<PassivesDefinition> passives
    = new List<PassivesDefinition>();


    [NonSerialized] public CombatStats baseStats = new CombatStats();

    [NonSerialized] public CombatStats bonusStats = new CombatStats();

    public MapPartyMemberDefinition sourceDefinition;  // null for enemies created from MapEnemyDefinition

    public List<TraitDefinition> Traits { get; } = new List<TraitDefinition>();
    public List<CharacterTrait> traitTypes = new List<CharacterTrait>();

    void Awake()
    {
        
        // Initialize current health at full
        currentHealth = Mathf.Max(1, maxHealth);

        // Initialize SP at full by default
        currentSp = Mathf.Max(0, maxSp);
    }

    public int TakeDamage(int amount)
    {
        if (amount <= 0 || IsDead) return 0;

        int oldHealth = currentHealth;
        currentHealth = Mathf.Max(0, currentHealth - amount);
        int dealt = oldHealth - currentHealth;

        if (IsDead)
        {
            Debug.Log($"{name} died.");

            if (BattleTurnManager.Instance != null)
                BattleTurnManager.Instance.HandleCharacterDeath(this);
        }
        else
        {
            Debug.Log($"{name} took {dealt} damage. HP: {currentHealth}/{maxHealth}");
        }

        return dealt;
    }


    [Header("AI / Threat")]
    [SerializeField] private int threat;
    public int Threat => threat;

    public void AddThreat(int amount)
    {
        if (amount <= 0) return;
        threat += amount;
    }

    // Optional, for later if you want to reset completely
    public void ResetThreat()
    {
        threat = 0;
    }

    public void Heal(int amount)
    {
        if (amount <= 0 || IsDead) return;

        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        Debug.Log($"{name} healed {amount}. HP: {currentHealth}/{maxHealth}");
    }

    public void SetMaxHealth(int newMax, bool fillToMax = true)
    {
        maxHealth = Mathf.Max(1, newMax);
        if (fillToMax || currentHealth > maxHealth)
            currentHealth = maxHealth;
    }

    [Header("Skills")]
    [SerializeField] private List<Skill> skills = new List<Skill>();
    public IReadOnlyList<Skill> Skills => skills;

    public void UseSkill(int index, BattleCharacter target)
    {
        if (index < 0 || index >= skills.Count)
        {
            Debug.LogWarning($"{name} tried to use skill at index {index}, but it is out of range.");
            return;
        }

        Skill skill = skills[index];
        if (skill == null)
        {
            Debug.LogWarning($"{name} has a null skill at index {index}.");
            return;
        }

        int cost = skill.spCost; // new field on Skill
        if (!TrySpendSp(cost))
        {
            Debug.Log($"{name} does not have enough SP ({currentSp}/{cost}) to use {skill.skillName}.");
            return;
        }

        // Trait tests
        if(!HasEnoughAmmoFor(skill)) return;

        skill.Execute(this, target);
    }

    public void ClearPassives() => passives.Clear();
    public void AddPassive(PassivesDefinition p)
    {
        if (p != null) {
            passives.Add(p);
            p.OnCreated(this);
        }
    }
    public void RemovePassive(PassivesDefinition p)
    {
        if (p != null)
        {
            p.OnDestroyed(this);
            passives.Remove(p);
        } 
    }

    public void ClearSkills() => skills.Clear();
    public void AddSkill(Skill s)
    {
        if (s != null) skills.Add(s);
    }

    public void ClearTraits() { 
        Traits.Clear(); 
        traitTypes.Clear();    
    }

    
    public void ApplyStats(CombatStats stats, int currentHp)
    {
        baseStats = stats;
        SetMaxHealth(stats.maxHealth, fillToMax: false);

        currentHealth       = Mathf.Clamp(currentHp, 0, maxHealth);
        bonusStats = new CombatStats();
    }

    public CombatStats GetEffectiveStats()
    {
        bonusStats = new CombatStats();
        foreach (var p in passives)
        {
            if (p == null) continue;
            p.getStatBoosts(this);
        }
        CombatStats result = new CombatStats();
        result.maxHealth      = baseStats.maxHealth      + bonusStats.maxHealth;
        result.maxSp          = baseStats.maxSp          + bonusStats.maxSp;
        result.physicalAttack = baseStats.physicalAttack + bonusStats.physicalAttack;
        result.elementalPower = baseStats.elementalPower + bonusStats.elementalPower;
        result.defense        = baseStats.defense        + bonusStats.defense;
        result.elementalResistance = baseStats.elementalResistance + bonusStats.elementalResistance;
        result.speed          = baseStats.speed          + bonusStats.speed;
        result.critChance     = baseStats.critChance     + bonusStats.critChance;
        result.critDamage = baseStats.critDamage + bonusStats.critDamage;

        result.piercingAttack = baseStats.piercingAttack + bonusStats.piercingAttack;
        result.bludgeoningAttack = baseStats.bludgeoningAttack + bonusStats.bludgeoningAttack;
        result.slashingAttack = baseStats.slashingAttack + bonusStats.slashingAttack;

        result.fireAttack = baseStats.fireAttack + bonusStats.fireAttack;
        result.iceAttack  = baseStats.iceAttack  + bonusStats.iceAttack;
        result.stormAttack  = baseStats.stormAttack  + bonusStats.stormAttack;
        result.acidAttack   = baseStats.acidAttack   + bonusStats.acidAttack;
        result.psychicAttack = baseStats.psychicAttack + bonusStats.psychicAttack;
        result.bloodAttack    = baseStats.bloodAttack    + bonusStats.bloodAttack;

        result.piercingDefense = baseStats.piercingDefense + bonusStats.piercingDefense;
        result.bludgeoningDefense = baseStats.bludgeoningDefense + bonusStats.bludgeoningDefense;
        result.slashingDefense  = baseStats.slashingDefense  + bonusStats.slashingDefense;

        result.fireDefense  = baseStats.fireDefense  + bonusStats.fireDefense;
        result.iceDefense   = baseStats.iceDefense   + bonusStats.iceDefense;
        result.stormDefense = baseStats.stormDefense + bonusStats.stormDefense;
        result.acidDefense    = baseStats.acidDefense    + bonusStats.acidDefense;
        result.psychicDefense = baseStats.psychicDefense + bonusStats.psychicDefense;
        result.bloodDefense     = baseStats.bloodDefense     + bonusStats.bloodDefense;

        return result;
    }

    public void setName(string newName)
    {
        this.name = newName;
    }

    public int getSpeed()
    {
        return baseStats.speed + bonusStats.speed;
    }

    public int GetSubAttack(DamageSubType subType)
    {
        switch (subType)
        {
            case DamageSubType.Bludgeoning: return baseStats.bludgeoningAttack + bonusStats.bludgeoningAttack;
            case DamageSubType.Slashing:    return baseStats.slashingAttack + bonusStats.slashingAttack;
            case DamageSubType.Piercing:    return baseStats.piercingAttack + bonusStats.piercingAttack;

            case DamageSubType.Fire:        return baseStats.fireAttack + bonusStats.fireAttack;
            case DamageSubType.Ice:         return baseStats.iceAttack + bonusStats.iceAttack;
            case DamageSubType.Storm:       return baseStats.stormAttack + bonusStats.stormAttack;
            case DamageSubType.Acid:        return baseStats.acidAttack + bonusStats.acidAttack;
            case DamageSubType.Psychic:     return baseStats.psychicAttack + bonusStats.psychicAttack;
            case DamageSubType.Blood:       return baseStats.bloodAttack + bonusStats.bloodAttack;

            default: return 0;
        }
    }

    public int GetSubDefense(DamageSubType subType)
    {
        switch (subType)
        {
            case DamageSubType.Bludgeoning: return baseStats.bludgeoningDefense + bonusStats.bludgeoningDefense;
            case DamageSubType.Slashing:    return baseStats.slashingDefense + bonusStats.slashingDefense;
            case DamageSubType.Piercing:    return baseStats.piercingDefense + bonusStats.piercingDefense;

            case DamageSubType.Fire:        return baseStats.fireDefense + bonusStats.fireDefense;
            case DamageSubType.Ice:         return baseStats.iceDefense + bonusStats.iceDefense;
            case DamageSubType.Storm:       return baseStats.stormDefense + bonusStats.stormDefense;
            case DamageSubType.Acid:        return baseStats.acidDefense + bonusStats.acidDefense;
            case DamageSubType.Psychic:     return baseStats.psychicDefense + bonusStats.psychicDefense;
            case DamageSubType.Blood:       return baseStats.bloodDefense + bonusStats.bloodDefense;

            default: return 0;
        }
    }

    public void SetMaxSp(int newMax, bool fillToMax = true)
    {
        maxSp = Mathf.Max(0, newMax);
        if (fillToMax || currentSp > maxSp)
            currentSp = maxSp;
    }

    public void SetSp(int value)
    {
        currentSp = Mathf.Clamp(value, 0, maxSp);
    }

    public bool TrySpendSp(int amount)
    {
        Debug.Log($"{name} attempting to spend {amount} SP. Current SP: {currentSp}/{maxSp}");
        if (amount <= 0) return true;
        Debug.Log($"{name} has enough SP to spend.");
        if (currentSp < amount) return false;

        currentSp -= amount;
        Debug.Log($"{name} spent {amount} SP. SP: {currentSp}/{maxSp}");
        return true;
    }

    public void RecoverSp(int amount)
    {
        currentSp = Mathf.Min(maxSp, currentSp + amount);
        Debug.Log($"{name} recovered {amount} SP. SP: {currentSp}/{maxSp}");
    }
    
    //TRAITS
    
    // Called by skills to run trait hooks
    public int ApplyTraitDamageModifiers(Skill skill, BattleCharacter target, int baseDamage)
    {
        int damage = baseDamage;
        if (Traits != null)
        {
            for (int i = 0; i < Traits.Count; i++)
            {
                var t = Traits[i];
                if (t == null) continue;
                t.OnModifySkillDamage(this, skill, target, ref damage);
            }
        }
        return damage;
    }

    
    // Marksman ammo
    public int ConstantMaxAmmo { get; set; }   // “design” max used for % cost
    public int MaxAmmo         { get; set; }
    public int CurrentAmmo     { get; private set; }

    public void SetAmmo(int max, int current, int constantMax)
    {
        ConstantMaxAmmo = Mathf.Max(0, constantMax);
        MaxAmmo         = Mathf.Max(0, max);
        CurrentAmmo     = Mathf.Clamp(current, 0, MaxAmmo);
    }

    public void SpendAmmo(int amount)
    {
        if (amount <= 0) return;
        CurrentAmmo = Mathf.Max(0, CurrentAmmo - amount);
    }

    public void AddAmmo(int amount)
    {
        if (amount <= 0) return;
        CurrentAmmo = Mathf.Min(MaxAmmo, CurrentAmmo + amount);
    }
    
    public bool HasEnoughAmmoFor(Skill skill)
    {
        if (skill == null) return true;

        if (traitTypes.Contains(CharacterTrait.Marksman) == false)
            return true;

        // If the skill has no ammo requirement, it's always usable.
        if (skill.ammoCost <= 0f)
            return true;

        if (ConstantMaxAmmo <= 0)
            return false;

        // Required ammo = ceil(percent * constantMaxAmmo)
        float percent = Mathf.Clamp01(skill.ammoCost);
        int needed = Mathf.CeilToInt(percent * ConstantMaxAmmo);

        return CurrentAmmo >= needed;
    }

}
