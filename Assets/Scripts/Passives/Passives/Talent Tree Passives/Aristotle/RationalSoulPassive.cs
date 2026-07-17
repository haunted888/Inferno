using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Passives/Characters/Aristotle/Rational Soul")]
public class RationalSoulPassive : PassivesDefinition
{
    public override void OnResetLevels(MapPartyMemberDefinition self, List<TalentDefinition> talents)
    {
        foreach (var t in talents)
        {
            foreach (var s in t.grantSkills)
            {
                self.LearnSkill(s, true);
            }
        }

        self.ResetXp();
    }

    public override int OnGainXp(MapPartyMemberDefinition self, int xpGained)
    {
        return xpGained * 2;
    }

    public override void OnGetSkills(List<Skill> effectiveSkills, List<Skill> skills)
    {
        foreach (var s in skills)
        {
            if (s != null && !effectiveSkills.Contains(s))
            {
                effectiveSkills.Add(s);
            }
        }
    }
}
