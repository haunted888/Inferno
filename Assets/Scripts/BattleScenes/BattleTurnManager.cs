using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

// Action queue
public enum ActionKind { Skill, Item }
public class QueuedAction
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

    [NonSerialized] public PassiveMutationUtility.PassiveMutationContext passiveMutationContext = new PassiveMutationUtility.PassiveMutationContext();
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
        if(lastChar.DelayedCastSkill != null)
        {
            if(!TryStepBack(1))
                return false;
        }
        commandOrder.RemoveAt(commandOrder.Count - 1);
        chosenSkillIndices.Remove(lastChar);
        chosenTargets.Remove(lastChar);
        chosenItems.Remove(lastChar);

        int idx = playerParty.IndexOf(lastChar);
        currentPlayerIndex = Mathf.Max(0, idx);

        return true;
    }

    private bool TryStepBack(int steps)
    {
        if (commandOrder.Count - steps == 0)
            return false;

        var lastChar = commandOrder[commandOrder.Count - steps - 1];
        if(lastChar.DelayedCastSkill != null)
        {
            if(!TryStepBack(1 + steps))
                return false;
        }
        commandOrder.RemoveAt(commandOrder.Count - steps - 1);
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

    

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        playerParty = playerPartyParent.GetComponentsInChildren<BattleCharacter>(false).ToList();
        enemyParty  = enemyPartyParent.GetComponentsInChildren<BattleCharacter>(false).ToList();

        AssignPassiveMutationContext();

        Trigger_BattleStart();

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
            ClearAllTargetOutlines();

            yield return StartCoroutine(CommandSelectionPhase());

            state = TurnState.ActionResolve;
            yield return StartCoroutine(ActionResolutionPhase());

            turnIndex++;
            yield return null;
        }
    }

    private IEnumerator CommandSelectionPhase()
    {
        Trigger_CommandPhaseStart();
        while (currentPlayerIndex < playerParty.Count)
        {
            BattleCharacter chr = playerParty[currentPlayerIndex];

            // Skip invalid characters
            if (chr == null || chr.IsDead)
            {
                currentPlayerIndex++;
                continue;
            }

            if(chr.DelayedCastSkill != null)
            {
                Debug.Log($"{chr.name} is still casting {chr.DelayedCastSkill.skill.skillName}, {chr.DelayedCastTurns} turns remaining. Automatically skipping turn.");
                QueueSkill(chr, chr.DelayedCastSkill.skillIndex, chr.DelayedCastSkill.target);
                continue;
            }

            bool hasPrevious = commandOrder.Count > 0;

            // 1) Main command menu
            bool waitingForCommand = true;
            BattleCommandType chosenCommand = BattleCommandType.Skills;

            ClearAllTargetOutlines();

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

                    IEnumerable<BattleCharacter> targetPool =
                        chosenSkill.targetType == SkillTargetType.SingleEnemy || chosenSkill.targetType == SkillTargetType.AllEnemies
                            ? GetEnemiesOf(chr)
                            : chosenSkill.targetType == SkillTargetType.SingleAlly || chosenSkill.targetType == SkillTargetType.AllAllies
                                ? GetAlliesOf(chr)
                                : new List<BattleCharacter> { chr };

                    SetOutlineEnabled(targetPool, true);

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
        Trigger_ResolvePhaseStart();
        var actions = ActionOrderUtility.GetOrderedActions(EnumerateQueuedActions().ToList());

        // Execute
        foreach (var a in actions)
        {
            var action = a;
            if (IsSideDefeated(playerParty)) { OnBattleEnd(false); yield break; }
            if (IsSideDefeated(enemyParty))  { OnBattleEnd(true);  yield break; }

            if (action.user == null || action.user.IsDead) continue;
            if (action.user.IsAsleep)
            {
                Debug.Log($"{action.user.name} is asleep and skips their turn!");
                action.user.HandleSkippedAction();
                
                continue;
            }

            //Handle delayed skills
            bool delayFinished = false;
            if(action.user.DelayedCastSkill != null)
            {
                action.user.DelayedCastTurns--;
                if (action.user.DelayedCastTurns <= 0)
                {
                    delayFinished = true;
                    Debug.Log($"{action.user.name} finishes casting {action.skill.skillName}!");
                    action = action.user.DelayedCastSkill;
                    action.user.DelayedCastSkill = null;
                    action.user.DelayedCastTurns = 0;
                }
                else
                {
                    Debug.Log($"{action.user.name} is still casting {action.skill.skillName}, {action.user.DelayedCastTurns} turns remaining.");
                    continue;
                }
            }

            switch (action.kind)
            {
                case ActionKind.Skill:
                {
                    if (action.skill == null) break;
                    if (action.skill.delay > 0 && !delayFinished) // Handle delayed cast: store skill and remaining turns on character, skip execution for now
                    {
                        action.user.DelayedCastSkill = action;
                        action.user.DelayedCastTurns = action.skill.delay;
                        Debug.Log($"{action.user.name} begins casting {action.skill.skillName}, which will execute in {action.skill.delay} turns!");
                        break;
                    }

                    // Resolve target (may retarget)
                    BattleCharacter effectiveTarget = ResolveEffectiveTarget(action);

                    // For single-target skills, require valid target
                    if ((action.skill.targetType == SkillTargetType.SingleEnemy ||
                         action.skill.targetType == SkillTargetType.SingleAlly) &&
                        effectiveTarget == null)
                        break;

                    // Apply passive effects (NOTE: Kinda janky, might fix later)
                    if (action.user.passives != null)
                    {
                        PassiveMutationUtility.InvokePassivesWithMutation(
                            action.user,
                            () => action.user.passives,
                            p => p.OnSkillUsed(action.user, action.target, action.skill),
                            passiveMutationContext
                        );
                    }

                    List<BattleCharacter> targets = BattleUtility.GetTargetsForSkill(action.skill, action.user, action.target);
                    foreach (var t in targets)
                    {
                        PassiveMutationUtility.InvokePassivesWithMutation(
                            t,
                            () => t.passives,
                            p => p.OnSkillReceived(t, action.user, action.skill),
                            passiveMutationContext
                        );

                    }

                    // Players spend SP via UseSkill; enemies ignore SP
                    if (playerParty.Contains(action.user))
                        action.user.UseSkill(action.skillIndex, effectiveTarget);
                    else
                        action.skill.Execute(action.user, effectiveTarget);

                    // Apply passive effects
                    if (action.user.passives != null)
                    {
                        PassiveMutationUtility.InvokePassivesWithMutation(
                            action.user,
                            () => action.user.passives,
                            p => p.OnSkillUsedEnd(action.user, action.target, action.skill),
                            passiveMutationContext
                        );
                    }

                    foreach (var t in targets)
                    {
                        PassiveMutationUtility.InvokePassivesWithMutation(
                            t,
                            () => t.passives,
                            p => p.OnSkillReceivedEnd(t, action.user, action.skill),
                            passiveMutationContext
                        );
                    }
                    

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
        Trigger_ResolvePhaseEnd();
    }

    private IEnumerable<QueuedAction> EnumerateQueuedActions()
    {

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
        
        foreach (var kvp in chosenSkillIndices)
        {
            var user = kvp.Key;
            int skillIndex = kvp.Value;

            if (user == null || user.IsDead) continue;

            
            Skill skill = null;
            if (skillIndex >= 0 && skillIndex < user.Skills.Count) skill = user.Skills[skillIndex];

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

                    List<BattleCharacter> itemTargetPool = new List<BattleCharacter>();
                    foreach(var p in playerParty)
                    {
                        if (bc.CanTarget(chr, p))
                        {
                            itemTargetPool.Add(p);
                        }
                    }

                    foreach(var e in enemyParty)
                    {
                        if (bc.CanTarget(chr, e))
                        {
                            itemTargetPool.Add(e);
                        }

                    }

                    SetOutlineEnabled(itemTargetPool, true);

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
        bool clickedIsSelf   = (user == clicked);

        switch (skill.targetType)
        {
            case SkillTargetType.SingleEnemy:
            case SkillTargetType.AllEnemies:
                return clickedIsEnemy;

            case SkillTargetType.SingleAlly:
            case SkillTargetType.AllAllies:
                return clickedIsAlly;

            case SkillTargetType.Self:
                return clickedIsSelf;

            default:
                return false;
        }
    }

    public void HandleCharacterDeath(BattleCharacter c)
    {
        if (c == null) return;
        foreach (var t in c.Traits) t.OnDeath(c);
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

        int idx = UnityEngine.Random.Range(0, candidates.Count);
        return candidates[idx];
    }

    public void RegisterDamage(BattleCharacter source, BattleCharacter target, int amount)
    {
        PassiveMutationUtility.InvokePassivesWithMutation(
            source,
            () => source.passives,
            p => p.OnAfterDealDamage(source, target, amount),
            passiveMutationContext
        );

        PassiveMutationUtility.InvokePassivesWithMutation(
            target,
            () => target.passives,
            p => p.OnAfterTakeDamage(target, source, amount),
            passiveMutationContext
        );

            
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
            
            Skill skill = null;
            if (skillIndex > 0) skill = enemy.Skills[skillIndex];;


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

        if(enemy.DelayedCastSkill != null)
        {
            return;
        }

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
                case SkillTargetType.Self:
                    candidatesEnum = new List<BattleCharacter> { enemy };
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
            int j = UnityEngine.Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    void ForEachCombatant(Action<BattleCharacter> action)
    {
        if (playerParty != null) foreach (var c in playerParty) { if (c != null) action(c); }
        if (enemyParty  != null) foreach (var c in enemyParty)  { if (c != null) action(c); }
    }

    void Trigger_BattleStart()
    {
        ForEachCombatant(c => { foreach (var t in c.Traits) t.OnBattleStart(c); });
        ForEachCombatant(c => { PassiveMutationUtility.InvokePassivesWithMutation(
                c,
                () => c.passives,
                p => p.OnBattleStart(c),
                passiveMutationContext
            );
        });
    }

    void Trigger_CommandPhaseStart()
    {
        ForEachCombatant(c => { PassiveMutationUtility.InvokePassivesWithMutation(
                c,
                () => c.passives,
                p => p.OnCommandPhaseStart(c),
                passiveMutationContext
            );
        });
    }

    void Trigger_ResolvePhaseStart()
    {
        
         ForEachCombatant(c => { PassiveMutationUtility.InvokePassivesWithMutation(
                c,
                () => c.passives,
                p => p.OnResolvePhaseStart(c),
                passiveMutationContext
            );
        });

    }

    void Trigger_ResolvePhaseEnd()
    {
        ForEachCombatant(c => { PassiveMutationUtility.InvokePassivesWithMutation(
                c,
                () => c.passives,
                p => p.OnResolvePhaseEnd(c),
                passiveMutationContext
            );
        });
    }

    private void AssignPassiveMutationContext()
    {
        foreach (var chr in playerParty)
        {
            if (chr != null)
                chr.passiveMutationContext = passiveMutationContext;
        }

        foreach (var chr in enemyParty)
        {
            if (chr != null)
                chr.passiveMutationContext = passiveMutationContext;
        }
    }

    private void SetOutlineEnabled(IEnumerable<BattleCharacter> characters, bool enabled)
    {
        if (characters == null) return;

        foreach (var chr in characters)
        {
            if (chr == null || chr.IsDead) continue;

            var outline = chr.GetComponent<Outline>();
            if (outline != null)
                outline.enabled = enabled;
        }
    }

    private void ClearAllTargetOutlines()
    {
        SetOutlineEnabled(playerParty, false);
        SetOutlineEnabled(enemyParty, false);
    }
    

}
