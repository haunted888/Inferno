using System.Collections.Generic;
using UnityEngine;

public static class BattleCharacterPreviewFactory
{
    public sealed class PreviewContext
    {
        public readonly List<BattleCharacter> party = new();
        public readonly List<BattleCharacter> enemies = new();

        private readonly List<GameObject> spawned = new();
        

        public void Track(GameObject go)
        {
            if (go != null)
                spawned.Add(go);
        }

        public void Dispose()
        {
            for (int i = 0; i < spawned.Count; i++)
            {
                if (spawned[i] != null)
                    Object.Destroy(spawned[i]);
            }

            spawned.Clear();
            party.Clear();
            enemies.Clear();
        }
    }

    public static PreviewContext Build(
        List<MapPartyMemberDefinition> partyDefs,
        List<MapEnemyDefinition> enemyDefs)
    {
        var context = new PreviewContext();
        var mutationContext = new PassiveMutationUtility.PassiveMutationContext();

        if (partyDefs != null)
        {
            foreach (var def in partyDefs)
            {
                if (def == null) continue;
                def.EnsureInitializedFromAsset();

                var go = new GameObject($"PreviewParty_{def.GetDisplayName()}");
                go.hideFlags = HideFlags.HideAndDontSave;
                var chr = go.AddComponent<BattleCharacter>();
                chr.passiveMutationContext = mutationContext;
                context.Track(go);

                chr.sourceDefinition = def;

                int currentHp = def.health < 0
                    ? def.GetMaxHealth()
                    : Mathf.Clamp(def.health, 0, def.GetMaxHealth());

                int currentSp = def.sp < 0
                    ? def.GetMaxSp()
                    : Mathf.Clamp(def.sp, 0, def.GetMaxSp());

                chr.ApplyStats(def.GetEffectiveStats(), currentHp);
                chr.SetMaxSp(def.GetMaxSp(), fillToMax: false);
                chr.SetSp(currentSp);

                chr.ClearSkills();
                foreach (var s in def.GetEffectiveSkills())
                    if (s != null) chr.AddSkill(s);

                chr.ClearPassives();
                if (def.passives != null)
                {
                    foreach (var p in def.passives)
                        if (p != null) chr.AddPassive(p);
                }

                chr.ClearTraits();
                if (def.traits != null)
                {
                    foreach (var t in def.traits)
                    {
                        if (t == null) continue;
                        chr.Traits.Add(t);
                        chr.traitTypes.Add(t.traitType);
                    }

                    foreach (var t in chr.Traits)
                        t.SetupForBattle(def, chr);
                }

                context.party.Add(chr);
            }
        }

        if (enemyDefs != null)
        {
            foreach (var def in enemyDefs)
            {
                if (def == null) continue;
                def.EnsureInitializedFromAsset();

                var go = new GameObject($"PreviewEnemy_{def.GetDisplayName()}");
                go.hideFlags = HideFlags.HideAndDontSave;
                var chr = go.AddComponent<BattleCharacter>();
                chr.passiveMutationContext = mutationContext;
                context.Track(go);

                chr.ApplyStats(def.GetEffectiveStats(), def.GetMaxHealth());
                chr.SetMaxSp(def.GetMaxSp(), fillToMax: false);
                chr.SetSp(def.GetMaxSp());

                chr.ClearSkills();
                foreach (var s in def.GetEffectiveSkills())
                    if (s != null) chr.AddSkill(s);

                chr.ClearPassives();
                if (def.passives != null)
                {
                    foreach (var p in def.passives)
                        if (p != null) chr.AddPassive(p);
                }

                context.enemies.Add(chr);
            }
        }

        for (int i = 0; i < context.party.Count; i++)
            context.party[i].SetPreviewTeams(context.party, context.enemies);

        for (int i = 0; i < context.enemies.Count; i++)
            context.enemies[i].SetPreviewTeams(context.enemies, context.party);

        TriggerBattleStartLikeBattleScene(context.party);
        TriggerBattleStartLikeBattleScene(context.enemies);

        return context;
    }

    private static void TriggerBattleStartLikeBattleScene(List<BattleCharacter> side)
    {
        for (int i = 0; i < side.Count; i++)
        {
            var chr = side[i];
            if (chr == null) continue;

            foreach (var trait in chr.Traits)
            {
                if (trait != null)
                    trait.OnBattleStart(chr);
            }
        }

        for (int i = 0; i < side.Count; i++)
        {
            var chr = side[i];
            if (chr == null) continue;

            var snapshot = new List<PassivesDefinition>(chr.passives);
            foreach (var passive in snapshot)
            {
                if (passive != null)
                    passive.OnBattleStart(chr);
            }
        }
    }
}