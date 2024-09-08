using System.Linq;
using UnityEngine;

public class LootChest : MonoBehaviour, ILootContainer
{
    public LootTable LootTable {
        get => lootTable;
        set => lootTable = value;
    }
    public bool Looted {
        get => looted;
        set => looted = value;
    }

    public LootTable lootTable;
    public bool looted;

    public string GetLoot()
    {
        if(looted)
            return string.Empty;

        // looted = true;

        // gives >= 0.8 most of the time, very hard.
        // float random = 1 - Mathf.Pow(Random.value, 2);
        float random = Random.value;
        Debug.Log(random.ToString());

        string[] items = lootTable.lootTable.ToList().FindAll(x => x.chance >= random).OrderBy(x => x.chance).Select(x => x.item).ToArray();
        Debug.Log(items.Length);
        if(items.Length == 0)
        {
            Debug.Log("Returned default item");
            return lootTable.defaultItem.item;
        }

        return items[0];
    }
}