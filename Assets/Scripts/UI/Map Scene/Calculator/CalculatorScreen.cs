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

    public Dictionary<string, Dictionary<targetEffectType, int>> effectsByTarget = new Dictionary<string, Dictionary<targetEffectType, int>>();


    private readonly List<CalculatorPartyEntryUI> partyEntries = new List<CalculatorPartyEntryUI>();
    private readonly List<CalculatorEnemyEntryUI> enemyEntries = new List<CalculatorEnemyEntryUI>();
    private readonly List<PartyLineupEntryUI> partyLineupEntries = new List<PartyLineupEntryUI>();
    private readonly List<EnemyLineupEntryUI> enemyLineupEntries = new List<EnemyLineupEntryUI>();


    public enum targetEffectType
    {
        Damage,
        Heal,
        Buff,
        Debuff
    }

    void Awake()
    {
        if (skillListUI != null)
            skillListUI.onSkillSelected += HandleSkillSelected;

        gameObject.SetActive(false);
    }

    void Update()
    {
        if (!gameObject.activeSelf) return;
        if(InputSystem.actions.FindAction("exit").WasPressedThisFrame())        {
            gameObject.SetActive(false);
        }
    }

    public void Open(List<MapPartyMemberDefinition> party, List<MapEnemyDefinition> enemies)
    {
        Debug.Log("Opening Calculator");
        gameObject.SetActive(true);

        skillListContainer.SetActive(false);

        PopulateParty(party);
        PopulateEnemies(enemies);

        PopulatePartyLineup(party);
        PopulateEnemyLineup(enemies);

        currentParty = party;
        currentEnemies = enemies;

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
            row.SetSkillList(skillListUI, skillListContainer);
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
            row.SetSkillList(skillListUI, skillListContainer);
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

        if (skillListContainer != null && skillListContainer.gameObject.activeSelf)
            skillListContainer.gameObject.SetActive(false);

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
    private Dictionary<string, Dictionary<targetEffectType, int>> EstimateEffectsIncludingFollowUps(
        Skill rootSkill,
        bool casterIsEnemy,
        int primaryTargetIndex)
    {
        var effects = new Dictionary<string, Dictionary<targetEffectType, int>>();

        if (rootSkill == null)
            return effects;

        CombatStats casterStats;

        if (casterIsEnemy)
        {
            var casterEnemy = skillListUI != null ? skillListUI.GetActiveEnemy() : null;
            if (casterEnemy == null) return effects;

            casterEnemy.EnsureInitializedFromAsset();
            casterStats = casterEnemy.GetEffectiveStats();
        }
        else
        {
            var casterParty = skillListUI != null ? skillListUI.GetActivePartyMember() : null;
            if (casterParty == null) return effects;

            casterParty.EnsureInitializedFromAsset();
            casterStats = casterParty.GetEffectiveStats();
        }

        ApplySkillEffectsToDictionary(rootSkill, casterStats, casterIsEnemy, primaryTargetIndex, effects);

        if (rootSkill.followUpSkills != null)
        {
            for (int i = 0; i < rootSkill.followUpSkills.Length; i++)
            {
                var followUp = rootSkill.followUpSkills[i];
                if (followUp == null) continue;

                ApplySkillEffectsToDictionary(followUp, casterStats, casterIsEnemy, primaryTargetIndex, effects);
            }
        }

        return effects;
    }

    private void ApplySkillEffectsToDictionary(
        Skill skill,
        CombatStats casterStats,
        bool casterIsEnemy,
        int primaryTargetIndex,
        Dictionary<string, Dictionary<targetEffectType, int>> effects)
    {
        if (skill == null) return;

        // Single-target damage
        if (skill is DamageSkill singleDamageSkill)
        {
            if (skill.targetType != SkillTargetType.SingleEnemy)
                return;

            if (casterIsEnemy)
            {
                if (currentParty == null || primaryTargetIndex < 0 || primaryTargetIndex >= currentParty.Count)
                    return;

                var target = currentParty[primaryTargetIndex];
                if (target == null) return;

                target.EnsureInitializedFromAsset();
                int damage = singleDamageSkill.EstimateDamage(casterStats, target.GetEffectiveStats());
                AddEffect(effects, $"P{primaryTargetIndex}", targetEffectType.Damage, damage);
            }
            else
            {
                if (currentEnemies == null || primaryTargetIndex < 0 || primaryTargetIndex >= currentEnemies.Count)
                    return;

                var target = currentEnemies[primaryTargetIndex];
                if (target == null) return;

                target.EnsureInitializedFromAsset();
                int damage = singleDamageSkill.EstimateDamage(casterStats, target.GetEffectiveStats());
                AddEffect(effects, $"E{primaryTargetIndex}", targetEffectType.Damage, damage);
            }

            return;
        }

        // All-enemies damage
        if (skill is DamageAllEnemiesSkill aoeDamageSkill)
        {
            if (skill.targetType != SkillTargetType.AllEnemies)
                return;

            if (casterIsEnemy)
            {
                if (currentParty == null) return;

                for (int i = 0; i < currentParty.Count; i++)
                {
                    var target = currentParty[i];
                    if (target == null) continue;

                    target.EnsureInitializedFromAsset();
                    int damage = aoeDamageSkill.EstimateDamage(casterStats, target.GetEffectiveStats());
                    AddEffect(effects, $"P{i}", targetEffectType.Damage, damage);
                }
            }
            else
            {
                if (currentEnemies == null) return;

                for (int i = 0; i < currentEnemies.Count; i++)
                {
                    var target = currentEnemies[i];
                    if (target == null) continue;

                    target.EnsureInitializedFromAsset();
                    int damage = aoeDamageSkill.EstimateDamage(casterStats, target.GetEffectiveStats());
                    AddEffect(effects, $"E{i}", targetEffectType.Damage, damage);
                }
            }
        }
    }

    private void AddEffect(
        Dictionary<string, Dictionary<targetEffectType, int>> effects,
        string targetId,
        targetEffectType effectType,
        int delta)
    {
        if (string.IsNullOrEmpty(targetId)) return;

        if (!effects.ContainsKey(targetId))
            effects[targetId] = new Dictionary<targetEffectType, int>();

        if (!effects[targetId].ContainsKey(effectType))
            effects[targetId][effectType] = 0;

        effects[targetId][effectType] += delta;
    }

    private void DisableAllLineupButtons()
    {
        SetPartyLineupInteractable(false);
        SetEnemyLineupInteractable(false);
    }

    private void UpdateLineupEffects()
    {
        foreach(var entry in partyLineupEntries)
        {
            entry.ClearEffectText();
            String id = $"P{partyLineupEntries.IndexOf(entry)}";
            if (effectsByTarget.ContainsKey(id))
            {
                var targetEffects = effectsByTarget[id];
                if (targetEffects.ContainsKey(targetEffectType.Damage))
                {
                    entry.SetEffectDamage($"<color=red> -{targetEffects[targetEffectType.Damage]}</color>");
                }
            }
        }
        
        foreach(var entry in enemyLineupEntries)
        {
            entry.ClearEffectText();
            String id = $"E{enemyLineupEntries.IndexOf(entry)}";
            if (effectsByTarget.ContainsKey(id))
            {
                var targetEffects = effectsByTarget[id];
                if (targetEffects.ContainsKey(targetEffectType.Damage))
                {
                    entry.SetEffectDamage($"<color=red> -{targetEffects[targetEffectType.Damage]}</color>");
                }
            }
        }
        
    }

    private void UpdateTurnOrder()
    {
        Dictionary<GameObject, int> characterSpeeds = new Dictionary<GameObject, int>();

        foreach(var entry in partyEntries)
        {
            int index = partyEntries.IndexOf(entry);
            if (index >= 0 && index < currentParty.Count)
            {
                var def = currentParty[index];
                if (def != null)
                {
                    characterSpeeds[entry.gameObject] = def.stats.speed;
                }
            }
        }

        foreach(var entry in enemyEntries)
        {
            int index = enemyEntries.IndexOf(entry);
            if (index >= 0 && index < currentEnemies.Count)
            {
                var def = currentEnemies[index];
                if (def != null)
                {
                    characterSpeeds[entry.gameObject] = def.stats.speed;
                }
            }
        }

        List<GameObject> sortedCharacters = new List<GameObject>(characterSpeeds.Keys);
        sortedCharacters.Sort((a, b) => characterSpeeds[b].CompareTo(characterSpeeds[a]));
        for (int i = 0; i < sortedCharacters.Count; i++)
        {
            var entry = sortedCharacters[i].GetComponent<CalculatorPartyEntryUI>();
            if (entry != null)
            {
                entry.SetTurnOrder(i + 1);
                continue;
            }

            var enemyEntry = sortedCharacters[i].GetComponent<CalculatorEnemyEntryUI>();
            if (enemyEntry != null)
            {
                enemyEntry.SetTurnOrder(i + 1);
                continue;
            }
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
}
