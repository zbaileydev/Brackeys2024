using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModifier : MonoBehaviour
{

    public void ApplyModifier(ModifierItem modifier)
    {
        Player player= GetComponent<Player>();
        switch (modifier.Type)
        {
            case ModifierType.Damage:
                player.baseDamage += modifier.Value;
                break;
            case ModifierType.MovementSpeed:
                player.baseMovementSpeed += modifier.Value;
                break;
            case ModifierType.Health:
                player.baseHealth += modifier.Value;
                break;
            case ModifierType.Crit:
                player.baseCritChance += modifier.Value;
                break;
            case ModifierType.WeaponSize:
                player.baseWeaponSize += modifier.Value;
                break;
            case ModifierType.Knockback:
                player.baseKnockbackChance += modifier.Value;
                break;
            default:
                Debug.LogWarning("Unknown modifier type: " + modifier.Type);
                break;
        }
    }

    
}
