using UnityEngine;

public enum ModifierType { Damage, MovementSpeed, Health, Crit, WeaponSize, Knockback }

[System.Serializable]
public struct ModifierItem
{
    public ModifierType type;
    public float value;
}

[CreateAssetMenu(fileName = "Modifier", menuName = "Items/Modifier")]
public class LootModifier : Loot
{
    [HideInInspector]
    public new LootType Type { get => LootType.Modifier; }
    public new string name = " ";
    public Sprite sprite;
    public ModifierItem modifier;
}