using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float Health;
    public GameObject[] Inventory;

    public float baseCritChance;
    public float baseMovementSpeed;
    public float baseHealth;
    public float baseDamage;
    public float baseKnockbackChance;
    public float baseWeaponSize;

    [HideInInspector]
    public List<ModifierItem> modifiers = new List<ModifierItem>();

    // Damage, MovementSpeed, Health, Crit, WeaponSize, Knockback

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ApplyModifier(ModifierItem modifier)
    {
        switch (modifier.type)
        {
            case ModifierType.Damage:
                baseDamage += modifier.value;
                break;
            case ModifierType.MovementSpeed:
                baseMovementSpeed += modifier.value;
                break;
            case ModifierType.Health:
                baseHealth += modifier.value;
                break;
            case ModifierType.Crit:
                baseCritChance += modifier.value;
                break;
            case ModifierType.WeaponSize:
                baseWeaponSize += modifier.value;
                break;
            case ModifierType.Knockback:
                baseKnockbackChance += modifier.value;
                break;
            default:
                Debug.LogWarning("Unknown modifier type: " + modifier.type);
                break;
        }
    }
}
