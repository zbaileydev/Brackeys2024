using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Consumable", menuName = "Items/Consumable")]
public class LootConsumable : Loot
{
    [HideInInspector]
    public new LootType Type { get => LootType.Consumable; }
    public new string name = " ";
    public Sprite sprite;
    public float healthGain;
}
