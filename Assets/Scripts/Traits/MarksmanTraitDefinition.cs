using UnityEngine;

[CreateAssetMenu(menuName = "Traits/Marksman")]
public class MarksmanTraitDefinition : TraitDefinition
{
    [Header("Ammo settings")]
    public int constantMaxAmmo = 16;      // used only for % cost
    public int startingMaxAmmo = 16;      // actual max (modifiable later)
    public float bonusDamagePercentPerAmmo = 0.05f; // 5% per ammo by default

    public void Awake()
    {
        traitType = CharacterTrait.Marksman;
    }

    public override void SetupForBattle(MapPartyMemberDefinition source, BattleCharacter battleChar)
    {
        // Initialize ammo when battle starts
        int max = Mathf.Max(0, startingMaxAmmo);
        int cur = Mathf.Max(0, startingMaxAmmo);
        int cst = Mathf.Max(0, constantMaxAmmo);

        battleChar.SetAmmo(max, cur, cst);
    }

    public override void OnModifySkillDamage(
        BattleCharacter user,
        Skill skill,
        BattleCharacter target,
        ref int damage)
    {
        if (skill == null) return;

        // Only apply to skills that use ammo
        if (skill.ammoCost <= 0f)
            return;

        if (user.ConstantMaxAmmo <= 0 || user.CurrentAmmo <= 0)
            return;

        // Determine ammo cost from skill percent
        float percent = Mathf.Clamp01(skill.ammoCost);
        int needed = Mathf.CeilToInt(percent * user.ConstantMaxAmmo);
        if (needed <= 0) return;

        // Spend what we actually have
        int spend = Mathf.Min(needed, user.CurrentAmmo);
        if (spend <= 0) return;

        user.SpendAmmo(spend);

        // -----------------------------
        //  NEW: percentage-based scaling
        // -----------------------------
        float totalPercent = bonusDamagePercentPerAmmo * spend;  // e.g., 5% per ammo × 3 ammo = +15%
        if (totalPercent > 0f)
        {
            int bonus = Mathf.RoundToInt(damage * totalPercent);
            damage += bonus;
        }
    }

}
