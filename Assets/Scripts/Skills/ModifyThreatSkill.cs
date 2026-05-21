using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Modify Threat")]
public class ModifyThreatSkill : Skill
{
    public enum ThreatModificationType
    {
        AddFlat,
        AddPercent,
        Set
    }
    public int threatValue = 0;
    public ThreatModificationType modificationType = ThreatModificationType.AddFlat;


    public override void Execute(BattleCharacter user, BattleCharacter target)
    {
        switch (modificationType)
        {
            case ThreatModificationType.AddFlat:
                target.AddThreat(threatValue);
                break;
            case ThreatModificationType.AddPercent:
                int addedThreat = Mathf.RoundToInt(target.Threat * (threatValue / 100f));
                target.AddThreat(addedThreat);
                break;
            case ThreatModificationType.Set:
                target.SetThreat(threatValue);
                break;
        }

        ExecuteFollowUps(user, target);

        EndExecution();
    }
}
