using UnityEngine;

[CreateAssetMenu(menuName = "Passives/Statuses/Bleeding")]
public class BleedingPassiveDefinition : PassivesDefinition
{
    public int bleedDamage = 1;

    public float healCutPercent = .5f;
    public int counter = 1;
    private bool canTrigger = false;
    
    public override void OnCreated(BattleCharacter self)
    {
        bool removeThisBurn = false;
        foreach(var passive in self.passives)
        {
            if(passive is BurnPassiveDefinition && passive != this)
            {
                ((BurnPassiveDefinition)passive).counter++;
                removeThisBurn = true;
            }
        }
        if (removeThisBurn)
        {
            self.RemovePassive(this);
        }
    }

    public override void OnSkillReceived(BattleCharacter self, BattleCharacter attacker, Skill skill)
    {
        canTrigger = true;
    }

    public override void OnSkillReceivedEnd(BattleCharacter self, BattleCharacter attacker, Skill skill)
    {

        if(!skill.GetAllEffectTypes().Contains(SkillEffectType.Damage)) return;
        if (bleedDamage <= 0) return;
        if (!canTrigger) return;
        self.TakeDamage(bleedDamage);
    }

    public override void BeforeReceivingHealing(BattleCharacter self, int healingAmount)
    {
        self.ModifyIncomingHealingMultiplier(1 -healCutPercent);
    }

    public override void OnResolvePhaseEnd(BattleCharacter self)
    {
        if (self == null) return;
        counter--;
        if (counter <= 0)
        {
            self.QueuePassiveToRemove(this);
        }
    }

}
