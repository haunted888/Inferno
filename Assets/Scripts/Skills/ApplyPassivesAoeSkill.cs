using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Apply Passives To Team")]
public class ApplyPassivesAoeSkill : Skill
{
    [Header("Passives To Apply")]
    public List<PassivesDefinition> passivesToApply = new List<PassivesDefinition>();
    public affectsCharacters characters;

    public override void Execute(BattleCharacter user, BattleCharacter target)
    {
        if (user == null || target == null) return;
        if (passivesToApply == null || passivesToApply.Count == 0) return;

        
        BeforeSkillExecute(user, target);

        List<BattleCharacter> group;

        
        switch (characters)
        {
            case affectsCharacters.Target:
                group = new List<BattleCharacter> { target };
                break;
            case affectsCharacters.TargetTeam:
                group = new List<BattleCharacter>(target.GetAllies());
                break;
            case affectsCharacters.Self:
                group = new List<BattleCharacter> { user };
                break;
            case affectsCharacters.Allies:
                group = new List<BattleCharacter>(user.GetAllies());
                break;
            case affectsCharacters.Enemies:
                group = new List<BattleCharacter>(user.GetEnemies());
                break;
            case affectsCharacters.AllOtherAllies:
                group = new List<BattleCharacter>(user.GetAllies());
                group.Remove(user);
                break;
            default:
                group = new List<BattleCharacter>(target.GetAllies());
                break;
        }
        

        foreach (var member in group)
        {
            if (member == null || member.IsDead) continue;
            if (Random.value > skillDetailShell.bonusEffectChance) continue;

            for (int i = 0; i < passivesToApply.Count; i++)
            {
                var passive = passivesToApply[i];
                if (passive == null) continue;

                member.AddPassive(passive);
            }
        }

        
        
        ExecuteFollowUps(user, target);
        
        EndExecution();
    }
}