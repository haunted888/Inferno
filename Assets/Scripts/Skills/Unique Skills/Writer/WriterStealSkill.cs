using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Writer/Steal Skill")]
public class WriterStealSkill : Skill
{
    public override void Execute(BattleCharacter user, BattleCharacter target)
    {
        List<Skill> skillsToSteal = new List<Skill>();
        foreach (var skill in target.Skills)
        {
            
            if(skill.spCost > 0 && !user.Skills.Contains(skill))
            {
                skillsToSteal.Add(skill);
            }
        }

        if (skillsToSteal.Count == 0)
            return;

        var skillStolen = skillsToSteal[Random.Range(0, skillsToSteal.Count)];
        
        user.AddSkill(skillStolen);
        user.RemoveSkill(this);
        user.sourceDefinition.skills.Add(skillStolen);
        user.sourceDefinition.skills.Remove(this);
        
    }
}
