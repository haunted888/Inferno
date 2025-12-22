using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class PassivesDefinition : ScriptableObject
{
    // Passive stats
    public virtual void getStatBoosts(BattleCharacter self) {  }

    // Phase hooks
    public virtual void OnBattleStart(BattleCharacter self) { }
    public virtual void OnCommandPhaseStart(BattleCharacter self) { }
    public virtual void OnResolvePhaseStart(BattleCharacter self) { }

    // Combat event hooks
    public virtual void OnAfterDealDamage(BattleCharacter self, BattleCharacter target, int amount) { }
    public virtual void OnAfterTakeDamage(BattleCharacter self, BattleCharacter attacker, int amount) { }
    public virtual void OnSkillUsed(BattleCharacter self, Skill skill) { }
    public virtual void OnSkillUsedEnd(BattleCharacter self, Skill skill) { }
    public virtual void OnSkillReceived(BattleCharacter self, BattleCharacter attacker, Skill skill) { }
    public virtual void OnSkillReceivedEnd(BattleCharacter self, BattleCharacter attacker, Skill skill) { }
    public virtual void OnActionOrdered(QueuedAction action, List<QueuedAction> actions) { }

    // Existence hooks
    public virtual void OnCreated(BattleCharacter self) { }
    public virtual void OnDestroyed(BattleCharacter self) { }

}
