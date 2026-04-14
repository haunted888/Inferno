using UnityEngine;
using System.Linq;

public class PartySlotController : MonoBehaviour
{
    public BattleSlots battleSlots;
    private BattleCharacter[] partyMembers;
    private Transform[] partyHomes;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Awake()
    {
        var defs = MapCombatTransfer.Instance.GetParty();

        foreach (var def in defs)
        {
            var inst = Instantiate(def.characterPrefab, this.transform);
            var chr  = inst.GetComponent<BattleCharacter>();
            if (chr == null) continue;

            

            chr.sourceDefinition = def;

            CombatStats stats = def.GetEffectiveStats();
            int maxHp = stats.maxHealth;
            
            // If this is somehow uninitialized, default to full HP
            int currentHp = def.health < 0
                ? maxHp
                : Mathf.Clamp(def.health, 0, maxHp);

            int maxSp    = def.GetMaxSp();
            int currentSp = def.sp < 0
                ? maxSp
                : Mathf.Clamp(def.sp, 0, maxSp);

            chr.ApplyStats(stats, currentHp);
            chr.SetName(def.GetDisplayName());
            chr.SetLevel(def.level);
            
            chr.SetMaxSp(maxSp, fillToMax: false);
            chr.SetSp(currentSp);

            // Traits must be set up before passives/skills since they can be affected by those
            chr.ClearTraits();
            if (def.traits != null)
                chr.Traits.AddRange(def.traits);
                foreach (var t in chr.Traits)
                {
                    if (t != null)
                        chr.traitTypes.Add(t.traitType);
                }

            // Allow traits to configure battle state (ammo, etc.)
            for (int i = 0; i < chr.Traits.Count; i++)
            {
                var t = chr.Traits[i];
                if (t != null)
                    t.SetupForBattle(def, chr);
            }

            chr.ClearPassives();
            foreach (var p in def.passives)
                if (p != null) chr.AddPassive(p);

            chr.ClearSkills();
            foreach (var s in def.GetEffectiveSkills())
                if (s != null) chr.AddSkill(s);

            
            

            if(inst.GetComponent<Outline>() != null)
                inst.GetComponent<Outline>().enabled = false; // Disable outline by default; can be enabled later when selecting targets, etc.

        }
    }


    void Start()
    {
        RefreshPositions();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GoToHome()
    {

        for (int i = 0; i < partyMembers.Length; i++)
        {
            partyMembers[i].transform.position = partyHomes[i].position;
        }
    }

    public void RefreshPositions()
    {
        partyMembers = GetComponentsInChildren<BattleCharacter>(false)
            .Where(c => c != null && c.gameObject.activeSelf)
            .ToArray();

        partyHomes = battleSlots.GetSlots(true, partyMembers, 1);
        GoToHome();
    }
}
