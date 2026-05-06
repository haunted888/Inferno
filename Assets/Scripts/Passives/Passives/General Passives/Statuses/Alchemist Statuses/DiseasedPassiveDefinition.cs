using UnityEngine;

[CreateAssetMenu(menuName = "Passives/Statuses/Alchemist/Diseased Passive")]
public class DiseasedPassiveDefinition : StatusPassiveDefinition
{
    private const float diseasedStatDecrease = 0.05f;


    public override void GetStatBoosts(BattleCharacter self)
    {
        self.bonusStats.physicalAttack -= (int)(diseasedStatDecrease * counter * self.baseStats.physicalAttack);
        self.bonusStats.elementalPower -= (int)(diseasedStatDecrease * counter * self.baseStats.elementalPower);
        self.bonusStats.speed -= (int)(diseasedStatDecrease * counter * self.baseStats.speed);
    }

    public override void OnResolvePhaseEnd(BattleCharacter self)
    {
        if (self == null) return;

        counter++;
        SetDisplayText($"{self.name}'s disease counter has increased to {counter}. Their stats have decreased by {diseasedStatDecrease * counter * 100}%.");
        if(counter >= 20)
        {
            self.Die();
            SetDisplayText($"{self.name} has died from their disease.");
        }
    }

    public override void OnDestroyed(BattleCharacter self)
    {
        ApplyStatusBuffer(self);
    }
}
