using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TalentNodeUI : MonoBehaviour
{
    public TalentDefinition talent;
    public TMP_Text nameText;
    public TMP_Text costText;
    public Button button;

    private MapPartyMemberDefinition member;
    private System.Action<TalentDefinition> onToggle;

    public void Bind(MapPartyMemberDefinition m, System.Action<TalentDefinition> toggle)
    {
        member = m;
        onToggle = toggle;
        if (nameText) nameText.text = talent ? talent.displayName : "Talent";
        if (costText) costText.text = talent ? talent.cost.ToString() : "NULL";
        Refresh();
        if (button)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => onToggle?.Invoke(talent));
        }
    }

    public void Refresh()
    {
        if (button == null || member == null || talent == null) return;

        bool learned = member.HasTalent(talent.id);
        bool canBuy  = member.CanLearn(talent);

        // Disable once learned (no reallocation yet)
        button.interactable = !learned && canBuy;
    }

}
