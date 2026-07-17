using UnityEngine;
using System.Collections.Generic;
using System;
using System.Xml.XPath;
using UnityEngine.Events;

public enum ResourceType
{
    HP,
    SP,
    Ammo
}

public class BattleCharacter : MonoBehaviour
{
    [Min(1)]
    public int slotSize = 1;   // how many slots this character “occupies”


    public int level = 1;

    //NOTE: Check later to see if these need to be serialized
    [Header("Health")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;

    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;
    public bool IsDead => currentHealth <= 0;
    [NonSerialized] public bool IsAsleep = false;
    [NonSerialized] public bool IsDazed = false;
    [NonSerialized] public bool IsSoaked = false;
    [NonSerialized] public bool IsSteamed = false;
    [NonSerialized] public bool IsFrostbitten = false;
    [NonSerialized] public float FrostBitePercent = 1.0f/3.0f;
    [NonSerialized] public bool IsFrozen = false;
    [NonSerialized] public int DelayedCastTurns = 0;
    [NonSerialized] public QueuedAction DelayedCastSkill = null;


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
    public MapEnemyDefinition sourceEnemyDefinition;

    [NonSerialized] public bool defeatRewardsGranted = false;

    public List<TraitDefinition> Traits { get; } = new List<TraitDefinition>();
    public List<CharacterTrait> traitTypes = new List<CharacterTrait>();

    [NonSerialized] public PassiveMutationUtility.PassiveMutationContext passiveMutationContext;

    public UnityEvent<PassivesDefinition[]> OnPassivesChanged;

    public List<QueuedAction> currentActionOrder;

    [NonSerialized] public bool hideWhileSummonIsAlive;

    [NonSerialized] public Skill lastUsedSkill;
    
    

    void Awake()
    {
        
        // Initialize current health at full
        currentHealth = Mathf.Max(1, maxHealth);

        // Initialize SP at full by default
        currentSp = Mathf.Max(0, maxSp);

        IsAsleep = false;
        IsDazed = false;
        IsSoaked = false;
        IsSteamed = false;
        IsFrostbitten = false;
        IsFrozen = false;
        FrostBitePercent = 1.0f/3.0f;
        DelayedCastTurns = 0;
        DelayedCastSkill = null;
    }

    public int TakeDamage(int amount, SkillDamageType damageType = SkillDamageType.None, DamageSubType subType = DamageSubType.None)
    {
        if (amount <= 0 || IsDead) return 0;

        int oldHealth = currentHealth;
        currentHealth = Mathf.Max(0, currentHealth - amount);
        int dealt = oldHealth - currentHealth;

        
        PassiveMutationUtility.InvokePassivesWithMutation(
            this,
            () => passives,
            p => p.OnAfterTakeDamage(this, amount, damageType, subType),
            PassivesDefinition.PassiveHook.OnAfterTakeDamage,
            passiveMutationContext
        );

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

    public void Die()
    {
        if (IsDead) return;

        currentHealth = 0;
        Debug.Log($"{name} has died.");

        if (BattleTurnManager.Instance != null)
            BattleTurnManager.Instance.HandleCharacterDeath(this);

    }


    [Header("AI / Threat")]
    [SerializeField] private int threat;
    public int Threat => threat;

    public void AddThreat(int amount)
    {
        threat += amount;
    }

    public void SetThreat(int newThreat)
    {
        threat = newThreat;
    }

    // Optional, for later if you want to reset completely
    public void ResetThreat()
    {
        threat = 0;
    }

    private float healingMultiplier = 1f;
    private float healingMultiplierMax = float.PositiveInfinity;

    private float healingMultiplierMin = 0f;

    public void ModifyIncomingHealingMultiplier(float percentBonus)
    {
        healingMultiplier *= percentBonus;
    }

    public int ApplyIncomingHealingModifiers(int healing)
    {
        if (passives != null)
        {
            PassiveMutationUtility.InvokePassivesWithMutation(
                this,
                () => passives,
                p => p.BeforeReceivingHealing(this, healing),
                PassivesDefinition.PassiveHook.BeforeReceivingHealing,
                passiveMutationContext
            );
        }

        if (healingMultiplierMin > healingMultiplierMax)
            healingMultiplierMin = healingMultiplierMax;
        if (healingMultiplier > healingMultiplierMax)
            healingMultiplier = healingMultiplierMax;
        if (healingMultiplier < healingMultiplierMin)
            healingMultiplier = healingMultiplierMin;
        return Mathf.Max(0, Mathf.CeilToInt(healing * healingMultiplier));
    }

    public void SetIncomingHealingMultiplierLimits(float min = 0f, float max = float.PositiveInfinity)
    {
        if(healingMultiplierMin < min)
            healingMultiplierMin = min;
        if(healingMultiplierMax > max)
            healingMultiplierMax = max;
    }

    public void ClearIncomingHealingModifiers()
    {
        healingMultiplier = 1f;
        healingMultiplierMin = 0f;
        healingMultiplierMax = float.PositiveInfinity;
    }

    public void Heal(int amount)
    {
        if (amount <= 0 || IsDead) return;

        amount = ApplyIncomingHealingModifiers(amount);

        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        Debug.Log($"{name} healed {amount}. HP: {currentHealth}/{maxHealth}");
    }

    public void SetCurrentHealth(int newHealth)
    {
        currentHealth = Mathf.Clamp(newHealth, 0, maxHealth);
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

    public void UseSkill(int skillIndex, BattleCharacter target)
    {
        if (skillIndex < 0 || skillIndex >= Skills.Count) return;
        UseSkill(Skills[skillIndex], target);
    }

    public void UseSkill(Skill skill, BattleCharacter target)
    {

        if (skill == null)
        {
            Debug.LogWarning($"{name} has null skill, cannot execute.");
            return;
        }

        int cost = skill.skillDetailShell.spCost; // new field on Skill
        if (!TrySpendSp(cost))
        {
            Debug.Log($"{name} does not have enough SP ({currentSp}/{cost}) to use {skill.skillName}.");
            return;
        }

        float hpCost = skill.skillDetailShell.hpCost; // new field on Skill
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
    public void AddPassive(PassivesDefinition p, BattleCharacter applicator = null)
    {
        var passive = p;
        if (p != null) {
            if(p.isInstance)
                passive = Instantiate(p);
                passive.applicator = applicator;
            passives.Add(passive);
            passive.OnCreated(this);

            if(passive.applicator != null){
                foreach(var t in passive.applicator.Traits)
                {
                    t.OnPassiveApplied(passive.applicator, passive, this);
                }
            }

            OnPassivesChanged?.Invoke(passives.ToArray());
        }
        
    }

    public void AddPassiveNoInstance(PassivesDefinition p)
    {
        if (p != null) {
            passives.Add(p);
            p.OnCreated(this);
            
            if(p.applicator != null){
                foreach(var t in p.applicator.Traits)
                {
                    t.OnPassiveApplied(p.applicator, p, this);
                }
            }

            OnPassivesChanged?.Invoke(passives.ToArray());
        }
        
    }

    public void RemovePassive(PassivesDefinition p)
    {
        if (p != null)
        {
            Debug.Log($"Removing passive {p.name} from {name}");
            p.OnDestroyed(this);
            passives.Remove(p);
            if(p.isInstance)
                Destroy(p);
            OnPassivesChanged?.Invoke(passives.ToArray());
        } 
    }

    public void UpdatePassives()
    {
        OnPassivesChanged?.Invoke(passives.ToArray());
    }

    public void ClearSkills() => skills.Clear();
    public void AddSkill(Skill s)
    {
        if (s == null) return;
        var skill = Instantiate(s);
        skills.Add(skill);
        skill.OnCreated(this);
        Debug.Log(skill.skillDetailShell);
    }
    public void RemoveSkill(Skill s)
    {
        if (s != null)
        {
            skills.Remove(s);
        }
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
        CombatStats result = new CombatStats(){
            maxHealth           = baseStats.maxHealth      + bonusStats.maxHealth,
            maxSp               = baseStats.maxSp          + bonusStats.maxSp,
            spGeneration        = baseStats.spGeneration + bonusStats.spGeneration,
            physicalAttack      = baseStats.physicalAttack + bonusStats.physicalAttack,
            elementalPower      = baseStats.elementalPower + bonusStats.elementalPower,
            defense             = baseStats.defense + bonusStats.defense,
            elementalResistance = baseStats.elementalResistance + bonusStats.elementalResistance,
            speed               = baseStats.speed + bonusStats.speed,
            critChance          = baseStats.critChance + bonusStats.critChance,
            critDamage          = baseStats.critDamage + bonusStats.critDamage,

            piercingAttack      = baseStats.piercingAttack + bonusStats.piercingAttack,
            bludgeoningAttack   = baseStats.bludgeoningAttack + bonusStats.bludgeoningAttack,
            slashingAttack      = baseStats.slashingAttack + bonusStats.slashingAttack,

            fireAttack          = baseStats.fireAttack + bonusStats.fireAttack,
            iceAttack           = baseStats.iceAttack  + bonusStats.iceAttack,
            stormAttack         = baseStats.stormAttack  + bonusStats.stormAttack,
            acidAttack          = baseStats.acidAttack   + bonusStats.acidAttack,
            psychicAttack       = baseStats.psychicAttack + bonusStats.psychicAttack,
            bloodAttack         = baseStats.bloodAttack    + bonusStats.bloodAttack,

            piercingDefense     = baseStats.piercingDefense + bonusStats.piercingDefense,
            bludgeoningDefense  = baseStats.bludgeoningDefense + bonusStats.bludgeoningDefense,
            slashingDefense     = baseStats.slashingDefense  + bonusStats.slashingDefense,

            fireDefense         = baseStats.fireDefense  + bonusStats.fireDefense,
            iceDefense          = baseStats.iceDefense   + bonusStats.iceDefense,
            stormDefense        = baseStats.stormDefense + bonusStats.stormDefense,
            acidDefense         = baseStats.acidDefense    + bonusStats.acidDefense,
            psychicDefense      = baseStats.psychicDefense + bonusStats.psychicDefense,
            bloodDefense        = baseStats.bloodDefense     + bonusStats.bloodDefense,

            accuracy          = baseStats.accuracy + bonusStats.accuracy,
            evasion           = baseStats.evasion + bonusStats.evasion
        };

        if (IsSoaked)
        {
            var soak = passives.Find(p => p is SoakedPassiveDefinition) as SoakedPassiveDefinition;
            if(soak != null)
            {
                result.physicalAttack = (int)(result.physicalAttack * soak.GetSoakMultiplier());
                result.elementalPower = (int)(result.elementalPower * soak.GetSoakMultiplier());
                result.speed = (int)(result.speed * soak.GetSoakMultiplier());
                Debug.Log($"{result.speed} is the new speed after soak multiplier");
            }

        }

        if (IsSteamed)
        {
            var steamed = passives.Find(p => p is SteamedPassiveDefinition) as SteamedPassiveDefinition;
            if(steamed != null)
            {
                result.defense = (int)(result.defense * steamed.GetSteamedMultiplier());
                result.elementalResistance = (int)(result.elementalResistance * steamed.GetSteamedMultiplier());
            }
        }

        return result;
    }

    public void SetName(string newName)
    {
        this.name = newName;
    }

    public void SetLevel(int newLevel)
    {
        level = Mathf.Max(1, newLevel);
    }

    public int GetSpeed()
    {
        var stats = GetEffectiveStats();
        return stats.speed;
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

    public bool HasEnoughResourcesFor(Skill skill)
    {
        if (skill == null) return true;

        Debug.Log($"Checking if {name} has enough resources for {skill.skillName}.");

        Debug.Log($"{skill.skillDetailShell}");

        if (currentSp < skill.skillDetailShell.spCost) return false;

        if (currentHealth < skill.skillDetailShell.hpCost) return false;

        if (!HasEnoughAmmoFor(skill)) return false;

        return true;
    }

    public void GenerateTurnSp()
    {
        CombatStats stats = GetEffectiveStats();
        int amount = Mathf.CeilToInt(MaxSp * (stats.spGeneration / 100f));
        RecoverSp(amount);
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

    public void SetAmmo(int max, int current)
    {
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
        if (skill.skillDetailShell.ammoCost <= 0f && skill.skillDetailShell.flatAmmoCost <= 0)
            return true;

        if (ConstantMaxAmmo <= 0)
            return false;

        // Required ammo = ceil(percent * constantMaxAmmo)
        float percent = Mathf.Clamp01(skill.skillDetailShell.ammoCost);
        int needed = Mathf.CeilToInt(percent * ConstantMaxAmmo);
        needed += skill.skillDetailShell.flatAmmoCost;

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

    public void QueuePassiveToAdd(PassivesDefinition passive, PassivesDefinition.PassiveHook hook, BattleCharacter applicator = null)
    {
        if (passive == null) return;

        if (passiveMutationContext != null)
        {
            if(passive.isInstance)
                passive = Instantiate(passive);
                passive.applicator = applicator;
            passiveMutationContext.passivesToAdd.Add(passive, hook);
        }
        else
        {
            AddPassive(passive, applicator);
        }
    }

    public void QueuePassiveToRemove(PassivesDefinition passive, PassivesDefinition.PassiveHook hook)
    {
        if (passive == null) return;

        if (passiveMutationContext != null)
        {
            passiveMutationContext.passivesToRemove.Add(passive, hook);
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

    public List<QueuedAction> GetCurrentActionOrderMutable()
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
        return Mathf.Max(0, Mathf.CeilToInt(damage * incomingDamageMultiplier));
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

    public void HandleSkippedAction()
    {
        DelayedCastSkill = null;
        DelayedCastTurns = 0;
    }



    //SUMMON
    [NonSerialized] public BattleCharacter activeSummon;
    [NonSerialized] public BattleCharacter summoner;
    [NonSerialized] public PassivesDefinition onSummonDeathPassive;

    public bool HasLivingSummon()
    {
        return activeSummon != null && !activeSummon.IsDead;
    }

    public bool HasTrait(CharacterTrait trait)
    {
        return traitTypes.Contains(trait);
    }

    public bool IsProtectedFromSkills()
    {
        foreach (var p in passives)
        {
            if (p == null) continue;
            if (p is StasisPassiveDefinition)
                return true;
        }
        return false;
    }

    private bool isDodging = false;

    public void SetDodge(int accuracy)
    {
        int evasion = GetEffectiveStats().evasion - accuracy;
        isDodging = UnityEngine.Random.value > Mathf.Max(0, 1f - evasion / (100f + evasion)); //100 is evasion at which you have 50% dodge chance, formula is ev/(100+ev)
    }

    public bool IsDodging()
    {
        return isDodging;
    }
}
