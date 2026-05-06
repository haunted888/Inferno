using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

[CreateAssetMenu(menuName = "Skills/Charge")]
public class ChargeSkill : Skill
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("Charge Skill")]
    public AffectsCharacters characters;
    public Skill skillToUse;


    public override void Execute(BattleCharacter user, BattleCharacter target)
    {
        List<BattleCharacter> group;

        
        BeforeSkillExecute(user, target);

        switch (characters)
        {
            case AffectsCharacters.Allies:
                group = new List<BattleCharacter>(user.GetAllies());
                break;
            default:
                group = new List<BattleCharacter>(user.GetAllies());
                group.Remove(user);
                break;
        }

        foreach (var character in group)
        {
            if (character == null) continue;
            UseNewSkill(character, target, skillToUse);

        }
        
        
        ExecuteFollowUps(user, target);
        
        EndExecution();
    }
}
