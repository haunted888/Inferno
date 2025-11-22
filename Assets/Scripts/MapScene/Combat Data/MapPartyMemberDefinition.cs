using System.ComponentModel;
using UnityEngine;

[System.Serializable]
public class MapPartyMemberDefinition
{
    public GameObject characterPrefab;

    public string displayName = "Unnamed";

    [Header("Optional Character Asset")]
    public CharacterTemplate characterAsset;

    [Header("Inspector Overrides")]
    public bool overrideStats  = false;
    public bool overrideSkills = false;

    [Header("Stats (max values, editable)")]
    public CombatStats stats = new CombatStats
    {
        maxHealth          = 100,
        maxSp              = 10,
        speed              = 10,
        physicalAttack     = 100,
        elementalPower     = 100,
        defense            = 0,
        elementalResistance= 0,
        critChance         = 5,
        critDamage         = 150
    };

    [Header("Skills (used if overrideSkills = true or no asset)")]
    public Skill[] skills;

    [HideInInspector] public int health = -1;
    [HideInInspector] public int sp     = -1; 

    [HideInInspector] public bool initializedFromAssetStats  = false;
    [HideInInspector] public bool initializedFromAssetSkills = false;

    public void EnsureInitializedFromAsset()
    {
        if (characterAsset == null)
            return;

        if (!initializedFromAssetStats && !overrideStats)
        {
            stats = characterAsset.baseStats;
            stats.maxHealth = Mathf.Max(1, stats.maxHealth);
            stats.maxSp     = Mathf.Max(0, stats.maxSp);

            initializedFromAssetStats = true;
        }

        if (!initializedFromAssetSkills && !overrideSkills &&
            characterAsset.skills != null && characterAsset.skills.Length > 0)
        {
            skills = (Skill[])characterAsset.skills.Clone();
            initializedFromAssetSkills = true;
        }
    }

    public string GetDisplayName()
    {
        if (!string.IsNullOrEmpty(displayName))
            return displayName;

        if (characterAsset != null && !string.IsNullOrEmpty(characterAsset.displayName))
            return characterAsset.displayName;

        return "Unnamed";
    }

    public int GetMaxHealth()
    {
        return Mathf.Max(1, stats.maxHealth);
    }
    
    public int GetMaxSp()
    {
        return Mathf.Max(0, stats.maxSp);
    }
    public CombatStats GetEffectiveStats()
    {
        var result = stats;
        result.maxHealth = Mathf.Max(1, result.maxHealth);
        return result;
    }
    public Skill[] GetEffectiveSkills()
    {
        return skills ?? System.Array.Empty<Skill>();
    }

}
