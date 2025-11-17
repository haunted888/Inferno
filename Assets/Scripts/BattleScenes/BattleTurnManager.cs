using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine.Events;
using UnityEngine;


public class BattleTurnManager : MonoBehaviour
{

    public static BattleTurnManager Instance { get; private set; }

    private int turnIndex = 0;


    [Header("Participants")]
    public Transform playerPartyParent;  // assign in inspector
    public Transform enemyPartyParent;   // assign in inspector

    private List<BattleCharacter> playerParty = new List<BattleCharacter>();
    private List<BattleCharacter> enemyParty  = new List<BattleCharacter>();

    [Header("UI")]
    public SkillSelectionUI skillSelectionUI;
    public BattleCommandUI commandUI;   // NEW

    private List<BattleCharacter> commandOrder = new List<BattleCharacter>(); // NEW


    private enum TurnState { Idle, CommandSelect, ActionResolve }
    #pragma warning disable 0414
    private TurnState state = TurnState.Idle;
    #pragma warning restore 0414

    private int currentPlayerIndex = 0;

    // chosen skill index per player character
    private Dictionary<BattleCharacter, int> chosenSkillIndices = new Dictionary<BattleCharacter, int>();
    private Dictionary<BattleCharacter, BattleCharacter> chosenTargets = new Dictionary<BattleCharacter, BattleCharacter>();


    private class QueuedAction
    {
        public BattleCharacter user;
        public Skill skill;
        public BattleCharacter target;
    }

    void Awake()
    {

        Instance = this;
    }

    void Start()
    {
        
        // Get all slot children in order
        playerParty = playerPartyParent.GetComponentsInChildren<BattleCharacter>(false).ToList();
        enemyParty  = enemyPartyParent.GetComponentsInChildren<BattleCharacter>(false).ToList();
        Debug.Log($"BattleTurnManager: Found {playerParty.Count} player characters and {enemyParty.Count} enemies.");
        Debug.Log("Enemies: " + enemyParty);
        StartCoroutine(TurnLoop());
    }

    public void setEnemyParty(List<BattleCharacter> enemies)
    {
        enemyParty = enemies;
    }

    private IEnumerator TurnLoop()
    {
        while (true)
        {
            state = TurnState.CommandSelect;
            chosenSkillIndices.Clear();
            chosenTargets.Clear();
            currentPlayerIndex = 0;

            yield return StartCoroutine(CommandSelectionPhase());

            state = TurnState.ActionResolve;
            yield return StartCoroutine(ActionResolutionPhase());

            turnIndex++;   // advance global turn counter

            yield return null;
        }
    }

