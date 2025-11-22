using UnityEngine;

public class EnemySlotController : MonoBehaviour
{
    public BattleSlots battleSlots;
    private BattleCharacter[] enemies;
    private Transform[] enemyHomes;
    public GameObject enemyHealthBarPrefab;


    //Awake is called when the script instance is being loaded
    void Awake()
    {
        var defs = MapCombatTransfer.Instance.GetEnemies();

        foreach (var def in defs)
        {
            if (def == null) continue;

            // Initialize enemy stats/skills from asset once
            def.EnsureInitializedFromAsset();

            var inst = Instantiate(def.enemyPrefab, this.transform);
            Debug.Log("Instantiated enemy prefab: " + def.enemyPrefab.name);
            var chr = inst.GetComponent<BattleCharacter>();
            if (chr == null) continue;

            CombatStats stats = def.GetEffectiveStats();
            int maxHp = stats.maxHealth;
            int currentHp = maxHp;
            
            // SP for enemies: always full at start of each battle
            int maxSp = def.GetMaxSp();     
            int currentSp = maxSp;          

            chr.ApplyStats(stats, currentHp);
            chr.setName(def.GetDisplayName());

            chr.ClearSkills();
            foreach (var s in def.GetEffectiveSkills())
                if (s != null) chr.AddSkill(s);
            // Spawn health bar
            if (enemyHealthBarPrefab != null)
            {
                var barObj = Instantiate(enemyHealthBarPrefab);
                var bar    = barObj.GetComponent<WorldSpaceStatusUI>();
                if (bar != null)
                    bar.Initialize(chr);

                // Optional: parent to enemy so it moves with them
                barObj.transform.SetParent(chr.transform);
            }
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        enemies = GetComponentsInChildren<BattleCharacter>();
        Debug.Log("EnemySlotController found " + enemies.Length + " enemies.");
        enemyHomes = battleSlots.GetSlots(false, enemies, 1);
        GoToHome();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GoToHome()
    {

        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].transform.position = enemyHomes[i].position;
        }
    }
}
