using UnityEngine;
using System.Collections.Generic;

public class BattleCharacter : MonoBehaviour
{
    [Min(1)]
    public int slotSize = 1;   // how many slots this character “occupies”

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


    [Header("Stats")]
    [SerializeField] private int speed = 10;
    [SerializeField] private int physicalAttack = 100;
    [SerializeField] private int elementalPower = 100;
    [SerializeField] private int defense = 0;
    [SerializeField] private int elementalResistance = 0;
    [SerializeField] private int critChance = 5;     // percent
    [SerializeField] private int critDamage = 150;   // percent multiplier
    

    public int Speed                => speed;
    public int PhysicalAttack       => physicalAttack;
    public int ElementalPower       => elementalPower;
    public int Defense              => defense;
    public int ElementalResistance  => elementalResistance;
    public int CritChance           => critChance;
    public int CritDamage           => critDamage;


    [Header("Physical Sub-Attack")]
    [SerializeField] private int bludgeoningAttack;
    [SerializeField] private int slashingAttack;
    [SerializeField] private int piercingAttack;

    [Header("Physical Sub-Defense")]
    [SerializeField] private int bludgeoningDefense;
    [SerializeField] private int slashingDefense;
    [SerializeField] private int piercingDefense;

    [Header("Elemental Sub-Attack")]
    [SerializeField] private int fireAttack;
    [SerializeField] private int iceAttack;
    [SerializeField] private int stormAttack;
    [SerializeField] private int acidAttack;
    [SerializeField] private int psychicAttack;
    [SerializeField] private int bloodAttack;

    [Header("Elemental Sub-Defense")]
    [SerializeField] private int fireDefense;
    [SerializeField] private int iceDefense;
    [SerializeField] private int stormDefense;
    [SerializeField] private int acidDefense;
    [SerializeField] private int psychicDefense;
    [SerializeField] private int bloodDefense;

    // Optional read-only accessors if you want:
    public int BludgeoningAttack => bludgeoningAttack;
    public int SlashingAttack    => slashingAttack;
    public int PiercingAttack    => piercingAttack;
    
    public int BludgeoningDefense => bludgeoningDefense;
    public int SlashingDefense    => slashingDefense;
    public int PiercingDefense    => piercingDefense;

    public int FireAttack        => fireAttack;
    public int IceAttack         => iceAttack;
    public int StormAttack       => stormAttack;
    public int AcidAttack        => acidAttack;
    public int PsychicAttack     => psychicAttack;
    public int BloodAttack       => bloodAttack;

    public int FireDefense        => fireDefense;
    public int IceDefense         => iceDefense;
    public int StormDefense       => stormDefense;
    public int AcidDefense        => acidDefense;
    public int PsychicDefense     => psychicDefense;
    public int BloodDefense       => bloodDefense;

    public MapPartyMemberDefinition sourceDefinition;  // null for enemies created from MapEnemyDefinition

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

        skill.Execute(this, target);
    }

    public void ClearSkills() => skills.Clear();
    public void AddSkill(Skill s)
    {
        if (s != null) skills.Add(s);
    }

    public void SetStats(
        int currentHp,
        int spd,
        int physAtk,
        int elemPower,
        int def,
        int elemRes,
        int critChance,
        int critDamage)
    {
        currentHealth       = Mathf.Clamp(currentHp, 0, maxHealth);
        speed               = Mathf.Max(1, spd);
        physicalAttack      = Mathf.Max(0, physAtk);
        elementalPower      = Mathf.Max(0, elemPower);
        defense             = Mathf.Max(0, def);
        elementalResistance = Mathf.Max(0, elemRes);
        this.critChance     = Mathf.Max(0, critChance);
        this.critDamage     = Mathf.Max(0, critDamage);
    }
    public void ApplyStats(CombatStats stats, int currentHp)
    {
        SetMaxHealth(stats.maxHealth, fillToMax: false);

        currentHealth       = Mathf.Clamp(currentHp, 0, maxHealth);
        speed               = Mathf.Max(1, stats.speed);
        physicalAttack      = Mathf.Max(0, stats.physicalAttack);
        elementalPower      = Mathf.Max(0, stats.elementalPower);
        defense             = Mathf.Max(0, stats.defense);
        elementalResistance = Mathf.Max(0, stats.elementalResistance);
        critChance          = Mathf.Max(0, stats.critChance);
        critDamage          = Mathf.Max(0, stats.critDamage);
        bludgeoningAttack   = Mathf.Max(0, stats.bludgeoningAttack);
        slashingAttack      = Mathf.Max(0, stats.slashingAttack);
        piercingAttack      = Mathf.Max(0, stats.piercingAttack);
        bludgeoningDefense  = Mathf.Max(0, stats.bludgeoningDefense);
        slashingDefense     = Mathf.Max(0, stats.slashingDefense);
        piercingDefense     = Mathf.Max(0, stats.piercingDefense);
        fireAttack          = Mathf.Max(0, stats.fireAttack);
        iceAttack           = Mathf.Max(0, stats.iceAttack);
        stormAttack         = Mathf.Max(0, stats.stormAttack);
        acidAttack          = Mathf.Max(0, stats.acidAttack);
        psychicAttack       = Mathf.Max(0, stats.psychicAttack);
        bloodAttack         = Mathf.Max(0, stats.bloodAttack);
        fireDefense         = Mathf.Max(0, stats.fireDefense);
        iceDefense          = Mathf.Max(0, stats.iceDefense);
        stormDefense        = Mathf.Max(0, stats.stormDefense);
        acidDefense         = Mathf.Max(0, stats.acidDefense);
        psychicDefense      = Mathf.Max(0, stats.psychicDefense);
        bloodDefense        = Mathf.Max(0, stats.bloodDefense);
    }
    public void setName(string newName)
    {
        this.name = newName;
    }
    public int GetSubAttack(DamageSubType subType)
    {
        switch (subType)
        {
            case DamageSubType.Bludgeoning: return bludgeoningAttack;
            case DamageSubType.Slashing:    return slashingAttack;
            case DamageSubType.Piercing:    return piercingAttack;

            case DamageSubType.Fire:        return fireAttack;
            case DamageSubType.Ice:         return iceAttack;
            case DamageSubType.Storm:       return stormAttack;
            case DamageSubType.Acid:        return acidAttack;
            case DamageSubType.Psychic:     return psychicAttack;
            case DamageSubType.Blood:       return bloodAttack;

            default: return 0;
        }
    }

    public int GetSubDefense(DamageSubType subType)
    {
        switch (subType)
        {
            case DamageSubType.Bludgeoning: return bludgeoningDefense;
            case DamageSubType.Slashing:    return slashingDefense;
            case DamageSubType.Piercing:    return piercingDefense;

            case DamageSubType.Fire:        return fireDefense;
            case DamageSubType.Ice:         return iceDefense;
            case DamageSubType.Storm:       return stormDefense;
            case DamageSubType.Acid:        return acidDefense;
            case DamageSubType.Psychic:     return psychicDefense;
            case DamageSubType.Blood:       return bloodDefense;

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


}
