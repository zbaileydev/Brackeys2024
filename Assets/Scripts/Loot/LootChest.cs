using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class LootChest : MonoBehaviour, ILootContainer
{
    public LootTable LootTable
    {
        get => lootTable;
        set => lootTable = value;
    }
    public bool Looted
    {
        get => looted;
        set => looted = value;
    }

    public GameObject LootPrefab;
    public LootTable lootTable;
    public bool looted;

    public void GetLoot()
    {
        if (looted)
        {
            Debug.Log("Already looted!");
            return;
        }

        // looted = true;

        // gives >= 0.8 most of the time, very difficult.
        // float random = 1 - Mathf.Pow(Random.value, 2);

        float random = Random.value;

        Loot[] items = lootTable.lootTable.ToList().FindAll(x => x.chance >= random).OrderBy(x => x.chance).Select(x => x.item).ToArray();
        Loot selectedLoot;
        int choice = Random.Range(0, items.Length);
        //Debug.Log(items.Length);
        if (items.Length == 0)
        {
            Debug.Log("Returned Nothing!");
            // return lootTable.defaultItem.item;

            return;
        }
        else
            selectedLoot = items[choice];

        GameObject lootObject = Instantiate(LootPrefab, transform.position, Quaternion.identity);
        lootObject.AddComponent<LootItem>();

        // Loot.LootType type = Loot.LootType.Consumable;
        // else if ((selectedLoot as ).Name != null)
        //     type = Loot.LootType.;
        // else if ((selectedLoot as ).Name != null)
        //     type = Loot.LootType.;
        // else if ((selectedLoot as ).Name != null)
        //     type = Loot.LootType.;
        // else
        // {
        //     Debug.LogError($);
        //     return null;
        // }

        // LootTool tool = selectedLoot as LootTool;
        lootObject.GetComponent<LootItem>().loot = selectedLoot;
        lootObject.GetComponent<LootItem>().Init();

        // switch (type)
        // {
        //     case Loot.LootType.Tool:
        //         {
        //             LootTool tool = selectedLoot as LootTool;
        //             lootObject.GetComponent<LootItem>().loot = tool;
        //             lootObject.GetComponent<LootItem>().Init();
        //             break;
        //         }
        //     case Loot.LootType.Consumable:
        //         {
        //             LootConsumable consumable = selectedLoot as LootConsumable;
        //             lootObject.GetComponent<LootItem>().loot = consumable;
        //             lootObject.GetComponent<LootItem>().Init();
        //             break;
        //         }
        //     case Loot.LootType.Weapon:
        //         {
        //             lootObject.GetComponent<LootItem>().loot = selectedLoot as LootWeapon;
        //             lootObject.GetComponent<LootItem>().Init();
        //             break;
        //         }
        //     case Loot.LootType.Modifier:
        //         {
        //             LootModifier modifier = selectedLoot as LootModifier;
        //             lootObject.GetComponent<LootItem>().loot = modifier;
        //             lootObject.GetComponent<LootItem>().Init();
        //             break;
        //         }
        //     default:
        //         break;
        // }

        // Destroy(gameObject);
    }
}