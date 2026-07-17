using UnityEngine;

[CreateAssetMenu(menuName = "Passives/Statuses/Bleeding")]
public class BleedingPassiveDefinition : StatusPassiveDefinition
{
    public int bleedDamage = 1;

    public override void OnCreated(BattleCharacter self)
    {
        if(CharHasStatus(self)) return;
        bool removeThisBleed = false;
        foreach(var passive in self.passives)
        {
            if(passive is BleedingPassiveDefinition definition && passive != this)
            {
                definition.counter++;
                removeThisBleed = true;
            }
        }
        if (removeThisBleed)
        {
            self.RemovePassive(this);
        }
    }


    public override void OnSkillReceivedEnd(BattleCharacter self, BattleCharacter attacker, Skill skill)
    {

        if(!skill.GetAllEffectTypes().Contains(SkillEffectType.Damage)) return;
        if (bleedDamage <= 0) return;
        self.TakeDamage(bleedDamage, SkillDamageType.Elemental, DamageSubType.Blood);
    }


    public override void OnResolvePhaseEnd(BattleCharacter self)
    {
        if (self == null) return;

        base.OnResolvePhaseEnd(self);
    }

    public override void OnDestroyed(BattleCharacter self)
    {
        ApplyStatusBuffer(self);
    }


}
