using TMPro;
using UnityEngine;

public class MarksmanTraitResourceUI : TraitResourceDefinitionUI
{

    public TMP_Text AmmoText;

    public override void Initialize(BattleCharacter character)
    {
        UpdateUI(character);
    }

    public override void UpdateUI(BattleCharacter character)
    {
        if (AmmoText == null || character == null) return;

        AmmoText.text = $"{character.CurrentAmmo}/{character.MaxAmmo}";
    }
}

