using UnityEngine;

public abstract class PassivesDefinition : ScriptableObject
{
    // Phase hooks
    public virtual void OnBattleStart(BattleCharacter self) { }
    public virtual void OnCommandPhaseStart(BattleCharacter self) { }
    public virtual void OnResolvePhaseStart(BattleCharacter self) { }

    // Combat event hooks
    public virtual void OnAfterDealDamage(BattleCharacter self, BattleCharacter target, int amount) { }
    public virtual void OnAfterTakeDamage(BattleCharacter self, BattleCharacter attacker, int amount) { }
}
