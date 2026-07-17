using UnityEngine;

[System.Serializable]
public class MapEnemyDefinition : MapCharacterDefinition
{
    public GameObject enemyPrefab;

    [Header("Progression")]
    public const int MaxLevel = 20;

    [Header("Rewards Granted On Defeat")]
    public MapRewardGroup[] defeatRewardGroups;

    [Header("Stats (added to character asset unless overrideStats = true)")]
    public CombatStats overrideStatsValues = new CombatStats { };

    protected override float LevelUpBaseMultiplier => .14f;
    protected override float LevelUpLevelMultiplier => 0.06f;
    protected override bool IncludeDamageTypeStatsInLevelUps => true;

    public MapEnemyDefinition()
    {
        stats = new CombatStats
        {
            maxHealth = 50,
            maxSp = 10,
            spGeneration = 20,
            speed = 10,
            physicalAttack = 100,
            elementalPower = 100,
            defense = 0,
            elementalResistance = 0,
            critChance = 5,
            critDamage = 150
        };
    }

    protected override void InitializeStatsFromAsset()
    {
        if (initializedFromAssetStats)
            return;

        if (!overrideStats)
        {
            stats = characterAsset.baseStats;
            ApplyStatsDelta(overrideStatsValues, +1);
        }
        else
        {
            stats = overrideStatsValues;
        }

        baseStats = stats;

        for (int i = 1; i < level; i++)
            ApplyLevelUpEffects(i);

        ClampStats();
        initializedFromAssetStats = true;
    }
}
