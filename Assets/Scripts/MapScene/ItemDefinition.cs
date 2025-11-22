using UnityEngine;

[CreateAssetMenu(menuName = "Items/Item Definition")]
public class ItemDefinition : ScriptableObject
{
    public string displayName = "Item";
    [TextArea] public string description;
    public Sprite icon;
}
