using System;
using System.Collections.Generic;
using UnityEngine;

public class CalculatorSkillList : MonoBehaviour
{

    [NonSerialized]
    public Action<Skill> onSkillSelected;

    public Transform skillListContainer;
    public CalcSkillPrefab skillEntryPrefab;
    private MapEnemyDefinition activeEnemy;
    private MapPartyMemberDefinition activePartyMember;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void UpdateSkillList(List<Skill> skills, bool showSPCost = false)
    {
        foreach (Transform child in skillListContainer)
        {
            Destroy(child.gameObject);
        }
        foreach (Skill skill in skills)
        {
            CalcSkillPrefab skillEntry = Instantiate(skillEntryPrefab, skillListContainer);

            if (showSPCost)
                skillEntry.SetSkillWithSPCost(skill);
            else
                skillEntry.SetSkill(skill);

            var btn = skillEntry.GetComponentInChildren<UnityEngine.UI.Button>();
            if (btn != null)
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => onSkillSelected?.Invoke(skill));
            }
        }
    }

    public void SetActiveCharacter(MapEnemyDefinition enemy)
    {
        activeEnemy = enemy;
        activePartyMember = null;
    }

    public void SetActiveCharacter(MapPartyMemberDefinition partyMember)
    {
        activePartyMember = partyMember;
        activeEnemy = null;
    }

    public MapEnemyDefinition GetActiveEnemy() => activeEnemy;
    public MapPartyMemberDefinition GetActivePartyMember() => activePartyMember;
}
