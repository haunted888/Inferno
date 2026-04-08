using UnityEngine;

[CreateAssetMenu(menuName = "Passives/Trait Passives/Entertainer Passive Definition")]
public class EntertainerPassiveDefinition : PassivesDefinition
{
    public float spHealPercentage = 0f; // Example value for spHealPercentage
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void setSpHeal(float percentage)
    {
        spHealPercentage = percentage;
    }

    public override void OnSkillUsed(BattleCharacter self, BattleCharacter target, Skill skill)
    {
        var allies = self.GetAllies();;
        foreach (var a in allies)
        {
            if (a == self) continue;
            a.RecoverSp(Mathf.RoundToInt(spHealPercentage * skill.skillDetailShell.spCost));
        }
    }
}
