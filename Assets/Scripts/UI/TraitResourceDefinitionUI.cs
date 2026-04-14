using Unity.Properties;
using UnityEngine;

public abstract class TraitResourceDefinitionUI : MonoBehaviour
{
    public CharacterTrait trait;

    public abstract void Initialize(BattleCharacter character);

    public abstract void UpdateUI(BattleCharacter character);
}
