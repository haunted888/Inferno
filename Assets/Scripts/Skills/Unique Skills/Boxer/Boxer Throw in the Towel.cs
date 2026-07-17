using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Boxer/Throw in the Towel")]
public class BoxerThrowInTheTowel : Skill
{
    public StatBoostFlatPassiveDefinition buffToApply;

    public override void Execute(BattleCharacter user, BattleCharacter target)
    {
        CombatStats stats = user.GetEffectiveStats();
        buffToApply.SetStatBoosts(new CombatStats
        {
            physicalAttack = stats.physicalAttack,
            elementalPower = stats.elementalPower
        });

        foreach (var a in user.GetAllies())
        {
            if(a == user) continue;
            BeforeSkillExecute(user, a);
            a.AddPassive(buffToApply, user);
            a.RecoverSp(a.MaxSp);
            EndExecution();
        }

        user.TakeDamage(user.CurrentHealth); // Knock out the user

        EndExecution();

    }
}
