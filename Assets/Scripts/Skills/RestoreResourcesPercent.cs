using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Restore Resources Percent")]
public class RestoreResourcesPercent : Skill
{
    [Header("Restore Percent Skill")]
    [Header("Defaults to percent of max")]
    public bool percentMissing = false;

    [Range(0f, 1f)]
    public float spRestore = 0f;

    [Range(0f, 1f)]
    public float ammoRestore = 0f;

    public override void Execute(BattleCharacter user, BattleCharacter target)
    {

        BeforeSkillExecute(user, target);

        if(!percentMissing)
        {
            target.RecoverSp(Mathf.RoundToInt(spRestore * target.MaxSp));
            if(target.traitTypes.Contains(CharacterTrait.Marksman))
                target.AddAmmo(Mathf.RoundToInt(ammoRestore * target.MaxAmmo));
        }
        else
        {
            target.RecoverSp(Mathf.RoundToInt(Mathf.RoundToInt(spRestore * (target.MaxSp - target.CurrentSp))));
            if(target.traitTypes.Contains(CharacterTrait.Marksman))
                target.AddAmmo(Mathf.RoundToInt(Mathf.RoundToInt(ammoRestore * (target.MaxAmmo - target.CurrentAmmo))));
        }

        
        
        ExecuteFollowUps(user, target);
        
        EndExecution();
    }

    
}
