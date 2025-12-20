using System.Collections.Generic;
using UnityEngine;

public class CampSkillListUI : MonoBehaviour
{
    public Transform skillListContainer;
    public GameObject skillEntryPrefab;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void UpdateSkillList(List<Skill> skills)
    {
        foreach (Transform child in skillListContainer)
        {
            Destroy(child.gameObject);
        }
        foreach (Skill skill in skills)
        {
            GameObject skillEntry = Instantiate(skillEntryPrefab, skillListContainer);
            skillEntry.GetComponent<SkillEntryPrefab>().SetSkill(skill);
        }
    }
}
