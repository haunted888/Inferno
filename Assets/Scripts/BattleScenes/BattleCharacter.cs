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

    [NonSerialized] public PassiveMutationUtility.PassiveMutationContext passiveMutationContext;

    public List<QueuedAction> currentActionOrder;
    

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

        float hpCost = skill.hpCost; // new field on Skill
        if (!TrySpendHp(hpCost))
        {
            Debug.Log($"{name} does not have enough HP ({currentHealth}/{cost}) to use {skill.skillName}.");
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
            p.GetStatBoosts(this);
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

    public Dictionary<DamageSubType, int> GetSubAttackStats()
    {
        bonusStats = new CombatStats();
        foreach (var p in passives)
        {
            if (p == null) continue;
            p.GetStatBoosts(this);
        }
        Dictionary<DamageSubType, int> subAttackStats = new Dictionary<DamageSubType, int>
        {
            { DamageSubType.Bludgeoning, baseStats.bludgeoningAttack + bonusStats.bludgeoningAttack + baseStats.physicalAttack + bonusStats.physicalAttack },
            { DamageSubType.Slashing, baseStats.slashingAttack + bonusStats.slashingAttack + baseStats.physicalAttack + bonusStats.physicalAttack },
            { DamageSubType.Piercing, baseStats.piercingAttack + bonusStats.piercingAttack + baseStats.physicalAttack + bonusStats.physicalAttack },
            { DamageSubType.Fire, baseStats.fireAttack + bonusStats.fireAttack + baseStats.elementalPower + bonusStats.elementalPower },
            { DamageSubType.Ice, baseStats.iceAttack + bonusStats.iceAttack + baseStats.elementalPower + bonusStats.elementalPower },
            { DamageSubType.Storm, baseStats.stormAttack + bonusStats.stormAttack + baseStats.elementalPower + bonusStats.elementalPower },
            { DamageSubType.Acid, baseStats.acidAttack + bonusStats.acidAttack + baseStats.elementalPower + bonusStats.elementalPower },
            { DamageSubType.Psychic, baseStats.psychicAttack + bonusStats.psychicAttack + baseStats.elementalPower + bonusStats.elementalPower },
            { DamageSubType.Blood, baseStats.bloodAttack + bonusStats.bloodAttack + baseStats.elementalPower + bonusStats.elementalPower }
        };
        return subAttackStats;
    }

    public Dictionary<DamageSubType, int> GetSubDefenseStats()
    {
        bonusStats = new CombatStats();
        foreach (var p in passives)
        {
            if (p == null) continue;
            p.GetStatBoosts(this);
        }
        Dictionary<DamageSubType, int> subDefenseStats = new Dictionary<DamageSubType, int>
        {
            { DamageSubType.Bludgeoning, baseStats.bludgeoningDefense + bonusStats.bludgeoningDefense + baseStats.defense + bonusStats.defense },
            { DamageSubType.Slashing, baseStats.slashingDefense + bonusStats.slashingDefense + baseStats.defense + bonusStats.defense },
            { DamageSubType.Piercing, baseStats.piercingDefense + bonusStats.piercingDefense + baseStats.defense + bonusStats.defense },
            { DamageSubType.Fire, baseStats.fireDefense + bonusStats.fireDefense + baseStats.elementalResistance + bonusStats.elementalResistance },
            { DamageSubType.Ice, baseStats.iceDefense + bonusStats.iceDefense + baseStats.elementalResistance + bonusStats.elementalResistance },
            { DamageSubType.Storm, baseStats.stormDefense + bonusStats.stormDefense + baseStats.elementalResistance + bonusStats.elementalResistance },
            { DamageSubType.Acid, baseStats.acidDefense + bonusStats.acidDefense + baseStats.elementalResistance + bonusStats.elementalResistance },
            { DamageSubType.Psychic, baseStats.psychicDefense + bonusStats.psychicDefense + baseStats.elementalResistance + bonusStats.elementalResistance },
            { DamageSubType.Blood, baseStats.bloodDefense + bonusStats.bloodDefense + baseStats.elementalResistance + bonusStats.elementalResistance }
        };
        return subDefenseStats;
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

    public bool TrySpendHp(float amount)
    {
        int hpLost = Mathf.CeilToInt(amount * maxHealth);
        Debug.Log($"{name} attempting to spend {hpLost} HP. Current HP: {currentHealth}/{maxHealth}");
        if (amount <= 0) return true;
        Debug.Log($"{name} has enough HP to spend.");
        if (currentHealth <= hpLost) return false;

        currentHealth -= hpLost;
        Debug.Log($"{name} spent {hpLost} HP. HP: {currentHealth}/{maxHealth}");
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


    // Fighting side info
    private IReadOnlyList<BattleCharacter> previewAllies;
    private IReadOnlyList<BattleCharacter> previewEnemies;

    public void SetPreviewTeams(
        IReadOnlyList<BattleCharacter> allies,
        IReadOnlyList<BattleCharacter> enemies)
    {
        previewAllies = allies;
        previewEnemies = enemies;
    }

    public IEnumerable<BattleCharacter> GetAllies()
    {
        if (BattleTurnManager.Instance != null)
            return BattleTurnManager.Instance.GetAlliesOf(this);

        return previewAllies ?? Array.Empty<BattleCharacter>();
    }

    public IEnumerable<BattleCharacter> GetEnemies()
    {
        if (BattleTurnManager.Instance != null)
            return BattleTurnManager.Instance.GetEnemiesOf(this);

        return previewEnemies ?? Array.Empty<BattleCharacter>();
    }

    public void QueuePassiveToAdd(PassivesDefinition passive)
    {
        if (passive == null) return;

        if (passiveMutationContext != null)
        {
            passiveMutationContext.passivesToAdd.Add(passive);
        }
        else
        {
            AddPassive(passive);
        }
    }

    public void QueuePassiveToRemove(PassivesDefinition passive)
    {
        if (passive == null) return;

        if (passiveMutationContext != null)
        {
            passiveMutationContext.passivesToRemove.Add(passive);
        }
        else
        {
            RemovePassive(passive);
        }
    }

    public IReadOnlyList<QueuedAction> GetCurrentActionOrder()
    {
        return currentActionOrder;
    }

    // Direct damage modifiers
    private float incomingDamageMultiplier = 1f;
    private float incomingDamageMultiplierMax = float.PositiveInfinity;

    private float incomingDamageMultiplierMin = 0f;

    public void AddIncomingDamageMultiplier(float percentBonus)
    {
        incomingDamageMultiplier += percentBonus;
    }

    public int ApplyIncomingDamageModifiers(int damage)
    {
        if (incomingDamageMultiplierMin > incomingDamageMultiplierMax)
            incomingDamageMultiplierMin = incomingDamageMultiplierMax;
        if (incomingDamageMultiplier > incomingDamageMultiplierMax)
            incomingDamageMultiplier = incomingDamageMultiplierMax;
        if (incomingDamageMultiplier < incomingDamageMultiplierMin)
            incomingDamageMultiplier = incomingDamageMultiplierMin;
        return Mathf.Max(0, Mathf.RoundToInt(damage * incomingDamageMultiplier));
    }

    public void SetIncomingDamageMultiplierLimits(float min = 0f, float max = float.PositiveInfinity)
    {
        if(incomingDamageMultiplierMin < min)
            incomingDamageMultiplierMin = min;
        if(incomingDamageMultiplierMax > max)
            incomingDamageMultiplierMax = max;
    }

    private float outgoingDamageMultiplier = 1f;

    private float outgoingDamageMultiplierMax = float.PositiveInfinity;

    private float outgoingDamageMultiplierMin = 0f;

    public void AddOutgoingDamageMultiplier(float percentBonus)
    {
        outgoingDamageMultiplier += percentBonus;
    }

    public int ApplyOutgoingDamageModifiers(int damage)
    {
        if (outgoingDamageMultiplierMin > outgoingDamageMultiplierMax)
            outgoingDamageMultiplierMin = outgoingDamageMultiplierMax;
        if (outgoingDamageMultiplier > outgoingDamageMultiplierMax)
            outgoingDamageMultiplier = outgoingDamageMultiplierMax;
        if (outgoingDamageMultiplier < outgoingDamageMultiplierMin)
            outgoingDamageMultiplier = outgoingDamageMultiplierMin;
        return Mathf.Max(0, Mathf.RoundToInt(damage * outgoingDamageMultiplier));
    }

    public void SetOutgoingDamageMultiplierLimits(float min = 0f, float max = float.PositiveInfinity)
    {
        if(outgoingDamageMultiplierMin < min)
            outgoingDamageMultiplierMin = min;
        if(outgoingDamageMultiplierMax > max)
            outgoingDamageMultiplierMax = max;
    }

    public void ClearIncomingDamageModifiers()
    {
        incomingDamageMultiplier = 1f;
        incomingDamageMultiplierMin = 0f;
        incomingDamageMultiplierMax = float.PositiveInfinity;

    }
    
    public void ClearOutgoingDamageModifiers()
    {
        outgoingDamageMultiplier = 1f;
        outgoingDamageMultiplierMin = 0f;
        outgoingDamageMultiplierMax = float.PositiveInfinity;

    }
}
