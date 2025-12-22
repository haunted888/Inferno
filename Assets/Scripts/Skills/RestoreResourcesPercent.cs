using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Battle/Skills/Restore Resources Percent")]
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
        if(!percentMissing)
        {
            user.RecoverSp(Mathf.RoundToInt(spRestore * user.MaxSp));
            if(user.traitTypes.Contains(CharacterTrait.Marksman))
                user.AddAmmo(Mathf.RoundToInt(ammoRestore * user.MaxAmmo));
        }
        else
        {
            user.RecoverSp(Mathf.RoundToInt(Mathf.RoundToInt(spRestore * (user.MaxSp - user.CurrentSp))));
            if(user.traitTypes.Contains(CharacterTrait.Marksman))
                user.AddAmmo(Mathf.RoundToInt(Mathf.RoundToInt(ammoRestore * (user.MaxAmmo - user.CurrentAmmo))));
        }
    }
}
