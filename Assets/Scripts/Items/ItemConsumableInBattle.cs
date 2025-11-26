using UnityEngine;

public abstract class ItemConsumableInBattle : ScriptableObject
{
    // Default: items need a target. Override to false for self-use, no targeting (e.g., Health Potion).
    public virtual bool RequiresTarget => true;

    // Optional validity check for click-to-target flow.
    public virtual bool CanTarget(BattleCharacter user, BattleCharacter target)
        => target != null && !target.IsDead;

    // Apply effect. Implementations must remove 1 from inventory if they succeed.
    public abstract void Execute(BattleCharacter user, BattleCharacter target, ItemDefinition item);
}
