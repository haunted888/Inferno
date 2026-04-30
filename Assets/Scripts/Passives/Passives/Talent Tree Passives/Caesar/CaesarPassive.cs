using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Passives/Caesar/Caesar")]
public class CaesarPassive : PassivesDefinition
{
    public float attackBoostPercent = 0.1f;
    public float defenseBoostPercent = 0.1f;
    public float elementalResistanceBoostPercent = 0.1f;
    public float elementalPowerBoostPercent = 0.1f;

    public TempStatBoostPercent passiveBoost;

    public override void OnActionEnd(BattleCharacter self, BattleCharacter target)
    {
        var actionOrder = self.GetCurrentActionOrder();
        bool userActed = false;
        var allies = self.GetAllies();

        foreach (var action in actionOrder)
        {
            if(!userActed && action?.user != null)
            {
                if(action.user == self) userActed = true;
            } 
            else if (userActed && action?.user != null && allies.Contains(action.user) && action.user != self && !action.user.IsDead)
            {
                passiveBoost.setStatBoosts(
                    attackBoostPercent,
                    elementalPowerBoostPercent,
                    defenseBoostPercent,
                    elementalResistanceBoostPercent
                );
                action.user.AddPassive(passiveBoost, self);
                Debug.Log($"CaesarPassive triggered for {action.user.name}.");
            }

        }
    }

    
}
