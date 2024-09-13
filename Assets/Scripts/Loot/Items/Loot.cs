using UnityEngine;

public partial class Loot : ScriptableObject
{
    public enum LootType
    {
        Tool, Weapon, Modifier, Consumable
    }

    public string name;
    public LootType Type { get; }
}