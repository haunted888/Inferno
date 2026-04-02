using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;

public class CalculatorScreen : MonoBehaviour
{
    [Header("Party UI")]
    public Transform partyListContainer;
    public CalculatorPartyEntryUI partyEntryPrefab;

    [Header("Enemy UI")]
    public Transform enemyListContainer;
    public CalculatorEnemyEntryUI enemyEntryPrefab;


    [Header("Party Lineup UI")]
    public Transform partyLineupContainer;
    public PartyLineupEntryUI partyLineupPrefab;

    [Header("Enemy Lineup UI")]
    public Transform enemyLineupContainer;
    public EnemyLineupEntryUI enemyLineupPrefab;

    [Header("Skill List UI")]
    public CalculatorSkillList skillListUI;
    public GameObject skillListContainer;

    private List<MapPartyMemberDefinition> currentParty;
    private List<MapEnemyDefinition> currentEnemies;

    private Skill pendingSkill;
    private bool pendingCasterIsEnemy;

    public Dictionary<string, Dictionary<effectKey, int>> effectsByTarget = new Dictionary<string, Dictionary<effectKey, int>>();


    private readonly List<CalculatorPartyEntryUI> partyEntries = new List<CalculatorPartyEntryUI>();
    private readonly List<CalculatorEnemyEntryUI> enemyEntries = new List<CalculatorEnemyEntryUI>();
    private readonly List<PartyLineupEntryUI> partyLineupEntries = new List<PartyLineupEntryUI>();
    private readonly List<EnemyLineupEntryUI> enemyLineupEntries = new List<EnemyLineupEntryUI>();

    [Header("Stat Screen")]
    public GameObject statScreenRoot;
    public Transform statEntryContainer;
    public CalcStatEntryUI statEntryPrefab;

    public enum effectKey
    {
        Damage,
        Heal,
        SpChange,
        AmmoChange,

        MaxHealth,
        MaxSp,
        Speed,
        PhysicalAttack,
        ElementalPower,
        Defense,
        ElementalResistance,
        CritChance,
        CritDamage,

        BludgeoningAttack,
        SlashingAttack,
        PiercingAttack,

        BludgeoningDefense,
        SlashingDefense,
        PiercingDefense,

        FireAttack,
        IceAttack,
        StormAttack,
        AcidAttack,
        PsychicAttack,
        BloodAttack,

        FireDefense,
        IceDefense,
        StormDefense,
        AcidDefense,
        PsychicDefense,
        BloodDefense
    }



    private class CharacterSnapshot
    {
        public int currentHp;
        public int currentSp;
        public int currentAmmo;
        public CombatStats effectiveStats;
    }

    private HashSet<BattleCharacter> previewPartyLookup = new HashSet<BattleCharacter>();

    void Awake()
    {
        if (skillListUI != null)
            skillListUI.onSkillSelected += HandleSkillSelected;

        gameObject.SetActive(false);
    }

