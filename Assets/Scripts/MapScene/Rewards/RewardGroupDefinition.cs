using UnityEngine;

[System.Serializable]
public class MapRewardGroup
{
    [Tooltip("One of these will be chosen at random when this group is processed.")]
    public MapRewardDefinition[] options;
}

