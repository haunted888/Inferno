using UnityEngine;
using System.Collections.Generic;

public class BattleCharacter : MonoBehaviour
{
    [Min(1)]
    public int slotSize = 1;   // how many slots this character “occupies”

    [Header("Health")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;

    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;
    public bool IsDead => currentHealth <= 0;

    [Header("Stats")]
    [SerializeField] private int speed = 10;
    public int Speed => speed;

    void Awake()
    {
        // Initialize current health at full
        currentHealth = Mathf.Max(1, maxHealth);
    }

    public int TakeDamage(int amount)
    {
        if (amount <= 0 || IsDead) return 0;

        int oldHealth = currentHealth;
        currentHealth = Mathf.Max(0, currentHealth - amount);
        int dealt = oldHealth - currentHealth;

        if (IsDead)
        {
            Debug.Log($"{name} died.");

            if (BattleTurnManager.Instance != null)
                BattleTurnManager.Instance.HandleCharacterDeath(this);
        }
        else
        {
            Debug.Log($"{name} took {dealt} damage. HP: {currentHealth}/{maxHealth}");
        }

        return dealt;
    }


    [Header("AI / Threat")]
    [SerializeField] private int threat;
    public int Threat => threat;

    public void AddThreat(int amount)
    {
        if (amount <= 0) return;
        threat += amount;
    }

    // Optional, for later if you want to reset completely
    public void ResetThreat()
    {
        threat = 0;
    }

    public void Heal(int amount)
    {
        if (amount <= 0 || IsDead) return;

        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        Debug.Log($"{name} healed {amount}. HP: {currentHealth}/{maxHealth}");
    }

    public void SetMaxHealth(int newMax, bool fillToMax = true)
    {
        maxHealth = Mathf.Max(1, newMax);
        if (fillToMax || currentHealth > maxHealth)
            currentHealth = maxHealth;
    }

    [Header("Skills")]
    [SerializeField] private List<Skill> skills = new List<Skill>();
    public IReadOnlyList<Skill> Skills => skills;

    public void UseSkill(int index, BattleCharacter target)
    {
        if (index < 0 || index >= skills.Count)
        {
            Debug.LogWarning($"{name} tried to use skill at index {index}, but it is out of range.");
            return;
        }

        Skill skill = skills[index];
        if (skill == null)
        {
            Debug.LogWarning($"{name} has a null skill at index {index}.");
            return;
        }

        skill.Execute(this, target);
    }
    public void ClearSkills() => skills.Clear();
    public void AddSkill(Skill s)
    {
        if (s != null) skills.Add(s);
    }
    
    public void SetStats(int health, int spd)
    {
        maxHealth = Mathf.Max(1, health);
        currentHealth = maxHealth;
        speed = Mathf.Max(1, spd);
    }
}
