using UnityEngine;
using System.Collections.Generic;

public class MapCombatTransfer : MonoBehaviour
{
    public static MapCombatTransfer Instance { get; private set; }

    private List<MapEnemyDefinition> pendingEnemies = new List<MapEnemyDefinition>();

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetupEnemies(MapEnemyDefinition[] defs)
    {
        pendingEnemies.Clear();
        if (defs != null)
            pendingEnemies.AddRange(defs);
    }

    public List<MapEnemyDefinition> ConsumeEnemies()
    {
        var copy = new List<MapEnemyDefinition>(pendingEnemies);
        pendingEnemies.Clear();
        return copy;
    }
}
