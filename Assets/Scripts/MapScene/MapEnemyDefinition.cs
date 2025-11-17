using UnityEngine;

[System.Serializable]
public class MapEnemyDefinition
{
    public GameObject enemyPrefab;

    [Header("Stats")]
    public int health = 50;
    public int speed = 10;
    
    public Skill[] skills;
}