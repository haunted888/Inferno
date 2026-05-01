using UnityEngine;
using System.Linq;


public class EnemySlotController : MonoBehaviour
{
    public BattleSlots battleSlots;
    private BattleCharacter[] enemies;
    private Transform[] enemyHomes;
    public GameObject enemyHealthBarPrefab;
    public Transform healthBarParent;


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
            
            // SP for enemies: Not currently used, but can be implemented later if desired. For now, just initialize to 0.
            //int maxSp = def.GetMaxSp();
            //int currentSp = 0;    

            chr.ApplyStats(stats, currentHp);
            chr.SetName(def.GetDisplayName());
            chr.SetLevel(def.level);

            //chr.SetMaxSp(maxSp, fillToMax: false);
            //chr.SetSp(currentSp);

            chr.ClearSkills();
            foreach (var s in def.GetEffectiveSkills())
                if (s != null) chr.AddSkill(s);

            chr.ClearPassives();
            foreach (var p in def.passives)
                if (p != null) chr.AddPassive(p);
            // Spawn health bar
            if (enemyHealthBarPrefab != null)
            {
                var barObj = Instantiate(enemyHealthBarPrefab, healthBarParent);
                var bar    = barObj.GetComponent<WorldSpaceStatusUI>();
                if (bar != null)
                    bar.Initialize(chr);

            }
            
            if(inst.GetComponent<Outline>() != null)
                inst.GetComponent<Outline>().enabled = false; // Disable outline by default; can be enabled later when selecting targets, etc.

        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        RefreshPositions();

    }

    void GoToHome()
    {

        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].transform.position = enemyHomes[i].position;
        }
    }

    public void RefreshPositions()
    {
        enemies = GetComponentsInChildren<BattleCharacter>(false)
            .Where(c => c != null && c.gameObject.activeSelf)
            .ToArray();

        enemyHomes = battleSlots.GetSlots(false, enemies, 1);
        GoToHome();
    }
}
