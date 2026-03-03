using UnityEngine;
using TMPro;

public class CalcSkillPrefab : MonoBehaviour
{
    public TMP_Text skillNameText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void SetSkill(Skill skill)
    {
        if (skillNameText != null)
            skillNameText.text = skill.skillName;
            
    }

    public void SetSkillWithSPCost(Skill skill)
    {
        if (skillNameText != null)
            skillNameText.text = $"{skill.skillName} (SP: {skill.spCost})";
    }
}
