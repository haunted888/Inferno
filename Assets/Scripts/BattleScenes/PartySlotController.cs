using UnityEngine;

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
            chr.setName(def.GetDisplayName());
            
            chr.SetMaxSp(maxSp, fillToMax: false);
            chr.SetSp(currentSp);

            chr.ClearSkills();
            foreach (var s in def.GetEffectiveSkills())
                if (s != null) chr.AddSkill(s);
        }
    }


    void Start()
    {
        partyMembers = GetComponentsInChildren<BattleCharacter>();
        partyHomes = battleSlots.GetSlots(true, partyMembers, 1);
        GoToHome();
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
}
