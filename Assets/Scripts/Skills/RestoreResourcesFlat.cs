using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Restore Resources Flat")]
public class RestoreResourcesFlat : Skill
{
    [Header("Restore Flat Skill")]

    public int spRestore = 0;

    public int ammoRestore = 0;

    public override void Execute(BattleCharacter user, BattleCharacter target)
    {

        BeforeSkillExecute(user, target);

        target.RecoverSp(spRestore);

        if(target.traitTypes.Contains(CharacterTrait.Marksman))
            target.AddAmmo(ammoRestore);

        
        
        ExecuteFollowUps(user, target);
        
        EndExecution();
    }

    
}
