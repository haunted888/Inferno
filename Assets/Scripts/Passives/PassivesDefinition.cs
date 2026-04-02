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
    Protected
}

public abstract class PassivesDefinition : ScriptableObject
{
    // Display Name
    public string displayName = "Passive";

    public PassivesTypes type = PassivesTypes.Misc;
    // Passive stats
    public virtual void GetStatBoosts(BattleCharacter self) {  }

    // Phase hooks
    public virtual void OnBattleStart(BattleCharacter self) { }
    public virtual void OnCommandPhaseStart(BattleCharacter self) { }
    public virtual void OnResolvePhaseStart(BattleCharacter self) { }
    public virtual void OnResolvePhaseEnd(BattleCharacter self) { }

    // Combat event hooks
    public virtual void OnAfterDealDamage(BattleCharacter self, BattleCharacter target, int amount) { }
    public virtual void OnAfterTakeDamage(BattleCharacter self, BattleCharacter attacker, int amount) { }
    public virtual void OnSkillUsed(BattleCharacter self, BattleCharacter target, Skill skill) { }
    public virtual void BeforeDamageSkillExecute(BattleCharacter self, BattleCharacter target, Skill skill) { }
    public virtual void BeforeHealingSkillExecute(BattleCharacter self, BattleCharacter target, Skill skill) { }
    public virtual void BeforeReceivingHealing(BattleCharacter self, int healingAmount) { }
    public virtual void OnSkillUsedEnd(BattleCharacter self, BattleCharacter target, Skill skill) { }
    public virtual void OnSkillReceived(BattleCharacter self, BattleCharacter attacker, Skill skill) { }
    public virtual void OnSkillReceivedEnd(BattleCharacter self, BattleCharacter attacker, Skill skill) { }
    public virtual void OnActionOrdered(QueuedAction action, List<QueuedAction> actions) { }

    // Existence hooks
    public virtual void OnCreated(BattleCharacter self) { }
    public virtual void OnDestroyed(BattleCharacter self) { }

}