    private IEnumerator CommandSelectionPhase()
    {
        chosenSkillIndices.Clear();
        chosenTargets.Clear();
        commandOrder.Clear();
        currentPlayerIndex = 0;

        while (currentPlayerIndex < playerParty.Count)
        {
            BattleCharacter chr = playerParty[currentPlayerIndex];

            // Skip invalid characters
            if (chr == null || chr.IsDead || chr.Skills.Count == 0)
            {
                currentPlayerIndex++;
                continue;
            }

            bool hasPrevious = commandOrder.Count > 0;

            // 1) Main command menu
            bool waitingForCommand = true;
            BattleCommandType chosenCommand = BattleCommandType.Skills;

            commandUI.ShowForCharacter(chr, hasPrevious, (cmd) =>
            {
                chosenCommand = cmd;
                waitingForCommand = false;
            });

            while (waitingForCommand)
                yield return null;

            // Handle command
            if (chosenCommand == BattleCommandType.Items)
            {
                // For now: do nothing, stay on same character
                Debug.Log("Items not implemented yet.");
                continue;
            }

            if (chosenCommand == BattleCommandType.Skip)
            {
                // Mark as skip: no skill, no target
                chosenSkillIndices[chr] = -1;
                chosenTargets[chr] = null;

                if (!commandOrder.Contains(chr))
                    commandOrder.Add(chr);

                currentPlayerIndex++;
                continue;
            }

            if (chosenCommand == BattleCommandType.Back)
            {
                if (hasPrevious)
                {
                    var lastChar = commandOrder[commandOrder.Count - 1];
                    commandOrder.RemoveAt(commandOrder.Count - 1);
                    chosenSkillIndices.Remove(lastChar);
                    chosenTargets.Remove(lastChar);

                    int idx = playerParty.IndexOf(lastChar);
                    currentPlayerIndex = Mathf.Max(0, idx);
                }
                continue;
            }

            // If we got here, the command is Skills

            // Skills path: command menu stays active, skills can be cancelled by other commands
            bool decisionMade = false;
            bool skillChosen = false;
            int chosenIndex = -1;
            BattleCommandType secondaryCommand = BattleCommandType.Skills;

            // Show BOTH UIs
            commandUI.ShowForCharacter(chr, hasPrevious, cmd =>
            {
                secondaryCommand = cmd;
                decisionMade = true;
            });
            skillSelectionUI.ShowForCharacter(chr, skillIndex =>
            {
                chosenIndex = skillIndex;
                skillChosen = true;
                decisionMade = true;
            });

            // Wait until either a skill or another command is chosen
            while (!decisionMade)
                yield return null;

            skillSelectionUI.Hide();

            // If a skill was picked, proceed to target selection
            if (skillChosen)
            {
                Skill chosenSkill = chr.Skills[chosenIndex];

                BattleCharacter chosenTarget = null;
                bool waitingForTarget = true;
                UnityAction<BattleCharacter> handler = null;

                handler = (clicked) =>
                {
                    if (!IsTargetValidForSkill(chosenSkill, chr, clicked))
                        return;

                    chosenTarget = clicked;
                    waitingForTarget = false;
                    ClickManagerBattle.OnCharacterClicked.RemoveListener(handler);
                };

                ClickManagerBattle.OnCharacterClicked.AddListener(handler);

                while (waitingForTarget)
                    yield return null;

                chosenSkillIndices[chr] = chosenIndex;
                chosenTargets[chr] = chosenTarget;

                if (!commandOrder.Contains(chr))
                    commandOrder.Add(chr);

                currentPlayerIndex++;
                continue;
            }

            // Otherwise, a non-Skills command was pressed while the skills UI was open

            if (secondaryCommand == BattleCommandType.Items)
            {
                Debug.Log("Items not implemented yet.");
                // Stay on the same character so they can pick again
                continue;
            }

            if (secondaryCommand == BattleCommandType.Skip)
            {
                chosenSkillIndices[chr] = -1;
                chosenTargets[chr] = null;

                if (!commandOrder.Contains(chr))
                    commandOrder.Add(chr);

                currentPlayerIndex++;
                continue;
            }

            if (secondaryCommand == BattleCommandType.Back)
            {
                if (hasPrevious)
                {
                    var lastChar = commandOrder[commandOrder.Count - 1];
                    commandOrder.RemoveAt(commandOrder.Count - 1);
                    chosenSkillIndices.Remove(lastChar);
                    chosenTargets.Remove(lastChar);

                    int idx = playerParty.IndexOf(lastChar);
                    currentPlayerIndex = Mathf.Max(0, idx);
                }
                continue;
            }

        }

        commandUI.Hide();
        skillSelectionUI.Hide();
        GenerateEnemyCommands();
    }

