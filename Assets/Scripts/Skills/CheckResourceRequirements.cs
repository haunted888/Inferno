using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Utility/Check Resource Requirements")]
public class CheckResourceRequirements : Skill
{
    public Skill skillReference;
    
    public override void Execute(BattleCharacter user, BattleCharacter target)
    {
        if (user == null || target == null) return;
        if (skillReference == null) return;

        int hpCost = Mathf.CeilToInt(skillReference.skillDetailShell.hpCost * user.MaxHealth);

        int spCost = skillReference.skillDetailShell.spCost;


        if (user.CurrentHealth < hpCost || user.CurrentSp < spCost)
        {
            EndExecution();
            return; // Not enough resources, so the skill fails
        }

        
        // Marksman ammo check
        if(user.traitTypes.Contains(CharacterTrait.Marksman))
        {
            float percent = Mathf.Clamp01(skillReference.skillDetailShell.ammoCost);
            int neededAmmo = Mathf.CeilToInt(percent * user.ConstantMaxAmmo);
            neededAmmo += skillReference.skillDetailShell.flatAmmoCost;

            if (user.CurrentAmmo < neededAmmo )
            {
                EndExecution();
                return; // Not enough ammo, so the skill fails
            }

        }
        
        ExecuteFollowUps(user, target);
        
        EndExecution();
    }

    public override int EstimateDamage(BattleCharacter user, BattleCharacter target)
    {
        return 0;
    }
}
