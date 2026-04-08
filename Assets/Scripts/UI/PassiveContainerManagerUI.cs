using Unity.VisualScripting;
using UnityEngine;

public class PassiveContainerManagerUI : MonoBehaviour
{
    public PassiveIconPrefab passiveIconPrefab;
    public Transform containerParent;

    public void UpdatePassives(PassivesDefinition[] passives)
    {
        // Clear existing icons
        foreach (Transform child in containerParent)
        {
            Destroy(child.gameObject);
        }

        // Create new icons for each passive
        foreach (var passive in passives)
        {
            if(passive == null) continue; // Skip null passives
            if(passive.icon == null) continue; // Skip passives without icons
            var iconInstance = Instantiate(passiveIconPrefab, containerParent);
            iconInstance.SetData(passive);
        }
    }

    public void UpdatePassives(PassivesDefinition[] passives, BattleCharacter character)
    {
        // Clear existing icons
        foreach (Transform child in containerParent)
        {
            Destroy(child.gameObject);
        }

        // Create new icons for each passive
        foreach (var passive in passives)
        {
            if(passive == null) continue; // Skip null passives
            if(passive.icon == null) continue; // Skip passives without icons
            var iconInstance = Instantiate(passiveIconPrefab, containerParent);
            iconInstance.SetData(passive, character);
        }
    }
}
