using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Charge")]
public class ChargeSkill : Skill
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("Charge Skill")]
    public affectsCharacters characters;
    public Skill skillToUse;


    public override void Execute(BattleCharacter user, BattleCharacter target)
    {
        List<BattleCharacter> group;

        switch (characters)
        {
            case affectsCharacters.Allies:
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
            skillToUse.Execute(character, target);

        }
    }
}
