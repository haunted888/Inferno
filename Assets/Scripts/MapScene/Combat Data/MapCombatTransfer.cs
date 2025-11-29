using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class ItemStack
{
    public ItemDefinition item;
    public int quantity;
    public ItemStack(ItemDefinition i, int q) { item = i; quantity = q; }
}

public class MapCombatTransfer : MonoBehaviour
{
    public static MapCombatTransfer Instance { get; private set; }

    public CampUIManager campUIManager;

    public List<MapPartyMemberDefinition> party = new List<MapPartyMemberDefinition>();
    public List<MapPartyMemberDefinition> camp  = new List<MapPartyMemberDefinition>();

    private readonly HashSet<string> visitedNodeNames    = new HashSet<string>();
    private readonly HashSet<string> destroyedNodeNames  = new HashSet<string>();
    [HideInInspector] public string currentMapNodeName = "";

    private List<MapEnemyDefinition> pendingEnemies = new List<MapEnemyDefinition>();
    private readonly List<MapRewardDefinition> pendingRewards = new List<MapRewardDefinition>();

    [HideInInspector] public string lastSafeNodeName;
    [HideInInspector] public string combatNodeObjectName;
    [HideInInspector] public bool  lastBattlePlayerWon;


    public bool HasPartyData { get; private set; }
    public bool HasPathState => visitedNodeNames.Count > 0 || !string.IsNullOrEmpty(currentMapNodeName);

