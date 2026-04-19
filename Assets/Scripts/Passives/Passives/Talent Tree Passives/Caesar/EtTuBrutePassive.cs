using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Passives/Caesar/Et Tu Brute")]
public class EtTuBrutePassive : PassivesDefinition
{
    
    public float attackBoostPercent = 0.4f;
    public float defenseBoostPercent = 0.4f;
    public float elementalResistanceBoostPercent = 0.4f;
    public float elementalPowerBoostPercent = 0.4f;
    public BetrayalPassive passiveToApply;

    public override void OnBattleStart(BattleCharacter self)
    {
        bool passiveReplaced = false;
        foreach (var p in self.passives)
        {
            if (p == null) continue;
            if (p is CaesarPassive)
            {
                var caesarPassive = p as CaesarPassive;
                caesarPassive.attackBoostPercent = attackBoostPercent;
                caesarPassive.defenseBoostPercent = defenseBoostPercent;   
                caesarPassive.elementalResistanceBoostPercent = elementalResistanceBoostPercent;
                caesarPassive.elementalPowerBoostPercent = elementalPowerBoostPercent;
                passiveReplaced = true;
                break;
            }
        }
        if (!passiveReplaced) return;

        passiveToApply.betrayed = self;
    }

    public override void OnSkillUsedEnd(BattleCharacter self, BattleCharacter target, Skill skill)
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
                action.user.AddPassive(passiveToApply, self);
                Debug.Log($"Betrayal applied to {action.user.name}.");
            }

        }
    }

}
