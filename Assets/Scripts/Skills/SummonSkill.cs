using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Summon")]
public class SummonSkill : Skill
{
    [Header("Assign one only")]
    public MapPartyMemberDefinition partySummonDefinition;
    public MapEnemyDefinition enemySummonDefinition;

    [Header("Applied to summoner when summon dies")]
    public PassivesDefinition dazedPassiveOnSummonDeath;

    [Header("Visual")]
    public bool hideSummonerWhileSummonIsAlive = true;

    public override void Execute(BattleCharacter user, BattleCharacter target)
    {
        if (user == null) return;
        if (user.HasLivingSummon()) return;

        BeforeSkillExecute(user, target);

        if (BattleTurnManager.Instance == null) return;

        BattleTurnManager.Instance.SpawnSummon(
            user,
            partySummonDefinition,
            enemySummonDefinition,
            dazedPassiveOnSummonDeath,
            hideSummonerWhileSummonIsAlive
        );

        ExecuteFollowUps(user, target);
        EndExecution();
    }
}