    void Update()
    {
        if (!gameObject.activeSelf) return;

        if (InputSystem.actions.FindAction("exit").WasPressedThisFrame())
        {
            bool hadOverlayOpen =
                (skillListContainer != null && skillListContainer.activeSelf) ||
                (statScreenRoot != null && statScreenRoot.activeSelf);

            if (hadOverlayOpen)
            {
                CloseOverlayScreens();
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }

    public void Open(List<MapPartyMemberDefinition> party, List<MapEnemyDefinition> enemies)
    {
        Debug.Log("Opening Calculator");
        gameObject.SetActive(true);

        CloseOverlayScreens();

       

        currentParty = new List<MapPartyMemberDefinition>();
        foreach (var def in party)
        {
            if (def != null)
                currentParty.Add(ClonePartyDef(def));
        }

        currentEnemies = new List<MapEnemyDefinition>();
        foreach (var def in enemies)
        {
            if (def != null)
                currentEnemies.Add(CloneEnemyDef(def));
        }

        PopulateParty(currentParty);
        PopulateEnemies(currentEnemies);

        PopulatePartyLineup(currentParty);
        PopulateEnemyLineup(currentEnemies);

        pendingSkill = null;
        DisableAllLineupButtons();
        UpdateTurnOrder();
    }

    private void PopulateParty(List<MapPartyMemberDefinition> party)
    {
        if (partyListContainer == null || partyEntryPrefab == null) return;

        for (int i = partyListContainer.childCount - 1; i >= 0; i--)
            Destroy(partyListContainer.GetChild(i).gameObject);

        partyEntries.Clear();

        if (party == null) return;

        foreach (var def in party)
        {
            if (def == null) continue;

            var row = Instantiate(partyEntryPrefab, partyListContainer);
            row.SetCalculatorScreen(this);
            row.Setup(def);
            partyEntries.Add(row);
        }
    }

    private void PopulateEnemies(List<MapEnemyDefinition> enemies)
    {
        if (enemyListContainer == null || enemyEntryPrefab == null) return;

        for (int i = enemyListContainer.childCount - 1; i >= 0; i--)
            Destroy(enemyListContainer.GetChild(i).gameObject);

        enemyEntries.Clear();
        if (enemies == null) return;

        foreach (var enemyDef in enemies)
        {
            if (enemyDef == null) continue;

            var row = Instantiate(enemyEntryPrefab, enemyListContainer);
            row.SetCalculatorScreen(this);
            row.Setup(enemyDef);
            enemyEntries.Add(row);
        }
    }

    private void PopulatePartyLineup(List<MapPartyMemberDefinition> party)
    {
        if (partyLineupContainer == null || partyLineupPrefab == null) return;

        for (int i = partyLineupContainer.childCount - 1; i >= 0; i--)
            Destroy(partyLineupContainer.GetChild(i).gameObject);

        if (party == null) return;

        partyLineupEntries.Clear();

        for (int i = 0; i < party.Count; i++)
        {
            var def = party[i];
            if (def == null) continue;

            var entry = Instantiate(partyLineupPrefab, partyLineupContainer);
            entry.Setup(def, i, OnPartyTargetClicked);
            entry.SetInteractable(false);
            partyLineupEntries.Add(entry);
        }
    }

    private void PopulateEnemyLineup(List<MapEnemyDefinition> enemies)
    {
        if (enemyLineupContainer == null || enemyLineupPrefab == null) return;

        for (int i = enemyLineupContainer.childCount - 1; i >= 0; i--)
            Destroy(enemyLineupContainer.GetChild(i).gameObject);

        if (enemies == null) return;

        enemyLineupEntries.Clear();

        for (int i = 0; i < enemies.Count; i++)
        {
            var def = enemies[i];
            if (def == null) continue;

            var entry = Instantiate(enemyLineupPrefab, enemyLineupContainer);
            entry.Setup(def, i, OnEnemyTargetClicked);
            entry.SetInteractable(false);
            enemyLineupEntries.Add(entry);
        }
    }

    private void HandleSkillSelected(Skill skill)
    {
        effectsByTarget.Clear();

        CloseOverlayScreens();

        if (skill == null) return;

        bool supported =
            skill is DamageSkill ||
            skill is DamageAllEnemiesSkill;

        if (!supported) return;

        pendingSkill = skill;

        pendingCasterIsEnemy = skillListUI != null && skillListUI.GetActiveEnemy() != null;

        if (pendingCasterIsEnemy)
            SetPartyLineupInteractable(true);
        else
            SetEnemyLineupInteractable(true);
    }

    private void OnPartyTargetClicked(int partyIndex)
    {
        if (pendingSkill == null) return;
        if (currentParty == null || partyIndex < 0 || partyIndex >= currentParty.Count) return;

        effectsByTarget = EstimateEffectsIncludingFollowUps(pendingSkill, true, partyIndex);

        pendingSkill = null;
        DisableAllLineupButtons();
        UpdateLineupEffects();
    }

    private void OnEnemyTargetClicked(int enemyIndex)
    {
        if (pendingSkill == null) return;
        if (currentEnemies == null || enemyIndex < 0 || enemyIndex >= currentEnemies.Count) return;

        effectsByTarget = EstimateEffectsIncludingFollowUps(pendingSkill, false, enemyIndex);

        pendingSkill = null;
        DisableAllLineupButtons();
        UpdateLineupEffects();
    }
    private Dictionary<string, Dictionary<effectKey, int>> EstimateEffectsIncludingFollowUps(
        Skill rootSkill,
        bool casterIsEnemy,
        int primaryTargetIndex)
    {
        var effects = new Dictionary<string, Dictionary<effectKey, int>>();
        if (rootSkill == null) return effects;

        var preview = BattleCharacterPreviewFactory.Build(currentParty, currentEnemies);

        previewPartyLookup.Clear();
        foreach (var chr in preview.party)
        {
            if (chr != null)
                previewPartyLookup.Add(chr);
        }

        try
        {
            var caster = GetActivePreviewCaster(preview);
            if (caster == null) return effects;

            BattleCharacter primaryTarget = null;

            if (rootSkill.targetType == SkillTargetType.SingleEnemy ||
                rootSkill.targetType == SkillTargetType.AllEnemies)
            {
                if (casterIsEnemy)
                {
                    if (primaryTargetIndex < 0 || primaryTargetIndex >= preview.party.Count) return effects;
                    primaryTarget = preview.party[primaryTargetIndex];
                }
                else
                {
                    if (primaryTargetIndex < 0 || primaryTargetIndex >= preview.enemies.Count) return effects;
                    primaryTarget = preview.enemies[primaryTargetIndex];
                }
            }
            else if (rootSkill.targetType == SkillTargetType.SingleAlly ||
                    rootSkill.targetType == SkillTargetType.AllAllies)
            {
                if (casterIsEnemy)
                {
                    if (primaryTargetIndex < 0 || primaryTargetIndex >= preview.enemies.Count) return effects;
                    primaryTarget = preview.enemies[primaryTargetIndex];
                }
                else
                {
                    if (primaryTargetIndex < 0 || primaryTargetIndex >= preview.party.Count) return effects;
                    primaryTarget = preview.party[primaryTargetIndex];
                }
            }
            else if (rootSkill.targetType == SkillTargetType.Self)
            {
                primaryTarget = caster;
            }

            var actions = BuildCalculatorActionList(preview.party, preview.enemies, caster, rootSkill, primaryTarget, 0);

            var orderedActions = ActionOrderUtility.GetOrderedActions(actions);

            // Capture initial state before any skill effects
            var before = CaptureSnapshots(preview.party, preview.enemies);

            ExecutePreviewSkill(caster, rootSkill, primaryTarget);

            CollectEffectsFromSnapshots(effects, preview.party, preview.enemies, before);

            return effects;
        }
        finally
        {
            preview.Dispose();
            previewPartyLookup.Clear();
        }
    }

    private void DisableAllLineupButtons()
    {
        SetPartyLineupInteractable(false);
        SetEnemyLineupInteractable(false);
    }

    private void UpdateLineupEffects()
    {
        Debug.Log(string.Join(", ", effectsByTarget.Keys) + " have effects");
        foreach (var entry in partyLineupEntries)
        {
            entry.ClearEffectText();
            string id = $"P{partyLineupEntries.IndexOf(entry)}";
            WriteEffectsToEntry(entry, id);
        }

        foreach (var entry in enemyLineupEntries)
        {
            entry.ClearEffectText();
            string id = $"E{enemyLineupEntries.IndexOf(entry)}";
            WriteEffectsToEntry(entry, id);
        }
    }

    private void WriteEffectsToEntry(PartyLineupEntryUI entry, string id)
    {
        if (!effectsByTarget.ContainsKey(id)) return;
        var targetEffects = effectsByTarget[id];

        if (targetEffects.TryGetValue(effectKey.Damage, out int damage))
            entry.SetEffectDamage($"<color=red>-{damage} HP</color>");

        if (targetEffects.TryGetValue(effectKey.Heal, out int heal))
            entry.SetEffectDamage($"<color=green>+{heal} HP</color>");

        if (targetEffects.TryGetValue(effectKey.SpChange, out int sp) && sp != 0)
            entry.SetEffectDamage(sp > 0 ? $"<color=blue>+{sp} SP</color>" : $"<color=blue>{sp} SP</color>");

        if (targetEffects.TryGetValue(effectKey.AmmoChange, out int ammo) && ammo != 0)
            entry.SetEffectDamage(ammo > 0 ? $"<color=yellow>+{ammo} Ammo</color>" : $"<color=yellow>{ammo} Ammo</color>");
    }

    private void WriteEffectsToEntry(EnemyLineupEntryUI entry, string id)
    {
        if (!effectsByTarget.ContainsKey(id)) return;
        var targetEffects = effectsByTarget[id];

        if (targetEffects.TryGetValue(effectKey.Damage, out int damage))
            entry.SetEffectDamage($"<color=red>-{damage} HP</color>");

        if (targetEffects.TryGetValue(effectKey.Heal, out int heal))
            entry.SetEffectDamage($"<color=green>+{heal} HP</color>");

        if (targetEffects.TryGetValue(effectKey.SpChange, out int sp) && sp != 0)
            entry.SetEffectDamage(sp > 0 ? $"<color=blue>+{sp} SP</color>" : $"<color=blue>{sp} SP</color>");

        if (targetEffects.TryGetValue(effectKey.AmmoChange, out int ammo) && ammo != 0)
            entry.SetEffectDamage(ammo > 0 ? $"<color=yellow>+{ammo} Ammo</color>" : $"<color=yellow>{ammo} Ammo</color>");
    }


    private void UpdateTurnOrder()
    {
        var preview = BattleCharacterPreviewFactory.Build(currentParty, currentEnemies);

        try
        {
            Dictionary<GameObject, int> characterSpeeds = new Dictionary<GameObject, int>();

            for (int i = 0; i < partyEntries.Count; i++)
            {
                if (i < preview.party.Count && partyEntries[i] != null && preview.party[i] != null)
                    characterSpeeds[partyEntries[i].gameObject] = preview.party[i].GetEffectiveStats().speed;
            }

            for (int i = 0; i < enemyEntries.Count; i++)
            {
                if (i < preview.enemies.Count && enemyEntries[i] != null && preview.enemies[i] != null)
                    characterSpeeds[enemyEntries[i].gameObject] = preview.enemies[i].GetEffectiveStats().speed;
            }

            List<GameObject> sortedCharacters = new List<GameObject>(characterSpeeds.Keys);
            sortedCharacters.Sort((a, b) => characterSpeeds[b].CompareTo(characterSpeeds[a]));

            for (int i = 0; i < sortedCharacters.Count; i++)
            {
                var partyEntry = sortedCharacters[i].GetComponent<CalculatorPartyEntryUI>();
                if (partyEntry != null)
                {
                    partyEntry.SetTurnOrder(i + 1);
                    continue;
                }

                var enemyEntry = sortedCharacters[i].GetComponent<CalculatorEnemyEntryUI>();
                if (enemyEntry != null)
                    enemyEntry.SetTurnOrder(i + 1);
            }
        }
        finally
        {
            preview.Dispose();
        }
    }

    private void SetPartyLineupInteractable(bool value)
    {
        for (int i = 0; i < partyLineupEntries.Count; i++)
            if (partyLineupEntries[i] != null) partyLineupEntries[i].SetInteractable(value);
    }

    private void SetEnemyLineupInteractable(bool value)
    {
        for (int i = 0; i < enemyLineupEntries.Count; i++)
            if (enemyLineupEntries[i] != null) enemyLineupEntries[i].SetInteractable(value);
    }

    private BattleCharacter GetActivePreviewCaster(BattleCharacterPreviewFactory.PreviewContext preview)
    {
        var activeEnemy = skillListUI != null ? skillListUI.GetActiveEnemy() : null;
        if (activeEnemy != null)
        {
            int index = currentEnemies.IndexOf(activeEnemy);
            if (index >= 0 && index < preview.enemies.Count)
                return preview.enemies[index];
        }

        var activeParty = skillListUI != null ? skillListUI.GetActivePartyMember() : null;
        if (activeParty != null)
        {
            int index = currentParty.IndexOf(activeParty);
            if (index >= 0 && index < preview.party.Count)
                return preview.party[index];
        }

        return null;
    }

    private Dictionary<BattleCharacter, CharacterSnapshot> CaptureSnapshots(
        List<BattleCharacter> party,
        List<BattleCharacter> enemies)
    {
        var result = new Dictionary<BattleCharacter, CharacterSnapshot>();

        void captureList(List<BattleCharacter> list)
        {
            if (list == null) return;

            foreach (var chr in list)
            {
                if (chr == null) continue;

                result[chr] = new CharacterSnapshot
                {
                    currentHp = chr.CurrentHealth,
                    currentSp = chr.CurrentSp,
                    currentAmmo = chr.CurrentAmmo,
                    effectiveStats = chr.GetEffectiveStats()
                };
            }
        }

        captureList(party);
        captureList(enemies);

        return result;
    }

    private void AddEffect(
        Dictionary<string, Dictionary<effectKey, int>> effects,
        string targetId,
        effectKey key,
        int amount)
    {
        if (string.IsNullOrEmpty(targetId) || amount == 0)
            return;

        if (!effects.ContainsKey(targetId))
            effects[targetId] = new Dictionary<effectKey, int>();

        if (!effects[targetId].ContainsKey(key))
            effects[targetId][key] = 0;

        effects[targetId][key] += amount;
    }

    private void AddStatDiffEffects(
        Dictionary<string, Dictionary<effectKey, int>> effects,
        string targetId,
        CombatStats before,
        CombatStats after)
    {
        AddEffect(effects, targetId, effectKey.MaxHealth, after.maxHealth - before.maxHealth);
        AddEffect(effects, targetId, effectKey.MaxSp, after.maxSp - before.maxSp);
        AddEffect(effects, targetId, effectKey.Speed, after.speed - before.speed);
        AddEffect(effects, targetId, effectKey.PhysicalAttack, after.physicalAttack - before.physicalAttack);
        AddEffect(effects, targetId, effectKey.ElementalPower, after.elementalPower - before.elementalPower);
        AddEffect(effects, targetId, effectKey.Defense, after.defense - before.defense);
        AddEffect(effects, targetId, effectKey.ElementalResistance, after.elementalResistance - before.elementalResistance);
        AddEffect(effects, targetId, effectKey.CritChance, after.critChance - before.critChance);
        AddEffect(effects, targetId, effectKey.CritDamage, after.critDamage - before.critDamage);

        AddEffect(effects, targetId, effectKey.BludgeoningAttack, after.bludgeoningAttack - before.bludgeoningAttack);
        AddEffect(effects, targetId, effectKey.SlashingAttack, after.slashingAttack - before.slashingAttack);
        AddEffect(effects, targetId, effectKey.PiercingAttack, after.piercingAttack - before.piercingAttack);

        AddEffect(effects, targetId, effectKey.BludgeoningDefense, after.bludgeoningDefense - before.bludgeoningDefense);
        AddEffect(effects, targetId, effectKey.SlashingDefense, after.slashingDefense - before.slashingDefense);
        AddEffect(effects, targetId, effectKey.PiercingDefense, after.piercingDefense - before.piercingDefense);

        AddEffect(effects, targetId, effectKey.FireAttack, after.fireAttack - before.fireAttack);
        AddEffect(effects, targetId, effectKey.IceAttack, after.iceAttack - before.iceAttack);
        AddEffect(effects, targetId, effectKey.StormAttack, after.stormAttack - before.stormAttack);
        AddEffect(effects, targetId, effectKey.AcidAttack, after.acidAttack - before.acidAttack);
        AddEffect(effects, targetId, effectKey.PsychicAttack, after.psychicAttack - before.psychicAttack);
        AddEffect(effects, targetId, effectKey.BloodAttack, after.bloodAttack - before.bloodAttack);

        AddEffect(effects, targetId, effectKey.FireDefense, after.fireDefense - before.fireDefense);
        AddEffect(effects, targetId, effectKey.IceDefense, after.iceDefense - before.iceDefense);
        AddEffect(effects, targetId, effectKey.StormDefense, after.stormDefense - before.stormDefense);
        AddEffect(effects, targetId, effectKey.AcidDefense, after.acidDefense - before.acidDefense);
        AddEffect(effects, targetId, effectKey.PsychicDefense, after.psychicDefense - before.psychicDefense);
        AddEffect(effects, targetId, effectKey.BloodDefense, after.bloodDefense - before.bloodDefense);
    }

    private void CollectEffectsFromSnapshots(
        Dictionary<string, Dictionary<effectKey, int>> effects,
        List<BattleCharacter> previewParty,
        List<BattleCharacter> previewEnemies,
        Dictionary<BattleCharacter, CharacterSnapshot> before)
    {
        for (int i = 0; i < previewParty.Count; i++)
        {
            var chr = previewParty[i];
            if (chr == null || !before.ContainsKey(chr)) continue;

            string id = $"P{i}";
            var pre = before[chr];
            var postStats = chr.GetEffectiveStats();

            int hpDelta = chr.CurrentHealth - pre.currentHp;
            if (hpDelta < 0) AddEffect(effects, id, effectKey.Damage, -hpDelta);
            if (hpDelta > 0) AddEffect(effects, id, effectKey.Heal, hpDelta);

            AddEffect(effects, id, effectKey.SpChange, chr.CurrentSp - pre.currentSp);
            AddEffect(effects, id, effectKey.AmmoChange, chr.CurrentAmmo - pre.currentAmmo);

            AddStatDiffEffects(effects, id, pre.effectiveStats, postStats);
        }

        for (int i = 0; i < previewEnemies.Count; i++)
        {
            var chr = previewEnemies[i];
            if (chr == null || !before.ContainsKey(chr)) continue;

            string id = $"E{i}";
            var pre = before[chr];
            var postStats = chr.GetEffectiveStats();

            Debug.Log($"Collecting effects for {id}: HP {pre.currentHp} -> {chr.CurrentHealth}, SP {pre.currentSp} -> {chr.CurrentSp}, Ammo {pre.currentAmmo} -> {chr.CurrentAmmo}");
            int hpDelta = chr.CurrentHealth - pre.currentHp;
            if (hpDelta < 0) AddEffect(effects, id, effectKey.Damage, -hpDelta);
            if (hpDelta > 0) AddEffect(effects, id, effectKey.Heal, hpDelta);

            AddEffect(effects, id, effectKey.SpChange, chr.CurrentSp - pre.currentSp);
            AddEffect(effects, id, effectKey.AmmoChange, chr.CurrentAmmo - pre.currentAmmo);

            AddStatDiffEffects(effects, id, pre.effectiveStats, postStats);
        }
    }
    

    private void ExecutePreviewSkill(BattleCharacter caster, Skill skill, BattleCharacter target)
    {
        if (caster == null || skill == null) return;

        var targets = GetPreviewTargetsForSkill(skill, caster, target);

        

        foreach (var t in targets)
        {
            if (t == null) continue;

            PassiveMutationUtility.InvokePassivesWithMutation(
                            t,
                            () => t.passives,
                            p => p.OnSkillReceived(t, caster, skill),
                            t.passiveMutationContext
                        );
        }

        PassiveMutationUtility.InvokePassivesWithMutation(
            caster,
            () => caster.passives,
            p => p.OnSkillUsed(caster, target, skill),
            caster.passiveMutationContext
        );

        // Match battle SP behavior for party-side casters
        if (currentParty != null && previewPartyLookup.Contains(caster))
        {
            if (!caster.TrySpendSp(skill.spCost))
                return;
            if(!caster.TrySpendHp(skill.hpCost))
                return;
        }

        skill.Execute(caster, target);

        PassiveMutationUtility.InvokePassivesWithMutation(
            caster,
            () => caster.passives,
            p => p.OnSkillUsedEnd(caster, target, skill),
            caster.passiveMutationContext
        );

        foreach (var t in targets)
        {
            if (t == null) continue;

            PassiveMutationUtility.InvokePassivesWithMutation(
                t,
                () => t.passives,
                p => p.OnSkillReceivedEnd(t, caster, skill),
                t.passiveMutationContext
            );
        }
    }

    private List<BattleCharacter> GetPreviewTargetsForSkill(
        Skill skill,
        BattleCharacter caster,
        BattleCharacter target)
    {
        var results = new List<BattleCharacter>();
        if (skill == null || caster == null) return results;

        switch (skill.targetType)
        {
            case SkillTargetType.SingleEnemy:
            case SkillTargetType.SingleAlly:
                if (target != null)
                    results.Add(target);
                break;

            case SkillTargetType.AllEnemies:
                foreach (var c in caster.GetEnemies())
                {
                    if (c != null && !c.IsDead)
                        results.Add(c);
                }
                break;

            case SkillTargetType.AllAllies:
                foreach (var c in caster.GetAllies())
                {
                    if (c != null && !c.IsDead)
                        results.Add(c);
                }
                break;

            case SkillTargetType.Self:
                results.Add(caster);
                break;
        }

        return results;
    }

    private MapPartyMemberDefinition ClonePartyDef(MapPartyMemberDefinition source)
    {
        if (source == null) return null;

        return new MapPartyMemberDefinition
        {
            characterPrefab = source.characterPrefab,
            displayName = source.displayName,
            characterAsset = source.characterAsset,

            overrideTraits = source.overrideTraits,
            overrideStats = source.overrideStats,
            overrideSkills = source.overrideSkills,

            traits = source.traits != null ? new List<TraitDefinition>(source.traits) : new List<TraitDefinition>(),
            traitTypes = source.traitTypes != null ? new List<CharacterTrait>(source.traitTypes) : new List<CharacterTrait>(),

            stats = source.stats,
            skills = source.skills != null ? new List<Skill>(source.skills) : new List<Skill>(),

            level = source.level,
            currentXp = source.currentXp,
            health = source.health,
            sp = source.sp,

            initializedFromAssetTraits = source.initializedFromAssetTraits,
            initializedFromAssetStats = source.initializedFromAssetStats,
            initializedFromAssetSkills = source.initializedFromAssetSkills,

            passives = source.passives != null ? (PassivesDefinition[])source.passives.Clone() : null,

            talentTreePrefab = source.talentTreePrefab,
            talentPoints = source.talentPoints,
            learnedTalentIds = source.learnedTalentIds != null ? new List<string>(source.learnedTalentIds) : new List<string>(),

            mainSubStats = source.mainSubStats != null ? new List<CombatSubStat>(source.mainSubStats) : new List<CombatSubStat>(),
            subSubStats = source.subSubStats != null ? new List<CombatSubStat>(source.subSubStats) : new List<CombatSubStat>(),
            initializedSubStats = source.initializedSubStats
        };
    }

    private MapEnemyDefinition CloneEnemyDef(MapEnemyDefinition source)
    {
        if (source == null) return null;

        return new MapEnemyDefinition
        {
            enemyPrefab = source.enemyPrefab,
            displayName = source.displayName,
            characterAsset = source.characterAsset,

            overrideStats = source.overrideStats,
            overrideSkills = source.overrideSkills,

            stats = source.stats,
            skills = source.skills != null ? new List<Skill>(source.skills) : new List<Skill>(),

            initializedFromAssetStats = source.initializedFromAssetStats,
            initializedFromAssetSkills = source.initializedFromAssetSkills,

            passives = source.passives != null ? (PassivesDefinition[])source.passives.Clone() : null
        };
    }


    private void RefreshCalculatorLists()
    {
        PopulateParty(currentParty);
        PopulateEnemies(currentEnemies);
        PopulatePartyLineup(currentParty);
        PopulateEnemyLineup(currentEnemies);
        UpdateTurnOrder();
        UpdateLineupEffects();
    }

    private void BuildStatEntries(CombatStats stats, Action<CombatStats> onChanged)
    {
        if (statEntryContainer == null || statEntryPrefab == null) return;

        for (int i = statEntryContainer.childCount - 1; i >= 0; i--)
            Destroy(statEntryContainer.GetChild(i).gameObject);

        void addEntry(string label, int value, System.Func<CombatStats, int, CombatStats> apply)
        {
            var entry = Instantiate(statEntryPrefab, statEntryContainer);
            entry.Setup(label, value, newValue =>
            {
                stats = apply(stats, newValue);
                onChanged?.Invoke(stats);
            });
        }

        addEntry("HP", stats.maxHealth, (s, v) => { s.maxHealth = v; return s; });
        addEntry("SP", stats.maxSp, (s, v) => { s.maxSp = v; return s; });
        addEntry("Speed", stats.speed, (s, v) => { s.speed = v; return s; });
        addEntry("Physical Attack", stats.physicalAttack, (s, v) => { s.physicalAttack = v; return s; });
        addEntry("Elemental Power", stats.elementalPower, (s, v) => { s.elementalPower = v; return s; });
        addEntry("Defense", stats.defense, (s, v) => { s.defense = v; return s; });
        addEntry("Elemental Resistance", stats.elementalResistance, (s, v) => { s.elementalResistance = v; return s; });
        addEntry("Crit Chance", stats.critChance, (s, v) => { s.critChance = v; return s; });
        addEntry("Crit Damage", stats.critDamage, (s, v) => { s.critDamage = v; return s; });

        addEntry("Bludgeoning Attack", stats.bludgeoningAttack, (s, v) => { s.bludgeoningAttack = v; return s; });
        addEntry("Slashing Attack", stats.slashingAttack, (s, v) => { s.slashingAttack = v; return s; });
        addEntry("Piercing Attack", stats.piercingAttack, (s, v) => { s.piercingAttack = v; return s; });

        addEntry("Bludgeoning Defense", stats.bludgeoningDefense, (s, v) => { s.bludgeoningDefense = v; return s; });
        addEntry("Slashing Defense", stats.slashingDefense, (s, v) => { s.slashingDefense = v; return s; });
        addEntry("Piercing Defense", stats.piercingDefense, (s, v) => { s.piercingDefense = v; return s; });

        addEntry("Fire Attack", stats.fireAttack, (s, v) => { s.fireAttack = v; return s; });
        addEntry("Ice Attack", stats.iceAttack, (s, v) => { s.iceAttack = v; return s; });
        addEntry("Storm Attack", stats.stormAttack, (s, v) => { s.stormAttack = v; return s; });
        addEntry("Acid Attack", stats.acidAttack, (s, v) => { s.acidAttack = v; return s; });
        addEntry("Psychic Attack", stats.psychicAttack, (s, v) => { s.psychicAttack = v; return s; });
        addEntry("Blood Attack", stats.bloodAttack, (s, v) => { s.bloodAttack = v; return s; });

        addEntry("Fire Defense", stats.fireDefense, (s, v) => { s.fireDefense = v; return s; });
        addEntry("Ice Defense", stats.iceDefense, (s, v) => { s.iceDefense = v; return s; });
        addEntry("Storm Defense", stats.stormDefense, (s, v) => { s.stormDefense = v; return s; });
        addEntry("Acid Defense", stats.acidDefense, (s, v) => { s.acidDefense = v; return s; });
        addEntry("Psychic Defense", stats.psychicDefense, (s, v) => { s.psychicDefense = v; return s; });
        addEntry("Blood Defense", stats.bloodDefense, (s, v) => { s.bloodDefense = v; return s; });
    }

    public void CloseOverlayScreens()
    {
        if (skillListContainer != null)
            skillListContainer.SetActive(false);

        if (statScreenRoot != null)
            statScreenRoot.SetActive(false);
    }

    public void OpenPartySkillScreen(MapPartyMemberDefinition def)
    {
        if (def == null || skillListUI == null || skillListContainer == null) return;

        CloseOverlayScreens();

        skillListUI.SetActiveCharacter(def);
        skillListUI.UpdateSkillList(def.skills, true);
        skillListContainer.SetActive(true);
    }

    public void OpenEnemySkillScreen(MapEnemyDefinition def)
    {
        if (def == null || skillListUI == null || skillListContainer == null) return;

        CloseOverlayScreens();

        skillListUI.SetActiveCharacter(def);
        skillListUI.UpdateSkillList(def.skills, false);
        skillListContainer.SetActive(true);
    }

    public void OpenPartyStatScreen(MapPartyMemberDefinition def)
    {
        if (def == null) return;

        CloseOverlayScreens();

        BuildStatEntries(def.stats, updatedStats =>
        {
            def.stats = updatedStats;
            RefreshCalculatorLists();
        });

        if (statScreenRoot != null)
            statScreenRoot.SetActive(true);
    }

    public void OpenEnemyStatScreen(MapEnemyDefinition def)
    {
        if (def == null) return;

        CloseOverlayScreens();

        BuildStatEntries(def.stats, updatedStats =>
        {
            def.stats = updatedStats;
            RefreshCalculatorLists();
        });

        if (statScreenRoot != null)
            statScreenRoot.SetActive(true);
    }

    private List<QueuedAction> BuildCalculatorActionList(
        List<BattleCharacter> party,
        List<BattleCharacter> enemies,
        BattleCharacter actingCharacter,
        Skill chosenSkill,
        BattleCharacter chosenTarget,
        int chosenSkillIndex)
    {
        var actions = new List<QueuedAction>();

        void addGroup(List<BattleCharacter> group)
        {
            if (group == null) return;

            foreach (var chr in group)
            {
                if (chr == null || chr.IsDead) continue;

                if (chr == actingCharacter)
                {
                    actions.Add(new QueuedAction
                    {
                        kind = ActionKind.Skill,
                        user = chr,
                        skill = chosenSkill,
                        skillIndex = chosenSkillIndex,
                        target = chosenTarget
                    });
                }
                else
                {
                    actions.Add(new QueuedAction
                    {
                        kind = ActionKind.Skill,
                        user = chr,
                        skill = null,
                        skillIndex = -1,
                        target = null
                    });
                }
            }
        }

        addGroup(party);
        addGroup(enemies);

        return actions;
    }
}
