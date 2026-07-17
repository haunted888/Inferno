using System;
using System.Collections.Generic;
using UnityEngine;

public enum PassivesTypes
{
    Misc,
    StatModifier,
    StatusEffect,
    Buff,
    Debuff,
    Equipment,
    Protected
}

public abstract class PassivesDefinition : ScriptableObject
{
    public enum PassiveHook
    {
        GetStatBoosts,
        OnBattleStart,
        OnBattleEnd,
        OnCommandPhaseStart,
        OnResolvePhaseStart,
        OnResolvePhaseEnd,
        OnAfterDealDamage,
        OnAfterTakeDamage,
        OnSkillUsed,
        BeforeDamageSkillExecute,
        BeforeHealingSkillExecute,
        BeforeSkillExecute,
        BeforeSkillExecuteReceived,
        BeforeReceivingHealing,
        OnSkillUsedEnd,
        OnSkillReceived,
        OnSkillReceivedEnd,
        OnActionEnd,
        OnActionOrdered,
        OnCreated,
        OnDestroyed
    }

    // Display Name
    public string displayName = "Passive";

    public PassivesTypes type = PassivesTypes.Misc;
    
    public bool isInstance = true;
    
    [Header("Icon")]
    public Sprite icon;
    
    [Header("Description")]
    [TextArea (minLines: 3, maxLines: 10)] public string description;

    
    [Header("Stacking")]
    public bool isStackable = false; // Whether this passive can stack with others of the same type
    public int stacks = 1; // Number of stacks for this passive, if stackable

    [Header("Duration")]
    public int counter = 0;

    private string displayText = "";

    [NonSerialized]public BattleCharacter applicator = null;



    // Passive stats
    public virtual void GetStatBoosts(BattleCharacter self) {  }

    // Phase hooks
    public virtual void OnBattleStart(BattleCharacter self) { }
    public virtual void OnBattleEnd(BattleCharacter self, bool playerWon) { }
    public virtual void OnCommandPhaseStart(BattleCharacter self) { }
    public virtual void OnResolvePhaseStart(BattleCharacter self) { }
    public virtual void OnResolvePhaseEnd(BattleCharacter self)
    {
        if (self == null) return;
        if (counter <= 0 || counter > 100000) return;

        counter--;
        if (counter <= 0)
        {
            self.QueuePassiveToRemove(this, PassiveHook.OnResolvePhaseEnd);
        }
    }

    // Combat event hooks
    public virtual void OnAfterDealDamage(BattleCharacter self, BattleCharacter target, int amount, SkillDamageType damageType, DamageSubType subDamageType) { }
    public virtual void OnAfterTakeDamage(BattleCharacter self, int amount, SkillDamageType damageType, DamageSubType subDamageType) { }
    public virtual void OnSkillUsed(BattleCharacter self, BattleCharacter target, Skill skill) { }
    public virtual void BeforeDamageSkillExecute(BattleCharacter self, BattleCharacter target, Skill skill) { }
    public virtual void BeforeHealingSkillExecute(BattleCharacter self, BattleCharacter target, Skill skill) { }
    public virtual void BeforeSkillExecute(BattleCharacter self, BattleCharacter target, Skill skill) { }
    public virtual void BeforeSkillExecuteReceived(BattleCharacter self, BattleCharacter attacker, Skill skill) { }
    public virtual void BeforeReceivingHealing(BattleCharacter self, int healingAmount) { }
    public virtual void OnSkillUsedEnd(BattleCharacter self, BattleCharacter target, Skill skill) { }
    public virtual void OnSkillReceived(BattleCharacter self, BattleCharacter attacker, Skill skill) { }
    public virtual void OnSkillReceivedEnd(BattleCharacter self, BattleCharacter attacker, Skill skill) { }
    public virtual void OnActionEnd(BattleCharacter self, BattleCharacter target) { }
    public virtual void OnActionOrdered(QueuedAction action, List<QueuedAction> actions) { }

    // Out of combat hooks
    public virtual void OnGetSkills(List<Skill> effectiveSkills, List<Skill> skills) { }
    public virtual void OnResetLevels(MapPartyMemberDefinition self, List<TalentDefinition> talents) { }
    public virtual int OnGainXp(MapPartyMemberDefinition self, int xpGained) { return xpGained; }
    public virtual bool BeforeDamageSkillExecuteOncePerSkill => false;

    // Existence hooks
    public virtual void OnCreated(BattleCharacter self) { }
    public virtual void OnDestroyed(BattleCharacter self) { }

    public virtual string GetDescription(BattleCharacter character)
    {
        return description;
    }

    public void SetDisplayText(string text)
    {
        displayText = text;
    }

    public String GetDisplayText()
    {
        return displayText;
    }





    



}
