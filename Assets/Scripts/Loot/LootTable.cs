using UnityEngine;

[CreateAssetMenu(fileName = "lootTable", menuName = "Loot Table")]
public class LootTable : ScriptableObject
{
    [System.Serializable]
    public struct Item
    {
        public string item;
        [Range(0, 1)]
        public float chance;
    }

    [Header("At least one item in the loot table must have a 1(100%) chance or the default item will be used")]
    public Item defaultItem;
    public Item[] lootTable;
}
