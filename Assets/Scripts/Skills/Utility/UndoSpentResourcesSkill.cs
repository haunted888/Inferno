using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Utility/Restore Resources From Skill")]
public class RestoreResourcesFromSkill : Skill
{
    public Skill skillReference;

    [Header("Restore Multipliers")]
    public float hpRestoreMultiplier = 1f;
    public float spRestoreMultiplier = 1f;
    public float ammoRestoreMultiplier = 1f;

    public override void Execute(BattleCharacter user, BattleCharacter target)
    {
        if (user == null) return;
        if (skillReference == null) return;


        int hpCost = Mathf.CeilToInt(skillReference.skillDetailShell.hpCost * user.MaxHealth);
        int spCost = skillReference.skillDetailShell.spCost;

        if (hpCost > 0)
            user.Heal(Mathf.CeilToInt(hpCost * hpRestoreMultiplier));

        if (spCost > 0)
            user.RecoverSp(Mathf.CeilToInt(spCost * spRestoreMultiplier));

        if (user.traitTypes.Contains(CharacterTrait.Marksman))
        {
            float percent = Mathf.Clamp01(skillReference.skillDetailShell.ammoCost);
            int ammoCost = Mathf.CeilToInt(percent * user.ConstantMaxAmmo);
            ammoCost += skillReference.skillDetailShell.flatAmmoCost;

            if (ammoCost > 0)
                user.AddAmmo(Mathf.CeilToInt(ammoCost * ammoRestoreMultiplier));
        }

        ExecuteFollowUps(user, target);

        EndExecution();
    }

}