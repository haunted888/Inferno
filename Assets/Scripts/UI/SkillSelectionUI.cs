using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillSelectionUI : MonoBehaviour
{
    public GameObject buttonPrefab;   // a UI Button prefab with a Text child
    public Transform buttonParent;    // where to spawn buttons (e.g. Vertical Layout Group)

    private Action<int> onSkillChosen;

    public void ShowForCharacter(BattleCharacter character, Action<int> callback)
    {
        gameObject.SetActive(true);
        onSkillChosen = callback;

        // Clear previous buttons
        foreach (Transform child in buttonParent)
            Destroy(child.gameObject);

        // Create a button per skill
        for (int i = 0; i < character.Skills.Count; i++)
        {
            Skill skill = character.Skills[i];
            if (skill == null) continue;

            GameObject buttonObj = Instantiate(buttonPrefab, buttonParent);
            Button btn = buttonObj.GetComponent<Button>();
            TMP_Text  text = buttonObj.GetComponentInChildren<TMP_Text>();

            int cost = skill.spCost;

            if (text != null)
            {
                text.text = cost > 0
                    ? $"{skill.skillName} (SP: {cost})"
                    : skill.skillName;
            }

            // Disable if not enough SP
            if (character.CurrentSp < cost)
                btn.interactable = false;

            int index = i; // capture for lambda
            btn.onClick.AddListener(() => OnButtonClicked(index));
        }

    }

    public void Hide()
    {
        gameObject.SetActive(false);
        onSkillChosen = null;
    }

    private void OnButtonClicked(int index)
    {
        onSkillChosen?.Invoke(index);
    }
}
