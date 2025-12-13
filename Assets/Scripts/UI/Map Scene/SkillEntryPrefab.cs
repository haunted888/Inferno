using UnityEngine;
using TMPro;

public class SkillEntryPrefab : MonoBehaviour
{
    public TMP_Text skillNameText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void SetSkill(Skill skill)
    {
        if (skillNameText != null)
            skillNameText.text = skill.skillName;
    }
}
