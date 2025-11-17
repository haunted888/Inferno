using UnityEngine;

public class EnemySlotController : MonoBehaviour
{
    public BattleSlots battleSlots;
    private BattleCharacter[] enemies;
    private Transform[] enemyHomes;

    //Awake is called when the script instance is being loaded
    void Awake()
    {
        var defs = MapCombatTransfer.Instance.ConsumeEnemies();

        foreach (var def in defs)
        {
            var inst = Instantiate(def.enemyPrefab, this.transform);
            Debug.Log("Instantiated enemy prefab: " + def.enemyPrefab.name);
            var chr = inst.GetComponent<BattleCharacter>();
            if (chr == null) continue;

            chr.SetStats(def.health, def.speed);   
            chr.ClearSkills();
            foreach (var s in def.skills)
            {
                if (s != null)
                    chr.AddSkill(s);
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
