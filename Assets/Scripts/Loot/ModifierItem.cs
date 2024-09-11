public enum ModifierType { Damage, MovementSpeed, Health, Crit, WeaponSize, Knockback }

public class ModifierItem
{
    public ModifierType Type { get; private set; }
    public float Value { get; private set; }

    public ModifierItem(ModifierType type, float value)
    {
        Type = type;
        Value = value;
    }
}
