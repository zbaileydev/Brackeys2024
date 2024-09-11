using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public Weapon currentWeapon;
    public Tool currentTool;
    public List<Consumable> inventory = new List<Consumable>();

    public void SwitchToWeapon(Weapon weapon)
    {
        currentWeapon = weapon;
    }

    public void SwitchToTool(Tool tool)
    {
        currentTool = tool;
    }

    public void AddToInventory(Consumable consumable)
    {
        inventory.Insert(0, consumable);
    }
}