    [HideInInspector] public bool starterChoiceCompleted = false;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        BootstrapCampMembers();
    }

    public void SetupEnemies(MapEnemyDefinition[] defs)
    {
        pendingEnemies.Clear();
        if (defs != null)
            pendingEnemies.AddRange(defs);
    }

    public List<MapEnemyDefinition> GetEnemies()
    {
        var copy = new List<MapEnemyDefinition>(pendingEnemies);
        pendingEnemies.Clear();
        return copy;
    }

    public void SetupParty(MapPartyMemberDefinition[] defs)
    {
        party.Clear();
        if (defs != null)
        {
            party.AddRange(defs);

            // Ensure every party member also exists in camp
            foreach (var d in defs)
            {
                AddCampMember(d);
            }
        }
        HasPartyData = true;
    }

    public List<MapPartyMemberDefinition> GetParty()
    {
        return new List<MapPartyMemberDefinition>(party);
    }

    public void SyncCampMembers(IEnumerable<MapPartyMemberDefinition> defs)
    {
        var unique = new HashSet<MapPartyMemberDefinition>();
        camp.Clear();

        if (defs != null)
        {
            foreach (var def in defs)
            {
                if (def == null || !unique.Add(def))
                    continue;
                camp.Add(def);
            }
        }

        // Ensure party members are also present even if camp data missed them
        foreach (var def in party)
        {
            if (def == null || !unique.Add(def))
                continue;
            camp.Add(def);
        }
    }

    // called from BattleTurnManager at end of combat
    public void ApplyBattleResult(bool playerWon, List<BattleCharacter> battlePlayers)
    {
        // Write back HP into the definitions (camp + party see the same objects)
        if (battlePlayers != null)
        {
            foreach (var bc in battlePlayers)
            {
                if (bc == null) continue;
                var def = bc.sourceDefinition;
                if (def == null) continue;

                def.health = Mathf.Max(0, bc.CurrentHealth);
                def.sp     = Mathf.Max(0, bc.CurrentSp);   // NEW
            }
        }

        // Party becomes only living members (dead ones are no longer in party)
        party = party
            .Where(d => d != null && d.health > 0)
            .ToList();

        // If literally nobody in camp is alive, end the game
        // If literally nobody in camp (which now includes party members) is alive, end the game
        bool anyAlive = camp.Any(d => d != null && d.health > 0);
        if (!anyAlive)
        {
            Debug.Log("All camp members are dead. Game Over.");
            Application.Quit();
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
            return;
        }

        lastBattlePlayerWon = playerWon;
        if (!playerWon)
        {
            // do not keep rewards if the battle was lost
            ClearPendingRewards();
        }
    }


    public void RegisterBattleContext(string safeNodeName, string combatNodeName)
    {
        lastSafeNodeName     = safeNodeName;
        combatNodeObjectName = combatNodeName;
    }

    public void SavePathState(IEnumerable<PathNode> visitedNodes, PathNode currentNode)
    {
        visitedNodeNames.Clear();
        if (visitedNodes != null)
        {
            foreach (var node in visitedNodes)
            {
                if (node == null) continue;
                var name = node.name;
                if (!string.IsNullOrEmpty(name))
                    visitedNodeNames.Add(name);
            }
        }

        currentMapNodeName = currentNode != null ? currentNode.name : "";
    }

    public IReadOnlyCollection<string> GetVisitedNodeNames()
    {
        return visitedNodeNames;
    }

    public void RegisterDestroyedNode(string nodeName)
    {
        if (string.IsNullOrEmpty(nodeName)) return;
        destroyedNodeNames.Add(nodeName);
    }

    public void ApplyDestroyedNodesInScene()
    {
        foreach (var nodeName in destroyedNodeNames)
        {
            if (string.IsNullOrEmpty(nodeName)) continue;
            var go = GameObject.Find(nodeName);
            if (go != null)
                Destroy(go);
        }
    }

    void BootstrapCampMembers()
    {
        var manager = campUIManager;

        if (manager == null)
        {
            foreach (var candidate in Resources.FindObjectsOfTypeAll<CampUIManager>())
            {
                if (candidate == null) continue;
                var go = candidate.gameObject;
                if (go == null) continue;
                var scene = go.scene;
                if (!scene.IsValid()) continue;
                manager = candidate;
                break;
            }
        }

        if (manager != null)
        {
            SyncCampMembers(manager.campMembers);
        }
    }

    public void StartNewGame()
    {
        // Clear party and camp
        party.Clear();
        camp.Clear();
        HasPartyData = false;

        // Clear enemies and map state
        pendingEnemies.Clear();
        visitedNodeNames.Clear();
        destroyedNodeNames.Clear();
        currentMapNodeName = "";

        lastSafeNodeName = "";
        combatNodeObjectName = "";
        lastBattlePlayerWon = false;

        starterChoiceCompleted = false;
    }
        public void SetPendingRewards(IEnumerable<MapRewardDefinition> rewards)
    {
        pendingRewards.Clear();
        if (rewards == null) return;

        foreach (var r in rewards)
        {
            if (r != null)
                pendingRewards.Add(r);
        }
    }

    private readonly List<ItemStack> inventory = new List<ItemStack>();

    public IReadOnlyList<ItemStack> GetInventory() => inventory;

    public void AddItem(ItemDefinition item, int qty)
    {
        if (item == null || qty <= 0) return;
        var stack = inventory.Find(s => s.item == item);
        if (stack != null) stack.quantity += qty;
        else inventory.Add(new ItemStack(item, qty));
    }
    public List<MapRewardDefinition> GetPendingRewards()
    {
        return new List<MapRewardDefinition>(pendingRewards);
    }

    public void ClearPendingRewards()
    {
        pendingRewards.Clear();
    }

    public void AddPartyMember(MapPartyMemberDefinition def, bool addToPartyIfSpace)
    {
        if (def == null) return;

        // ensure in camp
        AddCampMember(def);

        // add to party if there is space and not already present
        if (addToPartyIfSpace && !party.Contains(def) && party.Count < 4)
        {
            party.Add(def);
            HasPartyData = true;
        }
    }
    public void SetPendingRewardsFromGroups(IEnumerable<MapRewardGroup> groups)
    {
        pendingRewards.Clear();
        if (groups == null) return;

        foreach (var group in groups)
        {
            if (group == null || group.options == null || group.options.Length == 0)
                continue;

            // filter out null entries
            var candidates = new List<MapRewardDefinition>();
            foreach (var opt in group.options)
            {
                if (opt != null)
                    candidates.Add(opt);
            }

            if (candidates.Count == 0)
                continue;

            int index = Random.Range(0, candidates.Count);
            var chosen = candidates[index];
            pendingRewards.Add(chosen);
        }
    }
    private void AddCampMember(MapPartyMemberDefinition def)
    {
        if (def == null) return;

        // Initialize from asset only the first time this definition enters the camp
        def.EnsureInitializedFromAsset();

        // If current HP has never been set, start at full
        if (def.health < 0)
            def.health = def.GetMaxHealth();
        Debug.Log($"Added camp member {def.displayName} with health {def.health}");
        // If current SP has never been set, start at full
        if (def.sp < 0)
            def.sp = def.GetMaxSp();

        if (!camp.Contains(def)){
            def.sp = def.GetMaxSp();
            camp.Add(def);
        }
    }
    public void RemoveItem(ItemDefinition item, int qty)
    {
        if (item == null || qty <= 0) return;
        var stack = inventory.Find(s => s.item == item);
        if (stack == null) return;
        stack.quantity -= qty;
        if (stack.quantity <= 0) inventory.Remove(stack);
    }
    
    // Held-item equip state: one-to-one between member and item
    private readonly Dictionary<MapPartyMemberDefinition, ItemDefinition> equippedByMember
        = new Dictionary<MapPartyMemberDefinition, ItemDefinition>();
    private readonly Dictionary<ItemDefinition, MapPartyMemberDefinition> equippedToMember
        = new Dictionary<ItemDefinition, MapPartyMemberDefinition>();

    public ItemDefinition GetEquippedItem(MapPartyMemberDefinition member)
    {
        if (member == null) return null;
        equippedByMember.TryGetValue(member, out var item);
        return item;
    }
    public MapPartyMemberDefinition GetItemHolder(ItemDefinition item)
    {
        if (item == null) return null;
        equippedToMember.TryGetValue(item, out var who);
        return who;
    }

    // Equip: ensure bijection, adjust inventory counts appropriately
    public void EquipHeldItem(MapPartyMemberDefinition member, ItemDefinition item)
    {
        if (member == null || item == null) return;

        // If item is equipped on someone else, detach from them (no inventory change)
        if (equippedToMember.TryGetValue(item, out var previousHolder) && previousHolder != null && previousHolder != member)
        {
            equippedByMember.Remove(previousHolder);
            equippedToMember[item] = member; // reassign mapping
        }
        else
        {
            // Item is not currently equipped — must consume from inventory
            RemoveItem(item, 1); // will no-op if not present (author your data so stacks exist)
            equippedToMember[item] = member;
        }

        // If member had a different item, return that to inventory
        if (equippedByMember.TryGetValue(member, out var oldItem) && oldItem != null && oldItem != item)
        {
            equippedToMember.Remove(oldItem);
            AddItem(oldItem, 1);
        }

        // Finalize mapping
        equippedByMember[member] = item;
    }

    public void UnequipHeldItemFromMember(MapPartyMemberDefinition member)
    {
        if (member == null) return;
        if (!equippedByMember.TryGetValue(member, out var item) || item == null) return;

        // Break mapping and return to inventory
        equippedByMember.Remove(member);
        if (equippedToMember.TryGetValue(item, out var holder) && holder == member)
            equippedToMember.Remove(item);

        AddItem(item, 1);
    }


}
