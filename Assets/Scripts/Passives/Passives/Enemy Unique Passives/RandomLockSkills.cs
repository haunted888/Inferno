using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Passives/Debuffs/Lock Skills/Random Lock Skills")]
public class RandomLockSkills : PassivesDefinition
{
    [Header("Number to Lock")]
    public int numSkillsToLock = 1; // Number of skills to lock
    private readonly List<Skill> lockedSkills = new List<Skill>();

    public override void OnCreated(BattleCharacter self)
    {

        // Get the list of skills that can be locked
        var lockableSkills = new List<Skill>(self.Skills);

        // If there are fewer lockable skills than the number to lock, adjust the number to lock
        int skillsToLockCount = Mathf.Min(numSkillsToLock, lockableSkills.Count);

        // Randomly select skills to lock
        for (int i = 0; i < skillsToLockCount; i++)
        {
            int randomIndex = Random.Range(0, lockableSkills.Count);
            Skill skillToLock = lockableSkills[randomIndex];
            lockedSkills.Add(skillToLock);
            skillToLock.skillDisabledCounter++;

            // Remove the locked skill from the list to avoid locking it again
            lockableSkills.RemoveAt(randomIndex);
        }
    }

    public override void OnDestroyed(BattleCharacter self)
    {
        // Unlock all skills when the passive is destroyed
        foreach (var skill in lockedSkills)
        {
            skill.skillDisabledCounter--;
            if(skill.skillDisabledCounter < 0)
            {
                skill.skillDisabledCounter = 0; // Ensure the counter doesn't go below zero
            }
        }
    }

    public override string GetDescription(BattleCharacter character)
    {
        string skillNames = string.Join(", ", lockedSkills.ConvertAll(skill => skill.skillName));
        return $"The following skills are locked: {skillNames}";
    }


}
