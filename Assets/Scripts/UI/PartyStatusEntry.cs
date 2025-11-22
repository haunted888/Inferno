using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartyStatusEntry : MonoBehaviour
{
    public TMP_Text nameLabel;
    public Slider hpSlider;
    public Slider spSlider;
    public TMP_Text hpText;
    public TMP_Text spText;

    private BattleCharacter character;

    public void Initialize(BattleCharacter chr)
    {
        character = chr;

        if (character == null)
        {
            gameObject.SetActive(false);
            return;
        }

        if (nameLabel != null)
            nameLabel.text = character.name;

        if (hpSlider != null)
        {
            hpSlider.minValue = 0;
            hpSlider.maxValue = character.MaxHealth;
            hpSlider.value = character.CurrentHealth;
        }

        if (spSlider != null)
        {
            spSlider.minValue = 0;
            spSlider.maxValue = character.MaxSp;
            spSlider.value = character.CurrentSp;
        }

        UpdateTexts();
    }

    void Update()
    {
        if (character == null) return;

        if (hpSlider != null)
            hpSlider.value = character.CurrentHealth;

        if (spSlider != null)
            spSlider.value = character.CurrentSp;

        UpdateTexts();
    }

    private void UpdateTexts()
    {
        if (hpText != null)
            hpText.text = $"{character.CurrentHealth}/{character.MaxHealth}";

        if (spText != null)
            spText.text = $"{character.CurrentSp}/{character.MaxSp}";
    }
}
