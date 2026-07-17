using UnityEngine;

[CreateAssetMenu(menuName = "Passives/Trait Passives/Sage/Kongming/Longzhong Plan Counter")]
public class LongzhongPlanCounter : PassivesDefinition
{
    public override void OnCreated(BattleCharacter self)
    {
        bool removeThis = false;
        foreach(var passive in self.passives)
        {
            if(passive is LongzhongPlanCounter definition && passive != this)
            {
                definition.counter += counter;
                removeThis = true;
            }
        }
        if (removeThis)
        {
            self.RemovePassive(this);
        }
    }

    public override string GetDescription(BattleCharacter character)
    {
        return $"Currently at stage {counter} of the Longzhong plan.";
    }

    public override void OnResolvePhaseEnd(BattleCharacter self) { }
}