    private IEnumerator ActionResolutionPhase()
    {
        List<QueuedAction> actions = new List<QueuedAction>();

        foreach (var kvp in chosenSkillIndices)
        {
            var user = kvp.Key;
            int skillIndex = kvp.Value;

            if (user == null || user.IsDead) continue;
            if (skillIndex < 0 || skillIndex >= user.Skills.Count) continue;

            Skill skill = user.Skills[skillIndex];
            chosenTargets.TryGetValue(user, out BattleCharacter target);

            actions.Add(new QueuedAction
            {
                user = user,
                skill = skill,
                target = target
            });
        }

        // Sort by speed (descending)
        actions = actions
            .OrderByDescending(a => a.user.Speed)
            .ThenBy(_ => Random.value)
            .ToList();

        // Execute in order
        foreach (var action in actions)
        {
            // Check if either side is already wiped; end battle immediately
            if (IsSideDefeated(playerParty) || IsSideDefeated(enemyParty))
            {
                Debug.Log("Battle ended (one side defeated).");
                yield break;
            }

            if (action.user == null || action.user.IsDead) continue;
            if (action.skill == null) continue;

            // Resolve target (may retarget if original target died)
            BattleCharacter effectiveTarget = ResolveEffectiveTarget(action);

            // For single-target skills, if we have no valid target, skip the action
            if ((action.skill.targetType == SkillTargetType.SingleEnemy ||
                action.skill.targetType == SkillTargetType.SingleAlly) &&
                effectiveTarget == null)
            {
                continue;
            }

            action.skill.Execute(action.user, effectiveTarget);

            // After executing, check again if a side is wiped
            if (IsSideDefeated(playerParty) || IsSideDefeated(enemyParty))
            {
                Debug.Log("Battle ended (one side defeated).");
                yield break;
            }

            yield return new WaitForSeconds(0.3f);
        }

    }


    private BattleCharacter GetFirstAliveEnemy()
    {
        foreach (var e in enemyParty)
        {
            if (e != null && !e.IsDead)
                return e;
        }
        return null;
    }
    public IEnumerable<BattleCharacter> GetAlliesOf(BattleCharacter c)
    {
        if (playerParty.Contains(c)) return playerParty;
        if (enemyParty.Contains(c))  return enemyParty;
        return new List<BattleCharacter>();
    }

    public IEnumerable<BattleCharacter> GetEnemiesOf(BattleCharacter c)
    {
        if (playerParty.Contains(c)) return enemyParty;
        if (enemyParty.Contains(c))  return playerParty;
        return new List<BattleCharacter>();
    }

    private bool IsTargetValidForSkill(Skill skill, BattleCharacter user, BattleCharacter clicked)
    {
        if (skill == null || user == null || clicked == null) return false;
        if (clicked.IsDead) return false;

        bool clickedIsAlly  = GetAlliesOf(user).Contains(clicked);
        bool clickedIsEnemy = GetEnemiesOf(user).Contains(clicked);

        switch (skill.targetType)
        {
            case SkillTargetType.SingleEnemy:
            case SkillTargetType.AllEnemies:
                return clickedIsEnemy;

            case SkillTargetType.SingleAlly:
            case SkillTargetType.AllAllies:
                return clickedIsAlly;

            default:
                return false;
        }
    }

    public void HandleCharacterDeath(BattleCharacter c)
    {
        if (c == null) return;

        playerParty.Remove(c);
        enemyParty.Remove(c);

        Destroy(c.gameObject);
    }

    private bool IsSideDefeated(IEnumerable<BattleCharacter> group)
    {
        foreach (var c in group)
        {
            if (c != null && !c.IsDead)
                return false;
        }
        return true;
    }

