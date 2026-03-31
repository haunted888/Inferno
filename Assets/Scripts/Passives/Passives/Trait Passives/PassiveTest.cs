using UnityEngine;

[CreateAssetMenu(menuName = "Passives/Trait Passives/Passive Test")]
public class PassiveTest : PassivesDefinition
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void OnCommandPhaseStart(BattleCharacter self)
    {
        Debug.Log($"{self.name}'s PassiveTest triggered OnCommandPhaseStart!");
    }
}
