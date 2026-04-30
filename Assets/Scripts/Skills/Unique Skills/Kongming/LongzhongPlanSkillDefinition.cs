using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Kongming/Longzhong Plan")]
public class LongzhongPlanSkillDefinition : Skill
{
    public LongzhongPlanCounter counterPassive;
    public Skill followUpSkill;
    public Skill upgradedSkill;
    public int counterBreakpoint = 5;

    public override void Execute(BattleCharacter user, BattleCharacter target)
    {
        foreach (var passive in user.passives)
        {
            if(passive is LongzhongPlanCounter definition)
            {
                for (int i = 0; i < definition.counter; i++)
                {
                    
                    followUpSkill.InstantiateDetailShells();
                    skillDetailShell.followUpSkills.Add(followUpSkill);
                }
                if(definition.counter >= counterBreakpoint - counterPassive.counter && upgradedSkill != null)
                {
                    user.AddSkill(upgradedSkill);
                    user.RemoveSkill(this);
                }
                break;
            }
        }
        user.AddPassive(counterPassive);


        ExecuteFollowUps(user, target);
    }
}