    private BattleCharacter ResolveEffectiveTarget(QueuedAction action)
    {
        if (action == null || action.skill == null || action.user == null)
            return null;

        var type = action.skill.targetType;

        // AoE skills don't care about the specific clicked target
        if (type == SkillTargetType.AllEnemies || type == SkillTargetType.AllAllies)
            return action.target;

        // Single-target skills: retarget if original is dead or invalid
        bool isAllyTarget =
            type == SkillTargetType.SingleAlly;

        var pool = isAllyTarget
            ? GetAlliesOf(action.user)
            : GetEnemiesOf(action.user);

        // build list of alive candidates
        var candidates = new List<BattleCharacter>();
        foreach (var c in pool)
        {
            if (c != null && !c.IsDead)
                candidates.Add(c);
        }

        if (candidates.Count == 0)
            return null;

        // If original target is still a valid alive candidate, keep it
        if (action.target != null && !action.target.IsDead && candidates.Contains(action.target))
            return action.target;

        // Otherwise pick a random other of same affiliation
        int idx = Random.Range(0, candidates.Count);
        return candidates[idx];
    }
    public void RegisterDamage(BattleCharacter source, BattleCharacter target, int amount)
    {
        if (source == null || amount <= 0) return;

        // Only players generate threat currently
        if (playerParty.Contains(source))
            source.AddThreat(amount);
    }
    private void GenerateEnemyCommands()
    {
        foreach (var enemy in enemyParty)
        {
            if (enemy == null || enemy.IsDead || enemy.Skills.Count == 0)
                continue;

            EvaluateEnemyAction(enemy, out int skillIndex, out BattleCharacter target);
            if (skillIndex < 0) continue;

            Skill skill = enemy.Skills[skillIndex];
            if (skill == null) continue;

            // For single-target skills, require a concrete target
            if ((skill.targetType == SkillTargetType.SingleEnemy ||
                skill.targetType == SkillTargetType.SingleAlly) &&
                (target == null || target.IsDead))
            {
                continue;
            }

            chosenSkillIndices[enemy] = skillIndex;
            chosenTargets[enemy] = target;
        }
    }
    private void EvaluateEnemyAction(BattleCharacter enemy, out int bestSkillIndex, out BattleCharacter bestTarget)
    {
        bestSkillIndex = -1;
        bestTarget = null;

        var skills = enemy.Skills;
        if (skills == null || skills.Count == 0) return;

        int focusIndex = turnIndex % skills.Count; // rotating +3 per turn

        int globalBestValue   = int.MinValue;
        int globalBestThreat  = int.MinValue;

        // Randomized order of skills to remove positional bias
        var skillOrder = Enumerable.Range(0, skills.Count).ToList();
        Shuffle(skillOrder);

        foreach (int i in skillOrder)
        {
            Skill skill = skills[i];
            if (skill == null) continue;

            int baseValue = 0;
            if (i == focusIndex)
                baseValue += 3;

            IEnumerable<BattleCharacter> candidatesEnum;
            switch (skill.targetType)
            {
                case SkillTargetType.SingleEnemy:
                case SkillTargetType.AllEnemies:
                    candidatesEnum = GetEnemiesOf(enemy);
                    break;

                case SkillTargetType.SingleAlly:
                case SkillTargetType.AllAllies:
                    candidatesEnum = GetAlliesOf(enemy);
                    break;

                default:
                    continue;
            }

            var candidates = new List<BattleCharacter>();
            foreach (var c in candidatesEnum)
            {
                if (c != null && !c.IsDead)
                    candidates.Add(c);
            }

            if (candidates.Count == 0)
                continue;

            // Randomize candidate order to remove target bias on ties
            Shuffle(candidates);

            int skillBestValue  = int.MinValue;
            int skillBestThreat = int.MinValue;
            BattleCharacter skillBestTarget = null;

            foreach (var target in candidates)
            {
                int effectiveThreat = target.Threat;
                int value = baseValue;

                int estDamage = skill.EstimateDamage(enemy, target);

                if (estDamage > 0 && estDamage >= target.CurrentHealth)
                {
                    bool isAoE = skill.targetType == SkillTargetType.AllEnemies ||
                                skill.targetType == SkillTargetType.AllAllies;

                    value += isAoE ? 15 : 10;
                    effectiveThreat += 100000; // temporary kill bias
                }

                if (value > skillBestValue ||
                (value == skillBestValue && effectiveThreat > skillBestThreat))
                {
                    skillBestValue  = value;
                    skillBestThreat = effectiveThreat;
                    skillBestTarget = target;
                }
            }

            if (skillBestTarget == null)
                continue;

            if (skillBestValue > globalBestValue ||
            (skillBestValue == globalBestValue && skillBestThreat > globalBestThreat))
            {
                globalBestValue  = skillBestValue;
                globalBestThreat = skillBestThreat;
                bestSkillIndex   = i;
                bestTarget       = skillBestTarget;
            }
        }
    }


    private void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

}
