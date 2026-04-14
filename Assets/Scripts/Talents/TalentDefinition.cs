using UnityEngine;

[CreateAssetMenu(menuName = "Talents/Talent")]
public class TalentDefinition : ScriptableObject
{
    [Header("Identity")]
    public string id;                 // unique per talent (e.g., GUID or readable key)
    public string displayName = "Talent";
    [TextArea]public string description = "Talent description here.";

    [Header("Cost")]
    public int cost = 1;

    [Header("Grants")]
    public CombatStats statBonus;     // additive deltas (can be zeroes)
    public Skill[]     grantSkills;   // optional
    public PassivesDefinition[]   grantPassives; // optional
}
