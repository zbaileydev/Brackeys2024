using UnityEngine;

[CreateAssetMenu(fileName = "lootTable", menuName = "Loot Table")]
public partial class LootTable : ScriptableObject
{
    [System.Serializable]
    public partial struct Item
    {
        public Loot item;
        [Range(0, 1)]
        public float chance;
    }

    public Item[] lootTable;
}
// [System.Serializable]
// public struct Item
// {
//     public GameObject item;
//     [Range(0, 1)]
//     public float chance;
// }

// [Header("At least one item in the loot table must have a 1(100%) chance or the default item will be used")]
// public Item defaultItem;
// public Item[] lootTable;
