using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public struct Weapon
{
    public LootWeapon stats;

    public Weapon(LootWeapon stats)
    {
        this.stats = stats;
    }

    public void SetStats(LootWeapon stats)
    {
        this.stats = stats;
    }
}
public struct Tool
{
    public LootTool stats;

    public Tool(LootTool stats)
    {
        this.stats = stats;
    }

    public void SetStats(LootTool stats)
    {
        this.stats = stats;
    }
}
public struct Consumable
{
    public LootConsumable stats;
    public Consumable(LootConsumable stats)
    {
        this.stats = stats;
    }
}

public class PlayerInventory : MonoBehaviour
{
    public enum InHand { Tool, Weapon }

    public InHand holding = InHand.Tool;
    public GameObject LootPrefab;
    public GameObject WeaponToolHolder;
    public Action OnWeaponChanged;

    public Weapon currentWeapon;
    public Tool currentTool;
    public List<Consumable> inventory = new();

    public bool HoldingTool { get => holding == InHand.Tool; }
    public bool HoldingWeapon { get => holding == InHand.Weapon; }

    void Start()
    {
        currentWeapon = new Weapon(null);
        currentTool = new Tool(null);
    }

    public void SwitchToWeapon() => SwitchWeapon(currentWeapon);
    public void SwitchToTool() => SwitchTool(currentTool);

    public void SwitchWeapon(Weapon weapon)
    {
        holding = InHand.Weapon;
        if (currentWeapon.stats != null)
            DisposeItem(currentWeapon);
        currentWeapon = weapon;
        WeaponToolHolder.GetComponent<SpriteRenderer>().sprite = currentWeapon.stats.sprite;
        ClearAnimations();
        if (currentWeapon.stats.attackAnimation != null)
            WeaponToolHolder.GetComponent<Animation>().AddClip(currentWeapon.stats.attackAnimation, "attackAnimation");

        OnWeaponChanged?.Invoke();
    }

    public void SwitchTool(Tool tool)
    {
        holding = InHand.Tool;
        if (currentTool.stats != null)
            DisposeItem(currentTool);
        currentTool = tool;
        WeaponToolHolder.GetComponent<SpriteRenderer>().sprite = currentTool.stats.sprite;

        ClearAnimations();
        if (currentTool.stats.attackAnimation != null)
            WeaponToolHolder.GetComponent<Animation>().AddClip(currentTool.stats.attackAnimation, "attackAnimation");
        if (currentTool.stats.useAnimation != null)
            WeaponToolHolder.GetComponent<Animation>().AddClip(currentTool.stats.useAnimation, "useAnimation");

        OnWeaponChanged?.Invoke();
    }

    public void AddToInventory(Consumable consumable)
    {
        inventory.Insert(0, consumable);
    }

    public void RemoveFromInventory(int index)
    {
        if (index < inventory.Count && inventory[index].stats != null)
            DisposeItem(inventory[index]);
        inventory.RemoveAt(index);
    }

    void DisposeItem(Weapon item)
    {
        GameObject lootObject = Instantiate(LootPrefab, transform.position, Quaternion.identity);
        lootObject.AddComponent<LootItem>();

        lootObject.GetComponent<LootItem>().loot = item.stats;
        lootObject.GetComponent<LootItem>().Init();
    }

    void DisposeItem(Tool item)
    {
        GameObject lootObject = Instantiate(LootPrefab, transform.position, Quaternion.identity);
        lootObject.AddComponent<LootItem>();

        lootObject.GetComponent<LootItem>().loot = item.stats;
        lootObject.GetComponent<LootItem>().Init();
    }

    void DisposeItem(Consumable item)
    {
        GameObject lootObject = Instantiate(LootPrefab, transform.position, Quaternion.identity);
        lootObject.AddComponent<LootItem>();

        lootObject.GetComponent<LootItem>().loot = item.stats;
        lootObject.GetComponent<LootItem>().Init();
    }

    void ClearAnimations()
    {
        foreach (AnimationClip animation in WeaponToolHolder.GetComponent<Animation>())
            WeaponToolHolder.GetComponent<Animation>().RemoveClip(animation);
    }
}