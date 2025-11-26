using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class BattleTurnManager : MonoBehaviour
{
    public static BattleTurnManager Instance { get; private set; }

    private int turnIndex = 0;

    [Header("Participants")]
    public Transform playerPartyParent;   // assign in inspector
    public Transform enemyPartyParent;    // assign in inspector

    private List<BattleCharacter> playerParty = new List<BattleCharacter>();
    private List<BattleCharacter> enemyParty  = new List<BattleCharacter>();

    [Header("UI")]
    public SkillSelectionUI      skillSelectionUI;
    public BattleItemSelectionUI itemSelectionUI;
    public BattleCommandUI       commandUI;

    public IReadOnlyList<BattleCharacter> PlayerParty => playerParty;

    private List<BattleCharacter> commandOrder = new List<BattleCharacter>();

    private enum TurnState { Idle, CommandSelect, ActionResolve }
#pragma warning disable 0414
    private TurnState state = TurnState.Idle;
#pragma warning restore 0414

    private int currentPlayerIndex = 0;

    // Player choices
    private Dictionary<BattleCharacter, int>             chosenSkillIndices = new Dictionary<BattleCharacter, int>();
    private Dictionary<BattleCharacter, BattleCharacter> chosenTargets      = new Dictionary<BattleCharacter, BattleCharacter>();
    private Dictionary<BattleCharacter, ItemDefinition>  chosenItems        = new Dictionary<BattleCharacter, ItemDefinition>();

    private void EnsureCommandOrder(BattleCharacter chr)
    {
        if (chr != null && !commandOrder.Contains(chr))
            commandOrder.Add(chr);
    }

    private void QueueSkip(BattleCharacter chr)
    {
        chosenSkillIndices[chr] = -1;
        chosenTargets[chr]      = null;
        EnsureCommandOrder(chr);
        currentPlayerIndex++;
    }

    private void QueueSkill(BattleCharacter chr, int skillIndex, BattleCharacter target)
    {
        chosenSkillIndices[chr] = skillIndex;
        chosenTargets[chr]      = target;
        EnsureCommandOrder(chr);
        currentPlayerIndex++;
    }

    private void QueueItem(BattleCharacter chr, ItemDefinition item, BattleCharacter target)
    {
        chosenItems[chr]   = item;
        chosenTargets[chr] = target;
        EnsureCommandOrder(chr);
        currentPlayerIndex++;
    }

    private bool TryStepBack(bool hasPrevious)
    {
        if (!hasPrevious || commandOrder.Count == 0)
            return false;

        var lastChar = commandOrder[commandOrder.Count - 1];
        commandOrder.RemoveAt(commandOrder.Count - 1);
        chosenSkillIndices.Remove(lastChar);
        chosenTargets.Remove(lastChar);
        chosenItems.Remove(lastChar);

        int idx = playerParty.IndexOf(lastChar);
        currentPlayerIndex = Mathf.Max(0, idx);
        return true;
    }

    private bool HandleCommonCommand(BattleCommandType command, BattleCharacter chr, bool hasPrevious)
    {
        switch (command)
        {
            case BattleCommandType.Skip:
                QueueSkip(chr);
                return true;
            case BattleCommandType.Back:
                TryStepBack(hasPrevious);
                return true;
            default:
                return false;
        }
    }

    // Action queue
    private enum ActionKind { Skill, Item }
    private class QueuedAction
    {
        public ActionKind       kind;
        public BattleCharacter  user;

        // Skill
        public int   skillIndex;
        public Skill skill;

        // Item
        public ItemDefinition item;

        // Target (may be null; resolved later for skills; self for self-use items)
        public BattleCharacter target;
    }

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        playerParty = playerPartyParent.GetComponentsInChildren<BattleCharacter>(false).ToList();
        enemyParty  = enemyPartyParent.GetComponentsInChildren<BattleCharacter>(false).ToList();
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
            chosenItems.Clear();
            commandOrder.Clear();
            currentPlayerIndex = 0;

            itemSelectionUI.Hide();
            skillSelectionUI.Hide();

            yield return StartCoroutine(CommandSelectionPhase());

            state = TurnState.ActionResolve;
            yield return StartCoroutine(ActionResolutionPhase());

            turnIndex++;
            yield return null;
        }
    }

    private IEnumerator CommandSelectionPhase()
    {
        while (currentPlayerIndex < playerParty.Count)
        {
            BattleCharacter chr = playerParty[currentPlayerIndex];

            // Skip invalid characters
            if (chr == null || chr.IsDead)
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

            // Handle Items command (with secondary commands)
            if (chosenCommand == BattleCommandType.Items)
            {
                yield return StartCoroutine(ItemsFlow(chr, hasPrevious));
                // ItemsFlow adjusts dictionaries and currentPlayerIndex as needed.
                // Stay in the outer loop to either advance or retry on same character.
                continue;
            }

            if (HandleCommonCommand(chosenCommand, chr, hasPrevious))
                continue;

            // 2) Skills flow (with secondary commands)
            if (chosenCommand == BattleCommandType.Skills)
            {
                bool decisionMade  = false;
                bool skillChosen   = false;
                int  chosenIndex   = -1;
                BattleCommandType secondaryCommand = BattleCommandType.Skills;

                commandUI.ShowForCharacter(chr, hasPrevious, cmd =>
                {
                    secondaryCommand = cmd;
                    decisionMade     = true;
                });
                skillSelectionUI.ShowForCharacter(chr, skillIndex =>
                {
                    chosenIndex = skillIndex;
                    skillChosen = true;
                    decisionMade = true;
                });

                while (!decisionMade)
                    yield return null;

                skillSelectionUI.Hide();

                if (skillChosen)
                {
                    Skill chosenSkill = chr.Skills[chosenIndex];

                    BattleCharacter chosenTarget = null;
                    bool waitingForTarget        = true;
                    bool cancelToCommand         = false;
                    BattleCommandType cmdAfterSkill = BattleCommandType.Skills;

                    commandUI.ShowForCharacter(chr, hasPrevious, cmd =>
                    {
                        cmdAfterSkill   = cmd;
                        cancelToCommand = true;
                    });

                    UnityAction<BattleCharacter> handler = null;
                    handler = (clicked) =>
                    {
                        if (!IsTargetValidForSkill(chosenSkill, chr, clicked))
                            return;
                        if (cancelToCommand)
                            return;

                        chosenTarget     = clicked;
                        waitingForTarget = false;
                    };

                    ClickManagerBattle.OnCharacterClicked.AddListener(handler);

                    while (waitingForTarget && !cancelToCommand)
                        yield return null;

                    ClickManagerBattle.OnCharacterClicked.RemoveListener(handler);

                    if (cancelToCommand)
                    {
                        if (cmdAfterSkill == BattleCommandType.Items)
                        {
                            yield return StartCoroutine(ItemsFlow(chr, hasPrevious));
                            continue;
                        }

                        if (HandleCommonCommand(cmdAfterSkill, chr, hasPrevious) ||
                            cmdAfterSkill == BattleCommandType.Skills)
                            continue;
                    }

                    QueueSkill(chr, chosenIndex, chosenTarget);
                    continue;
                }

                // A secondary command was pressed while the skills UI was open
                if (secondaryCommand == BattleCommandType.Items)
                {
                    // show items on same character next loop
                    continue;
                }
                if (HandleCommonCommand(secondaryCommand, chr, hasPrevious))
                    continue;
            }
        }

        commandUI.Hide();
        skillSelectionUI.Hide();
        itemSelectionUI.Hide();

        GenerateEnemyCommands();
    }

    private IEnumerator ActionResolutionPhase()
    {
        var actions = EnumerateQueuedActions()
            .OrderByDescending(a => a.user.Speed)
            .ThenBy(_ => Random.value)
            .ToList();

        // Execute
        foreach (var action in actions)
        {
            if (IsSideDefeated(playerParty)) { OnBattleEnd(false); yield break; }
            if (IsSideDefeated(enemyParty))  { OnBattleEnd(true);  yield break; }

            if (action.user == null || action.user.IsDead) continue;

            switch (action.kind)
            {
                case ActionKind.Skill:
                {
                    if (action.skill == null) break;

                    // Resolve target (may retarget)
                    BattleCharacter effectiveTarget = ResolveEffectiveTarget(action);

                    // For single-target skills, require valid target
                    if ((action.skill.targetType == SkillTargetType.SingleEnemy ||
                         action.skill.targetType == SkillTargetType.SingleAlly) &&
                        effectiveTarget == null)
                        break;

                    // Players spend SP via UseSkill; enemies ignore SP
                    if (playerParty.Contains(action.user))
                        action.user.UseSkill(action.skillIndex, effectiveTarget);
                    else
                        action.skill.Execute(action.user, effectiveTarget);

                    break;
                }

                case ActionKind.Item:
                {
                    var def = action.item;
                    var bc  = def?.battleConsumable;
                    if (bc == null) break;

                    // Self if no targeting requested
                    var tgt = bc.RequiresTarget ? action.target : action.user;

                    if (bc.RequiresTarget && !bc.CanTarget(action.user, tgt))
                        break;

                    bc.Execute(action.user, tgt, def);
                    break;
                }
            }

            if (IsSideDefeated(playerParty)) { OnBattleEnd(false); yield break; }
            if (IsSideDefeated(enemyParty))  { OnBattleEnd(true);  yield break; }

            yield return new WaitForSeconds(0.3f);
        }
    }

    private IEnumerable<QueuedAction> EnumerateQueuedActions()
    {
        foreach (var kvp in chosenSkillIndices)
        {
            var user = kvp.Key;
            int skillIndex = kvp.Value;

            if (user == null || user.IsDead) continue;
            if (skillIndex < 0 || skillIndex >= user.Skills.Count) continue;

            Skill skill = user.Skills[skillIndex];
            chosenTargets.TryGetValue(user, out BattleCharacter target);

            yield return new QueuedAction
            {
                kind       = ActionKind.Skill,
                user       = user,
                skillIndex = skillIndex,
                skill      = skill,
                target     = target
            };
        }

        foreach (var kvp in chosenItems)
        {
            var user = kvp.Key;
            var item = kvp.Value;
            if (user == null || user.IsDead) continue;
            if (item == null || item.battleConsumable == null) continue;

            chosenTargets.TryGetValue(user, out BattleCharacter target);

            yield return new QueuedAction
            {
                kind   = ActionKind.Item,
                user   = user,
                item   = item,
                target = target // may be null; self-use items will ignore and use self
            };
        }
    }

    private IEnumerator ItemsFlow(BattleCharacter chr, bool hasPrevious)
    {
        bool decisionMade   = false;
        bool itemChosen     = false;
        int  chosenInvIndex = -1;
        BattleCommandType secondaryCommand = BattleCommandType.Items;

        // Keep command UI active so Back/Skip/Skills are available
        commandUI.ShowForCharacter(chr, hasPrevious, cmd =>
        {
            secondaryCommand = cmd;
            decisionMade     = true;
        });

        itemSelectionUI.ShowForCharacter(chr, invIndex =>
        {
            chosenInvIndex = invIndex;
            itemChosen     = true;
            decisionMade   = true;
        });

        while (!decisionMade) yield return null;

        itemSelectionUI.Hide();

        // If an item row was clicked
        if (itemChosen)
        {
            var inv = MapCombatTransfer.Instance?.GetInventory();
            if (inv != null && chosenInvIndex >= 0 && chosenInvIndex < inv.Count)
            {
                var item = inv[chosenInvIndex]?.item;
                var bc   = item?.battleConsumable;

                if (item != null && bc != null)
                {
                    // Self-use item: queue immediately, no targeting
                    if (!bc.RequiresTarget)
                    {
                        QueueItem(chr, item, chr);
                        yield break;
                    }

                    // Targeted item: click-to-target, with secondary commands
                    BattleCharacter chosenTarget = null;
                    bool waitingForTarget        = true;
                    bool cancelToCommand         = false;
                    BattleCommandType cmdAfterItem = BattleCommandType.Items;

                    commandUI.ShowForCharacter(chr, hasPrevious, cmd =>
                    {
                        cmdAfterItem    = cmd;
                        cancelToCommand = true;
                    });

                    UnityAction<BattleCharacter> handler = null;
                    handler = clicked =>
                    {
                        if (!bc.CanTarget(chr, clicked)) return;
                        if (cancelToCommand) return;

                        chosenTarget     = clicked;
                        waitingForTarget = false;
                    };
                    ClickManagerBattle.OnCharacterClicked.AddListener(handler);

                    while (waitingForTarget && !cancelToCommand) yield return null;

                    ClickManagerBattle.OnCharacterClicked.RemoveListener(handler);

                    if (cancelToCommand)
                    {
                        if (HandleCommonCommand(cmdAfterItem, chr, hasPrevious) ||
                            cmdAfterItem == BattleCommandType.Skills ||
                            cmdAfterItem == BattleCommandType.Items)
                            yield break;
                    }

                    QueueItem(chr, item, chosenTarget);
                    yield break;
                }
            }

            // Invalid item/stack → do nothing; stay on same character
            yield break;
        }

        // A secondary command was pressed while the list was open
        if (HandleCommonCommand(secondaryCommand, chr, hasPrevious))
            yield break;
        // Skills or Items fall-through: caller decides next step
        yield break;
}

    public IEnumerable<BattleCharacter> GetAlliesOf(BattleCharacter c)
    {
        if (playerParty.Contains(c)) return new List<BattleCharacter>(playerParty);
        if (enemyParty.Contains(c))  return new List<BattleCharacter>(enemyParty);
        return new List<BattleCharacter>();
    }

    public IEnumerable<BattleCharacter> GetEnemiesOf(BattleCharacter c)
    {
        if (playerParty.Contains(c)) return new List<BattleCharacter>(enemyParty);
        if (enemyParty.Contains(c))  return new List<BattleCharacter>(playerParty);
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
        c.gameObject.SetActive(false);
    }

    private bool IsSideDefeated(IEnumerable<BattleCharacter> group)
    {
        foreach (var c in group)
            if (c != null && !c.IsDead) return false;
        return true;
    }

    private BattleCharacter ResolveEffectiveTarget(QueuedAction action)
    {
        if (action == null || action.skill == null || action.user == null)
            return null;

        var type = action.skill.targetType;

        // AoE skills: we don't need a specific clicked target
        if (type == SkillTargetType.AllEnemies || type == SkillTargetType.AllAllies)
            return action.target;

        bool isAllyTarget = (type == SkillTargetType.SingleAlly);
        var pool = isAllyTarget ? GetAlliesOf(action.user) : GetEnemiesOf(action.user);

        var candidates = new List<BattleCharacter>();
        foreach (var c in pool)
            if (c != null && !c.IsDead) candidates.Add(c);

        if (candidates.Count == 0)
            return null;

        if (action.target != null && !action.target.IsDead && candidates.Contains(action.target))
            return action.target;

        int idx = Random.Range(0, candidates.Count);
        return candidates[idx];
    }

    public void RegisterDamage(BattleCharacter source, BattleCharacter target, int amount)
    {
        if (source == null || amount <= 0) return;
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

            if ((skill.targetType == SkillTargetType.SingleEnemy ||
                 skill.targetType == SkillTargetType.SingleAlly) &&
                (target == null || target.IsDead))
                continue;

            chosenSkillIndices[enemy] = skillIndex;
            chosenTargets[enemy]      = target;
        }
    }

    private void EvaluateEnemyAction(BattleCharacter enemy, out int bestSkillIndex, out BattleCharacter bestTarget)
    {
        bestSkillIndex = -1;
        bestTarget     = null;

        var skills = enemy.Skills;
        if (skills == null || skills.Count == 0) return;

        int focusIndex = turnIndex % skills.Count;

        int globalBestValue  = int.MinValue;
        int globalBestThreat = int.MinValue;

        var skillOrder = Enumerable.Range(0, skills.Count).ToList();
        Shuffle(skillOrder);

        foreach (int i in skillOrder)
        {
            Skill skill = skills[i];
            if (skill == null) continue;

            int baseValue = (i == focusIndex) ? 3 : 0;

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
                if (c != null && !c.IsDead) candidates.Add(c);
            if (candidates.Count == 0) continue;

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
                    effectiveThreat += 100000;
                }

                if (value > skillBestValue ||
                   (value == skillBestValue && effectiveThreat > skillBestThreat))
                {
                    skillBestValue   = value;
                    skillBestThreat  = effectiveThreat;
                    skillBestTarget  = target;
                }
            }

            if (skillBestTarget == null) continue;

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

    private void OnBattleEnd(bool playerWon)
    {
        MapCombatTransfer.Instance.ApplyBattleResult(playerWon, playerParty);
        SceneManager.LoadScene("Scenes/Map Scene");
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
