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
        if (skill.skillDetailShell.ammoCost <= 0f && skill.skillDetailShell.flatAmmoCost <= 0)
            return;

        // Determine ammo cost from skill percent
        float percent = Mathf.Clamp01(skill.skillDetailShell.ammoCost);
        int needed = Mathf.CeilToInt(percent * user.ConstantMaxAmmo);
        needed += skill.skillDetailShell.flatAmmoCost;
        if (needed <= 0) return;

        // Spend what we actually have
        int spend = Mathf.Min(needed, user.CurrentAmmo);

        if(needed > user.CurrentAmmo)
        {
            damage = 0;
            return; // Not enough ammo to use the skill, so it misses (0 damage)
        }

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
